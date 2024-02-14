using Framework.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class TestPanel : UIPanel
    {
        [SerializeField]
        private Button _closeBtn;

        private void Start()
        {
            _closeBtn.onClick.AddListener(OnCloseBtnClicked);
        }

        private void OnDestroy()
        {
            _closeBtn.onClick.RemoveAllListeners();
        }

        private void OnCloseBtnClicked()
        {
            _uiSystem.HidePeekPanel();
        }
    }
}
