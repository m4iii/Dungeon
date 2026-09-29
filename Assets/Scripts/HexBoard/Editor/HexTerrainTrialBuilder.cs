using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Dungeon.HexBoard.Editor
{
    public static class HexTerrainTrialBuilder
    {
        const string Root = "Assets/HexBoard/TerrainTrial";
        static Sprite Import(string name, Vector2 pivot, float ppu)
        {
            string path=Root+"/Art/"+name+".png";
            var importer=(TextureImporter)AssetImporter.GetAtPath(path);
            importer.textureType=TextureImporterType.Sprite;
            importer.spriteImportMode=SpriteImportMode.Single;
            importer.spritePixelsPerUnit=ppu;
            importer.mipmapEnabled=false;
            importer.alphaIsTransparency=true;
            importer.textureCompression=TextureImporterCompression.Uncompressed;
            var settings=new TextureImporterSettings(); importer.ReadTextureSettings(settings);
            settings.spriteAlignment=(int)SpriteAlignment.Custom;
            settings.spritePivot=pivot; settings.spriteMeshType=SpriteMeshType.FullRect;
            importer.SetTextureSettings(settings); importer.SaveAndReimport();
            return AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }
        static HexTerrainDefinition Definition(string name, Sprite prop, int count, float scale, Color tint)
        {
            var mat=new Material(Shader.Find("Dungeon/Trial Terrain Surface"));
            mat.SetTexture("_ArtTex",AssetDatabase.LoadAssetAtPath<Texture2D>(Root+"/Art/hexPlains00.png"));
            mat.SetColor("_Tint",tint);
            AssetDatabase.CreateAsset(mat,Root+"/"+name+".mat");
            var data=ScriptableObject.CreateInstance<HexTerrainDefinition>();
            data.surfaceMaterial=mat; data.decoration=prop; data.decorationCount=count; data.decorationScale=scale;
            AssetDatabase.CreateAsset(data,Root+"/"+name+".asset");
            return data;
        }
        static void Label(string text, Vector3 position, float size, Color color)
        {
            var label=new GameObject(text).AddComponent<TextMesh>();
            label.text=text; label.fontSize=80; label.characterSize=size; label.anchor=TextAnchor.MiddleCenter;
            label.color=color; label.transform.position=position;
            label.GetComponent<MeshRenderer>().sortingOrder=31000;
        }
        [MenuItem("Tools/Hex Board/Create Terrain Trial")]
        public static void Create()
        {
            if (AssetDatabase.LoadAssetAtPath<SceneAsset>("Assets/Scenes/HexTerrainTrial.unity") != null)
                throw new System.InvalidOperationException("Trial scene already exists. Open it to preserve edits.");
            var grass=Import("hexPlains00",new Vector2(.5f,1f/3f),128);
            var forest=Import("hexForestBroadleaf00",new Vector2(.5f,1f/3f),128);
            var mountain=Import("hexMountain00",new Vector2(.5f,1f/3f),128);
            var tree=Import("treesA_cluster00",new Vector2(.5f,.08f),100);
            var lake=Import("lakeSmall03",new Vector2(.5f,.5f),140);
            var plain=Definition("01 Meadow",null,0,1,new Color(1.12f,1.12f,1.15f));
            var woods=Definition("02 Woodland",tree,2,.9f,new Color(1.04f,1.08f,1.1f));
            var dense=Definition("03 Forest",tree,3,1,new Color(.98f,1.04f,1.07f));
            var pond=Definition("04 Pond",lake,1,.9f,new Color(1.08f,1.12f,1.14f));
            // Preserve all pending board changes before opening a separate trial scene.
            EditorSceneManager.SaveOpenScenes();
            var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);
            // Opening a scene can unload assets referenced only by local variables.
            plain=AssetDatabase.LoadAssetAtPath<HexTerrainDefinition>(Root+"/01 Meadow.asset");
            woods=AssetDatabase.LoadAssetAtPath<HexTerrainDefinition>(Root+"/02 Woodland.asset");
            dense=AssetDatabase.LoadAssetAtPath<HexTerrainDefinition>(Root+"/03 Forest.asset");
            pond=AssetDatabase.LoadAssetAtPath<HexTerrainDefinition>(Root+"/04 Pond.asset");
            var camera=new GameObject("Terrain trial camera").AddComponent<Camera>();
            camera.tag="MainCamera"; camera.orthographic=true; camera.orthographicSize=6.2f;
            camera.transform.position=new Vector3(0,.25f,-10);
            camera.clearFlags=CameraClearFlags.SolidColor; camera.backgroundColor=new Color(.055f,.09f,.13f);
            var mat=AssetDatabase.LoadAssetAtPath<Material>("Assets/HexBoard/Art/HexUnlit.mat");
            var prefab=AssetDatabase.LoadAssetAtPath<GameObject>("Assets/HexBoard/Prefabs/BaseHexCell.prefab");
            var board=new GameObject("A - Modular terrain (shared base)");
            board.transform.position=new Vector3(-5,0,0);
            board.transform.localScale=Vector3.one*.78f;
            int index=0;
            for(int q=-2;q<=2;q++) for(int r=-2;r<=2;r++)
            {
                if(Mathf.Abs(q+r)>2) continue;
                var cell=(GameObject)PrefabUtility.InstantiatePrefab(prefab,board.transform);
                cell.name="Trial hex ["+q+", "+r+"]";
                cell.transform.localPosition=HexGeometry.Position(q,r);
                cell.GetComponent<HexCell>().axial=new Vector2Int(q,r);
                cell.GetComponent<HexCell>().layoutVersion=1;
                cell.GetComponent<HexCellThickness>().thickness=.55f;
                cell.GetComponent<HexCellThickness>().Rebuild();
                var recipe=cell.AddComponent<HexTerrainRecipe>();
                recipe.terrain=q==1&&r==-1?pond:q<0?(r==0?dense:woods):plain;
                recipe.seed=7231+index++*31; recipe.decorationMaterial=mat; recipe.Rebuild();
                cell.GetComponent<HexCellSorting>().Refresh();
                PrefabUtility.RecordPrefabInstancePropertyModifications(cell.transform);
                PrefabUtility.RecordPrefabInstancePropertyModifications(cell.GetComponent<HexCell>());
                PrefabUtility.RecordPrefabInstancePropertyModifications(cell.GetComponent<SpriteRenderer>());
                PrefabUtility.RecordPrefabInstancePropertyModifications(cell.GetComponent<HexCellThickness>());
            }
            var originals=new GameObject("B - Original artwork (native proportions)");
            originals.transform.position=new Vector3(5,-.25f,0);
            for(int q=-1;q<=1;q++) for(int r=-1;r<=1;r++)
            {
                if(Mathf.Abs(q+r)>1)continue;
                var renderer=new GameObject("Original ["+q+", "+r+"]").AddComponent<SpriteRenderer>();
                renderer.transform.SetParent(originals.transform,false);
                renderer.transform.localPosition=new Vector3(2*q+r,1.5f*r,0);
                renderer.sprite=r==1?mountain:q<0?forest:grass;
                renderer.sharedMaterial=mat; renderer.sortingOrder=Mathf.RoundToInt(-renderer.transform.position.y*100);
                renderer.spriteSortPoint=SpriteSortPoint.Pivot;
            }
            var white=new Color(.85f,.9f,.93f);
            Label("TERRAIN / COMPOSITION STUDY",new Vector3(0,5,0),.075f,white);
            Label("A   SHARED HEX BASE",new Vector3(-5,3.95f,0),.055f,white);
            Label("B   ORIGINAL TILE ART",new Vector3(5,3.95f,0),.055f,white);
            Label("19 cells / 1 ground + 1 tree cluster + 1 pond\nReusable recipes / adjustable thickness",new Vector3(-5,-4.4f,0),.042f,white);
            Label("7 cells / grass + forest + mountain\nNative shape / baked terrain",new Vector3(5,-4.4f,0),.042f,white);
            Label("Sample artwork: David Baumgart  |  Style and assembly evaluation",new Vector3(0,-5.1f,0),.05f,new Color(.55f,.65f,.7f));
            EditorSceneManager.SaveScene(scene,"Assets/Scenes/HexTerrainTrial.unity");
            AssetDatabase.SaveAssets();
            Selection.activeGameObject=board;
        }
        public static string Capture()
        {
            // Immediate editor captures can precede Unity's normal sorting-group update.
            UnityEngine.Rendering.SortingGroup.UpdateAllSortingGroups();
            var camera=Camera.main; var previous=camera.targetTexture;
            var rt=new RenderTexture(1600,900,24);
            camera.targetTexture=rt; camera.Render();
            var old=RenderTexture.active; RenderTexture.active=rt;
            var image=new Texture2D(1600,900,TextureFormat.RGB24,false);
            image.ReadPixels(new Rect(0,0,1600,900),0,0); image.Apply();
            Directory.CreateDirectory("Temp/HexBoard");
            const string path="Temp/HexBoard/TerrainTrial.png";
            File.WriteAllBytes(path,image.EncodeToPNG());
            camera.targetTexture=previous; RenderTexture.active=old;
            Object.DestroyImmediate(image); Object.DestroyImmediate(rt);
            return Path.GetFullPath(path);
        }
    }
}
