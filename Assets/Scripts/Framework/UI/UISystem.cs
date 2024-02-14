using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

#pragma warning disable CS4014

namespace Framework.UI
{
    public class UISystem : IUISystem
    {
        private GameObject _root;

        private UISettings _uiSettings;

        private string _focusPanelId;

        private Dictionary<string, UIPanel> _panelsLoaded = new Dictionary<string, UIPanel>();

        private Stack<string> _panelStack   = new Stack<string>();
        private List<string>  _activePanels = new List<string>();

        private Dictionary<string, List<int>> _sortingOrders = new Dictionary<string, List<int>>();

        private Dictionary<int, Action<UIData>> _events = new Dictionary<int, Action<UIData>>();

        #region - Init -

        public void Init(UISettings uiSettings)
        {
            _uiSettings = uiSettings;

            var canvasObj = new GameObject("UI Canvas");
            Object.DontDestroyOnLoad(canvasObj);

            var canvas       = canvasObj.AddComponent<Canvas>();
            var canvasScaler = canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();

            canvas.renderMode                = RenderMode.ScreenSpaceOverlay;
            canvasScaler.uiScaleMode         = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = _uiSettings.Basic.CanvasResolution;

            _root = new GameObject("Root");
            _root.transform.SetParent(canvas.transform);

            var rootRect = _root.AddComponent<RectTransform>();
            rootRect.anchorMin = Vector2.zero;
            rootRect.anchorMax = Vector2.one;
            rootRect.offsetMax = Vector2.zero;
            rootRect.offsetMin = Vector2.zero;

            InitSortingLayers();
        }

        #endregion

        #region - Show & Load -

        public async Task ShowPanel(string panelId, UIData data = null, bool isPlayAnimation = true)
        {
            if (panelId == _focusPanelId) return;

            if (IsPanelActive(panelId))
            {
                HidePanel(panelId, false);
            }

            if (_panelsLoaded.ContainsKey(panelId))
            {
                await ImplementShowPanel(panelId, data, isPlayAnimation);
                return;
            }

            var panelSpawned = LoadPanel(panelId);
            await ImplementShowPanel(panelId, data, isPlayAnimation);
        }

        private async Task ImplementShowPanel(string panelId, UIData data = null, bool isPlayAnimation = true)
        {
            var panel = _panelsLoaded[panelId];

            // 加入 Stack
            if (panel.PanelType == UIPanelType.Stack)
            {
                PushToStack(panelId);

                if (_focusPanelId != null)
                {
                    _panelsLoaded[_focusPanelId].PauseFocus();
                }
            }

            // 設定 Sorting Order
            var sortingOrder = GetNewSortingOrder(panelId);
            panel.SetSortingOrder(sortingOrder);

            // 設定 UIData
            if (data != null)
            {
                panel.SetData(data);
            }

            // 動畫
            await panel.StartShow(isPlayAnimation);

            // 是否隱藏其他在 Stack 中的 Panel
            if (panel.IsHideOtherInStackPanels)
            {
                foreach (var panelInStack in _panelStack)
                {
                    if (panelInStack == panelId) continue;
                    _panelsLoaded[panelInStack].gameObject.SetActive(false);
                }
            }

            // 設定 Focus
            if (panel.PanelType == UIPanelType.Stack)
            {
                SetPanelFocus(panelId);
            }

            // 加入 _panelShowing
            _activePanels.Add(panelId);
        }

        private UIPanel LoadPanel(string panelId)
        {
            UIPanel prefab = GetPanelPrefab(panelId);

            if (prefab == null)
            {
                Debug.Log("Con not Find Panel Prefab");
                return null;
            }

            // TODO 改成工廠
            var panelSpawned = Object.Instantiate(prefab, _root.transform);

            _panelsLoaded[panelId] = panelSpawned;
            panelSpawned.gameObject.SetActive(false);
            panelSpawned.Init(this);
            return panelSpawned;
        }

        // TODO 有資源管理模塊再串接
        private UIPanel GetPanelPrefab(string panelId)
        {
            UIPanel prefab = null;

            foreach (var panelSetting in _uiSettings.PanelSettings)
            {
                if (panelSetting.PanelId == panelId)
                {
                    prefab = panelSetting.Panel;
                    break;
                }
            }

            return prefab;
        }

        #endregion

        #region - Hide -

        public async Task HidePeekPanel(bool isPlayAnimation = true)
        {
            if (!IsStackLeftOnePanel())
            {
                var panelId = _panelStack.Pop();
                await HidePanel(panelId, isPlayAnimation);
            }

            if (!IsStackEmpty())
            {
                var currentPeekPanelId = _panelStack.Peek();
                if (_panelsLoaded.TryGetValue(currentPeekPanelId, out var currentPeekPanel))
                {
                    if (currentPeekPanel.IsNeedHideWhenNotFocus)
                    {
                        currentPeekPanel.StartShow();
                    }

                    currentPeekPanel.ResumeFocus();
                    SetPanelFocus(currentPeekPanelId);
                }
            }
            else
            {
                SetPanelFocus(null);
            }
        }

        public async Task HidePanel(string panelId, bool isPlayAnimation = true)
        {
            if (_panelsLoaded.TryGetValue(panelId, out var panelLoaded))
            {
                // 回復隱藏的 Panel
                if (panelLoaded.IsHideOtherInStackPanels)
                {
                    foreach (var panelInStack in _panelStack)
                    {
                        if (panelInStack == panelId) continue;
                        _panelsLoaded[panelInStack].gameObject.SetActive(true);
                    }
                }

                await panelLoaded.StartHide(isPlayAnimation);

                // 要先移除 SortingOrder，才可以移除 Active PanelId
                RemoveSortingOrder(panelId);

                _activePanels.Remove(panelId);
            }
        }

        #endregion

        #region - Check -

        public bool IsPanelActive(string panelId)
        {
            return _activePanels.Contains(panelId);
        }

        #endregion

        #region - Stack -

        private void PushToStack(string panelId)
        {
            if (!IsStackLeftOnePanel())
            {
                var currentPeekPanelId = _panelStack.Peek();
                if (_panelsLoaded.TryGetValue(currentPeekPanelId, out var currentPeekPanel))
                {
                    if (currentPeekPanel.IsNeedHideWhenNotFocus)
                    {
                        currentPeekPanel.StartHide();
                    }
                }
            }

            if (_panelStack.Contains(panelId))
            {
                PopPanelsAbove(panelId);
                return;
            }

            _panelStack.Push(panelId);
        }

        // Pop 掉指定 panelId 之後的所有 PanelId
        private void PopPanelsAbove(string panelId)
        {
            int index = 0;
            int i     = 0;
            foreach (var item in _panelStack)
            {
                if (item == panelId) index = i;
                i++;
            }

            int popCount = (_panelStack.Count - 1) - index;

            for (int j = 0; j < popCount; j++)
            {
                HidePeekPanel(false);
            }
        }

        private bool IsStackLeftOnePanel() => _panelStack.Count <= 1;

        private bool IsStackEmpty() => _panelStack.Count <= 0;

        #endregion

        #region - Sorting Order -

        private void InitSortingLayers()
        {
            for (var i = 0; i < _uiSettings.SortingLayers.Count; i++)
            {
                var sortingLayer = _uiSettings.SortingLayers[i];
                _sortingOrders[sortingLayer] = new List<int>();
            }
        }

        private int GetNewSortingOrder(string panelId)
        {
            var panel = _panelsLoaded[panelId];

            var sortingOrdersList = _sortingOrders[panel.SortingLayer];
            var panelSortingOrder = panel.GetSortingOrder();

            if (IsPanelActive(panelId))
            {
                sortingOrdersList.Remove(panelSortingOrder);
            }

            int newSortingOrder = 0;

            if (sortingOrdersList.Count == 0)
            {
                // 定義自訂 Sorting Layer的 Sorting Order 區間
                var division = (short.MaxValue - short.MinValue) / _uiSettings.SortingLayers.Count;
                newSortingOrder = division * _uiSettings.SortingLayers.IndexOf(panel.SortingLayer) + short.MinValue;

                sortingOrdersList.Add(newSortingOrder);
            }
            else
            {
                var currentTopOrder = sortingOrdersList[^1];
                newSortingOrder = currentTopOrder + 1;

                sortingOrdersList.Add(newSortingOrder);
            }

            return newSortingOrder;
        }

        private void RemoveSortingOrder(string panelId)
        {
            var panel = _panelsLoaded[panelId];

            var sortingOrdersList = _sortingOrders[panel.SortingLayer];
            var panelSortingOrder = panel.GetSortingOrder();

            if (IsPanelActive(panelId))
            {
                sortingOrdersList.Remove(panelSortingOrder);
            }
        }

        #endregion

        #region - Focus -

        private void SetPanelFocus(string panelId)
        {
            if (panelId == null)
            {
                _focusPanelId = null;
                return;
            }

            if (_panelsLoaded.TryGetValue(panelId, out var panel))
            {
                _focusPanelId = panelId;
            }
        }

        #endregion

        #region - Event Bus -

        public void RegisterEventHandler(int eventId, Action<UIData> eventHandler)
        {
            if (_events.TryGetValue(eventId, out var handler))
            {
                handler += eventHandler;
            }
            else
            {
                _events[eventId] = eventHandler;
            }
        }

        public void UnregisterEventHandler(int eventId, Action<UIData> eventHandler)
        {
            if (_events.TryGetValue(eventId, out var handler))
            {
                handler -= eventHandler;
            }
        }

        public void InvokeEvent(int eventId, UIData data)
        {
            if (_events.TryGetValue(eventId, out var eventHandlers))
            {
                eventHandlers?.Invoke(data);
            }
        }

        #endregion
    }
}