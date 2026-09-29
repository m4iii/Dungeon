using System;
using System.Collections.Generic;
using System.Linq;

namespace Dungeon.Core
{
    /// <summary>Positions define the allowed envelope; CellCount counts walkable cells, including entrance/exit.</summary>
    public sealed class MapShapeDefinition
    {
        public int CellCount { get; }
        public double Compactness { get; }
        public double BranchChance { get; }
        public bool PlaceExit { get; }
        public MapShapeDefinition(int cellCount, double compactness = .2, double branchChance = .4, bool placeExit = true)
        {
            if (cellCount < 2 || Data.Finite(compactness) < 0 || compactness > 1 || Data.Finite(branchChance) < 0 || branchChance > 1)
                throw new ArgumentOutOfRangeException();
            CellCount = cellCount; Compactness = compactness; BranchChance = branchChance; PlaceExit = placeExit;
        }
    }

    internal static class ConnectedMapGenerator
    {
        internal static MapDefinition Generate(MapGenerationDefinition definition, IMapTopology topology, IRandomSource random)
        {
            if (topology == null || random == null) throw new ArgumentNullException();
            var settings = definition.Shape;
            var fixedCells = definition.FixedCells.ToDictionary(x => x.Position);
            var allowed = new HashSet<CellPosition>(definition.Positions.Where(p => !fixedCells.TryGetValue(p, out var cell) || cell.IsWalkable));
            var graph = allowed.ToDictionary(p => p, p => topology.Neighbors(p).Where(allowed.Contains).Distinct().ToArray());
            // A single randomized BFS supplies connected paths to every authored landmark, without retrying seeds.
            var parent = new Dictionary<CellPosition, CellPosition>();
            var reachable = new HashSet<CellPosition> { definition.Entrance };
            var queue = new Queue<CellPosition>(); queue.Enqueue(definition.Entrance);
            while (queue.Count > 0)
            {
                var current = queue.Dequeue(); var neighbors = (CellPosition[])graph[current].Clone();
                for (int i = neighbors.Length - 1; i > 0; i--) { int j = random.Next(i + 1); var old = neighbors[i]; neighbors[i] = neighbors[j]; neighbors[j] = old; }
                foreach (var next in neighbors)
                    if (reachable.Add(next)) { parent.Add(next, current); queue.Enqueue(next); }
            }
            if (settings.CellCount > reachable.Count) throw new ArgumentException("目标地块数量超过入口可到达的生成范围，请增大范围或减少固定障碍。");
            var selected = new HashSet<CellPosition> { definition.Entrance };
            foreach (var cell in definition.FixedCells.Where(x => x.IsWalkable))
            {
                if (!reachable.Contains(cell.Position)) throw new ArgumentException("固定地块无法与入口连通：" + cell.Position);
                var current = cell.Position;
                while (selected.Add(current)) current = parent[current];
            }
            if (selected.Count > settings.CellCount) throw new ArgumentException("连接固定地块所需格数超过目标数量，请增加地块数量或调整固定地块。");

            var frontier = new List<CellPosition>(); var inFrontier = new HashSet<CellPosition>();
            void Expand(CellPosition p)
            {
                foreach (var neighbor in graph[p]) if (!selected.Contains(neighbor) && inFrontier.Add(neighbor)) frontier.Add(neighbor);
            }
            // Never use hash iteration order for random decisions or result ordering.
            foreach (var p in definition.Positions.Where(selected.Contains)) Expand(p);
            var tip = definition.Entrance;
            while (selected.Count < settings.CellCount)
            {
                if (frontier.Count == 0) throw new InvalidOperationException("Connected frontier exhausted.");
                var candidates = frontier;
                if (random.Next(1000000) >= settings.BranchChance * 1000000)
                {
                    var adjacent = new HashSet<CellPosition>(graph[tip]);
                    var continuation = frontier.Where(adjacent.Contains).ToList();
                    if (continuation.Count > 0) candidates = continuation;
                }
                int Weight(CellPosition p)
                {
                    int neighbors = Math.Max(1, graph[p].Count(selected.Contains));
                    return Math.Max(1, Data.Int(1000 * (settings.Compactness * neighbors + (1 - settings.Compactness) / (neighbors * neighbors * neighbors))));
                }
                int roll = random.Next(candidates.Sum(Weight)); var chosen = candidates[0];
                foreach (var candidate in candidates) { chosen = candidate; roll -= Weight(candidate); if (roll < 0) break; }
                selected.Add(chosen); frontier.Remove(chosen); inFrontier.Remove(chosen); Expand(chosen); tip = chosen;
            }

            var cells = definition.FixedCells.ToDictionary(x => x.Position);
            cells.Add(definition.Entrance, new CellDefinition(definition.Entrance, CellKind.Empty));
            if (settings.PlaceExit)
            {
                if (definition.FixedCells.Any(x => x.Kind == CellKind.Exit)) throw new ArgumentException("自动出口与固定出口不能同时启用。");
                var distances = new Dictionary<CellPosition, int> { [definition.Entrance] = 0 }; queue.Enqueue(definition.Entrance);
                while (queue.Count > 0)
                {
                    var current = queue.Dequeue();
                    foreach (var neighbor in graph[current])
                        if (selected.Contains(neighbor) && !distances.ContainsKey(neighbor)) { distances.Add(neighbor, distances[current] + 1); queue.Enqueue(neighbor); }
                }
                var exits = definition.Positions.Where(p => selected.Contains(p) && !cells.ContainsKey(p)).ToArray();
                if (exits.Length == 0) throw new ArgumentException("没有可放置自动出口的非固定地块，请增加地块数量。");
                int farthest = exits.Max(p => distances[p]); var choices = exits.Where(p => distances[p] == farthest).ToArray();
                var exit = choices[random.Next(choices.Length)]; cells.Add(exit, new CellDefinition(exit, CellKind.Exit));
            }
            // Terrain is applied after the walkable graph is complete; never cut its paths with random blockers.
            var pool = new WeightedPool<CellTemplate>(definition.Pool.Entries.Where(x => x.Value.IsWalkable));
            foreach (var position in definition.Positions.Where(p => selected.Contains(p) && !cells.ContainsKey(p)))
            {
                var sample = pool.Draw(random, 1);
                if (sample.Count == 0) throw new ArgumentException("不规则地图需要至少一种非障碍随机地块。");
                cells.Add(position, sample[0].At(position));
            }
            return new MapDefinition(definition.Id, definition.Entrance, definition.Positions.Where(cells.ContainsKey).Select(p => cells[p]), definition.BossGate, definition.RestRatio);
        }
    }
}
