using System;
using System.Collections.Generic;
using System.Linq;

namespace Dungeon.Core
{
    [Serializable] public sealed class InventorySaveData { public int Capacity, Coins; public Dictionary<string, int> Items; }
    [Serializable] public sealed class ShopSaveData { public string[] Stock; public int RefreshCount, RefreshBase, RefreshGrowth; public double PriceMultiplier; }
    [Serializable] public sealed class CellSaveData { public int X, Y; public bool Revealed, ContentKnown, Completed; }
    [Serializable] public sealed class EncounterSaveData { public int X, Y; public UnitSaveData[] Units; public UnitSaveData[] Summons; }
    [Serializable] public sealed class CellShopSaveData { public int X, Y; public ShopSaveData Shop; }
    [Serializable] public sealed class CellOfferSaveData { public int X, Y; public string[] Offers; }
    [Serializable] public sealed class SessionSaveData
    {
        public int Version = 2;
        public string MapId;
        public SessionPhase Phase;
        public CellSaveData[] Cells;
        public EncounterSaveData[] Encounters;
        public CellShopSaveData[] Shops;
        public CellOfferSaveData[] Offers;
        public bool HasInteraction;
        public int InteractionX, InteractionY;
    }
    [Serializable] public sealed class RunSaveData
    {
        public int Version = 3;
        public uint InitialSeed;
        public bool HasInitialSeed, GeneratedMaps;
        public int MapGenerationVersion;
        public string MapTopology;
        public string RunId, CampaignId, ContentVersion;
        public int Difficulty, RegionIndex, OpenSkillSlots, AwardedExperience;
        public RunPhase Phase;
        public bool Settled;
        public uint RandomState;
        public UnitSaveData Player;
        public InventorySaveData Inventory;
        public SessionSaveData Session;
        public Dictionary<int, int> Kills;
        public string[] CountedEnemies;
    }
    [Serializable] public sealed class HeroProgressSaveData
    { public string HeroId; public int Experience, Level, SpentPoints; public string[] Talents; }
    [Serializable] public sealed class ProfileSaveData
    {
        public int Version = 1, HighestDifficulty;
        public HeroProgressSaveData[] Heroes;
        public string[] Unlocks, Discoveries, Achievements, SettledRuns;
    }
    public sealed partial class Inventory
    {
        public InventorySaveData Capture() => new InventorySaveData { Capacity = Capacity, Coins = Coins, Items = new Dictionary<string, int>(counts) };
        public static Inventory Restore(GameCatalog catalog, InventorySaveData data)
        {
            if (data?.Items == null) throw new ArgumentException("Invalid inventory save.");
            var inventory = new Inventory(catalog, data.Capacity, data.Coins);
            if (!inventory.TryGrant(new RewardBundle(items: data.Items))) throw new ArgumentException("Saved inventory exceeds capacity or references unknown items.");
            return inventory;
        }
    }
    public sealed partial class Shop
    {
        public ShopSaveData Capture() => new ShopSaveData { Stock = stock.ToArray(), RefreshCount = RefreshCount, RefreshBase = refreshBase, RefreshGrowth = refreshGrowth, PriceMultiplier = priceMultiplier };
        public static Shop Restore(GameCatalog catalog, ShopSaveData data)
        {
            if (data?.Stock == null) throw new ArgumentException("Invalid shop save.");
            return new Shop(catalog, data.Stock, data.RefreshBase, data.RefreshGrowth, data.PriceMultiplier) { RefreshCount = Data.NonNegative(data.RefreshCount) };
        }
    }
    public sealed partial class GameSession
    {
        public SessionSaveData Capture()
        {
            if (Phase == SessionPhase.Battle) throw new InvalidOperationException("Save at the exploration checkpoint or after battle settlement.");
            return new SessionSaveData
            {
                MapId = map.Id, Phase = Phase,
                Cells = Cells.Select(x => new CellSaveData { X = x.Definition.Position.X, Y = x.Definition.Position.Y, Revealed = x.IsRevealed, ContentKnown = x.IsContentKnown, Completed = x.IsCompleted }).ToArray(),
                Encounters = encounters.Select(x => new EncounterSaveData
                {
                    X = x.Key.X, Y = x.Key.Y, Units = x.Value.Select(u => u.Capture()).ToArray(),
                    Summons = encounterSummons.TryGetValue(x.Key, out var summoned) ? summoned.Select(u => u.Capture()).ToArray() : Array.Empty<UnitSaveData>()
                }).ToArray(),
                Shops = shops.Select(x => new CellShopSaveData { X = x.Key.X, Y = x.Key.Y, Shop = x.Value.Capture() }).ToArray(),
                Offers = offers.Select(x => new CellOfferSaveData { X = x.Key.X, Y = x.Key.Y, Offers = x.Value.ToArray() }).ToArray(),
                HasInteraction = interaction != null, InteractionX = interaction?.Definition.Position.X ?? 0, InteractionY = interaction?.Definition.Position.Y ?? 0
            };
        }
        internal void RestoreState(SessionSaveData data)
        {
            if (data == null || (data.Version != 1 && data.Version != 2) || data.MapId != map.Id || data.Cells == null || data.Encounters == null || data.Shops == null || data.Offers == null || data.Phase == SessionPhase.Battle)
                throw new ArgumentException("Invalid session save.");
            Data.EnumValue(data.Phase);
            if (data.Cells.Length != cells.Count || data.Cells.Any(x => x == null) || data.Cells.Select(x => new CellPosition(x.X, x.Y)).Distinct().Count() != cells.Count) throw new ArgumentException("Saved map cells do not match.");
            foreach (var saved in data.Cells)
            {
                if (!cells.TryGetValue(new CellPosition(saved.X, saved.Y), out var cell) || (saved.Completed && !saved.ContentKnown)) throw new ArgumentException("Invalid saved cell.");
                if (saved.Completed && !cell.Definition.IsWalkable) throw new ArgumentException("阻挡地形不能标记为已完成探索。");
                cell.IsRevealed = saved.Revealed; cell.IsContentKnown = saved.ContentKnown; cell.IsCompleted = saved.Completed;
            }
            encounters.Clear(); encounterSummons.Clear(); shops.Clear(); offers.Clear();
            foreach (var saved in data.Encounters)
            {
                var position = new CellPosition(saved.X, saved.Y);
                if (!cells.TryGetValue(position, out var cell) || saved.Units == null || saved.Units.Length != cell.Definition.Enemies.Count || cell.Definition.Enemies.Count == 0) throw new ArgumentException("Invalid saved encounter.");
                var restored = saved.Units.Select(x => UnitState.Restore(catalog, x)).ToArray();
                for (int i = 0; i < restored.Length; i++)
                    if (restored[i].Definition.Id != cell.Definition.Enemies[i] || restored[i].IsSummoned || restored[i].Team != Team.Enemy || restored[i].Control != Control.Ai || restored[i].Id != map.Id + "/" + position + "/" + i) throw new ArgumentException("Saved enemy does not match encounter.");
                if (data.Version >= 2 && saved.Summons == null) throw new ArgumentException("Missing saved encounter summons.");
                var summons = (saved.Summons ?? Array.Empty<UnitSaveData>()).Select(x => UnitState.Restore(catalog, x)).ToArray();
                var all = restored.Concat(summons).ToArray();
                if (summons.Any(x => !x.IsSummoned || x.IsDead || x.Team != Team.Enemy || x.Control != Control.Ai || x.Id == Player.Id) ||
                    all.Select(x => x.Id).Distinct().Count() != all.Length || all.Where(x => !x.IsDead).Sum(x => x.Definition.Size) > catalog.Rules.TeamCapacity)
                    throw new ArgumentException("Invalid saved encounter participants.");
                if (data.Phase == SessionPhase.Exploration && !cell.IsCompleted && all.All(x => x.IsDead))
                    throw new ArgumentException("Unfinished encounter has no surviving enemies. Legacy lost summons cannot be reconstructed; restore a pre-battle checkpoint.");
                encounters.Add(position, restored);
                if (summons.Length > 0) encounterSummons.Add(position, summons);
            }
            foreach (var saved in data.Shops)
            {
                var position = new CellPosition(saved.X, saved.Y);
                if (!cells.TryGetValue(position, out var cell) || cell.Definition.Kind != CellKind.Shop) throw new ArgumentException("Invalid saved shop location.");
                var offer = world.Shops[cell.Definition.ContentId];
                if (saved.Shop?.Stock == null || saved.Shop.Stock.Length > offer.Count || saved.Shop.Stock.Distinct().Count() != saved.Shop.Stock.Length ||
                    saved.Shop.Stock.Any(x => !offer.Pool.Entries.Any(e => e.Value == x))) throw new ArgumentException("Saved stock does not match offer pool.");
                double multiplier = difficulty?.ShopMultiplier ?? 1;
                if (saved.Shop.RefreshBase != offer.RefreshBase || saved.Shop.RefreshGrowth != offer.RefreshGrowth || saved.Shop.PriceMultiplier != multiplier)
                    throw new ArgumentException("Saved shop pricing conflicts with the current offer/difficulty rules.");
                shops.Add(position, Shop.Restore(catalog, new ShopSaveData
                {
                    Stock = saved.Shop.Stock, RefreshCount = saved.Shop.RefreshCount,
                    RefreshBase = offer.RefreshBase, RefreshGrowth = offer.RefreshGrowth, PriceMultiplier = multiplier
                }));
            }
            foreach (var saved in data.Offers)
            {
                var position = new CellPosition(saved.X, saved.Y);
                if (!cells.TryGetValue(position, out var cell) || (cell.Definition.Kind != CellKind.SkillChoice && cell.Definition.Kind != CellKind.RelicChoice) || saved.Offers == null) throw new ArgumentException("Invalid saved offer location.");
                var definition = cell.Definition.Kind == CellKind.SkillChoice ? world.Skills[cell.Definition.ContentId] : world.Relics[cell.Definition.ContentId];
                if (saved.Offers.Length > definition.Count || saved.Offers.Distinct().Count() != saved.Offers.Length || saved.Offers.Any(x => !definition.Pool.Entries.Any(e => e.Value == x))) throw new ArgumentException("Invalid saved offer.");
                offers.Add(position, saved.Offers.ToList());
            }
            Phase = data.Phase; interaction = null;
            if (data.HasInteraction)
            {
                if (!cells.TryGetValue(new CellPosition(data.InteractionX, data.InteractionY), out interaction) || !IsInteraction(interaction.Definition.Kind) || interaction.IsCompleted || !interaction.IsContentKnown) throw new ArgumentException("Invalid active interaction.");
            }
            if (!cells[map.Entrance].IsCompleted || (Phase == SessionPhase.Exit && !Cells.Any(x => x.Definition.Kind == CellKind.Exit && x.IsCompleted))) throw new ArgumentException("Invalid session progress.");
        }
    }
    public sealed partial class GameRun
    {
        public RunSaveData Capture()
        {
            Synchronize();
            return new RunSaveData
            {
                RunId = Id, CampaignId = definition.Id, ContentVersion = definition.ContentVersion, Difficulty = Difficulty.Level,
                InitialSeed = InitialSeed, HasInitialSeed = HasInitialSeed, GeneratedMaps = definition.GeneratesMaps,
                MapGenerationVersion = definition.GeneratesMaps ? RunSeeds.MapGenerationVersion : 0,
                MapTopology = definition.GeneratesMaps ? RunSeeds.TopologyId(topology) : null,
                RegionIndex = RegionIndex, OpenSkillSlots = Skills.OpenSlots, AwardedExperience = AwardedExperience, Phase = Phase, Settled = Settled,
                RandomState = random.State, Player = Player.Capture(), Inventory = Inventory.Capture(), Session = Session.Capture(),
                Kills = new Dictionary<int, int>(kills), CountedEnemies = countedEnemies.OrderBy(x => x, StringComparer.Ordinal).ToArray()
            };
        }
        public static GameRun Restore(GameCatalog catalog, CampaignDefinition definition, WorldCatalog world, IMapTopology topology,
            PlayerProfile profile, RunSaveData data, IDictionary<string, IAiPolicy> policies = null) => new GameRun(catalog, definition, world, topology, profile, data, policies);
        private GameRun(GameCatalog catalog, CampaignDefinition definition, WorldCatalog world, IMapTopology topology,
            PlayerProfile profile, RunSaveData data, IDictionary<string, IAiPolicy> policies)
        {
            if (data == null || data.Version < 1 || data.Version > 3 || data.CampaignId != definition.Id || data.ContentVersion != definition.ContentVersion || data.RandomState == 0 || data.RegionIndex < 0 ||
                data.RegionIndex >= definition.RegionCount || data.Kills == null || data.CountedEnemies == null) throw new ArgumentException("Unsupported run save or changed content version. Migrate the save before restoring.");
            if (data.Version >= 3 && data.GeneratedMaps != definition.GeneratesMaps) throw new ArgumentException("Saved map generation mode differs from the campaign.");
            if (definition.GeneratesMaps && (data.Version < 3 || !data.HasInitialSeed || data.MapGenerationVersion != RunSeeds.MapGenerationVersion || data.MapTopology != RunSeeds.TopologyId(topology)))
                throw new ArgumentException("Seed-generated map requires its original seed, topology and generator version. Do not substitute the current time or RandomState.");
            HasInitialSeed = data.Version >= 3 && data.HasInitialSeed; InitialSeed = HasInitialSeed ? data.InitialSeed : 0;
            Id = Data.Id(data.RunId); this.catalog = catalog; this.definition = definition; this.world = world; this.topology = topology; this.profile = profile;
            this.policies = policies == null ? null : new Dictionary<string, IAiPolicy>(policies); random = new SeededRandom(data.RandomState); Difficulty = definition.Difficulties.At(data.Difficulty);
            RegionIndex = data.RegionIndex; Phase = Data.EnumValue(data.Phase); Settled = data.Settled; AwardedExperience = Data.NonNegative(data.AwardedExperience);
            Player = UnitState.Restore(catalog, data.Player); Inventory = Inventory.Restore(catalog, data.Inventory);
            if (Player.Id != Id + "/player" || Inventory.Capacity != definition.BagCapacity || (Settled && Phase == RunPhase.Playing)) throw new ArgumentException("Invalid run ownership or settlement.");
            Skills = new SkillBook(Player, definition.SkillSlots, Difficulty.SlotSurcharge, data.OpenSkillSlots);
            // The session constructor needs a living player; dead saves are reconstructed using the validated unit and restored immediately afterwards.
            int health = Player.Health; if (Player.IsDead) Player.Health = 1;
            try { Session = new GameSession(catalog, Player, Inventory, definition.CreateRegion(RegionIndex, InitialSeed, topology), topology, random, new RewardService(catalog, definition.Rewards, random), this.policies, Difficulty, world, profile, Skills); }
            finally { Player.Health = health; }
            Session.RestoreState(data.Session);
            BindSession();
            foreach (var pair in data.Kills) kills.Add(Data.NonNegative(pair.Key), Data.NonNegative(pair.Value));
            foreach (var id in data.CountedEnemies) if (!countedEnemies.Add(Data.Id(id))) throw new ArgumentException("Duplicate counted enemy.");
            if ((Phase == RunPhase.Victory && (RegionIndex != definition.RegionCount - 1 || Session.Phase != SessionPhase.Exit)) || (Phase == RunPhase.Playing && Player.IsDead)) throw new ArgumentException("Invalid saved run phase.");
            if (Settled != profile.settledRuns.Contains(Id)) throw new ArgumentException("Run and profile settlement records differ; restore snapshots from the same commit.");
        }
    }
    public sealed partial class PlayerProfile
    {
        public ProfileSaveData Capture() => new ProfileSaveData
        {
            HighestDifficulty = HighestDifficulty,
            Heroes = heroes.Values.OrderBy(x => x.HeroId, StringComparer.Ordinal).Select(x => new HeroProgressSaveData { HeroId = x.HeroId, Experience = x.Experience, Level = x.Level, SpentPoints = x.SpentPoints, Talents = x.Talents.ToArray() }).ToArray(),
            Unlocks = Unlocks.ToArray(), Discoveries = Discoveries.ToArray(), Achievements = Achievements.ToArray(), SettledRuns = settledRuns.OrderBy(x => x, StringComparer.Ordinal).ToArray()
        };
        public static PlayerProfile Restore(ProfileSaveData data)
        {
            if (data == null || data.Version != 1 || data.Heroes == null || data.Unlocks == null || data.Discoveries == null || data.Achievements == null || data.SettledRuns == null) throw new ArgumentException("Invalid profile save.");
            var profile = new PlayerProfile { HighestDifficulty = Data.NonNegative(data.HighestDifficulty) };
            foreach (var saved in data.Heroes)
            {
                if (saved == null || saved.Talents == null || saved.SpentPoints < 0 || saved.SpentPoints > saved.Level) throw new ArgumentException("Invalid hero progress.");
                var hero = new HeroProgress(saved.HeroId) { Experience = Data.NonNegative(saved.Experience), Level = Data.NonNegative(saved.Level), SpentPoints = saved.SpentPoints };
                foreach (var id in saved.Talents) if (!hero.Learned.Add(Data.Id(id))) throw new ArgumentException("Duplicate saved talent.");
                profile.heroes.Add(hero.HeroId, hero);
            }
            foreach (var id in data.Unlocks) profile.unlocks.Add(Data.Id(id));
            foreach (var id in data.Discoveries) profile.discoveries.Add(Data.Id(id));
            foreach (var id in data.Achievements) profile.achievements.Add(Data.Id(id));
            foreach (var id in data.SettledRuns) profile.settledRuns.Add(Data.Id(id));
            return profile;
        }
    }
}
