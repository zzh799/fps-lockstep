using System;
using System.Collections.Generic;
using Common;
using Manager;


public class WindowDefine
{
    public string packckageName;
    public string resName;
    public bool Cache;
    public BaseWindow Instance;
}

public class UIManager : Singleton<UIManager>
{
    private Dictionary<Type, WindowDefine> _WindowDefines = new Dictionary<Type, WindowDefine>();

    public UIManager()
    {
    }

    public void Register<T>(string packageName, string resName) where T : BaseWindow
    {
        _WindowDefines.Add(typeof(T),
            new WindowDefine() {packckageName = packageName, resName = resName, Cache = false});
    }

    public T Show<T>() where T : BaseWindow, new()
    {
        Type type = typeof(T);
        if (_WindowDefines.TryGetValue(type, out WindowDefine info))
        {
            if (info.Instance == null)
            {
                info.Instance = new T();
            }

            info.Instance.Show();
        }

        return default;
    }

    public void Close<T>() where T : BaseWindow
    {
        Type type = typeof(T);
        if (_WindowDefines.TryGetValue(type, out WindowDefine define))
        {
            if (define == null || define.Instance == null)
            {
                return;
            }

            define.Instance.Hide();
        }
    }


    public T Get<T>() where T : BaseWindow
    {
        if (_WindowDefines.TryGetValue(typeof(T), out WindowDefine baseWindow))
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
}