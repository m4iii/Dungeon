using UnityEngine;
using UnityEngine.Rendering;

namespace Dungeon.HexBoard
{
    /// <summary>Sort the complete tile by its top-center anchor, not by changing image bounds.</summary>
    [ExecuteAlways, DisallowMultipleComponent]
    [RequireComponent(typeof(SortingGroup), typeof(HexCell))]
    public sealed class HexCellSorting : MonoBehaviour
    {
        private SortingGroup group;
        private HexCell cell;
        private SpriteRenderer top;
        private HexCellThickness depth;

        private void OnEnable()
        {
            Refresh();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.update -= EditorTick;
            UnityEditor.EditorApplication.update += EditorTick;
#endif
        }
        private void OnDisable()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.update -= EditorTick;
#endif
        }
#if UNITY_EDITOR
        private void EditorTick() { if (!Application.isPlaying && this != null) Refresh(); }
#endif
        private void LateUpdate() { Refresh(); }

        public void Refresh()
        {
            if (group == null) group = GetComponent<SortingGroup>();
            if (cell == null) cell = GetComponent<HexCell>();
            if (top == null) top = GetComponent<SpriteRenderer>();
            if (depth == null) depth = GetComponent<HexCellThickness>();
            // World position includes parent movement; lower on screen means nearer.
            // Reserve the lowest orders for the backdrop and ground shadow.
            int order = Mathf.Clamp(Mathf.RoundToInt(-transform.position.y * 100), -30000, 30000);
            if (group.sortingOrder != order) group.sortingOrder = order;
            if (top.sortingOrder != 0) top.sortingOrder = 0;
            if (top.spriteSortPoint != SpriteSortPoint.Pivot) top.spriteSortPoint = SpriteSortPoint.Pivot;
            if (depth != null && depth.sideRenderer != null && depth.sideRenderer.sortingOrder != -1) depth.sideRenderer.sortingOrder = -1;
            if (cell.terrainRenderer != null && cell.terrainRenderer.sortingOrder != 1) cell.terrainRenderer.sortingOrder = 1;
            if (cell.outlineRenderer != null && cell.outlineRenderer.sortingOrder != 5) cell.outlineRenderer.sortingOrder = 5;
        }
    }
}
