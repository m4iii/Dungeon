using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Dungeon.Core
{
    public sealed class DifficultyStep
    {
        public int Level { get; }
        public IReadOnlyDictionary<int, double> EnemyHealthBonus { get; }
        public double ShopPriceBonus { get; }
        public double RestPenalty { get; }
        public double WeaponLevelBonus { get; }
        public int SkillLevelBonus { get; }
        public int SlotSurcharge { get; }
        public DifficultyStep(int level, IDictionary<int, double> enemyHealthBonus = null, double shopPriceBonus = 0,
            double restPenalty = 0, double weaponLevelBonus = 0, int skillLevelBonus = 0, int slotSurcharge = 0)
        {
            Level = Data.NonNegative(level); var copy = new Dictionary<int, double>(enemyHealthBonus ?? new Dictionary<int, double>());
            foreach (var pair in copy) { Data.NonNegative(pair.Key); if (Data.Finite(pair.Value) < 0) throw new ArgumentOutOfRangeException(nameof(enemyHealthBonus)); }
            EnemyHealthBonus = new ReadOnlyDictionary<int, double>(copy);
            if (Data.Finite(shopPriceBonus) < 0 || Data.Finite(restPenalty) < 0 || Data.Finite(weaponLevelBonus) < 0) throw new ArgumentOutOfRangeException();
            ShopPriceBonus = shopPriceBonus; RestPenalty = restPenalty; WeaponLevelBonus = weaponLevelBonus;
            SkillLevelBonus = Data.NonNegative(skillLevelBonus); SlotSurcharge = Data.NonNegative(slotSurcharge);
        }
    }
    public sealed class DifficultyRules
    {
        private readonly IReadOnlyList<DifficultyStep> steps;
        public int MaximumLevel => steps.Count == 0 ? 0 : steps.Max(x => x.Level);
        public DifficultyRules(IEnumerable<DifficultyStep> steps)
        {
            this.steps = Data.List(steps);
            if (this.steps.Select(x => x.Level).Distinct().Count() != this.steps.Count) throw new ArgumentException("Duplicate difficulty levels.");
        }
        public DifficultyState At(int level)
        {
            if (level < 0 || level > MaximumLevel) throw new ArgumentOutOfRangeException(nameof(level));
            return new DifficultyState(level, steps.Where(x => x.Level <= level));
        }
    }
    public sealed class DifficultyState
    {
        private readonly Dictionary<int, double> health = new Dictionary<int, double>();
        public int Level { get; }
        public double ShopMultiplier { get; }
        public double RestPenalty { get; }
        public double WeaponMultiplier { get; }
        public int SkillBonus { get; }
        public int SlotSurcharge { get; }
        internal DifficultyState(int level, IEnumerable<DifficultyStep> steps)
        {
            Level = level; ShopMultiplier = 1; WeaponMultiplier = 1;
            foreach (var step in steps)
            {
                foreach (var pair in step.EnemyHealthBonus) { health.TryGetValue(pair.Key, out var old); health[pair.Key] = old + pair.Value; }
                ShopMultiplier += step.ShopPriceBonus; RestPenalty += step.RestPenalty; WeaponMultiplier += step.WeaponLevelBonus;
                SkillBonus = checked(SkillBonus + step.SkillLevelBonus); SlotSurcharge = checked(SlotSurcharge + step.SlotSurcharge);
            }
        }
        public double RestRatio(double baseRatio) => Math.Max(0, Math.Min(1, Data.Finite(baseRatio) - RestPenalty));
        internal void Apply(UnitState enemy)
        {
            if (enemy.InBattle) throw new InvalidOperationException("Scale enemies before starting the battle.");
            health.TryGetValue(enemy.Definition.Rank, out var bonus);
            enemy.BaseStats[Stat.MaxHealth] = Data.Finite(enemy.BaseStats[Stat.MaxHealth] * (1 + bonus));
            if (enemy.Weapon != null)
            {
                int original = enemy.Weapon.Level, scaled = Data.Int(original * WeaponMultiplier);
                enemy.Weapon.Level = WeaponMultiplier > 1 && scaled == original ? checked(original + 1) : Math.Max(1, scaled);
            }
            foreach (var skill in enemy.Skills) skill.Level = checked(skill.Level + SkillBonus);
            enemy.Health = enemy.MaxHealth; enemy.Mana = enemy.MaxMana;
        }
    }
    public sealed class ExperienceRules
    {
        public int RegionBase { get; }
        public int RegionGrowth { get; }
        public IReadOnlyDictionary<int, int> KillExperience { get; }
        public double DifficultyBonus { get; }
        public double VictoryMultiplier { get; }
        public int LevelBase { get; }
        public int LevelGrowth { get; }
        public ExperienceRules(int regionBase, int regionGrowth, IDictionary<int, int> killExperience, double difficultyBonus,
            double victoryMultiplier, int levelBase, int levelGrowth)
        {
            RegionBase = Data.NonNegative(regionBase); RegionGrowth = Data.NonNegative(regionGrowth);
            var copy = new Dictionary<int, int>(killExperience ?? throw new ArgumentNullException(nameof(killExperience)));
            foreach (var pair in copy) { Data.NonNegative(pair.Key); Data.NonNegative(pair.Value); }
            KillExperience = new ReadOnlyDictionary<int, int>(copy);
            if (Data.Finite(difficultyBonus) < 0 || Data.Finite(victoryMultiplier) < 0 || levelBase < 1 || levelGrowth < 0) throw new ArgumentOutOfRangeException();
            DifficultyBonus = difficultyBonus; VictoryMultiplier = victoryMultiplier; LevelBase = levelBase; LevelGrowth = levelGrowth;
        }
        public int RequiredExperience(int nextLevel) { if (nextLevel < 1) throw new ArgumentOutOfRangeException(nameof(nextLevel)); return checked(LevelBase + LevelGrowth * nextLevel); }
        public int Calculate(int regionLevel, IDictionary<int, int> kills, int difficulty, bool victory)
        {
            Data.NonNegative(regionLevel); Data.NonNegative(difficulty); long total = regionLevel > 1 ? checked(RegionBase + RegionGrowth * (regionLevel - 1)) : 0;
            foreach (var pair in kills) { Data.NonNegative(pair.Value); if (KillExperience.TryGetValue(pair.Key, out var value)) total = checked(total + (long)value * pair.Value); }
            return Math.Max(0, Data.Int(total * (1 + difficulty * DifficultyBonus) * (victory ? VictoryMultiplier : 1)));
        }
    }
    public sealed class TalentDefinition
    {
        public string Id { get; }
        public int Cost { get; }
        public IReadOnlyList<string> Prerequisites { get; }
        public IReadOnlyList<StatModifier> Modifiers { get; }
        public TalentDefinition(string id, int cost, IEnumerable<StatModifier> modifiers, IEnumerable<string> prerequisites = null)
        { Id = Data.Id(id); Cost = Data.NonNegative(cost); Modifiers = Data.List(modifiers); Prerequisites = Data.List(prerequisites); }
    }
    public sealed class HeroProgress
    {
        public string HeroId { get; }
        public int Experience { get; internal set; }
        public int Level { get; internal set; }
        public int SpentPoints { get; internal set; }
        public int AvailablePoints => Level - SpentPoints;
        internal readonly HashSet<string> Learned = new HashSet<string>(StringComparer.Ordinal);
        public IReadOnlyList<string> Talents => Array.AsReadOnly(Learned.OrderBy(x => x, StringComparer.Ordinal).ToArray());
        internal HeroProgress(string id) { HeroId = Data.Id(id); }
    }
    public sealed partial class PlayerProfile
    {
        internal readonly Dictionary<string, HeroProgress> heroes = new Dictionary<string, HeroProgress>(StringComparer.Ordinal);
        internal readonly HashSet<string> unlocks = new HashSet<string>(StringComparer.Ordinal);
        internal readonly HashSet<string> discoveries = new HashSet<string>(StringComparer.Ordinal);
        internal readonly HashSet<string> achievements = new HashSet<string>(StringComparer.Ordinal);
        internal readonly HashSet<string> settledRuns = new HashSet<string>(StringComparer.Ordinal);
        public int HighestDifficulty { get; private set; }
        public IReadOnlyList<string> Unlocks => Array.AsReadOnly(unlocks.OrderBy(x => x, StringComparer.Ordinal).ToArray());
        public IReadOnlyList<string> Discoveries => Array.AsReadOnly(discoveries.OrderBy(x => x, StringComparer.Ordinal).ToArray());
        public IReadOnlyList<string> Achievements => Array.AsReadOnly(achievements.OrderBy(x => x, StringComparer.Ordinal).ToArray());
        public HeroProgress Hero(string id)
        { Data.Id(id); if (!heroes.TryGetValue(id, out var hero)) heroes.Add(id, hero = new HeroProgress(id)); return hero; }
        public bool IsUnlocked(string id) => unlocks.Contains(id);
        public bool Unlock(string id) => unlocks.Add(Data.Id(id));
        public bool Discover(string id) => discoveries.Add(Data.Id(id));
        public bool Achieve(string id) => achievements.Add(Data.Id(id));
        public bool LearnTalent(string heroId, TalentDefinition talent)
        {
            var hero = Hero(heroId);
            if (hero.Learned.Contains(talent.Id) || hero.AvailablePoints < talent.Cost || !talent.Prerequisites.All(hero.Learned.Contains)) return false;
            hero.Learned.Add(talent.Id); hero.SpentPoints += talent.Cost; return true;
        }
        internal void ApplyTalents(UnitState unit, IEnumerable<TalentDefinition> definitions)
        {
            var table = definitions.ToDictionary(x => x.Id, StringComparer.Ordinal);
            foreach (var id in Hero(unit.Definition.Id).Learned)
            {
                if (!table.TryGetValue(id, out var talent)) throw new ArgumentException("Missing talent: " + id);
                foreach (var modifier in talent.Modifiers) { unit.BaseStats.TryGetValue(modifier.Stat, out var old); unit.BaseStats[modifier.Stat] = Data.Finite(old + modifier.Amount.Evaluate(1)); }
            }
            unit.Health = unit.MaxHealth; unit.Mana = unit.MaxMana;
        }
        internal bool Settle(string runId, string heroId, int experience, ExperienceRules rules, int difficulty, bool victory, int maxDifficulty)
        {
            if (settledRuns.Contains(runId)) return false;
            var hero = Hero(heroId); int remainder = checked(hero.Experience + Data.NonNegative(experience)), level = hero.Level;
            while (remainder >= rules.RequiredExperience(checked(level + 1))) { remainder -= rules.RequiredExperience(level + 1); level++; }
            hero.Experience = remainder; hero.Level = level; settledRuns.Add(Data.Id(runId));
            if (victory) HighestDifficulty = Math.Max(HighestDifficulty, Math.Min(maxDifficulty, checked(difficulty + 1)));
            return true;
        }
    }
}
