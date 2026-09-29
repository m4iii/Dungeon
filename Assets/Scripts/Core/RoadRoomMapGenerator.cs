using System;
using System.Collections.Generic;
using System.Linq;

namespace Dungeon.Core
{
    public sealed class RequiredMapContent
    {
        public CellTemplate Content { get; }
        public int Count { get; }
        public RequiredMapContent(CellTemplate content, int count)
        {
            Content = content ?? throw new ArgumentNullException(nameof(content)); Count = Data.NonNegative(count);
            if (!content.IsWalkable || content.Kind == CellKind.Exit || content.Kind == CellKind.Empty)
                throw new ArgumentException("必放内容必须是可进入的事件、奖励或战斗；出口由主路配置控制。");
        }
    }
    public sealed class RoadRoomDefinition
    {
        public int MainRoadLength { get; }
        public int ExitCount { get; }
        public int MinRoomSize { get; }
        public int MaxRoomSize { get; }
        public double EmptyRatio { get; }
        public IReadOnlyList<RequiredMapContent> Required { get; }
        public RoadRoomDefinition(int mainRoadLength = 12, int exitCount = 1, int minRoomSize = 5, int maxRoomSize = 9,
            double emptyRatio = .45, IEnumerable<RequiredMapContent> required = null)
        {
            if (mainRoadLength < 1 || exitCount < 1 || exitCount > 4 || minRoomSize < 2 || maxRoomSize < minRoomSize ||
                maxRoomSize > 4096 || Data.Finite(emptyRatio) < 0 || emptyRatio >= 1) throw new ArgumentOutOfRangeException();
            MainRoadLength = mainRoadLength; ExitCount = exitCount; MinRoomSize = minRoomSize; MaxRoomSize = maxRoomSize;
            EmptyRatio = emptyRatio; Required = Data.List(required);
        }
    }
    public sealed class CellLayoutInfo
    {
        public bool MainRoad { get; }
        public int RoomId { get; }
        public bool RoomEntrance { get; }
        public int Depth { get; }
        public CellLayoutInfo(bool mainRoad, int roomId, bool roomEntrance, int depth)
        { MainRoad = mainRoad; RoomId = roomId; RoomEntrance = roomEntrance; Depth = depth; }
    }

    /// <summary>Main roads first, attached rooms second, positional content last. All decisions use the supplied RNG.</summary>
    internal sealed class RoadRoomMapGenerator
    {
        private readonly MapGenerationDefinition definition;
        private readonly RoadRoomDefinition settings;
        private readonly IRandomSource random;
        private readonly Dictionary<CellPosition, CellPosition[]> graph;
        private readonly Dictionary<CellPosition, CellDefinition> cells;
        private readonly HashSet<CellPosition> selected = new HashSet<CellPosition>();
        private readonly HashSet<CellPosition> road = new HashSet<CellPosition>();
        private readonly HashSet<CellPosition> exits = new HashSet<CellPosition>();
        private readonly HashSet<CellPosition> gateways = new HashSet<CellPosition>();
        private readonly Dictionary<CellPosition, int> rooms = new Dictionary<CellPosition, int>();
        private readonly Dictionary<CellPosition, int> distance = new Dictionary<CellPosition, int>();
        private readonly Dictionary<CellPosition, CellPosition> parent = new Dictionary<CellPosition, CellPosition>();

        private RoadRoomMapGenerator(MapGenerationDefinition definition, IMapTopology topology, IRandomSource random)
        {
            this.definition = definition; settings = definition.RoadRooms; this.random = random;
            cells = definition.FixedCells.ToDictionary(x => x.Position);
            var allowed = new HashSet<CellPosition>(definition.Positions.Where(p => !cells.TryGetValue(p, out var cell) || cell.IsWalkable));
            graph = definition.Positions.Where(allowed.Contains).ToDictionary(p => p, p => topology.Neighbors(p).Where(allowed.Contains).Distinct().ToArray());
        }
        internal static MapDefinition Generate(MapGenerationDefinition definition, IMapTopology topology, IRandomSource random)
        {
            if (topology == null || random == null) throw new ArgumentNullException();
            return new RoadRoomMapGenerator(definition, topology, random).Build();
        }
        private List<CellPosition> Ordered(IEnumerable<CellPosition> values) => values.OrderBy(p => p.Y).ThenBy(p => p.X).ToList();
        private void Shuffle<T>(IList<T> values)
        {
            for (int i = values.Count - 1; i > 0; i--) { int j = random.Next(i + 1); var old = values[i]; values[i] = values[j]; values[j] = old; }
        }
        private void Measure(HashSet<CellPosition> allowed)
        {
            distance.Clear(); parent.Clear(); var queue = new Queue<CellPosition>();
            distance.Add(definition.Entrance, 0); queue.Enqueue(definition.Entrance);
            while (queue.Count > 0)
            {
                var current = queue.Dequeue(); var neighbors = graph[current].Where(allowed.Contains).ToList(); Shuffle(neighbors);
                foreach (var next in neighbors)
                    if (!distance.ContainsKey(next)) { distance.Add(next, distance[current] + 1); parent.Add(next, current); queue.Enqueue(next); }
            }
        }
        private void Connect(CellPosition target)
        {
            if (!distance.ContainsKey(target)) throw new ArgumentException("固定地点无法从入口到达：" + target);
            while (true)
            {
                selected.Add(target); road.Add(target);
                if (target.Equals(definition.Entrance)) break;
                target = parent[target];
            }
        }
        private MapDefinition Build()
        {
            Measure(new HashSet<CellPosition>(graph.Keys));
            selected.Add(definition.Entrance); road.Add(definition.Entrance);
            foreach (var cell in definition.FixedCells.Where(x => x.IsWalkable))
            { Connect(cell.Position); if (cell.Kind == CellKind.Exit) exits.Add(cell.Position); }
            if (definition.Shape.PlaceExit)
            {
                if (exits.Count > 0) throw new ArgumentException("自动出口与固定出口不能同时启用。");
                var candidates = Ordered(distance.Keys.Where(p => distance[p] == settings.MainRoadLength && !cells.ContainsKey(p)));
                Shuffle(candidates);
                for (int i = 0; i < settings.ExitCount; i++)
                {
                    var available = candidates.Where(p => !exits.Contains(p)).ToList();
                    if (available.Count == 0) throw new ArgumentException("范围内没有足够的主路出口位置；请扩大范围或缩短主路长度。");
                    int NewPath(CellPosition p)
                    {
                        int count = 0;
                        while (!p.Equals(definition.Entrance)) { if (!selected.Contains(p)) count++; p = parent[p]; }
                        return count;
                    }
                    var exit = available.OrderByDescending(NewPath).First();
                    Connect(exit); exits.Add(exit); cells.Add(exit, new CellDefinition(exit, CellKind.Exit));
                }
            }
            else if (exits.Count == 0)
            {
                var ends = Ordered(distance.Keys.Where(p => distance[p] == settings.MainRoadLength)); Shuffle(ends);
                if (ends.Count == 0) throw new ArgumentException("生成范围不足以容纳指定主路长度。");
                Connect(ends[0]);
            }
            cells.Add(definition.Entrance, new CellDefinition(definition.Entrance, CellKind.Empty));
            int requiredCount = checked(settings.Required.Sum(x => x.Count));
            int fixedCount = cells.Values.Count(x => x.IsWalkable);
            double estimated = Math.Ceiling(requiredCount / (1 - settings.EmptyRatio)) + fixedCount;
            if (estimated > distance.Count) throw new ArgumentException("必放内容及空地比例需要更大的生成范围。");
            int target = Math.Max(definition.Shape.CellCount, Math.Max(selected.Count, (int)estimated));
            if (target > distance.Count) throw new ArgumentException("可到达范围小于地图最低格数。");
            int roomId = 0;
            while (selected.Count < target)
            {
                int size = Math.Min(target - selected.Count, settings.MinRoomSize + random.Next(settings.MaxRoomSize - settings.MinRoomSize + 1));
                var roots = Ordered(selected.SelectMany(p => graph[p]).Where(p => !selected.Contains(p) && !graph[p].Any(exits.Contains)).Distinct());
                Shuffle(roots);
                bool preferRoad = random.Next(1000000) >= definition.Shape.BranchChance * 1000000;
                if (preferRoad) roots = roots.OrderByDescending(p => graph[p].Any(road.Contains)).ToList();
                List<CellPosition> best = null;
                foreach (var root in roots)
                {
                    var room = new List<CellPosition> { root }; var roomSet = new HashSet<CellPosition>(room);
                    while (room.Count < size)
                    {
                        // A room can touch previous rooms/main roads only through its entrance tile.
                        var frontier = Ordered(room.SelectMany(p => graph[p]).Where(p => !selected.Contains(p) && !roomSet.Contains(p) && !graph[p].Any(selected.Contains)).Distinct());
                        if (frontier.Count == 0) break;
                        Shuffle(frontier);
                        var next = frontier.OrderByDescending(p => graph[p].Count(roomSet.Contains)).First();
                        room.Add(next); roomSet.Add(next);
                    }
                    if (best == null || room.Count > best.Count) best = room;
                    if (room.Count == size) break;
                }
                if (best == null || best.Count < Math.Min(settings.MinRoomSize, target - selected.Count))
                    throw new MapLayoutCapacityException($"房间布局卡住：已放 {selected.Count}/{target} 格，当前最大候选房间 {best?.Count ?? 0} 格，要求至少 {Math.Min(settings.MinRoomSize, target - selected.Count)} 格。");
                gateways.Add(best[0]);
                foreach (var position in best) { selected.Add(position); rooms.Add(position, roomId); }
                roomId++;
            }
            Measure(selected);
            if (distance.Count != selected.Count) throw new InvalidOperationException("房间与主路未连通。");
            PlaceContent();
            var layout = selected.ToDictionary(p => p, p => new CellLayoutInfo(road.Contains(p), rooms.TryGetValue(p, out var id) ? id : -1, gateways.Contains(p), distance[p]));
            return new MapDefinition(definition.Id, definition.Entrance, Ordered(cells.Keys).Select(p => cells[p]), definition.BossGate, definition.RestRatio, layout);
        }

        private void PlaceContent()
        {
            var free = Ordered(selected.Where(p => !cells.ContainsKey(p))); Shuffle(free);
            var rewards = new HashSet<CellPosition>(cells.Values.Where(x => x.Kind == CellKind.Reward).Select(x => x.Position));
            var protectedPath = new HashSet<CellPosition>(road);
            void Protect(CellPosition p)
            {
                while (true) { protectedPath.Add(p); if (p.Equals(definition.Entrance)) break; p = parent[p]; }
            }
            foreach (var cell in cells.Values.Where(x => x.Kind == CellKind.Boss || x.Kind == CellKind.Shop)) Protect(cell.Position);
            var toPlace = new List<CellTemplate>();
            foreach (var required in settings.Required) for (int i = 0; i < required.Count; i++) toPlace.Add(required.Content);
            bool needsBoss = definition.BossGate && !cells.Values.Any(x => x.Kind == CellKind.Boss) && !toPlace.Any(x => x.Kind == CellKind.Boss);
            if (needsBoss)
            {
                var bosses = definition.Pool.Draw(random, 1, predicate: x => x.IsWalkable && x.Kind == CellKind.Boss);
                if (bosses.Count == 0) throw new ArgumentException("要求击败首领，但未配置固定首领、必放首领或首领池。");
                toPlace.Add(bosses[0]);
            }
            if (toPlace.Count > free.Count) throw new ArgumentException("必放内容超过可用房间格数。");
            int wanted = Math.Max(toPlace.Count, (int)Math.Round(free.Count * (1 - settings.EmptyRatio)));
            var filler = new WeightedPool<CellTemplate>(definition.Pool.Entries.Where(x => x.Value.IsWalkable &&
                x.Value.Kind != CellKind.Empty && x.Value.Kind != CellKind.Exit && x.Value.Kind != CellKind.Boss));
            if (wanted > toPlace.Count && filler.Entries.Count == 0) throw new ArgumentException("随机内容池为空，请配置战斗、奖励或事件。");
            while (toPlace.Count < wanted) toPlace.Add(filler.Draw(random, 1)[0]);
            int Priority(CellKind kind) => kind == CellKind.Boss ? 0 : kind == CellKind.Shop ? 1 : kind == CellKind.Reward ? 2 : kind == CellKind.Battle ? 3 : kind == CellKind.Door ? 5 : 4;
            foreach (var content in toPlace.OrderBy(x => Priority(x.Kind)))
            {
                var available = free.Where(p => content.Kind != CellKind.Door || !protectedPath.Contains(p)).ToList();
                if (available.Count == 0) throw new ArgumentException("没有可放置内容的空位；锁门不能占据出口、首领或商店的保护路径。");
                int Score(CellPosition p)
                {
                    int degree = graph[p].Count(selected.Contains);
                    switch (content.Kind)
                    {
                        case CellKind.Boss: return distance[p];
                        case CellKind.Reward: return (degree == 1 ? 100000 : 0) + (!road.Contains(p) ? 10000 : 0) + (rooms.ContainsKey(p) ? 1000 : 0) - degree * 100 + distance[p];
                        case CellKind.Battle: return (gateways.Contains(p) ? 10000 : 0) + (graph[p].Any(rewards.Contains) ? 5000 : 0) + (protectedPath.Contains(p) ? 1000 : 0) + distance[p];
                        case CellKind.Shop: case CellKind.Rest: return -distance[p];
                        default: return 0;
                    }
                }
                var position = available.OrderByDescending(Score).First(); free.Remove(position); cells.Add(position, content.At(position));
                if (content.Kind == CellKind.Reward) rewards.Add(position);
                if (content.Kind == CellKind.Boss || content.Kind == CellKind.Shop) Protect(position);
            }
            foreach (var position in free) cells.Add(position, new CellDefinition(position, CellKind.Empty));
            // Fixed content is never silently deleted to repair a map. Reject authored locks that block required destinations.
            var reached = new HashSet<CellPosition> { definition.Entrance }; var queue = new Queue<CellPosition>(); queue.Enqueue(definition.Entrance);
            while (queue.Count > 0)
                foreach (var next in graph[queue.Dequeue()])
                    if (selected.Contains(next) && cells[next].Kind != CellKind.Door && reached.Add(next)) queue.Enqueue(next);
            if (cells.Values.Any(x => (x.Kind == CellKind.Exit || x.Kind == CellKind.Boss || x.Kind == CellKind.Shop) && !reached.Contains(x.Position)))
                throw new ArgumentException("固定锁门阻断了出口、首领或商店；请调整固定布局。");
        }
    }
}
