using UnityEngine;

namespace Dungeon.HexBoard
{
    public sealed class HexCell : MonoBehaviour
    {
        public Vector2Int axial;
        [HideInInspector] public int layoutVersion;
        public Sprite normalSprite;
        public Sprite highlightedSprite;
        public Sprite selectedSprite;
        [Tooltip("Optional terrain art. The base slab and selection outline remain independent.")]
        public Sprite terrainSprite;
        public SpriteRenderer terrainRenderer;
        public SpriteRenderer outlineRenderer;
        public void Show(bool selected, bool hovered)
        {
            GetComponent<SpriteRenderer>().sprite = normalSprite;
            if (terrainRenderer != null) terrainRenderer.sprite = terrainSprite;
            if (outlineRenderer != null)
                outlineRenderer.sprite = selected ? selectedSprite : hovered ? highlightedSprite : null;
        }
    }
}
