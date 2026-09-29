using System;
using System.Collections.Generic;
using System.Linq;

namespace Dungeon.Core
{
    public readonly struct CellPosition : IEquatable<CellPosition>
    {
        public int X { get; }
        public int Y { get; }
        public CellPosition(int x, int y) { X = x; Y = y; }
        public bool Equals(CellPosition other) => X == other.X && Y == other.Y;
        public override bool Equals(object obj) => obj is CellPosition other && Equals(other);
        public override int GetHashCode() => unchecked(X * 397 ^ Y);
        public override string ToString() => X + "," + Y;
    }
    public interface IMapTopology { IEnumerable<CellPosition> Neighbors(CellPosition position); }
    public sealed class SquareTopology : IMapTopology
    {
        public IEnumerable<CellPosition> Neighbors(CellPosition p)
        {
            yield return new CellPosition(p.X, p.Y + 1); yield return new CellPosition(p.X + 1, p.Y);
            yield return new CellPosition(p.X, p.Y - 1); yield return new CellPosition(p.X - 1, p.Y);
        }
    }
    /// <summary>Axial (q,r) coordinates. Conversion from Unity offset/world coordinates belongs to the adapter.</summary>
    public sealed class HexTopology : IMapTopology
    {
        public IEnumerable<CellPosition> Neighbors(CellPosition p)
        {
            yield return new CellPosition(p.X + 1, p.Y); yield return new CellPosition(p.X + 1, p.Y - 1);
            yield return new CellPosition(p.X, p.Y - 1); yield return new CellPosition(p.X - 1, p.Y);
            yield return new CellPosition(p.X - 1, p.Y + 1); yield return new CellPosition(p.X, p.Y + 1);
        }
    }
    public enum CellKind { Empty, Battle, Boss, Reward, Door, Blocked, Rest, Exit, Shop, SkillChoice, RelicChoice, Event }
    public sealed class CellDefinition
    {
        public CellPosition Position { get; }
        public CellKind Kind { get; }
        public TerrainKind Terrain { get; }
        public bool HasAuthoredTerrain { get; }
        public bool IsWalkable => Kind != CellKind.Blocked && TerrainRules.IsWalkable(Terrain);
        public string RequiredKey { get; }
        public IReadOnlyList<string> Enemies { get; }
        public RewardBundle Reward { get; }
        public string ContentId { get; }
        public CellDefinition(CellPosition position, CellKind kind, IEnumerable<string> enemies = null, RewardBundle reward = null, string requiredKey = null, string contentId = null, TerrainKind? terrain = null)
        {
            HasAuthoredTerrain = terrain.HasValue;
            Terrain = Data.EnumValue(terrain ?? (kind == CellKind.Blocked ? TerrainKind.Mountain : TerrainKind.Plains));
            if (!TerrainRules.IsWalkable(Terrain) && kind != CellKind.Blocked && kind != CellKind.Empty)
                throw new ArgumentException("阻挡地形不能承载战斗、奖励或交互内容。");
            Position = position; Kind = Data.EnumValue(kind); Enemies = Data.List(enemies); Reward = reward ?? new RewardBundle(); RequiredKey = requiredKey;
            if ((kind == CellKind.Battle || kind == CellKind.Boss) != (Enemies.Count > 0)) throw new ArgumentException("Only battle cells require enemy definitions.");
            if (requiredKey != null && kind != CellKind.Door && kind != CellKind.Reward) throw new ArgumentException("Only doors and reward cells use keys.");
            ContentId = contentId;
            if (kind == CellKind.Shop || kind == CellKind.SkillChoice || kind == CellKind.RelicChoice || kind == CellKind.Event) Data.Id(contentId);
            if (contentId != null && (Reward.Coins != 0 || Reward.Items.Count != 0)) throw new ArgumentException("Interactive rewards belong to the offer/event, or a separate reward cell.");
        }
    }
    public sealed class MapDefinition
    {
        public string Id { get; }
        public CellPosition Entrance { get; }
        public IReadOnlyList<CellDefinition> Cells { get; }
        public bool RequireBossForExit { get; }
        public double RestRatio { get; }
        public IReadOnlyDictionary<CellPosition, CellLayoutInfo> Layout { get; }
        public IReadOnlyDictionary<CellPosition, TerrainKind> SurfaceTerrain { get; }
        public MapDefinition(string id, CellPosition entrance, IEnumerable<CellDefinition> cells, bool requireBossForExit = true, double restRatio = 1,
            IDictionary<CellPosition, CellLayoutInfo> layout = null, IDictionary<CellPosition,TerrainKind> surfaceTerrain = null)
        {
            SurfaceTerrain = new System.Collections.ObjectModel.ReadOnlyDictionary<CellPosition,TerrainKind>(
                surfaceTerrain == null ? new Dictionary<CellPosition,TerrainKind>() : new Dictionary<CellPosition,TerrainKind>(surfaceTerrain));
            Layout = new System.Collections.ObjectModel.ReadOnlyDictionary<CellPosition, CellLayoutInfo>(
                layout == null ? new Dictionary<CellPosition, CellLayoutInfo>() : new Dictionary<CellPosition, CellLayoutInfo>(layout));
            Id = Data.Id(id); Entrance = entrance; Cells = Data.List(cells); RequireBossForExit = requireBossForExit;
            if (Data.Finite(restRatio) < 0 || restRatio > 1) throw new ArgumentOutOfRangeException(nameof(restRatio)); RestRatio = restRatio;
            if (Cells.Select(x => x.Position).Distinct().Count() != Cells.Count || !Cells.Any(x => x.Position.Equals(entrance) && x.Kind == CellKind.Empty && x.IsWalkable))
                throw new ArgumentException("Map requires unique coordinates and an empty entrance.");
            if (requireBossForExit && !Cells.Any(x => x.Kind == CellKind.Boss)) throw new ArgumentException("A boss-gated map needs a boss.");
        }
    }
    public sealed class CellState
    {
        public CellDefinition Definition { get; }
        public bool IsRevealed { get; internal set; }
        public bool IsContentKnown { get; internal set; }
        public bool IsCompleted { get; internal set; }
        internal CellState(CellDefinition definition) { Definition = definition; }
    }
    public enum ExplorationError { None, WrongPhase, Hidden, Blocked, Completed, MissingKey, BossAlive, InventoryFull, BattleFailed }
    public enum SessionPhase { Exploration, Battle, Exit, Defeat }
    public sealed class ExplorationResult
    {
        public ExplorationError Error { get; }
        public bool Success => Error == ExplorationError.None;
        public IReadOnlyList<GameEvent> BattleEvents { get; }
        internal ExplorationResult(ExplorationError error, IEnumerable<GameEvent> events = null) { Error = error; BattleEvents = Data.List(events); }
    }
    /// <summary>Owns the exploration -> encounter -> settlement transition; callers cannot complete a battle tile by supplying a forged outcome.</summary>
    public sealed partial class GameSession
    {
        private readonly GameCatalog catalog;
        private readonly MapDefinition map;
        private readonly IMapTopology topology;
        private readonly IRandomSource random;
        private readonly RewardService rewards;
        private readonly Dictionary<CellPosition, CellState> cells;
        private readonly Dictionary<CellPosition, UnitState[]> encounters = new Dictionary<CellPosition, UnitState[]>();
        private readonly Dictionary<CellPosition, UnitState[]> encounterSummons = new Dictionary<CellPosition, UnitState[]>();
        private Func<bool> operationAllowed;
        private bool commandsClosed;
        private bool CanExplore => !commandsClosed && (operationAllowed?.Invoke() ?? true) && Phase == SessionPhase.Exploration && !Player.IsDead;
        internal void BindRun(Func<bool> allowed) { operationAllowed = allowed ?? throw new ArgumentNullException(nameof(allowed)); }
        internal void CloseCommands() { commandsClosed = true; interaction = null; }
        private readonly IDictionary<string, IAiPolicy> aiPolicies;
        private CellState pending;
        public UnitState Player { get; }
        public Inventory Inventory { get; }
        public SessionPhase Phase { get; private set; }
        public Battle CurrentBattle { get; private set; }
        public MapDefinition Map => map;
        private readonly DifficultyState difficulty;
        public IReadOnlyList<CellState> Cells { get; }
        public GameSession(GameCatalog catalog, UnitState player, Inventory inventory, MapDefinition map, IMapTopology topology,
            IRandomSource random, RewardService rewards, IDictionary<string, IAiPolicy> aiPolicies = null, DifficultyState difficulty = null,
            WorldCatalog world = null, PlayerProfile profile = null, SkillBook skillBook = null)
        {
            this.catalog = catalog ?? throw new ArgumentNullException(nameof(catalog)); Player = player ?? throw new ArgumentNullException(nameof(player));
            Inventory = inventory ?? throw new ArgumentNullException(nameof(inventory)); this.map = map ?? throw new ArgumentNullException(nameof(map));
            this.topology = topology ?? throw new ArgumentNullException(nameof(topology)); this.random = random ?? throw new ArgumentNullException(nameof(random));
            this.rewards = rewards ?? throw new ArgumentNullException(nameof(rewards)); this.aiPolicies = aiPolicies == null ? null : new Dictionary<string, IAiPolicy>(aiPolicies);
            this.difficulty = difficulty; this.world = world; this.profile = profile; this.skillBook = skillBook;
            if (skillBook != null && !ReferenceEquals(skillBook.Owner, player)) throw new ArgumentException("Skill book belongs to another unit.");
            if (player.Catalog != catalog || inventory.Catalog != catalog || rewards.Catalog != catalog || player.InBattle || player.IsDead || player.Control != Control.Player || player.Team != Team.Player)
                throw new ArgumentException("Invalid session player or inventory.");
            foreach (var cell in map.Cells)
            {
                ValidateContent(cell);
                foreach (var enemy in cell.Enemies) if (!catalog.Units.ContainsKey(enemy)) throw new ArgumentException("Unknown enemy: " + enemy);
                if (cell.Enemies.Sum(x => catalog.Units[x].Size) > catalog.Rules.TeamCapacity) throw new ArgumentException("Encounter exceeds capacity.");
                foreach (var item in cell.Reward.Items.Keys.Concat(cell.RequiredKey == null ? Array.Empty<string>() : new[] { cell.RequiredKey }))
                    if (!catalog.Items.ContainsKey(item)) throw new ArgumentException("Unknown map item: " + item);
                if (cell.RequiredKey != null && !catalog.Items[cell.RequiredKey].IsCurrency) throw new ArgumentException("Keys must be currency items that do not occupy slots.");
            }
            cells = map.Cells.ToDictionary(x => x.Position, x => new CellState(x)); Cells = Array.AsReadOnly(map.Cells.Select(x => cells[x.Position]).ToArray());
            var entrance = cells[map.Entrance]; entrance.IsRevealed = true; Complete(entrance);
        }
        public ExplorationResult Explore(CellPosition position)
        {
            if (!CanExplore) return new ExplorationResult(ExplorationError.WrongPhase);
            if (!cells.TryGetValue(position, out var cell) || !cell.IsRevealed) return new ExplorationResult(ExplorationError.Hidden);
            if (cell.IsCompleted) return new ExplorationResult(ExplorationError.Completed);
            if (!cell.Definition.IsWalkable) return new ExplorationResult(ExplorationError.Blocked);
            if (cell.Definition.RequiredKey != null && Inventory.Count(cell.Definition.RequiredKey) == 0) return new ExplorationResult(ExplorationError.MissingKey);
            if (cell.Definition.Kind == CellKind.Exit && map.RequireBossForExit && cells.Values.Any(x => x.Definition.Kind == CellKind.Boss && !x.IsCompleted))
                return new ExplorationResult(ExplorationError.BossAlive);
            if (IsInteraction(cell.Definition.Kind)) { OpenInteraction(cell); return new ExplorationResult(ExplorationError.None); }
            if (cell.Definition.Kind == CellKind.Battle || cell.Definition.Kind == CellKind.Boss)
            {
                if (!encounters.TryGetValue(position, out var enemies))
                {
                    enemies = cell.Definition.Enemies.Select((id, index) => catalog.CreateUnit(id, map.Id + "/" + position + "/" + index, Team.Enemy)).ToArray();
                    foreach (var enemy in enemies) difficulty?.Apply(enemy);
                    encounters.Add(position, enemies);
                }
                encounterSummons.TryGetValue(position, out var summons);
                var combatants = enemies.Concat(summons ?? Array.Empty<UnitState>()).Where(x => !x.IsDead);
                var battle = new Battle(catalog, new[] { Player }.Concat(combatants), random, aiPolicies);
                battle.RewardUnits = Array.AsReadOnly(enemies);
                CurrentBattle = battle; pending = cell; cell.IsContentKnown = true; Phase = SessionPhase.Battle;
                interaction = null;
                var result = battle.Start();
                return new ExplorationResult(result.Success ? ExplorationError.None : ExplorationError.BattleFailed, result.Events);
            }
            // Key consumption and reward delivery commit together: full bags never lose a key.
            if (!Inventory.TryGrant(cell.Definition.Reward)) return new ExplorationResult(ExplorationError.InventoryFull);
            if (cell.Definition.RequiredKey != null) Inventory.TryRemove(cell.Definition.RequiredKey);
            if (cell.Definition.Kind == CellKind.Rest) Progression.Rest(Player, difficulty?.RestRatio(map.RestRatio) ?? map.RestRatio);
            Complete(cell); if (cell.Definition.Kind == CellKind.Exit) Phase = SessionPhase.Exit;
            return new ExplorationResult(ExplorationError.None);
        }
        public ExplorationResult ContinueAfterBattle()
        {
            if (Phase != SessionPhase.Battle || CurrentBattle == null || CurrentBattle.Phase != BattlePhase.Finished) return new ExplorationResult(ExplorationError.WrongPhase);
            if (CurrentBattle.Outcome == BattleOutcome.Defeat)
            {
                encounterSummons.Remove(pending.Definition.Position);
                LastCompletedBattle = CurrentBattle; CurrentBattle = null; pending = null; interaction = null;
                Phase = SessionPhase.Defeat; return new ExplorationResult(ExplorationError.None);
            }
            if (commandsClosed || !(operationAllowed?.Invoke() ?? true)) return new ExplorationResult(ExplorationError.WrongPhase);
            if (CurrentBattle.Outcome == BattleOutcome.Victory)
            {
                if (!CurrentBattle.RewardsClaimed && !rewards.TryClaim(CurrentBattle, Inventory)) return new ExplorationResult(ExplorationError.InventoryFull);
                if (!Inventory.TryGrant(pending.Definition.Reward)) return new ExplorationResult(ExplorationError.InventoryFull);
                Complete(pending);
            }
            if (CurrentBattle.Outcome == BattleOutcome.Escaped)
                encounterSummons[pending.Definition.Position] = CurrentBattle.Units.Where(x => x.Team == Team.Enemy && x.IsSummoned && !x.IsDead).ToArray();
            else encounterSummons.Remove(pending.Definition.Position);
            LastCompletedBattle = CurrentBattle;
            pending = null; CurrentBattle = null; Phase = SessionPhase.Exploration;
            return new ExplorationResult(ExplorationError.None);
        }
        private void Complete(CellState cell)
        {
            PrepareCompletion(cell)();
        }
        private Action PrepareCompletion(CellState cell)
        {
            // Resolve the topology before committing any progress or rewards.
            var neighbors = topology.Neighbors(cell.Definition.Position).Where(cells.ContainsKey).Select(x => cells[x]).ToArray();
            return () =>
            {
                cell.IsCompleted = true; cell.IsContentKnown = true;
                foreach (var neighbor in neighbors) neighbor.IsRevealed = true;
            };
        }
        internal bool CloseForAbandon()
        {
            if (CurrentBattle != null && CurrentBattle.Phase != BattlePhase.Finished) return false;
            if (Phase == SessionPhase.Battle)
            {
                if (pending != null) encounterSummons.Remove(pending.Definition.Position);
                LastCompletedBattle = CurrentBattle; CurrentBattle = null; pending = null; Phase = SessionPhase.Defeat;
            }
            CloseCommands(); return true;
        }
    }
}
