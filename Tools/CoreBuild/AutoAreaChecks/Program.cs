using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Dungeon.Core;

static void Check(bool value, string message) { if (!value) throw new Exception(message); }
static string Signature(MapDefinition map) => string.Join(";", map.Cells.Select(c => $"{c.Position}:{c.Kind}"));
var watch = Stopwatch.StartNew();
int generated = 0;
foreach (IMapTopology topology in new IMapTopology[] { new HexTopology(), new SquareTopology() })
foreach (int count in new[] { 30, 60, 160 })
foreach (uint seed in Enumerable.Range(0, 30).Select(x => (uint)x))
{
    var origin = new CellPosition(12, 2);
    var required = new[] { new RequiredMapContent(new CellTemplate(CellKind.Reward), 5),
        new RequiredMapContent(new CellTemplate(CellKind.Event, contentId: "test-event"), 3) };
    var rooms = new RoadRoomDefinition(12, 1, 5, 9, .45, required);
    var positions = AutomaticMapArea.Create(count, origin, Array.Empty<CellDefinition>(), rooms);
    var definition = new MapGenerationDefinition("test", positions, origin, Array.Empty<CellDefinition>(),
        new WeightedPool<CellTemplate>(new[] { new WeightedEntry<CellTemplate>(new CellTemplate(CellKind.Rest)) }),
        false, 1, new MapShapeDefinition(count), null, rooms, true);
    var map = RunSeeds.GenerateRegion(seed, definition, 0, topology);
    Check(map.Cells.Count(c => c.IsWalkable) >= count, "Minimum count lost");
    Check(map.Cells.Count(c => c.Kind == CellKind.Reward) == 5, "Required rewards lost");
    Check(map.Cells.Count(c => c.Kind == CellKind.Event) == 3, "Required events lost");
    Check(map.Cells.Count(c => c.Kind == CellKind.Exit) == 1, "Exit lost");
    var selected = map.Cells.Where(c => c.IsWalkable).Select(c => c.Position).ToHashSet();
    var seen = new HashSet<CellPosition> { origin }; var queue = new Queue<CellPosition>(); queue.Enqueue(origin);
    while (queue.Count > 0) foreach (var next in topology.Neighbors(queue.Dequeue()))
        if (selected.Contains(next) && seen.Add(next)) queue.Enqueue(next);
    Check(seen.Count == selected.Count, "Disconnected map");
    if (seed < 3) Check(Signature(map) == Signature(RunSeeds.GenerateRegion(seed, definition, 0, topology)), "Not deterministic");
    generated++;
}
foreach (IMapTopology topology in new IMapTopology[] { new HexTopology(), new SquareTopology() })
{
    var fixedCells = new[] { new CellDefinition(new CellPosition(-3, -2), CellKind.Rest) };
    var compact = AutomaticMapArea.Compact(60, new CellPosition(0, 0), fixedCells, topology);
    Check(compact.Count == 60 && compact.Distinct().Count() == 60, "Compact count wrong");
    Check(compact.Contains(fixedCells[0].Position), "Negative fixed coordinate lost");
}
var longRoad = new RoadRoomDefinition(80);
var expanded = AutomaticMapArea.Create(100, new CellPosition(0, 0), Array.Empty<CellDefinition>(), longRoad);
Check(expanded.Contains(new CellPosition(80, 0)), "Old width cap remains");
// An artificially cramped initial workspace must recover through spatial expansion.
var cramped = new List<CellPosition>();
for (int y = -4; y <= 4; y++) for (int x = -4; x <= 4; x++) cramped.Add(new CellPosition(x, y));
var retryRooms = new RoadRoomDefinition(3, 1, 5, 9);
var retryPool = new WeightedPool<CellTemplate>(new[] { new WeightedEntry<CellTemplate>(new CellTemplate(CellKind.Rest)) });
bool recovered = false;
for (uint seed = 0; seed < 20 && !recovered; seed++)
{
    var fixedArea = new MapGenerationDefinition("retry", cramped, new CellPosition(0, 0), Array.Empty<CellDefinition>(),
        retryPool, false, 1, new MapShapeDefinition(70), null, retryRooms);
    try { MapGenerator.Generate(fixedArea, new HexTopology(), new SeededRandom(seed)); }
    catch (ArgumentException)
    {
        var autoArea = new MapGenerationDefinition("retry", cramped, new CellPosition(0, 0), Array.Empty<CellDefinition>(),
            retryPool, false, 1, new MapShapeDefinition(70), null, retryRooms, true);
        var map = MapGenerator.Generate(autoArea, new HexTopology(), new SeededRandom(seed));
        Check(map.Cells.Count >= 70, "Expanded workspace lost cells");
        Check(map.Cells.Any(c => !cramped.Contains(c.Position)), "Expansion was not exercised");
        recovered = true;
    }
}
Check(recovered, "Missing regression coverage for cramped room layout");
bool rejected = false;
try { AutomaticMapArea.Create(int.MaxValue, new CellPosition(0, 0), Array.Empty<CellDefinition>(), null); }
catch (ArgumentException) { rejected = true; }
Check(rejected, "Oversized allocation not rejected");
Console.WriteLine($"PASS: {generated} generated maps, deterministic replay, compact counts, negative coordinates, long road, cramped-area recovery, allocation guard. {watch.ElapsedMilliseconds} ms");
