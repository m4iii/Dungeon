using System;
using System.Collections.Generic;
using System.Linq;
using Dungeon.Core;
using UnityEngine;

namespace Dungeon.MapDebug
{
    public enum DebugGrid { Hex, Square }

    [Serializable]
    public sealed class DebugTerrainVisual
    {
        public TerrainKind terrain;
        public Texture2D hexTexture;
        public Texture2D squareTexture;
    }

    [Serializable]
    public sealed class DebugItemAmount
    {
        public string id;
        [Min(1)] public int amount = 1;
    }

    [Serializable]
    public sealed class DebugCellContent
    {
        public CellKind kind;
        public bool overrideTerrain;
        public TerrainKind terrain;
        public string[] enemies = Array.Empty<string>();
        [Min(0)] public int coins;
        public DebugItemAmount[] items = Array.Empty<DebugItemAmount>();
        public string requiredKey;
        public string contentId;

        public CellTemplate ToCore()
        {
            var rewards = new Dictionary<string, int>(StringComparer.Ordinal);
            foreach (var item in items ?? Array.Empty<DebugItemAmount>())
            {
                if (item == null || string.IsNullOrWhiteSpace(item.id) || item.amount <= 0)
                    throw new ArgumentException("奖励道具需要有效 ID 和正数数量。");
                if (rewards.ContainsKey(item.id)) throw new ArgumentException("奖励道具 ID 重复：" + item.id);
                rewards.Add(item.id, item.amount);
            }
            return new CellTemplate(kind, enemies, new RewardBundle(coins, rewards),
                string.IsNullOrWhiteSpace(requiredKey) ? null : requiredKey,
                string.IsNullOrWhiteSpace(contentId) ? null : contentId, overrideTerrain ? terrain : (TerrainKind?)null);
        }
    }

    [Serializable]
    public sealed class DebugWeightedCell
    {
        [Min(0)] public int weight = 1;
        public DebugCellContent content = new DebugCellContent();
    }

    [Serializable]
    public sealed class DebugFixedCell
    {
        public Vector2Int position;
        public DebugCellContent content = new DebugCellContent();
    }

    [Serializable]
    public sealed class DebugRequiredCell
    {
        [Min(0)] public int count = 1;
        public DebugCellContent content = new DebugCellContent { kind = CellKind.Reward, coins = 10 };
    }

    /// <summary>Unity authoring data. Conversion creates independent Core definitions on every request.</summary>
    [CreateAssetMenu(menuName = "Dungeon/地图调试配置", fileName = "MapDebugPreset")]
    public sealed class MapDebugPreset : ScriptableObject
    {
        [InspectorName("地图主题")] public MapTheme theme;
        [Header("地图设置（空间根据格数自动分配；六边形使用 q/r）")]
        public string mapId = "debug-map";
        public DebugGrid grid = DebugGrid.Hex;
        public Vector2Int entrance = new Vector2Int(12, 2);
        [Header("随机形状")]
        [InspectorName("不规则连通地图")] public bool irregularShape = true;
        [Min(2), InspectorName("地图格数（房间模式为最低可通行数）"), Tooltip("生成空间自动分配。房间模式为最低可通行数；旧扩展模式为可通行数；完整网格模式为总格数。外围阻挡圈另计。")]
        public int cellCount = 60;
        [Range(0, 1), InspectorName("紧凑度（旧扩展模式）"), Tooltip("仅用于关闭主路房间模式后的连通扩展。")]
        public float compactness = .2f;
        [Range(0, 1), InspectorName("分支倾向"), Tooltip("房间模式：低值优先沿主路开房间，高值也从已有房间扩展分支。")]
        public float branchChance = .4f;
        [Header("主路与分支房间（需要不规则模式）")]
        public bool roadRoomLayout = true;
        [Min(1)] public int mainRoadLength = 12;
        [Range(1, 4)] public int exitCount = 1;
        [Range(2, 32)] public int minRoomSize = 5;
        [Range(2, 32)] public int maxRoomSize = 9;
        [Range(0, .95f)] public float emptyRatio = .45f;
        [Tooltip("额外保证放入的数量，不包含固定坐标内容。必放内容优先于空地比例。")]
        public DebugRequiredCell[] requiredContent = Array.Empty<DebugRequiredCell>();
        [Header("地形与阻挡边界")]
        public bool generateTerrain = true;
        [Range(0, 1), Tooltip("阻挡区域成为深水的概率，其余为高山；按地形片区抽取，不是精确面积比例。")]
        public float waterChance = .45f;
        [Range(1, 64), Tooltip("越大越倾向大片同类地形。")]
        public int terrainPatchSize = 12;
        public bool shallowShores = true;
        [Min(0)] public int plainsWeight = 5;
        [Min(0)] public int woodlandWeight = 2;
        [Min(0)] public int forestWeight = 3;
        [Tooltip("可选的地形图片覆盖；留空时使用 Assets/Arts 中的同系列样包。图片使用原包的 256×384 布局。")]
        public DebugTerrainVisual[] terrainVisuals = Array.Empty<DebugTerrainVisual>();
        [Tooltip("房间模式先确定主路出口；其他模式选择远端格子。自定义出口时关闭此项。")]
        public bool placeExit = true;
        public bool requireBossForExit;
        [Range(0, 1)] public float restRatio = 1;

        [Header("随机地块池（权重 0 表示关闭）")]
        [Tooltip("不规则模式只抽取可通行内容。阻挡圈和内部阻挡地形在连通区域生成后补齐。")]
        public DebugWeightedCell[] pool =
        {
            new DebugWeightedCell { weight = 50, content = new DebugCellContent { kind = CellKind.Empty } },
            new DebugWeightedCell { weight = 30, content = new DebugCellContent { kind = CellKind.Battle, enemies = new[] { "debug-wolf" } } },
            new DebugWeightedCell { weight = 15, content = new DebugCellContent { kind = CellKind.Reward, coins = 10 } },
            new DebugWeightedCell { weight = 5, content = new DebugCellContent { kind = CellKind.Rest } },
            new DebugWeightedCell { weight = 0, content = new DebugCellContent { kind = CellKind.Blocked } }
        };

        [Header("固定地块（不包含入口）")]
        public DebugFixedCell[] fixedCells = Array.Empty<DebugFixedCell>();

        public IMapTopology CreateTopology() => grid == DebugGrid.Hex ? (IMapTopology)new HexTopology() : new SquareTopology();

        public MapGenerationDefinition ToCore()
        {
            if (!Enum.IsDefined(typeof(DebugGrid), grid)) throw new ArgumentException("未知网格类型。");
            if (cellCount < 2) throw new ArgumentException("地图格数至少为 2。");
            var fixedDefinitions = new List<CellDefinition>();
            foreach (var cell in fixedCells ?? Array.Empty<DebugFixedCell>())
            {
                if (cell?.content == null) throw new ArgumentException("固定地块不能留空。");
                fixedDefinitions.Add(cell.content.ToCore().At(new CellPosition(cell.position.x, cell.position.y)));
            }
            var weighted = new List<WeightedEntry<CellTemplate>>();
            foreach (var entry in pool ?? Array.Empty<DebugWeightedCell>())
            {
                if (entry == null || entry.weight < 0) throw new ArgumentException("随机池条目不能为空，权重不能为负数。");
                if (entry.weight == 0) continue;
                if (entry.content == null) throw new ArgumentException("随机池地块内容不能为空。");
                weighted.Add(new WeightedEntry<CellTemplate>(entry.content.ToCore(), entry.weight));
            }
            var land = new List<WeightedEntry<TerrainKind>>();
            void AddLand(TerrainKind kind, int weight)
            {
                if (weight < 0) throw new ArgumentException("地形权重不能为负数。");
                if (weight > 0) land.Add(new WeightedEntry<TerrainKind>(kind, weight));
            }
            if(theme == null)
            { AddLand(TerrainKind.Plains, plainsWeight); AddLand(TerrainKind.Woodland, woodlandWeight); AddLand(TerrainKind.Forest, forestWeight); }
            var required = new List<RequiredMapContent>();
            if(theme!=null)
                foreach(var location in theme.locations ?? Array.Empty<ThemeLocation>())
                {
                    if(location==null||location.count<0)throw new ArgumentException("地点配置或数量无效。");
                    if(location.count==0)continue;
                    if(!irregularShape||!roadRoomLayout)throw new ArgumentException("主题地点需要主路房间布局。");
                    if((grid==DebugGrid.Hex?location.hexTexture:location.squareTexture)==null)
                        throw new ArgumentException("地点缺少当前网格的素材："+location.displayName);
                    required.Add(new RequiredMapContent(new CellTemplate(CellKind.Event,contentId:location.contentId,terrain:location.terrain),location.count));
                }
            if(theme != null && theme.peakEventCount > 0)
            {
                if(!irregularShape || !roadRoomLayout)throw new ArgumentException("主题雪峰事件需要主路房间布局。");
                required.Add(new RequiredMapContent(new CellTemplate(CellKind.Event,contentId:theme.peakEventId),theme.peakEventCount));
            }
            if (irregularShape && roadRoomLayout)
                foreach (var entry in requiredContent ?? Array.Empty<DebugRequiredCell>())
                {
                    if (entry == null || entry.count < 0) throw new ArgumentException("必放内容不能为空且数量不能为负。");
                    if (entry.count == 0) continue;
                    if (entry.content == null) throw new ArgumentException("必放内容缺少地块配置。");
                    required.Add(new RequiredMapContent(entry.content.ToCore(), entry.count));
                }
            var origin = new CellPosition(entrance.x, entrance.y);
            var rooms = irregularShape && roadRoomLayout ? new RoadRoomDefinition(mainRoadLength, exitCount, minRoomSize, maxRoomSize, emptyRatio, required) : null;
            var positions = irregularShape ? AutomaticMapArea.Create(cellCount, origin, fixedDefinitions, rooms)
                : AutomaticMapArea.Compact(cellCount, origin, fixedDefinitions, CreateTopology());
            if (placeExit && !irregularShape)
            {
                var occupied = new HashSet<CellPosition>(fixedDefinitions.Select(c => c.Position)) { origin };
                var available = positions.Where(p => !occupied.Contains(p)).ToList();
                if (available.Count == 0) throw new ArgumentException("没有空位放置自动出口，请增加格数或减少固定地块。");
                var exit = available.OrderByDescending(p => Math.Abs((long)p.X-origin.X) + Math.Abs((long)p.Y-origin.Y)).First();
                fixedDefinitions.Add(new CellDefinition(exit, CellKind.Exit));
            }
            return new MapGenerationDefinition(mapId, positions, origin, fixedDefinitions,
                new WeightedPool<CellTemplate>(weighted), requireBossForExit, restRatio,
                irregularShape ? new MapShapeDefinition(cellCount, compactness, branchChance, placeExit) : null,
                theme != null ? theme.ToCore() : generateTerrain ? new TerrainGenerationDefinition(waterChance, terrainPatchSize, new WeightedPool<TerrainKind>(land), shallowShores) : null,
                rooms, automaticArea: irregularShape);
        }
    }
}
