using System;
using System.Collections.Generic;
using System.Linq;

namespace Dungeon.Core
{
    /// <summary>Non-combat effects use the same definitions, without an artificial battle or a Unity callback.</summary>
    public static class WorldActions
    {
        public static bool UseItem(UnitState unit, Inventory inventory, string id)
        {
            if (unit.InBattle || unit.IsDead || inventory.Catalog != unit.Catalog || inventory.Count(id) == 0 || !unit.Catalog.Items.TryGetValue(id, out var item) ||
                item.Scope == UseScope.Battle || item.IsCurrency || item.Upgrade != null || item.RuneId != null || item.Effects.Count == 0 || !CanApply(unit.Catalog, item.Effects)) return false;
            return ExplorationTransaction.Run(unit, inventory, () =>
            {
                if (item.Consumable) inventory.TryRemove(id);
                var execution = new Execution(unit); execution.Apply(item.Effects, 1); execution.Raise(Trigger.AfterItem); return true;
            });
        }
        public static bool UseSkill(UnitState unit, string id)
        {
            if (unit.InBattle || unit.IsDead) return false;
            var skill = unit.FindSkill(id); if (skill == null) return false;
            var definition = unit.Catalog.Skills[id];
            if (definition.Kind == SkillKind.Passive || definition.Scope == UseScope.Battle || skill.Cooldown > 0 || !CanApply(unit.Catalog, definition.Effects) ||
                (definition.RequiredStatus != null && unit.Stacks(definition.RequiredStatus) == 0)) return false;
            int cost = unit.SkillResourceCost(id);
            if ((definition.Resource == Resource.Health ? unit.Health : unit.Mana) < cost) return false;
            return ExplorationTransaction.Run(unit, null, () => { new Execution(unit).Cast(skill); return true; });
        }
        public static void EnterRegion(UnitState unit)
        {
            if (unit.InBattle) throw new InvalidOperationException("Cannot enter a region during battle.");
            ExplorationTransaction.Run(unit, null, () => { new Execution(unit).Raise(Trigger.EnterRegion); return true; });
        }
        internal static void TriggerSkill(UnitState unit, SkillState skill, Trigger trigger) => new Execution(unit).Skill(skill, trigger, true);
        internal static void TriggerRune(UnitState unit, SkillState skill, Trigger trigger) => new Execution(unit).Rune(skill, trigger);
        public static bool CanApply(IEnumerable<EffectDefinition> effects) => effects.All(x =>
            (x.Target == Target.Self || x.Target == Target.AllAllies || x.Target == Target.LowestHealthAlly) &&
            (x.Kind == EffectKind.Heal || x.Kind == EffectKind.Mana || x.Kind == EffectKind.Damage || x.Kind == EffectKind.LoseMana ||
             x.Kind == EffectKind.AddStatus || x.Kind == EffectKind.RemoveStatus || x.Kind == EffectKind.Cleanse || x.Kind == EffectKind.ChangeStat || x.Kind == EffectKind.ReduceCooldown || x.Kind == EffectKind.CastSkill || x.Kind == EffectKind.ChangeCounter));
        public static bool CanApply(GameCatalog catalog, IEnumerable<EffectDefinition> effects) => ExplorationError(catalog, effects) == null;
        public static string ExplorationError(GameCatalog catalog, IEnumerable<EffectDefinition> effects)
        {
            var visiting = new HashSet<string>(StringComparer.Ordinal);
            var complete = new HashSet<string>(StringComparer.Ordinal);
            return Visit(effects, "effects");
            string Visit(IEnumerable<EffectDefinition> sequence, string path)
            {
                foreach (var effect in sequence)
                {
                    if (!CanApply(new[] { effect })) return path + ": unsupported exploration effect " + effect.Kind + "/" + effect.Target;
                    if (effect.Kind != EffectKind.CastSkill) continue;
                    string id = effect.ReferenceId, next = path + " -> " + id;
                    if (!catalog.Skills.TryGetValue(id, out var skill) || skill.Kind == SkillKind.Passive) return next + ": missing or passive skill";
                    if (complete.Contains(id)) continue;
                    if (!visiting.Add(id)) return next + ": cyclic additional cast";
                    string error = Visit(skill.Effects, next); visiting.Remove(id);
                    if (error != null) return error;
                    complete.Add(id);
                }
                return null;
            }
        }
        internal static void Apply(UnitState unit, IEnumerable<EffectDefinition> effects, int level = 1)
        {
            if (unit.InBattle) throw new InvalidOperationException("Invalid exploration phase.");
            string error = ExplorationError(unit.Catalog, effects); if (error != null) throw new InvalidOperationException(error);
            ExplorationTransaction.Run(unit, null, () => { new Execution(unit).Apply(effects, level); return true; });
        }
        private sealed class Execution
        {
            private readonly UnitState unit;
            private int operations;
            private bool resolvingDeath, deathResolved;
            internal Execution(UnitState unit) { this.unit = unit; }
            private void Step() { if (++operations > unit.Catalog.Rules.EventLimit) throw new ExplorationExecutionLimitException(); }
            internal void Cast(SkillState skill, bool payCost = true)
            {
                Step(); var definition = unit.Catalog.Skills[skill.Id];
                string error = ExplorationError(unit.Catalog, definition.Effects);
                if (definition.Kind == SkillKind.Passive || error != null) throw new InvalidOperationException(error ?? "Cannot cast a passive skill.");
                int cost = payCost ? unit.SkillResourceCost(skill.Id) : 0; Raise(Trigger.BeforeSkill);
                if (!unit.IsDead || resolvingDeath) Apply(definition.Effects, skill.Level, skill, unit.Rune(skill));
                if (payCost)
                {
                    skill.Cooldown = checked(skill.Cooldown + unit.SkillCooldown(skill.Id));
                    if (definition.Resource == Resource.Health) unit.Health = Math.Max(0, unit.Health - cost); else unit.Mana = Math.Max(0, unit.Mana - cost);
                }
                Raise(Trigger.AfterSkill); ResolveDeath();
            }
            internal void Apply(IEnumerable<EffectDefinition> effects, int level, SkillState context = null, RuneDefinition rune = null, bool sharedTrigger = false)
            {
                foreach (var effect in effects)
                {
                    Step(); if (unit.IsDead && !resolvingDeath) break;
                    // Battle-only target/effect definitions in shared passive triggers are inactive in exploration.
                    if (!CanApply(new[] { effect }) || (effect.Condition != null && !effect.Condition.Matches(unit, unit, context))) continue;
                    if (sharedTrigger && !CanApply(unit.Catalog, new[] { effect })) continue;
                    int amount = Data.Int(effect.Amount.Evaluate(level, unit.GetStat));
                    if (effect.Kind == EffectKind.Damage && rune != null) amount = Data.Int((amount + rune.DamageFlat) * rune.DamageMultiplier);
                    if (effect.Kind == EffectKind.AddStatus && rune != null) amount = checked(amount + rune.StatusStacks);
                    if (effect.Kind != EffectKind.ChangeStat && effect.Kind != EffectKind.ChangeCounter) amount = Math.Max(0, amount);
                    switch (effect.Kind)
                    {
                        case EffectKind.Heal: unit.Health = (int)Math.Min(unit.MaxHealth, (long)unit.Health + amount); break;
                        case EffectKind.Mana: unit.Mana = (int)Math.Min(unit.MaxMana, (long)unit.Mana + amount); break;
                        case EffectKind.LoseMana: unit.Mana = Math.Max(0, unit.Mana - amount); break;
                        case EffectKind.Damage:
                            for (int hit = 0; hit < Math.Max(1, checked(effect.Hits + (rune?.ExtraHits ?? 0))) && !unit.IsDead; hit++) { Step(); unit.Health = Math.Max(0, unit.Health - amount); }
                            break;
                        case EffectKind.AddStatus: unit.SetStatus(effect.ReferenceId, amount); break;
                        case EffectKind.RemoveStatus: unit.SetStatus(effect.ReferenceId, amount, true); break;
                        case EffectKind.Cleanse:
                            foreach (var status in unit.Statuses.Where(x => unit.Catalog.Statuses[x.Id].IsDebuff).ToArray()) unit.SetStatus(status.Id, amount, true); break;
                        case EffectKind.ChangeStat:
                            unit.BaseStats.TryGetValue(effect.Stat, out var value); unit.BaseStats[effect.Stat] = Data.Finite(value + amount); unit.ClampResources(); break;
                        case EffectKind.ReduceCooldown:
                            var skill = unit.AllSkills.OrderByDescending(x => x.Cooldown).FirstOrDefault(); if (skill != null) skill.Cooldown = Math.Max(0, skill.Cooldown - amount); break;
                        case EffectKind.CastSkill:
                            var extra = unit.FindSkill(effect.ReferenceId) ?? new SkillState(new SkillLoadout(effect.ReferenceId, Math.Min(level, unit.Catalog.Skills[effect.ReferenceId].MaxLevel))); Cast(extra, false); break;
                        case EffectKind.ChangeCounter:
                            if (context == null) throw new InvalidOperationException("Counter effects require a skill or rune context.");
                            context.MutableCounters.TryGetValue(effect.ReferenceId, out var previous); context.MutableCounters[effect.ReferenceId] = checked(previous + amount); break;
                    }
                    ResolveDeath();
                }
            }
            private void ResolveDeath()
            {
                if (!unit.IsDead) { deathResolved = false; return; }
                if (resolvingDeath || deathResolved) return;
                deathResolved = true; resolvingDeath = true;
                try { Raise(Trigger.SelfDeath); } finally { resolvingDeath = false; if (!unit.IsDead) deathResolved = false; }
            }
            internal void Raise(Trigger trigger)
            {
                Step(); if (unit.IsDead && trigger != Trigger.SelfDeath) return;
                foreach (var status in unit.Statuses.ToArray())
                    foreach (var handler in unit.Catalog.Statuses[status.Id].Triggers.Where(x => x.When == trigger))
                    {
                        if (!unit.MutableStatuses.Contains(status)) break;
                        int level = status.Stacks;
                        if (handler.ConsumeStacks > level) continue;
                        if (handler.ConsumeStacks > 0) unit.SetStatus(status.Id, handler.ConsumeStacks, true);
                        Apply(handler.Effects, level, sharedTrigger: true);
                    }
                foreach (var skill in unit.Relics.Concat(unit.AllSkills).ToArray()) Skill(skill, trigger, true);
            }
            internal void Skill(SkillState skill, Trigger trigger, bool includeRune)
            {
                Step(); foreach (var handler in unit.Catalog.Skills[skill.Id].Triggers.Where(x => x.When == trigger)) Apply(handler.Effects, skill.Level, skill, sharedTrigger: true);
                if (includeRune) Rune(skill, trigger);
            }
            internal void Rune(SkillState skill, Trigger trigger)
            {
                Step(); var rune = unit.Rune(skill); if (rune == null) return;
                foreach (var handler in rune.Triggers.Where(x => x.When == trigger)) Apply(handler.Effects, skill.Level, skill, sharedTrigger: true);
            }
        }
    }
    public sealed partial class GameSession
    {
        public bool UseItem(string id) { if (!CanExplore) return false; var result = WorldActions.UseItem(Player, Inventory, id); CheckPlayer(); return result; }
        public bool UseSkill(string id) { if (!CanExplore) return false; var result = WorldActions.UseSkill(Player, id); CheckPlayer(); return result; }
        private void CheckPlayer() { if (Player.IsDead) Phase = SessionPhase.Defeat; }
    }
}
