using Framework.UI;
using UnityEngine;

public class GameEntry : MonoBehaviour
{
    private IUISystem _uiSystem;

    private void Start()
    {
        _uiSystem = new UISystem();
    }
    
    
}
