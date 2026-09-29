using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Dungeon.Core
{
    public sealed class EffectCondition
    {
        public string StatusId { get; }
        public int MinimumStacks { get; }
        public double MaximumHealthRatio { get; }
        public bool OnSource { get; }
        public string CounterKey { get; }
        public int MinimumCounter { get; }
        public EffectCondition(string statusId = null, int minimumStacks = 1, double maximumHealthRatio = 1, bool onSource = false, string counterKey = null, int minimumCounter = 1)
        {
            if (statusId != null) Data.Id(statusId);
            if (Data.Finite(maximumHealthRatio) < 0 || maximumHealthRatio > 1) throw new ArgumentOutOfRangeException(nameof(maximumHealthRatio));
            StatusId = statusId; MinimumStacks = Data.NonNegative(minimumStacks); MaximumHealthRatio = maximumHealthRatio; OnSource = onSource;
            CounterKey = counterKey; MinimumCounter = minimumCounter; if (counterKey != null) Data.Id(counterKey);
        }
        internal bool Matches(UnitState source, UnitState target, SkillState skill = null)
        {
            if (CounterKey != null && (skill == null || !skill.MutableCounters.TryGetValue(CounterKey, out var counter) || counter < MinimumCounter)) return false;
            var unit = OnSource ? source : target;
            return unit != null && (StatusId == null || unit.Stacks(StatusId) >= MinimumStacks) && unit.Health <= unit.MaxHealth * MaximumHealthRatio;
        }
    }
    public sealed class RuneDefinition
    {
        public string Id { get; }
        public IReadOnlyList<string> RequiredTags { get; }
        public IReadOnlyList<SkillKind> Kinds { get; }
        public bool AllowSingleLevel { get; }
        public double DamageMultiplier { get; }
        public int DamageFlat { get; }
        public int ExtraHits { get; }
        public int ResourceCost { get; }
        public int ActionCost { get; }
        public int Cooldown { get; }
        public int StatusStacks { get; }
        public IReadOnlyList<TriggerDefinition> Triggers { get; }
        public RuneDefinition(string id, IEnumerable<SkillKind> kinds, IEnumerable<string> requiredTags = null,
            bool allowSingleLevel = false, double damageMultiplier = 1, int damageFlat = 0, int extraHits = 0,
            int resourceCost = 0, int actionCost = 0, int cooldown = 0, int statusStacks = 0, IEnumerable<TriggerDefinition> triggers = null)
        {
            Id = Data.Id(id); Kinds = Data.List(kinds); foreach (var kind in Kinds) Data.EnumValue(kind);
            RequiredTags = Data.List(requiredTags); foreach (var tag in RequiredTags) Data.Id(tag);
            if (Data.Finite(damageMultiplier) < 0) throw new ArgumentOutOfRangeException(nameof(damageMultiplier));
            DamageMultiplier = damageMultiplier; DamageFlat = damageFlat; ExtraHits = extraHits; ResourceCost = resourceCost;
            ActionCost = actionCost; Cooldown = cooldown; StatusStacks = statusStacks; AllowSingleLevel = allowSingleLevel; Triggers = Data.List(triggers);
            if (Triggers.Any(x => x.ConsumeStacks != 0)) throw new ArgumentException("Rune triggers do not consume status stacks.");
        }
        public bool Accepts(SkillDefinition skill) => Kinds.Contains(skill.Kind) && (AllowSingleLevel || skill.MaxLevel > 1) && RequiredTags.All(skill.Tags.Contains);
    }
    public sealed class ItemUpgrade
    {
        public int AllowedRank { get; }
        public bool BreakLimit { get; }
        public ItemUpgrade(int allowedRank, bool breakLimit = false) { AllowedRank = Data.NonNegative(allowedRank); BreakLimit = breakLimit; }
    }
    public sealed class SkillSlotDefinition
    {
        public int InitialSlots { get; }
        public IReadOnlyList<int> UnlockPrices { get; }
        public int MaximumSlots => InitialSlots + UnlockPrices.Count;
        public SkillSlotDefinition(int initialSlots, IEnumerable<int> unlockPrices)
        {
            InitialSlots = Data.NonNegative(initialSlots); UnlockPrices = Data.List(unlockPrices);
            foreach (var price in UnlockPrices) Data.NonNegative(price);
        }
    }
    public sealed class SkillBook
    {
        private readonly UnitState unit;
        internal UnitState Owner => unit;
        private readonly SkillSlotDefinition definition;
        private readonly int surcharge;
        public int OpenSlots { get; private set; }
        public int MaximumSlots => definition.MaximumSlots;
        public int? NextUnlockPrice => OpenSlots >= MaximumSlots ? (int?)null : checked(definition.UnlockPrices[OpenSlots - definition.InitialSlots] + surcharge);
        public SkillBook(UnitState unit, SkillSlotDefinition definition, int surcharge = 0, int? openSlots = null)
        {
            this.unit = unit ?? throw new ArgumentNullException(nameof(unit)); this.definition = definition ?? throw new ArgumentNullException(nameof(definition));
            this.surcharge = Data.NonNegative(surcharge); OpenSlots = openSlots ?? definition.InitialSlots;
            if (OpenSlots < definition.InitialSlots || OpenSlots > MaximumSlots || unit.Skills.Count > OpenSlots) throw new ArgumentException("Invalid skill capacity.");
        }
        public bool Unlock(Inventory inventory)
        {
            if (unit.InBattle || inventory.Catalog != unit.Catalog || !NextUnlockPrice.HasValue || !inventory.TrySpend(NextUnlockPrice.Value)) return false;
            OpenSlots++; return true;
        }
        public bool Learn(string id) => Progression.LearnSkill(unit, id, OpenSlots);
        public bool Forget(string id)
        {
            var skill = unit.MutableSkills.FirstOrDefault(x => x.Id == id);
            if (unit.InBattle || skill == null || skill.Locked) return false;
            return ExplorationTransaction.Run(unit, null, () => { WorldActions.TriggerSkill(unit, skill, Trigger.Unequipped); unit.MutableSkills.Remove(skill); return true; });
        }
        public bool SetLocked(string id, bool locked)
        {
            var skill = unit.FindSkill(id); if (unit.InBattle || skill == null) return false;
            skill.Locked = locked; return true;
        }
        public bool Move(int from, int to)
        {
            if (unit.InBattle || from < 0 || to < 0 || from >= unit.Skills.Count || to >= unit.Skills.Count) return false;
            var skill = unit.MutableSkills[from]; unit.MutableSkills.RemoveAt(from); unit.MutableSkills.Insert(to, skill); return true;
        }
    }
    public static partial class Progression
    {
        public static bool UseRune(UnitState unit, Inventory inventory, string itemId, string skillId)
        {
            if (inventory.Catalog != unit.Catalog || inventory.Count(itemId) < 1 || !unit.Catalog.Items.TryGetValue(itemId, out var item) || item.RuneId == null || item.Scope == UseScope.Battle) return false;
            return ExplorationTransaction.Run(unit, inventory, () =>
            {
                if (!EquipRune(unit, skillId, item.RuneId)) return false;
                if (item.Consumable) inventory.TryRemove(itemId); return true;
            });
        }
        public static bool EquipRune(UnitState unit, string skillId, string runeId)
        {
            var skill = unit.FindSkill(skillId);
            if (unit.InBattle || unit.IsDead || skill == null || skill.RuneId == runeId || !unit.Catalog.Runes.TryGetValue(runeId, out var rune) || !rune.Accepts(unit.Catalog.Skills[skillId])) return false;
            return ExplorationTransaction.Run(unit, null, () =>
            {
                if (skill.RuneId != null) WorldActions.TriggerRune(unit, skill, Trigger.Unequipped);
                skill.RuneId = runeId; WorldActions.TriggerRune(unit, skill, Trigger.Equipped); return true;
            });
        }
        public static bool RemoveRune(UnitState unit, string skillId)
        {
            var skill = unit.FindSkill(skillId); if (unit.InBattle || skill?.RuneId == null) return false;
            return ExplorationTransaction.Run(unit, null, () => { WorldActions.TriggerRune(unit, skill, Trigger.Unequipped); skill.RuneId = null; return true; });
        }
        public static bool RemoveRelic(UnitState unit, string skillId)
        {
            var relic = unit.MutableRelics.FirstOrDefault(x => x.Id == skillId); if (unit.InBattle || relic == null) return false;
            return ExplorationTransaction.Run(unit, null, () => { WorldActions.TriggerSkill(unit, relic, Trigger.Unequipped); unit.MutableRelics.Remove(relic); return true; });
        }
        public static bool UseUpgrade(UnitState unit, Inventory inventory, string itemId, string skillId)
        {
            if (unit.InBattle || unit.IsDead || inventory.Catalog != unit.Catalog || inventory.Count(itemId) < 1 || !unit.Catalog.Items.TryGetValue(itemId, out var item) || item.Upgrade == null || item.Scope == UseScope.Battle) return false;
            if (!UpgradeSkill(unit, skillId, item.Upgrade.AllowedRank, item.Upgrade.BreakLimit)) return false;
            if (item.Consumable) inventory.TryRemove(itemId); return true;
        }
    }
    public sealed partial class UnitState
    {
        internal static SkillSaveData SaveSkill(SkillState skill) => new SkillSaveData
        { Id = skill.Id, Level = skill.Level, Cooldown = skill.Cooldown, Locked = skill.Locked, RuneId = skill.RuneId, Counters = new Dictionary<string, int>(skill.MutableCounters) };
        internal RuneDefinition Rune(SkillState skill) => skill?.RuneId == null ? null : Catalog.Runes[skill.RuneId];
        public int SkillResourceCost(string id)
        {
            var skill = FindSkill(id) ?? throw new ArgumentException("Unknown skill.");
            return Math.Max(0, checked(Data.Int(Catalog.Skills[id].Cost.Evaluate(skill.Level, GetStat)) + (Rune(skill)?.ResourceCost ?? 0)));
        }
        public int SkillActionCost(string id)
        {
            var skill = FindSkill(id) ?? throw new ArgumentException("Unknown skill.");
            return Math.Max(0, checked(Catalog.Skills[id].ActionCost + Data.Int(GetStat(Stat.ActionCost)) + (Rune(skill)?.ActionCost ?? 0)));
        }
        public int SkillCooldown(string id)
        {
            var skill = FindSkill(id) ?? throw new ArgumentException("Unknown skill.");
            return Math.Max(0, checked(Data.Int(Catalog.Skills[id].Cooldown.Evaluate(skill.Level, GetStat)) + (Rune(skill)?.Cooldown ?? 0)));
        }
    }
    public sealed class CraftRecipe
    {
        public string Id { get; }
        public IReadOnlyDictionary<string, int> Ingredients { get; }
        public RewardBundle Result { get; }
        public int CoinCost { get; }
        public CraftRecipe(string id, IDictionary<string, int> ingredients, RewardBundle result, int coinCost = 0)
        {
            Id = Data.Id(id); Ingredients = new RewardBundle(items: ingredients).Items; Result = result ?? throw new ArgumentNullException(nameof(result)); CoinCost = Data.NonNegative(coinCost);
            if (Ingredients.Count == 0 && coinCost == 0) throw new ArgumentException("Recipe must have a cost.");
        }
    }
    public sealed partial class Inventory
    {
        public bool TryExchange(RewardBundle reward, IDictionary<string, int> cost, int coins = 0)
        {
            Data.NonNegative(coins); if (Coins < coins) return false;
            var next = new Inventory(Catalog, Capacity, Coins - coins); next.counts = new Dictionary<string, int>(counts);
            if (cost != null) foreach (var pair in cost) if (!next.TryRemove(pair.Key, pair.Value)) return false;
            if (!next.TryGrant(reward)) return false;
            counts = next.counts; Coins = next.Coins; return true;
        }
        public bool TryCraft(CraftRecipe recipe) => TryExchange(recipe.Result, recipe.Ingredients.ToDictionary(x => x.Key, x => x.Value), recipe.CoinCost);
        public bool TrySell(string itemId, int quantity, double priceRatio = .5)
        {
            if (Data.Finite(priceRatio) < 0 || quantity < 1 || !Catalog.Items.TryGetValue(itemId, out var item) || item.IsCurrency) return false;
            return TryExchange(new RewardBundle(checked(Data.Int(item.Price * priceRatio) * quantity)), new Dictionary<string, int> { [itemId] = quantity });
        }
    }
}
