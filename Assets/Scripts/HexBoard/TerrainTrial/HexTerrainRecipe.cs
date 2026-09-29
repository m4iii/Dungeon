using UnityEngine;
using UnityEngine.Rendering;

namespace Dungeon.HexBoard
{
    public sealed class HexTerrainRecipe : MonoBehaviour
    {
        public HexTerrainDefinition terrain;
        public int seed;
        public Material decorationMaterial;

        [ContextMenu("Rebuild Terrain")]
        public void Rebuild()
        {
            if (terrain == null) return;
            GetComponent<SpriteRenderer>().sharedMaterial = terrain.surfaceMaterial;
            var old = transform.Find("Terrain decorations");
            if (old != null)
            {
                old.gameObject.SetActive(false);
                if (Application.isPlaying) Destroy(old.gameObject); else DestroyImmediate(old.gameObject);
            }
            var root = new GameObject("Terrain decorations").transform;
            root.SetParent(transform, false);
            // Curated anchors; the seed only adds small changes to prevent untidy scattering.
            var anchors = new[] { new Vector2(-.25f,.1f), new Vector2(.28f,.22f), new Vector2(.05f,-.22f), new Vector2(-.5f,-.12f), new Vector2(.48f,-.12f) };
            var random = new System.Random(seed);
            for (int i=0; i<Mathf.Min(terrain.decorationCount, anchors.Length); i++)
            {
                if (terrain.decoration == null) break;
                var obj = new GameObject("Decoration " + i);
                obj.transform.SetParent(root, false);
                var anchor = terrain.decorationCount == 1 ? Vector2.zero : anchors[i];
                obj.transform.localPosition = anchor + new Vector2((float)random.NextDouble()-.5f,(float)random.NextDouble()-.5f)*.08f;
                obj.transform.localScale = Vector3.one*terrain.decorationScale*(.93f+(float)random.NextDouble()*.14f);
                var renderer = obj.AddComponent<SpriteRenderer>();
                renderer.sprite = terrain.decoration;
                renderer.sharedMaterial = decorationMaterial;
                renderer.spriteSortPoint = SpriteSortPoint.Pivot;
                obj.AddComponent<TerrainPropSorting>();
            }
        }
    }
}
