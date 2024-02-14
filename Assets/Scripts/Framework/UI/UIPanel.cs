using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Framework.UI
{
    public enum UIPanelType
    {
        Normal,
        Stack
    }

    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(GraphicRaycaster))]
    public class UIPanel : UIView
    {
        [Space(5)]
        [Header("最下層遮擋其他UI的遮罩(非必要)")]
        [SerializeField]
        private MaskWidget _mask;

        [Space(10)]
        [Header("基本配置")]
        public string SortingLayer = "Default";

        public UIPanelType PanelType              = UIPanelType.Stack;
        public bool        IsNeedHideWhenNotFocus = false;
        public bool        IsHideOtherInStackPanels           = false;

        private Canvas _canvas;

        private void OnEnable()
        {
            _canvas                 = GetComponent<Canvas>();
            _canvas.overrideSorting = true;
        }

        public sealed override void Init(IUISystem uiSystem)
        {
            if (_mask != null)
            {
                _mask.Init(uiSystem);
            }

            base.Init(uiSystem);
        }

        public sealed override Task StartShow(bool isPlayAnimation = true)
        {
            if (_mask != null)
            {
                _mask.SetActive(true);
            }

            return base.StartShow(isPlayAnimation);
        }

        public sealed override Task StartHide(bool isPlayAnimation = true)
        {
            if (_mask != null)
            {
                _mask.SetActive(false);
            }

            return base.StartHide(isPlayAnimation);
        }

        public void PauseFocus()
        {
            OnPauseFocus();
        }

        public void ResumeFocus()
        {
            OnResumeFocus();
        }

        protected virtual void OnPauseFocus()
        {
            
        }
        
        protected virtual void OnResumeFocus()
        {
            
        }

        public void SetSortingOrder(int order)
        {
            _canvas.sortingOrder = order;
        }

        public int GetSortingOrder() => _canvas.sortingOrder;
    }
}