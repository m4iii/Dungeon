using UnityEngine;

namespace Dungeon.HexBoard
{
    [CreateAssetMenu(menuName = "Hex Board/Terrain Definition")]
    public sealed class HexTerrainDefinition : ScriptableObject
    {
        public Material surfaceMaterial;
        public Sprite decoration;
        [Range(0, 5)] public int decorationCount;
        public float decorationScale = 1;
    }
}
