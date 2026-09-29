using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Dungeon.Core
{
    public sealed class CampaignDefinition
    {
        public string Id { get; }
        public string ContentVersion { get; }
        public IReadOnlyList<MapDefinition> Regions { get; }
        public IReadOnlyList<MapGenerationDefinition> RegionGenerators { get; }
        public bool GeneratesMaps => RegionGenerators.Count > 0;
        public int RegionCount => GeneratesMaps ? RegionGenerators.Count : Regions.Count;
        public DifficultyRules Difficulties { get; }
        public ExperienceRules Experience { get; }
        public SkillSlotDefinition SkillSlots { get; }
        public IReadOnlyList<RankRewardDefinition> Rewards { get; }
        public IReadOnlyList<TalentDefinition> Talents { get; }
        public int BagCapacity { get; }
        public RewardBundle StartingItems { get; }
        public CampaignDefinition(string id, string contentVersion, IEnumerable<MapDefinition> regions, DifficultyRules difficulties,
            ExperienceRules experience, SkillSlotDefinition skillSlots, IEnumerable<RankRewardDefinition> rewards,
            int bagCapacity, RewardBundle startingItems = null, IEnumerable<TalentDefinition> talents = null,
            IEnumerable<MapGenerationDefinition> regionGenerators = null)
        {
            Id = Data.Id(id); ContentVersion = Data.Id(contentVersion); Regions = Data.List(regions);
            RegionGenerators = Data.List(regionGenerators);
            if ((Regions.Count == 0) == (RegionGenerators.Count == 0)) throw new ArgumentException("Supply either fixed regions or region generation definitions.");
            var ids = GeneratesMaps ? RegionGenerators.Select(x => x.Id) : Regions.Select(x => x.Id);
            if (ids.Distinct().Count() != RegionCount) throw new ArgumentException("Campaign needs unique regions.");
            Difficulties = difficulties ?? throw new ArgumentNullException(nameof(difficulties)); Experience = experience ?? throw new ArgumentNullException(nameof(experience));
            SkillSlots = skillSlots ?? throw new ArgumentNullException(nameof(skillSlots)); Rewards = Data.List(rewards); BagCapacity = Data.NonNegative(bagCapacity);
            StartingItems = startingItems ?? new RewardBundle(); Talents = Data.List(talents);
            if (Talents.Select(x => x.Id).Distinct().Count() != Talents.Count || Talents.Any(x => x.Prerequisites.Any(p => !Talents.Any(t => t.Id == p)))) throw new ArgumentException("Invalid talent references.");
        }
        internal MapDefinition CreateRegion(int index, uint runSeed, IMapTopology topology)
        {
            if (!GeneratesMaps) return Regions[index];
            var generator = RegionGenerators[index];
            return RunSeeds.GenerateRegion(runSeed, generator, index, topology);
        }
    }
    public enum RunPhase { Playing, Victory, Defeat, Abandoned }
    /// <summary>Application-facing owner of one run. All I/O and SO conversion are external.</summary>
    public sealed partial class GameRun
    {
        private readonly GameCatalog catalog;
        private readonly CampaignDefinition definition;
        private readonly WorldCatalog world;
        private readonly IMapTopology topology;
        private readonly IDictionary<string, IAiPolicy> policies;
        private readonly PlayerProfile profile;
        private readonly Dictionary<int, int> kills = new Dictionary<int, int>();
        private readonly HashSet<string> countedEnemies = new HashSet<string>(StringComparer.Ordinal);
        private readonly SeededRandom random;
        public string Id { get; }
        public uint InitialSeed { get; }
        public bool HasInitialSeed { get; }
        public string HeroId => Player.Definition.Id;
        public UnitState Player { get; }
        public Inventory Inventory { get; }
        public SkillBook Skills { get; }
        public DifficultyState Difficulty { get; }
        public GameSession Session { get; private set; }
        public int RegionIndex { get; private set; }
        public int RegionLevel => RegionIndex + 1;
        public RunPhase Phase { get; private set; }
        public int AwardedExperience { get; private set; }
        public bool Settled { get; private set; }
        public RunSaveData Checkpoint { get; private set; }
        public IReadOnlyDictionary<int, int> Kills => new ReadOnlyDictionary<int, int>(kills);
        public GameRun(string runId, GameCatalog catalog, CampaignDefinition definition, WorldCatalog world, IMapTopology topology,
            PlayerProfile profile, string heroId, int difficulty, uint seed, IDictionary<string, IAiPolicy> policies = null)
        {
            Id = Data.Id(runId); this.catalog = catalog ?? throw new ArgumentNullException(nameof(catalog)); this.definition = definition ?? throw new ArgumentNullException(nameof(definition));
            this.world = world; this.topology = topology ?? throw new ArgumentNullException(nameof(topology)); this.profile = profile ?? throw new ArgumentNullException(nameof(profile));
            this.policies = policies == null ? null : new Dictionary<string, IAiPolicy>(policies); Difficulty = definition.Difficulties.At(difficulty); random = new SeededRandom(seed);
            InitialSeed = seed; HasInitialSeed = true;
            if (difficulty > profile.HighestDifficulty) throw new ArgumentException("Difficulty has not been unlocked.");
            Player = catalog.CreateUnit(heroId, Id + "/player", Team.Player, Control.Player); profile.ApplyTalents(Player, definition.Talents);
            Inventory = new Inventory(catalog, definition.BagCapacity); if (!Inventory.TryGrant(definition.StartingItems)) throw new ArgumentException("Starting items exceed bag capacity.");
            Skills = new SkillBook(Player, definition.SkillSlots, Difficulty.SlotSurcharge);
            foreach (var skill in Player.AllSkills.ToArray()) WorldActions.TriggerSkill(Player, skill, Trigger.Equipped);
            StartRegion(0);
        }
        private void StartRegion(int index)
        {
            var nextSession = new GameSession(catalog, Player, Inventory, definition.CreateRegion(index, InitialSeed, topology), topology, random,
                new RewardService(catalog, definition.Rewards, random), policies, Difficulty, world, profile, Skills);
            WorldActions.EnterRegion(Player);
            RegionIndex = index; Session = nextSession;
            BindSession();
            Synchronize();
        }
        private void BindSession()
        {
            var ownedSession = Session;
            ownedSession.BindRun(() => Phase == RunPhase.Playing && ReferenceEquals(Session, ownedSession));
            if (Phase != RunPhase.Playing) ownedSession.CloseCommands();
        }
        public ExplorationResult Explore(CellPosition position)
        {
            Synchronize(); if (Phase != RunPhase.Playing) return new ExplorationResult(ExplorationError.WrongPhase);
            if (Session.Phase == SessionPhase.Exploration && Session.Cells.Any(x => x.Definition.Position.Equals(position) && x.IsRevealed && !x.IsCompleted &&
                (x.Definition.Kind == CellKind.Battle || x.Definition.Kind == CellKind.Boss))) Checkpoint = Capture();
            var result = Session.Explore(position); Synchronize(); return result;
        }
        public ExplorationResult ContinueAfterBattle()
        {
            if (Session.Phase != SessionPhase.Battle || Session.CurrentBattle?.Phase != BattlePhase.Finished ||
                (Phase != RunPhase.Playing && Session.CurrentBattle.Outcome != BattleOutcome.Defeat)) return new ExplorationResult(ExplorationError.WrongPhase);
            CountKills(Session.CurrentBattle);
            var result = Session.ContinueAfterBattle(); Synchronize(); return result;
        }
        private void CountKills(Battle battle)
        {
            if (battle == null || battle.Phase != BattlePhase.Finished) return;
            foreach (var enemy in battle.Units.Where(x => x.Team == Team.Enemy && x.IsDead && !x.IsSummoned))
                if (countedEnemies.Add(enemy.Id)) { kills.TryGetValue(enemy.Definition.Rank, out var count); kills[enemy.Definition.Rank] = checked(count + 1); profile.Discover(enemy.Definition.Id); }
        }
        public bool NextRegion()
        {
            Synchronize(); if (Phase != RunPhase.Playing || Session.Phase != SessionPhase.Exit) return false;
            if (RegionIndex + 1 == definition.RegionCount) { Phase = RunPhase.Victory; return true; }
            StartRegion(RegionIndex + 1); return true;
        }
        public void Synchronize()
        {
            if (Phase != RunPhase.Playing) { Session?.CloseCommands(); return; }
            CountKills(Session?.CurrentBattle); CountKills(Session?.LastCompletedBattle);
            if (Player.IsDead || Session?.Phase == SessionPhase.Defeat) Phase = RunPhase.Defeat;
            else if (Session?.Phase == SessionPhase.Exit && RegionIndex + 1 == definition.RegionCount) Phase = RunPhase.Victory;
            if (Phase != RunPhase.Playing) Session?.CloseCommands();
        }
        public bool Abandon()
        {
            Synchronize(); if (Phase == RunPhase.Victory || (Phase != RunPhase.Playing && Session.CurrentBattle == null)) return false;
            if (Session.CurrentBattle != null && Session.CurrentBattle.Phase != BattlePhase.Finished)
            {
                var result = Session.CurrentBattle.Abandon();
                if (!result.Success) Session.CurrentBattle.TerminateWithoutEffects();
                if (Session.CurrentBattle.Phase != BattlePhase.Finished || Session.CurrentBattle.Units.Any(x => x.InBattle)) return false;
            }
            CountKills(Session.CurrentBattle);
            if (!Session.CloseForAbandon()) return false;
            if (Phase == RunPhase.Playing) Phase = RunPhase.Abandoned;
            return true;
        }
        public bool Settle()
        {
            Synchronize(); if (Phase == RunPhase.Playing || Settled) return false;
            int experience = definition.Experience.Calculate(RegionLevel, kills, Difficulty.Level, Phase == RunPhase.Victory);
            if (!profile.Settle(Id, HeroId, experience, definition.Experience, Difficulty.Level, Phase == RunPhase.Victory, definition.Difficulties.MaximumLevel)) return false;
            AwardedExperience = experience; Settled = true; return true;
        }
    }
    public sealed partial class Battle
    {
        public CommandResult Abandon() => Run(() =>
        {
            if (Phase == BattlePhase.NotStarted || Phase == BattlePhase.Finished) return CommandError.WrongPhase;
            if (Phase == BattlePhase.Faulted)
            {
                TerminateWithoutEffects();
                Emit(EventKind.BattleEnded, amount: (int)Outcome); return CommandError.None;
            }
            Finish(BattleOutcome.Defeat); return CommandError.None;
        });
        internal void TerminateWithoutEffects()
        {
            if (Phase == BattlePhase.Finished && units.All(x => !x.InBattle)) return;
            foreach (var unit in units) { unit.InBattle = false; unit.Armor = 0; unit.Actions = 0; }
            turns.Clear(); intents.Clear(); ActiveUnit = null; Phase = BattlePhase.Finished; Outcome = BattleOutcome.Defeat;
        }
        public CommandError CanUseSkill(string actorId, string skillId, string targetId = null)
        {
            var error = ValidatePlayer(actorId); return error != CommandError.None ? error : ValidateSkill(Player, Player.FindSkill(skillId), Find(targetId), false);
        }
        public IReadOnlyList<string> ValidTargetIds(string skillId)
        {
            var skill = Player.FindSkill(skillId); if (skill == null) return Array.Empty<string>();
            if (!catalog.Skills[skillId].Effects.Any(x => x.Target == Target.SelectedEnemy || x.Target == Target.SelectedAlly)) return Array.Empty<string>();
            return Array.AsReadOnly(units.Where(x => !x.IsDead && ValidTargets(Player, catalog.Skills[skillId].Effects, x)).Select(x => x.Id).ToArray());
        }
    }
}
