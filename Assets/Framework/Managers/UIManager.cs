using System.Collections.Generic;
using Scripts.UI;
using UnityEngine;
using UnityEngine.UI;
using UnityUtils;

namespace Managers
{
    public enum UICategory
    {
        SceneUI,
        PopupUI,
        //게임이 커지면 씬별로 나눠도 됌
    }

    public class UIManager : Singleton<UIManager>
    {
        private readonly Dictionary<UICategory, string> _uiPrefixes = new()
        {
            { UICategory.SceneUI, "UI/Scene/" },
            { UICategory.PopupUI, "UI/Popup/" },
        };


        private int _currentOrder = 10; // 현재까지 최근에 사용된 오더
        private readonly Dictionary<string, UIPopup> _activeUIs = new();
        public int CurrentPopupCount => _activeUIs.Count;

        public UIScene CurrentSceneUI { get; private set; }

        private GameObject Root
        {
            get
            {
                var root = GameObject.Find("@UI_Root") ?? new GameObject { name = "@UI_Root" };
                return root;
            }
        }
        

        public void SetCanvas(GameObject go, bool sort = true)
        {
            var canvas = go.GetOrAdd<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.overrideSorting = true;
            canvas.sortingOrder = sort ? _currentOrder++ : 0;
            go.GetOrAdd<GraphicRaycaster>();
        }

        public T ShowUI<T>( UICategory category = UICategory.SceneUI) where T : UIScene
        {
            return ShowUI<T>(typeof(T).Name,category);
        }

        public T ShowPopup<T>( UICategory category = UICategory.PopupUI) where T : UIPopup
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            return ShowUI<T>(typeof(T).Name,category);
        }
        T ShowUI<T>(string uiName, UICategory category = UICategory.PopupUI) where T : UIBase
        {
            if (_activeUIs.TryGetValue(uiName, out var existingUI))
            {
                existingUI.Show();
                return existingUI as T;
            }

            string uiPath = _uiPrefixes[category];
            var prefab = LoadUIResource(uiPath, uiName);
            if (prefab == null)
                return null;

            return CreateUIInstance<T>(prefab, uiName);
        }

        private T CreateUIInstance<T>(GameObject prefab, string uiName) where T : UIBase
        {
            var obj = Instantiate(prefab, Root.transform);
            obj.name = uiName;
            var uiComponent = EnableUIComponent<T>(obj, uiName);
            

            return uiComponent;
        }

        private GameObject LoadUIResource(string path, string resourceName)
        {
            var resource = Resources.Load<GameObject>($"{path}{resourceName}");
            if (resource == null)
                Debug.LogError($"UI Resource '{resourceName}' not found in path '{path}'");
            return resource;
        }
        
        public void HideUI<T>() where T : UIBase
        {
            string uiName = typeof(T).Name;

            if (_activeUIs.TryGetValue(uiName, out var ui))
            {
                ui.Hide();
            }
            else if(CurrentSceneUI is T)
            {
                CurrentSceneUI.Hide();
            }
            else
            {
                Debug.LogWarning($"UIManager: {uiName} UI가 활성화 상태가 아닙니다.");
            }
        }

        private T EnableUIComponent<T>(GameObject obj,string uiName) where T : UIBase
        {
            var uiComponent = obj.GetOrAdd<T>();
            if (uiComponent is UIPopup popup)
            {
                Debug.Log($"Open UI: {uiName}");
                _activeUIs[uiName] = popup;
            }
            else if (uiComponent is UIScene hud)
            {
                CurrentSceneUI = hud;
            }

            uiComponent.Initialize();
            uiComponent.Show();
            return uiComponent;
        }

        public void ClosePopup<T>()
        {
            ClosePopup(typeof(T).Name);
        }
        public void ClosePopup(string uiName)
        {
            // 딕셔너리에서 제거시도
            if (!_activeUIs.TryGetValue(uiName, out var ui))
            {
                Debug.LogWarning($"Close UI Failed: UI '{uiName}' not found.");
                return;
            }

            ui.Hide();
            
            //ClosePopup(ui);
        }
        public void ClosePopup(UIPopup ui)
        {
            if (CurrentPopupCount == 0)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }

            Debug.Log($"Close UI: {ui.name}");
            _activeUIs.Remove(ui.name);
            Destroy(ui.gameObject);
            _currentOrder--;
        }
        
        public void CloseAllPopup()
        {
            foreach (var key in _activeUIs.Keys)
            {
                if (_activeUIs[key] is { } popup)
                {
                    ClosePopup(popup);
                }
            }

        }
    }
}