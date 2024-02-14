using System.Collections.Generic;
using UnityEngine;

namespace Framework.UI
{
    [CreateAssetMenu(fileName = "ViewAnimationClip", menuName = "UIFramework/ViewAnimationClip")]
    public class UIAnimationClip : ScriptableObject
    {
        public List<PositionAnimation> PositionAnimations;
        public List<RotationAnimation> RotationAnimations;
        public List<ScaleAnimation>    ScaleAnimations;
        public List<AlphaAnimation>    AlphaAnimations;
    }
}