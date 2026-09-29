using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Dungeon.Core
{
    public sealed class SkillState
    {
        public string Id { get; }
        public int Level { get; internal set; }
        public int Cooldown { get; internal set; }
        public string RuneId { get; internal set; }
        public bool Locked { get; internal set; }
        internal readonly Dictionary<string, int> MutableCounters = new Dictionary<string, int>(StringComparer.Ordinal);
        public IReadOnlyDictionary<string, int> Counters => new ReadOnlyDictionary<string, int>(MutableCounters);
        internal SkillState(SkillLoadout definition) { Id = definition.SkillId; Level = definition.Level; Cooldown = definition.InitialCooldown; }
    }
    public sealed class StatusState
    {
        public string Id { get; }
        public int Stacks { get; internal set; }
        internal StatusState(string id, int stacks) { Id = id; Stacks = stacks; }
    }
    public sealed partial class UnitState
    {
        internal GameCatalog Catalog { get; }
        internal readonly Dictionary<Stat, double> BaseStats;
        internal readonly List<StatusState> MutableStatuses = new List<StatusState>();
        internal readonly List<SkillState> MutableSkills;
        internal readonly List<SkillState> MutableRelics = new List<SkillState>();
        public string Id { get; }
        public UnitDefinition Definition { get; }
        public Team Team { get; }
        public Control Control { get; }
        public int Health { get; internal set; }
        public int Mana { get; internal set; }
        public int Armor { get; internal set; }
        public int Actions { get; internal set; }
        public bool IsSummoned { get; internal set; }
        public bool IsDead => Health <= 0;
        public int MaxHealth => Math.Max(1, Data.Int(GetStat(Stat.MaxHealth)));
        public int MaxMana => Math.Max(0, Data.Int(GetStat(Stat.MaxMana)));
        public SkillState Weapon { get; }
        public IReadOnlyList<SkillState> Skills { get; }
        public IReadOnlyList<StatusState> Statuses { get; }
        public IReadOnlyList<SkillState> Relics { get; }
        internal bool DeathResolved;
        internal bool ReceivedStartArmor;
        internal bool InBattle;
        internal UnitState(GameCatalog catalog, UnitDefinition definition, string id, Team team, Control control)
        {
            Catalog = catalog; Definition = definition; Id = Data.Id(id); Team = Data.EnumValue(team); Control = Data.EnumValue(control);
            BaseStats = definition.Stats.ToDictionary(x => x.Key, x => x.Value);
            Weapon = definition.Weapon == null ? null : new SkillState(definition.Weapon);
            MutableSkills = definition.Skills.Select(x => new SkillState(x)).ToList();
            Skills = MutableSkills.AsReadOnly(); Statuses = MutableStatuses.AsReadOnly(); Relics = MutableRelics.AsReadOnly();
            Health = MaxHealth; Mana = MaxMana;
        }
        public double GetStat(Stat stat)
        {
            double value = BaseStats.TryGetValue(stat, out var found) ? found : stat == Stat.CounterMultiplier ? Catalog.Rules.CounterMultiplier : 0;
            foreach (var status in MutableStatuses)
                foreach (var modifier in Catalog.Statuses[status.Id].Modifiers)
                    if (modifier.Stat == stat) value += modifier.Amount.Evaluate(status.Stacks);
            if (Definition.DeriveAttributes)
            {
                var rules = Catalog.Rules;
                switch (stat)
                {
                    case Stat.MaxHealth: value += GetStat(Stat.Strength) * rules.HealthPerStrength; break;
                    case Stat.MaxMana: value += GetStat(Stat.Intelligence) * rules.ManaPerIntelligence; break;
                    case Stat.HealthRecovery: value += GetStat(Stat.Strength) * rules.RecoveryPerStrength; break;
                    case Stat.ManaRecovery: value += GetStat(Stat.Intelligence) * rules.RecoveryPerIntelligence; break;
                    case Stat.Defense: value += GetStat(Stat.Agility) * rules.DefensePerAgility; break;
                }
            }
            return Data.Finite(value);
        }
        public int Stacks(string id) => MutableStatuses.FirstOrDefault(x => x.Id == id)?.Stacks ?? 0;
        public bool HasTrait(StatusTrait trait) => MutableStatuses.Any(x => (Catalog.Statuses[x.Id].Traits & trait) != 0);
        internal SkillState FindSkill(string id) => Weapon?.Id == id ? Weapon : MutableSkills.FirstOrDefault(x => x.Id == id);
        internal IEnumerable<SkillState> AllSkills => Weapon == null ? MutableSkills : new[] { Weapon }.Concat(MutableSkills);
        internal void ClampResources() { Health = Math.Max(0, Math.Min(Health, MaxHealth)); Mana = Math.Max(0, Math.Min(Mana, MaxMana)); }
        internal void SetStatus(string id, int amount, bool remove = false)
        {
            var definition = Catalog.Statuses[id];
            var state = MutableStatuses.FirstOrDefault(x => x.Id == id);
            int old = state?.Stacks ?? 0;
            long next = remove ? Math.Max(0L, (long)old - amount) : definition.Stacking == StackMode.Maximum ? Math.Max(old, amount) :
                definition.Stacking == StackMode.Replace ? amount : (long)old + amount;
            next = Math.Min(next, definition.MaxStacks);
            if (next <= 0) { if (state != null) MutableStatuses.Remove(state); }
            else if (state == null) MutableStatuses.Add(new StatusState(id, (int)next));
            else state.Stacks = (int)next;
            ClampResources();
        }
        internal bool ConsumeTrait(StatusTrait trait)
        {
            var state = MutableStatuses.FirstOrDefault(x => (Catalog.Statuses[x.Id].Traits & trait) != 0);
            if (state == null) return false;
            SetStatus(state.Id, 1, true); return true;
        }

        /// <summary>Persistence boundary: only outside a battle. Queue/intent state is deliberately not serialized here.</summary>
        public UnitSaveData Capture()
        {
            if (InBattle) throw new InvalidOperationException("Finish the battle before saving a unit.");
            return new UnitSaveData
            {
                DefinitionId = Definition.Id, InstanceId = Id, Team = Team, Control = Control, Health = Health, Mana = Mana, IsSummoned = IsSummoned,
                Stats = BaseStats.ToDictionary(x => x.Key, x => x.Value),
                Skills = AllSkills.Select(SaveSkill).ToArray(),
                Statuses = MutableStatuses.Select(x => new StatusSaveData { Id = x.Id, Stacks = x.Stacks }).ToArray(),
                Relics = MutableRelics.Select(x => x.Id).ToArray(), RelicStates = MutableRelics.Select(SaveSkill).ToArray()
            };
        }
        public static UnitState Restore(GameCatalog catalog, UnitSaveData data)
        {
            if (data == null || (data.Version != 1 && data.Version != 2) || data.Stats == null || data.Skills == null || data.Statuses == null || data.Relics == null)
                throw new ArgumentException("Invalid or unsupported unit save.");
            var unit = catalog.CreateUnit(data.DefinitionId, data.InstanceId, data.Team, data.Control);
            if (data.Skills.Any(x => x == null) || data.Statuses.Any(x => x == null)) throw new ArgumentException("Null save entries.");
            unit.BaseStats.Clear();
            foreach (var pair in data.Stats) { Data.EnumValue(pair.Key); Data.Finite(pair.Value); unit.BaseStats[pair.Key] = pair.Value; }
            if (data.Skills.Select(x => x.Id).Distinct().Count() != data.Skills.Length) throw new ArgumentException("Duplicate saved skills.");
            unit.MutableSkills.Clear();
            foreach (var skill in data.Skills)
            {
                if (skill.Level < 1 || !catalog.Skills.ContainsKey(skill.Id) || skill.Cooldown < 0) throw new ArgumentException("Invalid saved skill.");
                if (unit.Weapon?.Id == skill.Id) { unit.Weapon.Level = skill.Level; unit.Weapon.Cooldown = skill.Cooldown; }
                else unit.MutableSkills.Add(new SkillState(new SkillLoadout(skill.Id, skill.Level, skill.Cooldown)));
                var restored = unit.FindSkill(skill.Id); restored.Locked = skill.Locked;
                if (skill.RuneId != null && (!catalog.Runes.TryGetValue(skill.RuneId, out var rune) || !rune.Accepts(catalog.Skills[skill.Id]))) throw new ArgumentException("Invalid saved rune.");
                restored.RuneId = skill.RuneId;
                if (skill.Counters != null) foreach (var pair in skill.Counters) restored.MutableCounters.Add(Data.Id(pair.Key), pair.Value);
            }
            if (unit.Weapon != null && !data.Skills.Any(x => x.Id == unit.Weapon.Id)) throw new ArgumentException("Missing saved weapon.");
            if (data.Statuses.Select(x => x.Id).Distinct().Count() != data.Statuses.Length) throw new ArgumentException("Duplicate saved statuses.");
            foreach (var status in data.Statuses)
            {
                if (!catalog.Statuses.TryGetValue(status.Id, out var definition) || status.Stacks <= 0 || status.Stacks > definition.MaxStacks)
                    throw new ArgumentException("Invalid saved status.");
                unit.MutableStatuses.Add(new StatusState(status.Id, status.Stacks));
            }
            foreach (var relic in data.Relics)
            {
                if (!catalog.Skills.TryGetValue(relic, out var definition) || definition.Kind != SkillKind.Passive || unit.Relics.Any(x => x.Id == relic)) throw new ArgumentException("Invalid saved relic.");
                var saved = data.RelicStates?.SingleOrDefault(x => x.Id == relic);
                var state = new SkillState(new SkillLoadout(relic, saved?.Level ?? 1, saved?.Cooldown ?? 0));
                if (saved?.Counters != null) foreach (var pair in saved.Counters) state.MutableCounters.Add(Data.Id(pair.Key), pair.Value);
                unit.MutableRelics.Add(state);
            }
            if (data.Health < 0 || data.Health > unit.MaxHealth || data.Mana < 0 || data.Mana > unit.MaxMana)
                throw new ArgumentException("Invalid saved resources.");
            unit.Health = data.Health; unit.Mana = data.Mana; unit.IsSummoned = data.IsSummoned;
            return unit;
        }
    }

    // Serializer-neutral DTOs: consumers choose JSON, binary, or another persistence format.
    [Serializable] public sealed class UnitSaveData
    {
        public int Version = 2;
        public string DefinitionId, InstanceId;
        public Team Team;
        public Control Control;
        public int Health, Mana;
        public bool IsSummoned;
        public Dictionary<Stat, double> Stats;
        public SkillSaveData[] Skills;
        public StatusSaveData[] Statuses;
        public string[] Relics;
        public SkillSaveData[] RelicStates;
    }
    [Serializable] public sealed class SkillSaveData { public string Id, RuneId; public int Level, Cooldown; public bool Locked; public Dictionary<string, int> Counters; }
    [Serializable] public sealed class StatusSaveData { public string Id; public int Stacks; }

    public interface IRandomSource { int Next(int exclusiveMaximum); }
    /// <summary>Explicit, portable PRNG state. No dependency on platform Random or frame timing.</summary>
    public sealed class SeededRandom : IRandomSource
    {
        public uint State { get; private set; }
        public SeededRandom(uint seed) { State = seed == 0 ? 0x9E3779B9u : seed; }
        public int Next(int exclusiveMaximum)
        {
            if (exclusiveMaximum <= 0) throw new ArgumentOutOfRangeException(nameof(exclusiveMaximum));
            uint bound = (uint)exclusiveMaximum, limit = unchecked(0u - bound) % bound, value;
            do { value = State; value ^= value << 13; value ^= value >> 17; value ^= value << 5; State = value; } while (value < limit);
            return (int)(value % bound);
        }
    }

    public enum BattlePhase { NotStarted, AwaitingPlayer, AwaitingAdvance, Finished, Faulted }
    public enum BattleOutcome { None, Victory, Defeat, Escaped }
    public enum CommandError { None, WrongPhase, WrongActor, UnknownSkill, PassiveSkill, WrongScope, Cooldown, InsufficientResource, InsufficientActions, InvalidTarget, Condition, MissingItem, ExecutionLimit, ExecutionFailed }
    public enum EventKind { BattleStarted, RoundStarted, TurnStarted, TurnEnded, SkillUsed, ItemUsed, Damage, Armor, Health, Mana, Actions, StatusChanged, UnitDied, UnitRevived, Summoned, BattleEnded, Faulted }
    public sealed class GameEvent
    {
        public long Sequence { get; }
        public EventKind Kind { get; }
        public string SourceId { get; }
        public string TargetId { get; }
        public string DefinitionId { get; }
        public int Amount { get; }
        public int ArmorDamage { get; }
        internal GameEvent(long sequence, EventKind kind, string source, string target, string definition, int amount, int armorDamage = 0)
        { Sequence = sequence; Kind = kind; SourceId = source; TargetId = target; DefinitionId = definition; Amount = amount; ArmorDamage = armorDamage; }
    }
    public sealed class CommandResult
    {
        public CommandError Error { get; }
        public bool Success => Error == CommandError.None;
        public IReadOnlyList<GameEvent> Events { get; }
        internal CommandResult(CommandError error, IEnumerable<GameEvent> events = null) { Error = error; Events = Data.List(events); }
    }
    public sealed class Intent
    {
        public string UnitId { get; }
        public string SkillId { get; }
        public string TargetId { get; }
        internal Intent(string unitId, string skillId, string targetId) { UnitId = unitId; SkillId = skillId; TargetId = targetId; }
    }
    public sealed class AiContext
    {
        public UnitState Unit { get; }
        public int Round { get; }
        public IReadOnlyList<string> AvailableSkills { get; }
        public IReadOnlyList<UnitState> Allies { get; }
        internal AiContext(UnitState unit, int round, IEnumerable<string> skills, IEnumerable<UnitState> allies)
        { Unit = unit; Round = round; AvailableSkills = Data.List(skills); Allies = Data.List(allies); }
    }
    /// <summary>Policies must not keep hidden mutable run state; decisions use context and the supplied random source.</summary>
    public interface IAiPolicy { string ChooseSkill(AiContext context, IRandomSource random); }
    public sealed class RandomAiPolicy : IAiPolicy
    {
        public string ChooseSkill(AiContext context, IRandomSource random) => context.AvailableSkills.Count == 0 ? null : context.AvailableSkills[random.Next(context.AvailableSkills.Count)];
    }
}
