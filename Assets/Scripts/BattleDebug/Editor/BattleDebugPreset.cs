using System;
using System.Collections.Generic;
using System.Linq;
using Dungeon.Core;
using UnityEngine;

namespace Dungeon.BattleDebug
{
    [Serializable]
    public sealed class DebugEffect
    {
        public EffectKind kind;
        public Target target = Target.SelectedEnemy;
        public float constant;
        public float perLevel;
        public bool scaleWithStat;
        public Stat sourceStat = Stat.Attack;
        public float coefficient = 1;
        public string referenceId;
        public Stat changedStat = Stat.Attack;
        [Min(1)] public int hits = 1;
        public bool trueDamage, isAttack;
        public EffectDefinition ToCore() => new EffectDefinition(kind, target,
            new Magnitude(constant, perLevel, scaleWithStat ? sourceStat : (Stat?)null, coefficient),
            string.IsNullOrWhiteSpace(referenceId) ? null : referenceId, changedStat, hits, trueDamage, isAttack);
    }
    [Serializable]
    public sealed class DebugTrigger
    {
        public Trigger when = Trigger.TurnEnd;
        [Min(0)] public int consumeStacks;
        public DebugEffect[] effects = Array.Empty<DebugEffect>();
        public TriggerDefinition ToCore() => new TriggerDefinition(when, effects.Select(x => x.ToCore()), consumeStacks);
    }
    [Serializable]
    public sealed class DebugSkill
    {
        public string id, displayName;
        public SkillKind kind = SkillKind.Magic;
        [Min(1)] public int maxLevel = 3;
        public Resource resource;
        [Min(0)] public int resourceCost, cooldown;
        [Min(0)] public int actionCost = 1;
        public DebugEffect[] effects = Array.Empty<DebugEffect>();
        public DebugTrigger[] triggers = Array.Empty<DebugTrigger>();
        public SkillDefinition ToCore() => new SkillDefinition(id, kind, maxLevel, new Magnitude(resourceCost), new Magnitude(cooldown),
            actionCost, effects.Select(x => x.ToCore()), triggers.Select(x => x.ToCore()), resource);
    }
    [Serializable]
    public sealed class DebugStatus
    {
        public string id, displayName;
        public StackMode stacking;
        [Min(1)] public int maxStacks = 10;
        public bool debuff = true;
        public TickPhase decay = TickPhase.TurnEnd;
        public StatusTrait traits;
        public DebugTrigger[] triggers = Array.Empty<DebugTrigger>();
        public StatusDefinition ToCore() => new StatusDefinition(id, stacking, maxStacks, true, debuff, decay, traits, triggers: triggers.Select(x => x.ToCore()));
    }
    [Serializable]
    public sealed class DebugLoadout
    {
        public string skillId;
        [Min(1)] public int level = 1;
        [Min(0)] public int initialCooldown;
        public SkillLoadout ToCore() => new SkillLoadout(skillId, level, initialCooldown);
    }
    [Serializable]
    public sealed class DebugUnit
    {
        public string id, displayName;
        [Min(1)] public int health = 80;
        [Min(0)] public int mana = 20, attack = 10, defense = 2, startArmor;
        public float strength, agility, intelligence;
        public bool deriveAttributes;
        [Min(1)] public int size = 1;
        public DebugLoadout weapon = new DebugLoadout { skillId = "attack" };
        public DebugLoadout[] skills = Array.Empty<DebugLoadout>();
        public UnitDefinition ToCore() => new UnitDefinition(id, new Dictionary<Stat, double> {
            [Stat.MaxHealth] = health, [Stat.MaxMana] = mana, [Stat.Attack] = attack, [Stat.Defense] = defense,
            [Stat.StartArmor] = startArmor, [Stat.Strength] = strength, [Stat.Agility] = agility, [Stat.Intelligence] = intelligence
        }, weapon == null || string.IsNullOrWhiteSpace(weapon.skillId) ? null : weapon.ToCore(), skills.Select(x => x.ToCore()), deriveAttributes, size: size);
    }
    [Serializable]
    public sealed class DebugItem
    {
        public string id, displayName;
        [Min(0)] public int count = 2;
        public DebugEffect[] effects = Array.Empty<DebugEffect>();
        public ItemDefinition ToCore() => new ItemDefinition(id, 0, effects.Select(x => x.ToCore()), UseScope.Battle);
    }

    [CreateAssetMenu(menuName = "Dungeon/战斗调试配置", fileName = "BattleDebugPreset")]
    public sealed class BattleDebugPreset : ScriptableObject
    {
        [Header("战斗规则")]
        [Min(1)] public int actionsPerTurn = 3;
        [Min(0)] public int escapeCost = 2;
        [Min(1)] public int teamCapacity = 6;
        [Min(32)] public int eventLimit = 2048;
        [Header("参战单位 ID（敌人允许重复）")]
        public string playerId = "hero";
        public string[] enemyIds = { "wolf", "shaman" };
        [Header("配置目录")]
        public DebugUnit[] units = Array.Empty<DebugUnit>();
        public DebugSkill[] skills = Array.Empty<DebugSkill>();
        public DebugStatus[] statuses = Array.Empty<DebugStatus>();
        public DebugItem[] items = Array.Empty<DebugItem>();

        public GameCatalog ToCore() => new GameCatalog(new RuleSettings(actionsPerTurn, escapeCost, teamCapacity, eventLimit),
            skills.Select(x => x.ToCore()), statuses.Select(x => x.ToCore()), units.Select(x => x.ToCore()), items.Select(x => x.ToCore()));
        public string NameOf(string id)
        {
            if (id == null) return "—";
            string name = units.FirstOrDefault(x => x.id == id)?.displayName ?? skills.FirstOrDefault(x => x.id == id)?.displayName ??
                statuses.FirstOrDefault(x => x.id == id)?.displayName ?? items.FirstOrDefault(x => x.id == id)?.displayName;
            return string.IsNullOrWhiteSpace(name) ? id : name;
        }
        public void SetExample()
        {
            skills = new[] {
                new DebugSkill { id = "attack", displayName = "普通攻击", kind = SkillKind.Weapon, effects = new[] {
                    new DebugEffect { kind = EffectKind.Damage, scaleWithStat = true, isAttack = true } } },
                new DebugSkill { id = "fire", displayName = "火球术", resourceCost = 5, cooldown = 2, effects = new[] {
                    new DebugEffect { kind = EffectKind.Damage, constant = 16, perLevel = 2 } } },
                new DebugSkill { id = "guard", displayName = "防御", cooldown = 1, effects = new[] {
                    new DebugEffect { kind = EffectKind.Armor, target = Target.Self, constant = 12 } } },
                new DebugSkill { id = "poison", displayName = "施毒", resourceCost = 3, cooldown = 2, effects = new[] {
                    new DebugEffect { kind = EffectKind.AddStatus, referenceId = "poisoned", constant = 3 } } },
                new DebugSkill { id = "heal", displayName = "治疗", resourceCost = 4, cooldown = 2, effects = new[] {
                    new DebugEffect { kind = EffectKind.Heal, target = Target.Self, constant = 15 } } }
            };
            statuses = new[] { new DebugStatus { id = "poisoned", displayName = "中毒", triggers = new[] {
                new DebugTrigger { when = Trigger.TurnEnd, effects = new[] {
                    new DebugEffect { kind = EffectKind.Damage, target = Target.Self, perLevel = 2, trueDamage = true } } } } } };
            DebugLoadout Load(string id) => new DebugLoadout { skillId = id };
            units = new[] {
                new DebugUnit { id = "hero", displayName = "调试勇者", health = 100, mana = 30, attack = 12, defense = 3,
                    skills = new[] { Load("fire"), Load("guard"), Load("poison"), Load("heal") } },
                new DebugUnit { id = "wolf", displayName = "野狼", health = 45, mana = 0, attack = 8, defense = 1 },
                new DebugUnit { id = "shaman", displayName = "施毒者", health = 55, mana = 20, attack = 6, defense = 2,
                    skills = new[] { Load("poison") } }
            };
            items = new[] { new DebugItem { id = "potion", displayName = "治疗药水", count = 2, effects = new[] {
                new DebugEffect { kind = EffectKind.Heal, target = Target.Self, constant = 25 } } } };
        }
    }
}
