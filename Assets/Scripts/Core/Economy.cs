using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Dungeon.Core
{
    public sealed class RewardBundle
    {
        public int Coins { get; }
        public IReadOnlyDictionary<string, int> Items { get; }
        public RewardBundle(int coins = 0, IDictionary<string, int> items = null)
        {
            Coins = Data.NonNegative(coins); var copy = new Dictionary<string, int>(StringComparer.Ordinal);
            if (items != null) foreach (var pair in items) { Data.Id(pair.Key); if (pair.Value <= 0) throw new ArgumentOutOfRangeException(nameof(items)); copy.Add(pair.Key, pair.Value); }
            Items = new ReadOnlyDictionary<string, int>(copy);
        }
    }
    public sealed partial class Inventory
    {
        internal GameCatalog Catalog { get; }
        private Dictionary<string, int> counts = new Dictionary<string, int>(StringComparer.Ordinal);
        public int Capacity { get; }
        public int Coins { get; private set; }
        public IReadOnlyDictionary<string, int> Items => new ReadOnlyDictionary<string, int>(new Dictionary<string, int>(counts));
        public int OccupiedSlots => counts.Count(x => !Catalog.Items[x.Key].IsCurrency);
        public Inventory(GameCatalog catalog, int capacity, int coins = 0)
        { Catalog = catalog ?? throw new ArgumentNullException(nameof(catalog)); Capacity = Data.NonNegative(capacity); Coins = Data.NonNegative(coins); }
        public int Count(string id) => id != null && counts.TryGetValue(id, out var value) ? value : 0;
        public bool TryAdd(string id, int amount = 1)
        {
            if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount));
            return TryGrant(new RewardBundle(items: new Dictionary<string, int> { [Data.Id(id)] = amount }));
        }
        public bool TryGrant(RewardBundle reward)
        {
            if (reward == null) throw new ArgumentNullException(nameof(reward));
            if ((long)Coins + reward.Coins > int.MaxValue) return false;
            var next = new Dictionary<string, int>(counts, StringComparer.Ordinal);
            foreach (var pair in reward.Items)
            {
                if (!Catalog.Items.ContainsKey(pair.Key)) return false;
                next.TryGetValue(pair.Key, out var existing);
                if ((long)existing + pair.Value > int.MaxValue) return false;
                next[pair.Key] = existing + pair.Value;
            }
            if (next.Count(x => !Catalog.Items[x.Key].IsCurrency) > Capacity) return false;
            counts = next; Coins += reward.Coins; return true;
        }
        public bool TryRemove(string id, int amount = 1)
        {
            if (amount <= 0) throw new ArgumentOutOfRangeException(nameof(amount));
            int count = Count(id); if (count < amount) return false;
            if (count == amount) counts.Remove(id); else counts[id] = count - amount;
            return true;
        }
        public bool TrySpend(int amount)
        { Data.NonNegative(amount); if (Coins < amount) return false; Coins -= amount; return true; }
        public bool TryBuy(string id, int price)
        {
            Data.NonNegative(price); if (Coins < price || !TryAdd(id)) return false;
            Coins -= price; return true;
        }
    }
    public sealed class RankRewardDefinition
    {
        public int Rank { get; }
        public double DropChancePerEnemy { get; }
        public IReadOnlyList<string> DropPool { get; }
        public string PerKillItem { get; }
        public string PerEncounterItem { get; }
        public RankRewardDefinition(int rank, double dropChancePerEnemy, IEnumerable<string> dropPool = null, string perKillItem = null, string perEncounterItem = null)
        {
            if (Data.Finite(dropChancePerEnemy) < 0 || dropChancePerEnemy > 1) throw new ArgumentOutOfRangeException(nameof(dropChancePerEnemy));
            Rank = Data.NonNegative(rank); DropChancePerEnemy = dropChancePerEnemy; DropPool = Data.List(dropPool); PerKillItem = perKillItem; PerEncounterItem = perEncounterItem;
        }
    }
    public sealed class RewardService
    {
        internal GameCatalog Catalog => catalog;
        private readonly GameCatalog catalog;
        private readonly IReadOnlyList<RankRewardDefinition> ranks;
        private readonly IRandomSource random;
        public RewardService(GameCatalog catalog, IEnumerable<RankRewardDefinition> ranks, IRandomSource random)
        {
            this.catalog = catalog ?? throw new ArgumentNullException(nameof(catalog)); this.ranks = Data.List(ranks); this.random = random ?? throw new ArgumentNullException(nameof(random));
            if (this.ranks.Select(x => x.Rank).Distinct().Count() != this.ranks.Count) throw new ArgumentException("Duplicate reward rank.");
            foreach (var rank in this.ranks)
                foreach (var id in rank.DropPool.Concat(new[] { rank.PerKillItem, rank.PerEncounterItem }).Where(x => x != null))
                    if (!catalog.Items.ContainsKey(id)) throw new ArgumentException("Unknown reward item: " + id);
        }
        public bool TryClaim(Battle battle, Inventory inventory)
        {
            if (battle == null || inventory == null || inventory.Catalog != catalog || battle.Player.Catalog != catalog ||
                battle.Phase != BattlePhase.Finished || battle.Outcome != BattleOutcome.Victory || battle.RewardsClaimed) return false;
            if (battle.RolledRewards == null) battle.RolledRewards = Roll(battle);
            if (!inventory.TryGrant(battle.RolledRewards)) return false;
            battle.RewardsClaimed = true; return true;
        }
        private RewardBundle Roll(Battle battle)
        {
            var dead = (battle.RewardUnits ?? battle.Units).Where(x => x.Team == Team.Enemy && x.IsDead && !x.IsSummoned).ToArray();
            int coins = 0; var items = new Dictionary<string, int>(StringComparer.Ordinal);
            foreach (var unit in dead) coins = checked(coins + unit.Definition.Coins + Math.Max(0, Data.Int(battle.Player.GetStat(Stat.KillCoins))));
            foreach (var group in dead.GroupBy(x => x.Definition.Rank).OrderBy(x => x.Key))
            {
                var definition = ranks.FirstOrDefault(x => x.Rank == group.Key); if (definition == null) continue;
                Add(definition.PerKillItem, group.Count()); Add(definition.PerEncounterItem, 1);
                double chance = Math.Min(1, group.Count() * definition.DropChancePerEnemy);
                if (definition.DropPool.Count > 0 && chance > 0 && (chance >= 1 || random.Next(1000000) < chance * 1000000))
                    Add(definition.DropPool[random.Next(definition.DropPool.Count)], 1);
            }
            return new RewardBundle(coins, items);
            void Add(string id, int count) { if (id == null) return; items.TryGetValue(id, out var old); items[id] = checked(old + count); }
        }
    }
    public sealed partial class Shop
    {
        private readonly GameCatalog catalog;
        private readonly List<string> stock;
        private readonly int refreshBase, refreshGrowth;
        private readonly double priceMultiplier;
        public int RefreshCount { get; private set; }
        public IReadOnlyList<string> Stock => stock.AsReadOnly();
        public int RefreshPrice => checked(refreshBase + refreshGrowth * RefreshCount);
        public Shop(GameCatalog catalog, IEnumerable<string> stock, int refreshBase = 5, int refreshGrowth = 5, double priceMultiplier = 1)
        {
            this.catalog = catalog ?? throw new ArgumentNullException(nameof(catalog)); this.stock = Data.List(stock).ToList();
            Validate(this.stock); this.refreshBase = Data.NonNegative(refreshBase); this.refreshGrowth = Data.NonNegative(refreshGrowth);
            if (Data.Finite(priceMultiplier) < 0) throw new ArgumentOutOfRangeException(nameof(priceMultiplier)); this.priceMultiplier = priceMultiplier;
        }
        public int PriceAt(int slot) => Math.Max(0, Data.Int(catalog.Items[stock[slot]].Price * priceMultiplier));
        public bool TryBuy(int slot, Inventory inventory)
        {
            if (inventory.Catalog != catalog || slot < 0 || slot >= stock.Count || !inventory.TryBuy(stock[slot], PriceAt(slot))) return false;
            stock.RemoveAt(slot); return true;
        }
        public bool TryRefresh(Inventory inventory, IEnumerable<string> newStock)
        {
            var next = Data.List(newStock); Validate(next);
            if (inventory.Catalog != catalog || !inventory.TrySpend(RefreshPrice)) return false;
            stock.Clear(); stock.AddRange(next); RefreshCount++; return true;
        }
        private void Validate(IEnumerable<string> ids) { foreach (var id in ids) if (!catalog.Items.ContainsKey(id)) throw new ArgumentException("Unknown shop item: " + id); }
    }
    public static partial class Progression
    {
        public static bool LearnSkill(UnitState unit, string id, int capacity)
        {
            if (unit.InBattle || unit.IsDead || !unit.Catalog.Skills.TryGetValue(id, out var definition) || definition.Kind == SkillKind.Weapon || unit.FindSkill(id) != null || unit.Skills.Count >= capacity) return false;
            return ExplorationTransaction.Run(unit, null, () =>
            {
                var skill = new SkillState(new SkillLoadout(id)); unit.MutableSkills.Add(skill); WorldActions.TriggerSkill(unit, skill, Trigger.Equipped); return true;
            });
        }
        public static bool UpgradeSkill(UnitState unit, string id, int allowedRank, bool breakLimit = false)
        {
            var skill = unit.FindSkill(id); if (unit.InBattle || unit.IsDead || skill == null) return false;
            var definition = unit.Catalog.Skills[id];
            if (definition.Rank > allowedRank || definition.MaxLevel <= 1 || (!breakLimit && skill.Level >= definition.MaxLevel) || skill.Level == int.MaxValue) return false;
            skill.Level++; return true;
        }
        public static bool EquipRelic(UnitState unit, string skillId)
        {
            if (unit.InBattle || unit.IsDead || !unit.Catalog.Skills.TryGetValue(skillId, out var definition) || definition.Kind != SkillKind.Passive || unit.Relics.Any(x => x.Id == skillId)) return false;
            return ExplorationTransaction.Run(unit, null, () =>
            {
                var relic = new SkillState(new SkillLoadout(skillId)); unit.MutableRelics.Add(relic); WorldActions.TriggerSkill(unit, relic, Trigger.Equipped); return true;
            });
        }
        public static bool Rest(UnitState unit, double targetRatio)
        {
            if (Data.Finite(targetRatio) < 0 || targetRatio > 1) throw new ArgumentOutOfRangeException(nameof(targetRatio));
            if (unit.InBattle || unit.IsDead) return false;
            unit.Health = Math.Max(unit.Health, Data.Int(unit.MaxHealth * targetRatio)); unit.Mana = Math.Max(unit.Mana, Data.Int(unit.MaxMana * targetRatio));
            foreach (var skill in unit.AllSkills) skill.Cooldown = 0;
            return true;
        }
    }
}
