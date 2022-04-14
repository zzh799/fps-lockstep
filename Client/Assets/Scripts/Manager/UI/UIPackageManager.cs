using System.Collections.Generic;
using Common;
using UnityEngine;
using FairyGUI;



/// <summary>
/// Extend the ability of UIPackage
/// </summary>
public class UIPackageManager:Singleton<UIPackageManager>
{
    //全局UI包引用计数
    private static Dictionary<string, int> _packageRefs = new Dictionary<string, int>();

    public static void LoadPackage(string fileName,bool hasRes = true)
    {
        if (AppConst.AssetBundleMode)
        {
            try
            {
                string bundleName = AppConst.UIDir.ToLower() + fileName.ToLower();
                AssetBundle des_bundle = null;
                des_bundle = FileHelper.LoadABFromFile(bundleName);
                if (des_bundle)
                {
                    AssetBundle res_bundle = null;
                    if (hasRes)
                    {
                        res_bundle = FileHelper.LoadABFromFile(bundleName + "_res");
                        if (null == res_bundle) {
                            Debug.LogError("ab包：" + fileName + "_res 不存在，请检查！");
                        }
                    }
                    if (null != res_bundle)
                    {
                        UIPackage.AddPackage(des_bundle, res_bundle);
                    }
                    else
                    {
                        UIPackage.AddPackage(des_bundle, des_bundle);
                    }
                }
                else
                {
                    Debug.LogError("ab包：" + fileName + "不存在，请检查！");
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError("加载ab包：" + fileName + "报错:"+ex.Message);
            }
        }
        else
        {
            try {
                UIPackage.AddPackage("Assets/BundleResources/UI/" + fileName);
            }
            catch (System.Exception ex)
            {
                Debug.LogError("包：" + fileName + "报错:"+ex.Message);
            }

        }
    }

    // 增加包的引用计数
    private static void Retain(string pkgName)
    {
        if (!_packageRefs.ContainsKey(pkgName))
            _packageRefs[pkgName] = 0;

        _packageRefs[pkgName]++;
    }

    // 减少包的引用计数
    private static bool Release(string pkgName)
    {
        if (_packageRefs.ContainsKey(pkgName))
        {
            if (--_packageRefs[pkgName] <= 0)
            {
                _packageRefs[pkgName] = 0;
                return true;
            }
        }
        return false;
    }

    // 判断包是否已加载过
    public static bool IsLoaded(string pkgName)
    {
        return UIPackage.GetByName(pkgName) != null;
    }

    // 创建包对象
    public static GObject CreateObject(string pkgName, string resName)
    {
        Retain(pkgName);
        var obj = UIPackage.CreateObject(pkgName, resName);
        if (obj == null){
            Release(pkgName);
        }
        return obj;
    }

    // 移除包释放资源
    public static bool RemovePackage(string pkgName,bool clear=false)
    {
        if (clear){
            if (_packageRefs.ContainsKey(pkgName)){
                _packageRefs[pkgName] = 0;
            }
            if (null != UIPackage.GetByName(pkgName)){
                UIPackage.RemovePackage(pkgName);
            }
            return true;
        }
        else{
            if (Release(pkgName))
            {
                if (null != UIPackage.GetByName(pkgName)){
                    UIPackage.RemovePackage(pkgName);
                }
                return true;
            }
            return false;
        }
    }

    // 移除所有包资源
    public static void Clear(List<string> ignores, bool allowDestroyingAssets = true)
    {
        var packages = UIPackage.GetPackages();
        if (packages != null)
        {
            for (int i = 0; i < packages.Count;)
            {
                if (packages[i].name.Contains("&res")) {
                    i++;
                }
                else
                {
                    if (!ignores.Contains(packages[i].name))
                    {
                        if (_packageRefs.ContainsKey(packages[i].name))
                        {
                            _packageRefs[packages[i].name] = 0;
                        }
                        UIPackage.RemovePackage(packages[i].name);
                    }
                    else
                    {
                        ++i;
                    }
                }
            }
        }
    }
}

