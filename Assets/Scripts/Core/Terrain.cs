using System;
using System.Collections.Generic;
using System.Linq;

namespace Dungeon.Core
{
    public enum TerrainKind { Plains, Woodland, Forest, Mountain, ShallowWater, DeepWater, Sand, Rock, Snow, SnowForest, Ice, River, Ford, Lake }

    public static class TerrainRules
    {
        public static bool IsWalkable(TerrainKind terrain) => terrain != TerrainKind.Mountain && terrain != TerrainKind.DeepWater && terrain != TerrainKind.River && terrain != TerrainKind.Lake;
        public static bool IsWater(TerrainKind terrain) => terrain == TerrainKind.ShallowWater || terrain == TerrainKind.DeepWater || terrain == TerrainKind.River || terrain == TerrainKind.Ford || terrain == TerrainKind.Lake;
    }

    /// <summary>Rules only; art and clocks belong to Unity. Patch size controls clustering, not exact biome quotas.</summary>
    public sealed class TerrainGenerationDefinition
    {
        public double WaterChance { get; }
        public int PatchSize { get; }
        public WeightedPool<TerrainKind> Land { get; }
        public bool ShallowShores { get; }
        public InlandWaterDefinition InlandWater { get; }
        public bool ContinuousRegions { get; }
        public TerrainGenerationDefinition(double waterChance = .45, int patchSize = 12,
            WeightedPool<TerrainKind> land = null, bool shallowShores = true, InlandWaterDefinition inlandWater = null, bool continuousRegions = false)
        {
            if (Data.Finite(waterChance) < 0 || waterChance > 1 || patchSize < 1) throw new ArgumentOutOfRangeException();
            WaterChance = waterChance; PatchSize = patchSize; ShallowShores = shallowShores;
            InlandWater = inlandWater;
            ContinuousRegions = continuousRegions;
            Land = land ?? new WeightedPool<TerrainKind>(new[] {
                new WeightedEntry<TerrainKind>(TerrainKind.Plains, 5),
                new WeightedEntry<TerrainKind>(TerrainKind.Woodland, 2),
                new WeightedEntry<TerrainKind>(TerrainKind.Forest, 3) });
            if (Land.Entries.Count == 0 || Land.Entries.Any(x => !Enum.IsDefined(typeof(TerrainKind), x.Value) ||
                TerrainRules.IsWater(x.Value) || !TerrainRules.IsWalkable(x.Value)))
                throw new ArgumentException("陆地池只允许可通行的陆地；水域由主题水系规则生成。");
        }
    }

    internal static class TerrainMapGenerator
    {
        internal static MapDefinition Apply(MapDefinition source, TerrainGenerationDefinition settings, IMapTopology topology, IRandomSource random)
        {
            var original = source.Cells.ToDictionary(x => x.Position);
            var walkable = new HashSet<CellPosition>(source.Cells.Where(x => x.IsWalkable).Select(x => x.Position));
            var footprint = new HashSet<CellPosition>(original.Keys);
            foreach (var position in Ordered(walkable))
                foreach (var neighbor in topology.Neighbors(position)) footprint.Add(neighbor);

            // Flood the exterior of the footprint. Enclosed gaps become real blocking terrain, not voids.
            long minX = (long)footprint.Min(p => p.X) - 1, maxX = (long)footprint.Max(p => p.X) + 1;
            long minY = (long)footprint.Min(p => p.Y) - 1, maxY = (long)footprint.Max(p => p.Y) + 1;
            if (minX <= int.MinValue || maxX >= int.MaxValue || minY <= int.MinValue || maxY >= int.MaxValue ||
                maxX - minX + 1 > 1000000 || maxY - minY + 1 > 1000000 ||
                (maxX - minX + 1) * (maxY - minY + 1) > 1000000)
                throw new ArgumentException("地形生成范围过大或坐标越界。");
            var exterior = new HashSet<CellPosition>(); var queue = new Queue<CellPosition>();
            var corner = new CellPosition((int)minX, (int)minY); exterior.Add(corner); queue.Enqueue(corner);
            while (queue.Count > 0)
                foreach (var neighbor in topology.Neighbors(queue.Dequeue()))
                    if (neighbor.X >= minX && neighbor.X <= maxX && neighbor.Y >= minY && neighbor.Y <= maxY &&
                        !footprint.Contains(neighbor) && exterior.Add(neighbor)) queue.Enqueue(neighbor);
            for (int y = (int)minY + 1; y < maxY; y++)
                for (int x = (int)minX + 1; x < maxX; x++)
                {
                    var position = new CellPosition(x, y);
                    if (!exterior.Contains(position)) footprint.Add(position);
                }

            var blockers = new HashSet<CellPosition>(footprint.Where(p => !walkable.Contains(p)));
            var terrain = Patches(blockers, settings.PatchSize, topology, random,
                () => settings.InlandWater == null && random.Next(1000000) < settings.WaterChance * 1000000 ? TerrainKind.DeepWater : TerrainKind.Mountain);
            var landRegions=settings.ContinuousRegions?Regions(walkable,settings,topology,random):
                Patches(walkable, settings.PatchSize, topology, random, () => settings.Land.Draw(random, 1)[0]);
            foreach (var pair in landRegions)
                terrain.Add(pair.Key, pair.Value);
            if(!settings.ContinuousRegions && settings.InlandWater != null && settings.Land.Entries.Any(e => e.Value == TerrainKind.Forest) &&
                settings.Land.Entries.Any(e => e.Value == TerrainKind.Woodland) && settings.Land.Entries.Any(e => e.Value == TerrainKind.Plains))
            {
                // Preserve dense cores, soften their perimeter, and introduce small clearings.
                var dense = new HashSet<CellPosition>(terrain.Where(p => p.Value == TerrainKind.Forest).Select(p => p.Key));
                foreach(var p in Ordered(walkable))
                {
                    if(original[p].HasAuthoredTerrain)continue;
                    int neighbors=topology.Neighbors(p).Count(n => dense.Contains(n));
                    int roll=random.Next(100);
                    if(dense.Contains(p))
                        terrain[p] = roll < 12 ? TerrainKind.Plains : neighbors <= 2 || roll < 35 ? TerrainKind.Woodland : TerrainKind.Forest;
                    else if(terrain[p] == TerrainKind.Woodland && neighbors == 0 && roll < 22) terrain[p]=TerrainKind.Plains;
                }
            }
            foreach (var cell in source.Cells.Where(x => x.HasAuthoredTerrain)) terrain[cell.Position] = cell.Terrain;
            if(settings.ContinuousRegions&&settings.Land.Entries.Any(e=>e.Value==TerrainKind.Woodland))
            {
                // A continuous sparse woodland belt separates open ground from dense forest.
                // Authored locations and their clearings keep their original terrain.
                var forest=walkable.Where(p=>terrain[p]==TerrainKind.Forest&&
                    topology.Neighbors(p).Any(n=>terrain.TryGetValue(n,out var t)&&t==TerrainKind.Plains)).ToArray();
                foreach(var p in forest)if(!original[p].HasAuthoredTerrain)terrain[p]=TerrainKind.Woodland;
            }
            var surfaceTerrain = new Dictionary<CellPosition,TerrainKind>(terrain);
            if (settings.InlandWater != null)
                InlandWaterGenerator.Apply(source, settings.InlandWater, topology, random, terrain, walkable, surfaceTerrain);
            footprint.UnionWith(terrain.Keys);
            // Shores consume existing walkable cells, so the exact walkable count and connectivity are preserved.
            if (settings.ShallowShores && settings.InlandWater == null)
                foreach (var position in Ordered(walkable))
                    if (!position.Equals(source.Entrance) && !original[position].HasAuthoredTerrain &&
                        topology.Neighbors(position).Any(p => terrain.TryGetValue(p, out var kind) && kind == TerrainKind.DeepWater))
                        terrain[position] = TerrainKind.ShallowWater;
            if(settings.InlandWater == null || original[source.Entrance].HasAuthoredTerrain)
                terrain[source.Entrance] = original[source.Entrance].Terrain;
            var result = new List<CellDefinition>();
            foreach (var position in Ordered(footprint))
            {
                if (original.TryGetValue(position, out var cell))
                    result.Add(new CellDefinition(position, cell.Kind, cell.Enemies, cell.Reward, cell.RequiredKey, cell.ContentId, terrain[position]));
                else result.Add(new CellDefinition(position, CellKind.Empty, terrain: terrain[position]));
            }
            return new MapDefinition(source.Id, source.Entrance, result, source.RequireBossForExit, source.RestRatio, source.Layout.ToDictionary(x => x.Key, x => x.Value),surfaceTerrain);
        }

        private static IEnumerable<CellPosition> Ordered(IEnumerable<CellPosition> positions) => positions.OrderBy(p => p.Y).ThenBy(p => p.X);

        private static Dictionary<CellPosition,TerrainKind> Regions(HashSet<CellPosition> positions,
            TerrainGenerationDefinition settings,IMapTopology topology,IRandomSource random)
        {
            // One smooth spatial field, rather than independently thinning individual hexes.
            // Sorting the field into weighted bands yields connected plains / woodland / forest regions.
            int seed=random.Next(1000000);double angle=random.Next(360)*Math.PI/180;
            double scale=Math.Max(3,Math.Sqrt(settings.PatchSize)*2);
            double dx=Math.Cos(angle),dy=Math.Sin(angle);
            double Hash(int x,int y)
            {
                unchecked {uint h=(uint)seed^(uint)x*374761393u^(uint)y*668265263u;h=(h^(h>>13))*1274126177u;return (h^(h>>16))/(double)uint.MaxValue;}
            }
            double Field(CellPosition p)
            {
                double x=topology is HexTopology?p.X+p.Y*.5:p.X;
                double y=topology is HexTopology?p.Y*.8660254:p.Y;
                double u=x/scale,v=y/scale;int ix=(int)Math.Floor(u),iy=(int)Math.Floor(v);
                double tx=u-ix,ty=v-iy;tx=tx*tx*(3-2*tx);ty=ty*ty*(3-2*ty);
                double a=Hash(ix,iy)*(1-tx)+Hash(ix+1,iy)*tx;
                double b=Hash(ix,iy+1)*(1-tx)+Hash(ix+1,iy+1)*tx;
                return (a*(1-ty)+b*ty)*.7+(u*dx+v*dy)*.3;
            }
            var ordered=positions.OrderBy(Field).ThenBy(p=>p.Y).ThenBy(p=>p.X).ToArray();
            var entries=settings.Land.Entries.OrderBy(e=>e.Value==TerrainKind.Plains?0:e.Value==TerrainKind.Woodland?1:e.Value==TerrainKind.Forest?2:3+(int)e.Value).ToArray();
            double total=entries.Sum(e=>(double)e.Weight);int index=0;double cumulative=entries[0].Weight;
            var result=new Dictionary<CellPosition,TerrainKind>();
            for(int i=0;i<ordered.Length;i++)
            {
                double sample=(i+.5)/ordered.Length*total;
                while(index<entries.Length-1&&sample>=cumulative)cumulative+=entries[++index].Weight;
                result[ordered[i]]=entries[index].Value;
            }
            // Remove single-cell speckles only, preserving intentional region boundaries.
            var snapshot=new Dictionary<CellPosition,TerrainKind>(result);
            foreach(var p in Ordered(positions))
            {
                var adjacent=topology.Neighbors(p).Where(snapshot.ContainsKey).Select(n=>snapshot[n]).ToArray();
                if(adjacent.Length<3||adjacent.Contains(snapshot[p]))continue;
                var majority=adjacent.GroupBy(t=>t).OrderByDescending(g=>g.Count()).ThenBy(g=>g.Key).First();
                if(majority.Count()>=3)result[p]=majority.Key;
            }
            return result;
        }

        private static Dictionary<CellPosition, TerrainKind> Patches(HashSet<CellPosition> positions, int patchSize,
            IMapTopology topology, IRandomSource random, Func<TerrainKind> choose)
        {
            var shuffled = Ordered(positions).ToArray();
            for (int i = shuffled.Length - 1; i > 0; i--)
            {
                int j = random.Next(i + 1); var old = shuffled[i]; shuffled[i] = shuffled[j]; shuffled[j] = old;
            }
            var result = new Dictionary<CellPosition, TerrainKind>(); var queue = new Queue<CellPosition>();
            for (int i = 0; i < (shuffled.Length + patchSize - 1) / patchSize; i++)
            { result.Add(shuffled[i], choose()); queue.Enqueue(shuffled[i]); }
            void Grow()
            {
                while (queue.Count > 0)
                {
                    var current = queue.Dequeue();
                    foreach (var next in topology.Neighbors(current))
                        if (positions.Contains(next) && !result.ContainsKey(next))
                        { result.Add(next, result[current]); queue.Enqueue(next); }
                }
            }
            Grow();
            foreach (var position in Ordered(positions))
                if (!result.ContainsKey(position)) { result.Add(position, choose()); queue.Enqueue(position); Grow(); }
            return result;
        }
    }
}
