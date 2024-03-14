using UnityEngine;

namespace Dev.Wilson
{
    public class TestEntity : MonoBehaviour, ICollider
    {
        [SerializeField]
        private SpriteRenderer _spriteRenderer;

        [SerializeField]
        private Color _normalColor;

        [SerializeField]
        private Color _collideColor;

        public float X => transform.position.x;

        public float Y => transform.position.y;

        public float Radius => transform.localScale.x / 2;

        public void OnCollideEnter()
        {
            _spriteRenderer.color = _collideColor;
        }

        public void OnCollideExit()
        {
            _spriteRenderer.color = _normalColor;
        }
    }
}