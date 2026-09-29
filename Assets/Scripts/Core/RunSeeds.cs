using System;

namespace Dungeon.Core
{
    /// <summary>Stable seed derivation. Never read a clock or consume the gameplay random stream here.</summary>
    public static class RunSeeds
    {
        public const int MapGenerationVersion = 4;
        public static MapDefinition GenerateRegion(uint runSeed, MapGenerationDefinition definition, int regionIndex, IMapTopology topology)
        {
            if (definition == null) throw new ArgumentNullException(nameof(definition));
            TopologyId(topology);
            return MapGenerator.Generate(definition, topology, new SeededRandom(ForRegion(runSeed, definition.Id, regionIndex)));
        }
        public static uint ForRegion(uint runSeed, string regionId, int regionIndex)
        {
            Data.Id(regionId); Data.NonNegative(regionIndex);
            unchecked
            {
                uint hash = 2166136261u;
                void Add(uint value) { for (int i = 0; i < 4; i++) { hash = (hash ^ (byte)value) * 16777619u; value >>= 8; } }
                Add(runSeed); Add((uint)regionIndex);
                foreach (char character in regionId) Add(character);
                return hash == 0 ? 0x9E3779B9u : hash;
            }
        }
        public static string TopologyId(IMapTopology topology)
        {
            if (topology is HexTopology) return "hex-axial-v1";
            if (topology is SquareTopology) return "square-four-v1";
            throw new ArgumentException("Seed-generated campaigns require a known, versioned topology.");
        }
    }
}
