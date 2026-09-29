# Core 评审修复记录

依据用户提供的 R01–R16 评审意见修改。代码现位于 `Assets/Scripts/Core`；独立编译项目和地图调试台的默认 SO 路径已同步。没有新增、编写或运行测试。

## 修复对应

| 编号 | 修改 | 关键文件 |
| --- | --- | --- |
| R01 | 已终局 Run 仍允许失败战斗收尾；Session 清理战斗引用、待处理地块并进入 Defeat。 | Campaign、Exploration |
| R02 | 原始奖励单位与存活召唤物分开保存；逃跑后再次组队包含召唤物，奖励仍排除召唤物。DTO 升至 v2。 | Exploration、State、Persistence |
| R03 | 战斗与探索都先快照层数、扣除消费，再执行效果；不足消费层数不触发。 | Battle、WorldActions |
| R04 | 探索事件、道具、技能及装卸/技能选择效果使用原位回滚边界；保留单位、技能、状态实例身份，失败不消耗、不领奖、不解锁。 | ExplorationTransaction、WorldActions、Interactions、Customization、Economy |
| R05 | Start 前发布 CurrentBattle；普通异常转为 Faulted + ExecutionFailed，保存 Failure 诊断，开战返回 BattleFailed。 | Battle、Exploration、State |
| R06 | 放弃检查终止结果；正常结束效果失败则调用无触发器清理，再确认所有权释放后关闭 Session。 | Campaign、Exploration |
| R07 | 所有冷却恢复入口统一枚举 AllSkills，包含武器。 | Battle、WorldActions、Economy |
| R08 | 扣 HP 时记录本次伤害是否致死，本层 Kill 不再由后续回调后的状态推断。 | Battle |
| R09 | 探索执行器记录已解析死亡，仅在恢复存活时清除，覆盖效果和 HP 支付致死。 | WorldActions |
| R10 | 死亡附加施放显式传递 allowDead；探索内部施放保留 resolvingDeath 上下文。 | Battle、WorldActions |
| R11 | 每个技能/遗物触发器显式传入自身 SkillState，避免同 ID 串用计数器。 | Battle |
| R12 | 最终 SelectedEnemy/SelectedAlly 解析检查阵营及战斗成员身份；无选择目标技能不列出无关单位。 | Battle、Campaign |
| R13 | 探索主动效果递归检查 CastSkill 引用，提供引用链诊断；主动内部非法施放不再静默返回。共享被动战斗专用效果仍不激活。 | WorldActions、Content |
| R14 | 会话恢复商店核对配置定价，不信任 DTO 的免费价格覆盖；库存及刷新次数继续恢复。 | Persistence |
| R15 | SkillBook.Owner 与 Session.Player 按实例身份校验。 | Customization、Exploration |
| R16 | Session 绑定所属 Run 的操作许可；所有探索/交互命令统一门禁，终局清除交互，恢复时重新绑定，清理与保存仍可进行。 | Campaign、Exploration、Interactions、WorldActions、Persistence |

## 接口和行为变化

- `Battle.Failure` 保留异常对象供诊断；新增 `CommandError.ExecutionFailed`、`ExplorationError.BattleFailed`。调用方应分别展示“非法操作”和“执行故障”，故障后调用 `GameRun.Abandon()` 或恢复检查点。
- `ExplorationExecutionLimitException` 是可识别的探索执行上限异常。探索异常会在原位回滚后重新抛出，调用方可以展示诊断；没有吞掉错误并伪装成功。
- 状态消费后再读取 SourceStat；触发等级仍用消费前快照。这一取舍防止有限次数触发无限重入。
- 存活敌方召唤物跨逃跑保留，普通敌人全死也不会留下空敌方遭遇。召唤物继续没有普通敌人的奖励资格。
- 存档 v2 新增遭遇召唤成员；v1 无法重建的丢失成员明确拒绝，而非复活原始敌人。商店定价冲突、Run/Profile 结算代不一致也会明确拒绝。

## 待确认风险的处理

F01、F02、F03、F04、F06 不作为已确认 Bug 擅自改变玩法。其现有语义和配置限制已写入 `Core-Implementation.md`：费用后置与附加施放豁免、装卸一次性事件、单位级符文事件、首次结算时读取 KillCoins、无隐藏运行状态的 AI 策略。

F05 增加 Run/Profile 恢复时的结算记录一致性检查；文件级原子提交仍属于 Unity/I/O 适配层。本轮没有实现自动合并不一致存档或持续装备属性贡献系统。

## 检查范围

仅做代码静态核对、独立 Core 编译与 Unity 脚本编译检查，不执行评审中的操作序列，不将编译成功表述为运行验证通过。R01–R16 的修改对应见上表。

本轮编译结果：独立 Core 编译 0 错误、0 警告；Unity 完成脚本重载且无编译错误。Unity 仍提示 4 条 DTO 字典字段不受原生序列化支持的 UAC1009 警告，以及 HexReveal 中 4 条旧查找 API 的 CS0618 警告。Core DTO 应由支持字典的外部序列化器处理，不能直接交给 JsonUtility；本轮未通过隐藏警告改变这一边界，也未修改无关的 HexReveal 表现脚本。
