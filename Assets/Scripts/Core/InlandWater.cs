using System;
using System.Collections.Generic;
using System.Linq;

namespace Dungeon.Core
{
    public sealed class InlandWaterDefinition
    {
        public int Rivers { get; }
        public int Lakes { get; }
        public int LakeMinSize { get; }
        public int LakeMaxSize { get; }
        public int Tributaries { get; }
        public int RiverTargetLength { get; }
        public IReadOnlyList<TerrainKind> RiverSurfaces { get; }
        public InlandWaterDefinition(int rivers = 0, int lakes = 0, int lakeMinSize = 2, int lakeMaxSize = 5, int tributaries = 0,
            IEnumerable<TerrainKind> riverSurfaces = null, int riverTargetLength = 12)
        {
            if (rivers < 0 || rivers > 16 || lakes < 0 || lakes > 16 || lakeMinSize < 1 || lakeMaxSize < lakeMinSize || lakeMaxSize > 256 || tributaries < 0 || tributaries > 4)
                throw new ArgumentOutOfRangeException(nameof(rivers), "水系数量或湖泊面积无效。");
            Rivers = rivers; Lakes = lakes; LakeMinSize = lakeMinSize; LakeMaxSize = lakeMaxSize;
            Tributaries = tributaries;
            if(riverTargetLength < 4 || riverTargetLength > 64)throw new ArgumentOutOfRangeException(nameof(riverTargetLength));
            RiverTargetLength = riverTargetLength;
            RiverSurfaces = Data.List((riverSurfaces ?? new[] {TerrainKind.Plains, TerrainKind.Woodland, TerrainKind.Sand, TerrainKind.Snow}).Distinct());
            if(RiverSurfaces.Any(t => !Enum.IsDefined(typeof(TerrainKind),t) || !TerrainRules.IsWalkable(t) || TerrainRules.IsWater(t)))
                throw new ArgumentException("河流底层只能选择非水域、可通行的陆地地形。");
        }
    }

    internal static class InlandWaterGenerator
    {
        private static List<CellPosition> Ordered(IEnumerable<CellPosition> cells) => cells.OrderBy(p => p.Y).ThenBy(p => p.X).ToList();
        internal static void Apply(MapDefinition map, InlandWaterDefinition settings, IMapTopology topology, IRandomSource random,
            Dictionary<CellPosition, TerrainKind> terrain, HashSet<CellPosition> walkable, Dictionary<CellPosition,TerrainKind> surfaces)
        {
            // Water never replaces authored content, entrance, or explicit terrain.
            var protectedCells = new HashSet<CellPosition>(map.Cells.Where(c => c.HasAuthoredTerrain || c.Kind != CellKind.Empty).Select(c => c.Position));
            protectedCells.Add(map.Entrance);
            var available = new HashSet<CellPosition>(terrain.Keys.Where(p => !protectedCells.Contains(p)));
            for (int lake = 0; lake < settings.Lakes; lake++)
            {
                var candidates = new HashSet<CellPosition>(available.Where(p => !walkable.Contains(p)));
                var components = new List<List<CellPosition>>();
                while (candidates.Count > 0)
                {
                    var component = new List<CellPosition>(); var queue = new Queue<CellPosition>();
                    var start = Ordered(candidates)[0]; candidates.Remove(start); queue.Enqueue(start);
                    while (queue.Count > 0)
                    {
                        var p = queue.Dequeue(); component.Add(p);
                        foreach (var n in topology.Neighbors(p)) if (candidates.Remove(n)) queue.Enqueue(n);
                    }
                    if (component.Count >= settings.LakeMinSize) components.Add(component);
                }
                if (components.Count == 0)
                {
                    // Small rooms can leave a thin, disconnected blocking rim.
                    // Extend an exterior lake basin rather than consuming playable cells.
                    var edge = Ordered(available.Where(p => !walkable.Contains(p) && topology.Neighbors(p).Any(n => !terrain.ContainsKey(n))));
                    if(edge.Count == 0) throw new ArgumentException("主题湖泊缺少可扩展的外围空间。");
                    var basin = new List<CellPosition> { edge[random.Next(edge.Count)] };
                    while(basin.Count < settings.LakeMinSize)
                    {
                        var growth = Ordered(basin.SelectMany(p => topology.Neighbors(p)).Where(p => !terrain.ContainsKey(p)).Distinct());
                        if(growth.Count == 0)throw new ArgumentException("无法扩展主题湖泊。");
                        var p = growth[random.Next(growth.Count)];
                        terrain.Add(p,TerrainKind.Mountain);available.Add(p);basin.Add(p);
                    }
                    components.Add(basin);
                }
                var region = components[random.Next(components.Count)];
                int size = settings.LakeMinSize + random.Next(Math.Min(settings.LakeMaxSize, region.Count) - settings.LakeMinSize + 1);
                var allowed = new HashSet<CellPosition>(region); var frontier = new List<CellPosition> { region[random.Next(region.Count)] };
                var visited = new HashSet<CellPosition>(frontier); var placed = new List<CellPosition>();
                while (placed.Count < size)
                {
                    int index = random.Next(frontier.Count); var p = frontier[index]; frontier.RemoveAt(index);
                    terrain[p] = TerrainKind.Lake; placed.Add(p); available.Remove(p);
                    foreach (var n in topology.Neighbors(p)) if (allowed.Contains(n) && visited.Add(n)) frontier.Add(n);
                }
                // Keep separately configured lakes from accidentally joining.
                foreach (var p in placed) foreach (var n in topology.Neighbors(p)) available.Remove(n);
            }
            // Reserve valleys before drawing automatic interior walls. Authored mountains and
            // the exterior rim remain protected; a valley uses lowland art, never a mountain overlay.
            var authoredPositions=new HashSet<CellPosition>(map.Cells.Select(c=>c.Position));
            var valleyGround=new Dictionary<CellPosition,TerrainKind>();
            foreach(var p in Ordered(available))
                if(!authoredPositions.Contains(p)&&terrain[p]==TerrainKind.Mountain&&
                    topology.Neighbors(p).All(n=>terrain.ContainsKey(n)&&topology.Neighbors(n).All(terrain.ContainsKey)))
                {
                    var banks=topology.Neighbors(p).Where(n=>terrain.ContainsKey(n)&&settings.RiverSurfaces.Contains(terrain[n])).ToList();
                    if(banks.Count>0)valleyGround[p]=terrain[banks[0]];
                }
            available.RemoveWhere(p => !valleyGround.ContainsKey(p)&&(!walkable.Contains(p) || !settings.RiverSurfaces.Contains(terrain[p])));
            for (int river = 0; river < settings.Rivers; river++)
            {
                var path = FindRiver(available,terrain,topology,random,settings.RiverTargetLength);
                if(path == null)throw new ArgumentException("河流缺少相连的允许地形空地，无法避开内容格生成河道。");
                // Only the final cell may cut straight outward through an automatically generated rim.
                // It remains blocked water and does not turn the wall into a walkable passage.
                foreach(var p in path)if(valleyGround.TryGetValue(p,out var bank))surfaces[p]=bank;
                ExtendOutlet(path,map,terrain,surfaces,topology,walkable);
                foreach (var p in path)
                {
                    // Fords preserve the original playable graph; no encounter is stranded.
                    terrain[p] = walkable.Contains(p) ? TerrainKind.Ford : TerrainKind.River;
                    available.Remove(p);
                }
                var network=new HashSet<CellPosition>(path);
                for(int branch=0;branch<settings.Tributaries;branch++)
                {
                    List<CellPosition> best=null;
                    // Search all possible confluences; do not overwrite encounters or form loops.
                    foreach(var join in Ordered(network.Where(p=>topology.Neighbors(p).Count(network.Contains)==2)))
                        foreach(var first in topology.Neighbors(join))
                        {
                            if(!available.Contains(first)||topology.Neighbors(first).Count(network.Contains)!=1)continue;
                            var trail=new List<CellPosition>{first};
                            for(int step=0;step<5;step++)
                            {
                                var choices=Ordered(topology.Neighbors(trail[trail.Count-1]).Where(n=>available.Contains(n)&&!trail.Contains(n)&&
                                    !topology.Neighbors(n).Any(network.Contains)&&topology.Neighbors(n).Count(trail.Contains)==1));
                                if(choices.Count==0)break;
                                trail.Add(choices[random.Next(choices.Count)]);
                            }
                            if(trail.Count>=2&&(best==null||trail.Count>best.Count))best=trail;
                        }
                    if(best==null)break;
                    foreach(var p in best)
                    {
                        if(valleyGround.TryGetValue(p,out var bank))surfaces[p]=bank;
                        terrain[p]=walkable.Contains(p)?TerrainKind.Ford:TerrainKind.River;
                        available.Remove(p);network.Add(p);path.Add(p);
                    }
                }
                foreach (var p in path) foreach (var n in topology.Neighbors(p)) available.Remove(n);
            }
        }

        private static List<CellPosition> FindRiver(HashSet<CellPosition> available,
            Dictionary<CellPosition,TerrainKind> terrain,IMapTopology topology,IRandomSource random,int target)
        {
            var neighbors=available.ToDictionary(p=>p,p=>topology.Neighbors(p).ToArray());
            var depth=new Dictionary<CellPosition,int>();var queue=new Queue<CellPosition>();
            foreach(var p in Ordered(terrain.Keys))if(topology.Neighbors(p).Any(n=>!terrain.ContainsKey(n)))
            {depth[p]=0;queue.Enqueue(p);}
            while(queue.Count>0){var p=queue.Dequeue();foreach(var n in topology.Neighbors(p))
                if(terrain.ContainsKey(n)&&!depth.ContainsKey(n)){depth[n]=depth[p]+1;queue.Enqueue(n);}}
            var jitter=Ordered(available).ToDictionary(p=>p,p=>random.Next(100)*.001);
            Func<List<CellPosition>,double> score=path=>
            {
                var start=path[0];var end=path[path.Count-1];
                double dx=end.X-start.X,dy=end.Y-start.Y;
                double span=topology is HexTopology?Math.Max(Math.Abs(dx),Math.Max(Math.Abs(dy),Math.Abs(dx+dy))):Math.Abs(dx)+Math.Abs(dy);
                // Reward crossing the interior and spatial progress, not tracing the perimeter.
                int parallelRim=0;
                for(int i=1;i<path.Count;i++)if(depth[path[i-1]]<=1&&depth[path[i]]<=1)parallelRim++;
                double detour=Math.Max(0,path.Count-1-span*1.5);
                return path.Count*3+span*8+path.Sum(p=>Math.Min(depth[p],3)*2+jitter[p])-parallelRim*10-detour*15;
            };
            var beam=Ordered(available).Select(p=>new List<CellPosition>{p}).ToList();
            List<CellPosition> best=null;double bestScore=double.MinValue;
            for(int length=1;length<=target && beam.Count>0;length++)
            {
                var next=new List<List<CellPosition>>();
                foreach(var path in beam)
                {
                    double value=score(path);
                    int preferredMinimum=Math.Min(6,target);
                    bool reachesMinimum=path.Count>=preferredMinimum;
                    bool bestReachesMinimum=best!=null&&best.Count>=preferredMinimum;
                    if(path.Count>=4&&((reachesMinimum&&!bestReachesMinimum)||
                        (reachesMinimum==bestReachesMinimum&&value>bestScore))) {best=path;bestScore=value;}
                    if(length==target)continue;
                    foreach(var p in neighbors[path[path.Count-1]])
                    {
                        if(!available.Contains(p)||path.Contains(p)||neighbors[p].Count(path.Contains)!=1)continue;
                        if(path.Count>1)
                        {
                            // Keep progressing downstream. Curves may turn across the main flow,
                            // but cannot reverse it just to accumulate additional river cells.
                            var first=path[0];var second=path[1];var last=path[path.Count-1];
                            double ax=second.X-first.X,ay=second.Y-first.Y,bx=p.X-last.X,by=p.Y-last.Y;
                            double forward=topology is HexTopology?ax*bx+ay*by+(ax*by+ay*bx)*.5:ax*bx+ay*by;
                            if(forward<0)continue;
                        }
                        next.Add(new List<CellPosition>(path){p});
                    }
                }
                // Keep several alternatives per endpoint, so one room cannot crowd out another.
                beam=next.GroupBy(p=>p[p.Count-1]).SelectMany(g=>g.OrderByDescending(score).Take(8))
                    .OrderByDescending(score).Take(256).ToList();
            }
            return best;
        }

        private static void ExtendOutlet(List<CellPosition> path,MapDefinition map,Dictionary<CellPosition,TerrainKind> terrain,
            Dictionary<CellPosition,TerrainKind> surfaces,IMapTopology topology,HashSet<CellPosition> walkable)
        {
            var authored=new HashSet<CellPosition>(map.Cells.Select(c=>c.Position));
            // Try either end, without adding a turn along the outer ring.
            for(int side=0;side<2;side++)
            {
                if(side==1)path.Reverse();
                var end=path[path.Count-1];var previous=path[path.Count-2];
                foreach(var gate in topology.Neighbors(end))
                {
                    if(authored.Contains(gate)||!terrain.TryGetValue(gate,out var kind)||kind!=TerrainKind.Mountain||walkable.Contains(gate))continue;
                    if(topology.Neighbors(gate).Any(n=>terrain.TryGetValue(n,out var t)&&(t==TerrainKind.River||t==TerrainKind.Ford||t==TerrainKind.Lake)))continue;
                    if(topology.Neighbors(gate).Count(path.Contains)!=1)continue;
                    var continuation=new CellPosition(gate.X+gate.X-end.X,gate.Y+gate.Y-end.Y);
                    if(terrain.ContainsKey(continuation))continue;
                    // Adjacent hex sides would need an unsupported tight turn.
                    if(topology.Neighbors(previous).Contains(gate))continue;
                    surfaces[gate]=surfaces[end];path.Add(gate);return;
                }
            }
        }
    }
}
