using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#pragma warning disable CS4014

namespace Framework.UI
{
    [RequireComponent(typeof(Image))]
    public class MaskWidget : UIWidget, IPointerDownHandler
    {
        [SerializeField]
        private bool _isTouchToClose = false;
        
        private Image _image;

        protected override void OnInit()
        {
            _image = GetComponent<Image>();
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!_isTouchToClose) return;
            
            _uiSystem.HidePeekPanel();
        }

        public void SetActive(bool isActive)
        {
            _image.enabled = isActive;
        }
    }
}