using System;
using System.Collections.Generic;
using System.Linq;

namespace Dungeon.Core
{
    /// <summary>Internal workspace derived from content, never an authored map width/height.</summary>
    public static class AutomaticMapArea
    {
        public static List<CellPosition> Create(int cellCount, CellPosition entrance,
            IReadOnlyList<CellDefinition> fixedCells, RoadRoomDefinition rooms, int expansion = 0)
        {
            if (cellCount < 2) throw new ArgumentException("地图格数至少为 2。");
            if (expansion < 0 || expansion > 2) throw new ArgumentOutOfRangeException(nameof(expansion));
            long required = rooms == null ? 0 : rooms.Required.Sum(x => (long)x.Count);
            double contentBudget = rooms == null ? 0 : Math.Ceiling(required / (1 - rooms.EmptyRatio)) + fixedCells.Count + 2;
            double budget = Math.Max(cellCount, contentBudget);
            int roadReach = rooms == null ? 0 : rooms.MainRoadLength;
            // O(cellCount) initial area, plus enough room for the requested main road.
            double extent = Math.Max(Math.Ceiling(Math.Sqrt(budget * 2)), roadReach + 2);
            extent *= 1 << expansion;
            if (extent > 100000) throw new ArgumentException("配置所需生成空间过大，请减少格数或主路长度。");
            long radius = (long)extent;
            long minX = (long)entrance.X - radius, maxX = (long)entrance.X + radius;
            long minY = (long)entrance.Y - radius, maxY = (long)entrance.Y + radius;
            foreach (var cell in fixedCells)
            {
                minX = Math.Min(minX, (long)cell.Position.X - radius);
                maxX = Math.Max(maxX, (long)cell.Position.X + radius);
                minY = Math.Min(minY, (long)cell.Position.Y - radius);
                maxY = Math.Max(maxY, (long)cell.Position.Y + radius);
            }
            long width = maxX - minX + 1, height = maxY - minY + 1;
            // Resource protection, not a gameplay dimension setting.
            if (width > 262144 || height > 262144 || width * height > 262144 || minX <= int.MinValue || maxX >= int.MaxValue || minY <= int.MinValue || maxY >= int.MaxValue)
                throw new ArgumentException("自动生成空间超过安全预算；请减少格数、主路长度，或缩短固定地块与入口之间的距离。");
            long count = width * height;
            var positions = new List<CellPosition>((int)count);
            for (long y = minY; y <= maxY; y++)
                for (long x = minX; x <= maxX; x++) positions.Add(new CellPosition((int)x, (int)y));
            return positions;
        }

        public static List<CellPosition> Compact(int count, CellPosition entrance,
            IReadOnlyList<CellDefinition> fixedCells, IMapTopology topology)
        {
            if (count < 2 || count > 262144) throw new ArgumentException("地图格数须在 2～262144 之间。");
            var selected = new HashSet<CellPosition> { entrance };
            var ordered = new List<CellPosition> { entrance };
            // Reserve short geometric connections to authored cells before filling the compact footprint.
            long Distance(CellPosition a, CellPosition b)
            {
                long x = (long)a.X - b.X, y = (long)a.Y - b.Y;
                return topology is HexTopology ? Math.Max(Math.Max(Math.Abs(x), Math.Abs(y)), Math.Abs(x + y)) : Math.Abs(x) + Math.Abs(y);
            }
            foreach (var cell in fixedCells.OrderBy(c => c.Position.Y).ThenBy(c => c.Position.X))
            {
                var p = cell.Position;
                if (Distance(p, entrance) >= count) throw new ArgumentException("连接固定地块需要更多格子，请增加格数或调整固定坐标。");
                while (!selected.Contains(p))
                {
                    selected.Add(p); ordered.Add(p);
                    if (selected.Count > count) throw new ArgumentException("固定地块及其连接超过配置格数。");
                    p = topology.Neighbors(p).OrderBy(n => Distance(n, entrance)).First();
                }
            }
            var queue = new Queue<CellPosition>(ordered);
            while (ordered.Count < count)
                foreach (var p in topology.Neighbors(queue.Dequeue()))
                    if (selected.Add(p))
                    {
                        ordered.Add(p); queue.Enqueue(p);
                        if (ordered.Count == count) break;
                    }
            return ordered;
        }
    }

    internal sealed class MapLayoutCapacityException : ArgumentException
    {
        internal MapLayoutCapacityException(string message) : base(message) { }
    }
}
