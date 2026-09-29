using UnityEngine;

namespace Dungeon.HexBoard
{
    public static class HexGeometry
    {
        // Match the sample art: width 2, height 2, with vertical sides of length 1.
        public static Vector2[] Outline(float scale = 1)
        {
            return new[] { new Vector2(0,1)*scale, new Vector2(-1,.5f)*scale,
                new Vector2(-1,-.5f)*scale, new Vector2(0,-1)*scale,
                new Vector2(1,-.5f)*scale, new Vector2(1,.5f)*scale };
        }
        public static Vector3 Position(int q, int r) => new Vector3(2*q+r,1.5f*r,0);
    }
}
