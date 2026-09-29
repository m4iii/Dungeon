using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Dungeon.MapDebug.Editor
{
    [CustomEditor(typeof(MapDebugPreset))]
    public sealed class MapDebugPresetEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            var themes = AssetDatabase.FindAssets("t:MapTheme").Select(id => AssetDatabase.LoadAssetAtPath<MapTheme>(AssetDatabase.GUIDToAssetPath(id)))
                .Where(t => t != null).OrderBy(t => t.name).ToArray();
            var property = serializedObject.FindProperty("theme");
            var current = property.objectReferenceValue as MapTheme;
            var names = new[] { "请选择地图主题" }.Concat(themes.Select(t => t.displayName)).ToArray();
            int selected = System.Array.IndexOf(themes,current) + 1;
            int next = EditorGUILayout.Popup("地图主题",selected,names);
            if(next != selected) property.objectReferenceValue = next == 0 ? null : themes[next-1];
            current = property.objectReferenceValue as MapTheme;
            if(current != null)
            {
                EditorGUILayout.HelpBox(current.description,MessageType.Info);
                if(GUILayout.Button("编辑主题规则与素材")) Selection.activeObject = current;
            }
            else EditorGUILayout.HelpBox("选择具名主题后，地形配比、河流、湖泊和素材自动应用。未选择时保留旧配置兼容行为。",MessageType.Warning);
            DrawPropertiesExcluding(serializedObject,"m_Script","theme","generateTerrain","waterChance","terrainPatchSize","shallowShores","plainsWeight","woodlandWeight","forestWeight","terrainVisuals");
            serializedObject.ApplyModifiedProperties();
        }
    }
}
