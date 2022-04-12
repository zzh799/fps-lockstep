using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine.U2D;

#if UNITY_EDITOR
using UnityEditor;
using System.Linq;
using System.Xml;
#endif


public class GameObjectHelper
{
    
    /// <summary>
    /// 搜索子物体组件-GameObject版
    /// </summary>
    public static T Get<T>(GameObject go, string subnode) where T : Component
    {
        if (go != null)
        {
            Transform sub = go.transform.Find(subnode);
            if (sub != null) return sub.GetComponent<T>();
        }

        return null;
    }

    /// <summary>
    /// 搜索子物体组件-Transform版
    /// </summary>
    public static T Get<T>(Transform go, string subnode) where T : Component
    {
        if (go != null)
        {
            Transform sub = go.Find(subnode);
            if (sub != null) return sub.GetComponent<T>();
        }

        return null;
    }

    /// <summary>
    /// 搜索子物体组件-Component版
    /// </summary>
    public static T Get<T>(Component go, string subnode) where T : Component
    {
        return go.transform.Find(subnode).GetComponent<T>();
    }

    /// <summary>
    /// 添加组件
    /// </summary>
    public static T Add<T>(GameObject go) where T : Component
    {
        if (go != null)
        {
            T[] ts = go.GetComponents<T>();
            for (int i = 0; i < ts.Length; i++)
            {
                if (ts[i] != null) GameObject.Destroy(ts[i]);
            }

            return go.gameObject.AddComponent<T>();
        }

        return null;
    }

    /// <summary>
    /// 添加组件
    /// </summary>
    public static T Add<T>(Transform go) where T : Component
    {
        return Add<T>(go.gameObject);
    }

    /// <summary>
    /// 查找子对象
    /// </summary>
    public static GameObject Child(GameObject go, string subnode)
    {
        return Child(go.transform, subnode);
    }

    /// <summary>
    /// 查找子对象
    /// </summary>
    public static GameObject Child(Transform go, string subnode)
    {
        Transform tran = go.Find(subnode);
        if (tran == null) return null;
        return tran.gameObject;
    }

    /// <summary>
    /// 取平级对象
    /// </summary>
    public static GameObject Peer(GameObject go, string subnode)
    {
        return Peer(go.transform, subnode);
    }

    /// <summary>
    /// 取平级对象
    /// </summary>
    public static GameObject Peer(Transform go, string subnode)
    {
        Transform tran = go.parent.Find(subnode);
        if (tran == null) return null;
        return tran.gameObject;
    }


    /// <summary>
    /// 清除所有子节点
    /// </summary>
    public static void ClearChild(Transform go)
    {
        if (go == null) return;
        for (int i = go.childCount - 1; i >= 0; i--)
        {
            GameObject.Destroy(go.GetChild(i).gameObject);
        }
    }

    /// <summary>
    /// 清理内存
    /// </summary>
    public static void ClearMemory()
    {
        GC.Collect();
        Resources.UnloadUnusedAssets();
    }

#if UNITY_IOS
        [DllImport("__Internal")]
        private static extern string GetSysFontName();
#endif

    /// <summary>
    /// 系统字体
    /// </summary>
    public static string SystemFontName()
    {
#if UNITY_IOS
            return GetSysFontName();
#else
        return "";
#endif
    }
    
    /// <summary>
    /// 指定类型是否存在对应字段
    /// </summary>
    /// <param name="cs_type"></param>
    /// <param name="field_type"></param>
    /// <returns></returns>
    public static bool GetFieldChecker(object instance, string field_name)
    {
        var info = instance.GetType().GetProperty(field_name);
        if (info != null)
        {
            return info.GetValue(instance, null) != null;
        }

        return false;
    }
}