using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dungeon.Core;
using Dungeon.Runtime;
using UnityEditor;
using UnityEngine;

namespace Dungeon.BattleDebug
{
    public sealed class BattleDebugWindow : EditorWindow
    {
        private const string DefaultPath = "Assets/Scripts/BattleDebug/Presets/DefaultBattleDebug.asset";
        [SerializeField] private BattleDebugPreset preset;
        [SerializeField] private string seedText = "12345";
        private UnityEditor.Editor presetEditor;
        private BattleDebugPreset snapshot;
        private GameCatalog catalog;
        private Battle battle;
        private Inventory inventory;
        private uint sessionSeed;
        private string sessionJson, error, logText = "";
        private readonly List<string> log = new List<string>();
        private readonly Dictionary<string, string> targets = new Dictionary<string, string>();
        private Vector2 configScroll, battleScroll, logScroll;
        private bool showRaw, followLog = true;

        [MenuItem("Dungeon/战斗调试台")]
        public static void Open()
        {
            var window = GetWindow<BattleDebugWindow>();
            if (window.preset == null)
            {
                window.preset = AssetDatabase.LoadAssetAtPath<BattleDebugPreset>(DefaultPath);
                if (window.preset == null)
                {
                    if (!AssetDatabase.IsValidFolder("Assets/Scripts/BattleDebug/Presets"))
                        AssetDatabase.CreateFolder("Assets/Scripts/BattleDebug", "Presets");
                    window.preset = CreateInstance<BattleDebugPreset>();
                    window.preset.SetExample();
                    AssetDatabase.CreateAsset(window.preset, DefaultPath);
                    AssetDatabase.SaveAssets();
                }
            }
            window.Show();
        }

        private void OnEnable()
        {
            titleContent = new GUIContent("战斗调试台");
            minSize = new Vector2(1000, 680);
            if (preset == null) preset = AssetDatabase.LoadAssetAtPath<BattleDebugPreset>(DefaultPath);
        }
        private void OnDisable()
        {
            if (presetEditor != null) DestroyImmediate(presetEditor);
            if (snapshot != null) DestroyImmediate(snapshot);
            battle = null; catalog = null; inventory = null;
        }

        private void OnGUI()
        {
            EditorGUILayout.HelpBox("无需素材、场景或 Play。左侧配置 → 开始战斗 → 使用技能 → 结束玩家回合 → 单步推进。配置修改只影响下一场新战斗。", MessageType.Info);
            using (new EditorGUILayout.HorizontalScope())
            {
                using (new EditorGUILayout.VerticalScope(GUILayout.Width(310))) DrawConfig();
                using (new EditorGUILayout.VerticalScope())
                {
                    DrawToolbar();
                    if (!string.IsNullOrEmpty(error)) EditorGUILayout.HelpBox(error, MessageType.Error);
                    battleScroll = EditorGUILayout.BeginScrollView(battleScroll);
                    if (battle == null) EditorGUILayout.HelpBox("选择配置并开始战斗。脚本重新编译或关闭窗口后，需要重新开始。", MessageType.None);
                    else DrawBattle();
                    EditorGUILayout.EndScrollView();
                    DrawLog();
                }
            }
        }

        private void DrawConfig()
        {
            EditorGUILayout.LabelField("战斗配置 SO", EditorStyles.boldLabel);
            var next = (BattleDebugPreset)EditorGUILayout.ObjectField(preset, typeof(BattleDebugPreset), false);
            if (next != preset)
            {
                preset = next;
                if (presetEditor != null) DestroyImmediate(presetEditor);
            }
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("新建示例配置"))
                {
                    string path = EditorUtility.SaveFilePanelInProject("保存战斗配置", "BattleDebugPreset", "asset", "选择配置路径");
                    if (!string.IsNullOrEmpty(path))
                    {
                        var created = CreateInstance<BattleDebugPreset>(); created.SetExample();
                        AssetDatabase.CreateAsset(created, path); AssetDatabase.SaveAssets(); preset = created;
                        if (presetEditor != null) DestroyImmediate(presetEditor);
                    }
                }
                using (new EditorGUI.DisabledScope(preset == null))
                    if (GUILayout.Button("保存配置")) AssetDatabase.SaveAssets();
            }
            configScroll = EditorGUILayout.BeginScrollView(configScroll);
            if (preset != null)
            {
                UnityEditor.Editor.CreateCachedEditor(preset, null, ref presetEditor);
                presetEditor.OnInspectorGUI();
            }
            EditorGUILayout.EndScrollView();
        }

        private void DrawToolbar()
        {
            seedText = EditorGUILayout.TextField("随机种子 (uint)", seedText);
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("取时间种子")) seedText = RunFactory.CreateTimeSeed().ToString();
                using (new EditorGUI.DisabledScope(preset == null))
                    if (GUILayout.Button("开始新战斗"))
                    {
                        if (!uint.TryParse(seedText, out var seed)) error = "种子必须为 0 ～ 4294967295 的整数。";
                        else StartSession(JsonUtility.ToJson(preset, true), seed);
                    }
                using (new EditorGUI.DisabledScope(sessionJson == null))
                    if (GUILayout.Button("原配置 / 原种子重开")) StartSession(sessionJson, sessionSeed);
            }
        }

        private void StartSession(string json, uint seed)
        {
            BattleDebugPreset candidate = null;
            try
            {
                candidate = CreateInstance<BattleDebugPreset>(); candidate.hideFlags = HideFlags.HideAndDontSave;
                JsonUtility.FromJsonOverwrite(json, candidate);
                var nextCatalog = candidate.ToCore();
                var units = new List<UnitState> { nextCatalog.CreateUnit(candidate.playerId, "player", Team.Player, Control.Player) };
                for (int i = 0; i < candidate.enemyIds.Length; i++)
                    units.Add(nextCatalog.CreateUnit(candidate.enemyIds[i], "enemy-" + (i + 1), Team.Enemy, Control.Ai));
                var nextInventory = new Inventory(nextCatalog, candidate.items.Length);
                foreach (var item in candidate.items)
                {
                    if (item.count < 0 || (item.count > 0 && !nextInventory.TryAdd(item.id, item.count)))
                        throw new ArgumentException("道具数量或容量无效：" + item.id);
                }
                var nextBattle = new Battle(nextCatalog, units, new SeededRandom(seed));
                if (snapshot != null) DestroyImmediate(snapshot);
                snapshot = candidate; candidate = null;
                catalog = nextCatalog; inventory = nextInventory; battle = nextBattle;
                sessionJson = json; sessionSeed = seed; targets.Clear(); log.Clear();
                Append("种子：" + seed + "。相同配置、种子及操作顺序可复现本场战斗。");
                Execute("开始战斗", () => battle.Start());
            }
            catch (Exception ex) { error = "创建战斗失败：" + ex.Message; }
            finally { if (candidate != null) DestroyImmediate(candidate); }
        }

        private static IEnumerable<SkillState> Skills(UnitState unit)
        {
            if (unit.Weapon != null) yield return unit.Weapon;
            foreach (var skill in unit.Skills) yield return skill;
        }
        private string Name(string id)
        {
            var unit = battle?.Units.FirstOrDefault(x => x.Id == id);
            return unit == null ? snapshot.NameOf(id) : snapshot.NameOf(unit.Definition.Id) + " [" + id + "]";
        }
        private void DrawBattle()
        {
            EditorGUILayout.LabelField($"第 {battle.Round} 轮 · {PhaseName(battle.Phase)} · 结果：{OutcomeName(battle.Outcome)}", EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"本场种子：{sessionSeed}    当前行动：{Name(battle.ActiveUnit?.Id)}");
            if (battle.Failure != null) EditorGUILayout.HelpBox(battle.Failure.ToString(), MessageType.Error);
            foreach (var unit in battle.Units)
            {
                using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    EditorGUILayout.LabelField(Name(unit.Id) + (unit.IsDead ? "（已死亡）" : "") + (unit.IsSummoned ? "（召唤物）" : ""), EditorStyles.boldLabel);
                    EditorGUILayout.LabelField($"生命 {unit.Health}/{unit.MaxHealth}    法力 {unit.Mana}/{unit.MaxMana}    护甲 {unit.Armor}    行动点 {unit.Actions}");
                    EditorGUILayout.LabelField($"攻击 {unit.GetStat(Stat.Attack):0.##}    防御 {unit.GetStat(Stat.Defense):0.##}    力量 {unit.GetStat(Stat.Strength):0.##}    敏捷 {unit.GetStat(Stat.Agility):0.##}    智力 {unit.GetStat(Stat.Intelligence):0.##}");
                    EditorGUILayout.LabelField("状态：" + (unit.Statuses.Count == 0 ? "无" : string.Join("，", unit.Statuses.Select(x => Name(x.Id) + " ×" + x.Stacks))), EditorStyles.wordWrappedLabel);
                    EditorGUILayout.LabelField("技能：" + string.Join("，", Skills(unit).Select(x => $"{Name(x.Id)} Lv{x.Level} / 冷却{x.Cooldown}")), EditorStyles.wordWrappedLabel);
                    var intent = battle.Intents.FirstOrDefault(x => x.UnitId == unit.Id);
                    if (intent != null) EditorGUILayout.LabelField($"意图：{Name(intent.SkillId)} → {Name(intent.TargetId)}", EditorStyles.wordWrappedLabel);
                }
            }
            bool playerTurn = battle.Phase == BattlePhase.AwaitingPlayer;
            EditorGUILayout.LabelField("玩家技能（按钮旁显示 Core 判定结果）", EditorStyles.boldLabel);
            foreach (var skill in Skills(battle.Player).ToArray())
            {
                var definition = catalog.Skills[skill.Id];
                using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    string target = DrawTarget("skill:" + skill.Id, definition.Effects, battle.ValidTargetIds(skill.Id));
                    var check = battle.CanUseSkill(battle.Player.Id, skill.Id, target);
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        using (new EditorGUI.DisabledScope(check != CommandError.None))
                            if (GUILayout.Button(Name(skill.Id), GUILayout.Width(105)))
                            {
                                Execute($"使用技能 {skill.Id}，目标 {target ?? "自动"}", () => battle.UseSkill(battle.Player.Id, skill.Id, target));
                                GUIUtility.ExitGUI();
                            }
                        EditorGUILayout.LabelField($"{battle.Player.SkillActionCost(skill.Id)} 行动 / {battle.Player.SkillResourceCost(skill.Id)} {(definition.Resource == Resource.Mana ? "法力" : "生命")} / 冷却 {skill.Cooldown}（施放后 {battle.Player.SkillCooldown(skill.Id)}） · {ErrorName(check)}", EditorStyles.wordWrappedLabel);
                    }
                }
            }
            EditorGUILayout.LabelField("道具", EditorStyles.boldLabel);
            foreach (var item in snapshot.items)
            {
                var definition = catalog.Items[item.id];
                string target = DrawTarget("item:" + item.id, definition.Effects, battle.Units.Where(x => !x.IsDead).Select(x => x.Id).ToArray());
                using (new EditorGUI.DisabledScope(!playerTurn || inventory.Count(item.id) == 0))
                    if (GUILayout.Button($"使用 {Name(item.id)} ×{inventory.Count(item.id)}"))
                    {
                        Execute($"使用道具 {item.id}，目标 {target ?? "自动"}", () => battle.UseItem(battle.Player.Id, inventory, item.id, target));
                        GUIUtility.ExitGUI();
                    }
            }
            using (new EditorGUILayout.HorizontalScope())
            {
                using (new EditorGUI.DisabledScope(!playerTurn))
                {
                    if (GUILayout.Button("结束玩家回合")) { Execute("结束玩家回合", () => battle.EndTurn(battle.Player.Id)); GUIUtility.ExitGUI(); }
                    if (GUILayout.Button($"逃跑（{catalog.Rules.EscapeCost} 行动）")) { Execute("逃跑", () => battle.Escape(battle.Player.Id)); GUIUtility.ExitGUI(); }
                }
                using (new EditorGUI.DisabledScope(battle.Phase != BattlePhase.AwaitingAdvance))
                    if (GUILayout.Button("单步推进敌人 / 跳过回合")) { Execute("单步推进", () => battle.Advance()); GUIUtility.ExitGUI(); }
            }
            EditorGUILayout.HelpBox("一次推进执行一个 AI 或被跳过的单位回合。逃跑需继续推进本轮剩余回合才会结算。", MessageType.None);
        }

        private string DrawTarget(string key, IReadOnlyList<EffectDefinition> effects, IReadOnlyList<string> ids)
        {
            if (!effects.Any(x => x.Target == Target.SelectedEnemy || x.Target == Target.SelectedAlly)) return null;
            if (ids.Count == 0) { EditorGUILayout.LabelField("无合法目标"); return null; }
            targets.TryGetValue(key, out string selected);
            int index = Math.Max(0, Array.IndexOf(ids.ToArray(), selected));
            index = EditorGUILayout.Popup("目标", index, ids.Select(Name).ToArray());
            return targets[key] = ids[index];
        }

        private Dictionary<string, string> StateLines()
        {
            var result = new Dictionary<string, string>();
            foreach (var unit in battle.Units)
                result[unit.Id] = $"HP={unit.Health} MP={unit.Mana} 护甲={unit.Armor} AP={unit.Actions}; " +
                    string.Join(", ", Skills(unit).Select(x => x.Id + "冷却=" + x.Cooldown)) + "; 状态=" +
                    string.Join(", ", unit.Statuses.Select(x => x.Id + ":" + x.Stacks));
            result["背包"] = string.Join(", ", inventory.Items.Select(x => x.Key + ":" + x.Value));
            return result;
        }
        private void Execute(string command, Func<CommandResult> action)
        {
            error = null;
            Append("\n> " + command);
            var before = StateLines();
            try
            {
                var result = action();
                foreach (var e in result.Events) Append(Describe(e) + "\n    RAW " + JsonUtility.ToJson(new RawEvent(e)));
                if (!result.Success) { error = ErrorName(result.Error); Append("指令失败：" + error + " (" + result.Error + ")"); }
                foreach (var pair in StateLines())
                    if (!before.TryGetValue(pair.Key, out var previous) || previous != pair.Value)
                        Append($"变化 {Name(pair.Key)}：{previous ?? "新增"} → {pair.Value}");
                Append($"阶段：{PhaseName(battle.Phase)}；结果：{OutcomeName(battle.Outcome)}");
                if (battle.Failure != null) Append(battle.Failure.ToString());
            }
            catch (Exception ex) { error = ex.Message; Append(ex.ToString()); }
            Repaint();
        }
        private string Describe(GameEvent e)
        {
            string source = Name(e.SourceId), target = Name(e.TargetId), definition = Name(e.DefinitionId);
            string message;
            switch (e.Kind)
            {
                case EventKind.BattleStarted: message = "战斗开始"; break;
                case EventKind.RoundStarted: message = $"第 {e.Amount} 轮开始"; break;
                case EventKind.TurnStarted: message = $"{source} 回合开始（行动点 {e.Amount}）"; break;
                case EventKind.TurnEnded: message = $"{source} 回合结束"; break;
                case EventKind.SkillUsed: message = $"{source} 使用技能 {definition} → {target}"; break;
                case EventKind.ItemUsed: message = $"{source} 使用道具 {definition} → {target}"; break;
                case EventKind.Damage: message = $"{source} → {target}：生命伤害 {e.Amount}，护甲吸收 {e.ArmorDamage}（{definition}）"; break;
                case EventKind.Health: message = $"{target} 生命变为 {e.Amount}（{definition}）"; break;
                case EventKind.Mana: message = $"{target} 法力变为 {e.Amount}（{definition}）"; break;
                case EventKind.Armor: message = $"{target} 护甲变为 {e.Amount}（{definition}）"; break;
                case EventKind.Actions: message = $"{target} 行动点变为 {e.Amount}"; break;
                case EventKind.StatusChanged: message = $"{target} 状态 {definition} 变为 {e.Amount} 层"; break;
                case EventKind.UnitDied: message = $"{target} 死亡"; break;
                case EventKind.UnitRevived: message = $"{target} 复活"; break;
                case EventKind.Summoned: message = $"{source} 召唤 {target}"; break;
                case EventKind.BattleEnded: message = "战斗结束：" + OutcomeName((BattleOutcome)e.Amount); break;
                default: message = e.Kind + " " + source + " → " + target + " " + definition + " " + e.Amount; break;
            }
            return $"#{e.Sequence} {message}";
        }
        private void Append(string line)
        {
            log.Add(line);
            logText = string.Join("\n", log);
            if (followLog) logScroll.y = float.MaxValue;
        }
        private void DrawLog()
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("战斗日志", EditorStyles.boldLabel, GUILayout.Width(65));
                showRaw = GUILayout.Toggle(showRaw, "原始事件"); followLog = GUILayout.Toggle(followLog, "跟随最新");
                using (new EditorGUI.DisabledScope(log.Count == 0))
                {
                    if (GUILayout.Button("复制完整日志")) EditorGUIUtility.systemCopyBuffer = ExportText();
                    if (GUILayout.Button("导出日志"))
                    {
                        string path = EditorUtility.SaveFilePanel("导出战斗日志", "", "Battle-" + sessionSeed, "txt");
                        if (!string.IsNullOrEmpty(path))
                            try { File.WriteAllText(path, ExportText()); } catch (Exception ex) { error = ex.Message; }
                    }
                }
            }
            logScroll = EditorGUILayout.BeginScrollView(logScroll, GUILayout.Height(210));
            string visible = showRaw ? logText : string.Join("\n", logText.Split('\n').Where(x => !x.StartsWith("    RAW ", StringComparison.Ordinal)));
            EditorGUILayout.SelectableLabel(visible, EditorStyles.wordWrappedLabel,
                GUILayout.Height(Math.Max(190, EditorStyles.wordWrappedLabel.CalcHeight(new GUIContent(visible), Math.Max(200, position.width - 365)))));
            EditorGUILayout.EndScrollView();
        }
        private string ExportText() => "Dungeon 战斗调试日志\n种子：" + sessionSeed + "\nUnity：" + Application.unityVersion +
            "\n配置快照：\n" + sessionJson + "\n操作及事件：\n" + logText;
        private static string PhaseName(BattlePhase value) => value == BattlePhase.AwaitingPlayer ? "等待玩家操作" :
            value == BattlePhase.AwaitingAdvance ? "等待单步推进" : value == BattlePhase.Finished ? "已结束" : value == BattlePhase.Faulted ? "执行故障" : "未开始";
        private static string OutcomeName(BattleOutcome value) => value == BattleOutcome.Victory ? "胜利" : value == BattleOutcome.Defeat ? "战败" : value == BattleOutcome.Escaped ? "已逃跑" : "未结算";
        private static string ErrorName(CommandError value)
        {
            switch (value)
            {
                case CommandError.None: return "可用";
                case CommandError.WrongPhase: return "当前阶段不能操作";
                case CommandError.WrongActor: return "尚未轮到该单位";
                case CommandError.Cooldown: return "技能冷却中";
                case CommandError.InsufficientResource: return "生命或法力不足";
                case CommandError.InsufficientActions: return "行动点不足";
                case CommandError.InvalidTarget: return "目标无效";
                case CommandError.PassiveSkill: return "被动技能不可主动使用";
                case CommandError.WrongScope: return "不能在战斗中使用";
                case CommandError.Condition: return "不满足使用条件";
                case CommandError.MissingItem: return "道具不足";
                case CommandError.ExecutionLimit: return "超过事件执行上限";
                case CommandError.ExecutionFailed: return "规则执行失败";
                default: return value.ToString();
            }
        }
        [Serializable] private sealed class RawEvent
        {
            public long sequence;
            public string kind, source, target, definition;
            public int amount, armorDamage;
            public RawEvent(GameEvent e) { sequence = e.Sequence; kind = e.Kind.ToString(); source = e.SourceId; target = e.TargetId; definition = e.DefinitionId; amount = e.Amount; armorDamage = e.ArmorDamage; }
        }
    }
}
