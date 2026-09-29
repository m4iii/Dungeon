using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Dungeon.Core;
using Dungeon.Runtime;
using UnityEditor;
using UnityEngine;

namespace Dungeon.MapDebug.Editor
{
    public sealed class MapDebugWindow : EditorWindow
    {
        [SerializeField] private MapDebugPreset preset;
        [SerializeField] private string seedText = "12345";
        [SerializeField] private int regionIndex;
        [SerializeField] private bool showCoordinates = true;
        [SerializeField] private bool showTerrainArt = true;
        [SerializeField] private bool showLayout = true;
        [SerializeField] private bool showCellLabels;
        private readonly Dictionary<TerrainKind, Texture2D> terrainTextures = new Dictionary<TerrainKind, Texture2D>();
        private UnityEditor.Editor presetEditor;
        private Vector2 settingsScroll, detailsScroll, pan;
        private float zoom = 30;
        private bool fitPending;
        private MapDefinition map;
        private IMapTopology topology;
        private DebugGrid generatedGrid;
        private bool generatedIrregular;
        private uint generatedSeed;
        private uint generatedMapSeed;
        private int generatedRegionIndex;
        private string generatedPresetJson, error, summary;
        private string generatedThemeJson, generatedThemeName;
        private Color generatedWaterColor;
        private MapArtComposition composition;
        private double elapsedMilliseconds;
        private CellDefinition selected;
        private Dictionary<CellPosition, CellDefinition> cells;
        private GUIStyle centered;
        private const string DefaultPath = "Assets/Scripts/MapDebug/Presets/DefaultMapDebug.asset";

        [MenuItem("Dungeon/地图调试台")]
        public static void Open()
        {
            var window = GetWindow<MapDebugWindow>();
            window.titleContent = new GUIContent("地图调试台");
            window.minSize = new Vector2(920, 600);
            window.Show();
        }

        private void OnEnable()
        {
            titleContent = new GUIContent("地图调试台"); minSize = new Vector2(920, 600);
            if (preset == null) preset = AssetDatabase.LoadAssetAtPath<MapDebugPreset>(DefaultPath);
            Undo.undoRedoPerformed += OnUndo;
        }
        private void OnDisable()
        {
            composition?.Dispose();composition=null;
            Undo.undoRedoPerformed -= OnUndo;
            if (presetEditor != null) DestroyImmediate(presetEditor);
        }
        private void OnUndo() { Repaint(); }

        private void OnGUI()
        {
            const float side = 320;
            GUILayout.BeginArea(new Rect(8, 8, side - 16, position.height - 16));
            EditorGUILayout.LabelField("生成配置", EditorStyles.boldLabel);
            var next = (MapDebugPreset)EditorGUILayout.ObjectField(preset, typeof(MapDebugPreset), false);
            if (next != preset)
            {
                preset = next; if (presetEditor != null) DestroyImmediate(presetEditor); presetEditor = null;
            }
            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("新建配置")) CreatePreset(false);
                using (new EditorGUI.DisabledScope(preset == null))
                {
                    if (GUILayout.Button("另存配置")) CreatePreset(true);
                    if (GUILayout.Button("保存")) AssetDatabase.SaveAssetIfDirty(preset);
                }
            }
            settingsScroll = EditorGUILayout.BeginScrollView(settingsScroll);
            if (preset != null)
            {
                UnityEditor.Editor.CreateCachedEditor(preset, null, ref presetEditor);
                presetEditor.OnInspectorGUI();
            }
            else EditorGUILayout.HelpBox("选择一个地图配置，或点击“新建配置”。", MessageType.Info);
            EditorGUILayout.EndScrollView();
            GUILayout.EndArea();

            float left = side + 8, rightWidth = position.width - side - 16;
            GUILayout.BeginArea(new Rect(left, 8, rightWidth, 176));
            using (new EditorGUILayout.HorizontalScope())
            {
                GUILayout.Label("本局种子", GUILayout.Width(60));
                seedText = GUILayout.TextField(seedText, GUILayout.Width(112));
                using (new EditorGUI.DisabledScope(preset == null))
                {
                    if (GUILayout.Button("生成", GUILayout.Width(65))) Generate();
                    if (GUILayout.Button("新种子生成", GUILayout.Width(100)))
                    { seedText = RunFactory.CreateTimeSeed().ToString(); Generate(); }
                }
                using (new EditorGUI.DisabledScope(map == null))
                {
                    if (GUILayout.Button("居中", GUILayout.Width(55))) fitPending = true;
                    if (GUILayout.Button("导出 JSON", GUILayout.Width(90))) Export();
                }
            }
            regionIndex = EditorGUILayout.IntField("区域序号（从 0 开始）", regionIndex);
            showCoordinates = EditorGUILayout.ToggleLeft("显示坐标 · 滚轮缩放 · 中键或右键拖动 · 左键查看地块", showCoordinates);
            showTerrainArt = EditorGUILayout.ToggleLeft("显示主题素材（浅滩可进入；河流、湖泊与山地阻挡）", showTerrainArt);
            showLayout = EditorGUILayout.ToggleLeft("显示布局：主路金色边框 · 房间入口粉色边框 · 房间编号", showLayout);
            showCellLabels = EditorGUILayout.ToggleLeft("显示地块文字（关闭后仍可点击查看详情）", showCellLabels);
            if (map != null)
            {
                bool changed = preset == null || generatedPresetJson != JsonUtility.ToJson(preset) || generatedThemeJson != (preset.theme == null ? "" : JsonUtility.ToJson(preset.theme)) || regionIndex != generatedRegionIndex || seedText != generatedSeed.ToString();
                EditorGUILayout.LabelField($"当前结果：种子 {generatedSeed} · {(generatedGrid == DebugGrid.Hex ? "六边形" : "方格")} · {(generatedIrregular ? "不规则连通" : "完整网格")} · {map.Cells.Count} 格 · {elapsedMilliseconds:F2} ms", EditorStyles.miniLabel);
                EditorGUILayout.LabelField($"区域 {generatedRegionIndex} · 地图种子 {generatedMapSeed} · 生成版本 {RunSeeds.MapGenerationVersion}", EditorStyles.miniLabel);
                if (changed) EditorGUILayout.LabelField("配置已更改，点击生成更新预览。当前预览及导出仍是上次结果。", EditorStyles.miniLabel);
            }
            GUILayout.EndArea();

            var canvas = new Rect(left, 208, rightWidth, Mathf.Max(140, position.height - 414));
            DrawMap(canvas);
            GUILayout.BeginArea(new Rect(left, canvas.yMax + 8, rightWidth, position.height - canvas.yMax - 16));
            detailsScroll = EditorGUILayout.BeginScrollView(detailsScroll);
            if (!string.IsNullOrEmpty(error)) EditorGUILayout.HelpBox(error, MessageType.Error);
            if (map != null)
            {
                EditorGUILayout.LabelField(summary, EditorStyles.wordWrappedMiniLabel);
                DrawDetails();
            }
            else if (string.IsNullOrEmpty(error)) EditorGUILayout.HelpBox("设置配置和种子后点击生成。默认配置使用六边形网格。", MessageType.Info);
            EditorGUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        private void Generate()
        {
            map = null; selected = null; cells = null; error = null;
            if (!uint.TryParse(seedText, out var seed)) { error = "种子须为 0～4294967295 的整数。"; return; }
            try
            {
                var definition = preset.ToCore();
                var snapshotTopology = preset.CreateTopology();
                var timer = System.Diagnostics.Stopwatch.StartNew();
                var result = RunSeeds.GenerateRegion(seed, definition, regionIndex, snapshotTopology);
                timer.Stop(); elapsedMilliseconds = timer.Elapsed.TotalMilliseconds;
                map = result; topology = snapshotTopology; generatedGrid = preset.grid; generatedSeed = seed;
                generatedMapSeed = RunSeeds.ForRegion(seed, definition.Id, regionIndex); generatedRegionIndex = regionIndex;
                generatedIrregular = definition.Shape != null;
                generatedPresetJson = JsonUtility.ToJson(preset);
                generatedThemeJson = preset.theme == null ? "" : JsonUtility.ToJson(preset.theme);
                generatedThemeName = preset.theme == null ? "旧版配置" : preset.theme.displayName;
                generatedWaterColor = preset.theme == null ? new Color(.22f,.48f,.51f) : preset.theme.waterColor;
                cells = map.Cells.ToDictionary(x => x.Position);
                composition?.Dispose();composition=new MapArtComposition(map,topology,generatedGrid,preset.theme);
                terrainTextures.Clear();
                foreach (var visual in (preset.theme == null ? preset.terrainVisuals : preset.theme.visuals) ?? Array.Empty<DebugTerrainVisual>())
                    if (visual != null)
                    {
                        var texture = generatedGrid == DebugGrid.Hex ? visual.hexTexture : visual.squareTexture;
                        if (texture != null) terrainTextures[visual.terrain] = texture;
                    }
                summary = $"主题：{generatedThemeName} · 可进入 {map.Cells.Count(x => x.IsWalkable)} 格 · 阻挡 {map.Cells.Count(x => !x.IsWalkable)} 格\n" +
                    string.Join("    ", map.Cells.GroupBy(x => x.Terrain).OrderBy(x => x.Key).Select(g => $"{TerrainLabel(g.Key)} {g.Count()}"));
                if (map.Layout.Count > 0) summary = $"主路 {map.Layout.Values.Count(x => x.MainRoad)} 格 · 分支房间 {map.Layout.Values.Where(x => x.RoomId >= 0).Select(x => x.RoomId).Distinct().Count()} 个 · 出口 {map.Cells.Count(x => x.Kind == CellKind.Exit)} 个\n" + summary;
                selected = cells[map.Entrance]; fitPending = true;
            }
            catch (Exception exception)
            {
                error = "生成失败：" + exception.Message + "\n请检查生成范围、目标格数、固定地块和入口。无法满足的配置会明确报错，不会自动换种子重抽。";
            }
            Repaint();
        }

        private Vector2 LogicalCenter(CellPosition position)
        {
            // Same pointed hex proportions and axial mapping as the existing HexBoard renderer.
            return generatedGrid == DebugGrid.Hex ? new Vector2(2 * position.X + position.Y, -1.5f * position.Y) : new Vector2(2 * position.X, -2 * position.Y);
        }
        private Vector2[] Corners()
        {
            return generatedGrid == DebugGrid.Hex
                ? new[] { new Vector2(0, -1), new Vector2(1, -.5f), new Vector2(1, .5f), new Vector2(0, 1), new Vector2(-1, .5f), new Vector2(-1, -.5f) }
                : new[] { new Vector2(-1, -1), new Vector2(1, -1), new Vector2(1, 1), new Vector2(-1, 1) };
        }
        private void DrawMap(Rect canvas)
        {
            EditorGUI.DrawRect(canvas, new Color(.075f, .085f, .105f));
            if (map == null) return;
            var corners = Corners();
            if (fitPending && canvas.width > 0 && canvas.height > 0)
            {
                var centers = map.Cells.Select(x => LogicalCenter(x.Position)).ToArray();
                Vector2 min = new Vector2(centers.Min(x => x.x) - 1, centers.Min(x => x.y) - 2);
                Vector2 max = new Vector2(centers.Max(x => x.x) + 1, centers.Max(x => x.y) + 1);
                zoom = Mathf.Clamp(Mathf.Min((canvas.width - 32) / (max.x - min.x), (canvas.height - 32) / (max.y - min.y)), 1, 100);
                pan = canvas.size * .5f - (min + max) * .5f * zoom; fitPending = false;
            }
            var current = Event.current;
            if (canvas.Contains(current.mousePosition))
            {
                if (current.type == EventType.ScrollWheel)
                {
                    Vector2 local = current.mousePosition - canvas.position;
                    float previous = zoom; zoom = Mathf.Clamp(zoom * Mathf.Pow(1.12f, -current.delta.y), 1, 150);
                    pan = local - (local - pan) * (zoom / previous); current.Use(); Repaint();
                }
                if (current.type == EventType.MouseDrag && (current.button == 1 || current.button == 2))
                { pan += current.delta; current.Use(); Repaint(); }
                if (current.type == EventType.MouseDown && current.button == 0)
                {
                    Vector2 local = (current.mousePosition - canvas.position - pan) / zoom;
                    selected = map.Cells.FirstOrDefault(cell => Inside(local - LogicalCenter(cell.Position), corners));
                    current.Use(); Repaint();
                }
            }
            if (current.type != EventType.Repaint) return;
            if (centered == null) centered = new GUIStyle(EditorStyles.miniLabel) { alignment = TextAnchor.MiddleCenter, normal = { textColor = Color.white } };
            var neighbors = selected == null ? new HashSet<CellPosition>() : new HashSet<CellPosition>(topology.Neighbors(selected.Position));
            GUI.BeginClip(canvas);
            composition?.SetCanvasSize(canvas.size);
            Handles.BeginGUI();
            foreach (var cell in map.Cells.OrderBy(x => LogicalCenter(x.Position).y).ThenBy(x => LogicalCenter(x.Position).x))
            {
                Vector2 center = LogicalCenter(cell.Position) * zoom + pan;
                if (center.x < -zoom || center.y < -zoom || center.x > canvas.width + zoom || center.y > canvas.height + 2 * zoom) continue;
                var polygon = corners.Select(p => (Vector3)(center + p * zoom * .93f)).ToArray();
                Handles.color = TerrainTint(cell.Terrain); Handles.DrawAAConvexPolygon(polygon);
                if (showTerrainArt)
                {
                    var texture = TerrainTexture(cell.Terrain);
                    var artRect=new Rect(center.x-zoom,center.y-2*zoom,2*zoom,3*zoom);
                    if (composition != null && composition.DrawGround(cell,artRect)) { }
                    else if (texture != null)
                    {
                        var previousColor = GUI.color;
                        GUI.color = cell.Terrain == TerrainKind.Lake ? generatedWaterColor * 1.6f : cell.Terrain == TerrainKind.ShallowWater ? new Color(.55f, 1.7f, 1.6f, .65f) : Color.white;
                        // Original tiles reserve the top third for trees/mountains; bottom 256px is the footprint.
                        GUI.DrawTexture(new Rect(center.x - zoom, center.y - 2 * zoom, 2 * zoom, 3 * zoom), texture, ScaleMode.StretchToFill, true);
                        GUI.color = previousColor;
                    }
                }
                if(showTerrainArt)composition?.DrawWater(cell,center,zoom);
            }
            // Shared lake artwork goes over all ground tiles, below grid/selection UI.
            if(showTerrainArt)composition?.DrawLakes(pan,zoom);
            foreach(var cell in map.Cells)
            {
                Vector2 center=LogicalCenter(cell.Position)*zoom+pan;
                if(center.x < -zoom || center.y < -zoom || center.x > canvas.width+zoom || center.y > canvas.height+2*zoom)continue;
                var polygon=corners.Select(p=>(Vector3)(center+p*zoom*.93f)).ToArray();
                map.Layout.TryGetValue(cell.Position, out var layout);
                Color outline = showLayout && layout != null && layout.MainRoad ? new Color(.95f, .7f, .25f) :
                    showLayout && layout != null && layout.RoomEntrance ? new Color(.95f, .45f, .7f) : new Color(.17f, .2f, .25f);
                Handles.color = cell == selected ? Color.white : neighbors.Contains(cell.Position) ? new Color(.6f, .85f, 1) : outline;
                if(!showTerrainArt||showLayout||cell==selected||neighbors.Contains(cell.Position))
                    Handles.DrawAAPolyLine(cell == selected ? 3 : 1.5f, polygon.Concat(new[] { polygon[0] }).ToArray());
                if (showCellLabels && zoom >= 15)
                {
                    string name = cell.Position.Equals(map.Entrance) ? "入口" :
                        (!cell.IsWalkable || cell.Kind == CellKind.Empty ? TerrainLabel(cell.Terrain) : composition?.LocationName(cell) ?? Label(cell.Kind));
                    if (showLayout && layout != null && zoom >= 25) name += layout.RoomId >= 0 ? $"·房{layout.RoomId + 1}" : "·主";
                    if (showCoordinates && zoom >= 25) name += $"\n{cell.Position.X},{cell.Position.Y}";
                    var labelRect = new Rect(center.x - zoom, center.y - 10, zoom * 2, 30);
                    EditorGUI.DrawRect(labelRect, new Color(0, 0, 0, .48f));
                    GUI.Label(labelRect, name, centered);
                }
            }
            Handles.color = Color.white; Handles.EndGUI(); GUI.EndClip();
        }
        private void DrawDetails()
        {
            if (selected == null) { EditorGUILayout.LabelField("点击地块查看内容和邻居。"); return; }
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField($"选中 ({selected.Position.X}, {selected.Position.Y}) · {composition?.LocationName(selected) ?? Label(selected.Kind)}", EditorStyles.boldLabel);
            EditorGUILayout.LabelField("地形 / 通行", TerrainLabel(selected.Terrain) + (selected.IsWalkable ? " / 可以进入" : " / 阻挡"));
            if(map.SurfaceTerrain.TryGetValue(selected.Position,out var baseTerrain) && baseTerrain!=selected.Terrain)
                EditorGUILayout.LabelField("原有地形",TerrainLabel(baseTerrain));
            if (map.Layout.TryGetValue(selected.Position, out var layout))
                EditorGUILayout.LabelField("布局", $"{(layout.MainRoad ? "主路" : "房间 " + (layout.RoomId + 1))}{(layout.RoomEntrance ? " / 房间入口" : "")} / 距入口 {layout.Depth} 步");
            EditorGUILayout.LabelField("邻居", string.Join("  ", topology.Neighbors(selected.Position).Where(cells.ContainsKey).Select(x => $"({x.X},{x.Y})")), EditorStyles.wordWrappedLabel);
            if (selected.Enemies.Count > 0) EditorGUILayout.LabelField("敌人", string.Join(", ", selected.Enemies), EditorStyles.wordWrappedLabel);
            if (selected.Reward.Coins > 0) EditorGUILayout.LabelField("金币", selected.Reward.Coins.ToString());
            foreach (var item in selected.Reward.Items) EditorGUILayout.LabelField("奖励道具", item.Key + " × " + item.Value);
            if (selected.RequiredKey != null) EditorGUILayout.LabelField("钥匙", selected.RequiredKey);
            if (selected.ContentId != null) EditorGUILayout.LabelField("交互配置", selected.ContentId);
        }
        private void CreatePreset(bool copy)
        {
            string path = EditorUtility.SaveFilePanelInProject("保存地图配置", copy ? preset.name + " Copy" : "MapDebugPreset", "asset", "选择配置保存位置");
            if (string.IsNullOrEmpty(path)) return;
            var created = copy ? Instantiate(preset) : CreateInstance<MapDebugPreset>();
            AssetDatabase.CreateAsset(created, AssetDatabase.GenerateUniqueAssetPath(path)); AssetDatabase.SaveAssets();
            preset = created; if (presetEditor != null) DestroyImmediate(presetEditor); presetEditor = null;
        }
        private void Export()
        {
            string path = EditorUtility.SaveFilePanel("导出当前地图", "", "map-" + generatedSeed, "json");
            if (string.IsNullOrEmpty(path)) return;
            try
            {
                var data = new MapExport
                {
                    mapId = map.Id, topology = generatedGrid.ToString(), seed = generatedSeed.ToString(), entranceX = map.Entrance.X, entranceY = map.Entrance.Y,
                    mapSeed = generatedMapSeed.ToString(), regionIndex = generatedRegionIndex, mapGenerationVersion = RunSeeds.MapGenerationVersion,
                    requireBossForExit = map.RequireBossForExit, restRatio = map.RestRatio, generationPresetJson = generatedPresetJson,
                    themeName = generatedThemeName, themeJson = generatedThemeJson,
                    cells = map.Cells.Select(x => new CellExport { x = x.Position.X, y = x.Position.Y, kind = x.Kind.ToString(), terrain = x.Terrain.ToString(), walkable = x.IsWalkable, enemies = x.Enemies.ToArray(), coins = x.Reward.Coins,
                        surfaceTerrain = map.SurfaceTerrain.TryGetValue(x.Position,out var surface) ? surface.ToString() : x.Terrain.ToString(),
                        mainRoad = map.Layout.TryGetValue(x.Position, out var layout) && layout.MainRoad,
                        roomId = map.Layout.TryGetValue(x.Position, out layout) ? layout.RoomId : -1,
                        roomEntrance = map.Layout.TryGetValue(x.Position, out layout) && layout.RoomEntrance,
                        depth = map.Layout.TryGetValue(x.Position, out layout) ? layout.Depth : -1,
                        items = x.Reward.Items.Select(p => new DebugItemAmount { id = p.Key, amount = p.Value }).ToArray(), requiredKey = x.RequiredKey, contentId = x.ContentId }).ToArray()
                };
                File.WriteAllText(path, JsonUtility.ToJson(data, true), new System.Text.UTF8Encoding(false));
                ShowNotification(new GUIContent("地图已导出"));
            }
            catch (Exception exception) { error = "导出失败：" + exception.Message; }
        }
        private static bool Inside(Vector2 p, Vector2[] polygon)
        {
            bool inside = false;
            for (int i = 0, j = polygon.Length - 1; i < polygon.Length; j = i++)
                if ((polygon[i].y > p.y) != (polygon[j].y > p.y) && p.x < (polygon[j].x - polygon[i].x) * (p.y - polygon[i].y) / (polygon[j].y - polygon[i].y) + polygon[i].x) inside = !inside;
            return inside;
        }
        private static string Label(CellKind kind)
        {
            switch (kind)
            {
                case CellKind.Empty: return "空地"; case CellKind.Battle: return "战斗"; case CellKind.Boss: return "首领";
                case CellKind.Reward: return "奖励"; case CellKind.Door: return "门"; case CellKind.Blocked: return "障碍";
                case CellKind.Rest: return "休息"; case CellKind.Exit: return "出口"; case CellKind.Shop: return "商店";
                case CellKind.SkillChoice: return "技能"; case CellKind.RelicChoice: return "遗物"; case CellKind.Event: return "事件";
                default: return kind.ToString();
            }
        }
        private Texture2D TerrainTexture(TerrainKind terrain)
        {
            if (terrainTextures.TryGetValue(terrain, out var found)) return found;
            string folder = generatedGrid == DebugGrid.Hex ? "Hex Samples/Hex Basic Terrain Set/hex" : "Tile Samples/Tile Basic Terrain Set/";
            string suffix;
            switch (terrain)
            {
                case TerrainKind.Woodland: suffix = generatedGrid == DebugGrid.Hex ? "Woodlands00" : "woodlands00"; break;
                case TerrainKind.Forest: suffix = generatedGrid == DebugGrid.Hex ? "ForestBroadleaf00" : "forestBroadleaf00"; break;
                case TerrainKind.Mountain: suffix = generatedGrid == DebugGrid.Hex ? "Hills00" : "hills00"; break;
                case TerrainKind.DeepWater: suffix = generatedGrid == DebugGrid.Hex ? "Ocean00" : "ocean00"; break;
                case TerrainKind.ShallowWater:
                case TerrainKind.Lake:
                    folder = generatedGrid == DebugGrid.Hex ? "Hex Samples/Hex Rivers Coasts & Seas/hex" : "Tile Samples/Tile River Coasts & Seas/";
                    suffix = generatedGrid == DebugGrid.Hex ? "OceanCalm00" : "oceanCalm00"; break;
                default: suffix = generatedGrid == DebugGrid.Hex ? "Plains00" : "plains00"; break;
            }
            found = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Arts/Hex and Tile Samples 1.0.1/" + folder + suffix + ".png");
            terrainTextures[terrain] = found; return found;
        }
        private static string TerrainLabel(TerrainKind terrain)
        {
            switch (terrain)
            {
                case TerrainKind.Plains: return "草原"; case TerrainKind.Woodland: return "疏林";
                case TerrainKind.Forest: return "森林"; case TerrainKind.Mountain: return "高山";
                case TerrainKind.ShallowWater: return "浅水"; case TerrainKind.DeepWater: return "深水";
                case TerrainKind.Sand: return "沙地"; case TerrainKind.Rock: return "岩地";
                case TerrainKind.Snow: return "雪地"; case TerrainKind.SnowForest: return "雪林";
                case TerrainKind.Ice: return "冰原"; case TerrainKind.River: return "河流";
                case TerrainKind.Ford: return "涉水浅滩"; case TerrainKind.Lake: return "湖泊";
                default: return terrain.ToString();
            }
        }
        private static Color TerrainTint(TerrainKind terrain)
        {
            switch (terrain)
            {
                case TerrainKind.Plains: return new Color(.49f, .58f, .25f);
                case TerrainKind.Woodland: return new Color(.3f, .47f, .19f);
                case TerrainKind.Forest: return new Color(.14f, .3f, .13f);
                case TerrainKind.Mountain: return new Color(.45f, .45f, .49f);
                case TerrainKind.ShallowWater: return new Color(.22f, .72f, .78f);
                case TerrainKind.River: case TerrainKind.Ford: case TerrainKind.Lake: return new Color(.22f,.48f,.51f);
                case TerrainKind.Sand: return new Color(.76f,.64f,.39f);
                case TerrainKind.Rock: return new Color(.49f,.44f,.35f);
                case TerrainKind.Snow: case TerrainKind.Ice: return new Color(.8f,.88f,.9f);
                case TerrainKind.SnowForest: return new Color(.5f,.65f,.65f);
                default: return new Color(.06f, .22f, .45f);
            }
        }
        [Serializable] private sealed class MapExport
        {
            public int version = 6;
            public string mapId, topology, seed, mapSeed, generationPresetJson;
            public string themeName, themeJson;
            public int regionIndex, mapGenerationVersion;
            public int entranceX, entranceY;
            public bool requireBossForExit;
            public double restRatio;
            public CellExport[] cells;
        }
        [Serializable] private sealed class CellExport
        {
            public string surfaceTerrain;
            public int x, y, coins;
            public string kind, requiredKey, contentId;
            public string terrain;
            public bool walkable;
            public bool mainRoad, roomEntrance;
            public int roomId, depth;
            public string[] enemies;
            public DebugItemAmount[] items;
        }
    }
}
