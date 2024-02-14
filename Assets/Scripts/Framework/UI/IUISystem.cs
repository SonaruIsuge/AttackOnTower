using System;
using System.Threading.Tasks;

namespace Framework.UI
{
    public interface IUISystem
    {
        Task ShowPanel(string panelId, UIData data = null, bool isPlayAnimation = true);

        Task HidePanel(string panelId, bool isPlayAnimation = true);

        Task HidePeekPanel(bool isPlayAnimation = true);

        bool IsPanelActive(string panelId);

        void RegisterEventHandler(int eventId, Action<UIData> eventHandler);

        void UnregisterEventHandler(int eventId, Action<UIData> eventHandler);

        void InvokeEvent(int eventId, UIData data);
    }
}