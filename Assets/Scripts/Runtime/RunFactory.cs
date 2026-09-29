using System;
using System.Collections.Generic;
using Dungeon.Core;

namespace Dungeon.Runtime
{
    /// <summary>Unity/application boundary: only a NEW run reads the wall clock.</summary>
    public static class RunFactory
    {
        private static readonly object SeedLock = new object();
        private static long lastTicks;
        public static uint CreateTimeSeed()
        {
            lock (SeedLock)
            {
                long ticks = DateTime.UtcNow.Ticks;
                if (ticks <= lastTicks) ticks = lastTicks + 1;
                lastTicks = ticks;
                unchecked
                {
                    ulong value = (ulong)ticks;
                    value = (value ^ (value >> 30)) * 0xbf58476d1ce4e5b9UL;
                    value = (value ^ (value >> 27)) * 0x94d049bb133111ebUL;
                    uint seed = (uint)(value ^ (value >> 31));
                    return seed == 0 ? 0x9E3779B9u : seed;
                }
            }
        }
        public static GameRun StartNew(GameCatalog catalog, CampaignDefinition campaign, WorldCatalog world, IMapTopology topology,
            PlayerProfile profile, string heroId, int difficulty, IDictionary<string, IAiPolicy> policies = null)
        {
            return new GameRun(Guid.NewGuid().ToString("N"), catalog, campaign, world, topology, profile, heroId, difficulty, CreateTimeSeed(), policies);
        }
        public static GameRun Restore(GameCatalog catalog, CampaignDefinition campaign, WorldCatalog world, IMapTopology topology,
            PlayerProfile profile, RunSaveData save, IDictionary<string, IAiPolicy> policies = null) =>
            GameRun.Restore(catalog, campaign, world, topology, profile, save, policies);
    }
}
