using System;
using System.Collections.Generic;
using System.Linq;

namespace Dungeon.Core
{
    /// <summary>Synchronous command processor. Animation consumes result events and never controls rule timing.</summary>
    public sealed partial class Battle
    {
        private readonly GameCatalog catalog;
        private readonly IRandomSource random;
        private readonly Dictionary<string, IAiPolicy> policies;
        private readonly List<UnitState> units;
        private readonly Queue<UnitState> turns = new Queue<UnitState>();
        private readonly Dictionary<string, Intent> intents = new Dictionary<string, Intent>(StringComparer.Ordinal);
        private readonly List<GameEvent> events = new List<GameEvent>();
        private long sequence;
        private int operations, summonIndex;
        private bool resolvingDeaths, escaping, skipped;
        internal bool RewardsClaimed;
        internal RewardBundle RolledRewards;
        internal IReadOnlyList<UnitState> RewardUnits;
        public IReadOnlyList<UnitState> Units { get; }
        public UnitState Player { get; }
        public UnitState ActiveUnit { get; private set; }
        public BattlePhase Phase { get; private set; }
        public BattleOutcome Outcome { get; private set; }
        public int Round { get; private set; }
        public Exception Failure { get; private set; }
        public IReadOnlyList<Intent> Intents => Array.AsReadOnly(units.Where(x => intents.ContainsKey(x.Id)).Select(x => intents[x.Id]).ToArray());

        public Battle(GameCatalog catalog, IEnumerable<UnitState> combatants, IRandomSource random, IDictionary<string, IAiPolicy> aiPolicies = null)
        {
            this.catalog = catalog ?? throw new ArgumentNullException(nameof(catalog)); this.random = random ?? throw new ArgumentNullException(nameof(random));
            units = Data.List(combatants).OrderBy(x => x.Team).ToList(); Units = units.AsReadOnly();
            if (units.Count == 0 || units.Select(x => x.Id).Distinct().Count() != units.Count || units.Any(x => x.Catalog != catalog || x.InBattle || x.IsDead))
                throw new ArgumentException("Combatants must be distinct, alive, unowned units from this catalog.");
            if (units.Count(x => x.Control == Control.Player) != 1 || !units.Any(x => x.Team == Team.Enemy)) throw new ArgumentException("Battle requires one player and enemies.");
            Player = units.Single(x => x.Control == Control.Player);
            if (Player.Team != Team.Player) throw new ArgumentException("Player must belong to the player team.");
            if (units.GroupBy(x => x.Team).Any(g => g.Sum(x => x.Definition.Size) > catalog.Rules.TeamCapacity)) throw new ArgumentException("Team capacity exceeded.");
            policies = new Dictionary<string, IAiPolicy>(StringComparer.Ordinal) { ["random"] = new RandomAiPolicy() };
            if (aiPolicies != null) foreach (var pair in aiPolicies) policies[pair.Key] = pair.Value ?? throw new ArgumentException("Null AI policy.");
            foreach (var definition in catalog.Units.Values) EnsurePolicy(definition);
        }

        public CommandResult Start() => Run(() =>
        {
            if (Phase != BattlePhase.NotStarted) return CommandError.WrongPhase;
            if (units.Any(x => x.InBattle || x.IsDead)) return CommandError.Condition;
            foreach (var unit in units) { unit.InBattle = true; unit.DeathResolved = false; unit.ReceivedStartArmor = false; unit.Armor = 0; }
            Phase = BattlePhase.AwaitingAdvance;
            Emit(EventKind.BattleStarted);
            foreach (var unit in units.ToArray()) Raise(unit, Trigger.BattleStart, null);
            ResolveDeaths(); if (!CheckEnd()) BeginRound();
            return CommandError.None;
        });

        public CommandResult UseSkill(string actorId, string skillId, string targetId = null) => Run(() =>
        {
            var error = ValidatePlayer(actorId); if (error != CommandError.None) return error;
            var skill = Player.FindSkill(skillId); var target = Find(targetId);
            error = ValidateSkill(Player, skill, target, false);
            if (error != CommandError.None) return error;
            Cast(Player, skill, target, true, false);
            CheckEnd();
            return CommandError.None;
        });

        public CommandResult EndTurn(string actorId) => Run(() =>
        {
            var error = ValidatePlayer(actorId); if (error != CommandError.None) return error;
            FinishTurn(); return CommandError.None;
        });

        public CommandResult Escape(string actorId) => Run(() =>
        {
            var error = ValidatePlayer(actorId); if (error != CommandError.None) return error;
            if (Player.HasTrait(StatusTrait.NoEscape)) return CommandError.Condition;
            if (Player.Actions < catalog.Rules.EscapeCost) return CommandError.InsufficientActions;
            Player.Actions -= catalog.Rules.EscapeCost; escaping = true;
            Emit(EventKind.Actions, Player, Player, amount: Player.Actions);
            FinishTurn(); return CommandError.None;
        });

        /// <summary>Executes exactly one AI/disabled unit turn, then exposes the next turn. Never spins waiting for player input.</summary>
        public CommandResult Advance() => Run(() =>
        {
            if (Phase != BattlePhase.AwaitingAdvance || ActiveUnit == null) return CommandError.WrongPhase;
            var actor = ActiveUnit;
            if (!skipped && !actor.IsDead)
            {
                if (!intents.TryGetValue(actor.Id, out var intent) || ValidateSkill(actor, actor.FindSkill(intent.SkillId), Find(intent.TargetId), true) != CommandError.None)
                    intent = SelectIntent(actor);
                if (intent != null) intents[actor.Id] = intent;
                if (intent?.SkillId != null) Cast(actor, actor.FindSkill(intent.SkillId), Find(intent.TargetId), true, false);
            }
            if (!CheckEnd()) FinishTurn();
            return CommandError.None;
        });

        public CommandResult UseItem(string actorId, Inventory inventory, string itemId, string targetId = null) => Run(() =>
        {
            var error = ValidatePlayer(actorId); if (error != CommandError.None) return error;
            if (inventory == null || inventory.Catalog != catalog || !catalog.Items.TryGetValue(itemId ?? "", out var item) || inventory.Count(itemId) < 1)
                return CommandError.MissingItem;
            if (item.Scope == UseScope.Exploration) return CommandError.WrongScope;
            if (item.IsCurrency || item.Effects.Count == 0) return CommandError.Condition;
            var target = Find(targetId); if (!ValidTargets(Player, item.Effects, target)) return CommandError.InvalidTarget;
            if (item.Consumable) inventory.TryRemove(itemId, 1);
            Emit(EventKind.ItemUsed, Player, target, itemId);
            Apply(Player, target, item.Effects, 1, itemId, false, false);
            Raise(Player, Trigger.AfterItem, target); ResolveDeaths(); CheckEnd(); return CommandError.None;
        });

        private CommandError ValidatePlayer(string actorId)
        {
            if (Phase != BattlePhase.AwaitingPlayer) return CommandError.WrongPhase;
            return ActiveUnit == Player && actorId == Player.Id ? CommandError.None : CommandError.WrongActor;
        }
        private CommandError ValidateSkill(UnitState actor, SkillState skill, UnitState target, bool ai)
        {
            if (skill == null) return CommandError.UnknownSkill;
            var definition = catalog.Skills[skill.Id];
            if (actor.IsDead || actor.HasTrait(StatusTrait.SkipTurn)) return CommandError.Condition;
            if (definition.Kind == SkillKind.Passive) return CommandError.PassiveSkill;
            if (definition.Scope == UseScope.Exploration) return CommandError.WrongScope;
            if (skill.Cooldown > 0) return CommandError.Cooldown;
            if (skill == actor.Weapon && actor.HasTrait(StatusTrait.Disarm)) return CommandError.Condition;
            if (definition.RequiredStatus != null && actor.Stacks(definition.RequiredStatus) == 0) return CommandError.Condition;
            if (!ValidTargets(actor, definition.Effects, target)) return CommandError.InvalidTarget;
            if (!ai && actor.Actions < ActionCost(actor, definition)) return CommandError.InsufficientActions;
            int available = definition.Resource == Resource.Mana ? actor.Mana : actor.Health;
            if (!ai && available < actor.SkillResourceCost(skill.Id)) return CommandError.InsufficientResource;
            return CommandError.None;
        }
        private bool ValidTargets(UnitState source, IEnumerable<EffectDefinition> effects, UnitState target)
        {
            var selections = effects.ToArray();
            if (selections.Any(x => x.Target == Target.SelectedAlly) && (target == null || target.IsDead || target.Team != source.Team || !units.Contains(target))) return false;
            if (!selections.Any(x => x.Target == Target.SelectedEnemy)) return true;
            if (target == null || target.IsDead || target.Team == source.Team || !units.Contains(target)) return false;
            return !units.Any(x => x.Team != source.Team && !x.IsDead && x.HasTrait(StatusTrait.Taunt)) || target.HasTrait(StatusTrait.Taunt);
        }
        private int ActionCost(UnitState actor, SkillDefinition skill) => actor.SkillActionCost(skill.Id);

        private void BeginRound()
        {
            Round++; Emit(EventKind.RoundStarted, amount: Round); turns.Clear(); intents.Clear();
            foreach (var unit in units.Where(x => !x.IsDead).OrderBy(x => x.Team).ToArray())
            {
                int armor = ArmorGain(unit, Data.Int(unit.GetStat(Stat.Defense)));
                unit.Armor = unit.HasTrait(StatusTrait.RetainArmor) ? checked(unit.Armor + armor) : Math.Max(unit.Armor, armor);
                Emit(EventKind.Armor, unit, unit, amount: unit.Armor); turns.Enqueue(unit);
            }
            foreach (var unit in units.Where(x => !x.IsDead && x.Control == Control.Ai).ToArray()) intents[unit.Id] = SelectIntent(unit);
            NextTurn();
        }
        private void NextTurn()
        {
            Spend();
            while (turns.Count > 0)
            {
                var unit = turns.Dequeue(); if (unit.IsDead) continue;
                BeginTurn(unit); return;
            }
            foreach (var unit in units.Where(x => !x.IsDead).ToArray()) { Raise(unit, Trigger.RoundEnd, null); Decay(unit, TickPhase.RoundEnd); }
            ResolveDeaths(); if (CheckEnd()) return;
            if (escaping) { Finish(BattleOutcome.Escaped); return; }
            BeginRound();
        }
        private void BeginTurn(UnitState unit)
        {
            Spend(); ActiveUnit = unit;
            if (!unit.HasTrait(StatusTrait.RetainArmor)) unit.Armor = Math.Min(unit.Armor, ArmorGain(unit, Data.Int(unit.GetStat(Stat.Defense))));
            if (!unit.ReceivedStartArmor)
            { unit.Armor = checked(unit.Armor + ArmorGain(unit, Data.Int(unit.GetStat(Stat.StartArmor)))); unit.ReceivedStartArmor = true; }
            if (unit.Control == Control.Player) unit.Actions = catalog.Rules.ActionsPerTurn;
            Emit(EventKind.TurnStarted, unit, unit, amount: unit.Actions);
            Raise(unit, Trigger.TurnStart, null); ResolveDeaths();
            if (CheckEnd()) return;
            if (unit.IsDead) { NextTurn(); return; }
            skipped = unit.HasTrait(StatusTrait.SkipTurn);
            Phase = unit.Control == Control.Player && !skipped && !escaping ? BattlePhase.AwaitingPlayer : BattlePhase.AwaitingAdvance;
            if (unit.Control == Control.Player && escaping) skipped = true;
        }
        private void FinishTurn()
        {
            var unit = ActiveUnit;
            intents.Remove(unit.Id);
            if (!unit.IsDead)
            {
                Raise(unit, Trigger.TurnEnd, null);
                foreach (var skill in unit.AllSkills) skill.Cooldown = Math.Max(0, skill.Cooldown - 1);
                Decay(unit, TickPhase.TurnEnd);
            }
            Emit(EventKind.TurnEnded, unit, unit); ResolveDeaths(); if (CheckEnd()) return;
            if (!unit.IsDead && !escaping && ConsumeTrait(unit, StatusTrait.Haste)) BeginTurn(unit);
            else NextTurn();
        }
        private Intent SelectIntent(UnitState unit)
        {
            var living = units.Where(x => !x.IsDead).ToArray();
            var candidates = unit.AllSkills.Where(x => living.Any(target => ValidateSkill(unit, x, target, true) == CommandError.None)).Select(x => x.Id).ToArray();
            var choice = policies[unit.Definition.AiPolicyId].ChooseSkill(new AiContext(unit, Round, candidates, units.Where(x => x.Team == unit.Team && !x.IsDead)), random);
            if (choice != null && !candidates.Contains(choice)) throw new InvalidOperationException("AI selected an unavailable skill.");
            if (choice == null) return new Intent(unit.Id, null, null);
            var effects = catalog.Skills[choice].Effects;
            var targets = living.Where(x => ValidTargets(unit, effects, x)).ToArray();
            if (effects.Any(x => x.Target == Target.SelectedEnemy))
            {
                var marked = targets.Where(x => x.HasTrait(StatusTrait.Marked)).ToArray();
                if (marked.Length > 0) targets = marked;
            }
            if (!effects.Any(x => x.Target == Target.SelectedEnemy || x.Target == Target.SelectedAlly)) return new Intent(unit.Id, choice, null);
            return new Intent(unit.Id, choice, targets.Length == 0 ? null : targets[random.Next(targets.Length)].Id);
        }

        private void Cast(UnitState source, SkillState skill, UnitState target, bool payCost, bool counter, bool allowDead = false)
        {
            var definition = catalog.Skills[skill.Id];
            Spend();
            int cost = payCost ? source.SkillResourceCost(skill.Id) : 0;
            if (payCost && source.Control == Control.Player) source.Actions -= ActionCost(source, definition);
            Emit(EventKind.SkillUsed, source, target, skill.Id);
            Raise(source, Trigger.BeforeSkill, target);
            if (!source.IsDead || allowDead) Apply(source, target, definition.Effects, skill.Level, skill.Id, counter, allowDead, source.Rune(skill), skill);
            if (payCost)
            {
                skill.Cooldown = checked(skill.Cooldown + source.SkillCooldown(skill.Id));
                if (definition.Resource == Resource.Mana) source.Mana = Math.Max(0, source.Mana - cost);
                else source.Health = Math.Max(0, source.Health - cost);
            }
            Raise(source, Trigger.AfterSkill, target);
            ResolveDeaths();
        }

        private void Apply(UnitState source, UnitState other, IEnumerable<EffectDefinition> effects, int level, string origin, bool counter, bool allowDead, RuneDefinition rune = null, SkillState context = null)
        {
            context = context ?? source.FindSkill(origin) ?? source.Relics.FirstOrDefault(x => x.Id == origin);
            foreach (var effect in effects)
            {
                Spend(); if (source.IsDead && !allowDead) break;
                var targets = Targets(source, other, effect.Target);
                foreach (var target in targets)
                {
                    if (target.IsDead && !(allowDead && target == source)) continue;
                    if (effect.Condition != null && !effect.Condition.Matches(source, target, context)) continue;
                    int amount = Data.Int(effect.Amount.Evaluate(level, source.GetStat));
                    if (effect.Kind == EffectKind.Damage && rune != null) amount = Data.Int((amount + rune.DamageFlat) * rune.DamageMultiplier);
                    if (effect.Kind == EffectKind.AddStatus && rune != null) amount = checked(amount + rune.StatusStacks);
                    if (effect.Kind != EffectKind.ChangeStat && effect.Kind != EffectKind.ChangeCounter) amount = Math.Max(0, amount);
                    switch (effect.Kind)
                    {
                        case EffectKind.Damage:
                            for (int hit = 0; hit < Math.Max(1, checked(effect.Hits + (rune?.ExtraHits ?? 0))) && !target.IsDead && (!source.IsDead || allowDead); hit++) Damage(source, target, effect, amount, origin, counter, allowDead);
                            break;
                        case EffectKind.Armor:
                            target.Armor = checked(target.Armor + ArmorGain(target, amount)); Emit(EventKind.Armor, source, target, origin, target.Armor); break;
                        case EffectKind.Heal:
                            target.Health = (int)Math.Min(target.MaxHealth, (long)target.Health + amount); Emit(EventKind.Health, source, target, origin, target.Health); break;
                        case EffectKind.Mana:
                            target.Mana = (int)Math.Min(target.MaxMana, (long)target.Mana + amount); Emit(EventKind.Mana, source, target, origin, target.Mana); break;
                        case EffectKind.AddStatus: ChangeStatus(target, effect.ReferenceId, amount, false, source); break;
                        case EffectKind.RemoveStatus: ChangeStatus(target, effect.ReferenceId, amount, true, source); break;
                        case EffectKind.Actions:
                            target.Actions = checked(target.Actions + amount); Emit(EventKind.Actions, source, target, origin, target.Actions); break;
                        case EffectKind.ReduceCooldown:
                            var longest = target.AllSkills.OrderByDescending(x => x.Cooldown).FirstOrDefault();
                            if (longest != null) longest.Cooldown = Math.Max(0, longest.Cooldown - amount); break;
                        case EffectKind.ChangeStat:
                            target.BaseStats.TryGetValue(effect.Stat, out var stat); target.BaseStats[effect.Stat] = Data.Finite(stat + amount); target.ClampResources(); break;
                        case EffectKind.Summon: Summon(source, effect.ReferenceId); break;
                        case EffectKind.CopySummon: CopySummon(source, target); break;
                        case EffectKind.ChangeCounter:
                            if (context == null) throw new InvalidOperationException("Counter effects require a skill or rune context.");
                            context.MutableCounters.TryGetValue(effect.ReferenceId, out var previous); context.MutableCounters[effect.ReferenceId] = checked(previous + amount); break;
                        case EffectKind.Cleanse:
                            foreach (var status in target.Statuses.Where(x => catalog.Statuses[x.Id].IsDebuff).ToArray()) ChangeStatus(target, status.Id, amount, true, source); break;
                        case EffectKind.LoseMana:
                            target.Mana = Math.Max(0, target.Mana - amount); Emit(EventKind.Mana, source, target, origin, target.Mana); break;
                        case EffectKind.RemoveArmor:
                            target.Armor = Math.Max(0, target.Armor - amount); Emit(EventKind.Armor, source, target, origin, target.Armor); break;
                        case EffectKind.CastSkill:
                            var extra = source.FindSkill(effect.ReferenceId) ?? new SkillState(new SkillLoadout(effect.ReferenceId, Math.Min(level, catalog.Skills[effect.ReferenceId].MaxLevel)));
                            if (catalog.Skills[extra.Id].Kind != SkillKind.Passive && ValidTargets(source, catalog.Skills[extra.Id].Effects, target)) Cast(source, extra, target, false, counter, allowDead);
                            break;
                        default: throw new InvalidOperationException("Unsupported effect kind.");
                    }
                }
            }
        }
        private UnitState[] Targets(UnitState source, UnitState other, Target target)
        {
            switch (target)
            {
                case Target.Self: return new[] { source };
                case Target.SelectedEnemy: return other != null && other.Team != source.Team && units.Contains(other) ? new[] { other } : Array.Empty<UnitState>();
                case Target.SelectedAlly: return other != null && other.Team == source.Team && units.Contains(other) ? new[] { other } : Array.Empty<UnitState>();
                case Target.EventOther: return other == null ? Array.Empty<UnitState>() : new[] { other };
                case Target.AllEnemies: return units.Where(x => x.Team != source.Team && !x.IsDead).ToArray();
                case Target.AllAllies: return units.Where(x => x.Team == source.Team && !x.IsDead).ToArray();
                case Target.RandomEnemy:
                    var enemies = units.Where(x => x.Team != source.Team && !x.IsDead).ToArray();
                    return enemies.Length == 0 ? enemies : new[] { enemies[random.Next(enemies.Length)] };
                case Target.LowestHealthAlly:
                    return units.Where(x => x.Team == source.Team && !x.IsDead).OrderBy(x => (double)x.Health / x.MaxHealth).Take(1).ToArray();
                default: throw new ArgumentOutOfRangeException(nameof(target));
            }
        }
        private void Damage(UnitState source, UnitState target, EffectDefinition effect, int amount, string origin, bool counter, bool allowDead)
        {
            Spend();
            if (effect.IsAttack) { Raise(source, Trigger.BeforeAttack, target); Raise(target, Trigger.BeforeHit, source); }
            Raise(source, Trigger.BeforeDamage, target);
            if ((source.IsDead && !allowDead) || target.IsDead) { ResolveDeaths(); return; }
            if (counter) amount = Math.Max(0, Data.Int(amount * source.GetStat(Stat.CounterMultiplier)));
            if (!effect.TrueDamage)
            {
                amount = Math.Max(0, Data.Int((amount + source.GetStat(Stat.DamageFlat) + target.GetStat(Stat.TakenFlat) + Math.Max(0, -target.GetStat(Stat.Defense))) *
                    (1 + source.GetStat(Stat.DamagePercent) + target.GetStat(Stat.TakenPercent))));
                if (effect.IsAttack)
                    foreach (var frost in source.Statuses.Where(x => (catalog.Statuses[x.Id].Traits & StatusTrait.Frost) != 0).ToArray())
                    { int reduction = Math.Min(amount, frost.Stacks); amount -= reduction; ChangeStatus(source, frost.Id, reduction, true, source); }
            }
            int absorbed = effect.TrueDamage ? 0 : Math.Min(target.Armor, amount);
            int healthDamage = Math.Min(target.Health, amount - absorbed);
            target.Armor -= absorbed; target.Health -= healthDamage;
            bool killedByThisHit = healthDamage > 0 && target.IsDead;
            Emit(EventKind.Damage, source, target, origin, healthDamage, absorbed);
            if (healthDamage + absorbed > 0)
            {
                foreach (var status in target.Statuses.Where(x => (catalog.Statuses[x.Id].Traits & StatusTrait.BreakOnDamage) != 0).ToArray())
                    ChangeStatus(target, status.Id, status.Stacks, true, target);
                Raise(target, Trigger.AfterDamage, source);
                Raise(source, Trigger.AfterDealDamage, target);
            }
            if (effect.IsAttack)
            {
                Raise(target, Trigger.AfterHit, source);
                Raise(source, Trigger.AfterAttack, target);
            }
            if (killedByThisHit) Raise(source, Trigger.Kill, target);
            ResolveDeaths();
            if (effect.IsAttack && !counter && !source.IsDead && !target.IsDead && target.Weapon != null && target.HasTrait(StatusTrait.Counter))
            {
                ConsumeTrait(target, StatusTrait.Counter);
                Cast(target, target.Weapon, source, false, true);
                Raise(target, Trigger.AfterCounter, source);
            }
        }
        private int ArmorGain(UnitState unit, int amount) => amount <= 0 ? 0 : Math.Max(0, Data.Int((amount + unit.GetStat(Stat.ArmorFlat)) * (1 + unit.GetStat(Stat.ArmorPercent))));
        private void ChangeStatus(UnitState unit, string id, int amount, bool remove, UnitState source)
        { unit.SetStatus(id, amount, remove); Emit(EventKind.StatusChanged, source, unit, id, unit.Stacks(id)); }
        private bool ConsumeTrait(UnitState unit, StatusTrait trait)
        {
            var status = unit.Statuses.FirstOrDefault(x => (catalog.Statuses[x.Id].Traits & trait) != 0);
            if (status == null) return false;
            ChangeStatus(unit, status.Id, 1, true, unit); return true;
        }
        private void Decay(UnitState unit, TickPhase phase)
        {
            foreach (var status in unit.Statuses.Where(x => catalog.Statuses[x.Id].Decay == phase).ToArray()) ChangeStatus(unit, status.Id, 1, true, unit);
        }
        private void Raise(UnitState unit, Trigger when, UnitState other)
        {
            Spend(); if (unit.IsDead && when != Trigger.SelfDeath) return;
            foreach (var status in unit.Statuses.ToArray())
            {
                if (!unit.MutableStatuses.Contains(status)) continue;
                foreach (var trigger in catalog.Statuses[status.Id].Triggers.Where(x => x.When == when))
                {
                    if (!unit.MutableStatuses.Contains(status)) break;
                    int level = status.Stacks;
                    if (trigger.ConsumeStacks > level) continue;
                    if (trigger.ConsumeStacks > 0) ChangeStatus(unit, status.Id, trigger.ConsumeStacks, true, unit);
                    Apply(unit, other, trigger.Effects, level, status.Id, false, when == Trigger.SelfDeath);
                }
            }
            foreach (var skill in unit.Relics.Concat(unit.AllSkills).ToArray())
            {
                foreach (var trigger in catalog.Skills[skill.Id].Triggers.Where(x => x.When == when))
                    Apply(unit, other, trigger.Effects, skill.Level, skill.Id, false, when == Trigger.SelfDeath, context: skill);
                var rune = unit.Rune(skill);
                if (rune != null) foreach (var trigger in rune.Triggers.Where(x => x.When == when))
                    Apply(unit, other, trigger.Effects, skill.Level, rune.Id, false, when == Trigger.SelfDeath, context: skill);
            }
        }
        private void ResolveDeaths()
        {
            if (resolvingDeaths) return;
            resolvingDeaths = true;
            try
            {
                while (true)
                {
                    var dead = units.FirstOrDefault(x => x.IsDead && !x.DeathResolved); if (dead == null) break;
                    Spend(); dead.DeathResolved = true; Raise(dead, Trigger.SelfDeath, null);
                    if (!dead.IsDead) { dead.DeathResolved = false; Emit(EventKind.UnitRevived, dead, dead); continue; }
                    Emit(EventKind.UnitDied, dead, dead);
                    foreach (var survivor in units.Where(x => !x.IsDead).ToArray()) Raise(survivor, Trigger.OtherDeath, dead);
                }
            }
            finally { resolvingDeaths = false; }
        }
        private bool CheckEnd()
        {
            if (Phase == BattlePhase.Finished) return true;
            if (Player.IsDead) { Finish(BattleOutcome.Defeat); return true; }
            if (!units.Any(x => x.Team == Team.Enemy && !x.IsDead)) { Finish(BattleOutcome.Victory); return true; }
            return false;
        }
        private void Finish(BattleOutcome outcome)
        {
            if (Phase == BattlePhase.Finished) return;
            Phase = BattlePhase.Finished; Outcome = outcome; ActiveUnit = null;
            foreach (var unit in units.Where(x => !x.IsDead).ToArray())
            {
                Raise(unit, Trigger.BattleEnd, null);
                if (!unit.IsDead)
                {
                    unit.Health = (int)Math.Min(unit.MaxHealth, (long)unit.Health + Math.Max(0, Data.Int(unit.GetStat(Stat.HealthRecovery))));
                    unit.Mana = (int)Math.Min(unit.MaxMana, (long)unit.Mana + Math.Max(0, Data.Int(unit.GetStat(Stat.ManaRecovery))));
                    int reduction = Math.Max(0, Data.Int(unit.GetStat(Stat.Intelligence) / catalog.Rules.IntelligencePerCooldown));
                    if (unit == Player && outcome == BattleOutcome.Victory) reduction += catalog.Rules.VictoryCooldownReduction;
                    foreach (var skill in unit.AllSkills) skill.Cooldown = Math.Max(0, skill.Cooldown - reduction);
                }
                foreach (var status in unit.Statuses.Where(x => catalog.Statuses[x.Id].RemoveAfterBattle).ToArray()) ChangeStatus(unit, status.Id, status.Stacks, true, unit);
            }
            ResolveDeaths(); if (Player.IsDead) Outcome = BattleOutcome.Defeat;
            foreach (var unit in units) { unit.Armor = 0; unit.Actions = 0; unit.InBattle = false; }
            intents.Clear(); turns.Clear(); Emit(EventKind.BattleEnded, amount: (int)Outcome);
        }
        private void Summon(UnitState source, string id)
        {
            var definition = catalog.Units[id]; EnsurePolicy(definition);
            if (units.Where(x => x.Team == source.Team && !x.IsDead).Sum(x => x.Definition.Size) + definition.Size > catalog.Rules.TeamCapacity) return;
            string instanceId; do { instanceId = source.Id + "/summon/" + (++summonIndex); } while (Find(instanceId) != null);
            var unit = catalog.CreateUnit(id, instanceId, source.Team); unit.IsSummoned = true; unit.InBattle = true;
            units.Add(unit); Emit(EventKind.Summoned, source, unit, id); Raise(unit, Trigger.BattleStart, source);
            // Summoned units join the next global round, never the in-progress queue.
        }
        private void EnsurePolicy(UnitDefinition definition)
        { if (!policies.ContainsKey(definition.AiPolicyId)) throw new ArgumentException("Unregistered AI policy: " + definition.AiPolicyId); }
        private void CopySummon(UnitState source, UnitState template)
        {
            EnsurePolicy(template.Definition);
            if (units.Where(x => x.Team == source.Team && !x.IsDead).Sum(x => x.Definition.Size) + template.Definition.Size > catalog.Rules.TeamCapacity) return;
            string id; do { id = source.Id + "/copy/" + (++summonIndex); } while (Find(id) != null);
            var copy = catalog.CreateUnit(template.Definition.Id, id, source.Team);
            copy.BaseStats.Clear(); foreach (var pair in template.BaseStats) copy.BaseStats.Add(pair.Key, pair.Value);
            copy.MutableSkills.Clear();
            foreach (var skill in template.Skills) copy.MutableSkills.Add(new SkillState(new SkillLoadout(skill.Id, skill.Level, skill.Cooldown)) { RuneId = skill.RuneId });
            if (copy.Weapon != null && template.Weapon != null) { copy.Weapon.Level = template.Weapon.Level; copy.Weapon.RuneId = template.Weapon.RuneId; }
            copy.Health = copy.MaxHealth; copy.Mana = copy.MaxMana; copy.IsSummoned = true; copy.InBattle = true;
            units.Add(copy); Emit(EventKind.Summoned, source, copy, copy.Definition.Id); Raise(copy, Trigger.BattleStart, source);
        }
        private UnitState Find(string id) => id == null ? null : units.FirstOrDefault(x => x.Id == id);
        private sealed class ExecutionLimitException : Exception { }
        private void Spend() { if (++operations > catalog.Rules.EventLimit) throw new ExecutionLimitException(); }
        private CommandResult Run(Func<CommandError> command)
        {
            events.Clear(); operations = 0;
            try { var error = command(); return new CommandResult(error, events); }
            catch (Exception exception)
            {
                Failure = exception;
                Phase = BattlePhase.Faulted; events.Add(new GameEvent(++sequence, EventKind.Faulted, null, null, null, operations));
                return new CommandResult(exception is ExecutionLimitException ? CommandError.ExecutionLimit : CommandError.ExecutionFailed, events);
            }
        }
        private void Emit(EventKind kind, UnitState source = null, UnitState target = null, string definition = null, int amount = 0, int armor = 0)
        { Spend(); events.Add(new GameEvent(++sequence, kind, source?.Id, target?.Id, definition, amount, armor)); }
    }
}
