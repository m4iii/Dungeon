using UnityEngine;
using UnityEngine.Rendering;

namespace Dungeon.HexBoard
{
    [ExecuteAlways, RequireComponent(typeof(SortingGroup))]
    public sealed class TerrainPropSorting : MonoBehaviour
    {
        void OnEnable()
        {
            Refresh();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.update += Refresh;
#endif
        }
        void OnDisable()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.update -= Refresh;
#endif
        }
        void LateUpdate() { Refresh(); }
        public void Refresh()
        {
            if (this == null) return;
            var group = GetComponent<SortingGroup>();
            // A prop must escape the parent tile group so units can pass on either side.
            group.sortAtRoot = true;
            group.sortingOrder = Mathf.Clamp(Mathf.RoundToInt(-transform.position.y*100)+50,-29900,29900);
        }
    }
}
