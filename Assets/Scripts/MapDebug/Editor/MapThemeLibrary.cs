using System.Collections.Generic;
using System.IO;
using Dungeon.Core;
using UnityEditor;
using UnityEngine;

namespace Dungeon.MapDebug.Editor
{
    public static class MapThemeLibrary
    {
        public static void ConfigureLandscape(MapTheme theme)
        {
            bool snow=theme.themeId=="frozen-ridge";
            bool desert=theme.themeId=="dry-desert"||theme.themeId=="oasis-desert";
            theme.aridRocks=desert;
            theme.wallMountain=desert?MountainAppearance.Sandstone:snow?MountainAppearance.SnowCovered:
                theme.themeId=="forest-interior"?MountainAppearance.Wooded:MountainAppearance.BareRock;
            theme.tributaries=0;
            theme.riverSurfaces=desert?new[]{TerrainKind.Sand}:snow?new[]{TerrainKind.Snow}:theme.themeId=="forest-creek"?
                new[]{TerrainKind.Plains,TerrainKind.Woodland,TerrainKind.Forest}:
                new[]{TerrainKind.Plains,TerrainKind.Woodland};
            theme.peakEventCount=theme.themeId=="forest-creek"?1:0;
            theme.peakEventId="snow-peak";
            theme.mountains=System.Array.Empty<ThemeMountain>();
            theme.pondWidth=1.1f;
            theme.lakeMinSize=1;theme.lakeMaxSize=1;
            if(theme.themeId=="forest-lake")
            {
                theme.lakes=3;
                theme.description="森林、疏林与林间空地；三处单格小湖，统一裸岩山地边界。";
            }
        }
        const string Root = "Assets/Scripts/MapDebug/Themes";
        const string Art = "Assets/Arts/Hex and Tile Samples 1.0.1/";
        public static void ConfigureLocations(MapTheme theme)
        {
            bool temperate=theme.themeId.StartsWith("forest-")||theme.themeId=="open-plains";
            bool dense=theme.themeId=="forest-interior";
            ThemeLocation Location(string name,string id,int count,TerrainKind terrain,string hex,string square) => new ThemeLocation {
                displayName=name,contentId=id,count=count,terrain=terrain,
                hexTexture=AssetDatabase.LoadAssetAtPath<Texture2D>(Art+"Hex Samples/Hex Medieval Fantasy Locations/"+hex+".png"),
                squareTexture=AssetDatabase.LoadAssetAtPath<Texture2D>(Art+"Tile Samples/Tile Medieval Fantasy Locations/"+square+".png") };
            theme.locations=new[]{
                Location("村镇","location-town",temperate?1:0,TerrainKind.Plains,"hexDirtVillage00","dirtVillage00"),
                Location("城堡","location-castle",temperate&&!dense?1:0,TerrainKind.Rock,"hexDirtCastle00","dirtCastle00"),
                Location("农场","location-farm",temperate&&!dense?1:0,TerrainKind.Plains,"hexPlainsFarm00","plainsFarm00"),
                Location("伐木营地","location-forester",dense?1:0,TerrainKind.Woodland,"hexForestBroadleafForester00","forestForester00"),
                Location("矿场","location-mine",0,TerrainKind.Rock,"hexHillsMine01","hillsMine00")
            };
        }
        static ThemeLand Land(TerrainKind kind,int weight) => new ThemeLand { terrain=kind,weight=weight };
        static DebugTerrainVisual Visual(TerrainKind kind,string hex,string square) => new DebugTerrainVisual {
            terrain=kind,hexTexture=AssetDatabase.LoadAssetAtPath<Texture2D>(Art+"Hex Samples/"+hex+".png"),
            squareTexture=AssetDatabase.LoadAssetAtPath<Texture2D>(Art+"Tile Samples/"+square+".png") };

        [MenuItem("Dungeon/补齐内置地图主题")]
        public static void Install()
        {
            Directory.CreateDirectory(Root); AssetDatabase.Refresh();
            Create("01", "forest-interior", "密林腹地", "密林与林间空地；无河流、无湖泊。",0,0,2,4,0,
                Land(TerrainKind.Forest,4),Land(TerrainKind.Woodland,4),Land(TerrainKind.Plains,2));
            Create("02", "forest-creek", "林间溪谷", "森林与草地；保留一片小湖，不生成河流。",0,1,2,4,0,
                Land(TerrainKind.Forest,3),Land(TerrainKind.Woodland,4),Land(TerrainKind.Plains,3));
            Create("03", "forest-lake", "湖畔森林", "森林与林间草地；三处单格小湖，无河流。",0,3,1,1,0,
                Land(TerrainKind.Forest,4),Land(TerrainKind.Woodland,3),Land(TerrainKind.Plains,3));
            Create("04", "open-plains", "开阔草原", "大片草地、少量疏林；无河流、无独立湖泊。",0,0,2,4,0,
                Land(TerrainKind.Plains,8),Land(TerrainKind.Woodland,1),Land(TerrainKind.Rock,1));
            Create("05", "dry-desert", "荒芜沙海", "沙丘与裸岩；无水域、无森林、无雪峰。",0,0,2,4,1,
                Land(TerrainKind.Sand,8),Land(TerrainKind.Rock,2));
            Create("06", "oasis-desert", "绿洲沙漠", "沙丘与岩地；两片小型绿洲湖，无河流。",0,2,2,3,1,
                Land(TerrainKind.Sand,8),Land(TerrainKind.Rock,2));
            Create("07", "frozen-ridge", "冰封雪岭", "雪地、雪林与冰原；雪峰边界与一片冰湖。",0,1,3,6,2,
                Land(TerrainKind.Snow,6),Land(TerrainKind.SnowForest,3),Land(TerrainKind.Ice,1));
            var preset=AssetDatabase.LoadAssetAtPath<MapDebugPreset>("Assets/Scripts/MapDebug/Presets/DefaultMapDebug.asset");
            if(preset!=null&&preset.theme==null) { preset.theme=AssetDatabase.LoadAssetAtPath<MapTheme>(Root+"/01-forest-interior.asset"); EditorUtility.SetDirty(preset); }
            AssetDatabase.SaveAssets();
        }

        static void Create(string order,string id,string title,string description,int rivers,int lakes,int min,int max,int climate,params ThemeLand[] land)
        {
            string path=Root+"/"+order+"-"+id+".asset";
            if(AssetDatabase.LoadAssetAtPath<MapTheme>(path)!=null)return;
            var theme=ScriptableObject.CreateInstance<MapTheme>();
            theme.themeId=id;theme.displayName=title;theme.description=description;theme.land=land;
            theme.rivers=rivers;theme.lakes=lakes;theme.lakeMinSize=min;theme.lakeMaxSize=max;
            theme.patchSize=16;
            theme.waterColor=climate==2?new Color(.58f,.77f,.83f):new Color(.23f,.50f,.53f);
            var visuals=new List<DebugTerrainVisual> {
                Visual(TerrainKind.Plains,"Hex Basic Terrain Set/hexPlains00","Tile Basic Terrain Set/plains00"),
                Visual(TerrainKind.Woodland,"Hex Basic Terrain Set/hexWoodlands00","Tile Basic Terrain Set/woodlands00"),
                Visual(TerrainKind.Forest,"Hex Basic Terrain Set/hexForestBroadleaf00","Tile Basic Terrain Set/forestBroadleaf00"),
                Visual(TerrainKind.Sand,"Hex Basic Terrain Set/hexDesertDunes00","Tile Basic Terrain Set/desertDunes00"),
                Visual(TerrainKind.Rock,"Hex Basic Terrain Set/hexHighlands00","Tile Basic Terrain Set/highlands00"),
                Visual(TerrainKind.Snow,"Hex Cold Lands/hexSnowField00","Tile Cold Lands/snowField00"),
                Visual(TerrainKind.SnowForest,"Hex Cold Lands/hexForestPineSnowCovered00","Tile Cold Lands/forestPineSnowCovered00"),
                Visual(TerrainKind.Ice,"Hex Cold Lands/hexSnowField00","Tile Cold Lands/snowField00"),
                Visual(TerrainKind.Mountain,climate==2?"Hex Cold Lands/hexMountainSnow00":climate==1?"Hex Deserts Terrain/hexDesertYellowMesas00":"Hex Basic Terrain Set/hexHills00",
                    climate==2?"Tile Cold Lands/mountainSnow00":climate==1?"Tile Deserts/desertYellowMesaLarge00":"Tile Basic Terrain Set/hills00"),
                Visual(TerrainKind.Lake,"Hex Rivers Coasts & Seas/hexOceanCalm00","Tile River Coasts & Seas/oceanCalm00"),
                Visual(TerrainKind.River,"Hex Basic Terrain Set/hexPlains00","Tile Basic Terrain Set/plains00"),
                Visual(TerrainKind.Ford,"Hex Basic Terrain Set/hexPlains00","Tile Basic Terrain Set/plains00")
            };
            theme.visuals=visuals.ToArray();
            ConfigureLandscape(theme);
            ConfigureLocations(theme);
            AssetDatabase.CreateAsset(theme,path);
        }
    }
}
