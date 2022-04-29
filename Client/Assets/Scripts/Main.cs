using basic;
using FairyGUI;
using Network;
using UnityEngine;

public class Main:MonoBehaviour
{
    void Start()
    {
        basicBinder.BindAll();
        UIManager.Instance.Register<WindowLogin>("basic","WndLogin");
        
        
        UIManager.Instance.Show<WindowLogin>();

    }
}  