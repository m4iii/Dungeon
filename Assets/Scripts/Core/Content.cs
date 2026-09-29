using System;
using System.Collections.Generic;
using System.Linq;

namespace Dungeon.Core
{
    public sealed class WeightedEntry<T>
    {
        public T Value { get; }
        public int Weight { get; }
        public WeightedEntry(T value, int weight = 1)
        { if (value == null || weight < 1) throw new ArgumentException("A weighted entry needs a value and positive weight."); Value = value; Weight = weight; }
    }
    public sealed class WeightedPool<T>
    {
        public IReadOnlyList<WeightedEntry<T>> Entries { get; }
        public WeightedPool(IEnumerable<WeightedEntry<T>> entries) { Entries = Data.List(entries); checked { int total = 0; foreach (var entry in Entries) total += entry.Weight; } }
        public IReadOnlyList<T> Draw(IRandomSource random, int count, bool replacement = false, Func<T, bool> predicate = null)
        {
            Data.NonNegative(count); var candidates = Entries.Where(x => predicate == null || predicate(x.Value)).ToList(); var result = new List<T>();
            while (result.Count < count && candidates.Count > 0)
            {
                int roll = random.Next(candidates.Sum(x => x.Weight)), index = 0;
                while (roll >= candidates[index].Weight) roll -= candidates[index++].Weight;
                var entry = candidates[index]; result.Add(entry.Value);
                if (!replacement) candidates.RemoveAll(x => EqualityComparer<T>.Default.Equals(x.Value, entry.Value));
            }
            return result.AsReadOnly();
        }
    }
    public sealed class CellTemplate
    {
        public CellKind Kind { get; }
        public IReadOnlyList<string> Enemies { get; }
        public RewardBundle Reward { get; }
        public string RequiredKey { get; }
        public string ContentId { get; }
        public TerrainKind? Terrain { get; }
        public bool IsWalkable => Kind != CellKind.Blocked && TerrainRules.IsWalkable(Terrain ?? TerrainKind.Plains);
        public CellTemplate(CellKind kind, IEnumerable<string> enemies = null, RewardBundle reward = null, string requiredKey = null, string contentId = null, TerrainKind? terrain = null)
        {
            var cell = new CellDefinition(default, kind, enemies, reward, requiredKey, contentId, terrain);
            Terrain = terrain;
            Kind = cell.Kind; Enemies = cell.Enemies; Reward = cell.Reward; RequiredKey = cell.RequiredKey; ContentId = cell.ContentId;
        }
        public CellDefinition At(CellPosition position) => new CellDefinition(position, Kind, Enemies, Reward, RequiredKey, ContentId, Terrain);
    }
    public sealed class MapGenerationDefinition
    {
        public string Id { get; }
        public IReadOnlyList<CellPosition> Positions { get; }
        public CellPosition Entrance { get; }
        public IReadOnlyList<CellDefinition> FixedCells { get; }
        public WeightedPool<CellTemplate> Pool { get; }
        public bool BossGate { get; }
        public double RestRatio { get; }
        public MapShapeDefinition Shape { get; }
        public TerrainGenerationDefinition Terrain { get; }
        public RoadRoomDefinition RoadRooms { get; }
        public MapGenerationDefinition(string id, IEnumerable<CellPosition> positions, CellPosition entrance, IEnumerable<CellDefinition> fixedCells,
            WeightedPool<CellTemplate> pool, bool bossGate = true, double restRatio = 1, MapShapeDefinition shape = null, TerrainGenerationDefinition terrain = null,
            RoadRoomDefinition roadRooms = null)
        {
            Id = Data.Id(id); Positions = Data.List(positions); Entrance = entrance; FixedCells = Data.List(fixedCells); Pool = pool ?? throw new ArgumentNullException(nameof(pool)); BossGate = bossGate;
            Shape = shape;
            Terrain = terrain;
            RoadRooms = roadRooms;
            if (roadRooms != null && shape == null) throw new ArgumentException("主路房间模式需要形状配置。");
            if (Data.Finite(restRatio) < 0 || restRatio > 1) throw new ArgumentOutOfRangeException(nameof(restRatio)); RestRatio = restRatio;
            if (Positions.Count == 0 || Positions.Distinct().Count() != Positions.Count || !Positions.Contains(entrance) ||
                FixedCells.Select(x => x.Position).Distinct().Count() != FixedCells.Count || FixedCells.Any(x => !Positions.Contains(x.Position) || x.Position.Equals(entrance))) throw new ArgumentException("Invalid generation coordinates.");
        }
    }
    public static class MapGenerator
    {
        public static MapDefinition Generate(MapGenerationDefinition definition, IMapTopology topology, IRandomSource random)
        {
            if (definition.Shape != null)
            {
                var shaped = definition.RoadRooms == null ? ConnectedMapGenerator.Generate(definition, topology, random) : RoadRoomMapGenerator.Generate(definition, topology, random);
                return definition.Terrain == null ? shaped : TerrainMapGenerator.Apply(shaped, definition.Terrain, topology, random);
            }
            var cells = definition.FixedCells.ToDictionary(x => x.Position); cells.Add(definition.Entrance, new CellDefinition(definition.Entrance, CellKind.Empty));
            foreach (var position in definition.Positions.Where(x => !cells.ContainsKey(x)))
            {
                var sample = definition.Pool.Draw(random, 1); if (sample.Count == 0) throw new ArgumentException("Map pool is empty.");
                cells.Add(position, sample[0].At(position));
            }
            // Validate the generated graph; silently rerolling a disconnected map would change the random stream.
            var reached = new HashSet<CellPosition> { definition.Entrance }; var queue = new Queue<CellPosition>(); queue.Enqueue(definition.Entrance);
            while (queue.Count > 0)
                foreach (var neighbor in topology.Neighbors(queue.Dequeue()))
                    if (cells.TryGetValue(neighbor, out var cell) && cell.IsWalkable && reached.Add(neighbor)) queue.Enqueue(neighbor);
            if (cells.Values.Any(x => x.IsWalkable && !reached.Contains(x.Position))) throw new ArgumentException("Generated map contains unreachable cells; reserve a connected path in the generation definition.");
            var result = new MapDefinition(definition.Id, definition.Entrance, definition.Positions.Select(x => cells[x]), definition.BossGate, definition.RestRatio);
            return definition.Terrain == null ? result : TerrainMapGenerator.Apply(result, definition.Terrain, topology, random);
        }
    }
    public sealed class OfferDefinition
    {
        public string Id { get; }
        public WeightedPool<string> Pool { get; }
        public int Count { get; }
        public int Price { get; }
        public int RefreshBase { get; }
        public int RefreshGrowth { get; }
        public OfferDefinition(string id, WeightedPool<string> pool, int count, int price = 0, int refreshBase = 5, int refreshGrowth = 5)
        { Id = Data.Id(id); Pool = pool ?? throw new ArgumentNullException(nameof(pool)); Count = Data.NonNegative(count); Price = Data.NonNegative(price); RefreshBase = Data.NonNegative(refreshBase); RefreshGrowth = Data.NonNegative(refreshGrowth); }
    }
    public sealed class EventOptionDefinition
    {
        public string Id { get; }
        public RewardBundle Cost { get; }
        public RewardBundle Reward { get; }
        public int HealthCost { get; }
        public string RequiredUnlock { get; }
        public string GrantUnlock { get; }
        public IReadOnlyList<EffectDefinition> Effects { get; }
        public EventOptionDefinition(string id, RewardBundle reward = null, RewardBundle cost = null, int healthCost = 0,
            IEnumerable<EffectDefinition> effects = null, string requiredUnlock = null, string grantUnlock = null)
        {
            Id = Data.Id(id); Reward = reward ?? new RewardBundle(); Cost = cost ?? new RewardBundle(); HealthCost = Data.NonNegative(healthCost);
            Effects = Data.List(effects); if (!WorldActions.CanApply(Effects)) throw new ArgumentException("Event effects must support exploration.");
            RequiredUnlock = requiredUnlock; GrantUnlock = grantUnlock;
            if (requiredUnlock != null) Data.Id(requiredUnlock);
            if (grantUnlock != null) Data.Id(grantUnlock);
        }
    }
    public sealed class EventDefinition
    {
        public string Id { get; }
        public IReadOnlyList<EventOptionDefinition> Options { get; }
        public EventDefinition(string id, IEnumerable<EventOptionDefinition> options)
        { Id = Data.Id(id); Options = Data.List(options); if (Options.Count == 0 || Options.Select(x => x.Id).Distinct().Count() != Options.Count) throw new ArgumentException("Events need unique options."); }
    }
    public sealed class WorldCatalog
    {
        public IReadOnlyDictionary<string, OfferDefinition> Shops { get; }
        public IReadOnlyDictionary<string, OfferDefinition> Skills { get; }
        public IReadOnlyDictionary<string, OfferDefinition> Relics { get; }
        public IReadOnlyDictionary<string, EventDefinition> Events { get; }
        public WorldCatalog(GameCatalog catalog, IEnumerable<OfferDefinition> shops = null, IEnumerable<OfferDefinition> skills = null,
            IEnumerable<OfferDefinition> relics = null, IEnumerable<EventDefinition> events = null)
        {
            Shops = Index(shops, x => x.Id); Skills = Index(skills, x => x.Id); Relics = Index(relics, x => x.Id); Events = Index(events, x => x.Id);
            foreach (var offer in Shops.Values) foreach (var entry in offer.Pool.Entries) if (!catalog.Items.ContainsKey(entry.Value)) throw new ArgumentException("Unknown shop item.");
            foreach (var offer in Skills.Values.Concat(Relics.Values)) foreach (var entry in offer.Pool.Entries)
                if (!catalog.Skills.ContainsKey(entry.Value)) throw new ArgumentException("Unknown offered skill.");
            foreach (var offer in Relics.Values) foreach (var entry in offer.Pool.Entries)
                if (catalog.Skills[entry.Value].Kind != SkillKind.Passive) throw new ArgumentException("Relics must be passive skills.");
            foreach (var option in Events.Values.SelectMany(x => x.Options))
            {
                string executionError = WorldActions.ExplorationError(catalog, option.Effects);
                if (executionError != null) throw new ArgumentException(executionError);
                foreach (var id in option.Cost.Items.Keys.Concat(option.Reward.Items.Keys)) if (!catalog.Items.ContainsKey(id)) throw new ArgumentException("Unknown event item.");
                foreach (var effect in option.Effects)
                {
                    if (effect.Kind == EffectKind.ChangeCounter || effect.Condition?.CounterKey != null) throw new ArgumentException("Event effects have no skill counter context.");
                    if ((effect.Kind == EffectKind.AddStatus || effect.Kind == EffectKind.RemoveStatus) && !catalog.Statuses.ContainsKey(effect.ReferenceId)) throw new ArgumentException("Unknown event status.");
                    if (effect.Kind == EffectKind.CastSkill && !catalog.Skills.ContainsKey(effect.ReferenceId)) throw new ArgumentException("Unknown event skill.");
                }
            }
        }
        private static IReadOnlyDictionary<string, T> Index<T>(IEnumerable<T> values, Func<T, string> key) =>
            new System.Collections.ObjectModel.ReadOnlyDictionary<string, T>(Data.List(values).ToDictionary(key, StringComparer.Ordinal));
    }
    public sealed class ScheduledAiPolicy : IAiPolicy
    {
        private readonly IReadOnlyList<string> priority;
        private readonly int firstRound, interval;
        public ScheduledAiPolicy(IEnumerable<string> prioritySkills, int firstRound = 1, int interval = 1)
        { priority = Data.List(prioritySkills); if (firstRound < 1 || interval < 1) throw new ArgumentOutOfRangeException(); this.firstRound = firstRound; this.interval = interval; }
        public string ChooseSkill(AiContext context, IRandomSource random) =>
            (context.Round >= firstRound && (context.Round - firstRound) % interval == 0 ? priority.FirstOrDefault(context.AvailableSkills.Contains) : null) ?? new RandomAiPolicy().ChooseSkill(context, random);
    }
    public sealed class SummonerAiPolicy : IAiPolicy
    {
        private readonly string skill;
        private readonly IReadOnlyList<double> chances;
        public SummonerAiPolicy(string summonSkill, IEnumerable<double> chancesByAllyCount)
        {
            skill = Data.Id(summonSkill); chances = Data.List(chancesByAllyCount);
            foreach (var chance in chances) if (Data.Finite(chance) < 0 || chance > 1) throw new ArgumentOutOfRangeException(nameof(chancesByAllyCount));
        }
        public string ChooseSkill(AiContext context, IRandomSource random)
        {
            double chance = context.Allies.Count < chances.Count ? chances[context.Allies.Count] : 0;
            if (context.AvailableSkills.Contains(skill) && chance > 0 && (chance >= 1 || random.Next(1000000) < chance * 1000000)) return skill;
            var remaining = context.AvailableSkills.Where(x => x != skill).ToArray(); return remaining.Length == 0 ? null : remaining[random.Next(remaining.Length)];
        }
    }
}
