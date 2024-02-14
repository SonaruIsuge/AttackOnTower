using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;

namespace Framework.UI
{
    [Serializable]
    public class UIAnimationPlayer
    {
        [SerializeField] private GameObject _target;

        private Sequence _sequence;

        private RectTransform _rectTransform;
        private CanvasGroup   _canvasGroup;

        public async Task<Sequence> PlayClip(UIAnimationClip clip)
        {
            if (_target == null) return null;
            if (clip == null) return null;

            if (_rectTransform == null) _rectTransform = _target.GetComponent<RectTransform>();
            if (_canvasGroup == null) _canvasGroup     = _target.GetComponent<CanvasGroup>();

            if (_sequence != null && _sequence.IsPlaying())
            {
                _sequence.Kill();
            }

            _sequence = DOTween.Sequence();

            AddViewTweenToSequence(clip.PositionAnimations);
            AddViewTweenToSequence(clip.RotationAnimations);
            AddViewTweenToSequence(clip.ScaleAnimations);
            AddViewTweenToSequence(clip.AlphaAnimations);

            await _sequence.AsyncWaitForCompletion();

            return _sequence;
        }

        public void Pause()
        {
            _sequence.Pause();
        }

        private void AddViewTweenToSequence<T>(List<T> viewAnimations) where T : UIAnimation
        {
            if (viewAnimations == null) return;
            if (viewAnimations.Count <= 0) return;

            for (var i = 0; i < viewAnimations.Count; i++)
            {
                var animation = viewAnimations[i];
                if (animation.EndTime - animation.StartTime <= 0) continue;

                if (i == 0)
                {
                    SetToStartStatus(animation);
                }

                var duration = animation.EndTime - animation.StartTime;

                switch (animation)
                {
                    case PositionAnimation positionAnimation:
                    {
                        var tween = DOTween.To(
                            () => positionAnimation.StartPosition,
                            x => _rectTransform.anchoredPosition = x,
                            positionAnimation.EndPosition,
                            duration);
                        SetEase(tween, animation);
                        _sequence.Insert(animation.StartTime, tween);
                        break;
                    }
                    case RotationAnimation rotationAnimation:
                    {
                        var tween = DOTween.To(
                            () => rotationAnimation.StartAngle,
                            x => _rectTransform.localRotation = Quaternion.Euler(0, 0, x),
                            rotationAnimation.EndAngle,
                            duration);

                        SetEase(tween, animation);
                        _sequence.Insert(animation.StartTime, tween);
                        break;
                    }
                    case ScaleAnimation scaleAnimation:
                    {
                        var tween = DOTween.To(
                            () => scaleAnimation.StartScale,
                            x => _rectTransform.localScale = x,
                            scaleAnimation.EndScale,
                            duration);

                        SetEase(tween, animation);
                        _sequence.Insert(animation.StartTime, tween);
                        break;
                    }
                    case AlphaAnimation alphaAnimation:
                    {
                        var tween = DOTween.To(
                            () => alphaAnimation.StartAlpha,
                            x => _canvasGroup.alpha = x,
                            alphaAnimation.EndAlpha,
                            duration);

                        SetEase(tween, animation);
                        _sequence.Insert(animation.StartTime, tween);
                        break;
                    }
                }
            }
        }

        private void SetEase<T1, T2, TPluginType>(TweenerCore<T1, T2, TPluginType> tween, UIAnimation uiAnimation)
            where TPluginType : struct, IPlugOptions
        {
            var isUseEase = uiAnimation.Ease != Ease.Unset;

            if (isUseEase)
            {
                tween.SetEase(uiAnimation.Ease);
            }
            else
            {
                tween.SetEase(uiAnimation.AnimationCurve);
            }
        }

        private void SetToStartStatus(UIAnimation animation)
        {
            switch (animation)
            {
                case PositionAnimation positionAnimation:
                {
                    _rectTransform.anchoredPosition = positionAnimation.StartPosition;
                    break;
                }
                case RotationAnimation rotationAnimation:
                {
                    _rectTransform.localRotation = Quaternion.Euler(0, 0, rotationAnimation.StartAngle);
                    break;
                }
                case ScaleAnimation scaleAnimation:
                {
                    _rectTransform.localScale = scaleAnimation.StartScale;
                    break;
                }
                case AlphaAnimation alphaAnimation:
                {
                    _canvasGroup.alpha = alphaAnimation.StartAlpha;
                    break;
                }
            }
        }
    }
}