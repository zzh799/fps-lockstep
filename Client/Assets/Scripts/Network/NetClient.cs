using System;
using System.Collections;
using System.Collections.Generic;
using Network;
using UnityEngine;

public class NetClient : MonoSingleton<NetClient>
{
    private string address = "127.0.0.1:7777";
//    private string address = "10.200.10.192:3655";

    public static long starttime = 0;


    protected override void Init()
    {
        NetworkManager network = NetworkManager.Instance;
        network.InitService(NetworkProtocol.KCP);
        network.Connect(address);
        network.OnConnect += OnConnect;
        network.OnError += OnError;
    }

    private void OnError(int e)
    {
        Debug.LogError("net error：" + e);
    }

    private void OnConnect(int c)
    {
        Debug.Log("NetClient:OnConnect" + c);
    }
}