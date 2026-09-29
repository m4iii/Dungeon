using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Dungeon.HexBoard.Editor
{
    public static class HexBoardBuilder
    {
        const string Folder = "Assets/HexBoard";
        static readonly Vector2[] Outline = MakeHex(1);
        static Vector2[] MakeHex(float scale)
        {
            return HexGeometry.Outline(scale);
        }
        static bool Inside(Vector2 p, Vector2[] polygon)
        {
            bool inside = false;
            for (int i = 0, j = polygon.Length - 1; i < polygon.Length; j = i++)
                if ((polygon[i].y > p.y) != (polygon[j].y > p.y) &&
                    p.x < (polygon[j].x - polygon[i].x) * (p.y - polygon[i].y) / (polygon[j].y - polygon[i].y) + polygon[i].x)
                    inside = !inside;
            return inside;
        }
        static Sprite Tile(string name, Color top, int state)
        {
            // Render at twice the former resolution to keep diagonal silhouettes clean.
            // Top-only art: the texture pivot and root anchor are both the top-face center.
            const int w = 512, h = 512;
            const float pixelsPerUnit = 200f;
            var texture = new Texture2D(w, h, TextureFormat.RGBA32, false);
            var pixels = new Color[w * h];
            var inset = MakeHex(state == 0 ? .981f : .945f);
            for (int y = 0; y < h; y++) for (int x = 0; x < w; x++)
            {
                Vector2 p = new Vector2((x + .5f - w * .5f) / pixelsPerUnit, (y + .5f - h * .5f) / pixelsPerUnit);
                Color c = Color.clear;
                if (Inside(p, Outline))
                {
                    c = state == 2 ? new Color(.62f,.37f,.16f) : state == 1 ? new Color(.94f,.87f,.63f) : new Color(.56f,.55f,.42f);
                    if (Inside(p,inset))
                    {
                        c = state == 0 ? top : Color.clear;
                    }
                    if (state == 0 || !Inside(p,inset)) c.a = 1;
                }
                pixels[y * w + x] = c;
            }
            texture.SetPixels(pixels); texture.Apply();
            string path = Folder + "/Art/" + name + ".png";
            File.WriteAllBytes(path, texture.EncodeToPNG());
            Object.DestroyImmediate(texture);
            AssetDatabase.ImportAsset(path);
            var importer = (TextureImporter)AssetImporter.GetAtPath(path);
            importer.textureType = TextureImporterType.Sprite;
            importer.spritePixelsPerUnit = pixelsPerUnit;
            importer.mipmapEnabled = false;
            importer.filterMode = FilterMode.Bilinear;
            importer.spriteImportMode = SpriteImportMode.Single;
            var settings = new TextureImporterSettings(); importer.ReadTextureSettings(settings);
            settings.spriteAlignment = (int)SpriteAlignment.Custom;
            settings.spritePivot = new Vector2(.5f, .5f);
            settings.spriteMeshType = SpriteMeshType.FullRect;
            importer.SetTextureSettings(settings);
            importer.alphaIsTransparency = true;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.SaveAndReimport();
            return AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }
        static int Distance(int q,int r,int cq,int cr)
        { return Mathf.Max(Mathf.Abs(q-cq),Mathf.Abs(r-cr),Mathf.Abs(q+r-cq-cr)); }

        public static void MigratePointedLayout()
        {
            EditorSceneManager.SaveOpenScenes();
            string activePath=SceneManager.GetActiveScene().path;
            Tile("BaseHex",new Color(.76f,.74f,.67f),0);
            Tile("HoverOutline",Color.white,1);
            Tile("SelectionOutline",Color.white,2);
            const string prefabPath=Folder+"/Prefabs/BaseHexCell.prefab";
            var prefab=PrefabUtility.LoadPrefabContents(prefabPath);
            try
            {
                prefab.GetComponent<PolygonCollider2D>().points=HexGeometry.Outline();
                prefab.GetComponent<HexCellThickness>().Rebuild();
                PrefabUtility.SaveAsPrefabAsset(prefab,prefabPath);
            }
            finally { PrefabUtility.UnloadPrefabContents(prefab); }
            foreach(var path in new[]{"Assets/Scenes/HexBoard2D.unity","Assets/Scenes/HexTerrainTrial.unity"})
            {
                var scene=EditorSceneManager.OpenScene(path);
                bool changed=false;
                foreach(var cell in Object.FindObjectsByType<HexCell>())
                {
                    if(cell.layoutVersion!=1)
                    {
                        int q=cell.axial.x,r=cell.axial.y;
                        var old=new Vector3(1.5f*q,Mathf.Sqrt(3)*.78f*(r+q*.5f),0);
                        cell.transform.localPosition+=HexGeometry.Position(q,r)-old;
                        cell.layoutVersion=1;
                        changed=true;
                    }
                    cell.GetComponent<PolygonCollider2D>().points=HexGeometry.Outline();
                    cell.GetComponent<HexCellThickness>().Rebuild();
                    cell.GetComponent<HexCellSorting>().Refresh();
                    PrefabUtility.RecordPrefabInstancePropertyModifications(cell);
                    PrefabUtility.RecordPrefabInstancePropertyModifications(cell.transform);
                    PrefabUtility.RecordPrefabInstancePropertyModifications(cell.GetComponent<PolygonCollider2D>());
                }
                if(changed && path.Contains("Trial"))
                    GameObject.Find("A - Modular terrain (shared base)").transform.localScale=Vector3.one*.78f;
                if(changed && path.Contains("HexBoard2D"))
                {
                    var camera=Camera.main;
                    camera.transform.position=new Vector3(0,-1.5f,-10);
                    camera.orthographicSize=17.5f;
                    foreach(var input in Object.FindObjectsByType<HexBoardInput>()) input.minimumViewSize=17.5f;
                }
                EditorSceneManager.SaveScene(scene);
            }
            AssetDatabase.SaveAssets();
            EditorSceneManager.OpenScene(activePath);
        }

        static Material TopMaterial()
        {
            const string path=Folder+"/Art/HexAnimeTop.mat";
            var material=AssetDatabase.LoadAssetAtPath<Material>(path);
            if(material==null)
            {
                material=new Material(Shader.Find("Dungeon/Anime Hex Top"));
                AssetDatabase.CreateAsset(material,path);
            }
            material.SetTexture("_ArtTex",AssetDatabase.LoadAssetAtPath<Texture2D>(Folder+"/Art/AnimeBaseSurface.png"));
            EditorUtility.SetDirty(material);
            return material;
        }
        public static void ApplyAnimeStyle()
        {
            var material=TopMaterial();
            string path=Folder+"/Prefabs/BaseHexCell.prefab";
            var root=PrefabUtility.LoadPrefabContents(path);
            try
            {
                root.GetComponent<SpriteRenderer>().sharedMaterial=material;
                PrefabUtility.SaveAsPrefabAsset(root,path);
            }
            finally { PrefabUtility.UnloadPrefabContents(root); }
            foreach(var cell in Object.FindObjectsByType<HexCell>())
            {
                cell.GetComponent<SpriteRenderer>().sharedMaterial=material;
                cell.GetComponent<HexCellThickness>().Rebuild();
            }
            AssetDatabase.SaveAssets();
            EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
        }
        static Material SideMaterial()
        {
            const string path=Folder+"/Art/HexSides.mat";
            var material=AssetDatabase.LoadAssetAtPath<Material>(path);
            if(material==null)
            {
                material=new Material(Shader.Find("Dungeon/Hex Sides"));
                AssetDatabase.CreateAsset(material,path);
            }
            material.mainTexture=AssetDatabase.LoadAssetAtPath<Texture2D>(Folder+"/TerrainTrial/Art/hexUnderDirt00.png");
            EditorUtility.SetDirty(material);
            return material;
        }
        public static void UpgradePivotAndSorting()
        {
            Tile("BaseHex",new Color(.76f,.74f,.67f),0);
            Tile("HoverOutline",Color.white,1);
            Tile("SelectionOutline",Color.white,2);
            string path=Folder+"/Prefabs/BaseHexCell.prefab";
            var root=PrefabUtility.LoadPrefabContents(path);
            try
            {
                var sorting=root.GetComponent<HexCellSorting>();
                if(sorting==null) sorting=root.AddComponent<HexCellSorting>();
                sorting.Refresh();
                root.GetComponent<HexCellThickness>().sideRenderer.sharedMaterial=SideMaterial();
                PrefabUtility.SaveAsPrefabAsset(root,path);
            }
            finally { PrefabUtility.UnloadPrefabContents(root); }
            // Migrate in place, preserving user placement and per-instance thickness changes.
            foreach(var cell in Object.FindObjectsByType<HexCell>())
            {
                var sorting=cell.GetComponent<HexCellSorting>();
                if(sorting==null) sorting=cell.gameObject.AddComponent<HexCellSorting>();
                sorting.Refresh();
                foreach(var renderer in cell.GetComponentsInChildren<Renderer>())
                {
                    if(PrefabUtility.IsPartOfPrefabInstance(renderer))
                    {
                        var property=new SerializedObject(renderer).FindProperty("m_SortingOrder");
                        if(property!=null && property.prefabOverride)
                            PrefabUtility.RevertPropertyOverride(property,InteractionMode.AutomatedAction);
                    }
                }
                sorting.Refresh();
            }
            var shadow=GameObject.Find("Board ground shadow");
            if(shadow!=null) shadow.GetComponent<SpriteRenderer>().sortingOrder=-31000;
            foreach(var background in Object.FindObjectsByType<BoardBackground>())
                background.GetComponent<SpriteRenderer>().sortingOrder=-32000;
            EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
        }
        static Sprite Foundation()
        {
            const int w = 1600, h = 1100;
            var texture = new Texture2D(w,h,TextureFormat.RGBA32,false);
            var pixels = new Color[w*h];
            for (int y=0;y<h;y++) for(int x=0;x<w;x++)
            {
                Vector2 p = new Vector2((x-w*.5f)/70f,(y-h*.5f)/70f);
                float d = Mathf.Pow((p.x-.15f)/8.5f,2)+Mathf.Pow((p.y+.85f)/3.5f,2);
                float alpha = Mathf.Exp(-d * d * 2.2f) * .65f;
                pixels[y*w+x]=new Color(.018f,.018f,.020f,alpha);
            }
            texture.SetPixels(pixels); texture.Apply();
            string path=Folder+"/Art/Foundation.png";
            File.WriteAllBytes(path,texture.EncodeToPNG()); Object.DestroyImmediate(texture);
            AssetDatabase.ImportAsset(path);
            var importer=(TextureImporter)AssetImporter.GetAtPath(path);
            importer.textureType=TextureImporterType.Sprite; importer.spritePixelsPerUnit=70;
            importer.spriteImportMode=SpriteImportMode.Single;
            var settings = new TextureImporterSettings(); importer.ReadTextureSettings(settings);
            settings.spriteAlignment=(int)SpriteAlignment.Center;
            settings.spritePivot=new Vector2(.5f,.5f);
            settings.spriteMeshType=SpriteMeshType.FullRect;
            importer.SetTextureSettings(settings);
            importer.alphaIsTransparency=true; importer.textureCompression=TextureImporterCompression.Uncompressed;
            importer.SaveAndReimport();
            return AssetDatabase.LoadAssetAtPath<Sprite>(path);
        }
        public static void AddBackground(Camera camera, Material material)
        {
            const string path = Folder + "/Art/Background/blue-ink-abstract.jpg";
            AssetDatabase.ImportAsset(path);
            var importer = (TextureImporter)AssetImporter.GetAtPath(path);
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.spritePixelsPerUnit = 100;
            importer.mipmapEnabled = true;
            importer.maxTextureSize = 2048;
            importer.SaveAndReimport();
            var existing = GameObject.Find("Blue painted background") ?? GameObject.Find("Wood tabletop background");
            var background = existing != null ? existing : new GameObject("Blue painted background");
            background.name = "Blue painted background";
            var renderer = background.GetComponent<SpriteRenderer>();
            if (renderer == null) renderer = background.AddComponent<SpriteRenderer>();
            renderer.sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
            renderer.sharedMaterial = material;
            renderer.color = new Color(.88f,.94f,1f,1);
            renderer.sortingOrder = -32000;
            var fit = background.GetComponent<BoardBackground>();
            if (fit == null) fit = background.AddComponent<BoardBackground>();
            fit.boardCamera = camera;
            fit.Fit();
        }

        [MenuItem("Tools/Hex Board/Create 2D Board Scene")]
        public static void Create()
        {
            Directory.CreateDirectory(Folder + "/Art");
            var baseSprite = Tile("BaseHex", new Color(.76f,.74f,.67f), 0);
            var hoverSprite = Tile("HoverOutline", Color.white, 1);
            var selectedSprite = Tile("SelectionOutline", Color.white, 2);
            var foundation = Foundation();
            var scene = SceneManager.GetActiveScene();
            if (scene.path == "Assets/Scenes/HexBoard2D.unity")
            {
                foreach(var root in scene.GetRootGameObjects())
                {
                    // MCP may invoke this during an editor camera render callback.
                    // Disable old cameras immediately and destroy them on the next editor tick.
                    if (root.GetComponent<Camera>() != null)
                    {
                        root.SetActive(false);
                        var retired = root;
                        EditorApplication.delayCall += () => { if (retired != null) Object.DestroyImmediate(retired); };
                    }
                    else Object.DestroyImmediate(root);
                }
            }
            else scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Additive);
            SceneManager.SetActiveScene(scene);
            var camera = new GameObject("Main Camera").AddComponent<Camera>();
            camera.tag = "MainCamera"; camera.orthographic = true; camera.orthographicSize = 17.5f;
            camera.transform.position = new Vector3(0, -1.5f, -10);
            camera.clearFlags = CameraClearFlags.SolidColor; camera.backgroundColor = new Color(.10f,.15f,.18f);
            camera.gameObject.AddComponent<AudioListener>();
            var light = new GameObject("Main Light").AddComponent<Light>(); light.type = LightType.Directional;
            light.transform.rotation = Quaternion.Euler(40,-30,0);
            var material = AssetDatabase.LoadAssetAtPath<Material>(Folder + "/Art/HexUnlit.mat");
            if (material == null)
            {
                material = new Material(Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default"));
                AssetDatabase.CreateAsset(material, Folder + "/Art/HexUnlit.mat");
            }
            // Reference-matching background is pending; use a quiet blue backdrop.
            camera.backgroundColor = new Color(.045f,.12f,.20f);
            var board = new GameObject("Base hex map");
            var plinth = new GameObject("Board ground shadow").AddComponent<SpriteRenderer>();
            plinth.sprite = foundation; plinth.sharedMaterial = material; plinth.sortingOrder = -31000;
            plinth.transform.position = new Vector3(0,-4f,0);
            plinth.transform.localScale = new Vector3(1.55f,3.3f,1);
            var input = board.AddComponent<HexBoardInput>(); input.boardCamera = camera;
            Directory.CreateDirectory(Folder + "/Prefabs");
            var template = new GameObject("BaseHexCell");
            var top = template.AddComponent<SpriteRenderer>(); top.sharedMaterial=TopMaterial(); top.sprite=baseSprite;
            var cellData = template.AddComponent<HexCell>();
            cellData.normalSprite=baseSprite; cellData.highlightedSprite=hoverSprite; cellData.selectedSprite=selectedSprite;
            var terrainObject=new GameObject("Terrain Sprite (assign later)"); terrainObject.transform.SetParent(template.transform,false);
            cellData.terrainRenderer=terrainObject.AddComponent<SpriteRenderer>(); cellData.terrainRenderer.sharedMaterial=material;
            var outlineObject=new GameObject("Selection outline"); outlineObject.transform.SetParent(template.transform,false);
            cellData.outlineRenderer=outlineObject.AddComponent<SpriteRenderer>(); cellData.outlineRenderer.sharedMaterial=material;
            var sideObject=new GameObject("Adjustable 2D sides"); sideObject.transform.SetParent(template.transform,false);
            var depth=template.AddComponent<HexCellThickness>();
            depth.sideMesh=sideObject.AddComponent<MeshFilter>(); depth.sideRenderer=sideObject.AddComponent<MeshRenderer>();
            depth.sideRenderer.sharedMaterial=SideMaterial();
            template.AddComponent<PolygonCollider2D>().points=Outline;
            template.AddComponent<HexCellSorting>().Refresh();
            // The transient side mesh is regenerated by the component in edit and play mode.
            var prefab=PrefabUtility.SaveAsPrefabAsset(template,Folder+"/Prefabs/BaseHexCell.prefab");
            Object.DestroyImmediate(template);
            for (int q = -8; q <= 8; q++) for (int r = -10; r <= 8; r++)
            {
                if (Distance(q,r,-4,4)>4 && Distance(q,r,4,0)>4 && Distance(q,r,0,-4)>5) continue;
                var cell=(GameObject)PrefabUtility.InstantiatePrefab(prefab,board.transform);
                cell.name="Hex ["+q+", "+r+"]";
                cell.transform.localPosition=HexGeometry.Position(q,r);
                var renderer=cell.GetComponent<SpriteRenderer>();
                var data=cell.GetComponent<HexCell>(); data.axial=new Vector2Int(q,r); data.layoutVersion=1;
                cell.GetComponent<HexCellSorting>().Refresh();
                cell.GetComponent<HexCellThickness>().Rebuild();
                if (q == -3 && r == 3) input.selected=data;
                data.Show(data==input.selected,false);
                PrefabUtility.RecordPrefabInstancePropertyModifications(cell.transform);
                PrefabUtility.RecordPrefabInstancePropertyModifications(renderer);
                PrefabUtility.RecordPrefabInstancePropertyModifications(data);
                PrefabUtility.RecordPrefabInstancePropertyModifications(data.terrainRenderer);
                PrefabUtility.RecordPrefabInstancePropertyModifications(data.outlineRenderer);
            }
            EditorSceneManager.SaveScene(scene, "Assets/Scenes/HexBoard2D.unity");
            // Keep the original scene on disk; only the generated board remains loaded.
            for (int i = SceneManager.sceneCount - 1; i >= 0; i--)
            {
                var other = SceneManager.GetSceneAt(i);
                if (other != scene && !other.isDirty) EditorSceneManager.CloseScene(other, true);
            }
            Selection.activeGameObject = board;
            if (SceneView.lastActiveSceneView != null)
            {
                SceneView.lastActiveSceneView.in2DMode = true;
                SceneView.lastActiveSceneView.LookAt(Vector3.zero, Quaternion.identity, 10);
            }
            AssetDatabase.SaveAssets();
            Debug.Log("Base hex map created: "+board.transform.childCount+" sprite cells.");
        }
    }
}


