using System;
using System.Linq;
using Dungeon.Core;
using UnityEngine;

namespace Dungeon.MapDebug
{
    public enum MountainAppearance
    {
        [InspectorName("裸岩山地")] BareRock,
        [InspectorName("林木山地")] Wooded,
        [InspectorName("峰顶积雪（绿色山脚）")] SnowCapped,
        [InspectorName("全覆雪山（雪地山脚）")] SnowCovered,
        [InspectorName("使用主题指定素材")] ThemeTexture,
        [InspectorName("低矮砂岩山地")] Sandstone
    }
    [Serializable]
    public sealed class ThemeMountain
    {
        public MountainAppearance appearance;
        [Min(0)] public int weight=1;
    }
    [Serializable]
    public sealed class ThemeLand
    {
        public TerrainKind terrain;
        [Min(0)] public int weight = 1;
    }

    [CreateAssetMenu(menuName = "Dungeon/地图主题", fileName = "MapTheme")]
    public sealed class MapTheme : ScriptableObject
    {
        public string themeId;
        public string displayName;
        [TextArea] public string description;
        [Tooltip("按片区抽取的相对权重，不保证每张地图精确百分比。")]
        public ThemeLand[] land = Array.Empty<ThemeLand>();
        [Min(1),Tooltip("地形片区尺度，越大越连贯，越小变化越多。")] public int patchSize = 16;
        [Tooltip("以连续区域生成地形，并在草地与密林之间形成疏林过渡。")] public bool continuousRegions=true;
        [Min(0)] public int rivers;
        [Range(4,64),Tooltip("优先寻找穿过地图内部的长河道；空间受限时取可用的较长路线，不覆盖内容格。")] public int riverTargetLength=12;
        [Tooltip("河流允许经过的低地底图；手工山地、水域、入口和内容格禁止覆盖。自动生成的内部阻挡区可预留为河谷，外围只允许末端朝外出水。")]
        public TerrainKind[] riverSurfaces = {TerrainKind.Plains,TerrainKind.Woodland,TerrainKind.Sand,TerrainKind.Snow};
        [HideInInspector] public int tributaries;
        [Min(0)] public int lakes;
        [Min(1)] public int lakeMinSize = 2;
        [Min(1)] public int lakeMaxSize = 5;
        public Color waterColor = new Color(.22f,.48f,.51f);
        public DebugTerrainVisual[] visuals = Array.Empty<DebugTerrainVisual>();
        [Tooltip("将岩地显示为干燥砂岩色，避免沙漠使用带苔藓的绿色岩地。")] public bool aridRocks;
        [Header("城镇与特殊地点")]
        public ThemeLocation[] locations = Array.Empty<ThemeLocation>();
        [HideInInspector] public ThemeMountain[] mountains=Array.Empty<ThemeMountain>();
        [Header("统一的阻挡山地")]
        public MountainAppearance wallMountain=MountainAppearance.BareRock;
        [Tooltip("可选：覆盖阻挡地块素材，采用原包 2:3 画布布局。")] public Texture2D wallHexTexture;
        public Texture2D wallSquareTexture;
        [Tooltip("低矮六边形素材按地面轮廓裁剪，避免生成图边缘柔光漏到邻格。")] public bool clipWallToHex;
        [Header("独立雪峰事件格（不属于阻挡墙）")]
        [Min(0)] public int peakEventCount;
        [Tooltip("事件内容 ID，正式运行时需在事件目录中提供对应定义。")] public string peakEventId="snow-peak";
        [Range(.5f,1.4f),Tooltip("小湖在单格内的宽度；完整格宽为 2，同时限制不超过素材原生像素尺寸。")] public float pondWidth=1.1f;

        public TerrainGenerationDefinition ToCore()
        {
            if (string.IsNullOrWhiteSpace(themeId) || string.IsNullOrWhiteSpace(displayName)) throw new ArgumentException("主题需要唯一标识和名称。");
            if (land == null || land.Any(x => x == null || x.weight < 0)) throw new ArgumentException("主题地形权重无效。");
            var ids=new System.Collections.Generic.HashSet<string>(StringComparer.Ordinal);
            if(peakEventCount>0)ids.Add(peakEventId);
            foreach(var location in locations ?? Array.Empty<ThemeLocation>())
            {
                if(location==null||location.count<0)throw new ArgumentException("地点配置不能为空，数量不能为负数。");
                if(location.count==0)continue;
                if(string.IsNullOrWhiteSpace(location.displayName)||string.IsNullOrWhiteSpace(location.contentId)||!ids.Add(location.contentId))
                    throw new ArgumentException("地点需要名称和不重复的交互 ID。");
                if(!Enum.IsDefined(typeof(TerrainKind),location.terrain)||!TerrainRules.IsWalkable(location.terrain)||TerrainRules.IsWater(location.terrain))
                    throw new ArgumentException("城镇等地点需要可进入的陆地底图。");
            }
            return new TerrainGenerationDefinition(0, patchSize,
                new WeightedPool<TerrainKind>(land.Where(x => x.weight > 0).Select(x => new WeightedEntry<TerrainKind>(x.terrain,x.weight))),
                false, new InlandWaterDefinition(rivers,lakes,lakeMinSize,lakeMaxSize,0,riverSurfaces,riverTargetLength),continuousRegions);
        }
    }

    [Serializable]
    public sealed class ThemeLocation
    {
        public string displayName;
        [Tooltip("独立事件 ID；交易、任务等具体交互需另行配置。")] public string contentId;
        [Min(0)] public int count;
        public TerrainKind terrain=TerrainKind.Plains;
        [Tooltip("与地形相同的整格素材，包含地面与建筑。")] public Texture2D hexTexture;
        public Texture2D squareTexture;
    }
}
