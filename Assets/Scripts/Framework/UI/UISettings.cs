using System;
using System.Collections.Generic;
using UnityEngine;

namespace Framework.UI
{
    [Serializable]
    public class UIBasic
    {
        public Vector2 CanvasResolution = new Vector2(1920, 1080);
    }
    
    [Serializable]
    public class UIPanelSetting
    {
        public string  PanelId;
        public UIPanel Panel;
    }
    
    [CreateAssetMenu(fileName = "UISettings", menuName = "UIFramework/UISettings")]
    public class UISettings : ScriptableObject
    {
        public UIBasic              Basic;
        
        [Space(10)]
        public List<string>         SortingLayers;
        
        
        public List<UIPanelSetting> PanelSettings;
    }
}