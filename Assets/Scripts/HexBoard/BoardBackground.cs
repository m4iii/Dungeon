using UnityEngine;

namespace Dungeon.HexBoard
{
    [ExecuteAlways]
    [RequireComponent(typeof(SpriteRenderer))]
    public sealed class BoardBackground : MonoBehaviour
    {
        public Camera boardCamera;

        private void LateUpdate()
        {
            Fit();
        }

        public void Fit()
        {
            var renderer = GetComponent<SpriteRenderer>();
            if (boardCamera == null || renderer.sprite == null) return;
            float height = boardCamera.orthographicSize * 2;
            var size = renderer.sprite.bounds.size;
            float scale = Mathf.Max(height * boardCamera.aspect / size.x, height / size.y) * 1.01f;
            transform.localScale = Vector3.one * scale;
            transform.position = new Vector3(boardCamera.transform.position.x, boardCamera.transform.position.y, 1);
        }
    }
}
