using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Dungeon.Core
{
    public enum Stat
    {
        MaxHealth, MaxMana, Strength, Agility, Intelligence, Attack, Defense,
        HealthRecovery, ManaRecovery, StartArmor,
        DamageFlat, DamagePercent, TakenFlat, TakenPercent, ArmorFlat, ArmorPercent,
        ActionCost, CounterMultiplier, KillCoins
    }
    public enum Team { Player, Enemy }
    public enum Control { Player, Ai }
    public enum SkillKind { Weapon, Magic, Passive, Martial }
    public enum Resource { Mana, Health }
    public enum UseScope { Any, Battle, Exploration }
    public enum Target { Self, SelectedEnemy, AllEnemies, AllAllies, EventOther, SelectedAlly, RandomEnemy, LowestHealthAlly }
    public enum EffectKind { Damage, Armor, Heal, Mana, AddStatus, RemoveStatus, Actions, ReduceCooldown, ChangeStat, Summon, Cleanse, LoseMana, RemoveArmor, CastSkill, CopySummon, ChangeCounter }
    public enum Trigger { BattleStart, BattleEnd, TurnStart, TurnEnd, RoundEnd, BeforeSkill, AfterSkill, AfterAttack, AfterHit, AfterDamage, Kill, SelfDeath, OtherDeath, AfterItem, Equipped, Unequipped, EnterRegion, BeforeAttack, BeforeDamage, BeforeHit, AfterCounter, AfterDealDamage }
    public enum StackMode { Add, Maximum, Replace }
    public enum TickPhase { None, TurnEnd, RoundEnd }
    [Flags]
    public enum StatusTrait { None = 0, RetainArmor = 1, SkipTurn = 2, Disarm = 4, NoEscape = 8, Taunt = 16, Counter = 32, Haste = 64, Frost = 128, BreakOnDamage = 256, Marked = 512 }

    internal static class Data
    {
        internal static string Id(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("An ID must not be empty.");
            return value;
        }
        internal static double Finite(double value)
        {
            if (double.IsNaN(value) || double.IsInfinity(value)) throw new ArgumentOutOfRangeException(nameof(value));
            return value;
        }
        internal static int NonNegative(int value)
        {
            if (value < 0) throw new ArgumentOutOfRangeException(nameof(value));
            return value;
        }
        internal static T EnumValue<T>(T value) where T : struct
        {
            if (!Enum.IsDefined(typeof(T), value)) throw new ArgumentOutOfRangeException(nameof(value));
            return value;
        }
        internal static IReadOnlyList<T> List<T>(IEnumerable<T> values)
        {
            var array = (values ?? Enumerable.Empty<T>()).ToArray();
            if (array.Any(x => x == null)) throw new ArgumentException("Null entries are not allowed.");
            return Array.AsReadOnly(array);
        }
        internal static int Int(double value) => checked((int)Math.Truncate(Math.Round(Finite(value), 3)));
    }

    /// <summary>Value at level L: constant + growth*L + sourceStat*(coefficient + coefficientGrowth*L).</summary>
    public readonly struct Magnitude
    {
        public double Constant { get; }
        public double Growth { get; }
        public Stat? SourceStat { get; }
        public double Coefficient { get; }
        public double CoefficientGrowth { get; }
        public Magnitude(double constant, double growth = 0, Stat? sourceStat = null, double coefficient = 0, double coefficientGrowth = 0)
        {
            Constant = Data.Finite(constant); Growth = Data.Finite(growth); SourceStat = sourceStat;
            if (sourceStat.HasValue) Data.EnumValue(sourceStat.Value);
            Coefficient = Data.Finite(coefficient); CoefficientGrowth = Data.Finite(coefficientGrowth);
        }
        public double Evaluate(int level, Func<Stat, double> stats = null) => Constant + Growth * level +
            (SourceStat.HasValue ? (stats ?? throw new ArgumentNullException(nameof(stats)))(SourceStat.Value) * (Coefficient + CoefficientGrowth * level) : 0);
    }

    public sealed class EffectDefinition
    {
        public EffectKind Kind { get; }
        public Target Target { get; }
        public Magnitude Amount { get; }
        public string ReferenceId { get; }
        public Stat Stat { get; }
        public int Hits { get; }
        public bool TrueDamage { get; }
        public bool IsAttack { get; }
        public EffectCondition Condition { get; }
        public EffectDefinition(EffectKind kind, Target target, Magnitude amount, string referenceId = null,
            Stat stat = Stat.Attack, int hits = 1, bool trueDamage = false, bool isAttack = false, EffectCondition condition = null)
        {
            if (hits < 1) throw new ArgumentOutOfRangeException(nameof(hits));
            Kind = Data.EnumValue(kind); Target = Data.EnumValue(target); Amount = amount; ReferenceId = referenceId; Stat = Data.EnumValue(stat);
            Hits = hits; TrueDamage = trueDamage; IsAttack = isAttack; Condition = condition;
            if (kind == EffectKind.AddStatus || kind == EffectKind.RemoveStatus || kind == EffectKind.Summon) Data.Id(referenceId);
            if (kind == EffectKind.Summon && target != Target.Self) throw new ArgumentException("Summoning uses the caster's team.");
            if (kind == EffectKind.CastSkill) Data.Id(referenceId);
            if (kind == EffectKind.ChangeCounter) Data.Id(referenceId);
        }
    }

    public sealed class TriggerDefinition
    {
        public Trigger When { get; }
        public IReadOnlyList<EffectDefinition> Effects { get; }
        public int ConsumeStacks { get; }
        public TriggerDefinition(Trigger when, IEnumerable<EffectDefinition> effects, int consumeStacks = 0)
        { When = Data.EnumValue(when); Effects = Data.List(effects); ConsumeStacks = Data.NonNegative(consumeStacks); }
    }
    public sealed class StatModifier
    {
        public Stat Stat { get; }
        public Magnitude Amount { get; }
        public StatModifier(Stat stat, Magnitude amount)
        {
            if (amount.SourceStat.HasValue) throw new ArgumentException("Status modifiers cannot recursively reference stats.");
            Stat = Data.EnumValue(stat); Amount = amount;
        }
    }
    public sealed class StatusDefinition
    {
        public string Id { get; }
        public StackMode Stacking { get; }
        public int MaxStacks { get; }
        public bool RemoveAfterBattle { get; }
        public bool IsDebuff { get; }
        public TickPhase Decay { get; }
        public StatusTrait Traits { get; }
        public IReadOnlyList<StatModifier> Modifiers { get; }
        public IReadOnlyList<TriggerDefinition> Triggers { get; }
        public StatusDefinition(string id, StackMode stacking = StackMode.Add, int maxStacks = int.MaxValue,
            bool removeAfterBattle = true, bool isDebuff = false, TickPhase decay = TickPhase.None,
            StatusTrait traits = StatusTrait.None, IEnumerable<StatModifier> modifiers = null, IEnumerable<TriggerDefinition> triggers = null)
        {
            if (maxStacks < 1) throw new ArgumentOutOfRangeException(nameof(maxStacks));
            if (((int)traits & ~1023) != 0) throw new ArgumentOutOfRangeException(nameof(traits));
            Id = Data.Id(id); Stacking = Data.EnumValue(stacking); MaxStacks = maxStacks; RemoveAfterBattle = removeAfterBattle;
            IsDebuff = isDebuff; Decay = Data.EnumValue(decay); Traits = traits; Modifiers = Data.List(modifiers); Triggers = Data.List(triggers);
        }
    }
    public sealed class SkillDefinition
    {
        public string Id { get; }
        public SkillKind Kind { get; }
        public int MaxLevel { get; }
        public int Rank { get; }
        public Resource Resource { get; }
        public Magnitude Cost { get; }
        public Magnitude Cooldown { get; }
        public int ActionCost { get; }
        public UseScope Scope { get; }
        public string RequiredStatus { get; }
        public IReadOnlyList<EffectDefinition> Effects { get; }
        public IReadOnlyList<TriggerDefinition> Triggers { get; }
        public IReadOnlyList<string> Tags { get; }
        public SkillDefinition(string id, SkillKind kind, int maxLevel, Magnitude cost, Magnitude cooldown,
            int actionCost, IEnumerable<EffectDefinition> effects = null, IEnumerable<TriggerDefinition> triggers = null,
            Resource resource = Resource.Mana, UseScope scope = UseScope.Battle, int rank = 0,
            string requiredStatus = null, IEnumerable<string> tags = null)
        {
            if (maxLevel < 1) throw new ArgumentOutOfRangeException(nameof(maxLevel));
            Id = Data.Id(id); Kind = Data.EnumValue(kind); MaxLevel = maxLevel; Cost = cost; Cooldown = cooldown;
            ActionCost = Data.NonNegative(actionCost); Effects = Data.List(effects); Triggers = Data.List(triggers);
            Resource = Data.EnumValue(resource); Scope = Data.EnumValue(scope); Rank = Data.NonNegative(rank); RequiredStatus = requiredStatus; Tags = Data.List(tags);
            if (Effects.Any(x => x.Target == Target.EventOther) || Triggers.Any(x => x.ConsumeStacks != 0)) throw new ArgumentException("EventOther and stack consumption require a status trigger context.");
        }
    }
    public sealed class SkillLoadout
    {
        public string SkillId { get; }
        public int Level { get; }
        public int InitialCooldown { get; }
        public SkillLoadout(string skillId, int level = 1, int initialCooldown = 0)
        {
            if (level < 1) throw new ArgumentOutOfRangeException(nameof(level));
            SkillId = Data.Id(skillId); Level = level; InitialCooldown = Data.NonNegative(initialCooldown);
        }
    }
    public sealed class UnitDefinition
    {
        public string Id { get; }
        public IReadOnlyDictionary<Stat, double> Stats { get; }
        public SkillLoadout Weapon { get; }
        public IReadOnlyList<SkillLoadout> Skills { get; }
        public bool DeriveAttributes { get; }
        public string AiPolicyId { get; }
        public int Size { get; }
        public int Rank { get; }
        public int Coins { get; }
        public UnitDefinition(string id, IDictionary<Stat, double> stats, SkillLoadout weapon,
            IEnumerable<SkillLoadout> skills = null, bool deriveAttributes = false, string aiPolicyId = "random", int size = 1, int rank = 0, int coins = 0)
        {
            if (size < 1) throw new ArgumentOutOfRangeException(nameof(size));
            Id = Data.Id(id); var copy = new Dictionary<Stat, double>(stats ?? throw new ArgumentNullException(nameof(stats)));
            foreach (var entry in copy) { Data.EnumValue(entry.Key); Data.Finite(entry.Value); }
            if (!copy.TryGetValue(Stat.MaxHealth, out var hp) || hp <= 0) throw new ArgumentException("A unit needs positive base health.");
            Stats = new ReadOnlyDictionary<Stat, double>(copy); Weapon = weapon; Skills = Data.List(skills);
            DeriveAttributes = deriveAttributes; AiPolicyId = Data.Id(aiPolicyId); Size = size; Rank = Data.NonNegative(rank); Coins = Data.NonNegative(coins);
        }
    }
    public sealed class ItemDefinition
    {
        public string Id { get; }
        public int Price { get; }
        public bool IsCurrency { get; }
        public UseScope Scope { get; }
        public IReadOnlyList<EffectDefinition> Effects { get; }
        public bool Consumable { get; }
        public ItemUpgrade Upgrade { get; }
        public string RuneId { get; }
        public ItemDefinition(string id, int price, IEnumerable<EffectDefinition> effects = null, UseScope scope = UseScope.Any, bool isCurrency = false, bool consumable = true, ItemUpgrade upgrade = null, string runeId = null)
        {
            Id = Data.Id(id); Price = Data.NonNegative(price); Effects = Data.List(effects); Scope = Data.EnumValue(scope); IsCurrency = isCurrency; Consumable = consumable; Upgrade = upgrade;
            RuneId = runeId; if (runeId != null) Data.Id(runeId);
            if ((upgrade != null || runeId != null) && (Effects.Count > 0 || isCurrency || (upgrade != null && runeId != null))) throw new ArgumentException("A choice item must have exactly one operation.");
            if (Effects.Any(x => x.Target == Target.EventOther)) throw new ArgumentException("Items cannot target an event participant.");
        }
    }

    /// <summary>All balance constants cross the adapter boundary as one immutable definition.</summary>
    public sealed class RuleSettings
    {
        public int ActionsPerTurn { get; }
        public int EscapeCost { get; }
        public int TeamCapacity { get; }
        public int EventLimit { get; }
        public double HealthPerStrength { get; }
        public double RecoveryPerStrength { get; }
        public double DefensePerAgility { get; }
        public double ManaPerIntelligence { get; }
        public double RecoveryPerIntelligence { get; }
        public int IntelligencePerCooldown { get; }
        public double CounterMultiplier { get; }
        public int VictoryCooldownReduction { get; }
        public RuleSettings(int actionsPerTurn = 3, int escapeCost = 2, int teamCapacity = 6, int eventLimit = 2048,
            double healthPerStrength = 5, double recoveryPerStrength = .25, double defensePerAgility = .5,
            double manaPerIntelligence = 5, double recoveryPerIntelligence = 1, int intelligencePerCooldown = 3,
            double counterMultiplier = .5, int victoryCooldownReduction = 1)
        {
            if (actionsPerTurn < 1 || teamCapacity < 1 || eventLimit < 32 || intelligencePerCooldown < 1) throw new ArgumentOutOfRangeException();
            foreach (var n in new[] { healthPerStrength, recoveryPerStrength, defensePerAgility, manaPerIntelligence, recoveryPerIntelligence, counterMultiplier })
                if (Data.Finite(n) < 0) throw new ArgumentOutOfRangeException();
            ActionsPerTurn = actionsPerTurn; EscapeCost = Data.NonNegative(escapeCost); TeamCapacity = teamCapacity; EventLimit = eventLimit;
            HealthPerStrength = healthPerStrength; RecoveryPerStrength = recoveryPerStrength; DefensePerAgility = defensePerAgility;
            ManaPerIntelligence = manaPerIntelligence; RecoveryPerIntelligence = recoveryPerIntelligence; IntelligencePerCooldown = intelligencePerCooldown;
            CounterMultiplier = counterMultiplier;
            VictoryCooldownReduction = Data.NonNegative(victoryCooldownReduction);
        }
    }

    public sealed class GameCatalog
    {
        public RuleSettings Rules { get; }
        public IReadOnlyDictionary<string, SkillDefinition> Skills { get; }
        public IReadOnlyDictionary<string, StatusDefinition> Statuses { get; }
        public IReadOnlyDictionary<string, UnitDefinition> Units { get; }
        public IReadOnlyDictionary<string, ItemDefinition> Items { get; }
        public IReadOnlyDictionary<string, RuneDefinition> Runes { get; }
        public GameCatalog(RuleSettings rules, IEnumerable<SkillDefinition> skills, IEnumerable<StatusDefinition> statuses,
            IEnumerable<UnitDefinition> units, IEnumerable<ItemDefinition> items = null, IEnumerable<RuneDefinition> runes = null)
        {
            Rules = rules ?? throw new ArgumentNullException(nameof(rules));
            Skills = Index(skills, x => x.Id); Statuses = Index(statuses, x => x.Id); Units = Index(units, x => x.Id); Items = Index(items, x => x.Id);
            Runes = Index(runes, x => x.Id);
            foreach (var rune in Runes.Values) Validate(rune.Triggers);
            foreach (var skill in Skills.Values)
            {
                Validate(skill.Effects); Validate(skill.Triggers);
                if (skill.RequiredStatus != null) Require(Statuses.ContainsKey(skill.RequiredStatus), skill.RequiredStatus);
            }
            foreach (var status in Statuses.Values)
            {
                Validate(status.Triggers);
                foreach (var effect in status.Triggers.SelectMany(x => x.Effects)) Require(effect.Kind != EffectKind.ChangeCounter && effect.Condition?.CounterKey == null, "Status effects have no skill counter context.");
            }
            foreach (var item in Items.Values)
            {
                Validate(item.Effects); if (item.RuneId != null) Require(Runes.ContainsKey(item.RuneId), item.RuneId);
                foreach (var effect in item.Effects) Require(effect.Kind != EffectKind.ChangeCounter && effect.Condition?.CounterKey == null, "Item effects have no skill counter context.");
            }
            foreach (var unit in Units.Values)
            {
                var loadout = unit.Skills.Concat(unit.Weapon == null ? Array.Empty<SkillLoadout>() : new[] { unit.Weapon }).ToArray();
                Require(loadout.Select(x => x.SkillId).Distinct().Count() == loadout.Length, "Duplicate loadout skill in " + unit.Id);
                foreach (var entry in loadout)
                {
                    Require(Skills.ContainsKey(entry.SkillId), entry.SkillId);
                    Require(entry.Level <= Skills[entry.SkillId].MaxLevel, "Skill level above maximum: " + entry.SkillId);
                }
                if (unit.Weapon != null) Require(Skills[unit.Weapon.SkillId].Kind == SkillKind.Weapon, "Weapon must use the weapon skill kind.");
            }
        }
        private static IReadOnlyDictionary<string, T> Index<T>(IEnumerable<T> values, Func<T, string> key)
        {
            var result = new Dictionary<string, T>(StringComparer.Ordinal);
            foreach (var value in Data.List(values))
            { var id = key(value); if (result.ContainsKey(id)) throw new ArgumentException("Duplicate ID: " + id); result.Add(id, value); }
            return new ReadOnlyDictionary<string, T>(result);
        }
        private static void Require(bool valid, string reference) { if (!valid) throw new ArgumentException("Invalid definition reference: " + reference); }
        private void Validate(IEnumerable<TriggerDefinition> triggers) { foreach (var t in triggers) Validate(t.Effects); }
        private void Validate(IEnumerable<EffectDefinition> effects)
        {
            foreach (var e in effects)
            {
                if (e.Kind == EffectKind.AddStatus || e.Kind == EffectKind.RemoveStatus) Require(Statuses.ContainsKey(e.ReferenceId), e.ReferenceId);
                if (e.Kind == EffectKind.Summon) Require(Units.ContainsKey(e.ReferenceId), e.ReferenceId);
                if (e.Kind == EffectKind.CastSkill) Require(Skills.ContainsKey(e.ReferenceId), e.ReferenceId);
                if (e.Condition?.StatusId != null) Require(Statuses.ContainsKey(e.Condition.StatusId), e.Condition.StatusId);
            }
        }
        public UnitState CreateUnit(string definitionId, string instanceId, Team team, Control control = Control.Ai) =>
            new UnitState(this, Units[definitionId], instanceId, team, control);
    }
}
