using System;
using System.Collections.Generic;
using System.Linq;

namespace Dungeon.Core
{
    public sealed class ExplorationExecutionLimitException : InvalidOperationException
    {
        public ExplorationExecutionLimitException() : base("Exploration effect chain exceeded the execution limit.") { }
    }

    /// <summary>Small synchronous rollback boundary. Restores the original runtime objects, never replaces the player.</summary>
    internal static class ExplorationTransaction
    {
        internal static bool Run(UnitState unit, Inventory inventory, Func<bool> action)
        {
            var restoreUnit = unit.CreateRollback();
            var restoreInventory = inventory?.CreateRollback();
            try
            {
                if (action()) return true;
                restoreUnit(); restoreInventory?.Invoke(); return false;
            }
            catch
            {
                restoreUnit(); restoreInventory?.Invoke(); throw;
            }
        }
    }

    public sealed partial class UnitState
    {
        internal Action CreateRollback()
        {
            int health = Health, mana = Mana, armor = Armor, actions = Actions;
            bool deathResolved = DeathResolved, startArmor = ReceivedStartArmor, inBattle = InBattle, summoned = IsSummoned;
            var stats = new Dictionary<Stat, double>(BaseStats);
            var skills = MutableSkills.ToArray(); var relics = MutableRelics.ToArray(); var statuses = MutableStatuses.ToArray();
            var stacks = statuses.Select(x => x.Stacks).ToArray();
            var restoreSkills = AllSkills.Concat(Relics).Select(SkillRollback).ToArray();
            return () =>
            {
                Health = health; Mana = mana; Armor = armor; Actions = actions;
                DeathResolved = deathResolved; ReceivedStartArmor = startArmor; InBattle = inBattle; IsSummoned = summoned;
                BaseStats.Clear(); foreach (var pair in stats) BaseStats.Add(pair.Key, pair.Value);
                MutableSkills.Clear(); MutableSkills.AddRange(skills); MutableRelics.Clear(); MutableRelics.AddRange(relics);
                MutableStatuses.Clear(); MutableStatuses.AddRange(statuses);
                for (int i = 0; i < statuses.Length; i++) statuses[i].Stacks = stacks[i];
                foreach (var restore in restoreSkills) restore();
            };
        }
        private static Action SkillRollback(SkillState skill)
        {
            int level = skill.Level, cooldown = skill.Cooldown; string rune = skill.RuneId; bool locked = skill.Locked;
            var counters = new Dictionary<string, int>(skill.MutableCounters);
            return () =>
            {
                skill.Level = level; skill.Cooldown = cooldown; skill.RuneId = rune; skill.Locked = locked;
                skill.MutableCounters.Clear(); foreach (var pair in counters) skill.MutableCounters.Add(pair.Key, pair.Value);
            };
        }
    }
    public sealed partial class Inventory
    {
        internal Action CreateRollback()
        {
            int coins = Coins; var items = new Dictionary<string, int>(counts, StringComparer.Ordinal);
            return () => { Coins = coins; counts = new Dictionary<string, int>(items, StringComparer.Ordinal); };
        }
    }
}
