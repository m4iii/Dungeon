# Dungeon Core 实现与接入

更新：2026-09-28。

## 边界

`Assets/Scripts/Core` 是纯 C# 游戏规则程序集，使用 C# 9 / .NET Standard 2.1。`Dungeon.Core.asmdef` 设置 `noEngineReferences: true`，没有 Unity 引用，也不读取 SO、文件、时间、输入、场景或视图。

**没有闪避和暴击。** 普通伤害与真实伤害、护甲、状态、反击等独立结算。

Unity 负责把 SO 转成下述不可变定义，将用户操作传入 Core，并根据返回事件及公开状态驱动表现。Core 自己保存 HP、MP、CD、状态、背包和进度，绝不把运行状态写回配置。

本次实现的是规则系统；原作 515 条技能结构配置等具体内容没有硬编码进 Core。Unity SO 类型、SO 导入转换、预制体、场景接线和界面不属于本次 Core 代码。

## 模块

| 文件 | 职责 |
| --- | --- |
| Definitions.cs | 属性、单位、技能、状态、道具、效果、事件、规则参数、配置引用校验 |
| State.cs | 单位/技能/状态实例、随机源、战斗指令结果与事件 |
| Battle.cs | 行动与冷却、双方回合、伤害/护甲、多段、状态、连锁、死亡/复活、反击、召唤、AI 意图、逃跑与结算 |
| Customization.cs | 符文准入与修正、技能计数条件、技能槽、锁定/排序/遗忘、技能石、符文道具、合成与出售 |
| Economy.cs | 背包、金币、钥匙、奖励掉落、商店、技能升级、遗物和休息 |
| WorldActions.cs | 探索阶段的技能、道具及装备/卸装/进入区域效果 |
| ExplorationTransaction.cs | 探索效果失败时原位回滚单位、技能、状态和背包，不替换运行实例 |
| Exploration.cs | 方格/六边形拓扑、揭露、遇敌、门/宝箱、休息、Boss 门禁、出口 |
| Content.cs | 权重池、地图生成、商品/技能/遗物候选、选择事件、定时与召唤 AI |
| ConnectedMapGenerator.cs | 可选不规则地图轮廓、固定地块接入、连通扩展、最远出口 |
| RoadRoomMapGenerator.cs | 主路与分支房间、必放内容、位置偏好、关键路径锁门约束、布局元数据 |
| Interactions.cs | 商店购买/刷新、技能/遗物选择、事件费用/奖励、交互完成与离开 |
| Progress.cs | 累计难度、敌人数值调整、经验、天赋、解锁、图鉴和成就记录 |
| Campaign.cs | 整局入口 GameRun、区域链、战前检查点、击杀统计、通关/失败/放弃、局外结算 |
| Persistence.cs | 单位、背包、地图、怪物组、商店、候选、整局、局外进度的保存与恢复 |

## 配置输入

按依赖顺序构造配置：

1. `GameCatalog`：`RuleSettings`、技能、状态、角色/怪物、道具、符文。
2. `WorldCatalog`：商店池、技能池、遗物池、选择事件；每个池由 SO 转为 `WeightedPool<T>`。
3. `MapDefinition`：固定地图；或调用 `MapGenerator.Generate`，传入 `MapGenerationDefinition`、拓扑与随机源。
4. `CampaignDefinition`：有序区域、内容版本、难度、经验、技能槽、掉落、初始背包、天赋。

配置使用稳定 ID 关联。贴图、音效、Prefab 等资源在 Unity 侧用相同 ID 建立映射。没有任何 Core API 接受 Unity 对象。

技能和状态用效果序列组合。`Magnitude` 的等级公式是 `Constant + Growth × L + SourceStat × (Coefficient + CoefficientGrowth × L)`，沿用参考规则的 `L`，而非 `L - 1`。例如 1 级剑的 `Magnitude(4, 2)` 得到 6。

条件支持状态层数、生命比例和技能实例计数器。`ChangeCounter` 的引用 ID 是计数器键，只允许在技能/符文上下文使用；计数器进入存档。`CastSkill` 可附加施放已学技能，也可引用配置中的独立子技能；附加施放不扣资源、不增加冷却，并受到连锁执行上限约束。

符文每个技能只装备一个，可限制技能类型、标签和单级技能，修改伤害、次数、资源、行动、冷却、状态层数，并提供触发效果。通过 `Progression.UseRune` 消耗背包中的符文道具；`EquipRune` 是直接装备入口，适合奖励发放或初始化。

技能石配置 `ItemUpgrade`，调用 `Progression.UseUpgrade` 在确认技能后消费。合成用 `CraftRecipe` + `Inventory.TryCraft`，输入和产出作为一次背包交易提交。普通药水用 `UseItem`；`Consumable` 控制是否消耗。

## 整局接入

```csharp
var run = new GameRun(runId, gameCatalog, campaign, worldCatalog,
    new HexTopology(), profile, heroId, difficulty: 0, seed: seed,
    policies: aiPolicies);

var exploration = run.Explore(new CellPosition(q, r));
var battle = run.Session.CurrentBattle;
if (battle != null)
{
    // 由 UI 在玩家操作时调用，不要在渲染刷新时自动执行。
    var result = battle.UseSkill(run.Player.Id, skillId, targetId);
    // result.Success / Error / Events 用于更新表现。
}
```

`GameRun` 是推荐的整局入口；`Battle`、`GameSession`、背包等也可独立使用。

战斗推进协议：

- `AwaitingPlayer`：`UseSkill`、`UseItem`、`EndTurn`、`Escape`。
- `AwaitingAdvance`：调用一次 `Advance`，执行一个 AI 或被跳过的单位回合。Unity 可在动画结束后再推进。
- `Finished`：调用 `run.ContinueAfterBattle()`。背包满时先整理背包再调用，已抽取奖励保持不变，已领取奖励不会再次发放。
- `Faulted`：循环效果超限或执行异常。`CommandResult.Error` 分别为 `ExecutionLimit` / `ExecutionFailed`，诊断保存在 `Battle.Failure`。状态可能已部分执行；调用 `run.Abandon()` 无触发器清理所有权，或恢复战前检查点。

`Explore()` 在开战前即登记战斗引用，开战执行失败返回 `ExplorationError.BattleFailed`，故障战斗仍可访问。战败后即使 Run 已同步为 `Defeat`，仍可调用 `ContinueAfterBattle()` 完成失败收尾和保存。终局会关闭所属 Session 的游戏命令，清理入口不受该门禁阻断；不要通过修改枚举恢复游戏。

`CanUseSkill`、`ValidTargetIds`、`Intents` 供 UI 查询。查询不消耗随机数。回合事件有单调序号，视图只消费结果，不反向决定伤害和状态生效时机。

探索交互：

- `Session.UseItem` / `UseSkill`：探索阶段使用；技能石和符文分别调用 `Progression.UseUpgrade` / `UseRune`。
- `Session.ActiveShop` / `Buy` / `RefreshShop`：打开、购买和刷新商店。
- `Session.ActiveOffers` / `ChooseOffer`：技能和遗物选择。
- `Session.EventOptions` / `ChooseEvent`：选择事件，检查费用、生命、解锁和背包空间后执行。
- `Session.LeaveInteraction(false)`：暂时关闭，保留商品/候选；`true` 表示完成/放弃该节点并揭露邻居。
- `run.Skills`：解锁技能槽、学习、遗忘、锁定及排序。
- `run.NextRegion()`：通过非最终区域出口后进入下一地图。最终出口使整局进入 `Victory`。
- `run.Synchronize()`：直接操作下层 Session/Battle 后同步整局终局状态。
- `run.Settle()`：将经验和难度解锁记入 Profile；同一 Run ID 只结算一次。

初始难度仅开放 0；通关逐级解锁，具体上限由 `DifficultyRules` 决定。角色/技能职业候选由传入的池确定，跨角色解锁条件可在构造候选池时结合 `PlayerProfile` 过滤。成就和图鉴提供独立持久记录入口，具体展示及文案不进入 Core。

## 已明确的规则取舍

- 不保留原作暴击、闪避及对应累计点。
- 行动力为 0 不自动结束玩家回合，仍可使用满足条件的零行动消耗技能。
- 逃跑实际扣配置的行动消耗，完成本轮剩余敌方行动后返回探索。
- 退出战斗时按各单位自己的恢复属性结算；额外胜利冷却缩减来自规则配置。
- 武器与普通技能都参与回合结束、战后、休息及最长冷却缩减，带冷却符文的武器不会永久锁死。
- 正常伤害经过输出、承伤修正与护甲；真实伤害绕过普通修正和护甲。
- 数值先保留 3 位小数，再向零截断为整数；派生生命至少为 1，魔力至少为 0。
- 嘲讽约束合法敌方目标；AI 在合法候选中优先标记目标。随机范围伤害不额外受单体选择的嘲讽限制。
- 触发顺序：状态 → 遗物 → 武器 → 槽位技能及其符文。附加施放与反击仍发出明确的施放事件；反击不再连锁反击。
- 消费性状态先保存触发层数，再扣层，最后按保存的层数执行。嵌套触发不能复用已消费层数；效果读取的 SourceStat 使用扣层后的属性。同一死亡只解析一次；复活后再次死亡可重新触发。死亡附加施放沿用受限死亡上下文，普通死亡单位仍不能主动操作。
- 新召唤和复制召唤占用 Core 的队伍容量，并从下一全局轮开始行动；不查询场景对象数量。
- 逃跑后保留原敌人以及存活敌方召唤物的生命、资源与持久状态。再次参战使用全部存活成员，奖励仍只计算原始敌人，召唤物不产生普通击杀经验或掉落。整局击杀统计按实例 ID 去重。
- 地图生成提供两种模式：不传 `MapGenerationDefinition.Shape` 时保留完整坐标填充与连通校验；传入 `MapShapeDefinition` 时，以 Positions 为最大边界，连接固定地块并从入口扩展到指定可通行格数，生成不规则轮廓。形状参数包含格数、紧凑度、分支倾向和自动最远出口。后者跳过随机池 Blocked 条目，以未生成格形成空缺，固定障碍仍作为禁止扩展区域。配置无法连通或格数不足时报错，不自动重抽种子。保证的是拓扑可达，不是钥匙获取顺序。
- 构造配置/恢复存档时的错误抛异常，普通非法操作返回失败；战斗执行异常转为可清理的 Faulted。探索事件、道具、主动技能及装备选择的效果失败会原位回滚状态并保留异常诊断，执行超限使用 `ExplorationExecutionLimitException`。失败时不提交解锁和地块完成。
- 探索主动效果沿 CastSkill 引用链进行可执行性校验，禁止不支持的效果、被动施放及静态引用环；`WorldActions.ExplorationError` 可查询具体链路。共享被动触发器中战斗专用效果（包括指向战斗专用效果的附加施放）在探索中不激活。

评审 F01–F04/F06 涉及玩法语义，本轮保留现有规则并明确边界：

| 项目 | 当前语义 |
| --- | --- |
| F01 费用和豁免 | 玩家普通施放预检 AP/资源/CD/状态/场景；AP 先扣，资源及 CD 在主体效果后结算。AI 普通施放忽略 AP/资源资格，但仍扣资源并最低截到 0。附加施放与反击不扣 AP/资源、不增加 CD，绕过一般费用/CD/前置状态；附加施放仍要求合法目标和主动技能。探索公开入口限制场景，内部复用技能按效果可执行性判断。 |
| F02 装卸属性 | Equipped/Unequipped 是各自独立的一次性事件，ChangeStat 是永久基础值修改；不是持续属性贡献系统。不要用按当前等级计算的反向 ChangeStat 表达可升级装备的属性撤销。若需要该玩法，需另加贡献快照和迁移，当前没有自动撤销/升级差量功能。 |
| F03 符文事件 | 符文触发器是单位级事件订阅；伤害/次数/消耗等修正只作用于所属技能。事件计数器严格绑定各自实例。 |
| F04 额外金币 | KillCoins 仍在胜利奖励首次抽取时读取，战后已移除的临时状态不提供该加成；逃跑前击杀也按最终首次结算时的值计算，未改为逐次击杀快照。 |
| F06 AI 状态 | IAiPolicy 必须是无隐藏可变局内状态的策略，使用 AiContext、已有单位/技能状态及注入随机源决策；Core 不保存策略对象字段。 |

## 存档

`Capture()` 返回普通 C# DTO，Unity 适配层选择 JSON/二进制格式并进行文件读写。DTO 包含字典，不能直接假定 Unity `JsonUtility` 能完整序列化；选择支持字典的序列化器，或在适配层转换。

支持探索、区域出口及完成战斗结算后的状态保存。**不保存战斗中途的队列和意图。** `run.Explore()` 在有效遇敌动作前保存 `run.Checkpoint`；UI 可先将此 DTO 持久化，再开始动画。战斗胜利但背包尚未完成领奖时也保留战前检查点。

恢复调用 `GameRun.Restore`，传入同版本的配置、拓扑、AI 策略和 `PlayerProfile`。恢复不会重新触发装备、天赋或进入区域奖励，不重新抽取现有商品和候选。随机状态一同恢复。

整局保存的 `ContentVersion` 必须匹配 `CampaignDefinition`。配置变更后由适配层迁移 DTO 再恢复；Core 不猜测已删除 ID 的替代项。随机地图使用 `CampaignDefinition.regionGenerators`（此时 `regions` 传 null），由本局 `InitialSeed`、区域 ID 和序号派生独立地图种子。新局通过 Unity 适配层 `RunFactory.StartNew` 取 UTC 时间种子；`Capture` 保存初始种子和运行中 `RandomState`。`Restore` 使用初始种子重建当前区域，再恢复探索状态；后续区域同样派生生成，不依赖战斗消耗的随机数。调试器调用同一 `RunSeeds.GenerateRegion` 入口。

配置、拓扑和算法版本必须一致才能复现。修改 SO 生成参数应同步提升 `ContentVersion`，修改生成或派生算法应提升 `RunSeeds.MapGenerationVersion`；不兼容存档拒绝恢复，不悄悄替换种子。SO 保存模板，地图默认不写回 SO；文件存档 I/O 仍由应用层负责。

地图生成版本 2 增加 `TerrainGenerationDefinition`。`CellDefinition.Terrain` 与 `Kind` 分开：草原、疏林、森林、浅水可以进入，高山和深水不可进入；统一以 `IsWalkable` 判断。旧 `CellKind.Blocked` 仍兼容。配置地形生成后，连通区域外补一圈实际阻挡格，封闭内洞同样补为阻挡，圈外才是地图外部。地形使用多源相邻扩散形成片区；深水岸旁的可通行格可转为浅水，不改变连通性或通行数量。事件和奖励不会被覆盖。固定或池条目可显式指定地形，阻挡地形禁止承载事件。调试器使用 Unity 素材显示地形并导出通行标记，Core 不包含任何贴图或 Unity 引用。

`PlayerProfile.Capture/Restore` 单独保存英雄经验、天赋点、解锁、图鉴、成就及已结算 Run ID。应用应一起持久化整局和局外快照，避免文件写入中断导致两份数据版本不同。

地图生成版本 3 新增 `MapGenerationDefinition.RoadRooms`，与 Shape 一同启用。主路先连接入口、出口和固定地点，再扩展分支房间。Shape.CellCount 在此模式为最低通行数量，必放内容与空地比例可增大地图。RequiredMapContent 保证额外数量；内容按首领深处、奖励尽头/分支、战斗入口/奖励邻位、商店休息较近等偏好安排。自动锁门避开关键路径，固定布局冲突则拒绝生成。连通性在构造时保持并在结束后检查，不复制原作的递归拆墙修复或不稳定容器遍历。支持六边形和四邻接方格。MapDefinition.Layout 提供主路、房间入口、房间编号和最短距离，地形阶段保留，读档按种子重建。旧生成版本拒绝恢复。

Run DTO 当前版本为 **3**，增加初始种子、生成模式、生成版本和拓扑标识；Session、Unit DTO 仍为 **2**，包含遭遇 `Summons` 和单位 `IsSummoned`。固定地图模式仍接受可恢复的旧数据，旧档未知初始种子以 `HasInitialSeed=false` 表示；随机生成模式拒绝缺少初始种子的 v1/v2 存档。旧遭遇缺失召唤物数组视为空，但若未完成遭遇的原始敌人全死、没有任何存活成员，则明确拒绝恢复，需使用战前检查点，不能凭空补回旧存档丢失的召唤物。

会话商店恢复使用当前 OfferDefinition 和难度定价，检查旧 DTO 中的规则参数一致性，只恢复库存/刷新次数。Run/Profile 的 Settled 与已结算 Run ID 必须一致；不一致会拒绝恢复，不能把两份不同提交代的数据拼成一局。这是 F05 的一致性检查，不代替外部原子存储协议。

## 编译

```powershell
dotnet build Tools/CoreBuild/Dungeon.Core.csproj --configuration Release
```

该工程只包含 Core 源码，没有测试项目和外部 NuGet 包。本轮按要求没有新增或运行测试，并移除了先前创建的测试工程；独立编译仅确认语法、引用及程序集生成，不代表运行行为已经验证。
