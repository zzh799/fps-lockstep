using System;
using System.Collections.Generic;
using Common;
using FairyGUI;
using UnityEngine;

namespace Manager
{
    public class UIManager : Singleton<UIManager>
    {
        class UIElement
        {
            public string packckageName;
            public string resName;
            public bool Cache;
            public BaseWindow Instance;
        }

        private Dictionary<Type, UIElement> _UIDict = new Dictionary<Type, UIElement>();

        public UIManager()
        {
            
        }

        public void Register<T>(string packageName,string resName) where T : BaseWindow
        {
            _UIDict.Add(typeof(T), new UIElement(){packckageName = packageName,resName = resName,Cache = false});
        }

        public T Show<T>() where T : BaseWindow
        {
            Type type = typeof(T);
            if (_UIDict.ContainsKey(type))
            {
                UIElement info = _UIDict[type];
                if (info.Instance != null)
                {
                    info.Instance.Show();
                }
                else
                {
                    GObject gObject = UIPackageManager.CreateObject(info.packckageName, info.resName);
                    if (gObject !=null)
                    {
                        info.Instance = (BaseWindow) gObject;
                    }
                }
            }
            return default;
        }

        public T Get<T>() where T : BaseWindow
        {
            if (_UIDict.TryGetValue(typeof(T), out UIElement baseWindow))
            {
               return baseWindow.Instance as T;
            }
            return null;
        }
        
        public bool IsOpen<T>() where T : BaseWindow
        {
            T window = Get<T>();
            if (window != null)
            {
                return window.isShowing;
            }

            return false;
        }

        public void Close<T>() where T : BaseWindow
        {
            Type type = typeof(T);
            if (_UIDict.ContainsKey(type))
            {
                UIElement uiElement = _UIDict[type];
                if (uiElement == null || uiElement.Instance == null)
                {
                    return;
                }
                if (uiElement.Cache)
                {
                    uiElement.Instance.Hide();
                }
                else
                {
                    uiElement.Instance.Dispose();
                    uiElement.Instance = null;
                }
            }
        }
    }
}