# CubeDungenSource 配置索引

提取日期：2026-09-28。来源为用户指定的本地发布包。所有数值均为 SO 原始配置，不包含天赋、遗物、难度、运行时修正；不是新项目的最终平衡表。说明文本为配置原文，存在占位符、历史命名与旧描述，不能替代规则说明中的代码结论。

属性 ID 及等级算法见同目录 CubeDungenSource-Rules.md。L 是技能实际等级，原作通用参数采用 base + increment × L。事件 0 是主动执行，其他数字是触发时机。武器、遗物、怪物技能等也使用技能结构，因此“技能配置数”不是玩家可学习技能数。

## 英雄：7 条

| 名称 | ID | 原等级字段 | 基础HP | 基础MP | 基础属性 | 体型 | 普通攻击 | 初始技能@等级 | AI | 金币字段 |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 炼金术师 | MainPackage_Alchemist | 0 | 40 | 20 | 2101=4; 2102=5; 2103=5; 2201=0; 2002=0; 2202=0 | 1 | MainPackage_Weapen_3_BiShou | MainPackage_Skill_173_GeDang@1 |  | 0 |
| 刺客 | MainPackage_Assassin | 0 | 40 | 20 | 2101=4; 2102=7; 2103=3; 2201=0; 2002=0; 2202=0 | 1 | MainPackage_Weapen_3_BiShou | MainPackage_Skill_173_GeDang@1 |  | 0 |
| 护卫 | MainPackage_Guard | 0 | 40 | 20 | 2101=6; 2102=2; 2103=4; 2201=0; 2002=2; 2202=0 | 1 | MainPackage_Weapen_21_ChangBingDao | MainPackage_Skill_173_GeDang@1 |  | 0 |
| 暗影术士 | MainPackage_Lich | 0 | 20 | 40 | 2101=6; 2102=3; 2103=6; 2201=0; 2002=0; 2202=0 | 1 | MainPackage_Weapen_6_SiLingShu | MainPackage_Skill_173_GeDang@1 |  | 0 |
| 魔剑士 | MainPackage_MagicSword | 0 | 40 | 20 | 2101=4; 2102=4; 2103=6; 2201=0; 2002=0; 2202=0 | 1 | MainPackage_Weapen_5_MoJian | MainPackage_Skill_173_GeDang@1; MainPackage_Skill_315_DianNengZhuRu@1 |  | 0 |
| 法师 | MainPackage_Magician | 0 | 40 | 20 | 2101=4; 2102=2; 2103=8; 2201=0; 2002=0; 2202=0 | 1 | MainPackage_Weapen_2_FaZhang | MainPackage_Skill_173_GeDang@1; MainPackage_Skill_125_AoShuFeiDan@1 |  | 0 |
| 战士 | MainPackage_Warrior | 0 | 40 | 20 | 2101=7; 2102=4; 2103=3; 2201=0; 2002=0; 2202=0 | 1 | MainPackage_Weapen_1_Jian | MainPackage_Skill_173_GeDang@1 |  | 0 |

## 非英雄单位（含召唤物与特殊单位）：60 条

| 名称 | ID | hardLevel | 基础HP | 基础MP | 基础属性 | 体型 | 普通攻击 | 技能@等级 | AI | 击杀金币 |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 野狼 | MainPackage_monster_1001_Wolf | 0 | 25 | 100 | 2001=12; 2002=0 | 1 | SkillData_MonsterNormalAttack | MainPackage_Skill_1001_TongJi@1; SkillData_MonsterNormalDefense@2 |  | 5 |
| 野猪 | MainPackage_monster_1002_WildBoar | 0 | 40 | 100 | 2001=10; 2002=0 | 1 | SkillData_MonsterNormalAttack | SkillData_MonsterNormalDefense@2; MainPackage_Skill_44_ZhenFen@1 |  | 5 |
| 巨蟒 | MainPackage_monster_1003_Python | 0 | 30 | 100 | 2001=12; 2002=0 | 1 | SkillData_MonsterNormalAttack | MainPackage_Skill_1004_ChanRao@1; MainPackage_Skill_1073_JiaoSha@2 |  | 5 |
| 巨熊 | MainPackage_monster_1004_Bear | 1 | 55 | 100 | 2001=15; 2002=2 | 2 | SkillData_MonsterNormalAttack | SkillData_MonsterNormalDefense@3; MainPackage_Skill_28_ShangTongGanZhi@1; MainPackage_Skill_1005_PaoXiao@1 |  | 15 |
| 森林巨猿 | MainPackage_monster_1005_GiantApe | 2 | 150 | 100 | 2001=18; 2002=3 | 2 | SkillData_MonsterNormalAttack | MainPackage_Skill_1018_ChenZhongDaJi@2; SkillData_MonsterNormalDefense@7; MainPackage_Skill_44_ZhenFen@3 |  | 30 |
| 食人花 | MainPackage_monster_1006_Chomper | 2 | 180 | 100 | 2001=10; 2002=4 | 2 | SkillData_MonsterNormalAttack | MainPackage_Skill_78_DuCiShu@1; SkillData_MonsterNormalDefense@6; MainPackage_Skill_12_JingJiJia@1 |  | 30 |
| 奇美拉 | MainPackage_monster_1007_Chimera | 2 | 180 | 100 | 2001=20; 2002=3 | 2 | SkillData_MonsterNormalAttack | MainPackage_Skill_146_SuanYePenShe@3; MainPackage_Skill_68_HuoQiuShu@4; SkillData_MonsterNormalDefense@7; MainPackage_Skill_1075_DianLiuHuDun@1 |  | 30 |
| 劣等血族 | MainPackage_monster_10_LieDengXueZu | 1 | 100 | 100 | 2001=20; 2002=15 | 1 | SkillData_MonsterNormalAttack | MainPackage_Skill_1039_CiDengShengMingTouQu@2; SkillData_MonsterNormalDefense@6 |  | 10 |
| 小花妖 | MainPackage_monster_2001_HuaYao | 0 | 40 | 20 | 2001=5; 2002=0 | 1 | SkillData_MonsterNormalAttack | MainPackage_Skill_1043_XuRuoHuaFen@1; MainPackage_Skill_12_JingJiJia@1; SkillData_MonsterNormalDefense@3 |  | 5 |
| 蘑菇怪 | MainPackage_monster_2002_MoGuGuai | 0 | 60 | 20 | 2001=20; 2002=3 | 1 | SkillData_MonsterNormalAttack | SkillData_MonsterNormalDefense@6; MainPackage_Skill_1042_QinShiBaoZi@2; MainPackage_Skill_1074_ChongZhuang@1 |  | 5 |
| 人马 | MainPackage_monster_2003_RenMa | 0 | 100 | 15 | 2001=12; 2002=0 | 1 | SkillData_MonsterNormalAttack | MainPackage_Skill_1044_WuMouChongFeng@3 |  | 5 |
| 狼人 | MainPackage_monster_2004_LangRen | 1 | 80 | 25 | 2001=30; 2002=10 | 1 | SkillData_MonsterNormalAttack | MainPackage_Skill_1005_PaoXiao@1; MainPackage_Skill_50_HuoLiZhan@4; MainPackage_Skill_1041_ShangKouTianXue@2 |  | 15 |
| 影魔 | MainPackage_monster_2005_YingMo | 2 | 280 | 50 | 2001=20; 2002=6 | 1 | SkillData_MonsterNormalAttack | MainPackage_Skill_1003_DuoChongDaJi@2; MainPackage_Skill_163_HuoYanFuMo@1 |  | 30 |
| 丛林守护者 | MainPackage_monster_2006_ShuJuRen | 2 | 400 | 120 | 2001=20; 2002=15 | 2 | SkillData_MonsterNormalAttack | MainPackage_Skill_1045_ShenLinZhiNu@2; MainPackage_Skill_1046_SenLinZhiYou@1; MainPackage_Skill_249_ErLianJi@4 | Behavior_ShuJuRen_2006 | 30 |
| 绿龙 | MainPackage_monster_2007_LvLong | 2 | 380 | 60 | 2001=15; 2002=5 | 3 | SkillData_MonsterNormalAttack | MainPackage_Skill_72_DiYuYan@1; MainPackage_Skill_164_HuoYanQingHe@1; MainPackage_Skill_1050_LieYanHuShen@1; MainPackage_Skill_1076_LongXueFeiTeng@1 |  | 30 |
| 毒蝇 | MainPackage_monster_3001_LvYing | 0 | 50 | 20 | 2001=10; 2002=2 | 1 | SkillData_MonsterNormalAttack | MainPackage_Skill_1015_ChuanBoJiBing@1; SkillData_MonsterNormalDefense@3 |  | 5 |
| 毒蛙 | MainPackage_monster_3002_DuWa | 0 | 70 | 20 | 2001=13; 2002=0 | 1 | SkillData_MonsterNormalAttack | MainPackage_Skill_1016_DuZhiXue@2 |  | 5 |
| 毒蜥 | MainPackage_monster_3003_DuXi | 0 | 75 | 20 | 2001=10; 2002=2 | 1 | SkillData_MonsterNormalAttack | MainPackage_Skill_1069_JiXu@1; SkillData_MonsterNormalDefense@4 |  | 5 |
| 泥沼触手 | MainPackage_monster_3004_NiZhaoChuShou | 1 | 100 | 30 | 2001=20; 2002=0 | 1 | SkillData_MonsterNormalAttack | MainPackage_Skill_1003_DuoChongDaJi@2; MainPackage_Skill_44_ZhenFen@2; MainPackage_Skill_1014_RouRen@1 |  | 15 |
| 鳄龙 | MainPackage_monster_3005_ELong | 2 | 400 | 50 | 2001=15; 2002=15 | 1 | SkillData_MonsterNormalAttack | MainPackage_Skill_1010_XuePenDaKou@1; MainPackage_Skill_1011_QianFu@1; MainPackage_Skill_1012_WeiSuo@1; MainPackage_Skill_1013_ShiTan@3; SkillData_MonsterNormalDefense@6 | Behavior_ELong_3005 | 30 |
| 蛇头 | MainPackage_monster_30061_SheTou | 0 | 40 | 0 | 2001=15; 2002=0 | 1 | SkillData_MonsterNormalAttack | MainPackage_Skill_230_ZhuDuCiJi@1; SkillData_MonsterNormalDefense@2 |  | 0 |
| 堕落祭祀 | MainPackage_monster_30062_DuoLuoJiSi | 1 | 200 | 200 | 2001=10; 2002=5 | 1 | SkillData_MonsterNormalAttack | MainPackage_Skill_1049_FeiTeng@2; MainPackage_Skill_249_ErLianJi@5; SkillData_MonsterNormalDefense@10 |  | 0 |
| 多头蛇 | MainPackage_monster_3006_BaQi | 2 | 750 | 50 | 2001=20; 2002=5 | 2 | SkillData_MonsterNormalAttack | MainPackage_Skill_1017_DuoTouShengWu@1; MainPackage_Skill_231_XieTongGongJi@1 | Behavior_DuoTouShe_3006 | 30 |
| 影龙 | MainPackage_monster_3007_YingLong | 2 | 350 | 60 | 2001=25; 2002=10 | 1 | SkillData_MonsterNormalAttack | MainPackage_Skill_1008_ShiFaGanRao@1; MainPackage_Skill_1007_ZhouFu@1; MainPackage_Skill_1009_YingJia@2 |  | 30 |
| 淤泥巨物 | MainPackage_monster_3008_YuNiJuWu | 2 | 240 | 200 | 2001=10; 2002=0 | 1 | SkillData_MonsterNormalAttack | MainPackage_Skill_1047_BuWenDingJuHeWu@1; MainPackage_Skill_1048_ChongXinNingJu@2; MainPackage_Skill_1049_FeiTeng@1 |  | 30 |
| 石堆 | MainPackage_monster_40001_ShiDui | 0 | 15 | 0 | 2001=-1; 2002=0 | 1 | SkillData_MonsterNormalAttack | MainPackage_Skill_1027_AiShi@3 |  | 0 |
| 淤泥怪 | MainPackage_monster_40002_YuNiGuai | 0 | 5 | 0 | 2001=10; 2002=0 | 1 | SkillData_MonsterNormalAttack | MainPackage_Skill_1027_AiShi@3 |  | 0 |
| 火元素 | MainPackage_monster_40003_HuoYuanSu | 0 | 10 | 10 | 2001=6; 2002=0 | 1 | SkillData_MonsterNormalAttack | MainPackage_Skill_163_HuoYanFuMo@1 |  | 0 |
| 冰墙 | MainPackage_monster_40004_BingQiang | 0 | 5 | 0 | 2001=-1; 2002=0 | 1 | SkillData_MonsterNormalAttack | MainPackage_Skill_250_BingLengQuTi@1; MainPackage_Skill_1027_AiShi@3 |  | 0 |
| 淤泥分裂体 | MainPackage_monster_40005_YuNiFenLieTi | 0 | 1 | 0 | 2001=0; 2002=0 | 1 | SkillData_MonsterNormalAttack | MainPackage_Skill_174_QuanShenZhuangJi@4 |  | 0 |
| 荆棘墙 | MainPackage_monster_40006_JingJiQiang | 0 | 10 | 0 | 2001=-1; 2002=5 | 1 | SkillData_MonsterNormalAttack |  |  | 0 |
| 莉莉丝 | MainPackage_monster_40007_LiLiSi | 0 | 10 | 200 | 2001=-1; 2002=0 | 1 | SkillData_MonsterNormalAttack | MainPackage_Skill_1070_MeiHuoZhiWen@1; MainPackage_Skill_1071_TongKuMeiHeng@1; MainPackage_Skill_1072_WuLiMeiHeng@1 |  | 0 |
| 骷髅 | MainPackage_monster_40008_KuLou | 0 | 0 | 5 | 2001=1; 2002=0 | 1 | SkillData_MonsterNormalAttack |  |  | 0 |
| 骷髅弓箭手 | MainPackage_monster_40008_KuLouGongJianShou | 0 | 0 | 5 | 2001=1; 2002=0 | 1 | SkillData_MonsterNormalAttack |  |  | 0 |
| 骸骨堆 | MainPackage_monster_40009_HaiGuDui | 0 | 0 | 0 | 2001=1; 2002=0 | 1 | SkillData_MonsterNormalAttack | MainPackage_Skill_1027_AiShi@5 |  | 0 |
| 死亡骑士 | MainPackage_monster_40010_SiWangQiShi | 0 | 0 | 0 | 2001=1; 2002=0 | 1 | SkillData_MonsterNormalAttack |  |  | 0 |
| 硬岩蛇 | MainPackage_monster_4001_YingYanShe | 0 | 100 | 60 | 2001=35; 2002=10 | 1 | SkillData_MonsterNormalAttack | MainPackage_Skill_1019_ChenShui@1; MainPackage_Skill_1006_RaoLuanChongJi@2; MainPackage_Skill_1004_ChanRao@1; SkillData_MonsterNormalDefense@10 |  | 5 |
| 石人 | MainPackage_monster_4002_ShiRen | 0 | 80 | 100 | 2001=30; 2002=10 | 1 | SkillData_MonsterNormalAttack | MainPackage_Skill_1019_ChenShui@1; MainPackage_Skill_1020_JuShiQuan@9; MainPackage_Skill_1004_DuoCengHuJia@3 |  | 5 |
| 大雕 | MainPackage_monster_4003_JuDiao | 0 | 120 | 100 | 2001=22; 2002=10 | 1 | SkillData_MonsterNormalAttack | MainPackage_Skill_49_ZhunBeiZiTai@2; MainPackage_Skill_1021_LianHuanTuXi@1; SkillData_MonsterNormalDefense@5 |  | 5 |
| 巨犀 | MainPackage_monster_4004_JuXi | 1 | 200 | 200 | 2001=30; 2002=20 | 1 | SkillData_MonsterNormalAttack | MainPackage_Skill_1022_JuJiaoZhuangJi@3; MainPackage_Skill_1023_JianTa@2; MainPackage_Skill_1014_RouRen@2 |  | 15 |
| 比蒙 | MainPackage_monster_4005_BiMeng | 2 | 1000 | 200 | 2001=40; 2002=10 | 1 | SkillData_MonsterNormalAttack | MainPackage_Skill_1024_LiZhuaQieGe@1; MainPackage_Skill_1025_ChaoQiangZaiSheng@3; MainPackage_Skill_1026_JuShiBaoJun@2; MainPackage_Skill_1027_YeManZhiXue@1 |  | 30 |
| 巨石灵 | MainPackage_monster_4006_ShiJuLing | 2 | 800 | 200 | 2001=25; 2002=35 | 2 | SkillData_MonsterNormalAttack | MainPackage_Skill_1018_ChenZhongDaJi@4; MainPackage_Skill_1065_TanTa@1; MainPackage_Skill_1066_YanShiTouZhi@1 | Behavior_ShiJuLing_4006 | 30 |
| 哥布林斥候 | MainPackage_monster_5001_GeBuLingChiHou | 0 | 100 | 100 | 2001=25; 2002=10 | 1 | SkillData_MonsterNormalAttack | MainPackage_Skill_1029_CuiDuDuanRen@1; MainPackage_Skill_1030_DanXiao@1; SkillData_MonsterNormalDefense@10 |  | 5 |
| 兽人战士 | MainPackage_monster_5002_ShouRenZhanShi | 0 | 240 | 60 | 2001=35; 2002=25 | 1 | SkillData_MonsterNormalAttack | MainPackage_Skill_44_ZhenFen@4; MainPackage_Skill_249_ErLianJi@6; MainPackage_Skill_1031_BuQu@1 |  | 5 |
| 兽人狼骑兵 | MainPackage_monster_5003_ShouRenLangQiBing | 0 | 200 | 100 | 2001=30; 2002=15 | 1 | SkillData_MonsterNormalAttack | MainPackage_Skill_1023_JianTa@3; SkillData_MonsterNormalDefense@15; MainPackage_Skill_1032_TaoWang@1 |  | 5 |
| 重锤食人魔 | MainPackage_monster_5004_ZhongChuiShiRenMo | 1 | 300 | 20 | 2001=35; 2002=0 | 2 | SkillData_MonsterNormalAttack | MainPackage_Skill_210_HuiMieZhanJi@2; MainPackage_Skill_99_HuLuanDaJi@12; MainPackage_Skill_201_WeiHeNuHou@2 |  | 15 |
| 哥布林之王 | MainPackage_monster_5005_GeBuLingZhiWang | 3 | 2000 | 350 | 2001=45; 2002=50 | 2 | SkillData_MonsterNormalAttack | MainPackage_Skill_1033_KuaiLaiHuJia@1; MainPackage_Skill_1034_TongYuHaoLing@3; MainPackage_Skill_1035_AnHeiZhiMao@1 | Behavior_GeBuLingZhiWang_5005 | 100 |
| 诡异水泡 | MainPackage_monster_6001_GuiYiShuiPao | 0 | 100 | 100 | 2001=6; 2002=0 | 1 | SkillData_MonsterNormalAttack | MainPackage_Skill_1051_ZiBao@1; MainPackage_Skill_1052_MoLiFuZhuo@1 | Behavior_GuiYiShuiPao_6001 | 5 |
| 幽影 | MainPackage_monster_6002_YouYing | 0 | 150 | 200 | 2001=20; 2002=5 | 1 | SkillData_MonsterNormalAttack | MainPackage_Skill_1053_JuanWei@2; MainPackage_Skill_1054_MoLiXiQu@2 |  | 5 |
| 星湖蟹 | MainPackage_monster_6003_XingHuXie | 0 | 120 | 200 | 2001=25; 2002=10 | 1 | SkillData_MonsterNormalAttack | MainPackage_Skill_1055_PoSuiJiaJi@1; MainPackage_Skill_1056_JuQianNianYa@4 |  | 5 |
| 星空使徒 | MainPackage_monster_6004_XingKongShiTu | 1 | 230 | 400 | 2001=25; 2002=5 | 1 | SkillData_MonsterNormalAttack | MainPackage_Skill_1057_MiHuo@1; MainPackage_Skill_1054_MoLiXiQu@3; MainPackage_Skill_1058_JingShenCuoLuan@1 |  | 10 |
| 永恒巨龟 | MainPackage_monster_6005_YongHengJuGui | 2 | 800 | 1000 | 2001=15; 2002=50 | 1 | SkillData_MonsterNormalAttack | MainPackage_Skill_1059_TuiBian@1; MainPackage_Skill_105_DunPaiMengJi@2 |  | 30 |
| 深渊智者 | MainPackage_monster_6006_ShenYuanZhiZhe | 2 | 700 | 1000 | 2001=33; 2002=10 | 1 | SkillData_MonsterNormalAttack | MainPackage_Skill_1063_ShenYuanZhouFu@1; MainPackage_Skill_1062_ChuWanJinWo@1; MainPackage_Skill_1014_RouRen@6 |  | 30 |
| 幽灵猫 | MainPackage_monster_8_YouLingMao | 0 | 50 | 50 | 2001=20; 2002=10 | 1 | SkillData_MonsterNormalAttack | MainPackage_Skill_1040_DuoChongZhuaJi@1; MainPackage_Skill_1005_PaoXiao@2 |  | 5 |
| 僵尸士兵 | MainPackage_monster_9_JiangShiShiBing | 0 | 80 | 20 | 2001=15; 2002=0 | 1 | SkillData_MonsterNormalAttack | MainPackage_Skill_1004_DuoCengHuJia@2; MainPackage_Skill_1038_LiangQiangChongZhuang@3 |  | 5 |
| 渴血兽 | MainPackage_monster_KeXueShou_7 | 1 | 80 | 35 | 2001=25; 2002=10 | 2 | SkillData_MonsterNormalAttack | MainPackage_Skill_1036_XiXue@2; SkillData_MonsterNormalDefense@6 |  | 10 |
| 幽灵 | MainPackage_monster_ghost_3 | 0 | 35 | 10 | 2001=10; 2002=5 | 1 | SkillData_MonsterNormalAttack | MainPackage_Skill_1037_CiErJianJiao@1; SkillData_MonsterNormalDefense@15 |  | 5 |
| 巫妖 | MainPackage_monster_lich_6 | 2 | 200 | 30 | 2001=40; 2002=5 | 1 | SkillData_MonsterNormalAttack |  |  | 25 |
| 骷髅 | MainPackage_monster_skeleton_1 | 0 | 15 | 5 | 2001=25; 2002=10 | 1 | SkillData_MonsterNormalAttack | SkillData_MonsterNormalDefense@5 |  | 5 |
| 僵尸 | MainPackage_monster_zombie_2 | 0 | 60 | 5 | 2001=12; 2002=0 | 1 | SkillData_MonsterNormalAttack |  |  | 5 |

## 技能结构（含武器、遗物等）：515 条

| 名称 | ID | 稀有度 | 类型 | 被动 | 等级上限 | 资源类型:消耗/行动/CD/使用限制 | 基础参数 | 每级增量 | 配置描述 | 事件函数 |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 哑铃 | MainPackage_Relic_1001_YaLing | 0 | 2 | 1 | 30 | 0:0/0/0/0 | P01=2 | P01=0×L | 力量 +[P01] | 1001: SetTargetAttributeValue; 1002: SetTargetAttributeValue |
| 灵巧手套 | MainPackage_Relic_1002_LingQiaoShouTao | 0 | 2 | 1 | 30 | 0:0/0/0/0 | P01=2 | P01=0×L | 敏捷 +[P01] | 1001: SetTargetAttributeValue; 1002: SetTargetAttributeValue |
| 冥想戒指 | MainPackage_Relic_1003_MingXiangJieZhi | 0 | 2 | 1 | 30 | 0:0/0/0/0 | P01=2 | P01=0×L | 智力 +[P01] | 1001: SetTargetAttributeValue; 1002: SetTargetAttributeValue |
| 生命刻印 | MainPackage_Relic_1004_ShengMingKeYing | 0 | 0 | 0 | 0 | 0:0/0/0/0 | P01=15 | P01=0×L | 最大生命+[P01] | 1001: SetTargetAttributeValue; 1002: SetTargetAttributeValue |
| 法力刻印 | MainPackage_Relic_1005_FaLiKeYing | 0 | 0 | 0 | 0 | 0:0/0/0/0 | P01=15 | P01=0×L | 最大法力+[P01] | 1001: SetTargetAttributeValue; 1002: SetTargetAttributeValue |
| 天使之泪 | MainPackage_Relic_1006_TianShiZhiLei | 0 | 0 | 0 | 0 | 0:0/0/0/0 | P01=4 | P01=0×L | 法力回复+[P01] | 1001: SetTargetAttributeValue; 1002: SetTargetAttributeValue |
| 琥珀护符 | MainPackage_Relic_1007_HuPoHuFu | 0 | 0 | 0 | 0 | 0:0/0/0/0 | P01=8 | P01=0×L | 战斗开始时：获得[P01]点护甲。 | 1008: RegistOrLogOffSkillTag; 1009: RegistOrLogOffSkillTag; 1010: Special_HuPoHuFu_203 |
| 饮血匕 | MainPackage_Relic_1008_YinXueBi | 0 | 2 | 1 | 10 | 0:0/0/0/0 | P01=2 | P01=0×L | 每当有一名敌人死亡：恢复[P01]点生命。 | 1003: RecoverConstValue |
| 吸能咒符 | MainPackage_Relic_1009_XiNengZhouFu | 0 | 2 | 1 | 10 | 0:0/0/0/0 | P01=2 | P01=0×L | 每当有一名敌人死亡：恢复[P01]点魔力。 | 1003: RecoverConstValue |
| 怯战者烙印 | MainPackage_Relic_1010_QieZhanZheLaoYin | 0 | 0 | 0 | 0 | 0:0/0/0/0 | P01=1 | P01=0×L | 战斗开始时：随机一名敌人[P01]层{Buff_MainPackage_XuRuo_12_Title} | 1008: AddBuffToTarget |
| 龙血徽记 | MainPackage_Relic_1011_LongXueHuiJi | 0 | 0 | 0 | 0 | 0:0/0/0/0 | P01=1 | P01=0×L | 受到伤害后：获得1点行动力。（每场战斗限一次） | 1008: RegistOrLogOffSkillTag; 1009: RegistOrLogOffSkillTag; 1013: Special_LongXueHuiJi_1010 |
| 帐篷 | MainPackage_Relic_1012_ZhangPeng | 0 | 0 | 0 | 0 | 0:0/0/0/0 | P01=1 | P01=0×L | 营地回复量增加[P01]% | 1001: AddSpecialTag; 1002: RemoveSpecialTag |
| 净瓶 | MainPackage_Relic_1013_JingPing | 0 | 0 | 0 | 0 | 0:0/0/0/0 | P01=1 | P01=0×L | 泉水回复量增加[P01]% | 1001: AddSpecialTag; 1002: RemoveSpecialTag |
| 蓝色宝石 | MainPackage_Relic_1014_LanSeBaoShi | 0 | 0 | 0 | 0 | 0:0/0/0/0 | P01=4 | P01=0×L | 战斗开始时：恢复[P01]点魔力 | 1008: RecoverConstValue |
| 快剑剑鞘 | MainPackage_Relic_1015_KuaiJianJianQiao | 1 | 0 | 0 | 0 | 0:0/0/0/0 | P01=0 | P01=0×L | 使用技能后：如果该技能是武技，则获得该技能的行动力消耗（每场战斗限一次） | 1014: Special_KuaiJianJianQiao_1015; 1008: RegistOrLogOffSkillTag; 1009: RegistOrLogOffSkillTag |
| 烧毁的斗篷 | MainPackage_Relic_1016_ShaoHuiDeDouPeng | 0 | 0 | 0 | 0 | 0:0/0/0/0 | P01=5 | P01=0×L | 回合开始时：对所有敌人造成[P01]点伤害 | 1010: DamageByAllEnemyConstValue |
| 金钟 | MainPackage_Relic_2001_JingZhong | 1 | 0 | 0 | 0 | 0:0/0/0/0 | P01=5 | P01=0×L | 如果受到的生命值伤害不大于[P01]点，则将该伤害降至1 | 1001: AddSpecialTag; 1002: RemoveSpecialTag |
| 防御印章 | MainPackage_Relic_2002_FangYuYinZhang | 1 | 0 | 0 | 0 | 0:0/0/0/0 | P01=3 | P01=0×L | 防御力+[P01] | 1001: SetTargetAttributeValue; 1002: SetTargetAttributeValue |
| 优惠券 | MainPackage_Relic_2003_YouHuiJuan | 1 | 0 | 0 | 0 | 0:0/0/0/0 | P01=0.1 | P01=0×L | 商店的商品价格-[P01]% | 1001: Special_YouHuiJuan_2003; 1002: Special_YouHuiJuan_2003 |
| 巨人护肩 | MainPackage_Relic_2004_JuRenHuJian | 1 | 0 | 0 | 0 | 0:0/0/0/0 | P01=4 | P01=0×L | 力量+[P01] | 1001: SetTargetAttributeValue; 1002: SetTargetAttributeValue |
| 迅捷之靴 | MainPackage_Relic_2005_XunJieZhiXue | 1 | 0 | 0 | 0 | 0:0/0/0/0 | P01=4 | P01=0×L | 敏捷+[P01] | 1001: SetTargetAttributeValue; 1002: SetTargetAttributeValue |
| 智力斗篷 | MainPackage_Relic_2006_ZhiLiDouPeng | 1 | 0 | 0 | 0 | 0:0/0/0/0 | P01=4 | P01=0×L | 智力+[P01] | 1001: SetTargetAttributeValue; 1002: SetTargetAttributeValue |
| 巨力腰带 | MainPackage_Relic_2007_JuLiYaoDai | 1 | 0 | 0 | 0 | 0:0/0/0/0 | P01=30 | P01=0×L | 最大生命+[P01] | 1001: SetTargetAttributeValue; 1002: SetTargetAttributeValue |
| 法力之核 | MainPackage_Relic_2008_FaLiZhiHe | 1 | 0 | 0 | 0 | 0:0/0/0/0 | P01=30 | P01=0×L | 最大法力+[P01] | 1001: SetTargetAttributeValue; 1002: SetTargetAttributeValue |
| 贪婪钱袋 | MainPackage_Relic_2009_TanLanQianDai | 1 | 2 | 1 | 20 | 0:0/0/0/0 | P01=2 | P01=0×L | 击杀敌人时：获得的金币+[P01] | 1001: SetTargetAttributeValue; 1002: SetTargetAttributeValue |
| 能量手镯 | MainPackage_Relic_2010_NengLiangShouZhuo | 1 | 0 | 0 | 0 | 0:0/0/0/0 | P01=1 | P01=0×L | 战斗开始时：获得[P01]点行动力。 | 1008: ResetTagValue; 1010: Special_NengLiangShouZhuo_2010 |
| 活力护符 | MainPackage_Relic_2011_HuoLiHuFu | 1 | 0 | 0 | 0 | 0:0/0/0/0 | P01=2 | P01=0×L | 战斗开始时：获得[P01]层{Buff_MainPackage_Vitality_4_Title}。 | 1008: SelfAddBuff |
| 坚守印记 | MainPackage_Relic_2012_JianShouYinJi | 1 | 0 | 0 | 0 | 0:0/0/0/0 | P01=12 | P01=0×L | 战斗开始时：获得[P01]点护甲。 | 1008: RegistOrLogOffSkillTag; 1009: RegistOrLogOffSkillTag; 1010: Special_HuPoHuFu_203 |
| 蓄势腕甲 | MainPackage_Relic_2013_XuShiHuWan | 1 | 0 | 0 | 0 | 0:0/0/0/0 | P01=1 | P01=0×L | 回合开始时：如果上回合未攻击，获得1点行动力。 | 1010: Special_XuShiHuWan_2013 |
| 魂能汲取器 | MainPackage_Relic_2014_HunNengJiQuQi | 1 | 0 | 0 | 0 | 0:0/0/0/0 | P01=1 | P01=0×L | 击杀敌人后：获得1点行动力。 | 1003: GainActionPoint |
| 蓄力印章 | MainPackage_Relic_2015_XuLiYinZhang | 1 | 2 | 1 | 12 | 0:0/0/0/1 | P01=2 | P01=0×L | 回合开始时：如果上回合未攻击，则获得[P01]层{Buff_MainPackage_Vitality_4_Title} | 1010: RemoveSelfTargetAllBuff |
| 斗士腰带 | MainPackage_Relic_2016_DouShiBiZhang | 1 | 0 | 0 | 0 | 0:0/0/0/0 | P01=1 | P01=0×L | 每次武器攻击后，获得[P01]层临时{Buff_MainPackage_Vitality_4_Title}。 | 1014: Special_Relic_DouShiBiZhang_2016 |
| 循环挂坠 | MainPackage_Relic_3001_XunHuanDiaoZhui | 2 | 0 | 0 | 0 | 0:0/0/0/0 | P01=4 | P01=0×L | 每使用[P01]次技能，获得1点行动力。(已使用次数:<UseCount>) | 1001: RegistOrLogOffSkillTag; 1014: Special_XunHuanGuaShi_3001 |
| 连击指虎 | MainPackage_Relic_3002_LianJiZhiHu | 2 | 0 | 0 | 0 | 0:0/0/0/0 | P01=4 | P01=0×L | 每普通攻击[P01]次，获得1点行动力。(已使用次数:<UseCount>) | 1001: RegistOrLogOffSkillTag; 1014: Special_LianJiZhiHu_3002 |
| 战神铭文 | MainPackage_Relic_3003_ZhanShenMingWen | 2 | 0 | 0 | 0 | 0:0/0/0/0 | P01=10 | P01=0×L | 每击杀[P01]个敌人增加1点力量(已击杀数量:<KillCount>) | 1001: RegistOrLogOffSkillTag,RegistOrLogOffSkillTag; 1003: RegistOrLogOffListenTag,RegistOrLogOffListenTag,IncreaseAttributeOnTargetTagAndReset |
| 疾风铭文 | MainPackage_Relic_3004_JiFengMingWen | 2 | 0 | 0 | 0 | 0:0/0/0/0 | P01=10 | P01=0×L | 每击杀[P01]个敌人增加1点敏捷(已击杀数量:<KillCount>) | 1001: RegistOrLogOffSkillTag,RegistOrLogOffSkillTag; 1003: RegistOrLogOffListenTag,RegistOrLogOffListenTag,IncreaseAttributeOnTargetTagAndReset |
| 奥术铭文 | MainPackage_Relic_3005_AoShuMingWen | 2 | 0 | 0 | 0 | 0:0/0/0/0 | P01=10 | P01=0×L | 每击杀[P01]个敌人增加1点智力(已击杀数量:<KillCount>) | 1001: RegistOrLogOffSkillTag,RegistOrLogOffSkillTag; 1003: RegistOrLogOffListenTag,RegistOrLogOffListenTag,IncreaseAttributeOnTargetTagAndReset |
| 蓄能晶核 | MainPackage_Relic_3006_XuNengJingHe | 2 | 0 | 0 | 0 | 0:0/0/0/0 | P01=1 | P01=0×L | 未使用的行动力可以保留到下一回合。 | 1011: Special_XuNengJingHe_3006 |
| 进击号角 | MainPackage_Relic_3007_JinJiHaoJiao | 2 | 0 | 0 | 0 | 0:0/0/0/0 | P01=3 | P01=0×L | 每过[P01]个回合，获得1点行动力 | 1001: RegistOrLogOffSkillTag; 1010: Special_JinJiHaoJiao_3007 |
| 露娅的邪口 | MainPackage_Relic_3008_LuYaDeXieKou | 2 | 0 | 0 | 0 | 0:0/0/0/0 | P01=1 | P01=0×L | 回合开始时：随机释放一个就绪的主动技能 |  |
| 剑术大师的手套 | MainPackage_Relic_3009_JianShuDaShiDeShouTao | 2 | 0 | 0 | 0 | 0:0/0/0/0 | P01=1 | P01=0×L | 普通攻击后：使一个冷却中的技能冷却-[P01] | 1014: Special_Relic_JianShuDaShiDeShouTao_30 |
| 武器包 | MainPackage_Relic_3010_WuQiBao | 2 | 0 | 0 | 0 | 0:0/0/0/0 | P01=3; P02=15 | P01=0×L; P02=0×L | 在一个回合内使用了[P01]个不同的技能后，对所有敌人造成[P02]点伤害(已使用数量:<UseCount>) |  |
| 淬毒药袋 | MainPackage_Relic_3011_CuiDuYaoDai | 2 | 2 | 1 | 10 | 0:0/0/0/1 | BuffCount=2 | BuffCount=0×L | 攻击造成伤害后：如果该次攻击对生命值造成了伤害，对目标附加[BuffCount]层{Buff_MainPackage_ZhongDu_3_Title} | 1005: Special_DuZhiRen |
| 恶魔之角 | MainPackage_Relic_3012_EMoZhiJiao | 2 | 0 | 0 | 0 | 0:0/0/0/0 | P01=3; P02=1 | P01=0×L; P02=0×L | 最大行动力+1。<br>战斗开始时：随机[P01]个主动技能的冷却+[P02] | 1001: MaxActionChange; 1002: MaxActionChange; 1008: TargetActiveSkillGetCooldown,TargetActiveSkillGetCooldown,TargetActiveSkillGetCooldown |
| 汲能吊坠 | MainPackage_Relic_3013_JiNengDiaoZhui | 2 | 0 | 0 | 0 | 0:0/0/0/0 | P01=2 | P01=0×L | 最大行动力+1。<br>每场战斗的第一个回合行动力-[P01] | 1001: MaxActionChange; 1002: MaxActionChange; 1008: GainActionNextTurn |
| 极寒 | MainPackage_SceneEffect_1_JiHan | 0 | 0 | 1 | 20 | 0:0/0/0/0 | P01=0; P02=2 | P01=2×L; P02=0×L | 回合开始时：使所有单位获得{Buff_MainPackage_HanShuang_8_Title}直至[P01]层，同时每有1层{Buff_MainPackage_HanShuang_8_Title}，受到[P02]点伤害 |  |
| 很多石堆 | MainPackage_SceneEffect_2_HenDuoShiDui | 0 | 0 | 1 | 1 | 0:0/0/0/0 | P01=1 | P01=0×L | 战斗开始时：如果条件允许，为敌方生成一个带嘲讽的石堆 | 1008: SceneEffect_AddUnitToAllTeam; 1009: SceneEffect_DestoryAllTeamTargetUnit |
| 灼热 | MainPackage_SceneEffect_3_JiRe | 0 | 0 | 1 | 20 | 0:0/0/0/0 | P01=0 | P01=2×L | 回合开始时：所有单位获得[P01]层{Buff_MainPackage_ShaoShang_7_Title} |  |
| 亢奋 | MainPackage_SceneEffect_4_KangFen | 0 | 0 | 1 | 999999 | 0:0/0/0/0 | P01=0 | P01=0×L | 战斗开始时：所有敌方单位根据深度获得{Buff_MainPackage_KangFen_20_Title}和{Buff_MainPackage_Vitality_4_Title} | 1008: SceneEffect_AllEnemyUnitGetBuff,SceneEffect_AllEnemyUnitGetBuff |
| 痛击 | MainPackage_Skill_1001_TongJi | 1 | 3 | 0 | 20 | 0:5/1/6/1 | ConstValue=5; P02=2 | ConstValue=3×L; P02=1×L | 造成[ConstValue]点伤害并附加[P02]层{Buff_MainPackage_QingShi_11_Title} | 0: DamageTargetConst,AddBuffToTarget; -1001: SetExpectionDamage,SetBuffSettingSkillExpectation |
| 撕咬 | MainPackage_Skill_1002_SiYao | 2 | 3 | 0 | 5 | 0:10/2/10/1 | Precent=0.1 | Precent=0.05×L | 造成目标当前生命值[DamagePrecent]%的伤害 | 0: DamageByTargetCurrentHealth; -1001: DamageByTargetCurrentHealth |
| 多重打击 | MainPackage_Skill_1003_DuoChongDaJi | 2 | 4 | 0 | 4 | 0:0/2/6/1 | ConstValue=3; DamageCount=3 | ConstValue=2×L; DamageCount=0×L | 造成[ConstValue]伤害[DamageCount]次 | 0: DamageTargetConst; -1001: SetExpectionDamage,SetDamageCountExpectation |
| 缠绕 | MainPackage_Skill_1004_ChanRao | 0 | 4 | 0 | 3 | 0:0/2/10/1 | P02=2; COOLDOWN=0 | P02=0×L; COOLDOWN=-2×L | 使目标获得[P02]层{Buff_MainPackage_1005_ChanRao_Title} | 0: AddBuffToTarget; -1001: SetBuffSettingSkillExpectation |
| 多层护甲 | MainPackage_Skill_1004_DuoCengHuJia | 1 | 2 | 1 | 12 | 0:0/0/0/0 | P01=15; P02=3 | P01=5×L; P02=0×L | 防御力+[P01]<br>受到攻击后:获得[P02]层{Buff_MainPackage_QingShi_11_Title} | 1001: SetTargetAttributeValue_AdditionValue; 1002: RemoveTargetAttributeValue; 1012: SelfAddBuff |
| 咆哮 | MainPackage_Skill_1005_PaoXiao | 0 | 4 | 0 | 10 | 0:0/2/12/1 | P01=1 | P01=1×L | 对所有敌人附加[P01]层{Buff_MainPackage_CuiRuo_1_Title} | 0: AddBuffToTarget; -1001: SetBuffSettingSkillExpectation |
| 强力再生 | MainPackage_Skill_1005_QiangLiZaiSheng | 2 | 2 | 1 | 12 | 0:0/0/0/0 | P01=5 | P01=5×L | 回合开始时:恢复[P01]点生命值 |  |
| 扰乱冲击 | MainPackage_Skill_1006_RaoLuanChongJi | 1 | 1 | 0 | 12 | 0:5/0/4/1 | P01=1 | P01=1×L | 使敌人随机三个主动技能的冷却+[P01] | 0: AddTargetRandomSkillCoolDown; -1001: SetBuffSettingSkillExpectation |
| 咒缚 | MainPackage_Skill_1007_ZhouFu | 1 | 1 | 0 | 6 | 0:10/2/7/1 | P01=3; P02=3; COOLDOWN=0 | P01=0×L; P02=0×L; COOLDOWN=-1×L | 对所有敌人附加[P01]层{Buff_MainPackage_CuiRuo_1_Title}和[P02]层{Buff_MainPackage_XuRuo_12_Title} | 0: AddBuffToAllEnemy,AddBuffToPlayer; -1001: SetBuffSettingSkillExpectation |
| 施法干扰 | MainPackage_Skill_1008_ShiFaGanRao | 2 | 2 | 1 | 6 | 0:0/0/0/1 | P01=2 | P01=0×L | 每回合限一次，玩家使用技能后，玩家随机一个主动技能+[P01]冷却 | 1008: AddBuffToPlayer; 1010: AddBuffToPlayer |
| 影甲 | MainPackage_Skill_1009_YingJia | 2 | 1 | 0 | 6 | 0:15/0/4/1 | P01=12; P02=3; P03=0 | P01=3×L; P02=2×L; P03=1×L | 获得[P01]护甲，且直到下次回合结束时，如果受到伤害且护甲存在，获得[P02]的护甲 | 0: GainArmor,SelfAddBuff; -1001: ArmorSkillExpectation,SetBuffSettingSkillExpectation |
| 转守为攻 | MainPackage_Skill_100_ZhuanShouWeiGong | 2 | 4 | 0 | 4 | 0:5/1/3/1 | P01=0.3 | P01=0.2×L | 移除自身所有护甲，每移除1点护甲，获得[P01]层{Buff_MainPackage_Vitality_4_Title} | 0: Special_ZhuanShouWeiGong; -1001: Special_ZhuanShouWeiGong |
| 血盆大口 | MainPackage_Skill_1010_XuePenDaKou | 2 | 4 | 0 | 6 | 0:0/0/0/1 | P01=35 | P01=15×L | 造成[P01]点伤害，自身存在{Buff_MainPackage_1005_ChanRao_Title}时无法使用 | 0: DamageTargetConst; -1001: SetExpectionDamage |
| 潜伏 | MainPackage_Skill_1011_QianFu | 0 | 2 | 1 | 1 | 0:0/0/0/1 | P01=4 | P01=0×L | 战斗开始时:获得[P01]层{Buff_MainPackage_1005_ChanRao_Title}，受到攻击时有可能变化行动模式 | 1008: SelfAddBuff |
| 畏缩 | MainPackage_Skill_1012_WeiSuo | 1 | 2 | 1 | 6 | 0:0/0/0/1 | P01=100; P02=25 | P01=0×L; P02=5×L | 每受到[P01]点生命值伤害，获得[P02]点护甲，同时自身获得2层{Buff_MainPackage_1005_ChanRao_Title} | 1013: Special_WeiSuo_1014 |
| 试探 | MainPackage_Skill_1013_ShiTan | 1 | 4 | 0 | 6 | 0:0/0/0/1 | P01=7; P02=2 | P01=4×L; P02=0×L | 造成[P01]点伤害，并附加[P02]层{Buff_MainPackage_XuRuo_12_Title}，如果该次攻击造成了生命值伤害，移除自身1层{Buff_MainPackage_1005_ChanRao_Title} | 0: DamageTargetConst,AddBuffToTarget; -1001: SetExpectionDamage,SetBuffSettingSkillExpectation; 1005: Special_ShiTanGongJi_1015 |
| 柔韧 | MainPackage_Skill_1014_RouRen | 1 | 2 | 1 | 12 | 0:0/0/0/1 | P01=0.2 | P01=0.1×L | 受到伤害后:每受到1点生命值伤害，获得[P01]点护甲 | 1013: Special_RouRen_1014 |
| 传播疾病 | MainPackage_Skill_1015_ChuanBoJiBing | 0 | 1 | 0 | 12 | 0:15/0/20/1 | P01=1; P02=2 | P01=2×L; P02=0×L | 为目标附加[P01]层{Buff_MainPackage_ZhongDu_3_Title}和[P02]层{Buff_MainPackage_XuRuo_12_Title} | 0: AddBuffToTarget,AddBuffToTarget; -1001: SetBuffSettingSkillExpectation |
| 毒血 | MainPackage_Skill_1016_DuZhiXue | 1 | 2 | 1 | 3 | 0:0/0/0/1 | P01=1 | P01=1×L | 受到伤害后：为伤害来源附加[P01]层{Buff_MainPackage_ZhongDu_3_Title}(每回合限一次) | 1013: Special_DuXue_1016; 1010: ResetTagValue; 1008: RegistOrLogOffSkillTag; 1009: RegistOrLogOffSkillTag |
| 多头生物 | MainPackage_Skill_1017_DuoTouShengWu | 2 | 4 | 0 | 1 | 0:0/3/0/1 | P01=75 | P01=0×L | 召唤蛇头直到上限<br>蛇头死亡时，会对自身造成[P01]点真实伤害<br>自身死亡时，消灭所有蛇头 | 0: CacheTargetBuffData,SummonUnit; -1001: SetUnknowSkillExp; 1017: Special_DuoTouShengWu_Passitve_1017; 1018: Special_DuoTouShengWu_Passitve_1017 |
| 沉重打击 | MainPackage_Skill_1018_ChenZhongDaJi | 1 | 4 | 0 | 12 | 0:0/2/4/1 | P01=10; P02=1 | P01=5×L; P02=0×L | 造成[P01]点伤害，并附加[P02]层{Buff_MainPackage_1015_ChenZhong_Title} | 0: DamageTargetConst,AddBuffToTarget; -1001: SetExpectionDamage,SetBuffSettingSkillExpectation |
| 沉睡 | MainPackage_Skill_1019_ChenShui | 1 | 2 | 1 | 2 | 0:0/0/0/1 | P01=2; P02=10 | P01=0×L; P02=10×L | 战斗开始时:获得[P01]层{Buff_MainPackage_ShuiMian_15_Title}<br>回合结束时：如果拥有{Buff_MainPackage_ShuiMian_15_Title}，获得[P02]点护甲 | 1008: SelfAddBuff; 1010: Special_ChenShui_1019_01 |
| 破绽战吼 | MainPackage_Skill_101_PoZhanZhanHou | 1 | 4 | 0 | 6 | 0:1/1/3/1 | P01=0 | P01=1×L | 对所有敌人附加[P01]层{Buff_MainPackage_CuiRuo_1_Title} | 0: AddBuffToAllEnemy; -1001: SetBuffSettingSkillExpectation |
| 巨石拳 | MainPackage_Skill_1020_JuShiQuan | 0 | 4 | 0 | 12 | 0:5/0/4/1 | P01=4; P02=2 | P01=3×L; P02=0×L | 造成[P01]点伤害，如果该次攻击造成了生命值伤害，为目标附加[P02]层{Buff_MainPackage_CuiRuo_1_Title}或[P02]层{Buff_MainPackage_XuRuo_12_Title} | 0: DamageTargetConst; 1005: Special_ChenShui_1020; -1001: SetExpectionDamage,SetBuffSettingSkillExpectation |
| 连环突袭 | MainPackage_Skill_1021_LianHuanTuXi | 0 | 4 | 0 | 20 | 0:0/1/2/1 | P01=10 | P01=3×L | 造成[P01]点伤害2次 | 0: DamageTargetConst; -1001: SetExpectionDamage,SetDamageCount |
| 巨角撞击 | MainPackage_Skill_1022_JuJiaoZhuangJi | 2 | 4 | 0 | 12 | 0:5/1/2/1 | P01=10 | P01=5×L | 造成[P01]点伤害，如果目标护甲小于[P01]，则变为真实伤害 | 0: Special_JuJiaoZhuangJi_1022; -1001: Special_JuJiaoZhuangJi_1022 |
| 践踏 | MainPackage_Skill_1023_JianTa | 1 | 4 | 0 | 12 | 0:0/0/4/1 | P01=15; P02=1 | P01=5×L; P02=1×L | 对所有敌人造成[P01]点伤害，并附加[P02]层{Buff_MainPackage_QingShi_11_Title} | 0: DamageByAllEnemyConstValue,AddBuffToAllEnemy; -1001: SetExpectionDamage,SetBuffSettingSkillExpectation |
| 利爪切割 | MainPackage_Skill_1024_LiZhuaQieGe | 2 | 4 | 0 | 6 | 0:0/2/3/1 | P01=6 | P01=3×L | 造成[P01]点伤害3次，如果自身当前生命值小于最大生命值的50%，伤害次数+1   | 0: Special_LiZhuaQieGe_1024; -1001: Special_LiZhuaQieGe_1024 |
| 超强再生 | MainPackage_Skill_1025_ChaoQiangZaiSheng | 2 | 4 | 0 | 3 | 0:0/2/4/1 | P01=12; P02=5; P03=0.5 | P01=3×L; P02=5×L; P03=0.5×L | 恢复[P01]点生命值和[P02]点护甲，同时自身每有1层{Buff_MainPackage_Vitality_4_Title}，恢复[P03]点生命值 | 0: Special_ChaoQiangZaiSheng_1025; -1001: Special_ChaoQiangZaiSheng_1025 |
| 巨石暴君 | MainPackage_Skill_1026_JuShiBaoJun | 2 | 2 | 1 | 4 | 0:0/0/0/1 | P01=8 | P01=2×L | 回合开始时：如果当前生命值小于最大生命值的50%，获得{Buff_MainPackage_Vitality_4_Title}直到[P01]层  | 1011: Special_JuShiBaoJun_1026 |
| 碍事 | MainPackage_Skill_1027_AiShi | 0 | 2 | 1 | 1 | 0:0/0/0/1 | P01=0 | P01=1×L | 战斗开始时：获得[P01]层{Buff_MainPackage_ChaoFeng_16_Title} | 1008: SelfAddBuff; 1010: SelfAddBuff |
| 野蛮之血 | MainPackage_Skill_1027_YeManZhiXue | 2 | 4 | 1 | 4 | 0:0/0/0/1 | P01=1 | P01=1×L | 攻击后：如果造成了生命值伤害，获得[P01]层活力 | 1005: Special_YeManZhiXue_1027 |
| 冰冷聚集 | MainPackage_Skill_1028_BingLengJuJi | 0 | 2 | 1 | 1 | 0:0/0/0/1 | P01=1 | P01=0×L | 其他友方角色通过主动技能获得{Buff_MainPackage_HanShuang_8_Title}时，自身的{Buff_MainPackage_HanShuang_8_Title}层数+[P01] |  |
| 淬毒短刃 | MainPackage_Skill_1029_CuiDuDuanRen | 1 | 4 | 0 | 6 | 0:5/0/4/1 | P01=25; P02=1; P03=4 | P01=5×L; P02=0×L; P03=2×L | 造成[P01]伤害，附加附加[P02]层{Buff_MainPackage_CuiRuo_1_Title}、[P03]层{Buff_MainPackage_ZhongDu_3_Title} | 0: DamageTargetConst,AddBuffToTarget,AddBuffToTarget; -1001: SetExpectionDamage,SetBuffSettingSkillExpectation |
| 快速反应 | MainPackage_Skill_102_KuaiSuFanYing | 0 | 2 | 1 | 15 | 0:0/0/0/0 | P01=0 | P01=1×L | 受到伤害后：获得[P01]层护甲 | 1013: GainArmor |
| 胆小 | MainPackage_Skill_1030_DanXiao | 1 | 2 | 1 | 1 | 0:0/0/0/1 | P01=4 | P01=0×L | 回合开始时：在[P01]个回合后，如果自身为队伍中的唯一单位，则逃跑 | 1010: Special_DanXiao_1030; 1011: RegistOrLogOffListenTag |
| 不屈 | MainPackage_Skill_1031_BuQu | 2 | 2 | 1 | 2 | 0:0/0/0/1 | P01=0.3; P02=0.15; P03=3 | P01=0×L; P02=0.05×L; P03=2×L | 回合结束时：如果生命值小于最大生命值的[P01]%，则恢复最大生命值[P02]%的生命值，同时获得{Buff_MainPackage_Vitality_4_Title}（剩余触发次数:<LeftCount>） | 1008: ResetTagValue; 1011: Special_BuQu_1031; 1009: ResetTagValue; 1001: ResetTagValue |
| 套网 | MainPackage_Skill_1032_TaoWang | 1 | 4 | 0 | 2 | 0:5/1/6/1 | P01=2; P02=2; COOLDOWN=0 | P01=0×L; P02=0×L; COOLDOWN=-1×L | 为目标附加[P01]层{Buff_MainPackage_1015_ChenZhong_Title}和[P02]层{Buff_MainPackage_XuRuo_12_Title} | 0: AddBuffToTarget,AddBuffToTarget; -1001: SetBuffSettingSkillExpectation |
| 快来护驾 | MainPackage_Skill_1033_KuaiLaiHuJia | 2 | 4 | 0 | 3 | 0:5/1/6/1 | P01=3; COOLDOWN=0 | P01=0×L; COOLDOWN=-1×L | 召唤1个随机哥布林，并使其获得[P01]层{Buff_MainPackage_ChaoFeng_16_Title} | 0: Special_KuaiLaiHuJia_1033; -1001: SetUnknowSkillExp |
| 统御号令 | MainPackage_Skill_1034_TongYuHaoLing | 2 | 4 | 0 | 6 | 0:5/0/4/1 | P01=20; P02=0 | P01=10×L; P02=2×L | 使所有友方单位恢复[P01]点生命值和获得[P02]层{Buff_MainPackage_Vitality_4_Title} | 0: RecoverConstValueAllTeam,AddBuffToAllTeam; -1001: SetHealthRecover,SetBuffSettingSkillExpectation |
| 暗黑之矛 | MainPackage_Skill_1035_AnHeiZhiMao | 2 | 4 | 0 | 6 | 0:5/0/4/1 | P01=20; P02=6; P03=3 | P01=10×L; P02=0×L; P03=0×L | 造成[P01]点伤害并附加[P02]层{Buff_MainPackage_QingShi_11_Title}，如果造成了生命值伤害，为目标附加[P03]层{Buff_MainPackage_CuiRuo_1_Title} | 0: DamageTargetConst,AddBuffToTarget; 1005: Special_KuaiLaiHuJia_1035; -1001: SetExpectionDamage,SetBuffSettingSkillExpectation |
| 吸血 | MainPackage_Skill_1036_XiXue | 1 | 2 | 1 | 12 | 0:0/0/0/1 | P01=3 | P01=3×L | 攻击造成伤害后：恢复[P01]点生命值 | 1007: Special_KuaiLaiHuJia_1036 |
| 刺耳尖叫 | MainPackage_Skill_1037_CiErJianJiao | 0 | 1 | 0 | 20 | 0:5/0/3/1 | P01=5; P02=2 | P01=3×L; P02=0×L | 扣除目标[P01]点魔力值并附加[P02]层{Buff_MainPackage_XuRuo_12_Title} | 0: ReduceTargeCurMagic,AddBuffToTarget; -1001: SetBuffSettingSkillExpectation |
| 踉跄冲撞 | MainPackage_Skill_1038_LiangQiangChongZhuang | 0 | 4 | 0 | 12 | 0:0/0/3/1 | P01=10; P02=15 | P01=5×L; P02=0×L | 造成[P01]点伤害，如果该伤害没有造成生命值伤害，自身获得[P02]层{Buff_MainPackage_QingShi_11_Title} | 0: DamageTargetConst; 1007: Special_LiangQiangChongZhuang_1038; -1001: SetExpectionDamage,SetBuffSettingSkillExpectation |
| 次等生命偷取 | MainPackage_Skill_1039_CiDengShengMingTouQu | 0 | 1 | 0 | 20 | 0:5/0/5/1 | P01=7; P02=3 | P01=3×L; P02=1×L | 造成[P01]点{Buff_MainPackage_1006_ZhenShiShangHai_Title}，并恢复[P02]点生命值 | 0: DamageTargetConstOnRealDamage,RecoverConstValue; -1001: SetToRealDamage,SetHealthRecover |
| 冷静 | MainPackage_Skill_103_LengJing | 2 | 4 | 0 | 4 | 0:10/1/15/1 | RecoverCount=0.5; P01=7; P02=1 | RecoverCount=0.5×L; P01=0×L; P02=0×L | 移除自身所有{Buff_MainPackage_Vitality_4_Title}，每移除1层，恢复[RecoverCount]点生命值，如果移除了至少[P01]层，则永久提升[P02]点力量 | 0: Special_LengJing; -1001: Special_LengJing |
| 多重爪击 | MainPackage_Skill_1040_DuoChongZhuaJi | 2 | 4 | 0 | 4 | 0:0/2/10/1 | ConstValue=2; DamageCount=2 | ConstValue=3×L; DamageCount=1×L | 造成[ConstValue]伤害[DamageCount]次 | 0: DamageTargetConst; -1001: SetExpectionDamage,SetDamageCount |
| 伤口舔血 | MainPackage_Skill_1041_ShangKouTianXue | 2 | 2 | 1 | 6 | 0:0/0/0/1 | P01=0.4 | P01=0.2×L | 造成伤害后：如果造成了生命值伤害，恢复该伤害量[P01]%的生命值 | 1005: Special_ShangKouTianXue_1041 |
| 侵蚀孢子 | MainPackage_Skill_1042_QinShiBaoZi | 1 | 2 | 0 | 6 | 0:0/1/4/1 | P01=2; P02=1 | P01=0×L; P02=1×L | 对所有敌人附加[P01]层{Buff_MainPackage_QingShi_11_Title}，自身获得[P02]层{Buff_MainPackage_Vitality_4_Title} | 0: AddBuffToTarget,SelfAddBuff; -1001: SetBuffSettingSkillExpectation,SetBuffSettingSkillExpectation |
| 虚弱花粉 | MainPackage_Skill_1043_XuRuoHuaFen | 0 | 2 | 0 | 1 | 0:0/0/3/1 | P01=2 | P01=0×L | 使所有敌人获得[P01]层{Buff_MainPackage_XuRuo_12_Title} | 0: AddBuffToTarget; -1001: SetBuffSettingSkillExpectation |
| 无谋冲锋 | MainPackage_Skill_1044_WuMouChongFeng | 0 | 4 | 0 | 20 | 0:0/0/2/1 | ConstValue=7; P02=2 | ConstValue=7×L; P02=0×L | 造成[ConstValue]点伤害并给自身附加[P02]层{Buff_MainPackage_CuiRuo_1_Title} | 0: DamageTargetConst,SelfAddBuff; -1001: SetExpectionDamage,SetBuffSettingSkillExpectation |
| 森林之怒 | MainPackage_Skill_1045_ShenLinZhiNu | 2 | 1 | 1 | 6 | 0:0/0/0/1 | P01=2 | P01=1×L | 每有一个友方单位死亡，获得[P01]层{Buff_MainPackage_Vitality_4_Title} | 1017: Special_SenLinZhiNu_GainBuff_1045 |
| 森林之友 | MainPackage_Skill_1046_SenLinZhiYou | 2 | 1 | 0 | 6 | 0:0/3/3/1 | P01=2; P02=5; P03=3; P04=50 | P01=0×L; P02=2×L; P03=0×L; P04=0×L | 召唤[P01]个花妖，并获得[P02]层{Buff_MainPackage_JingJi_2_Title}<br>花妖死亡时：移除自身[P03]层{Buff_MainPackage_JingJi_2_Title}<br>回合开始时：如果有其他友方单位，获得[P04]点护甲 | 0: SummerTargetUnit,SelfAddBuff; -1001: SetUnknowSkillExp; 1008: SelfAddBuff; 1017: Special_SenLinZhiYou_1046; 1011: Special_SenLinZhiYou_Armor_1046 |
| 不稳定聚合物 | MainPackage_Skill_1047_BuWenDingJuHeWu | 2 | 2 | 1 | 4 | 0:0/0/0/1 | P01=0.75 | P01=0.25×L | 受到生命值伤害时：召唤一个[P01]%伤害生命值的淤泥分裂体，如果已经存在淤泥分裂体，改为为其提升[P01]%伤害的最大生命值 | 1013: Special_BuWenDingJuHeTi_1049 |
| 重新凝聚 | MainPackage_Skill_1048_ChongXinNingJu | 1 | 1 | 0 | 4 | 0:0/0/4/1 | P01=10; P02=5 | P01=5×L; P02=5×L | 获得[P01]点护甲，并恢复[P02]点生命值 | 0: GainArmor,RecoverConstValue; -1001: ArmorSkillExpectation,SetHealthRecover |
| 沸腾 | MainPackage_Skill_1049_FeiTeng | 2 | 2 | 1 | 4 | 0:0/0/0/1 | P01=1 | P01=1×L | 回合开始时：所有友方单位获得[P01]层{Buff_MainPackage_Vitality_4_Title} | 1010: AddBuffToAllTeam |
| 奋力一搏 | MainPackage_Skill_104_FenLiYiBo | 2 | 4 | 0 | 4 | 0:0/1/0/1 | P01=3 | P01=2×L | 移除自身所有{Buff_MainPackage_Vitality_4_Title}，造成等同于移除自身{Buff_MainPackage_Vitality_4_Title}层数[P01]%的伤害 | 0: Special_FenLiYiBo; -1001: SetDamageBySelfBuffLevel |
| 烈焰护身 | MainPackage_Skill_1050_LieYanHuShen | 1 | 2 | 1 | 6 | 0:0/0/0/1 | P01=0.5 | P01=0.25×L | 回合开始时：每有1层{Buff_MainPackage_ShaoShang_7_Title}，获得[P01]点护甲 | 1010: SelfAddBuffBySelfBuff |
| 自爆 | MainPackage_Skill_1051_ZiBao | 0 | 1 | 0 | 6 | 0:0/0/0/1 | P01=35; P02=9999 | P01=10×L; P02=0×L | 对所有敌人造成[P01]点伤害，同时对自身造成[P02]点{Buff_MainPackage_1006_ZhenShiShangHai_Title} | 0: DamageByAllEnemyConstValue,DamageSelfByReal; -1001: SetExpectionDamage,SetUnknowSkillExp |
| 魔力附着 | MainPackage_Skill_1052_MoLiFuZhuo | 1 | 2 | 1 | 4 | 0:0/0/0/1 | P01=1 | P01=1×L | 被攻击且受到生命值伤害时：伤害来源失去[P01]点魔力，该效果一回合一次 | 1013: Special_MoLiFuZhuo_Armor_1052; 1010: ResetTagValue |
| 卷尾 | MainPackage_Skill_1053_JuanWei | 1 | 1 | 0 | 4 | 0:5/2/5/1 | P01=10; P02=4 | P01=5×L; P02=1×L | 获得[P01]点护甲和[P02]层{Buff_MainPackage_JingJi_2_Title} | 0: GainArmor,SelfAddBuff; -1001: ArmorSkillExpectation,SetBuffSettingSkillExpectation |
| 魔力汲取 | MainPackage_Skill_1054_MoLiXiQu | 1 | 1 | 0 | 4 | 0:5/1/2/1 | P01=6 | P01=4×L | 减少目标[P01]点魔力值，如果目标魔力不足[P01]，则对目标造成[P01]点{Buff_MainPackage_1006_ZhenShiShangHai_Title} | 0: Special_MoLiJiQu_1054; -1001: Special_MoLiJiQu_1054 |
| 破碎夹击 | MainPackage_Skill_1055_PoSuiJiaJi | 1 | 4 | 0 | 2 | 0:0/0/2/1 | P01=3 | P01=1×L | 为目标附加[P01]层{Buff_MainPackage_CuiRuo_1_Title} | 0: AddBuffToTarget; -1001: SetBuffSettingSkillExpectation |
| 巨钳碾压 | MainPackage_Skill_1056_JuQianNianYa | 1 | 4 | 0 | 6 | 0:5/0/4/1 | P01=10; P02=1 | P01=5×L; P02=1×L | 造成[P01]点伤害并附加[P02]层{Buff_MainPackage_QingShi_11_Title} | 0: DamageTargetConst,AddBuffToTarget; -1001: SetExpectionDamage,SetBuffSettingSkillExpectation |
| 迷惑 | MainPackage_Skill_1057_MiHuo | 2 | 1 | 0 | 1 | 0:5/0/3/1 | P01=3; P02=3 | P01=0×L; P02=0×L | 为目标附加[P01]层{Buff_MainPackage_CuiRuo_1_Title}和[P01]层{Buff_MainPackage_XuRuo_12_Title} | 0: AddBuffToTarget,AddBuffToTarget; -1001: SetBuffSettingSkillExpectation,SetBuffSettingSkillExpectation |
| 精神错乱 | MainPackage_Skill_1058_JingShenCuoLuan | 2 | 1 | 0 | 1 | 0:5/0/4/1 | P01=4; P02=1 | P01=0×L; P02=1×L | 使目标最多[P01]个主动技能的冷却+[P02] | 0: Special_JingShenCuoLuan_1058; -1001: SetBuffSettingSkillExpectation |
| 蜕变 | MainPackage_Skill_1059_TuiBian | 2 | 2 | 1 | 1 | 0:0/0/0/1 | P01=2 | P01=0×L | 回合开始时：获得[P01]层{Buff_MainPackage_Vitality_4_Title}，同时获得[P01]层{Buff_MainPackage_QingShi_11_Title} | 1010: SelfAddBuff,SelfAddBuff |
| 盾牌猛击 | MainPackage_Skill_105_DunPaiMengJi | 0 | 2 | 0 | 15 | 0:0/1/2/1 | P01=0.8 | P01=0.2×L | 造成[P01]%护甲的伤害 | 0: DamageBySelfArmor; -1001: SetDamageBySelfArmor |
| 重压立场 | MainPackage_Skill_1060_ZhongYaLiChang | 2 | 2 | 1 | 1 | 0:0/0/0/1 | P01=1 | P01=0×L | 玩家每回合第一次使用技能后：玩家获得1层{Buff_MainPackage_1015_ChenZhong_Title} | 1010: AddBuffToPlayer; 1008: AddBuffToPlayer |
| 求知欲 | MainPackage_Skill_1061_QiuZhiYu | 2 | 2 | 1 | 1 | 0:0/0/0/1 | P01=3; P02=1 | P01=0×L; P02=0×L | 玩家每使用[P01]次技能后：获得[P02]层{Buff_MainPackage_Vitality_4_Title} |  |
| 触腕紧握 | MainPackage_Skill_1062_ChuWanJinWo | 2 | 4 | 0 | 1 | 0:15/0/4/1 | P01=2; P02=5; P03=1 | P01=0×L; P02=0×L; P03=0×L | 为目标附加[P01]层{Buff_MainPackage_1005_ChanRao_Title}，[P02]层{Buff_MainPackage_QingShi_11_Title},[P03]层{Buff_MainPackage_1015_ChenZhong_Title} | 0: AddBuffToTarget,AddBuffToTarget,AddBuffToTarget; -1001: SetBuffSettingSkillExpectation,SetBuffSettingSkillExpectation,SetBuffSettingSkillExpectation |
| 深渊咒缚 | MainPackage_Skill_1063_ShenYuanZhouFu | 2 | 2 | 1 | 1 | 0:0/0/0/1 | P01=3; P02=2 | P01=0×L; P02=0×L | 回合开始时：为所有敌方单位附加[P02]层{Buff_MainPackage_1025_ShenYuanZhouFu_Title} | 1008: AddBuffToAllEnemy; 1010: AddBuffToAllEnemy |
| 坍塌 | MainPackage_Skill_1065_TanTa | 2 | 1 | 0 | 2 | 0:15/0/3/1 | P01=10 | P01=10×L | 召唤石堆至上限，使你的石堆的最大生命值+[P01] | 0: CacheTargetAttributeData,SummonUnit; -1001: SetUnknowSkillExp |
| 岩石投掷 | MainPackage_Skill_1066_YanShiTouZhi | 2 | 4 | 0 | 2 | 0:15/0/2/1 | P01=10; P02=10 | P01=5×L; P02=0×L | 造成[P02]点伤害，同时消灭全部石堆，每消灭一个对目标额外造成[P01]点伤害 | 0: Special_YanShiTouZhi_1059; -1001: Special_YanShiTouZhi_1059 |
| 岩石灵体 | MainPackage_Skill_1067_YanShiLingTi | 2 | 2 | 1 | 2 | 0:0/0/0/1 | P01=80; P02=10 | P01=20×L; P02=0×L | 获得[P01]点防御力，在一个石堆移除后，自身获得[P02]层{Buff_MainPackage_QingShi_11_Title} |  |
| 脱离 | MainPackage_Skill_1068_TuoLi | 1 | 1 | 1 | 1 | 0:0/0/0/1 | P01=50; P02=8 | P01=0×L; P02=2×L | 每次受到[P01]点生命值伤害后，使全部的石堆的生命值+[P02]，同时召唤一个石堆 |  |
| 积蓄 | MainPackage_Skill_1069_JiXu | 1 | 2 | 1 | 5 | 0:0/0/0/1 | P01=4; P02=10 | P01=0×L; P02=5×L | 战斗开始的第[P01]个回合：获得[P02]层{Buff_MainPackage_Vitality_4_Title} | 1008: ResetTagValue; 1011: RegistOrLogOffListenTag,Special_YanShiTouZhi_1059 |
| 蓄力 | MainPackage_Skill_106_XuLi | 1 | 2 | 1 | 6 | 0:0/0/0/1 | P01=1 | P01=1×L | 回合开始时：如果上回合未攻击，则获得[P01]层{Buff_MainPackage_Vitality_4_Title} |  |
| 魅惑之吻 | MainPackage_Skill_1070_MeiHuoZhiWen | 2 | 1 | 0 | 4 | 0:0/1/2/1 | P01=2 | P01=2×L | 使所有其他友方单位获得[P01]层{Buff_MainPackage_Vitality_4_Title} | 0: Special_YanShiTouZhi_1070; -1001: SetBuffSettingSkillExpectation |
| 痛苦魅痕 | MainPackage_Skill_1071_TongKuMeiHeng | 2 | 1 | 0 | 1 | 0:0/1/2/1 | P01=2 | P01=0×L | 使所有敌方单位获得[P01]层{Buff_MainPackage_CuiRuo_1_Title} | 0: AddBuffToAllEnemy; -1001: SetBuffSettingSkillExpectation |
| 无力魅痕 | MainPackage_Skill_1072_WuLiMeiHeng | 2 | 1 | 0 | 1 | 0:0/1/2/1 | P01=2 | P01=0×L | 使所有敌方单位获得[P01]层{Buff_MainPackage_XuRuo_12_Title} | 0: AddBuffToAllEnemy; -1001: SetBuffSettingSkillExpectation |
| 绞杀 | MainPackage_Skill_1073_JiaoSha | 0 | 1 | 0 | 2 | 0:0/1/2/1 | P01=7; P02=1 | P01=3×L; P02=1×L | 造成[P01]点伤害，如果造成了生命值伤害，则附加[P02]层{Buff_MainPackage_XuRuo_12_Title} | 0: DamageTargetConst; -1001: SetExpectionDamage,SetBuffSettingSkillExpectation; 1005: AfterDamageTargetThisSkill_AddBuffToTarget |
| 冲撞 | MainPackage_Skill_1074_ChongZhuang | 0 | 1 | 0 | 6 | 0:0/1/4/1 | P01=15; P02=10 | P01=5×L; P02=0×L | 造成[P01]点伤害，如果没造成生命值伤害，自身受到[P02]点真实伤害 | 0: DamageTargetConst; -1001: SetExpectionDamage,SetBuffSettingSkillExpectation; 1005: AfterDamageTargetThisSkill_NotDamage_SelfTakeDamage |
| 电流护盾 | MainPackage_Skill_1075_DianLiuHuDun | 1 | 1 | 0 | 6 | 0:0/1/5/1 | P01=15; P02=2 | P01=5×L; P02=0×L | 获得[P01]点护甲，并使所有敌人一个随机就绪技能冷却+[P02] | 0: GainArmor,AllEnemySkillGetCooldown_RandomCount; -1001: ArmorSkillExpectation,SetBuffSettingSkillExpectation |
| 龙血沸腾 | MainPackage_Skill_1076_LongXueFeiTeng | 2 | 2 | 1 | 2 | 0:0/0/0/1 | P01=0.5; P02=25; P03=3 | P01=0×L; P02=10×L; P03=3×L | 受到伤害后：如果生命值小于最大生命值的[P01]%，获得[P02]点护甲和[P03]层{Buff_MainPackage_Vitality_4_Title}（每场战斗限一次） | 1013: Special_LongXueFeiTeng_1076; 1008: ResetTagValue |
| 就是现在 | MainPackage_Skill_107_JiuShiXianZai | 2 | 4 | 0 | 4 | 0:5/2/16/1 | P01=10; COOLDOWN=0; P02=1 | P01=0×L; COOLDOWN=-2×L; P02=0×L | 使用条件:至少[P01]层{Buff_MainPackage_Vitality_4_Title}<br>移除[P01]层{Buff_MainPackage_Vitality_4_Title}并获得[P02]层{Buff_MainPackage_JiSu_6_Title} | 0: RemoveSelfTargetAllBuff,SelfAddBuff; -1001: SetBuffSettingSkillExpectation; -1002: SelfUnitHaveTargetBuffInfo |
| 震慑 | MainPackage_Skill_108_ZhenShe | 1 | 2 | 1 | 10 | 0:0/0/0/1 | P01=1.5 | P01=0.5×L | 回合开始时：对所有敌人造成[P01]%{Buff_MainPackage_Vitality_4_Title}层数的伤害 | 1010: DamageTargetByTargetBuff |
| 紧急处理 | MainPackage_Skill_109_JingJiChuLi | 0 | 4 | 0 | 10 | 0:0/0/12/0 | BuffLevel=3; RecoverValue=5 | BuffLevel=2×L; RecoverValue=5×L | 获得[BuffLevel]层{Buff_MainPackage_ShaoShang_7_Title}，恢复[RecoverValue]点生命值 | 0: SelfAddBuff,RecoverConstValue; -1001: SetBuffSettingSkillExpectation,SetHealthRecover |
| 笨重装甲 | MainPackage_Skill_10_BenZhongZhuangJia | 1 | 2 | 1 | 12 | 0:0/0/0/0 | P01=2; P02=1 | P01=3×L; P02=1×L | 防御力+[P01]，敏捷-[P02],智力-[P02] | 1001: SetTargetAttributeValue,SetTargetAttributeValue,SetTargetAttributeValue; 1002: SetTargetAttributeValue,SetTargetAttributeValue,SetTargetAttributeValue |
| 求生本能 | MainPackage_Skill_110_QiuShengBenNeng | 2 | 4 | 0 | 4 | 0:0/0/50/0 | P01=0.27; RecoverPrecent=0.45 | P01=0.03×L; RecoverPrecent=0.05×L | 使用条件：生命值低于最大生命值[P01]%<br>恢复最大生命值的[RecoverPrecent]% | 0: RecoverTargetPrecent; -1002: TargetSliderValue; -1001: SetHealthRecover |
| 急速冷却 | MainPackage_Skill_111_JiSuLengQue | 2 | 1 | 0 | 4 | 0:0/0/2/0 | P01=30; P02=2.3 | P01=0×L; P02=-0.3×L | 消耗至多[P01]点魔力，每消耗[P02]点魔力，使一个冷却最长的技能冷却-1 | 0: Special_JiSuLengQue; -1002: SourceContainCooldownSkill; -1001: Special_JiSuLengQue |
| 下作攻击 | MainPackage_Skill_112_XiaZuoGongJi | 0 | 4 | 0 | 15 | 0:0/1/3/1 | DamagePrecent=0.85 | DamagePrecent=0.15×L | 造成[DamagePrecent]%([DamagePrecent*2102])敏捷的伤害，如果目标有{Buff_MainPackage_ZhongDu_3_Title}，则额外造成一次伤害 | 0: Special_XiaZuoGongJi; -1001: Special_XiaZuoGongJi; -1003: TargetUnitHaveTargetBuffInfo |
| 毒之刃 | MainPackage_Skill_113_DuZhiRen | 0 | 2 | 1 | 10 | 0:0/0/0/1 | BuffCount=1 | BuffCount=1×L | 攻击造成伤害后：如果该次攻击对生命值造成了伤害，对目标附加[BuffCount]层{Buff_MainPackage_ZhongDu_3_Title} | 1005: Special_DuZhiRen |
| 暗中谋划 | MainPackage_Skill_114_AnZhongMouHua | 2 | 2 | 1 | 6 | 0:0/0/0/1 | BuffCount=1 | BuffCount=1×L | 战斗开始时：为所有敌人附加[BuffCount]层{Buff_MainPackage_CuiRuo_1_Title} | 1008: AddBuffToAllEnemy |
| 先发制人 | MainPackage_Skill_115_XianFaZhiRen | 1 | 4 | 0 | 10 | 0:0/1/2/1 | DamagePrecent=0.8; AdditionPrecent=2 | DamagePrecent=0.2×L; AdditionPrecent=0×L | 造成[DamagePrecent]%([DamagePrecent*2102])敏捷的伤害，如果目标的当前生命值为最大生命值的90%或以上，则该伤害X[AdditionPrecent]% | 0: Special_XianFaZhiRen_115; -1001: Special_XianFaZhiRen_115; -1003: TargetUnitSliderValue |
| 整备护甲 | MainPackage_Skill_116_ZhenBeiHuJia | 0 | 4 | 0 | 2 | 0:2/1/4/1 | ArmorCount=4; P01=0 | ArmorCount=4×L; P01=1×L | 获得[ArmorCount]点护甲,并获得[P01]层{Buff_MainPackage_BiLei_10_Title} | 0: SelfAddBuff,GainArmor; -1001: ArmorSkillExpectation,SetBuffSettingSkillExpectation |
| 烟雾弹 | MainPackage_Skill_117_YanWuDan | 1 | 1 | 0 | 10 | 0:2/2/4/1 | P01=6; P02=1 | P01=6×L; P02=0×L | 获得[P01]点护甲，并对所有敌人附加[P02]层{Buff_MainPackage_XuRuo_12_Title} | 0: GainArmor,AddBuffToAllEnemy; -1001: ArmorSkillExpectation |
| 毒素灌刺 | MainPackage_Skill_118_DuSuGuanCi | 2 | 4 | 0 | 4 | 0:0/0/5/1 | ReducePrecent=0.1; reduceCount=0.5 | ReducePrecent=0×L; reduceCount=0.5×L | 减少当前[ReducePrecent]%的法力值，每减少1点法力，附加[reduceCount]层{Buff_MainPackage_ZhongDu_3_Title} | 0: Special_DuSuGuanCi_118; -1001: Special_DuSuGuanCi_118 |
| 冥想 | MainPackage_Skill_119_MingXiang | 1 | 2 | 1 | 1 | 0:0/0/0/0 | P01=0 | P01=1×L | 交互营地时，如果生命值是满的，则智力+[P01] |  |
| 双刺 | MainPackage_Skill_119_ShuangCi | 1 | 4 | 0 | 10 | 0:0/2/4/1 | DamagePrecent=0.6; P02=2 | DamagePrecent=0.1×L; P02=0×L | 对所有敌人造成[DamagePrecent]%([DamagePrecent*2102])敏捷的伤害[P02]次 | 0: DamageAllByAttributeInfo; -1001: SetExpectionDamageByTargetAttribute |
| 侧重身体 | MainPackage_Skill_11_CeZhongShenTi | 1 | 2 | 1 | 12 | 0:0/0/0/0 | P01=0; P02=0 | P01=35×L; P02=10×L | 最大生命值+[P01]，最大魔力值-[P02] | 1001: SetTargetAttributeValue,SetTargetAttributeValue; 1002: SetTargetAttributeValue,SetTargetAttributeValue |
| 敏捷训练 | MainPackage_Skill_120_MinJieXunLian | 2 | 2 | 1 | 3 | 0:0/0/0/1 | NeedCount=9 | NeedCount=-1×L | 每使用[NeedCount]次武技,永久获得1点敏捷 |  |
| 力量训练 | MainPackage_Skill_121_LiLiangXunLian | 2 | 2 | 1 | 3 | 0:0/0/0/1 | NeedCount=11 | NeedCount=-1×L | 每次受到[NeedCount]次生命值伤害,永久获得1点力量 |  |
| 智力训练 | MainPackage_Skill_122_ZhiLiXunLian | 2 | 2 | 1 | 3 | 0:0/0/0/1 | NeedCount=11 | NeedCount=-1×L | 行动结束时：该技能冷却完毕时,永久获得1点智力，同时该技能冷却+[NeedCount] |  |
| 引毒术 | MainPackage_Skill_123_YinDuShu | 1 | 1 | 0 | 10 | 0:0/1/6/1 | ConstDamage=1.2 | ConstDamage=0.2×L | 目标每有一层{Buff_MainPackage_ZhongDu_3_Title}，造成[ConstDamage]点伤害 | 0: DamageTargetRefTargetBuff; -1001: DamageTargetRefTargetBuff |
| 暴风骤雨 | MainPackage_Skill_124_BaoFengZhouYu | 2 | 1 | 0 | 4 | 0:0/0/6/1 | DamagePrecent=0.5; NeedCount=5 | DamagePrecent=0.5×L; NeedCount=0×L | 造成[DamagePrecent]%([DamagePrecent*2102])敏捷的伤害，如果目标存在至少[NeedCount]层{Buff_MainPackage_ZhongDu_3_Title}，则移除[NeedCount]层额外造成一次伤害并使技能冷却降至1 | 0: Special_BaoFengZhouYu_124; -1001: Special_BaoFengZhouYu_124; -1003: TargetUnitHaveTargetBuffInfo |
| 奥术飞弹 | MainPackage_Skill_125_AoShuFeiDan | 0 | 1 | 0 | 15 | 0:4/0/1/1 | DamagePrecent=0.8 | DamagePrecent=0.2×L | 造成[DamagePrecent]%([DamagePrecent*2103])智力的伤害 | 0: DamageTarget; -1001: SetDamageCountExpectation,SetExpectionDamageByTargetAttribute |
| 魔法盾 | MainPackage_Skill_126_MoFaDun | 0 | 1 | 0 | 15 | 0:0/0/2/1 | P01=1; ReducePrecent=0.15; reduceCount=0.8 | P01=0×L; ReducePrecent=0×L; reduceCount=0.2×L | 获得[P01]层{Buff_MainPackage_BiLei_10_Title}，同时减少当前[ReducePrecent]%的法力值，每减少1点法力，获得[reduceCount]点护甲 | 0: Special_MoFaDun_126,SelfAddBuff; -1001: Special_MoFaDun_126 |
| 奥术双弹 | MainPackage_Skill_127_AoShuShuangDan | 1 | 1 | 0 | 10 | 0:4/1/2/1 | DamagePrecent=0.8; P02=2 | DamagePrecent=0.2×L; P02=0×L | 造成[DamagePrecent]%([DamagePrecent*2103])智力的伤害[P02]次 | 0: DamageTarget; -1001: SetDamageCountExpectation,SetExpectionDamageByTargetAttribute |
| 施法精通 | MainPackage_Skill_128_ShiFaJingTong | 1 | 1 | 0 | 10 | 0:0/0/20/1 | P01=3; P02=1 | P01=2×L; P02=0×L | 恢复[P01]点魔力，在使用一个魔法技能后，冷却-1 | 0: RecoverConstValue; 1014: Special_ShiFaJingTong_128,SetBuffSettingSkillExpectation |
| 奥术连击 | MainPackage_Skill_129_AoShuLianJi | 2 | 1 | 0 | 4 | 0:10/1/3/1 | DamagePrecent=0.6; P02=3 | DamagePrecent=0.4×L; P02=0×L | 造成[DamagePrecent]%([DamagePrecent*2103])智力的伤害[P02]次 | 0: DamageTarget; -1001: SetDamageCountExpectation,SetExpectionDamageByTargetAttribute |
| 荆棘甲 | MainPackage_Skill_12_JingJiJia | 0 | 2 | 1 | 10 | 0:0/1/0/1 | P01=2 | P01=2×L | 战斗开始时:获得[P01]层{Buff_MainPackage_JingJi_2_Title} | 1008: SelfAddBuff |
| 灼热领域 | MainPackage_Skill_130_ZhuoReLingYu | 2 | 2 | 1 | 4 | 0:0/0/0/1 | ConstValue=0 | ConstValue=2×L | 回合开始时：为所有敌方单位附加[ConstValue]层{Buff_MainPackage_ShaoShang_7_Title} | 1010: AddBuffToAllEnemy |
| 伤口切割 | MainPackage_Skill_131_ShangKouQieGe | 0 | 4 | 0 | 2 | 0:0/1/4/1 | ConstValue=6; AddBuff=0 | ConstValue=4×L; AddBuff=1×L | 造成[ConstValue]点伤害，并附加[AddBuff]层{Buff_MainPackage_CuiRuo_1_Title} | 0: DamageTargetConst,AddBuffToTarget; -1001: SetExpectionDamage,SetBuffSettingSkillExpectation |
| 战前下毒 | MainPackage_Skill_132_ZhanQianXiaDu | 0 | 2 | 1 | 15 | 0:0/0/0/1 | ConstValue=1 | ConstValue=2×L | 战斗开始时：为所有敌方单位附加[ConstValue]层{Buff_MainPackage_ZhongDu_3_Title} | 1008: AddBuffToAllEnemy |
| 破甲刃 | MainPackage_Skill_133_PoJiaRen | 1 | 4 | 0 | 10 | 0:1/1/2/1 | ConstValue=0.9; P02=2 | ConstValue=0.1×L; P02=1×L | 造成[ConstValue]%([ConstValue*2102])敏捷的伤害并附加[P02]层{Buff_MainPackage_QingShi_11_Title} | 0: DamageTargetByMix,AddBuffToTarget; -1001: SetExpectionDamageByTargetAttribute,SetBuffSettingSkillExpectation |
| 剧毒领域 | MainPackage_Skill_134_JuDuLingYu | 2 | 1 | 0 | 4 | 0:2/0/1/1 | ConstValue=3; P02=2 | ConstValue=2×L; P02=1×L | 为所有敌方单位附加[ConstValue]层{Buff_MainPackage_ZhongDu_3_Title}，为所有友方单位附加[P02]层{Buff_MainPackage_ZhongDu_3_Title} | 0: AddBuffToAllTeam,AddBuffToAllEnemy; -1001: SetBuffSettingSkillExpectation |
| 毒疗 | MainPackage_Skill_135_DuLiao | 1 | 1 | 0 | 10 | 0:3/1/3/0 | P01=1.2 | P01=0.2×L | 恢复等同于自身{Buff_MainPackage_ZhongDu_3_Title}层数[P01]%的生命值 | 0: Special_DuLiao_135; -1001: Special_DuLiao_135 |
| 毒血涌动 | MainPackage_Skill_136_DuXueYongDong | 2 | 6 | 1 | 4 | 0:0/0/0/1 | P01=0 | P01=2×L | 回合开始时：移除至多[P01]层{Buff_MainPackage_ZhongDu_3_Title}，获得等量的{Buff_MainPackage_Vitality_4_Title} | 1010: Special_DuXueYongDong_136 |
| 等待时机 | MainPackage_Skill_137_DengDaiShiJi | 1 | 2 | 1 | 3 | 0:0/0/0/1 | P01=7; P02=0 | P01=-2×L; P02=1×L | 回合开始时：如果该技能未进入冷却，为选定的敌人附加[P02]层{Buff_MainPackage_CuiRuo_1_Title}，之后该技能冷却+[P01] | 1010: AddBuffToTarget,ChangeCurSkillCooldown |
| 致命爆弹 | MainPackage_Skill_138_ZhiMingBaoDan | 2 | 2 | 1 | 4 | 0:0/0/0/1 | P01=6; P02=20 | P01=0×L; P02=15×L | 回合结束时，如果该技能已就绪，则对所有敌人造成[P02]的伤害，之后该技能冷却+[P01]<br>战斗开始时：该技能冷却+[P01] | 1008: ChangeCurSkillCooldown; 1010: Special_ZhiMingBaoDan_138 |
| 雷影步 | MainPackage_Skill_139_LeiYingBu | 2 | 2 | 1 | 3 | 0:0/0/0/1 | P01=8; P02=1 | P01=-1×L; P02=0×L | 战斗开始后的每[P01]个回合，获得[P02]层{Buff_MainPackage_JiSu_6_Title}(已过回合：<Counter>) | 1010: Special_LeiYingBu_139; 1008: RegistOrLogOffSkillTag; 1009: RegistOrLogOffSkillTag |
| 重击术 | MainPackage_Skill_13_ZhongJiShu | 0 | 4 | 0 | 10 | 0:0/1/1/1 | ConstValue=6; P03=2 | ConstValue=4×L; P03=0×L | 造成[ConstValue]点伤害 | 0: DamageTargetConst; -1001: SetExpectionDamage |
| 幻影连击 | MainPackage_Skill_140_HuanYingLianJi | 2 | 4 | 0 | 4 | 0:0/3/3/1 | P01=8 | P01=8×L | 造成[P01]点伤害，本场战斗中每使用一个不同的武技能使该技能伤害次数+1 | 0: Special_HuanYingLianJi_140; -1001: Special_HuanYingLianJi_140 |
| 固化添加物 | MainPackage_Skill_141_GuHuaTianJiaWu | 0 | 2 | 1 | 15 | 0:0/0/0/1 | P01=5 | P01=3×L | 战斗中每回合一次：在使用药水后，获得[P01]点护甲 | 1102: Special_GuHuaTianJiaWu_141; 1009: ResetTagValue; 1010: ResetTagValue |
| 高级炼制 | MainPackage_Skill_142_GaoJiLianZhi | 2 | 1 | 0 | 1 | 0:20/1/15/1 |  |  | 随机获得一个3级药剂 | 0: Special_YaoShuiLianZhi_143 |
| 药水炼制 | MainPackage_Skill_143_YaoShuiLianZhi | 0 | 1 | 0 | 1 | 0:7/1/12/1 |  |  | 随机获得一个1级药剂 | 0: Special_YaoShuiLianZhi_143 |
| 肾上腺注射 | MainPackage_Skill_144_ShenShangXianZhuSe | 1 | 1 | 0 | 10 | 0:4/0/4/0 | P01=2; P02=1 | P01=2×L; P02=1×L | 获得[P01]层{Buff_MainPackage_Vitality_4_Title}，同时对自身造成[P02]点伤害 | 0: DamageSelf,SelfAddBuff; -1001: SetBuffSettingSkillExpectation,SetBuffSettingSkillExpectation |
| 炼制爆破药剂 | MainPackage_Skill_145_LianZhiBaoPoYaoJi | 1 | 1 | 0 | 1 | 0:15/1/6/0 | P01=2 | P01=0×L | 将最多[P01]瓶{Buff_MainPackage_1003_KongYaoPing_Title}转化为劣质爆破药剂，如果没有{Buff_MainPackage_1003_KongYaoPing_Title}，则获得[P01]个{Buff_MainPackage_1003_KongYaoPing_Title} | 0: Special_LianZhiBaoPoYao_145; -1001: SetUnknowSkillExp |
| 酸液喷射 | MainPackage_Skill_146_SuanYePenShe | 1 | 1 | 0 | 10 | 0:3/0/4/1 | P02=4; P01=1 | P02=2×L; P01=1×L | 对所有敌人造成[P02]点伤害，使所有敌人获得[P01]层{Buff_MainPackage_QingShi_11_Title} | 0: AddBuffToAllEnemy,DamageByAllEnemyConstValue; -1001: SetExpectionDamage,SetBuffSettingSkillExpectation |
| 反击姿态 | MainPackage_Skill_147_FanJiZiTai | 0 | 4 | 0 | 10 | 0:0/1/2/1 | P01=2 | P01=1×L | 获得[P01]层{Buff_MainPackage_FanJi_5_Title} | 0: SelfAddBuff; -1001: SetBuffSettingSkillExpectation |
| 格挡反击 | MainPackage_Skill_148_GeDangFanJi | 0 | 2 | 1 | 10 | 0:0/0/0/1 | P01=0; P02=1 | P01=3×L; P02=0×L | 受到攻击后：如果受到的生命值伤害小于等于[P01]点，获得[P02]层{Buff_MainPackage_FanJi_5_Title} | 1012: Special_GeDangFanJi_148 |
| 反击螺旋 | MainPackage_Skill_149_FanJiLuoXuan | 2 | 2 | 1 | 4 | 0:0/0/0/1 | P01=0 | P01=0.1×L | 你的{Buff_MainPackage_FanJi_5_Title}效果会反击所有敌人，但是非主目标的单位只受到[Precent]%的伤害 |  |
| 侧重头脑 | MainPackage_Skill_14_CeZhongTouNao | 1 | 2 | 1 | 12 | 0:0/0/0/0 | P01=0; P02=0 | P01=40×L; P02=10×L | 最大魔力值+[P01]，最大生命值-[P02] | 1001: SetTargetAttributeValue,SetTargetAttributeValue; 1002: SetTargetAttributeValue,SetTargetAttributeValue |
| 反击大师 | MainPackage_Skill_150_FanJiDaShi | 1 | 2 | 1 | 10 | 0:0/0/0/1 | P01=0 | P01=0.1×L | 你的{Buff_MainPackage_FanJi_5_Title}造成的伤害+[P01]% | 1001: SetTargetAttributeValue; 1002: SetTargetAttributeValue |
| 弹开武器 | MainPackage_Skill_151_TanKaiWuQi | 1 | 2 | 1 | 6 | 0:0/0/0/1 | P01=0; P02=1 | P01=3×L; P02=0×L | 受到攻击后：如果受到的生命值伤害小于等于[P01]点，使攻击来源获得[P02]层{Buff_MainPackage_CuiRuo_1_Title} | 1012: Special_TanKaiWuQi_151 |
| 剑舞 | MainPackage_Skill_152_JianWu | 2 | 2 | 1 | 4 | 0:0/0/0/1 | P01=13 | P01=-1×L | 每攻击[P01]次后，获得一层{Buff_MainPackage_JiSu_6_Title}（已攻击次数:<ATTACKCOUNT>） | 1001: RegistOrLogOffSkillTag; 1002: RegistOrLogOffSkillTag; 1007: RegistOrLogOffListenTag,Special_JianWu_152 |
| 流转 | MainPackage_Skill_153_LiuZhuan | 2 | 2 | 1 | 4 | 0:0/0/0/1 | P01=0 | P01=1×L | 武器攻击后：使随机[P01]个武技冷却-1 | 1007: Special_LiuZhuan_153 |
| 反击风暴 | MainPackage_Skill_154_FanJiFengBao | 2 | 4 | 0 | 4 | 0:3/2/8/1 | P01=10; P02=2 | P01=10×L; P02=2×L | 获得[P01]点护甲，获得[P02]层{Buff_MainPackage_FanJi_5_Title} | 0: SelfAddBuff,GainArmor; -1001: ArmorSkillExpectation,SetBuffSettingSkillExpectation |
| 加大剂量 | MainPackage_Skill_155_JiaDaJiLiang | 2 | 2 | 1 | 1 | 0:0/0/0/2 | P01=3 | P01=-1×L | 你使用的药水会额外触发一次效果 | 1001: AddSpecialTag; 1002: RemoveSpecialTag |
| 寒冰箭 | MainPackage_Skill_156_HanBingJian | 0 | 1 | 0 | 15 | 0:5/1/2/1 | ConstValue=4; P02=2 | ConstValue=3×L; P02=2×L | 造成[ConstValue]点真实伤害，并附加[P02]层{Buff_MainPackage_HanShuang_8_Title} | 0: DamageTargetConstOnRealDamage,AddBuffToTarget; -1001: SetExpectionDamage,SetBuffSettingSkillExpectation |
| 冰霜亲和 | MainPackage_Skill_157_BingShuangQingHe | 1 | 1 | 1 | 10 | 0:0/0/0/1 | P01=0.5 | P01=0.5×L | 回合开始时：自身每有1层{Buff_MainPackage_HanShuang_8_Title}，获得[P01]点护甲 | 1010: Special_BingShuangQinHe_157 |
| 寒冰盾 | MainPackage_Skill_158_HanBingDun | 0 | 2 | 0 | 15 | 0:5/0/2/1 | P01=6; P02=2; P03=1 | P01=6×L; P02=2×L; P03=0×L | 获得[P01]点护甲，获得[P02]层{Buff_MainPackage_HanShuang_8_Title}和[P03]层{Buff_MainPackage_BiLei_10_Title} | 0: GainArmor,SelfAddBuff,SelfAddBuff; -1001: ArmorSkillExpectation,SetBuffSettingSkillExpectation,SetBuffSettingSkillExpectation |
| 极寒领域 | MainPackage_Skill_159_JiHanLingYu | 2 | 2 | 1 | 4 | 0:0/0/0/1 | P01=1 | P01=1×L | 回合开始时：使所有单位获得[P01]层{Buff_MainPackage_HanShuang_8_Title} | 1010: AddBuffToAll |
| 撞击术 | MainPackage_Skill_15_ZhuangJiShu | 0 | 4 | 0 | 10 | 0:0/1/2/1 | ConstValue=6; P02=2 | ConstValue=6×L; P02=0×L | 造成[ConstValue]点伤害，再对自身造成[P02]点伤害 | 0: DamageTargetConst,DamageSelf; -1001: SetExpectionDamage |
| 冰晶粉碎 | MainPackage_Skill_160_BingJinFenSui | 1 | 1 | 0 | 10 | 0:5/0/6/1 | ConstValue=5; P02=0 | ConstValue=0×L; P02=2×L | 造成[ConstValue]点伤害，目标每有1层{Buff_MainPackage_HanShuang_8_Title}使该技能伤害+[P02]，同时移除目标所有{Buff_MainPackage_HanShuang_8_Title} | 0: Special_BingJinFenSui_160; -1001: Special_BingJinFenSui_160 |
| 破冰术 | MainPackage_Skill_161_PoBingShu | 1 | 2 | 1 | 6 | 0:0/0/0/1 | P01=0; P02=2 | P01=2×L; P02=2×L | 造成伤害后：移除目标最多[P01]层{Buff_MainPackage_HanShuang_8_Title}，每移除一层造成[P02]点伤害 | 1006: Special_PoBingShu_161 |
| 冰霜附魔 | MainPackage_Skill_162_BingShuangFuMo | 2 | 2 | 1 | 4 | 0:0/0/0/1 | P01=1 | P01=1×L | 攻击后：为目标附加[P01]层{Buff_MainPackage_HanShuang_8_Title} | 1007: AddBuffToTarget |
| 火焰附魔 | MainPackage_Skill_163_HuoYanFuMo | 1 | 2 | 1 | 10 | 0:0/0/0/1 | P01=1 | P01=1×L | 攻击后：为目标附加[P01]层{Buff_MainPackage_ShaoShang_7_Title} | 1007: AddBuffToTarget |
| 火焰亲和 | MainPackage_Skill_164_HuoYanQingHe | 2 | 2 | 1 | 4 | 0:0/0/0/1 | P01=1 | P01=1×L | 回合开始时：将最多[P01]层{Buff_MainPackage_ShaoShang_7_Title}转化为{Buff_MainPackage_Vitality_4_Title} | 1010: Special_HuoYanQingHe_164 |
| 狂野焰火 | MainPackage_Skill_165_KuangYeYanHuo | 0 | 1 | 0 | 15 | 0:0/0/1/1 | ConstValue=5; P03=0 | ConstValue=5×L; P03=1×L | 造成[ConstValue]点伤害，同时对自身附加[P03]层{Buff_MainPackage_ShaoShang_7_Title} | 0: DamageTargetConst,SelfAddBuff; -1001: SetExpectionDamage,SetBuffSettingSkillExpectation |
| 狂野施法 | MainPackage_Skill_166_KuangYeShiFa | 2 | 1 | 0 | 4 | 0:0/0/2/0 | P01=1; P02=0 | P01=2×L; P02=1×L | 将冷却最高且最多2个技能的冷却-[P01]，为自身附加[P02]层{Buff_MainPackage_ShaoShang_7_Title} | 0: ReduceHighestCoolDown,SelfAddBuff; -1001: SetBuffSettingSkillExpectation,SetBuffSettingSkillExpectation |
| 残留魔力 | MainPackage_Skill_167_CanLiuMoLi | 1 | 2 | 1 | 6 | 0:0/0/0/1 | P01=2 | P01=2×L | 回合开始时：如果当前魔力值小于[P01]，则将魔力值恢复至[P01] | 1010: Special_CanLiuMoLi_167 |
| 抵抗 | MainPackage_Skill_168_FangYu | 0 | 4 | 0 | 15 | 0:1/1/1/1 | DefensePrecent=1.8 | DefensePrecent=0.2×L | 获得[DefensePrecent]%([DefensePrecent*2002])防御力的护甲 | 0: Special_FangYu_168; -1001: Special_FangYu_168 |
| 表皮硬化 | MainPackage_Skill_169_BiaoPiYingHua | 1 | 2 | 1 | 10 | 0:0/0/0/1 | P01=0.03; P02=10 | P01=0.015×L; P02=0×L | 回合开始时：获得当前生命值[P01]%的护甲，每场战限[P02]次（剩余<Counter>次） | 1010: Special_BiaoPiYingHua_169; 1008: ResetTagValue |
| 研习 | MainPackage_Skill_16_YanXi | 2 | 2 | 1 | 4 | 0:0/0/0/1 | P01=0 | P01=1×L | 选择技能时，获得[P01]次免费的刷新 | 1001: SetTargetAttributeValue; 1002: SetTargetAttributeValue |
| 点燃 | MainPackage_Skill_170_DianRan | 1 | 1 | 0 | 10 | 0:5/1/4/1 | ConstDamage=1 | ConstDamage=0.4×L | 目标每有一层{Buff_MainPackage_ShaoShang_7_Title}，造成[ConstDamage]点真实伤害 | 0: CacheAudioName,DamageTargetRefTargetBuff; -1001: DamageTargetRefTargetBuff |
| 瓶装活力 | MainPackage_Skill_171_PingZhuangHuoLi | 0 | 2 | 1 | 4 | 0:0/0/0/1 | P01=0 | P01=0.25×L | 交互营地时，有[P01]%的概率获得一瓶劣质恢复药剂<br>交互魔泉时，有[P01]%的概率获得一瓶劣质魔法药剂 | 1001: AddSpecialTag; 1002: RemoveSpecialTag |
| 健体添加物 | MainPackage_Skill_172_JianTiTianJiaWu | 2 | 2 | 1 | 1 | 0:0/0/0/2 | P01=5; P02=4 | P01=0×L; P02=1×L | 在使用了[P01]次药水后(不包括空药品)，永久提升[P02]点最大生命值(已使用次数:<UseCount>) | 1102: Special_XueChang_172 |
| 格挡 | MainPackage_Skill_173_GeDang | 0 | 4 | 0 | 20 | 0:1/1/0/1 | P01=4 | P01=1×L | 获得[P01]点护甲 | 0: GainArmor; -1001: ArmorSkillExpectation |
| 全身撞击 | MainPackage_Skill_174_QuanShenZhuangJi | 0 | 4 | 0 | 15 | 0:0/1/2/1 | DamagePrecent=0.1 | DamagePrecent=0.04×L | 造成自身最大生命值[DamagePrecent]%([DamagePrecent*1001])的伤害 | 0: DamageTarget; -1001: SetExpectionDamageByTargetAttribute |
| 制作空药瓶 | MainPackage_Skill_175_ZhiZuoKongYaoPing | 0 | 1 | 0 | 6 | 0:5/1/6/0 | P01=1 | P01=1×L | 获得[P01]个{Buff_MainPackage_1003_KongYaoPing_Title} | 0: GetTargetPoition |
| 爆炸添加物 | MainPackage_Skill_176_BaoZhaTianJiaWu | 0 | 2 | 1 | 15 | 0:0/0/0/0 | P01=2 | P01=3×L | 使用药水后:对一个随机敌人造成[P01]点伤害 | 1102: DamageRandomEnemy |
| 暗器 | MainPackage_Skill_177_AnQi | 1 | 2 | 1 | 10 | 0:0/0/0/0 | P01=2 | P01=2×L | 攻击后:对一个随机敌人造成[P01]点伤害 | 1007: DamageRandomEnemy |
| 虚灵化 | MainPackage_Skill_178_XuLingHua | 1 | 2 | 1 | 6 | 0:0/0/0/0 | P01=3 | P01=3×L | 战斗开始时：将当前法力值变为0<br>攻击后:恢复[P01]点法力值 | 1008: RecoverConstValue; 1007: RecoverConstValue |
| 无影斩 | MainPackage_Skill_179_WuYingZhan | 1 | 4 | 0 | 10 | 0:0/1/4/1 | P01=4; P02=5 | P01=3×L; P02=0×L | 造成[P01]点伤害，每有[P02]点敏捷，伤害次数额外+1 | 0: Special_WuYingZhan_179; -1001: Special_WuYingZhan_179 |
| 急躁 | MainPackage_Skill_17_JiZao | 1 | 4 | 0 | 6 | 0:5/1/2/1 | P01=2 | P01=2×L | 如果你的所有主动技能(不包括本技能)都处于冷却状态，使所有技能的冷却-[P01] | 0: Special_JiZao_17; -1001: SetBuffSettingSkillExpectation; -1002: AllActiveSkillIsCooldown |
| 防御提升 | MainPackage_Skill_180_FangYuTiSheng | 1 | 2 | 1 | 20 | 0:0/0/0/0 | P01=0 | P01=2×L | 防御力+[P01] | 1001: SetTargetAttributeValue; 1002: SetTargetAttributeValue |
| 移动堡垒 | MainPackage_Skill_181_YiDongBaoLei | 2 | 2 | 1 | 4 | 0:0/0/0/0 | P01=3 | P01=1×L | 战斗开始时:获得[P01]层{Buff_MainPackage_BiLei_10_Title} | 1008: SelfAddBuff |
| 攻守兼备 | MainPackage_Skill_182_GongShouJianBei | 1 | 2 | 1 | 10 | 0:0/0/0/0 | P01=2 | P01=2×L | 武器攻击后:获得[P01]点护甲 | 1007: Special_GongShouJianBei_182 |
| 荆棘缠身 | MainPackage_Skill_183_JingJiCanShen | 2 | 1 | 0 | 4 | 0:0/0/2/1 | P01=0 | P01=5×L | 将最多[P01]点护甲转化为{Buff_MainPackage_JingJi_2_Title} | 0: Special_JingJiChanShen_183; -1001: Special_JingJiChanShen_183 |
| 野蛮生长 | MainPackage_Skill_184_YeManShengZhang | 2 | 2 | 1 | 4 | 0:0/0/0/0 | P01=1; P02=1 | P01=2×L; P02=1×L | 回合开始时:获得[P01]层{Buff_MainPackage_JingJi_2_Title}，并受到[P02]点{Buff_MainPackage_1006_ZhenShiShangHai_Title} | 1010: SelfAddBuff,DamageSelfByReal |
| 荆棘护身 | MainPackage_Skill_185_JingJiHuShen | 1 | 2 | 0 | 10 | 0:0/1/4/1 | P01=0.5 | P01=0.5×L | 每有1层{Buff_MainPackage_JingJi_2_Title}，获得[P01]点护甲 | 0: Special_JingJiHuShen_184; -1001: Special_JingJiHuShen_184 |
| 防御阵列 | MainPackage_Skill_186_FangYuZhenLie | 0 | 4 | 0 | 15 | 0:2/2/3/1 | P01=8 | P01=7×L | 使所有的友方单位获得[P01]层护甲 | 0: AllTeamMemberGainArmor; -1001: ArmorSkillExpectation |
| 鞭挞 | MainPackage_Skill_187_BianTa | 1 | 4 | 0 | 10 | 0:0/1/0/1 | P01=0; P02=1 | P01=4×L; P02=2×L | 移除最多[P01]层{Buff_MainPackage_JingJi_2_Title}，每移除1层，对所有敌人造成[P02]点伤害 | 0: Special_BianTa_187; -1001: Special_BianTa_187 |
| 忍耐 | MainPackage_Skill_188_RenNai | 0 | 2 | 1 | 10 | 0:0/0/0/0 | P01=0; P02=2 | P01=2×L; P02=2×L | 受到伤害时:如果伤害来源为自身，则受到的伤害-[P01] | 1001: AddSpecialTag; 1002: RemoveSpecialTag |
| 硬撑 | MainPackage_Skill_189_YingCheng | 1 | 4 | 0 | 10 | 0:1/0/3/1 | ArmorCount=6; P01=3 | ArmorCount=6×L; P01=0×L | 获得[ArmorCount]点护甲,使一个随机未就绪的主动技能的冷却+[P01] | 0: GainArmor,TargetActiveSkillGetCooldown; -1001: ArmorSkillExpectation,SetBuffSettingSkillExpectation |
| 落井下石 | MainPackage_Skill_18_LuoJingXiaShi | 2 | 2 | 1 | 4 | 0:0/0/0/1 | P01=0 | P01=5×L | 在你为一个敌方单位附加负面状态时，对目标造成[P01]点伤害 |  |
| 放马过来 | MainPackage_Skill_190_FangMaGuoLai | 1 | 4 | 0 | 10 | 0:1/2/3/1 | P01=1; P02=5 | P01=1×L; P02=5×L | 获得[P01]层{Buff_MainPackage_FanJi_5_Title}，同时每有一个冷却中的其他技能使自身获得[P02]点护甲 | 0: SelfAddBuff,GetArmorByHaveCoolDownSkill; -1001: SetBuffSettingSkillExpectation,GetArmorByHaveCoolDownSkill |
| 反击准备 | MainPackage_Skill_191_FanJiZhunBei | 0 | 2 | 1 | 10 | 0:0/0/0/0 | P01=0 | P01=2×L | 战斗开始时:获得[P01]层{Buff_MainPackage_FanJi_5_Title} | 1008: SelfAddBuff |
| 专注重击 | MainPackage_Skill_192_ZhuanZhuZhongJi | 2 | 4 | 0 | 4 | 0:0/3/3/1 | P01=9 | P01=9×L | 造成[P01]点伤害，每有一个就绪的其他主动技能，便使伤害次数+1 | 0: Special_ZhuanZhuZhongji_192; -1001: Special_ZhuanZhuZhongji_192 |
| 心流 | MainPackage_Skill_193_XinLiu | 1 | 2 | 1 | 6 | 0:0/0/0/0 | P01=3 | P01=2×L | 当技能冷却完毕：对一个随机敌人造成[P01]点伤害 |  |
| 据守 | MainPackage_Skill_194_JuShou | 1 | 2 | 1 | 6 | 0:0/0/0/0 | P01=2 | P01=2×L | 回合开始时：如果上回合未攻击，则获得[P01]点护甲 |  |
| 冲动克制 | MainPackage_Skill_195_ChongDongKeZhi | 2 | 2 | 1 | 4 | 0:0/0/0/0 | P01=1 | P01=1×L | 回合开始时：如果上回合未攻击，则使所有技能冷却-[P01] |  |
| 喘息 | MainPackage_Skill_196_ChuanXi | 0 | 4 | 0 | 10 | 0:0/2/2/1 | P01=6 | P01=4×L | 恢复[P01]点魔力值 | 0: RecoverConstValue; -1001: SetSkillExpectionType |
| 节奏斩击 | MainPackage_Skill_197_JieZouZhanJi | 1 | 4 | 0 | 15 | 0:0/1/2/1 | P01=3; P02=2 | P01=2×L; P02=2×L | 造成[P01]点伤害，每使用一次，伤害+[P02]，直到战斗结束（已使用次数:<UseCount>） | 0: Special_JieZouZhanJi_197,RegistOrLogOffListenTag; -1001: Special_JieZouZhanJi_197; 1008: RegistOrLogOffSkillTag; 1009: RegistOrLogOffSkillTag |
| 快速出鞘 | MainPackage_Skill_198_KuaiSuChuQiao | 0 | 4 | 0 | 10 | 0:0/0/1/1 | P01=2 | P01=2×L | 造成[P01]点伤害 | 0: DamageTargetConst; -1001: SetExpectionDamage |
| 透支 | MainPackage_Skill_199_TouZhi | 0 | 4 | 0 | 10 | 0:0/0/3/1 | P01=2 | P01=2×L | 获得[P01]层临时{Buff_MainPackage_Vitality_4_Title} | 0: SelfAddBuff,SelfAddBuff; -1001: SetBuffSettingSkillExpectation,SetBuffSettingSkillExpectation |
| 力量训练 | MainPackage_Skill_19_LiLiangXunLian | 2 | 2 | 1 | 1 | 0:0/0/0/1 | P01=1; P02=0 | P01=1×L; P02=1×L | 战斗开始时：获得[P01]层{Buff_MainPackage_CuiRuo_1_Title}<br>战斗结束时：永久提升[P02]点力量 | 1008: SelfAddBuff; 1009: ChangeTargertAttribute |
| 力量提升 | MainPackage_Skill_1_LiLiangTisheng | 0 | 2 | 1 | 20 | 0:0/0/0/1 | P01=0 | P01=2×L | 力量+[P01] | 1001: SetTargetAttributeValue; 1002: SetTargetAttributeValue |
| 契约召唤：莉莉丝 | MainPackage_Skill_2001_MeiMoQiYue | 2 | 1 | 0 | 4 | 0:10/2/2/1 | P01=0; P02=0 | P01=20×L; P02=1×L | 召唤一个魅魔，魅魔额外获得[P01]点生命值（该技能等级越高，魅惑之吻越强）。如果魅魔已存在，改为恢复魅魔所有生命值。 | 0: CacheTargetSkillData,CacheTargetAttributeData,Special_WeiSuo_1014; -1001: SetUnknowSkillExp |
| 连环打击 | MainPackage_Skill_200_LianHuanDaJi | 1 | 4 | 0 | 3 | 0:0/1/4/1 | P01=3; P02=1 | P01=2×L; P02=1×L | 造成[P01]点伤害[P02]次，随机分配到所有敌人上 | 0: DamageRandomEnemy; -1001: SetExpectionDamage,SetDamageCountExpectation |
| 威吓怒吼 | MainPackage_Skill_201_WeiHeNuHou | 1 | 4 | 0 | 6 | 0:2/0/4/1 | P01=1 | P01=1×L | 使所有敌人获得[P01]层{Buff_MainPackage_XuRuo_12_Title} | 0: AddBuffToAllEnemy; -1001: SetBuffSettingSkillExpectation |
| 血偿 | MainPackage_Skill_202_XueChang | 1 | 4 | 0 | 15 | 0:0/4/2/1 | P01=7; P02=1 | P01=7×L; P02=0×L | 造成[P01]点伤害；每受到1次生命值伤害，该技能行动力消耗-[P02]，直到战斗结束 | 0: DamageTargetConst; -1001: SetExpectionDamage; 1013: Special_XueChang_202; 1009: RemoveTargetSkillActionChange |
| 坚不可摧 | MainPackage_Skill_203_JianBuKeCui | 2 | 1 | 0 | 4 | 0:5/1/15/1 | P01=0.25 | P01=0.25×L | 获得当前护甲值[P01]%的护甲<br>战斗开始时：重制该技能冷却 | 0: GainArmorByCurrentArmor; -1001: GainArmorByCurrentArmor; 1008: ChangeCurSkillCooldown |
| 生命沸腾 | MainPackage_Skill_204_ShengMingFeiTeng | 1 | 4 | 0 | 3 | 0:0/1/8/1 | P01=2; P02=1; P03=4 | P01=0×L; P02=1×L; P03=0×L | 对自身造成[P01]点真实伤害，获得[P02]点行动力<br>战斗开始时：该技能冷却-[P03] | 0: DamageSelfByReal,GainActionPoint; -1001: SetBuffSettingSkillExpectation,SetExpectionDamage; 1008: ChangeCurSkillCooldown |
| 以攻为守 | MainPackage_Skill_205_YiGongWeiShou | 2 | 4 | 0 | 4 | 0:0/0/2/1 | P01=3 | P01=3×L | 使用后，你在这个回合内每攻击一次，获得[P01]格挡 | 0: SelfAddBuff; -1001: SetSkillExpectionType |
| 生命燃烧 | MainPackage_Skill_206_ShengMingRanShao | 1 | 2 | 1 | 10 | 0:0/0/0/1 | P01=4; P02=0 | P01=0×L; P02=1×L | 回合结束时触发[P02]次：对自身造成1点真实伤害，对所有敌人造成[P01]点伤害 | 1011: Special_ShengMingRanShao_206 |
| 刹那双影 | MainPackage_Skill_207_ChaNaShuangYing | 2 | 1 | 0 | 1 | 0:5/1/3/1 | COOLDOWN=0 | COOLDOWN=-1×L | 使用后，本回合的下一个武技会触发两次 | 0: SelfAddBuff; -1001: SetBuffSettingSkillExpectation |
| 痛苦专注 | MainPackage_Skill_208_TongKuZhuanZhu | 2 | 2 | 1 | 4 | 0:0/0/0/1 | P01=1 | P01=1×L | 回合结束时：受到一点真实伤害，使最左边的未就绪的技能冷却-[P01] | 1011: DamageSelfByReal,LeftSkillCoolDownChange |
| 活力之潮 | MainPackage_Skill_209_HuoLiZhiChao | 2 | 1 | 0 | 4 | 0:5/1/5/1 | P01=0.25; P02=4 | P01=0.25×L; P02=1×L | 获得自身[P01]%层数{Buff_MainPackage_Vitality_4_Title}的{Buff_MainPackage_Vitality_4_Title}<br>战斗开始时：该技能冷却-[P02] | 0: ChangeSelfBuffByPrecent; -1001: ChangeSelfBuffByPrecent; 1008: ChangeCurSkillCooldown |
| 敏捷训练 | MainPackage_Skill_20_MinJieXunLian | 2 | 2 | 1 | 1 | 0:0/0/0/1 | P01=1; P02=0 | P01=1×L; P02=1×L | 战斗开始时：获得[P01]层{Buff_MainPackage_1015_ChenZhong_Title}<br>战斗结束时：永久提升[P02]点敏捷 | 1008: SelfAddBuff; 1009: ChangeTargertAttribute |
| 毁灭重击 | MainPackage_Skill_210_HuiMieZhanJi | 1 | 4 | 0 | 10 | 0:0/3/2/1 | ConstValue=20 | ConstValue=15×L | 造成[ConstValue]点伤害 | 0: CacheEffect,DamageTargetConst; -1001: SetExpectionDamage |
| 临时格挡 | MainPackage_Skill_211_LingShiGeDang | 0 | 4 | 0 | 15 | 0:1/0/1/1 | ArmorCount=2 | ArmorCount=2×L | 获得[ArmorCount]点护甲 | 0: GainArmor; -1001: ArmorSkillExpectation |
| 背刺 | MainPackage_Skill_212_BeiCi | 0 | 4 | 0 | 15 | 0:0/0/1/1 | ConstValue=2 | ConstValue=3×L | 造成[ConstValue]点真实伤害 | 0: CacheAudioName,DamageTargetConstOnRealDamage; -1001: SetToRealDamage |
| 战术格挡 | MainPackage_Skill_213_ZhanShuGeDang | 1 | 4 | 0 | 10 | 0:1/1/4/1 | ArmorCount=5; P01=2 | ArmorCount=4×L; P01=0×L | 获得[ArmorCount]点护甲，随机使[P01]个技能冷却-1 | 0: GainArmor,ReduceCooldownRandomColldownSkill; -1001: ArmorSkillExpectation,SetBuffSettingSkillExpectation |
| 迅袭 | MainPackage_Skill_214_XunXi | 1 | 4 | 0 | 10 | 0:1/1/3/1 | P01=4; P02=2 | P01=4×L; P02=0×L | 造成[P01]点伤害，随机使[P02]个技能冷却-1 | 0: DamageTargetConst,ReduceCooldownRandomColldownSkill; -1001: SetExpectionDamage,SetBuffSettingSkillExpectation |
| 战术调整 | MainPackage_Skill_215_ZhanShuTiaoZheng | 1 | 4 | 0 | 3 | 0:0/1/3/1 | P01=1 | P01=1×L | 使下回合额外获得[P01]行动力 | 0: GainActionNextTurn; -1001: SetBuffSettingSkillExpectation |
| 扫腿 | MainPackage_Skill_216_SaoTui | 0 | 4 | 0 | 3 | 0:0/1/4/1 | P01=5; P02=0 | P01=3×L; P02=1×L | 造成[P01]点伤害，附加[P02]层{Buff_MainPackage_XuRuo_12_Title} | 0: DamageTargetConst,AddBuffToTarget; -1001: SetExpectionDamage,SetBuffSettingSkillExpectation |
| 膝踢 | MainPackage_Skill_217_XiTi | 1 | 4 | 0 | 3 | 0:0/1/4/1 | P01=5; P02=0 | P01=3×L; P02=1×L | 造成[P01]点伤害，同时使下回合额外获得[P02]行动力 | 0: DamageTargetConst,GainActionNextTurn; -1001: SetExpectionDamage,SetBuffSettingSkillExpectation |
| 贯穿冲击 | MainPackage_Skill_218_GuanChuanChongJi | 2 | 4 | 0 | 4 | 0:0/0/1/1 | P01=7 | P01=7×L | 消耗所有行动力，每消耗1点行动力，造成一次[P01]点伤害 | 0: DamageSelfByLeftAction,SetActionToZero; -1001: DamageSelfByLeftAction |
| 风险突袭 | MainPackage_Skill_219_FengXianTuXi | 0 | 4 | 0 | 15 | 0:0/1/0/1 | P01=5; P02=1; P03=2 | P01=5×L; P02=0×L; P03=0×L | 对所有敌人造成[P01]点伤害，同时使最右边就绪的主动技能冷却+[P03] | 0: DamageByAllEnemyConstValue,TargetRightestSkillGetCooldown; -1001: SetExpectionDamage,SetBuffSettingSkillExpectation |
| 智力训练 | MainPackage_Skill_21_ZhiLiXunLian | 2 | 2 | 1 | 1 | 0:0/0/0/1 | P01=10; P02=0 | P01=5×L; P02=1×L | 战斗开始时：当前魔力-[P01]<br>战斗结束时：永久提升[P02]点智力 | 1008: RecoverConstValue; 1009: ChangeTargertAttribute |
| 毒甲术 | MainPackage_Skill_220_DuJiaShu | 1 | 1 | 0 | 10 | 0:3/2/4/1 | P01=8; P02=3 | P01=4×L; P02=3×L | 获得[P01]点护甲，同时使目标获得[P02]层中毒 | 0: GainArmor,AddBuffToTarget; -1001: ArmorSkillExpectation,SetBuffSettingSkillExpectation |
| 惯性失控 | MainPackage_Skill_221_GuanXingShiKong | 0 | 4 | 0 | 15 | 0:0/0/1/1 | P01=5 | P01=5×L | 造成[P01]点伤害，同时该技能的行动力消耗+1 | 0: DamageTargetConst,ChangeTargetSkillActionCost; 1009: RemoveTargetSkillActionChange; -1001: SetExpectionDamage |
| 收尾 | MainPackage_Skill_222_Shouwei | 2 | 4 | 0 | 6 | 0:0/1/3/1 | P01=6 | P01=6×L | 造成[P01]点伤害，本回合内每使用过一个不同的武技，伤害次数+1（已使用：<Counter>） | 0: Special_ShouWei_222,SpawnEffectToTarget_OnAttackTarget; -1001: Special_ShouWei_222; 1010: Special_ShouWei_Counter; 1014: Special_ShouWei_Counter; 1009: RegistOrLogOffSkillTag |
| 压制 | MainPackage_Skill_223_YaZhi | 0 | 4 | 0 | 10 | 0:0/0/20/1 | P01=6; P02=0 | P01=3×L; P02=1×L | 造成[P01]点伤害，并附加[P02]层{Buff_MainPackage_XuRuo_12_Title}，该技能战斗开始时会重制冷却 | 0: DamageByAllEnemyConstValue,AddBuffToAllEnemy; 1008: ChangeCurSkillCooldown; -1001: SetExpectionDamage,SetBuffSettingSkillExpectation |
| 衰败吐息 | MainPackage_Skill_224_ShuaiBaiTuXi | 1 | 1 | 0 | 10 | 0:5/1/5/1 | P01=2; P02=2 | P01=2×L; P02=0×L | 使所有敌人获得[P01]层{Buff_MainPackage_ZhongDu_3_Title}和[P02]层{Buff_MainPackage_XuRuo_12_Title} | 0: AddBuffToAllEnemy,AddBuffToAllEnemy; -1001: SetBuffSettingSkillExpectation |
| 破绽回势 | MainPackage_Skill_225_PoZhanHuiShi | 1 | 4 | 0 | 10 | 0:0/2/4/1 | P01=7; P02=2 | P01=7×L; P02=0×L | 造成[P01]点伤害，如果目标有{Buff_MainPackage_XuRuo_12_Title}，则获得[P02]点行动力 | 0: DamageTargetConst,Special_ShouWei_225; -1001: SetExpectionDamage |
| 刃风 | MainPackage_Skill_226_RenFeng | 2 | 2 | 1 | 4 | 0:0/0/0/1 | P01=0 | P01=2×L | 使用技能后:如果该技能是武技，则对所有敌人造成[P01]点伤害 | 1014: Special_RenFeng_226 |
| 时空领域 | MainPackage_Skill_227_ShiKongLingYu | 2 | 1 | 0 | 4 | 0:5/2/4/1 | P01=1 | P01=1×L | 本回合内，使你下次使用的[P01]个技能的行动力消耗变为0 | 0: SelfAddBuff; -1001: SetBuffSettingSkillExpectation |
| 肃清 | MainPackage_Skill_228_SuQing | 2 | 4 | 0 | 4 | 0:0/0/3/1 | P01=8 | P01=8×L | 对所有敌人造成[P01]点伤害，如果该技能造成了击杀，重置该技能的冷却时间 | 0: Special_SuQing_228; -1001: SetExpectionDamage |
| 借势打击 | MainPackage_Skill_229_JieShiDaJi | 1 | 4 | 0 | 10 | 0:0/0/10/1 | P01=4 | P01=4×L | 造成[P01]点伤害；攻击后：该技能冷却-1 | 0: DamageTargetConst; 1007: ChangeCurSkillCooldown; -1001: SetExpectionDamage |
| 鲜血荆棘 | MainPackage_Skill_22_XianXueJingJi | 1 | 1 | 0 | 10 | 1:0/1/3/1 | P01=3; P02=2 | P01=2×L; P02=0×L | 获得[P01]层{Buff_MainPackage_JingJi_2_Title}，并对自身造成[P02]点{Buff_MainPackage_1006_ZhenShiShangHai_Title} | 0: SelfAddBuff,DamageSelfByReal; -1001: SetBuffSettingSkillExpectation,SetToSelfDamage |
| 注毒刺击 | MainPackage_Skill_230_ZhuDuCiJi | 1 | 4 | 0 | 10 | 0:2/1/4/1 | P01=4 | P01=3×L | 造成[P01]点伤害，该技能每造成1点生命值伤害，使目标获得1层中毒 | 0: DamageTargetConst; 1005: Special_ShouWei_230; -1001: SetExpectionDamage,SetBuffSettingSkillExpectation |
| 协同攻击 | MainPackage_Skill_231_XieTongGongJi | 1 | 4 | 0 | 10 | 0:0/2/3/1 | P01=6 | P01=6×L | 造成[P01]点伤害，场上每有一个其他友方单位便使伤害次数+1 | 0: Special_XieTongGongJi_231; -1001: Special_XieTongGongJi_231 |
| 二重唱 | MainPackage_Skill_232_ErChongChang | 2 | 1 | 0 | 1 | 0:5/0/4/1 |  |  | 使用后，本回合的下一个魔法会触发两次 | 0: SelfAddBuff; -1001: SetBuffSettingSkillExpectation |
| 冬眠 | MainPackage_Skill_233_DongMian | 1 | 1 | 0 | 10 | 0:5/1/99/1 | P01=0; P02=1 | P01=4×L; P02=0×L | 使目标获得[P02]层{Buff_MainPackage_ShuiMian_15_Title}和[P01]层寒霜<br>战斗开始时重置冷却 | 0: AddBuffToTarget,AddBuffToTarget; -1001: SetBuffSettingSkillExpectation; 1008: ChangeCurSkillCooldown |
| 警戒吟唱 | MainPackage_Skill_234_JingJieYongChang | 0 | 1 | 0 | 15 | 0:1/1/2/1 | P01=4; P02=6 | P01=4×L; P02=2×L | 获得[P02]点护甲。如果是第一次使用，恢复[P01]点魔力 | 0: GainArmor,Special_YinChangGroup_234; -1001: ArmorSkillExpectation,Special_YinChangGroup_234; 1008: ResetTagValue |
| 律动吟唱 | MainPackage_Skill_235_LvDongYongChang | 1 | 1 | 0 | 10 | 0:0/1/5/1 | P01=4; P02=1 | P01=4×L; P02=1×L | 随机的一个未就绪的技能冷却-[P02]。如果是第一次使用，恢复[P01]点魔力 | 0: ReduceCooldownRandomColldownSkill,Special_YinChangGroup_234; -1001: SetBuffSettingSkillExpectation,Special_YinChangGroup_234; 1008: ResetTagValue |
| 狂妄咏唱 | MainPackage_Skill_236_KuangWangYongChang | 1 | 1 | 0 | 10 | 0:0/1/5/1 | P01=4; P02=2 | P01=4×L; P02=0×L | 使下回合行动力+[P02]。如果是第一次使用，恢复[P01]点魔力 | 0: GainActionNextTurn,Special_YinChangGroup_234; -1001: SetBuffSettingSkillExpectation,Special_YinChangGroup_234; 1008: ResetTagValue |
| 灵感吟唱 | MainPackage_Skill_237_LingGanYongChang | 2 | 1 | 0 | 4 | 0:0/0/4/1 | P01=0 | P01=10×L | 本回合的下一个技能的行动消耗变为0。如果是第一次使用，恢复[P01]点魔力 | 0: SelfAddBuff,Special_YinChangGroup_234; -1001: SetBuffSettingSkillExpectation,Special_YinChangGroup_234; 1008: ResetTagValue |
| 间接施法 | MainPackage_Skill_238_JianJieShiFa | 2 | 1 | 0 | 4 | 0:10/2/5/1 | COOLDOWN=0; P01=4 | COOLDOWN=1×L; P01=0×L | （使用时无效果）。回合开始时：如果该技能的当前冷却大于等于[P01]，则无消耗的释放你最左边的魔法技能 | 1010: Special_JianJieShiFa_238; -1001: SetBuffSettingSkillExpectation |
| 冰冷刺骨 | MainPackage_Skill_239_BingLengCiGu | 2 | 2 | 1 | 4 | 0:0/0/0/0 | P01=0.25 | P01=0.25×L | 攻击后：目标每有1层{Buff_MainPackage_HanShuang_8_Title}，便对目标造成[P01]点真实伤害 | 1007: Special_BingLengCiGu_239 |
| 毒之血 | MainPackage_Skill_23_DuXue | 1 | 2 | 1 | 10 | 0:0/0/0/0 | ConstValue=1 | ConstValue=2×L | 受到伤害后：为一个随机敌人附加[ConstValue]层{Buff_MainPackage_ZhongDu_3_Title} | 1013: Special_DuXue |
| 极寒凝聚 | MainPackage_Skill_240_JiHanNingJu | 1 | 1 | 0 | 10 | 0:5/1/1/0 | P01=0.5 | P01=0.5×L | 自身每有1层{Buff_MainPackage_HanShuang_8_Title}，获得[P01]点护甲 | 0: Special_BingLengCiGu_240; -1001: Special_BingLengCiGu_240 |
| 暴风雪 | MainPackage_Skill_241_BaoFengXue | 2 | 1 | 0 | 4 | 0:5/0/4/1 | P01=1 | P01=0.5×L | 对所有敌人造成所有单位携带{Buff_MainPackage_HanShuang_8_Title}总层数[P01]%的真实伤害 | 0: Special_BaoFengXue_241; -1001: Special_BaoFengXue_241 |
| 炼狱 | MainPackage_Skill_242_LianYu | 2 | 1 | 0 | 4 | 0:5/1/3/1 | HealthPrecent=0.15 | HealthPrecent=0.05×L | 为所有敌方单位附加自身最大生命值[HealthPrecent]%([HealthPrecent*1001])的{Buff_MainPackage_ShaoShang_7_Title} | 0: Special_LianYu_242; -1001: Special_LianYu_242 |
| 炎魔化 | MainPackage_Skill_243_YanMoHua | 2 | 2 | 1 | 4 | 0:0/0/0/0 | P01=-2 | P01=2×L | 自身携带的{Buff_MainPackage_ShaoShang_7_Title}不再减少最大生命值上限，同时每有一层{Buff_MainPackage_ShaoShang_7_Title}提升[P01]点最大生命值;战斗结束后移除所有{Buff_MainPackage_ShaoShang_7_Title}层数 | 1001: AddSpecialTag,UpdateBuffState; 1002: RemoveSpecialTag,UpdateBuffState; 1009: RemoveSelfTargetAllBuff |
| 聚能光束 | MainPackage_Skill_244_JuNengGuangShu | 0 | 1 | 0 | 15 | 0:0/1/2/1 | P01=0.15; P02=1 | P01=0×L; P02=0.5×L | 消耗最大魔力的[P01]%，造成[P02]%消耗的魔力值的伤害 | 0: Special_JuNengGuangShu_244; -1001: Special_JuNengGuangShu_244 |
| 超载 | MainPackage_Skill_245_ChaoZai | 1 | 4 | 0 | 1 | 0:5/0/2/1 | P01=12 | P01=-1×L | 使最左边的未就绪的魔法技能冷却+[P01]，获得该技能行动力消耗的行动力 | 0: Special_ChaoZai_245; -1001: SetBuffSettingSkillExpectation |
| 商店会员 | MainPackage_Skill_246_ShangDianHuiYuan | 0 | 2 | 1 | 10 | 0:0/0/0/0 | P01=1 | P01=1×L | 商店的前[P01]次刷新免费 | 1001: SetTargetAttributeValue; 1002: SetTargetAttributeValue |
| 魔力过剩 | MainPackage_Skill_247_MoLiGuoSheng | 1 | 2 | 1 | 10 | 0:0/0/0/0 | P01=0.25 | P01=0.25×L | 使用技能后:对一个随机单位造成该技能[P01]%魔法消耗的伤害 | 1014: Special_MoLiGuoSheng_245 |
| 重火力 | MainPackage_Skill_248_ZhongHuoLi | 1 | 2 | 1 | 10 | 0:0/0/0/0 | P01=3; P02=3 | P01=4×L; P02=0×L | 战斗开始时：获得[P01]层{Buff_MainPackage_Vitality_4_Title}和[P02]层{Buff_MainPackage_1015_ChenZhong_Title} | 1008: SelfAddBuff,SelfAddBuff |
| 二连击 | MainPackage_Skill_249_ErLianJi | 1 | 4 | 0 | 10 | 0:0/1/3/1 | P01=3 | P01=3×L | 造成[P01]点伤害2次 | 0: DamageTargetConst; -1001: SetExpectionDamage,SetDamageCountExpectation |
| 穿透术 | MainPackage_Skill_24_ChuanTouShu | 1 | 2 | 1 | 10 | 0:0/0/0/0 | ConstValue=1 | ConstValue=2×L | 攻击造成伤害后： 对目标造成[ConstValue]点{Buff_MainPackage_1006_ZhenShiShangHai_Title} | 1007: DamageTargetConstOnRealDamage |
| 冰冷躯体 | MainPackage_Skill_250_BingLengQuTi | 1 | 2 | 1 | 10 | 0:0/0/0/0 | P01=1 | P01=2×L | 受到攻击后：为伤害来源附加[P01]层{Buff_MainPackage_HanShuang_8_Title} | 1013: AddBuffToTargetAfterAttacked |
| 低温静置 | MainPackage_Skill_251_DiWenJingZhi | 2 | 1 | 0 | 4 | 0:15/2/5/0 | P01=20; P02=0.5 | P01=15×L; P02=0.5×L | 获得[P01]点护甲，并对所有单位附加[P02]%([P02*2103])智力的{Buff_MainPackage_HanShuang_8_Title} | 0: GainArmor,AddBuffToAllUnitByAttribute; -1001: ArmorSkillExpectation,SetBuffSettingSkillExpectation |
| 烟雾弥漫 | MainPackage_Skill_252_YanWuMiMan | 1 | 1 | 0 | 10 | 0:2/1/3/0 | P01=0.8 | P01=0.2×L | 获得全部单位{Buff_MainPackage_ZhongDu_3_Title}总和[P01]%的护甲 | 0: Special_YanWuMiMan_252; -1001: Special_YanWuMiMan_252 |
| 虚假治疗 | MainPackage_Skill_253_XuJiaZhiLiao | 1 | 1 | 0 | 10 | 0:4/0/2/0 | P01=4; P02=1 | P01=2×L; P02=1×L | 恢复[P01]点生命值，自身获得[P02]层{Buff_MainPackage_ZhongDu_3_Title} | 0: RecoverConstValue,SelfAddBuff; -1001: SetHealthRecover,SetBuffSettingSkillExpectation |
| 躯体改造 | MainPackage_Skill_254_QuTiGaiZao | 1 | 1 | 0 | 1 | 0:8/0/1/2 | P01=4; P02=0.5 | P01=0×L; P02=0.5×L | 最大永久生命值+[P01]，最大永久魔力值-[P02] | 0: ChangeTargertAttribute,ChangeTargertAttribute |
| 键脑添加物 | MainPackage_Skill_255_JianNaoTianJiaWu | 1 | 2 | 1 | 10 | 0:0/0/0/0 | P01=1 | P01=1×L | 战斗中每回合一次：在使用药水后，随机一个未就绪的技能冷却-[P01] | 1102: Special_JianNaoTianJiaWu; 1009: ResetTagValue; 1010: ResetTagValue |
| 烟雾弹 | MainPackage_Skill_255_QuTiGaiZao | 1 | 2 | 0 | 12 | 0:5/2/4/1 | P01=6; P02=1 | P01=3×L; P02=0×L | 获得[P01]点护甲，并对所有敌人附加[P02]层虚弱 |  |
| 毒气迸发 | MainPackage_Skill_256_DuQiBengFa | 0 | 1 | 0 | 15 | 0:0/0/2/1 | P01=0.5 | P01=0.5×L | 对所有敌方单位造成自身[P01]%{Buff_MainPackage_ZhongDu_3_Title}层数的伤害 | 0: DamageTargetByTargetBuff; -1001: SetDamageBySelfBuffLevel |
| 硬化 | MainPackage_Skill_257_YingHua | 0 | 2 | 1 | 15 | 0:0/0/0/0 | P01=1; P02=0; P03=10 | P01=0×L; P02=2×L; P03=0×L | 回合开始时：受到[P01]点真实伤害，获得[P02]点护甲，每场战斗限[P03]次（还剩<Counter>次） | 1010: Special_YingHua_257; 1008: ResetTagValue |
| 毒性催化 | MainPackage_Skill_258_DuXingCuiHua | 2 | 1 | 0 | 4 | 0:5/1/5/1 | P01=0.25 | P01=0.25×L | 使一个敌人获得敌人自身{Buff_MainPackage_ZhongDu_3_Title}层数[P01]%的{Buff_MainPackage_ZhongDu_3_Title} | 0: ChangeTargetBuffByPrecent; -1001: ChangeTargetBuffByPrecent |
| 毒雾 | MainPackage_Skill_259_DuWu | 0 | 1 | 0 | 15 | 0:5/2/3/1 | P01=1; P02=1 | P01=2×L; P02=0×L | 使所有敌人获得[P01]层{Buff_MainPackage_ZhongDu_3_Title}，每使用一个药水，下次使用该技能的行动力消耗-[P02] | 0: AddBuffToAllEnemy,RemoveTargetSkillActionChange; -1001: SetBuffSettingSkillExpectation; 1102: ChangeTargetSkillActionCost |
| 审判击 | MainPackage_Skill_25_ShenPanJi | 0 | 2 | 1 | 10 | 0:0/0/0/0 | DamagePrecent=0.6 | DamagePrecent=0.4×L | 战斗开始时:对所有敌人造成[DamagePrecent]%([DamagePrecent*2101])力量的伤害 | 1008: SpawnEffectToTarget_ToAllUnit,DamageAllByAttributeInfo,PlayAudio,PlayAudio |
| 淬毒武器 | MainPackage_Skill_260_CuiDuWuQi | 1 | 2 | 1 | 6 | 0:0/0/0/0 | P01=0 | P01=1×L | 武器攻击后：为目标附加[P01]层{Buff_MainPackage_ZhongDu_3_Title} | 1007: Special_CuiDuWuQi_153 |
| 淬毒刺击 | MainPackage_Skill_261_CuiDuCiJi | 0 | 4 | 0 | 15 | 0:1/1/3/1 | P01=2; P02=0 | P01=2×L; P02=2×L | 造成[P01]点伤害，附加[P02]层{Buff_MainPackage_ZhongDu_3_Title} | 0: DamageTargetConst,AddBuffToTarget; -1001: SetExpectionDamage,SetBuffSettingSkillExpectation |
| 躯体激素 | MainPackage_Skill_262_QuTiJiSu | 2 | 2 | 0 | 2 | 0:5/0/15/0 | P01=2; P02=2 | P01=2×L; P02=0×L | 最大生命值永久+[P01]，获得[P02]层{Buff_MainPackage_ZhongDu_3_Title} | 0: ChangeTargertAttribute,SelfAddBuff; -1001: SetNothingToDoSkillExp |
| 萃取 | MainPackage_Skill_263_CuiQu | 1 | 2 | 0 | 6 | 0:0/1/2/0 | P01=3 | P01=2×L | 移除自身至多[P01]层中毒，如果你有至少1个虚无药品，获得一个随机1级药剂 | 0: Special_CuiQu_263,RemoveSelfTargetAllBuff; -1001: SetBuffSettingSkillExpectation |
| 炼药准备 | MainPackage_Skill_264_LianYaoZhunBei | 1 | 2 | 1 | 10 | 0:0/0/0/0 | P01=1 | P01=1×L | 战斗开始时：获得[P01]个{Buff_MainPackage_1003_KongYaoPing_Title} | 1008: GetTargetPoition |
| 硬化自身 | MainPackage_Skill_265_YingHuaZiShen | 2 | 2 | 0 | 1 | 0:10/0/1/2 | P01=1; P02=2 | P01=0×L; P02=0×L | 永久提升[P01]点防御力，最大生命值-[P02] | 0: ChangeTargertAttribute,ChangeTargertAttribute; -1001: SetNothingToDoSkillExp |
| 阵痛剂 | MainPackage_Skill_266_ZhenTongJi | 1 | 1 | 0 | 10 | 0:2/1/4/1 | P01=2; P02=5; P03=1 | P01=0×L; P02=5×L; P03=0×L | 获得[P01]层{Buff_MainPackage_ZhongDu_3_Title}，并获得[P02]点护甲和[P03]层{Buff_MainPackage_BiLei_10_Title} | 0: SelfAddBuff,GainArmor,SelfAddBuff; -1001: SetBuffSettingSkillExpectation,SetBuffSettingSkillExpectation,ArmorSkillExpectation |
| 致命吐息 | MainPackage_Skill_267_ZhiMingTuXi | 2 | 1 | 0 | 4 | 0:3/2/4/0 | P01=0.5 | P01=0.5×L | 对所有敌人附加[P01]%([P01*2103])智力层数的{Buff_MainPackage_ZhongDu_3_Title} | 0: AddBuffToAllUnitByAttribute; -1001: SetBuffSettingSkillExpectationByAttribute |
| 炼金残留 | MainPackage_Skill_268_LianJinCanLiu | 1 | 2 | 1 | 6 | 0:0/0/0/0 | P01=1 | P01=1×L | 使用药水后：如果不是{Buff_MainPackage_1003_KongYaoPing_Title}，获得[P01]金币 | 1102: Special_LianJinCanLiu_268 |
| 毒性耐受 | MainPackage_Skill_269_DuXingNaiShou | 0 | 2 | 1 | 15 | 0:0/0/0/0 | P01=1.2 | P01=0.2×L | 回合开始时：获得当前{Buff_MainPackage_ZhongDu_3_Title}层数[P01]%的护甲 | 1010: SelfAddBuffBySelfBuff |
| 兴奋添加物 | MainPackage_Skill_270_XingFenTianJiaWu | 2 | 2 | 1 | 4 | 0:0/0/0/0 | P01=1 | P01=1×L | 使用药水后：获得[P01]点{Buff_MainPackage_Vitality_4_Title}<br>回合结束时：本回合每攻击一次就会移除一层{Buff_MainPackage_Vitality_4_Title} | 1102: SelfAddBuff; 1007: SelfAddBuff |
| 毒爆术 | MainPackage_Skill_271_DuBaoShu | 2 | 2 | 1 | 4 | 0:0/0/0/0 | P01=0.75 | P01=0.25×L | 回合开始时：所有敌人的身上每有1层{Buff_MainPackage_ZhongDu_3_Title}，造成层数[P01]%的{Buff_MainPackage_1006_ZhenShiShangHai_Title} | 1010: Special_DuBaoShu_271 |
| 炼金造物 | MainPackage_Skill_272_LianJinZaoWu | 2 | 2 | 1 | 4 | 0:0/0/0/0 | P01=2; P02=2 | P01=2×L; P02=0×L | 战斗开始时：召唤一个的淤泥怪<br>使用药水后：淤泥怪的最大生命值+[P01]，并使其获得[P02]层{Buff_MainPackage_Vitality_4_Title} | 1008: SummerTargetUnit; 1102: Special_LianJinZaoWu_272 |
| 召唤火元素 | MainPackage_Skill_273_ZhaoHuanHuoYuanSu | 1 | 2 | 1 | 10 | 0:0/0/0/0 | P01=0.75; P02=0.85; SkillLevel=0 | P01=0.25×L; P02=0.15×L; SkillLevel=1×L | 战斗开始时：召唤一个火元素，火元素拥有[SkillLevel]级火焰附魔<br>火元素额外获得[P01]%([P01*2101])力量的生命值，额外获得[P02]%([P02*2103])智力的{Buff_MainPackage_Vitality_4_Title} | 1008: CacheTargetSkillData,CacheTargetBuffData,SummonUnit_ApplyCacheData,ChangeSummonUnitAttribute |
| 召唤冰墙 | MainPackage_Skill_274_ZhaoHuanBingQiang | 1 | 1 | 0 | 10 | 0:5/1/3/0 | P01=6; SkillLevel=0 | P01=4×L; SkillLevel=1×L | 使所有友方单位血量+[P01]和1层{Buff_MainPackage_ChaoFeng_16_Title}<br>战斗开始时：召唤一个冰墙，冰墙拥有[SkillLevel]级冰冷躯体 | 0: ChangeAllSummonUnitAttribute; 1008: CacheTargetSkillData,SummonUnit_ApplyCacheData |
| 兴奋冲击 | MainPackage_Skill_275_XingFenChongJi | 0 | 2 | 0 | 15 | 0:0/2/2/0 | P01=6 | P01=4×L | 造成[P01]点伤害，本回合每使用一次药水，该技能的行动力消耗-1 | 0: DamageTargetConst; -1001: SetExpectionDamage; 1102: ChangeTargetSkillActionCost; 1010: RemoveTargetSkillActionChange |
| 爆破实验 | MainPackage_Skill_276_BaoPoShiYan | 2 | 1 | 0 | 4 | 0:5/2/5/0 | P01=1 | P01=1×L | 将最多[P01]瓶虚无药水转化为劣质爆破药水<br>爆破药水会对全部敌人生效 | 0: Special_BaoPoShiYan_141; -1001: SetUnknowSkillExp; 1001: AddSpecialTag; 1002: RemoveSpecialTag |
| 毒素转移 | MainPackage_Skill_277_DuSuZhuanYi | 1 | 2 | 1 | 10 | 0:0/0/0/0 | P01=2 | P01=1×L | 武器攻击后：将自身最多[P01]层{Buff_MainPackage_ZhongDu_3_Title}转移至目标 | 1007: Special_DuSuZhuanYi_277 |
| 巩固 | MainPackage_Skill_278_GongGu | 2 | 4 | 0 | 4 | 0:5/1/5/1 | P01=0.5 | P01=0.5×L | 使下回合获得当前[P01]%的护甲 | 0: GainArmorByCurrentArmor_NextTurn; -1001: GainArmorByCurrentArmor_NextTurn |
| 侧身格挡 | MainPackage_Skill_279_CeShenGeDang | 0 | 4 | 0 | 15 | 0:1/0/0/1 | P01=4; P02=2 | P01=3×L; P02=0×L | 获得[P01]点护甲，最右侧的未就绪的主动技能冷却+[P02] | 0: GainArmor,TargetRightestSkillGetCooldown; -1001: ArmorSkillExpectation,SetBuffSettingSkillExpectation |
| 德鲁伊 | MainPackage_Skill_27_DeLuYi | 1 | 2 | 1 | 3 | 0:0/0/0/0 | P01=0.1 | P01=0.2×L | 营地和魔泉的回复量+[P01]% | 1001: AddSpecialTag; 1002: RemoveSpecialTag |
| 蓄势 | MainPackage_Skill_280_XuShi | 1 | 4 | 0 | 3 | 0:1/1/3/1 | P01=3; P02=0; P03=0 | P01=1×L; P02=1×L; P03=1×L | 获得[P01]点护甲，[P02]层{Buff_MainPackage_FanJi_5_Title}，下回合获得[P03]点额外行动力 | 0: GainArmor,SelfAddBuff,GainActionNextTurn; -1001: ArmorSkillExpectation,SetBuffSettingSkillExpectation |
| 鲜血之盾 | MainPackage_Skill_281_XianXueZhiDun | 1 | 2 | 0 | 10 | 0:0/2/3/1 | P01=7; P02=2 | P01=7×L; P02=0×L | 获得[P01]点护甲，并对自身造成[P02]点{Buff_MainPackage_1006_ZhenShiShangHai_Title} | 0: GainArmor,DamageSelfByReal; -1001: ArmorSkillExpectation,SetToSelfDamage |
| 浇灌 | MainPackage_Skill_282_JiaoGuan | 1 | 2 | 0 | 10 | 0:0/4/3/1 | P01=4 | P01=2×L | 获得[P01]层{Buff_MainPackage_JingJi_2_Title}。每次受到生命值伤害时，此技能行动力消耗-1，使用后重制 | 0: SelfAddBuff,RemoveTargetSkillActionChange; 1013: Special_XueChang_202 |
| 狂怒反击 | MainPackage_Skill_283_KuangNvFanJi | 2 | 4 | 0 | 4 | 0:0/2/6/1 | P01=1 | P01=1×L | 移除最多[P01]层反击，每移除1层，对目标使用一次武器的攻击效果 | 0: Special_JiaoGuan_283; -1001: Special_JiaoGuan_283 |
| 荆棘之墙 | MainPackage_Skill_284_JingJiZhiQiang | 2 | 1 | 0 | 4 | 0:5/0/7/1 | P01=3; P02=10; P03=0.75 | P01=0×L; P02=10×L; P03=0.25×L | 受到[P01]点真实伤害，召唤一个荆棘之墙（额外获得[P02]点生命值，获得等同于施法者拥有{Buff_MainPackage_JingJi_2_Title}层数[P03]%的{Buff_MainPackage_JingJi_2_Title}） | 0: CacheTargetBuffData,CacheTargetAttributeData,CacheTargetBuffData,SummonUnit_ApplyCacheData,DamageSelfByReal; -1001: SetUnknowSkillExp |
| 复仇荆棘 | MainPackage_Skill_285_FuChouJingJi | 1 | 2 | 1 | 10 | 0:0/0/0/1 | P01=0.8 | P01=0.2×L | 受到生命值伤害时：对一个随机敌人造成{Buff_MainPackage_JingJi_2_Title}层数[P01]%的伤害 | 1013: Special_FuChouJingJi_285 |
| 重装登场 | MainPackage_Skill_286_ZhongZhuangDengChang | 0 | 2 | 1 | 15 | 0:0/0/0/1 | P01=5; P02=1 | P01=3×L; P02=0×L | 战斗开始时：获得[P01]点护甲，并获得[P02]层{Buff_MainPackage_BiLei_10_Title} | 1008: GainArmorNextTurn,SelfAddBuff |
| 无惧疼痛 | MainPackage_Skill_287_WuJuTengTong | 1 | 4 | 0 | 10 | 0:2/4/2/1 | P01=4 | P01=4×L | 获得[P01]点护甲，每次受到生命值伤害时，此技能的行动力消耗-1，战斗开始时重置 | 0: GainArmor; -1001: GainArmor; 1013: Special_XueChang_202; 1008: RemoveTargetSkillActionChange |
| 复仇扫荡 | MainPackage_Skill_288_FuChouSaoDang | 0 | 2 | 1 | 15 | 0:0/0/0/1 | P01=4; P02=0.75 | P01=0×L; P02=0.25×L | 每受到[P01]次伤害后：对所有敌人造成[P02]%([P02*2101])力量的伤害（受伤次数：<Counter>） | 1013: Special_FuChouJingJi_288; 1001: RegistOrLogOffSkillTag |
| 尖刺护盾 | MainPackage_Skill_289_JianCiHuDun | 0 | 2 | 1 | 15 | 0:0/0/0/1 | P01=0.6 | P01=0.4×L | 被攻击时：如果没有受到生命值伤害，则对伤害来源造成[P01]%([P01*2002])防御力的伤害 | 1012: Special_JianCiHuDun_289 |
| 伤痛感知 | MainPackage_Skill_28_ShangTongGanZhi | 2 | 2 | 1 | 4 | 0:0/0/0/0 | P01=0 | P01=1×L | 受到伤害后：如果受到了生命值伤害，获得[P01]层{Buff_MainPackage_Vitality_4_Title} | 1013: Special_ShangTongGanZhi_28 |
| 疼痛刺激 | MainPackage_Skill_290_TengTongCiJi | 2 | 2 | 1 | 4 | 0:0/0/0/1 | P01=0 | P01=1×L | 每次受到生命值伤害时，使一个随机的技能冷却-[P01] | 1013: Special_TengTongCiJi_290 |
| 顶盾 | MainPackage_Skill_291_DingDun | 0 | 4 | 0 | 15 | 0:1/1/2/1 | P01=0.8 | P01=0.2×L | 获得[P01]%([P01*2101])力量的护甲 | 0: GainArmorByRefAtrribute; -1001: ArmorSkillExpectation |
| 挑衅 | MainPackage_Skill_292_TiaoXing | 0 | 4 | 0 | 15 | 0:0/0/1/1 | P01=5 | P01=3×L | 获得[P01]点护甲，强制目标对你进行行动 | 0: GainArmor,ExcuteTargetUnitSkill; -1001: ArmorSkillExpectation,SetUnknowSkillExp |
| 目中无人 | MainPackage_Skill_293_MuZhongWuRen | 2 | 4 | 0 | 4 | 0:1/2/3/1 | P01=10; P02=2 | P01=10×L; P02=0×L | 每有一个敌人，获得[P01]点护甲，使所有敌人获得[P02]层{Buff_MainPackage_XuRuo_12_Title}，并强制所有敌人对你进行行动 | 0: GainArmorByEnemyCount,AddBuffToAllEnemy,ExcuteAllEnemyUnitSkill; -1001: GainArmorByEnemyCount,SetBuffSettingSkillExpectation,SetUnknowSkillExp |
| 死命抵抗 | MainPackage_Skill_294_SiMingDiKang | 2 | 4 | 0 | 4 | 0:1/2/3/1 | P01=10; P02=2 | P01=5×L; P02=0×L | 获得[P01]点护甲，同时对自己造成[P02]点真实伤害1次<br>每有一个冷却中的其他技能，重复1次 | 0: GetArmorByHaveCoolDownSkill,DamageSelfByCooldownSkillCount; -1001: GetArmorByHaveCoolDownSkill,DamageSelfByCooldownSkillCount |
| 举盾攻击 | MainPackage_Skill_295_JuDunGongJi | 1 | 4 | 0 | 10 | 0:1/1/1/1 | P01=3 | P01=3×L | 获得[P01]点护甲，对目标使用一次武器攻击 | 0: GainArmor,ExcuteNormalAttack; -1001: ArmorSkillExpectation,SetUnknowSkillExp |
| 突围 | MainPackage_Skill_296_TuWei | 2 | 4 | 0 | 4 | 0:0/1/4/1 | P01=1; P02=15 | P01=0.5×L; P02=0×L | 对所有敌人造成[P01]%([P01*2101])力量的伤害，如果至少有[P02]点护甲，额外造成一次伤害 | 0: Special_TuWei_296; -1001: Special_TuWei_296; -1003: SelfArmorIsSatisfy |
| 跟进 | MainPackage_Skill_297_GenJin | 1 | 4 | 0 | 10 | 0:0/0/0/1 | P01=2; P02=3 | P01=1×L; P02=3×L | 移除[P01]点护甲，造成[P02]点伤害 | 0: Special_GenJin_297; -1001: SetExpectionDamage; -1002: SelfArmorIsSatisfy |
| 背水一战 | MainPackage_Skill_298_BeiShuiYiZhan | 2 | 4 | 0 | 4 | 0:0/1/4/1 | P01=5 | P01=5×L | 获得[P01]层{Buff_MainPackage_Vitality_4_Title}，获得[P01]层{Buff_MainPackage_QingShi_11_Title} | 0: SelfAddBuff,SelfAddBuff; -1001: SetBuffSettingSkillExpectation,SetBuffSettingSkillExpectation |
| 预备反击 | MainPackage_Skill_299_YuBeiFanJi | 1 | 4 | 0 | 10 | 0:0/1/1/1 | P01=0; P02=2 | P01=4×L; P02=0×L | 造成[P01]点伤害，如果拥有[P02]层{Buff_MainPackage_FanJi_5_Title}，移除对应层数，并使用武器攻击一次 | 0: DamageTargetConst,Special_YuBeiFanJi_299; -1001: SetExpectionDamage,Special_YuBeiFanJi_299; -1003: SelfUnitHaveTargetBuffInfo |
| 挥盾切割 | MainPackage_Skill_29_HuiDunQieGe | 1 | 4 | 0 | 10 | 0:1/2/3/1 | P01=1.5; P02=5; P03=1 | P01=0.5×L; P02=0×L; P03=0×L | 获得[P01]%([P01*2002])防御力的护甲，并为目标附加[P03]层{Buff_MainPackage_CuiRuo_1_Title}(每有[P02]点力量额外附加1层) | 0: AddBuffToTarget,GainArmorByRefAtrribute,AddBuffToTargetByAttribute; -1001: ArmorSkillExpectation,SetBuffSettingSkillExpectationByAttribute |
| 敏捷提升 | MainPackage_Skill_2_MinJieTiSheng | 0 | 2 | 1 | 20 | 0:0/0/0/1 | P01=0 | P01=2×L | 敏捷+[P01] | 1001: SetTargetAttributeValue; 1002: SetTargetAttributeValue |
| 决斗 | MainPackage_Skill_300_JueDou | 2 | 4 | 0 | 4 | 0:0/0/4/1 | P01=1 | P01=1×L | 对目标使用一次武器攻击，同时目标对你普通攻击（重复[P01]次） | 0: Special_JueDou_300; -1001: SetUnknowSkillExp |
| 游刃有余 | MainPackage_Skill_301_YouRenYouYu | 1 | 2 | 1 | 10 | 0:0/0/0/1 | P01=2; P02=0 | P01=0×L; P02=1×L | 反击[P01]次后：使一个随机冷却中的技能冷却-[P02] | 1101: Special_YouRenYouYu_301; 1008: RegistOrLogOffSkillTag |
| 穷追猛打 | MainPackage_Skill_302_QiongZhuiMengDa | 2 | 2 | 1 | 2 | 0:0/0/0/1 | P01=7 | P01=-1×L | 使用武器[P01]次后：额外使用武器攻击一次（已使用次数：<Counter>） | 1014: Special_QiongZhuiMengDa_302; 1008: RegistOrLogOffSkillTag |
| 复仇打击 | MainPackage_Skill_303_FuChouDaJi | 2 | 4 | 0 | 4 | 0:0/2/3/1 | P01=12; P02=3 | P01=0×L; P02=3×L | 对所有敌人造成[P01]点伤害，本场战斗每次受到生命值伤害时，伤害+[P02](受伤次数<Counter>) | 0: Special_FuChouDaJi_303; -1001: Special_FuChouDaJi_303; 1008: ResetTagValue; 1013: ChangeTagValueOnTakeHealthDamage |
| 战吼 | MainPackage_Skill_304_ZhanHou | 2 | 2 | 1 | 1 | 0:0/0/0/1 |  |  | 战斗开始时的效果会触发两次 | 1001: AddSpecialTag; 1002: RemoveSpecialTag |
| 闪亮登场 | MainPackage_Skill_305_ShanLiangDengChang | 0 | 2 | 1 | 15 | 0:0/0/0/1 | P01=2 | P01=6×L | 战斗开始时：对所有敌人造成[P01]点伤害 | 1008: DamageByAllEnemyConstValue |
| 旺盛 | MainPackage_Skill_306_WangSheng | 2 | 2 | 1 | 4 | 0:0/0/0/1 | P01=0 | P01=1×L | 每场战斗前[P01]个回合，行动力+1（还剩回合数：<Counter>） | 1008: ResetTagValue; 1010: Special_WangSheng_306; 1009: ResetTagValue |
| 快速冷却 | MainPackage_Skill_307_KuaiSuLengQue | 0 | 4 | 0 | 10 | 0:2/1/4/1 | P01=1 | P01=2×L | 使一个随机的技能冷却-[P01] | 0: ReduceCooldownRandomColldownSkill; -1001: SetBuffSettingSkillExpectation |
| 后空翻 | MainPackage_Skill_308_HouKongFan | 1 | 4 | 0 | 10 | 0:2/0/0/1 | P01=2 | P01=2×L | 获得[P01]点护甲，每次使用该技能行动力+2，每次使用其他武技时该技能行动力-1 | 0: GainArmor,ChangeTargetSkillActionCost; -1001: ArmorSkillExpectation; 1014: Special_WangSheng_308 |
| 激活 | MainPackage_Skill_309_JiHuo | 1 | 4 | 0 | 3 | 0:2/0/10/1 | P01=0 | P01=1×L | 获得[P01]点行动力<br>战斗开始时：该技能冷却-10 | 0: GainActionPoint; 1008: ChangeCurSkillCooldown |
| 精力旺盛 | MainPackage_Skill_30_JingLiWangSheng | 2 | 4 | 0 | 4 | 0:5/1/4/1 | P01=0 | P01=1×L | （主动使用时不会有效果）<br>如果该技能处于冷却中：回合开始时，获得[P01]层{Buff_MainPackage_Vitality_4_Title} | 1010: Special_JingLiWangSheng_30; -1001: SetBuffSettingSkillExpectation |
| 流光术 | MainPackage_Skill_30_LiuGuangShu | 0 | 1 | 0 | 10 | 0:3/0/3/1 | ConstValue=4 | ConstValue=3×L | 造成[ConstValue]点真实伤害 | 0: DamageTargetConstOnRealDamage; -1001: SetToRealDamage |
| 集中攻击 | MainPackage_Skill_310_JiZhongGongJi | 2 | 4 | 0 | 4 | 0:0/1/5/1 | P01=1 | P01=2×L | 本场战斗，每使用1次武器或技能便可造成[P01]点伤害（已使用技能数：<Counter>） | 0: DamageTargetBySkillCounter; -1001: DamageTargetBySkillCounter; 1009: ResetTagValue; 1008: ResetTagValue; 1014: RegistOrLogOffListenTag |
| 攻击技巧 | MainPackage_Skill_311_GongJiJiQiao | 2 | 4 | 1 | 4 | 0:0/0/0/1 | P01=8 | P01=-1×L | 每攻击[P01]次：获得1点行动力（已攻击次数：<Counter>） | 1007: RegistOrLogOffListenTag,GainActionPoint_OnTargetCounterSatisfy; 1008: ResetTagValue; 1009: ResetTagValue |
| 搏斗 | MainPackage_Skill_312_BoDou | 1 | 4 | 0 | 10 | 0:1/1/2/1 | P01=6; P02=1 | P01=3×L; P02=0×L | 造成[P01]点伤害，该技能每造成1点伤害便获得[P02]点护甲 | 0: DamageTargetConst; -1001: SetExpectionDamage; 1005: Special_BoDou_312 |
| 搏命 | MainPackage_Skill_313_BoMing | 2 | 4 | 1 | 4 | 0:0/0/0/1 | P01=1; P02=3 | P01=0×L; P02=3×L | 攻击后：受到[P01]点真实伤害，额外对目标造成[P02]点伤害 | 1007: DamageTargetConst,DamageSelfByReal |
| 迅捷之影 | MainPackage_Skill_314_XunJieZhiYing | 2 | 4 | 1 | 1 | 0:0/0/0/1 | P01=1 | P01=0×L | 使用行动消耗为0的技能时，该技能效果会额外触发1次 | 1019: Special_BoDou_314 |
| 电能注入 | MainPackage_Skill_315_DianNengZhuRu | 0 | 1 | 0 | 15 | 0:2/1/1/1 | P01=1 | P01=1×L | 为目标附加[P01]层{Buff_MainPackage_GanDian_18_Title} | 0: AddBuffToTarget; -1001: SetBuffSettingSkillExpectation |
| 过载电击 | MainPackage_Skill_316_GuoZaiDianJi | 0 | 1 | 0 | 15 | 0:2/0/2/1 | P01=5; P02=0 | P01=5×L; P02=1×L | 造成[P01]点伤害，并为自身附加[P02]层{Buff_MainPackage_GanDian_18_Title} | 0: CacheAudioName,DamageTargetConst,SelfAddBuff; -1001: SetExpectionDamage,SetBuffSettingSkillExpectation |
| 雷电奉还 | MainPackage_Skill_317_LeiDianFengHuan | 1 | 1 | 1 | 10 | 0:0/0/0/1 | P01=0 | P01=8×L | 受到来自{Buff_MainPackage_GanDian_18_Title}的伤害后：如果没有受到生命值伤害，对所有敌人造成[P01]点伤害 | 1013: CacheAudioName,Special_LeiDianFengHuan_314 |
| 余电 | MainPackage_Skill_318_YuDian | 1 | 1 | 1 | 6 | 0:0/0/0/1 | P01=0 | P01=1×L | 使用魔法后:为一个随机敌人附加[P01]层{Buff_MainPackage_GanDian_18_Title} | 1014: Special_YuDian_315 |
| 电能激活 | MainPackage_Skill_319_DianNengJiHuo | 1 | 1 | 0 | 10 | 0:1/0/2/1 | P01=2; P02=0 | P01=2×L; P02=1×L | 造成[P01]点伤害，再额外触发目标最多[P02]层{Buff_MainPackage_GanDian_18_Title} | 0: CacheAudioName,DamageTargetConst,TriggerTargetBuffTargetTrigger; -1001: SetExpectionDamage,SetBuffSettingSkillExpectation; -1003: TargetUnitHaveTargetBuffInfo |
| 融合击 | MainPackage_Skill_31_RongHeJi | 1 | 4 | 0 | 10 | 0:2/2/4/1 | DamagePrecent=0.7 | DamagePrecent=0.3×L | 造成[DamagePrecent]%([DamagePrecent*2101])力量+[DamagePrecent]%([DamagePrecent*2102])敏捷+[DamagePrecent]%([DamagePrecent*2103])智力的合计伤害 | 0: DamageTargetByMix; -1001: SetExpectionDamageByTargetAttribute,SetExpectionDamageByTargetAttribute,SetExpectionDamageByTargetAttribute |
| 雷电枪 | MainPackage_Skill_320_LeiDianQiang | 2 | 1 | 0 | 4 | 0:5/3/4/1 | P01=5 | P01=5×L | 移除目标所有{Buff_MainPackage_GanDian_18_Title}，每移除一层便对目标造成[P01]点伤害1次 | 0: CacheAudioName,Special_YuDian_316; -1001: Special_YuDian_316 |
| 唤雷仪式 | MainPackage_Skill_321_HuanLeiYiShi | 2 | 1 | 0 | 4 | 0:3/0/4/1 | P01=2; P02=2 | P01=3×L; P02=1×L | 获得[P01]层{Buff_MainPackage_Vitality_4_Title}和[P02]层{Buff_MainPackage_GanDian_18_Title} | 0: SelfAddBuff,SelfAddBuff; -1001: SetBuffSettingSkillExpectation,SetBuffSettingSkillExpectation |
| 电力迅捷 | MainPackage_Skill_322_DianLiXunJie | 1 | 1 | 1 | 6 | 0:0/0/0/1 | P01=0 | P01=1×L | 触发自身的{Buff_MainPackage_GanDian_18_Title}时，随机一个冷却中的技能冷却-[P01] | 1020: ReduceCooldownRandomColldownSkill |
| 迅雷攻击 | MainPackage_Skill_323_XunLeiGongJi | 2 | 1 | 0 | 4 | 0:3/1/4/1 | P01=0; P02=2 | P01=2×L; P02=0×L | 使用一次武器的攻击效果，同时移除自身最多[P01]层{Buff_MainPackage_GanDian_18_Title}，每移除[P02]层，便额外使用一次武器攻击效果 | 0: Special_XunLeiGongJi_323; -1001: Special_XunLeiGongJi_323; -1003: SelfUnitHaveTargetBuffInfo |
| 雷云 | MainPackage_Skill_324_LeiYun | 1 | 1 | 1 | 6 | 0:0/0/0/1 | P01=0 | P01=2×L | 战斗开始时：为所有敌人附加[P01]层{Buff_MainPackage_GanDian_18_Title} | 1008: AddBuffToAllEnemy |
| 连续轰击 | MainPackage_Skill_325_LianXuHongJi | 1 | 1 | 0 | 10 | 0:2/1/2/1 | P01=2; P02=3 | P01=1×L; P02=0×L | 造成[P01]点伤害[P02]次，随机分配到所有敌人身上；如果所有敌人都有{Buff_MainPackage_GanDian_18_Title}，则再对所有敌人造成[P01]点伤害 | 0: Special_LianXuHongJi_325; -1001: Special_LianXuHongJi_325; -1003: IsAllEnemyHaveTargetBuff |
| 漏电 | MainPackage_Skill_326_LouDian | 1 | 1 | 1 | 6 | 0:0/0/0/1 | P01=0 | P01=2×L | 受到伤害时：为一个随机敌人附加[P01]层{Buff_MainPackage_GanDian_18_Title} | 1013: AddBuffToRandomTarget |
| 奥术附魔 | MainPackage_Skill_327_AoShuFuMo | 1 | 1 | 1 | 10 | 0:0/0/0/1 | P01=0.1 | P01=0.1×L | 武器攻击后：对目标造成自身[P01]%([P01*2103])智力的伤害 | 1007: Special_AoShuFuMo_327 |
| 掠法连击 | MainPackage_Skill_328_LueFaLianJi | 2 | 1 | 1 | 4 | 0:0/0/0/1 | P01=0 | P01=1×L | 使用武器后：恢复[P01]点魔力 | 1014: Special_LueFaLianJi_328 |
| 召唤强力幻象 | MainPackage_Skill_329_ZhaoHuanQiangLiHuanXiang | 2 | 1 | 0 | 4 | 0:5/1/5/1 | P01=0 | P01=2×L | 召唤一个幻象，该幻象会继承所有被动技能（继承的被动等级不会超过[P01]级） | 0: CacheSkill_PasstiveSkill,SummonIllusionUnitSign; -1001: SetUnknowSkillExp |
| 魔力聚合 | MainPackage_Skill_330_MoLiJuHe | 2 | 1 | 0 | 4 | 0:10/1/3/1 | P01=0.4 | P01=0.2×L | 获得[P01]%([P01*2103])智力的{Buff_MainPackage_Vitality_4_Title} | 0: AddBuffToSelf_ByAttribute; -1001: AddBuffToSelf_ByAttribute |
| 聚能 | MainPackage_Skill_331_JuNeng | 0 | 1 | 0 | 10 | 0:5/0/2/1 | P01=0.03 | P01=0.02×L | 获得[P01]%([P01*1002])最大魔力值的临时{Buff_MainPackage_Vitality_4_Title} | 0: AddBuffToSelf_ByAttribute,AddBuffToSelf_ByAttribute; -1001: AddBuffToSelf_ByAttribute |
| 烈焰之墙 | MainPackage_Skill_332_LieYanZhiQiang | 2 | 1 | 0 | 4 | 0:5/2/0/1 | P01=5; P02=0; P03=0; Level=0 | P01=10×L; P02=10×L; P03=3×L; Level=1×L | 所有友方单位获得此效果:获得[P01]点护甲，受到攻击时，对所有敌人造成[P02]点伤害并附加[P03]层{Buff_MainPackage_ShaoShang_7_Title}；该效果持续至回合开始 | 0: AllTeamMemberGainArmor,AddBuffToAllTeam; -1001: ArmorSkillExpectation,SetBuffSettingSkillExpectation |
| 召唤幻影 | MainPackage_Skill_333_ZhaoHuanHuanYing | 0 | 1 | 0 | 10 | 0:5/2/3/1 | P01=0 | P01=10×L | 召唤一个幻象，该幻象额外获得[P01]点生命值 | 0: CacheTargetAttributeData,SummonIllusionUnitSign; -1001: SetUnknowSkillExp |
| 后撤幻影 | MainPackage_Skill_334_HouCheHuanYing | 1 | 1 | 0 | 6 | 0:3/2/2/1 | P01=0; P02=3 | P01=10×L; P02=0×L | 最右侧的其他友方单位获得[P01]点护甲和[P02]层嘲讽，如果你没有其他单位，则召唤一个幻象 | 0: Special_HouCheHuanYing_334; -1001: Special_HouCheHuanYing_334 |
| 替身 | MainPackage_Skill_335_TiShen | 2 | 1 | 1 | 4 | 0:0/0/0/1 | P01=0.5; P02=0; P03=2 | P01=0×L; P02=1×L; P03=0×L | 受到伤害时：如果当前血量小于最大血量的[P01]%，则召唤一个幻象，并为该幻象附加[P03]层{Buff_MainPackage_ChaoFeng_16_Title}，每场战斗最多触发[P02]次（剩余<Counter>次） | 1013: CacheTargetBuffData,Special_TiShen_335; 1008: ResetTagValue |
| 群起攻之 | MainPackage_Skill_336_QunQiGongZhi | 2 | 4 | 0 | 4 | 0:0/2/2/1 | P01=0 | P01=5×L | 使所有其他友方单位获得[P01]层{Buff_MainPackage_Vitality_4_Title}，并使自己和所有友方单位攻击目标一次 | 0: Special_QunQiGongZhi_336; -1001: Special_QunQiGongZhi_336 |
| 无形之影 | MainPackage_Skill_337_WuXingZhiYing | 1 | 1 | 1 | 6 | 0:0/0/0/1 | P01=0 | P01=5×L | 战斗开始时：召唤一个幻象，该幻象额外获得[P01]点生命值 | 1008: CacheTargetAttributeData,SummonIllusionUnitSign |
| 偷窃 | MainPackage_Skill_338_TouQie | 0 | 1 | 0 | 6 | 0:3/1/0/1 | P01=3 | P01=3×L | 偷取怪物身上的[P01]金币，每个怪物仅能偷取一次 | 0: Special_TouQie_338; 1008: AddBuffToAllEnemy; -1001: SetUnknowSkillExp |
| 贪婪者 | MainPackage_Skill_339_TanLanZhe | 0 | 1 | 1 | 15 | 0:0/0/0/1 | P01=3 | P01=3×L | 战斗结束时：额外获得[P01]金币 | 1009: GetCoin |
| 囤积者 | MainPackage_Skill_340_DunJiZhe | 1 | 1 | 1 | 6 | 0:0/0/0/1 | P01=0.15 | P01=0.15×L | 进入新区域时（不包括暗道和分岔路口）：获得当前金币[P01]%的金币 |  |
| 囤积者 | MainPackage_Skill_340_TunJiZhe | 1 | 1 | 1 | 6 | 0:0/0/0/0 | P01=0.15 | P01=0.15×L | 进入分岔路口时：获得当前金币[P01]%的金币 | 1021: Special_TunJiZhe_339 |
| 猛烈攻势 | MainPackage_Skill_341_MengLieGongShi | 2 | 1 | 1 | 4 | 0:0/0/0/0 | P01=0 | P01=1×L | 每个回合限[P01]次：你的武器会额外触发1次 | 1014: Special_MengLieGongShi_340; 1010: ResetTagValue |
| 魔能化剑 | MainPackage_Skill_342_MoNengHuaJian | 0 | 0 | 0 | 15 | 0:0/1/2/1 | P01=0.9 | P01=0.1×L | 造成[P01]%([P01*2103])智力的伤害，该技能视为武器攻击 | 0: DamageTarget; -1001: SetExpectionDamageByTargetAttribute |
| 魔能扫荡 | MainPackage_Skill_343_MoNengSaoDang | 1 | 0 | 0 | 10 | 0:0/1/3/1 | P01=0.8 | P01=0.2×L | 对所有敌人造成[P01]%([P01*2103])智力的伤害，该技能视为武器攻击 | 0: DamageAllByAttributeInfo; -1001: SetExpectionDamageByTargetAttribute |
| 贯穿魔刃 | MainPackage_Skill_344_GuanChuanMoRen | 2 | 1 | 0 | 4 | 0:3/2/3/1 | P01=0.5 | P01=0.5×L | 造成[P01]%([P01*2103])智力的{Buff_MainPackage_1006_ZhenShiShangHai_Title}；使用其他魔法后，该技能的行动力-1，使用后重制 | 0: DamageTarget,RemoveTargetSkillActionChange; -1001: SetExpectionDamageByTargetAttribute; 1014: Special_GuanChuanMoRen_344 |
| 引势 | MainPackage_Skill_345_YinShi | 2 | 1 | 0 | 1 | 0:5/1/4/1 | P01=0 | P01=0×L | 无消耗的使用该技能相邻的技能，技能会正常进入冷却 | 0: Special_YinShi_345; -1001: SetUnknowSkillExp |
| 咏唱壁垒 | MainPackage_Skill_346_YongChangBiLei | 1 | 2 | 1 | 10 | 0:0/0/0/1 | P01=0 | P01=2×L | 使用魔法后：获得[P01]点护甲 | 1014: Special_YongChangBiLei_346 |
| 佯攻 | MainPackage_Skill_347_YangGong | 0 | 4 | 0 | 10 | 0:0/1/3/1 | P01=2; P02=1 | P01=2×L; P02=1×L | 造成[P01]点伤害，如果该次伤害没有造成生命值伤害，为目标附加[P02]层{Buff_MainPackage_XuRuo_12_Title} | 0: DamageTargetConst; -1001: SetExpectionDamage; 1005: Special_YangGong_346 |
| 雷霆加速 | MainPackage_Skill_348_LeiTingJiaSu | 2 | 1 | 0 | 4 | 0:0/1/5/1 | P01=1; P02=1 | P01=2×L; P02=1×L | 使所有技能冷却-[P01]，获得[P02]层{Buff_MainPackage_GanDian_18_Title} | 0: Special_TouNaoFengBao_66,SelfAddBuff; -1001: SetBuffSettingSkillExpectation,SetBuffSettingSkillExpectation |
| 落雷 | MainPackage_Skill_349_LuoLei | 1 | 1 | 0 | 10 | 0:5/2/2/1 | P01=9; P02=5; P03=1.5 | P01=9×L; P02=0×L; P03=0×L | 造成[P01]点伤害，如果目标的{Buff_MainPackage_GanDian_18_Title}至少[P02]层，移除[P02]层使该次伤害提升[P03]% | 0: CacheAudioName,Special_YangGong_346; -1001: Special_YangGong_346; -1003: TargetUnitHaveTargetBuffInfo |
| 精准雷击 | MainPackage_Skill_350_JingZhunLeiJi | 1 | 1 | 1 | 10 | 0:0/0/0/0 | P01=2 | P01=2×L | 回合结束时：对一个随机敌人造成[P01]点伤害，之后再对有{Buff_MainPackage_GanDian_18_Title}敌人额外造成一次伤害 | 1011: Special_JingZhunLeiJi_350 |
| 涌动刺击 | MainPackage_Skill_351_YongDongCiJi | 1 | 4 | 0 | 10 | 0:1/2/1/1 | P01=6 | P01=6×L | 造成[P01]点伤害，并随机使用一个已就绪技能（该技能会正常进入冷却） | 0: DamageTargetConst,UseRandomSkill; -1001: SetExpectionDamage,SetUnknowSkillExp |
| 充能突刺 | MainPackage_Skill_352_ChongNengTuCi | 1 | 4 | 0 | 10 | 0:0/0/2/1 | P01=3; P02=2; P03=4; P04=1 | P01=3×L; P02=0×L; P03=0×L; P04=0×L | 消耗全部行动力，每消耗1点行动力便造成[P01]点伤害[P02]次<br>如果消耗的行动力至少[P03]，伤害次数+[P04] | 0: Special_ChongNengTuCi_352,SetActionToZero; -1001: Special_ChongNengTuCi_352 |
| 雷罚 | MainPackage_Skill_353_LeiFa | 0 | 1 | 0 | 15 | 0:4/3/5/1 | P01=20; P02=3 | P01=8×L; P02=0×L | 造成[P01]点伤害，如果该技能造成了击杀，重置该技能的冷却时间并获得[P02]点行动力 | 0: Special_LeiFa_353; -1001: SetExpectionDamage |
| 充能 | MainPackage_Skill_354_ChongNeng | 2 | 1 | 0 | 4 | 0:5/0/10/1 | P01=1; P02=1; P03=10 | P01=1×L; P02=0×L; P03=0×L | 获得[P01]点行动力，并使全部技能冷却-[P02]<br>战斗结束时：该技能冷却-[P03] | 0: CooldownTargetSkill,GainActionPoint; -1001: SetBuffSettingSkillExpectation; 1009: ChangeCurSkillCooldown |
| 静电场 | MainPackage_Skill_355_JingDianChang | 1 | 1 | 1 | 10 | 0:0/0/0/0 | P01=0.2 | P01=0.2×L | 使用魔法后：对一个随机敌人造成[P01]%([P01*2103])智力的伤害 | 1014: Special_JingDianChang_355 |
| 蓄势斩击 | MainPackage_Skill_356_XuShiZhanJi | 1 | 4 | 0 | 10 | 0:0/1/3/1 | P01=3 | P01=3×L | 本场战斗每使用过1次魔法，便造成[P01]点伤害 | 0: DamageTargetBySkillCounter; 1009: ResetTagValue; 1014: ChangeCounter_AfterUseMagic; -1001: DamageTargetBySkillCounter; 1008: ResetTagValue |
| 同调 | MainPackage_Skill_357_TongTiao | 2 | 1 | 1 | 4 | 0:0/0/0/0 | P01=0 | P01=1×L | 使用魔法后：使一个武技冷却-[P01] | 1014: Special_JingDianChang_356 |
| 虚弱电击 | MainPackage_Skill_358_XuRuoDianJi | 0 | 1 | 0 | 3 | 0:1/1/4/1 | P01=3; P02=0 | P01=3×L; P02=1×L | 造成[P01]点伤害，并附加[P02]层{Buff_MainPackage_XuRuo_12_Title} | 0: DamageTargetConst,AddBuffToTarget; -1001: SetExpectionDamage,SetBuffSettingSkillExpectation |
| 终结一击 | MainPackage_Skill_359_ZhongJieYiJi | 2 | 4 | 0 | 4 | 0:0/2/4/1 | P01=3 | P01=3×L | 造成[P01]点伤害，目标每有1个负面状态，该伤害翻倍 | 0: Special_ZhongJieYiJi_359; -1001: Special_ZhongJieYiJi_359 |
| 觉醒者 | MainPackage_Skill_35_JueXingZhe | 2 | 2 | 1 | 10 | 0:0/0/0/0 | P01=0 | P01=2×L | 力量 +[P01] 敏捷 +[P01] 智力 +[P01] | 1001: SetTargetAttributeValue,SetTargetAttributeValue,SetTargetAttributeValue; 1002: SetTargetAttributeValue,SetTargetAttributeValue,SetTargetAttributeValue |
| 濒死体验 | MainPackage_Skill_360_BingSiTiYan | 2 | 1 | 1 | 4 | 0:0/0/0/0 | P01=0.25; P02=1 | P01=0×L; P02=1×L | 受到生命值伤害时：有[P01]%的概率永久提升[P02]点最大生命值 | 1013: Special_BingSiTiYan_360 |
| 淬炼 | MainPackage_Skill_361_CuiLian | 2 | 2 | 1 | 1 | 0:0/0/0/2 | P01=1 | P01=0×L | 移除一个技能后：如果这个技能为传说技能，使你的武器等级+1 | 1103: Special_YingHua_361 |
| 回收 | MainPackage_Skill_362_HuiShou | 1 | 2 | 1 | 6 | 0:0/0/0/2 | P01=5 | P01=3×L | 移除一个技能后：获得该技能等级x[P01]金币，该技能稀有度越高，便再获得一次 | 1103: Special_HuiShou_362 |
| 灵魂收割 | MainPackage_Skill_363_LingHunShouGe | 0 | 1 | 0 | 15 | 0:1/1/3/1 | P01=0.9; P02=1 | P01=0.1×L; P02=0×L | 造成[P01]%([P01*2103])智力的伤害，并获得[P02]层{Buff_MainPackage_LingHun_21_Title} | 0: DamageTarget,SelfAddBuff; -1001: SetExpectionDamageByTargetAttribute,SetBuffSettingSkillExpectation |
| 招魂术 | MainPackage_Skill_364_ZhaoHunShu | 0 | 1 | 0 | 4 | 0:1/1/5/0 | P01=1 | P01=1×L | 获得[P01]层{Buff_MainPackage_LingHun_21_Title} | 0: SelfAddBuff; -1001: SetBuffSettingSkillExpectation |
| 尸爆术 | MainPackage_Skill_365_ShiBaoShu | 1 | 1 | 0 | 10 | 0:1/0/4/1 | P01=0.4 | P01=0.1×L | 消灭最大生命值最高的其他友方单位，对所有敌人造成被消灭单位最大生命值[P01]%的伤害 | 0: Special_ShiBaoShu_365; -1001: Special_ShiBaoShu_365 |
| 骨矛投掷 | MainPackage_Skill_366_GuMaoTouZhi | 0 | 1 | 0 | 10 | 1:1/1/2/1 | P01=2; P02=1 | P01=5×L; P02=0×L | 造成[P01]点真实伤害。灵魂([P02])：伤害翻倍 | 0: IncreaseSkillDamage_ByLingHun,DamageTargetConstOnRealDamage; -1001: IncreaseSkillDamage_ByLingHun,SetToRealDamage; -1003: SelfUnitHaveTargetBuffInfo |
| 断头台 | MainPackage_Skill_367_DuanTouTai | 2 | 1 | 0 | 4 | 0:2/0/6/1 | P01=2; P02=3 | P01=2×L; P02=0×L | 造成[P01]点真实伤害<br>本场战斗中，该技能每使用1次，伤害翻倍（已使用次数<C01>）<br>灵魂([P02]):刷新该技能冷却 | 0: Special_DuanTouTai_367,RemoveSkillCoolDown_ByLingHun; -1001: Special_DuanTouTai_367; -1003: SelfUnitHaveTargetBuffInfo; 1008: ResetTagValue |
| 唤醒尸骨 | MainPackage_Skill_368_HuanXingShiGu | 0 | 1 | 0 | 15 | 0:2/1/2/1 | P01=0; P02=0; P03=1 | P01=10×L; P02=3×L; P03=0×L | 召唤1个骷髅（获得[P01]点生命值，[P02]层活力）<br>灵魂（[P03]）:使其获得嘲讽 | 0: CacheTargetAttributeData,CacheTargetBuffData,ChaCeBuff_ByCasterHaveTargetLingHun,SummonUnit_ApplyCacheData; -1001: SetUnknowSkillExp; -1003: SelfUnitHaveTargetBuffInfo |
| 死亡契约 | MainPackage_Skill_369_SiWangQiYue | 1 | 1 | 0 | 10 | 0:0/0/2/1 | P01=0.05 | P01=0.1×L | 消灭最大生命值最高的其他友方单位，获得目标最大生命值[P01]%的{Buff_MainPackage_Vitality_4_Title} | 0: Special_ShiBaoShu_368; -1001: Special_ShiBaoShu_368 |
| 献祭仪式 | MainPackage_Skill_370_XianJiYiShi | 1 | 1 | 0 | 4 | 0:0/0/1/1 | P01=1 | P01=1×L | 消灭生命值最低的其他友方单位，获得[P01]点行动力 | 0: Special_ShiBaoShu_370; -1001: Special_ShiBaoShu_370 |
| 行动指令 | MainPackage_Skill_371_XingDongZhiLing | 1 | 1 | 0 | 3 | 0:0/2/1/1 | P01=0 | P01=1×L | 使所有其他友方单位行动[P01]次 | 0: AdditionAction_AllOtherTeamMember; -1001: SetBuffSettingSkillExpectation |
| 黑暗牵引 | MainPackage_Skill_372_HeiAnQianYing | 0 | 1 | 0 | 15 | 0:1/1/2/1 | P01=3; P02=2; P03=2; P04=2 | P01=3×L; P02=0×L; P03=0×L; P04=0×L | 造成[P01]点伤害<br>灵魂([P02]):附加[P03]层{Buff_MainPackage_XuRuo_12_Title}并获得[P04]点行动力 | 0: Special_HeiAnQianYing_372; -1001: Special_HeiAnQianYing_372; -1003: SelfUnitHaveTargetBuffInfo |
| 黑暗仪式 | MainPackage_Skill_373_HeiAnYiShi | 1 | 1 | 0 | 6 | 1:1/0/3/1 | P01=0 | P01=1×L | 使一个随机技能冷却-[P01] | 0: ReduceCooldownRandomColldownSkill; -1001: SetBuffSettingSkillExpectation |
| 灵魂透支 | MainPackage_Skill_374_LingHunTouZhi | 1 | 1 | 0 | 5 | 0:0/0/2/1 | P01=0; P02=1 | P01=1×L; P02=0×L | 获得[P01]点行动力和[P02]层{Buff_MainPackage_1015_ChenZhong_Title} | 0: GainActionPoint,SelfAddBuff; -1001: SetBuffSettingSkillExpectation |
| 灵魂冲击 | MainPackage_Skill_375_LingHunChongJi | 0 | 1 | 0 | 20 | 1:1/1/2/1 | P01=2; P02=1 | P01=2×L; P02=0×L | 造成[P01]点伤害<br>灵魂([P02]):该技能等级+1 | 0: Special_LingHunChongJi_372; -1001: Special_LingHunChongJi_372; -1003: SelfUnitHaveTargetBuffInfo |
| 不死 | MainPackage_Skill_376_BuSi | 2 | 2 | 1 | 4 | 0:0/0/0/1 | P01=0.8; P02=0.1; P03=0.3 | P01=0.2×L; P02=0×L; P03=0×L | 受到致命伤害后，有[P01]%的概率将当前生命值变为最大生命值的[P02]%，同时该技能触发率-[P03]%（进入新区域时重置，已触发次数<C01>） | 1018: Special_BuSi_376; 1021: ResetTagValue |
| 溃烂 | MainPackage_Skill_377_KuiLan | 1 | 2 | 1 | 10 | 0:0/0/0/1 | P01=2 | P01=4×L | 回合开始时：降低所有敌人[P01]点最大生命值 | 1010: ChangeAllEnemyMaxHealth |
| 骨刺术 | MainPackage_Skill_378_GuCiShu | 1 | 1 | 0 | 15 | 1:2/1/2/1 | P01=0.9; P02=0.25 | P01=0.1×L; P02=0×L | 造成[P01]%([P01*2103])智力的{Buff_MainPackage_1006_ZhenShiShangHai_Title}，目标每有1层{Buff_MainPackage_CuiRuo_1_Title}该伤害+[P02]% | 0: IncreaseSkillDamage_ByTargetHaveTargetBuff,DamageTarget; -1001: IncreaseSkillDamage_ByTargetHaveTargetBuff,SetExpectionDamageByTargetAttribute |
| 灵魂回收 | MainPackage_Skill_379_LingHunHuiShou | 1 | 2 | 1 | 6 | 0:0/0/0/1 | P01=0 | P01=2×L | 在其他单位死亡时：获得[P01]层{Buff_MainPackage_LingHun_21_Title} | 1017: SelfAddBuff |
| 生命汲取 | MainPackage_Skill_380_ShengMingJiQu | 0 | 1 | 0 | 15 | 0:3/1/2/1 | P01=5; P02=1 | P01=2×L; P02=0×L | 造成[P01]点伤害<br>每场战斗首次使用：恢复造成生命值伤害的生命值 | 0: DamageTargetConst; -1001: SetExpectionDamage; 1005: Special_ShengMingJiQu_380; 1008: ResetTagValue |
| 朽骨为薪 | MainPackage_Skill_381_XiuGuWeiXin | 1 | 1 | 1 | 10 | 0:0/0/0/1 | P01=3 | P01=3×L | 在其他友方单位死亡时：所有友方单位获得[P01]层{Buff_MainPackage_Vitality_4_Title} | 1017: Special_XiuGuWeiXing_381 |
| 先来一具 | MainPackage_Skill_382_XianLaiYiJu | 1 | 2 | 1 | 10 | 0:0/0/0/1 | P01=0; P02=1 | P01=20×L; P02=2×L | 战斗开始时：召唤一个骷髅，该骷髅额外获得[P01]点生命值和[P02]层{Buff_MainPackage_Vitality_4_Title}。 | 1008: CacheTargetBuffData,CacheTargetAttributeData,SummonUnit_ApplyCacheData |
| 亡灵大军 | MainPackage_Skill_383_WangLingDaJun | 1 | 2 | 1 | 10 | 0:0/0/0/1 | P01=2; P02=15; P03=0.4 | P01=0×L; P02=15×L; P03=0.1×L | 回合结束时：如果拥有[P01]层{Buff_MainPackage_LingHun_21_Title}，召唤一个骷髅，骷髅拥有[P02]点生命值并获得[P03]%([P03*2103])智力的{Buff_MainPackage_Vitality_4_Title} | 1011: CacheTargetBuffData,CacheTargetAttributeData,Special_WangLingDaJun_383 |
| 灵压 | MainPackage_Skill_384_LingYa | 1 | 2 | 1 | 10 | 0:0/0/0/1 | P01=1; P02=0 | P01=0×L; P02=3×L | 回合结束时：自身每有[P01]层{Buff_MainPackage_LingHun_21_Title}，便对所有敌人造成[P02]点伤害 | 1011: DamageTargetByTargetBuff |
| 骸骨壁垒 | MainPackage_Skill_385_HaiGuBiLei | 1 | 2 | 1 | 10 | 0:0/0/0/1 | P01=3 | P01=2×L | 回合结束时：每有一个其他友方单位，获得[P01]点护甲 | 1011: GainArmor_ByOtherTeamCount |
| 灵魂风暴 | MainPackage_Skill_386_LingHunFengBao | 2 | 2 | 1 | 4 | 0:0/0/0/1 | P01=1 | P01=1×L | 回合结束时：获得{Buff_MainPackage_LingHun_21_Title}直到[P01]层 | 1011: SelfAddBuffLimitToTargetLevel |
| 罐装灵魂 | MainPackage_Skill_387_GuanZhuangLingHun | 1 | 2 | 1 | 10 | 0:0/0/0/1 | P01=1 | P01=1×L | 战斗开始时：获得[P01]层{Buff_MainPackage_LingHun_21_Title} | 1008: SelfAddBuff |
| 瞬狱杀 | MainPackage_Skill_388_ShunYingSha | 2 | 2 | 1 | 4 | 0:0/0/0/1 | P01=0.25; P02=3 | P01=0.05×L; P02=0×L | 回合结束时：对所有满血的敌方单位造成目标最大生命值[P01]%的{Buff_MainPackage_1006_ZhenShiShangHai_Title}，并附加[P02]层{Buff_MainPackage_CuiRuo_1_Title} | 1011: Special_ShuYuSha_388 |
| 苦痛折磨 | MainPackage_Skill_389_KuTongZheMo | 1 | 1 | 0 | 10 | 0:0/0/2/1 | P01=2 | P01=2×L | 使所有敌方单位身上的负面状态层数+[P01]层 | 0: Special_TongKuZheMo_389; -1001: SetBuffSettingSkillExpectation |
| 蚀命之刃 | MainPackage_Skill_390_ShiMingZhiRen | 1 | 2 | 1 | 10 | 0:0/0/0/1 | P01=2 | P01=3×L | 攻击后：降低目标[P01]点生命值最大生命值 | 1007: ReduceTargetMaxHealth |
| 血誓 | MainPackage_Skill_391_XueShi | 2 | 2 | 1 | 4 | 0:0/0/0/1 | P01=0.3; P02=0.1 | P01=0×L; P02=0.1×L | 战斗开始时：受到当前生命值[P01]%的{Buff_MainPackage_1006_ZhenShiShangHai_Title}，获得伤害数值[P02]%的{Buff_MainPackage_Vitality_4_Title}<br>战斗结束时：恢复因该技能失去的生命值 | 1008: Special_XueShi_391; 1009: Special_XueShi_392_EndBattle |
| 灵魂出鞘 | MainPackage_Skill_392_LingHunChuQiao | 2 | 1 | 0 | 4 | 0:10/2/8/1 | P01=0; P02=0 | P01=1×L; P02=40×L | 获得[P01]层{Buff_MainPackage_JiSu_6_Title}，下个回合开始时，受到[P02]点{Buff_MainPackage_1006_ZhenShiShangHai_Title} | 0: SelfAddBuff,SelfAddBuff; -1001: SetBuffSettingSkillExpectation,SetToSelfDamage |
| 痛苦链接 | MainPackage_Skill_393_TongKuLianJie | 1 | 2 | 1 | 10 | 0:0/0/0/1 | P01=0.8 | P01=0.2×L | 受到生命值伤害时：对所有敌人造成该生命值伤害[P01]%的伤害 | 1013: Special_TongKuLianJie_393 |
| 代价转移 | MainPackage_Skill_394_DaiJiaZhuanYi | 2 | 2 | 1 | 4 | 0:0/0/0/1 | P01=0.5 | P01=0.1×L | 在自己的回合受到生命值伤害时：回合结束时会恢复受到生命值伤害[P01]%的生命值（回合结束时恢复<C01>点生命值） | 1013: Special_DaiJiaZhuanYi_394; 1011: Special_DaiJiaZhuanYi_Recover_395 |
| 咒蚀打击 | MainPackage_Skill_395_ZhouShiDaJi | 0 | 4 | 0 | 10 | 0:1/1/3/1 | P01=4; P02=2; P03=1 | P01=4×L; P02=0×L; P03=1×L | 造成[P01]点伤害<br>灵魂([P02]):附加[P03]层{Buff_MainPackage_CuiRuo_1_Title} | 0: DamageTargetConst,AddBuffToTarget_ByLingHun; -1001: SetExpectionDamage,AddBuffToTarget_ByLingHun; -1003: SelfUnitHaveTargetBuffInfo |
| 邪恶滋养 | MainPackage_Skill_396_XieEZiYang | 0 | 1 | 0 | 15 | 0:5/1/6/1 | P01=3; P02=3; P03=1 | P01=3×L; P02=0×L; P03=0×L | 恢复[P01]点生命值<br>灵魂([P02]):回复量+[P03]% | 0: IncreaseHealthRecoverPrecent,RecoverConstValue; -1001: IncreaseHealthRecoverPrecent,SetHealthRecover; -1003: SelfUnitHaveTargetBuffInfo |
| 灵魂护体 | MainPackage_Skill_397_LingHunHuTi | 0 | 1 | 0 | 15 | 0:2/1/3/1 | P01=2 | P01=2×L | 自身每有1层{Buff_MainPackage_LingHun_21_Title}额外获得[P01]点护甲 | 0: SelfAddBuffBySelfBuff; -1001: SelfAddBuffBySelfBuff |
| 行军指令 | MainPackage_Skill_398_XingJunZhiLing | 1 | 2 | 1 | 10 | 0:0/0/0/1 | P01=1 | P01=1×L | 回合开始时：为所有其他友方单位附加[P01]层{Buff_MainPackage_Vitality_4_Title} | 1011: AddBuffToAllTeam_NotIncludeSelf |
| 骸骨风暴 | MainPackage_Skill_399_HaiGuFengBao | 2 | 1 | 0 | 4 | 0:2/2/2/1 | P01=6 | P01=6×L | 对所有敌人造成[P01]点伤害。场上每有一个其他友方单位，伤害次数+1。 | 0: Special_HaiGuFengBao_399; -1001: Special_HaiGuFengBao_399 |
| 智力提升 | MainPackage_Skill_3_ZhiLiTiSheng | 0 | 2 | 1 | 20 | 0:0/0/0/1 | P01=0 | P01=2×L | 智力+[P01] | 1001: SetTargetAttributeValue; 1002: SetTargetAttributeValue |
| 灵魂炼成 | MainPackage_Skill_400_LingHunLianCheng | 2 | 1 | 0 | 4 | 0:0/0/2/1 | P01=5; P02=3; P03=2 | P01=0×L; P02=0×L; P03=3×L | 受到[P01]点真实伤害<br>灵魂([P02]):生命值永久+[P03] | 0: Special_LingHunLianCheng_400; -1001: Special_LingHunLianCheng_400; -1003: SelfUnitHaveTargetBuffInfo |
| 骨血盾 | MainPackage_Skill_401_GuXueDun | 0 | 1 | 0 | 15 | 0:2/1/1/1 | P01=3 | P01=2×L | 获得[P01]点护甲，如果本回合受到了生命值伤害，获得的护甲量翻倍 | 0: Special_GuXueDun_401; -1001: Special_GuXueDun_401; 1013: Special_GuXueDun_401; -1003: Condition_TargetSkillTagIsSatsify; 1011: ResetTagValue |
| 骸骨堆 | MainPackage_Skill_402_HaiGuDui | 1 | 2 | 1 | 10 | 0:0/0/0/1 | P01=0.2 | P01=0.1×L | 在其他友方单位死亡时：如果阵亡单位不是尸山，召唤一个最大生命值为死亡单位[P01]%的尸山，尸山自带嘲讽；如果已存在尸山，改为尸山获得对应的最大生命值 | 1017: Special_HaiGuDui_402 |
| 渎生铁卫 | MainPackage_Skill_403_DuShengTieWei | 2 | 1 | 0 | 4 | 0:5/2/5/1 | P01=0.5; P02=0.6 | P01=0.5×L; P02=0.4×L | 召唤一个死亡骑士，死亡骑士继承施法者[P01]%的最大生命值，并拥有施法者[P02]%([P02*2103])智力的{Buff_MainPackage_Vitality_4_Title}<br>如果死亡骑士已存在，为其恢复所有生命值<br>武器攻击后：死亡骑士进行会追击 | 0: CacheTargetAttributeData_ByCaster,CacheTargetBuffData,SummonUnit_ApplyCacheData_OrRecoverHeath; -1001: SetUnknowSkillExp; 1014: Special_SiWangQiShi_403 |
| 快斩 | MainPackage_Skill_40_KuaiZhan | 0 | 4 | 0 | 15 | 0:0/1/20/1 | ConstValue=10; P01=15 | ConstValue=10×L; P01=0×L | 造成[ConstValue]点伤害<br>战斗开始时：该技能冷却-[P01] | 0: DamageTargetConst; -1001: SetExpectionDamage; 1008: ChangeCurSkillCooldown |
| 剑术格挡 | MainPackage_Skill_42_JianShuGeDang | 0 | 4 | 0 | 10 | 0:1/1/2/1 | P01=6 | P01=4×L | 获得[P01]点护甲 | 0: GainArmor; -1001: ArmorSkillExpectation |
| 无谋打击 | MainPackage_Skill_43_MuMouDaJi | 0 | 4 | 0 | 10 | 0:0/0/3/1 | ConstValue=6; P01=1 | ConstValue=6×L; P01=0×L | 造成[ConstValue]点伤害并给自身附加[P01]层{Buff_MainPackage_CuiRuo_1_Title} | 0: DamageTargetConst,SelfAddBuff; -1001: SetExpectionDamage,SetBuffSettingSkillExpectationByAttribute |
| 振奋 | MainPackage_Skill_44_ZhenFen | 0 | 4 | 0 | 10 | 0:3/1/5/1 | P01=1 | P01=2×L | 获得[P01]层{Buff_MainPackage_Vitality_4_Title} | 0: SelfAddBuff; -1001: SetBuffSettingSkillExpectationByAttribute |
| 终结斩 | MainPackage_Skill_45_ZhongJieZhan | 0 | 4 | 0 | 20 | 0:0/1/2/1 | P01=3; P02=2 | P01=3×L; P02=0×L | 造成[P01]点伤害.如果该技能每击杀[P02]次敌人，该技能等级+1 | 0: Damage_SpecialZhongJieZhan; -1001: SetExpectionDamage |
| 重斩 | MainPackage_Skill_46_ZhongRen | 0 | 4 | 0 | 15 | 0:0/2/4/1 | DamagePrecent=1.7 | DamagePrecent=0.3×L | 造成[DamagePrecent]%([DamagePrecent*2101])力量的伤害 | 0: DamageTarget; -1001: SetExpectionDamageByTargetAttribute |
| 伤口刺击 | MainPackage_Skill_47_ShangKouCiJi | 1 | 4 | 0 | 2 | 0:0/0/4/1 | ConstValue=6; P01=0 | ConstValue=3×L; P01=1×L | 造成[ConstValue]点伤害.如果目标有{Buff_MainPackage_CuiRuo_1_Title}，为目标附加[P01]层{Buff_MainPackage_CuiRuo_1_Title} | 0: Special_ShangKouCiJi; -1003: TargetUnitHaveTargetBuffInfo; -1001: Special_ShangKouCiJi |
| 格挡打击 | MainPackage_Skill_48_GeDangDaJi | 0 | 4 | 0 | 15 | 0:0/1/2/1 | P01=4; P02=0.75 | P01=4×L; P02=0.25×L | 造成[P01]点伤害并获得[P02]%([P02*2002])防御力的护甲 | 0: GainArmorByRefAtrribute,DamageTargetConst; -1001: SetExpectionDamage,ArmorSkillExpectation |
| 准备姿态 | MainPackage_Skill_49_ZhunBeiZiTai | 1 | 4 | 0 | 10 | 0:3/2/3/1 | P01=2; P02=6 | P01=2×L; P02=6×L | 获得[P01]层{Buff_MainPackage_Vitality_4_Title}，获得[P02]点护甲 | 0: GainArmor,SelfAddBuff; -1001: ArmorSkillExpectation,SetBuffSettingSkillExpectation |
| 恢复力 | MainPackage_Skill_4_HuiFuLi | 1 | 2 | 1 | 12 | 0:0/0/0/1 | P01=0.5; P02=0.35 | P01=0×L; P02=0.05×L | 战斗结束后:如果生命值小于最大生命值的[P01]%，回复相当于[P02]%([P02*2101])力量的生命值 | 1009: Special_HuiFuLi_4 |
| 活力斩 | MainPackage_Skill_50_HuoLiZhan | 1 | 4 | 0 | 10 | 0:1/1/3/1 | P01=4; P02=0 | P01=3×L; P02=1×L | 造成[P01]伤害并获得[P02]层{Buff_MainPackage_Vitality_4_Title} | 0: DamageTargetConst,SelfAddBuff; -1001: SetExpectionDamage,SetBuffSettingSkillExpectation |
| 战前热身 | MainPackage_Skill_54_ZhanQianReShen | 1 | 2 | 1 | 10 | 0:0/0/0/0 | P01=1 | P01=1×L | 战斗开始时:获得[P01]层{Buff_MainPackage_Vitality_4_Title} | 1008: SelfAddBuff |
| 越战越勇 | MainPackage_Skill_55_YueZhanYueYong | 2 | 2 | 1 | 4 | 0:0/0/0/1 | P01=2 | P01=2×L | 使用技能后:如果该技能的行动力消耗大于等于2，获得[P01]层{Buff_MainPackage_Vitality_4_Title} | 1014: Special_YueZhanYueYong |
| 生命提升 | MainPackage_Skill_5_ShengMingTiSheng | 1 | 2 | 1 | 20 | 0:0/0/0/1 | P01=0 | P01=20×L | 最大生命值+[P01] | 1001: SetTargetAttributeValue; 1002: SetTargetAttributeValue |
| 电击术 | MainPackage_Skill_63_DianJiShu | 1 | 1 | 0 | 10 | 0:5/0/2/1 | ConstValue=10 | ConstValue=5×L | 造成[ConstValue]点伤害 | 0: CacheAudioName,DamageTargetConst; -1001: SetExpectionDamage |
| 头脑风暴 | MainPackage_Skill_66_TouNaoFengBao | 2 | 1 | 0 | 4 | 0:3/0/6/0 | ConstValue=0 | ConstValue=2×L | 使自身除此技能外所有技能的冷却-[ConstValue] | 0: Special_TouNaoFengBao_66; -1001: SetBuffSettingSkillExpectation |
| 火球术 | MainPackage_Skill_68_HuoQiuShu | 0 | 1 | 0 | 15 | 0:5/0/3/1 | ConstValue=4; P02=3 | ConstValue=2×L; P02=1×L | 造成[ConstValue]点伤害，并附加[P02]层{Buff_MainPackage_ShaoShang_7_Title} | 0: DamageTargetConst,AddBuffToTarget; -1001: SetExpectionDamage,SetBuffSettingSkillExpectation |
| 魔力提升 | MainPackage_Skill_6_MoLiTiSheng | 1 | 2 | 1 | 20 | 0:0/0/0/1 | P01=0 | P01=25×L | 最大魔力值+[P01] | 1001: SetTargetAttributeValue; 1002: SetTargetAttributeValue |
| 地狱炎 | MainPackage_Skill_72_DiYuYan | 2 | 1 | 0 | 6 | 0:5/2/8/1 | ConstValue=18; P02=0; P03=2 | ConstValue=7×L; P02=5×L; P03=3×L | 对所有敌人造成[ConstValue]点伤害，并附加[P02]层{Buff_MainPackage_ShaoShang_7_Title}，同时对自身附加[P03]层{Buff_MainPackage_ShaoShang_7_Title} | 0: DamageByAllEnemyConstValue,AddBuffToAllEnemy,SelfAddBuff; -1001: SetExpectionDamage,SetBuffSettingSkillExpectation |
| 毒刺术 | MainPackage_Skill_78_DuCiShu | 0 | 4 | 0 | 10 | 0:3/1/2/1 | ConstValue=3 | ConstValue=2×L | 为目标附加[ConstValue]层{Buff_MainPackage_ZhongDu_3_Title} | 0: AddBuffToTarget; -1001: SetBuffSettingSkillExpectation |
| 魔力恢复提升 | MainPackage_Skill_7_MoLiHuiFuTiSheng | 1 | 2 | 1 | 20 | 0:0/0/0/1 | P01=1 | P01=2×L | 魔力恢复+[P01] | 1001: SetTargetAttributeValue; 1002: SetTargetAttributeValue |
| 贪婪者 | MainPackage_Skill_83_TasnLan | 1 | 2 | 1 | 20 | 0:0/0/0/0 | P01=1 | P01=1×L | 击杀敌人后：获得[P01]金币 |  |
| 持续力 | MainPackage_Skill_8_ChiXuLi | 0 | 2 | 1 | 10 | 0:0/0/0/0 | P01=2 | P01=3×L | 战斗开始时：使最左边一个未就绪的技能冷却-[P01] | 1008: LeftestSkillCooldownReduce |
| 荆棘盾 | MainPackage_Skill_97_JinJiDun | 1 | 1 | 0 | 15 | 0:3/1/8/1 | ArmorCount=2; ThornCount=2 | ArmorCount=1×L; ThornCount=1×L | 获得[ArmorCount]点护甲,获得[ThornCount]层{Buff_MainPackage_JingJi_2_Title} | 0: GainArmor,SelfAddBuff; -1001: ArmorSkillExpectation,SetBuffSettingSkillExpectation |
| 专研 | MainPackage_Skill_98_ZhuanYan | 2 | 4 | 0 | 1 | 0:0/0/99/2 | P01=1 | P01=0×L | 永久获得一次当前所有属性提升技能的属性值 | 0: Special_ZhuanYan; -1001: SetSkillExpectionType |
| 胡乱打击 | MainPackage_Skill_99_HuLuanDaJi | 1 | 4 | 0 | 10 | 0:1/0/1/1 | ConstValue=2 | ConstValue=2×L | 对所有敌人造成[ConstValue]点伤害 | 0: DamageByAllEnemyConstValue; -1001: SetExpectionDamage |
| 战前冷却 | MainPackage_Skill_9_LengQueJiaSu | 1 | 2 | 1 | 10 | 0:0/0/0/0 | P01=0; P02=3 | P01=1×L; P02=2×L | 战斗开始时：使最多[P01]个未就绪的技能冷却-[P02] | 1008: ReduceCooldownRandomColldownSkill |
| Fire ball | MainPackage_Skill_FireBall | 0 | 1 | 0 | 10 | 0:100/0/2/1 | ConstValue=25 | ConstValue=5×L | 造成[ConstValue]点伤害 |  |
| 测试 | MainPackage_Skill_Test_Summed | 0 | 2 | 0 | 1 | 0:0/0/0/1 | P01=0 |  | 召唤测试 | 0: SummerTargetUnit |
| 冰心权杖 | MainPackage_Weapen_10_BingXinQuanZhang | 0 | 0 | 0 | 20 | 0:0/1/0/1 | P01=4; AddCount=0 | P01=1×L; AddCount=2×L | 造成[P01]点伤害，额外附加[AddCount]层{Buff_MainPackage_HanShuang_8_Title} | 0: DamageTargetConst,AddBuffToTarget; -1001: SetExpectionDamage,SetBuffSettingSkillExpectation |
| 时间沙漏 | MainPackage_Weapen_11_ShiJianShaLou | 0 | 0 | 0 | 20 | 0:0/2/0/1 | P01=6; ReduceCount=0 | P01=4×L; ReduceCount=2×L | 造成[P01]点伤害，使当前冷却最长的技能冷却-[ReduceCount] | 0: DamageTargetConst,ReduceHighestCoolDown; -1001: SetExpectionDamage,SetBuffSettingSkillExpectation |
| 袖箭 | MainPackage_Weapen_12_XiuJian | 0 | 0 | 0 | 20 | 0:0/1/0/1 | P01=4; AdditionDamage=1 | P01=1×L; AdditionDamage=1×L | 造成[P01]点伤害，同时对一个随机敌人造成[AdditionDamage]点伤害 | 0: SpawnEffectToTarget_OnAttackTarget,DamageTargetConst,DamageRandomEnemy; -1001: SetExpectionDamage |
| 剑盾 | MainPackage_Weapen_13_JianDun | 0 | 0 | 0 | 20 | 0:0/2/0/1 | P01=3; ArmorCount=4 | P01=3×L; ArmorCount=3×L | 造成[P01]点伤害，同时获得[ArmorCount]点护甲 | 0: SpawnEffectToTarget_OnAttackTarget,DamageTargetConst,GainArmor; -1001: SetExpectionDamage,ArmorSkillExpectation |
| 锈蚀匕首 | MainPackage_Weapen_14_XiuShiBiShou | 0 | 0 | 0 | 20 | 0:0/1/0/1 | P01=2; AddCount=1 | P01=1×L; AddCount=2×L | 造成[P01]点伤害，额外附加[AddCount]层{Buff_MainPackage_QingShi_11_Title} | 0: SpawnEffectToTarget_OnAttackTarget,DamageTargetConst,AddBuffToTarget; -1001: SetExpectionDamage,SetBuffSettingSkillExpectation |
| 盗贼匕首 | MainPackage_Weapen_15_DaoZeiBiShou | 0 | 0 | 0 | 20 | 0:0/1/0/1 | P01=2; AttackNeed=6; GetCoin=1 | P01=1×L; AttackNeed=0×L; GetCoin=1×L | 造成[P01]点伤害，每[AttackNeed]次普通攻击后，获得[GetCoin]枚金币(已攻击次数:<AttackCount>) | 0: RegistOrLogOffListenTag,SpawnEffectToTarget_OnAttackTarget,Special_DaoZeiBiShou_15,DamageTargetConst; 1001: RegistOrLogOffSkillTag; -1001: SetExpectionDamage |
| 双手斧 | MainPackage_Weapen_16_ShuangShouFu | 0 | 0 | 0 | 20 | 0:0/2/0/1 | P01=8; P02=2 | P01=8×L; P02=0×L | 造成[P01]点伤害，攻击后，魔力值-[P02] | 0: SpawnEffectToTarget_OnAttackTarget,DamageTargetConst,RecoverConstValue; -1001: SetExpectionDamage |
| 双手剑 | MainPackage_Weapen_17_ShuangShouJian | 0 | 0 | 0 | 20 | 0:0/2/0/1 | P01=7 | P01=5×L | 对所有敌人造成[P01]点伤害 | 0: DamageByAllEnemyConstValue; -1001: SetExpectionDamage |
| 巨人战斧 | MainPackage_Weapen_18_JuRenZhanFu | 2 | 0 | 0 | 20 | 0:0/3/0/1 | P01=2 | P01=0.2×L | 造成[P01]%([P01*2101])力量的伤害 | 0: SpawnEffectToTarget_OnAttackTarget,DamageTarget; -1001: SetExpectionDamageByTargetAttribute |
| 镰刀 | MainPackage_Weapen_19_LianDao | 0 | 0 | 0 | 20 | 0:0/1/0/1 | P01=3 | P01=1×L | 造成[P01]点真实伤害 | 0: DamageTargetConstOnRealDamage; -1001: SetToRealDamage |
| 剑 | MainPackage_Weapen_1_Jian | 0 | 0 | 0 | 20 | 0:0/1/0/1 | P01=4 | P01=2×L | 造成[P01]点伤害 | 0: SpawnEffectToTarget_OnAttackTarget,DamageTargetConst; -1001: SetExpectionDamage |
| 利爪刃 | MainPackage_Weapen_20_LiZhuaRen | 0 | 0 | 0 | 20 | 0:0/2/0/1 | P01=3; P02=4 | P01=1×L; P02=0×L | 造成[P01]点伤害[P02]次 | 0: DamageTargetConst; -1001: SetExpectionDamage,SetDamageCountExpectation |
| 长柄刀 | MainPackage_Weapen_21_ChangBingDao | 0 | 0 | 0 | 20 | 0:0/1/0/1 | P01=6 | P01=4×L | 造成[P01]点伤害，本回合每次使用行动力消耗+1 | 0: DamageTargetConst; -1001: SetExpectionDamage; 1014: Special_Weapen_ChangBingDao_21; 1010: RemoveTargetSkillActionChange |
| 饮血剑 | MainPackage_Weapen_22_YinXueJian | 2 | 0 | 0 | 20 | 0:0/1/0/1 | P01=4; P02=0 | P01=2×L; P02=1×L | 造成[P01]点伤害，如果造成了生命值伤害，恢复[P02]点生命值 | 0: DamageTargetConst; -1001: SetExpectionDamage; 1005: Special_Weapen_YingXueJian_22 |
| 法杖 | MainPackage_Weapen_2_FaZhang | 0 | 0 | 0 | 20 | 0:0/1/0/1 | P01=4; P02=3 | P01=1×L; P02=2×L | 造成[P01]点伤害<br>战斗开始时：恢复[P02]点魔力 | 0: DamageTargetConst,SpawnEffectToTarget; -1001: SetExpectionDamage; 1008: RecoverConstValue |
| 匕首 | MainPackage_Weapen_3_BiShou | 0 | 0 | 0 | 20 | 0:0/1/0/1 | P01=3; P02=4 | P01=1×L; P02=0×L | 造成[P01]点伤害，每回合第一次使用行动力消耗-1 | 0: SpawnEffectToTarget_OnAttackTarget,DamageTargetConst,RemoveTargetSkillActionChange; -1001: SetExpectionDamage; 1010: ChangeTargetSkillActionCost |
| 长枪 | MainPackage_Weapen_4_ChangQiang | 0 | 0 | 0 | 20 | 0:0/2/0/1 | P01=8 | P01=6×L | 造成[P01]点伤害 | 0: SpawnEffectToTarget_OnAttackTarget,DamageTargetConst; -1001: SetExpectionDamage |
| 魔剑 | MainPackage_Weapen_5_MoJian | 0 | 0 | 0 | 20 | 0:0/2/0/1 | P01=5 | P01=5×L | 造成[P01]点伤害，每次使用魔法后，该武器行动力-1，使用后重置 | 0: DamageTargetConst,RemoveTargetSkillActionChange; -1001: SetExpectionDamage; 1014: Special_Weapen_MoJian_5 |
| 死灵书 | MainPackage_Weapen_6_SiLingShu | 0 | 0 | 0 | 20 | 0:0/2/0/1 | P01=3; P02=0; P03=0 | P01=3×L; P02=15×L; P03=7×L | 造成[P01]点伤害<br>战斗开始时：召唤一个拥有[P02]点最大生命值和[P03]层{Buff_MainPackage_Vitality_4_Title}的骷髅 | 0: DamageTargetConst; -1001: SetExpectionDamage; 1008: CacheTargetAttributeData,CacheTargetBuffData,SummonUnit_ApplyCacheData |
| 双刃 | MainPackage_Weapen_7_ShuangRen | 0 | 0 | 0 | 20 | 0:0/2/0/1 | P01=4 | P01=4×L | 造成[P01]点伤害两次 | 0: SpawnEffectToTarget_OnAttackTarget,DamageTargetConst; -1001: SetExpectionDamage,SetDamageCount |
| 龙骨弓 | MainPackage_Weapen_8_LongGuGong | 0 | 0 | 0 | 20 | 0:0/1/0/1 | DamagePrecent=0.9 | DamagePrecent=0.1×L | 造成[DamagePrecent]%([DamagePrecent*2102])敏捷的伤害 |  |
| 毒牙鞭 | MainPackage_Weapen_9_DuYaBian | 0 | 0 | 0 | 20 | 0:0/2/0/1 | P01=5; AddCount=0 | P01=2×L; AddCount=2×L | 造成[P01]点伤害，额外附加[AddCount]层{Buff_MainPackage_ZhongDu_3_Title} | 0: SpawnEffectToTarget_OnAttackTarget,DamageTargetConst,AddBuffToTarget; -1001: SetExpectionDamage,SetBuffSettingSkillExpectation |
| 攻击 | SkillData_MonsterNormalAttack | 0 | 1 | 0 | 999 | 0:0/0/0/1 | P01=3 | P01=3×L | 造成[P01]点伤害 | 0: DamageTargetConst; -1001: SetExpectionDamage |
| 格挡 | SkillData_MonsterNormalDefense | 0 | 4 | 0 | 999 | 0:5/1/3/1 | P01=3 | P01=2×L | 获得[P01]点护甲 | 0: GainArmor; -1001: ArmorSkillExpectation |
| 眩晕 | SkillData_NothingToDo | 0 | 1 | 0 | 999 | 0:0/0/0/1 | P01=3 |  | 什么也不做 | -1001: SetNothingToDoSkillExp |

## 状态与说明词条：55 条

| 名称 | ID | 负面 | 叠加类型 | 自动衰减 | 战后清除 | 基础参数 | 每层增量 | 配置描述 | 事件函数 |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 力竭 | MainPackage_1001_LiJie | 1 | 2 | 1 | 1 | P01=0 | P01=0×L | 该状态被移除时：移除自身所有{Buff_MainPackage_Vitality_4_Title} | 1015: RemoveTargetBuff |
| 束缚 | MainPackage_1002_ShuFu | 1 | 2 | 0 | 1 | P01=0 | P01=0×L | 不可逃跑 | 1001: SetSpecialTag; 1002: SetSpecialTag |
| 虚无药瓶 | MainPackage_1003_KongYaoPing | 0 | 0 | 1 | 1 | P01=0 | P01=0×L | 没有效果的物品，使用时视作使用一次药水，战斗结束后移除 |  |
| 召唤物 | MainPackage_1004_ZhaoHuanWu | 0 | 0 | 1 | 1 | P01=0 | P01=0×L | 战斗中的帮手，在没有指挥的情况下会随机攻击敌人 |  |
| 缴械 | MainPackage_1005_ChanRao | 1 | 0 | 1 | 1 | P01=0 | P01=0×L | 无法使用普通攻击，回合结束减少一层 | 1001: SetSpecialTag; 1002: SetSpecialTag |
| 真实伤害 | MainPackage_1006_ZhenShiShangHai | 0 | 0 | 1 | 1 | P01=0 | P01=0×L | 该伤害无视护甲，该伤害不会受到任何增益和减益 |  |
| 就绪 | MainPackage_1007_JiuXu | 0 | 0 | 1 | 1 | P01=0 | P01=0×L | 技能没有进入冷却状态 |  |
| 透支 | MainPackage_1008_TouZhi | 1 | 0 | 0 | 1 | P01=0 | P01=1×L | 回合结束时，减少[P01*LV]层活力 | 1010: RuduceTargetBuff,ReduceLevel |
| 以攻为守 | MainPackage_1009_YiGongWeiShou | 0 | 0 | 1 | 1 | P01=0 | P01=1×L | 每次攻击后，获得[P01*LV]点护甲 | 1007: GainArmor; 1011: ReduceLevel |
| 刹那双影 | MainPackage_1010_ChaNaShuangYing | 0 | 3 | 0 | 1 | P01=0 | P01=0×L | 下一个使用的武技会触发两次 | 1014: Special_ChaNaShuangYing_1010 |
| 时空领域 | MainPackage_1011_ShiKongLingYu | 0 | 0 | 0 | 1 | P01=1 | P01=0×L | 下[P01*LV]个使用的技能不会消耗行动力 | 1001: SetAllSkillActionCostToZero; 1002: RemoveAllSkillActionCostChange; 1014: Special_ShiKongLingYu_1011; 1011: ReduceLevel |
| 施法干扰 | MainPackage_1012_ShiFaGanRao | 1 | 3 | 0 | 1 | P01=1 | P01=0×L | 使用技能后：你的一个随机主动技能冷却+[P01]，之后移除该状态 | 1014: Special_ShiFaGanRao_1012; 1011: ReduceLevel |
| 影甲 | MainPackage_1013_YingJia | 0 | 0 | 0 | 1 | P01=3 | P01=2×L | 受到伤害后：如果没有受到生命值伤害，护甲+[P01] | 1013: Special_YingJia_1013; 1010: ReduceLevel |
| 畏缩 | MainPackage_1014_WeiSuo | 0 | 0 | 0 | 1 | P01=100 | P01=0×L | 每受到[P01]点生命值伤害，获得一定量的护甲，同时自身获得1层{Buff_MainPackage_1005_ChanRao_Title} |  |
| 沉重 | MainPackage_1015_ChenZhong | 0 | 0 | 0 | 1 | P01=1 | P01=0×L | 武器和技能的行动消耗+[P01]，使用武器或技能该状态层数-1 | 1001: ChangeAllSkillActionCost; 1002: RemoveAllSkillActionCostChange; 1014: ReduceLevel |
| 二重唱 | MainPackage_1017_ErChongChang | 0 | 1 | 0 | 1 | P01=0 | P01=0×L | 下一个使用的魔法会触发两次 | 1014: Special_ErChongChang_1016 |
| 灵感施法 | MainPackage_1018_LingGanShiFa | 0 | 1 | 0 | 1 | P01=1 | P01=0×L | 下[P01]个使用的技能不会消耗行动力 | 1001: SetAllSkillActionCostToZero; 1002: RemoveAllSkillActionCostChange; 1014: Special_ShiKongLingYu_1011; 1011: ReduceLevel |
| 尝试逃跑 | MainPackage_1019_ChangShiTaoPao | 0 | 3 | 0 | 1 | P01=0 | P01=0×L | 回合结束时：该单位会逃跑 | 1001: SetEscape; 1002: SetEscape |
| 胆小 | MainPackage_1019_DanXiao | 0 | 0 | 0 | 1 | P01=4 | P01=0×L | 在[P01]个回合后逃跑 |  |
| 劣质爆破药剂 | MainPackage_1020_LieZhiBaoPoYao | 0 | 0 | 1 | 1 | P01=0 | P01=0×L | 药水，造成25点伤害 |  |
| 森林之友 | MainPackage_1021_SenLinZhiYou | 0 | 0 | 0 | 1 | P01=1; P02=50 | P01=1×L; P02=0×L | 花妖死亡时：移除自身[P01]层{Buff_MainPackage_JingJi_2_Title}<br>回合结束时：如果有其他友方单位，获得[P02]点护甲 |  |
| 多头生物 | MainPackage_1022_DuoTouShengWu | 0 | 0 | 0 | 1 | P01=0 | P01=0×L | 死亡时，会对多头蛇造成75点真实伤害<br>多头蛇死亡时，消灭自身 |  |
| 智者的求知 | MainPackage_1023_ZhiZheDeQiuZhi | 1 | 0 | 0 | 1 | P01=0 | P01=0×L | 每使用3次技能后，深渊智者会获得1层活力 |  |
| 重压立场 | MainPackage_1024_ZhongYaLiChang | 1 | 3 | 0 | 1 | P01=1 | P01=0×L | 使用技能后：获得1层沉重，之后移除该状态 | 1014: Special_ZhongYaLiChang_1060 |
| 深渊咒缚 | MainPackage_1025_ShenYuanZhouFu | 1 | 0 | 0 | 1 | P01=0 | P01=1×L | 回合开始时：受到[P01]点伤害，并移除[P01]点魔力 | 1010: DamageSelf,CurrentMagicChange |
| 烈焰之墙 | MainPackage_1026_LieYanZhiQiang | 1 | 0 | 0 | 1 | P01=0; P02=0 | P01=10×L; P02=2×L | 受到攻击时：对所有敌人造成[P01]点伤害并附加[P02]层{Buff_MainPackage_ShaoShang_7_Title}<br>回合开始时移除此效果 | 1012: DamageAllEnemy,GetTargetBuff; 1016: ReduceLevel |
| 钱袋 | MainPackage_1027_QianDai | 1 | 0 | 0 | 1 | P01=0 | P01=0×L | 这个单位身上貌似有金币... |  |
| 灵魂(x) | MainPackage_1028_LingHunDes | 1 | 0 | 0 | 1 | P01=0 | P01=0×L | 如果自身存在x层的灵魂层数，消耗对应层数并触发后续效果，否则获得1层灵魂 |  |
| 灵魂出鞘 | MainPackage_1029_LingHunChuQiao | 1 | 0 | 1 | 1 | P01=0 | P01=1×L | 回合开始时：受到[P01]点真实伤害 | 1010: DamageSelf,ReduceLevel |
| 力量 | MainPackage_2001_StrengthContent | 1 | 0 | 0 | 1 | P01=0 | P01=0×L | 角色的基础属性<br>部分技能享受这个属性的加成 |  |
| 敏捷 | MainPackage_2002_AglieContent | 1 | 0 | 0 | 1 | P01=0 | P01=0×L | 角色的基础属性<br>部分技能享受这个属性的加成 |  |
| 智力 | MainPackage_2003_IntelligenceContent | 1 | 0 | 0 | 1 | P01=0 | P01=0×L | 角色的基础属性<br>部分技能享受这个属性的加成 |  |
| 防御力 | MainPackage_2004_DefenseContent | 1 | 0 | 0 | 1 | P01=0 | P01=0×L | 角色的基础属性<br>部分技能享受这个属性的加成 |  |
| 壁垒 | MainPackage_BiLei_10 | 0 | 0 | 0 | 1 | P01=0 | P01=0×L | 保留下回合护甲 | 1010: ReduceLevel; 1001: SetBiLeiLevel; 1002: SetBiLeiLevel |
| 标记 | MainPackage_BiaoJi_17 | 1 | 0 | 0 | 1 | P01=0 | P01=0×L | 玩家以外的单位必定以该单位为攻击目标 |  |
| 嘲讽 | MainPackage_ChaoFeng_16 | 0 | 0 | 1 | 1 | P01=0 | P01=0×L | 对方只能选择这个单位为目标<br>回合结束时减少一层 | 1001: SetTaunt; 1002: SetTaunt |
| 易伤 | MainPackage_CuiRuo_1 | 1 | 2 | 1 | 1 | DamagePrecent=0.5 | DamagePrecent=0×L | 受到的攻击伤害+[DamagePrecent]%<br>回合结束时减少一层 | 1001: IncreaseUnitTakeDamage; 1002: RemoveSelfDamageChange |
| 反击 | MainPackage_FanJi_5 | 0 | 0 | 0 | 1 | P01=0 | P01=0×L | 受到攻击后:对伤害来源使用一次普通攻击，该次伤害只造成50%的伤害，之后移除一层 | 1001: SetCounterAttack; 1101: ReduceLevel; 1002: SetCounterAttack |
| 感电 | MainPackage_GanDian_18 | 1 | 0 | 0 | 1 | P01=6 | P01=0×L | 受到攻击时：受到[P01]点伤害，之后移除一层 | 1012: Special_GanDian_18,ReduceLevel |
| 过载 | MainPackage_GuoZai_9 | 1 | 0 | 0 | 1 | Damage=0 | Damage=1×L | 行动结束时：受到[Damage*LV]点伤害，同时移除该状态 | 1010: DamageSelf,ReduceLevel |
| 寒霜 | MainPackage_HanShuang_8 | 1 | 0 | 0 | 1 | Damage=0 | Damage=1×L | 攻击时，降低寒霜层数的伤害，并移除同等寒霜层数 | 1001: TargetAttributeValueChange; 1002: RemoveAttributeValueChange; 1006: Special_ShaoShang_8_AfterAttack |
| 幻象 | MainPackage_HuanXiang_19 | 0 | 0 | 0 | 1 | P01=0.2; P02=10 | P01=0×L; P02=0×L | 幻象继承施法者的属性，但是最大生命值只继承[P01]%，同时回合结束时，受到[P02]点真实伤害 | 1011: DamageSelf |
| 急速 | MainPackage_JiSu_6 | 0 | 0 | 0 | 1 | P01=0 | P01=1×L | 行动结束时：进行一个额外回合，同时急速减少一层 | 1001: SetJiSuCount; 1002: SetJiSuCount; 1010: ReduceLevel |
| 荆棘 | MainPackage_JingJi_2 | 0 | 0 | 1 | 1 | Damage=0 | Damage=1×L | 受到攻击后:伤害来源受到[Damage*LV]点伤害<br>回合结束时减少一层 | 1012: PlayAudio,DamageTargetByJingJi |
| 亢奋 | MainPackage_KangFen_20 | 0 | 0 | 0 | 1 | P01=0; P02=0; P03=0 | P01=0.1×L; P02=1×L; P03=1×L | 最大生命值+[P01]%，防御+[P03] | 1001: AllEnemyChangeTargetAttribute,AllEnemyChangeTargetAttribute |
| 亢奋 | MainPackage_KuangFen_20 | 0 | 0 | 0 | 1 | P01=0.1; P02=1 | P01=0×L; P02=0×L | 最大生命值+[P01]%，防御+[P03] |  |
| 灵魂 | MainPackage_LingHun_21 | 0 | 0 | 0 | 1 | P01=0 | P01=1×L | 回合开始时：恢复[P01]点魔力，并获得[P01]点护甲，受到[P01]点真实伤害 | 1010: DamageSelf,CurrentMagicChange,GainArmor |
| 破甲 | MainPackage_QingShi_11 | 1 | 0 | 1 | 1 | P01=0 | P01=1×L | 获得的护甲量-[P01]<br>回合结束时减少一层 | 1001: TargetAttributeValueChange; 1002: RemoveAttributeValueChange |
| 烧伤 | MainPackage_ShaoShang_7 | 1 | 0 | 1 | 0 | P01=0 | P01=1×L | 最大临时生命值-[P01*LV]<br>回合结束时减少一层 | 1001: Special_ShaoShang_7; 1002: Special_ShaoShang_7 |
| 睡眠 | MainPackage_ShuiMian_15 | 1 | 0 | 1 | 1 | P01=0 | P01=1×L | 跳过回合，回合结束减少一层S，受到伤害后移除全部层数 | 1001: TargetAttributeValueChange; 1002: RemoveAttributeValueChange; 1013: RuduceTargetBuffOnTakeHealthDamage |
| 活力 | MainPackage_Vitality_4 | 0 | 0 | 0 | 1 | Damage=0 | Damage=1×L | 攻击伤害+[Damage*LV] | 1001: IncreaseUnitAttackDamage; 1002: RemoveTargetDamageChange |
| 虚弱 | MainPackage_XuRuo_12 | 1 | 2 | 1 | 1 | DamagePrecent=0.25 | DamagePrecent=0×L | 使攻击的伤害-[DamagePrecent]%<br>回合结束时减少一层 | 1001: IncreaseUnitAttackDamage; 1002: RemoveTargetDamageChange |
| 眩晕 | MainPackage_XuanYun_13 | 1 | 0 | 1 | 1 | P01=0 | P01=1×L | 跳过回合 | 1001: TargetAttributeValueChange; 1002: RemoveAttributeValueChange |
| 硬化 | MainPackage_YingHua_14 | 1 | 0 | 0 | 1 | P01=0 | P01=1×L | 防御力+[P01]<br>受到伤害后减少一层 |  |
| 中毒 | MainPackage_ZhongDu_3 | 1 | 0 | 0 | 0 | Damage=0 | Damage=1×L | 回合开始时：受到 [Damage*LV] 点伤害，之后层数-1 | 1010: DamageSelf,PlayAudio,ReduceLevel |

## 道具：25 条

| 名称 | ID | 类型 | 稀有度 | 基础价格 | 仅战斗 | 自动消耗 | 参数 | 配置描述 | 事件函数 |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| 金币 | Core_Item_101_Coin | -1 | 0 | 0 | 0 | 0 |  | 闪闪亮亮的 |  |
| 铜钥匙 | Core_Item_201_Key01 | -1 | 0 | 0 | 0 | 0 |  | 也许能开启某个锁 |  |
| 银钥匙 | Core_Item_202_Key02 | -1 | 1 | 0 | 0 | 0 |  | 也许能开启某个锁 |  |
| 金钥匙 | Core_Item_203_Key03 | -1 | 2 | 0 | 0 | 0 |  | 也许能开启某个锁 |  |
| 体力药剂 | MainPackage_Item_ActionPoition_1018 | 0 | 0 | 7 | 1 | 1 | P01=1 | 获得[P01]点行动力 | 0: ActionChange |
| 爆裂药剂 | MainPackage_Item_BaoLiePoition_1014 | 0 | 3 | 200 | 1 | 1 | P01=200 | 对指定目标造成[P01]点伤害 | 0: DamageTarget |
| 初级技能石 | MainPackage_Item_ChuJiJiNengShi_1019 | 1 | 1 | 35 | 0 | 0 | P01=1 | 点击使用，提升一个技能的等级（最高普通），或者选择一个初级技能石让其变为中级技能石 | 0: IncreaseTargetSkillLevel |
| 冻结药剂 | MainPackage_Item_DongJiePoition_1021 | 0 | 1 | 10 | 1 | 1 | P01=20 | 为选择目标附加[P01]层{Buff_MainPackage_HanShuang_8_Title} | 0: AddBuffToTarget |
| 虚无药瓶 | MainPackage_Item_EmptyPoition_-1 | 0 | -1 | 0 | 1 | 1 | P01=0 | 空的药瓶，啥效果也没有<br>战斗结束后会从背包里移除 |  |
| 生命药剂 | MainPackage_Item_HealthPoition_1007 | 0 | 1 | 20 | 0 | 1 | P01=0.25 | 恢复最大生命值的[P01]% | 0: RecoverHealth |
| 劣质爆破药剂 | MainPackage_Item_InferiorBoomPoition_1005 | 0 | 0 | 7 | 1 | 1 | P01=25 | 对指定目标造成[P01]点伤害 | 0: DamageTarget |
| 劣质生命药剂 | MainPackage_Item_InferiorHealthPoition_1001 | 0 | 0 | 10 | 0 | 1 | P01=15 | 恢复[P01]点生命值 | 0: RecoverHealth |
| 劣质魔力药剂 | MainPackage_Item_InferiorMagicPoition_1002 | 0 | 0 | 10 | 0 | 1 | P01=20 | 恢复[P01]点魔力值 | 0: RecoverHealth |
| 毒药剂 | MainPackage_Item_InferiorPoisionPoition_1004 | 0 | 1 | 15 | 1 | 1 | P01=5 | 为选择目标附加[P01]层{Buff_MainPackage_ZhongDu_3_Title} | 0: AddBuffToTarget |
| 活力药剂 | MainPackage_Item_InferiorVitalityPoition_1003 | 0 | 1 | 15 | 1 | 1 | P01=2 | 获得[P01]层{Buff_MainPackage_Vitality_4_Title} | 0: AddBuffToSelf |
| 高级技能石 | MainPackage_Item_JiNengPoition_1013 | 1 | 2 | 90 | 0 | 0 | P01=1 | 点击使用，提升一个技能的等级 | 0: IncreaseTargetSkillLevel |
| 冷却药剂 | MainPackage_Item_LengQuePoition_1006 | 0 | 1 | 10 | 0 | 1 | P01=20 | 使当前冷却最长的技能的冷却时间-[P01] | 0: RefreshLongthestSkillCoolDown |
| 力量强化药剂 | MainPackage_Item_LiLiangPoition_1010 | 0 | 2 | 35 | 0 | 1 | P01=1 | 力量永久+[P01] | 0: IncreaseTargetAttribute |
| 龙血药剂 | MainPackage_Item_LongXuePoition_1015 | 0 | 3 | 200 | 0 | 1 | P01=999 | 恢复[P01]点生命值和[P01]点魔力值 | 0: RecoverHealth,RecoverHealth |
| 魔力药剂 | MainPackage_Item_MagicPoition_1008 | 0 | 1 | 20 | 0 | 1 | P01=0.25 | 恢复最大魔力值的[P01]% | 0: RecoverHealth |
| 敏捷强化药剂 | MainPackage_Item_MinJiePoition_1011 | 0 | 2 | 35 | 0 | 1 | P01=1 | 敏捷永久+[P01] | 0: IncreaseTargetAttribute |
| 突破技能石 | MainPackage_Item_TuPoPoition_1016 | 0 | 3 | 150 | 0 | 0 | P01=1 | 提升一个技能的等级，该效果无视等级上限（对等级上限为1的技能无效） | 0: IncreaseTargetSkillLevel |
| 易伤药剂 | MainPackage_Item_YiShangPoition_1009 | 0 | 1 | 15 | 1 | 1 | P01=2 | 为选择目标附加[P01]层{Buff_MainPackage_CuiRuo_1_Title} | 0: AddBuffToTarget |
| 智力强化药剂 | MainPackage_Item_ZhiLiPoition_1012 | 0 | 2 | 35 | 0 | 1 | P01=1 | 智力永久+[P01] | 0: IncreaseTargetAttribute |
| 中级技能石 | MainPackage_Item_ZhongJiJiNengShi_1020 | 1 | 2 | 60 | 0 | 0 | P01=1 | 点击使用，提升一个技能的等级（最高稀有），或者选择一个中级技能石让其变为高级技能石 | 0: IncreaseTargetSkillLevel |

## 符文：25 条

| 名称 | ID | 主被动限制 | 允许上限1技能 | 匹配标签 | 参数 | 配置描述 | 事件函数 |
| --- | --- | --- | --- | --- | --- | --- | --- |
| 轻盈符文 | MainPackage_Rune_10_QingYing | 1 | 1 |  | P01=1; P02=2 | 该技能行动力-[P01]，该技能冷却+[P02] | 1001: Rune_ChangeSkillAction,Rune_AdjustSkillCooldown; 1002: Rune_RemoveSkillAction,Rune_RemoveAdjustSkillCooldown |
| 技巧符文 | MainPackage_Rune_11_JiQiao | 0 | 1 | BaseDamage | P01=1; P02=3 | 该技能伤害次数+1，该技能冷却+[P02] | 1001: Rune_AdjustDamageCount,Rune_AdjustSkillCooldown; 1002: Rune_RemoveAdjustDamageCount,Rune_RemoveAdjustSkillCooldown |
| 暴力符文 | MainPackage_Rune_12_BaoLi | 0 | 1 | BaseDamage | P01=0.5; P02=1 | 该技能造成的伤害+[P01]%，该技能行动力消耗+[P02] | 1001: Rune_ChangeSkillAction,Rune_AdjustSkillDamage_Precent; 1002: Rune_RemoveSkillAction,Rune_RemoveAdjustSkillDamage_Precent |
| 心流符文 | MainPackage_Rune_13_XinLiu | 1 | 1 |  | P01=1 | 该技能的魔力消耗变为0 | 1001: Rune_AdjustCost; 1002: Rune_RemoveAdjustCost |
| 激昂符文 | MainPackage_Rune_14_JiAng | 0 | 1 | SetBuff | P01=2 | 附加状态时，额外附加[P01]层 | 1001: Rune_AdditionBuffAdd; 1002: Rune_RemoveAdditionBuffAdd |
| 幻影符文 | MainPackage_Rune_15_HuanYing | 1 | 1 |  | P01=1; P02=4 | 该技能魔力消耗+[P02]<br>每个回合第一次使用后：刷新该技能冷却 | 1014: Rune_HuanYingRune; 1010: Rune_SetRuneCounter; 1001: Rune_AdjustCost; 1002: Rune_RemoveAdjustCost |
| 沉重符文 | MainPackage_Rune_16_ChenZhong | 1 | 1 | BaseDamage | P01=0.5; P02=5 | 造成的伤害+[P01]%，使用该技能后，当前魔力-[P02] | 0: RecoverConstValue; 1001: Rune_AdjustSkillDamage_Precent; 1002: Rune_RemoveAdjustSkillDamage_Precent; 1014: Rune_ChenZhongRune |
| 流转符文 | MainPackage_Rune_17_LiuZhuan | 1 | 1 |  | P01=10 | 战斗结束时：该技能冷却-[P01] | 1009: ChangeCurSkillCooldown |
| 突破符文 | MainPackage_Rune_18_TuPo | 0 | 0 |  | P01=2 | 该技能等级上限+[P01] | 1001: Rune_AdditionSkillMaxLevel; 1002: Rune_RemoveAdditionMaxLevel |
| 重放符文 | MainPackage_Rune_19_ChongFang | 1 | 1 |  | P01=1 | 每场战斗的第一次使用时：该技能额外释放[P01]次 | 1008: Rune_SetRuneCounter; 1014: Rune_ChongFangRune_19 |
| 迅捷符文 | MainPackage_Rune_1_XunJie | 1 | 1 |  | P01=1 | 每场战斗第一次使用时，该技能行动力消耗为0 | 1008: Rune_SetSkillActionByRune; 1014: Rune_RemoveSkillActionByRuneAfteUseSkill |
| 魔能符文 | MainPackage_Rune_20_MoNeng | 1 | 1 |  | P01=1 | 战斗开始的第一个回合：无消耗的释放一次该技能 | 1008: Rune_SetRuneCounter; 1010: Rune_MoNeng_20 |
| 混乱符文 | MainPackage_Rune_21_HunLuan | 1 | 1 |  | P01=0; P02=3 | 回合开始时：该技能的行动力在[P01]到[P02]变化 | 1010: Rune_HunLuan_20 |
| 渐近符文 | MainPackage_Rune_22_JianJin | 1 | 1 |  | P01=1 | 回合结束时：该技能行动力-[P01]，使用后效果重制 | 1014: Rune_RemoveSkillActionByRuneAfteUseSkill; 1011: Rune_ChangeSkillAction_NotOverride |
| 能量符文 | MainPackage_Rune_23_NengLiang | 1 | 1 |  | P01=2 | 每场战斗第一次使用时：获得[P01]点行动力 | 1008: Rune_SetRuneCounter; 1014: Rune_NengLiang_23 |
| 过量符文 | MainPackage_Rune_24_GuoLiang | 1 | 1 |  | P01=2; P02=6 | 该技能的行动力和魔力消耗为0，使用该技能后，该技能冷却+9，战斗开始时重制冷却 | 1001: Rune_SetSkillActionByRune,Rune_AdjustCost; 1002: Rune_RemoveSkillAction,Rune_RemoveAdjustCost; 1014: Rune_NengLiang_24; 1008: ChangeCurSkillCooldown |
| 鲜血符文 | MainPackage_Rune_25_XianXueFuWen | 1 | 1 |  | P01=1 | 魔力消耗为0，使用该技能后，受到[P01]点真实伤害 | 1014: Rune_XianXue_25; 1001: Rune_AdjustCost; 1002: Rune_RemoveAdjustCost |
| 极速符文 | MainPackage_Rune_2_JiSu | 1 | 1 |  | P01=2 | 回合结束时：该技能冷却额外-[P01] | 1011: ChangeCurSkillCooldown |
| 蓄能符文 | MainPackage_Rune_3_XuNeng | 1 | 1 |  | P01=1 | 回合开始时：如果该主动技能已就绪，则无消耗的使用一次该技能，该技能会正常进入冷却 | 1010: Rune_XuNengRune |
| 力量符文 | MainPackage_Rune_4_LiLiang | 0 | 1 |  | P01=1 | 该技能每有1级，力量+[P01] | 1001: Rune_AttributeSetter; 1002: Rune_AttributeRemove |
| 敏捷符文 | MainPackage_Rune_5_MinJie | 0 | 1 |  | P01=1 | 该技能每有1级，敏捷+[P01] | 1001: Rune_AttributeSetter; 1002: Rune_AttributeRemove |
| 智力符文 | MainPackage_Rune_6_ZhiLiFuWen | 0 | 1 |  | P01=1 | 该技能每有1级，智力+[P01] | 1001: Rune_AttributeSetter; 1002: Rune_AttributeRemove |
| 多重符文 | MainPackage_Rune_7_DuoChong | 1 | 1 |  | P01=1 | 使用时：该主动技能会额外触发一次，该技能行动力+[P01] | 1001: Rune_ChangeSkillAction; 1002: Rune_RemoveSkillAction; 1014: Rune_DuoChongRune |
| 忍耐符文 | MainPackage_Rune_8_RenNai | 0 | 1 | SelfDamage | P01=1 | 该技能对自己造成的伤害-[P01] |  |
| 活力符文 | MainPackage_Rune_9_HuoLi | 0 | 1 | BaseDamage | P01=2 | 该技能每有1级，造成的伤害额外+[P01] | 1001: Rune_AdjustSkillDamage_Base; 1002: Rune_RemoveAdjustSkillDamage_Base |

## 地图：21 条

| 名称 | ID | 区域等级 | 地图类型 | 要求击杀Boss | Boss遗物奖励字段 | 制作中 |
| --- | --- | --- | --- | --- | --- | --- |
| 废弃城市莱尔 | ConstMap_1004_CityLayer | 4 | 0 | 1 | 1 | 0 |
| 森林入口 | ConstMap_2001_TutorialScene | 0 | 2 | 0 | 0 | 0 |
| 深渊(深度：{P01}) | ConstMap_3001_Abyss | 1 | 0 | 1 | -1 | 0 |
| 森林 | ConstMap_TestScene | 0 | 0 | 0 | 0 | 0 |
| 遗迹 | MainPackage_Secret_10011 | 1 | 2 | 0 | 1 | 0 |
| 遗迹 | MainPackage_Secret_10012 | 1 | 2 | 0 | 1 | 0 |
| 遗迹 | MainPackage_Secret_10013 | 1 | 2 | 0 | 1 | 0 |
| 遗迹 | MainPackage_Secret_20011 | 1 | 2 | 0 | 1 | 0 |
| 遗迹 | MainPackage_Secret_20012 | 1 | 2 | 0 | 1 | 0 |
| 遗迹 | MainPackage_Secret_20013 | 1 | 2 | 0 | 1 | 0 |
| 森林 | MapData_1001_RuinsForest | 1 | 0 | 1 | 0 | 0 |
| 分岔路口 | MapData_1001_RuinsForest_Rest | 0 | 1 | 0 | 1 | 0 |
| 分岔路口 | MapData_1002_DeepForest_Rest | 0 | 1 | 0 | 1 | 0 |
| 森林深处 | MapData_1002_DeepOfForest | 2 | 0 | 1 | 1 | 0 |
| 沼泽 | MapData_1003_Swamp | 2 | 0 | 1 | 1 | 0 |
| 分岔路口 | MapData_1003_Swamp_Rest | 0 | 1 | 0 | 1 | 0 |
| 石林 | MapData_1004_StoneForest | 3 | 0 | 1 | 2 | 0 |
| 分岔路口 | MapData_1004_StoneForest_Rest | 0 | 1 | 0 | 1 | 0 |
| 分岔路口 | MapData_1005_StarLake_Rest | 0 | 1 | 0 | 1 | 0 |
| 星空湖 | MapData_1006_StarLake | 3 | 0 | 1 | 2 | 0 |
| 城堡 | MapData_ConstMap_10051_CityCastle | 5 | 2 | 0 | -1 | 0 |
