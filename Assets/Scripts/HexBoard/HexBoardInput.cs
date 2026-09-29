using UnityEngine;
using UnityEngine.InputSystem;

namespace Dungeon.HexBoard
{
    public sealed class HexBoardInput : MonoBehaviour
    {
        public Camera boardCamera;
        public HexCell selected;
        public float minimumViewSize = 17.5f;
        public float horizontalViewSize = 16f;
        private HexCell hovered;
        private void Update()
        {
            if (boardCamera == null) return;
            // Fit the full board even when the Game view changes aspect ratio.
            boardCamera.orthographicSize = Mathf.Max(minimumViewSize, horizontalViewSize / boardCamera.aspect);
            if (Mouse.current == null) return;
            var screen = Mouse.current.position.ReadValue();
            var point = boardCamera.ScreenToWorldPoint(new Vector3(screen.x, screen.y, 10));
            var hit = Physics2D.OverlapPoint(point);
            var next = hit == null ? null : hit.GetComponent<HexCell>();
            if (next != hovered)
            {
                if (hovered != null) hovered.Show(hovered == selected, false);
                hovered = next;
                if (hovered != null) hovered.Show(hovered == selected, true);
            }
            if (Mouse.current.leftButton.wasPressedThisFrame && hovered != null)
            {
                if (selected != null) selected.Show(false, selected == hovered);
                selected = hovered;
                selected.Show(true, true);
                var reveal=selected.GetComponent<HexReveal>();
                if(reveal!=null)reveal.Reveal();
            }
        }
    }
}
