using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

#pragma warning disable CS4014

namespace Framework.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public abstract class UIView : MonoBehaviour
    {
        [Serializable]
        private class UISubview
        {
            public UIView Subview;
            public bool   IsAutoPlayShowAnimation = true;
            public bool   IsAutoPlayHideAnimation = true;
        }
        
        [Space(5)] 
        [Header("動畫")] 
        [SerializeField]
        private UIAnimationPlayer uiAnimationPlayer;

        [Space(5)]
        [SerializeField] 
        private UIAnimationClip _showAnimation;
        [SerializeField]            
        private UIAnimationClip _hideAnimation;

        [Space(5)]
        [Header("子UIView")] 
        [SerializeField]
        private List<UISubview> _subViews;

        protected RectTransform _rectTransform;

        protected IUISystem _uiSystem;
        
        private UIData _uiData;

        #region - Init -

        public virtual void Init(IUISystem uiSystem)
        {
            _uiSystem = uiSystem;
            
            _rectTransform = GetComponent<RectTransform>();
            
            foreach (var subView in _subViews)
            {
                subView.Subview.Init(_uiSystem);
            }

            OnInit();
        }

        protected virtual void OnInit()
        {
        }

        #endregion

        #region - Show & Hide -

        public virtual async Task StartShow(bool isPlayAnimation = true)
        {
            OnStartShow();

            foreach (var subView in _subViews)
            {
                if (subView.IsAutoPlayShowAnimation)
                {
                    subView.Subview.StartShow(isPlayAnimation);
                }
            }
            
            gameObject.SetActive(true);
            
            if (isPlayAnimation)
            {
                await uiAnimationPlayer.PlayClip(_showAnimation);
            }
            
            OnShowFinish();
        }

        public virtual async Task StartHide(bool isPlayAnimation = true)
        {
            OnStartHide();

            foreach (var subView in _subViews)
            {
                if (subView.IsAutoPlayHideAnimation)
                {
                    subView.Subview.StartHide(isPlayAnimation);
                }
            }

            if (isPlayAnimation)
            {
                await uiAnimationPlayer.PlayClip(_hideAnimation);
            }

            OnHideFinish();

            gameObject.SetActive(false);
        }

        protected virtual void OnStartShow()
        {
        }

        protected virtual void OnStartHide()
        {
        }

        protected virtual void OnShowFinish()
        {
        }

        protected virtual void OnHideFinish()
        {
        }

        #endregion

        #region - UIData -

        public void SetData(UIData uiData)
        {
            _uiData = uiData;
            OnUIDataSet(_uiData);
        }

        protected virtual void OnUIDataSet(UIData uiData)
        {
        }

        #endregion
    }
}