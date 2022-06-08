using basic;
using Common;
using FairyGUI;
using Network;
using Service;
using UnityEngine;

public class Main : MonoBehaviour
{
    void Start()
    {
        Log.Init("Client");
        basicBinder.BindAll();
        UIManager.Instance.Register<WindowLogin>("basic", "WndLogin");

        UserService.Instance.Init();

        GRoot.inst.SetContentScaleFactor(1024, 768, UIContentScaler.ScreenMatchMode.MatchHeight);
        UIManager.Instance.Show<WindowLogin>();
    }
}