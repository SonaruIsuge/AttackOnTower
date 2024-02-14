using Framework.UI;
using UnityEngine;

namespace Dev.Wilson
{
    public class UITest : MonoBehaviour
    {
        public static UITest Instance { get; private set; }

        public IUISystem UISystem { get; set; }

        [SerializeField]
        private UISettings _uiSettings;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            var uiSystem = new UISystem();
            uiSystem.Init(_uiSettings);
            UISystem = uiSystem;

            UISystem.ShowPanel("LobbyPanel");
        }
    }
}