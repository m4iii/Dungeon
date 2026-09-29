using UnityEngine;

namespace Dungeon.HexBoard
{
    [ExecuteAlways]
    public sealed class HexCellThickness : MonoBehaviour
    {
        [Min(0), Tooltip("2D side depth in world units. Does not stretch the tile top or terrain sprite.")]
        public float thickness = 1.1f;
        public MeshFilter sideMesh;
        public MeshRenderer sideRenderer;
        private Mesh generatedMesh;
        private float applied = -1;

        private void OnEnable() { Rebuild(); }
        private void Update()
        {
            if (generatedMesh == null || !Mathf.Approximately(applied, Mathf.Max(0, thickness))) Rebuild();
            var top = GetComponent<SpriteRenderer>();
            if (sideRenderer != null && top != null)
            {
                sideRenderer.sortingOrder = top.sortingOrder - 1;
                sideRenderer.sortingLayerID = top.sortingLayerID;
            }
        }
        public void Rebuild()
        {
            if (sideMesh == null || sideRenderer == null) return;
            var top = GetComponent<SpriteRenderer>();
            sideRenderer.sortingOrder = top.sortingOrder - 1;
            sideRenderer.sortingLayerID = top.sortingLayerID;
            thickness = Mathf.Max(0, thickness);
            if (generatedMesh == null)
            {
                generatedMesh = new Mesh { name = "Hex side geometry", hideFlags = HideFlags.HideAndDontSave };
            }
            var vertices = new Vector3[8];
            var colors = new Color[8];
            var uv = new Vector2[8];
            var triangles = new int[12];
            var outline = HexGeometry.Outline();
            for (int face = 0; face < 2; face++)
            {
                int i = face + 2, offset = face * 4, tri = face * 6;
                Vector3 left = outline[i], right = outline[i+1];
                vertices[offset] = left; vertices[offset+1] = right;
                vertices[offset+2] = right + Vector3.down * thickness;
                vertices[offset+3] = left + Vector3.down * thickness;
                // The source's two painted faces already include their lighting.
                // Its top follows (0,1), (.5,2/3), (1,1); depth occupies 2/3 of the image.
                // Only geometry changes with thickness; the top surface stays fixed.
                for (int j=0;j<4;j++) colors[offset+j]=Color.white;
                uv[offset] = face == 0 ? new Vector2(0,1) : new Vector2(.5f,2f/3f);
                uv[offset+1] = face == 0 ? new Vector2(.5f,2f/3f) : new Vector2(1,1);
                uv[offset+2] = uv[offset+1] - new Vector2(0,2f/3f);
                uv[offset+3] = uv[offset] - new Vector2(0,2f/3f);
                triangles[tri]=offset; triangles[tri+1]=offset+1; triangles[tri+2]=offset+2;
                triangles[tri+3]=offset; triangles[tri+4]=offset+2; triangles[tri+5]=offset+3;
            }
            generatedMesh.Clear(); generatedMesh.vertices=vertices; generatedMesh.colors=colors;
            generatedMesh.uv=uv; generatedMesh.triangles=triangles; generatedMesh.RecalculateBounds();
            sideMesh.sharedMesh=generatedMesh; sideRenderer.enabled=thickness>0;
            applied=thickness;
        }
        private void OnDisable()
        {
            if (generatedMesh == null) return;
            if (Application.isPlaying) Destroy(generatedMesh); else DestroyImmediate(generatedMesh);
            generatedMesh=null; applied=-1;
        }
    }
}
