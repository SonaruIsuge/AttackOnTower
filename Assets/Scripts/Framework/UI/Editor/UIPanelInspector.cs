using UnityEditor;
using UnityEngine;

namespace Framework.UI.Editor
{
    [CustomEditor(typeof(UIView), true)] 
    public class UIPanelInspector: UnityEditor.Editor
    {
        private UIView _target; 
        
        public void OnEnable()
        {
            _target = target as UIView; 
        }

        public override void OnInspectorGUI()
        {
            if (GUILayout.Button("測試播放Show"))
            {
                _target.StartShow();
            }
            if (GUILayout.Button("測試播放Hide"))
            {
                _target.StartHide();
            }
            
            base.OnInspectorGUI(); 
        }
    }
}