using Dev.Wilson;
using Framework.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class LobbyPanel : UIPanel
    {
        [SerializeField]
        private Button _testPanelBtn;

        protected override void OnStartShow()
        {
            _testPanelBtn.onClick.AddListener(OnTestPanelBtnClicked);
        }

        protected override void OnStartHide()
        {
            _testPanelBtn.onClick.RemoveAllListeners();
        }

        private void OnTestPanelBtnClicked()
        {
            UITest.Instance.UISystem.ShowPanel("TestPanel");
        }
    }
}