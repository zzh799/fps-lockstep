using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;


public class FileHelper
{
    /// <summary>
    /// 计算文件的MD5值
    /// </summary>
    public static string md5file(string file)
    {
        try
        {
            FileStream fs = new FileStream(file, FileMode.Open);
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] retVal = md5.ComputeHash(fs);
            fs.Close();

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
            {
                sb.Append(retVal[i].ToString("x2"));
            }

            return sb.ToString();
        }
        catch (Exception ex)
        {
            throw new Exception("md5file() fail, error:" + ex.Message);
        }
    }


    // 删除目录
    public static void DelDir(string path, bool recursive)
    {
        if (Directory.Exists(path))
        {
            Directory.Delete(path, recursive);
        }
    }

    // 文件是否存在
    public static bool FileExists(string path)
    {
        return File.Exists(path);
    }

    // 删除文件
    public static void FileDelete(string path)
    {
        File.Delete(path);
    }

    // 文件夹是否存在
    public static bool DirExists(string path)
    {
        return Directory.Exists(path);
    }

    /// <summary>
    /// 创建目录
    /// </summary>
    public static void MakePathExist(string dataPath)
    {
        if (!Directory.Exists(dataPath)) Directory.CreateDirectory(dataPath);
    }

    /// <summary>
    /// 拷贝目录和目录下所有文件
    /// </summary>
    public static void CopyDir(string targetPath, string destPath)
    {
        MakePathExist(destPath);

        var dirInfo = new DirectoryInfo(targetPath);
        if (dirInfo != null)
        {
            var fileInfos = dirInfo.GetFileSystemInfos();
            foreach (var file in fileInfos)
            {
                if (file is DirectoryInfo)
                {
                    CopyDir(file.FullName, destPath + "/" + file.Name);
                }
                else
                {
                    File.Copy(file.FullName, destPath + "/" + file.Name, true);
                }
            }
        }
    }

    /// <summary>
    /// 取得数据存放目录
    /// </summary>
    public static string DataPath
    {
        get
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            string dataPath =
                Application.persistentDataPath.Substring(0, Application.persistentDataPath.LastIndexOf('/'));
            dataPath = dataPath + "/" + AppConst.PackageName;
            return dataPath;
#else
                return Application.persistentDataPath;
#endif
        }
    }

    public static string GetRelativePath()
    {
        if (Application.isEditor)
            return "file:///" + System.Environment.CurrentDirectory.Replace("\\", "/") + "/Assets/" +
                   AppConst.AssetDir + "/";
        else if (Application.isMobilePlatform || Application.isConsolePlatform)
            return "file:///" + DataPath;
        else // For standalone player.
            return "file:///" + Application.streamingAssetsPath + "/";
    }

    /// <summary>
    /// 取得行文本
    /// </summary>
    public static string GetFileText(string path)
    {
        return File.ReadAllText(path);
    }


    /*
     * 加载AssetBundle, 先从可写目录读取，再从只读目录读取
     * 
     */
    public static AssetBundle LoadABFromFile(string bundleName, bool fromPkg = false)
    {
        if (fromPkg)
        {
            string url = Application.streamingAssetsPath + '/' + bundleName;
            return AssetBundle.LoadFromFile(url);
        }
        else
        {
            string url = DataPath + "/" + AppConst.AssetDir + "/" + bundleName;
            if (!File.Exists(url))
            {
                url = Application.streamingAssetsPath + '/' + bundleName;
            }

            return AssetBundle.LoadFromFile(url);
        }
    }

    /*
     * 获取AB包文件URI 
     * 
     */
    public static string GetABFileURI(string bundleName)
    {
        if (bundleName.Contains("http")) return bundleName;
        string url = DataPath + "/" + AppConst.AssetDir + "/" + bundleName;
        if (!File.Exists(url))
        {
            url = Application.streamingAssetsPath + '/' + bundleName;
        }

        return url;
    }

    /*
     * 判断文件是否存在
     *
     */
    public static bool IsExistsFile(string path, string name)
    {
        string url = DataPath + "/" + AppConst.AssetDir + "/" + path + "/" + name;
        if (File.Exists(url))
        {
            return true;
        }
        else
        {
            url = Application.streamingAssetsPath + '/' + path + "/" + name;
            if (Application.platform == RuntimePlatform.Android)
            {
                bool isExists = IsExistsFile(path, name);
                return isExists;
            }
            else
            {
                if (File.Exists(url))
                {
                    return true;
                }
            }
        }

        return false;
    }

    // 判断文件是否存在于StreamingAssets中
    public static bool IsExistsFileInStreaming(string path, string name)
    {
        string url = Application.streamingAssetsPath + '/' + path + "/" + name;
        if (Application.platform == RuntimePlatform.Android)
        {
            bool isExists = IsExistsFile(path, name);
            return isExists;
        }
        else
        {
            if (File.Exists(url))
            {
                return true;
            }
        }

        return false;
    }


    /*
     * 解析uri，获取bundleName， assetName， subAssetName
     * 
     */
    public static void ParseBundleAssetURI(string uri, out string bundleName, out string assetName,
        out string subAssetName, char mark = '@', char subMark = '$')
    {
        uri = uri.ToLower();
        int markIdx = uri.LastIndexOf(mark);
        int subMarkIdx = uri.LastIndexOf(subMark);
        int lastPathSepIdx = uri.LastIndexOf("/");
        if (markIdx > 0)
            bundleName = uri.Remove(markIdx);
        else if (subMarkIdx > 0)
            bundleName = uri.Remove(subMarkIdx);
        else
            bundleName = uri;

        if (lastPathSepIdx < uri.Length - 1)
        {
            if (lastPathSepIdx > 0)
                assetName = uri.Substring(lastPathSepIdx + 1);
            else
                assetName = uri;

            // process http format
            bool isWebAb = uri.Contains("http");
            if (isWebAb)
            {
                string verParam = "?ver=";
                int verIdx = assetName.IndexOf(verParam, StringComparison.OrdinalIgnoreCase);
                if (verIdx > 0)
                    assetName = assetName.Substring(0, verIdx);
            }

            // process subAssets format
            if (subMarkIdx > 0)
            {
                string assetNameWithSubAssetName = assetName;
                int subIdx = assetNameWithSubAssetName.LastIndexOf(subMark);
                assetName = assetNameWithSubAssetName.Remove(subIdx);
                subAssetName = assetNameWithSubAssetName.Substring(subIdx + 1);
            }
            else
            {
                subAssetName = String.Empty;
            }
        }
        else
        {
            assetName = string.Empty;
            subAssetName = string.Empty;
        }
    }

    public static string GetBundleAssetURI(string uri, char mark = '@')
    {
        uri = uri.ToLower();
        int markIdx = uri.LastIndexOf(mark);
        if (markIdx > 0)
            return uri.Remove(markIdx);
        else
            return uri;
    }

    /*
     * 解析uri，获取bundleName和assetName
     * 
     */
    public static void ParseBundleAssetURI(string uri, out string bundleName, out string assetName, char mark = '@')
    {
        string subAssetName;
        ParseBundleAssetURI(uri, out bundleName, out assetName, out subAssetName);
    }

    /*
     * Editor模式下获取资源
     * 
     */
    public static T LoadAsset<T>(string uri, string withExt = "", string resRoot = "BundleResources")
        where T : UnityEngine.Object
    {
#if UNITY_EDITOR
        string rootPath = Path.Combine(Application.dataPath, resRoot);
        string assetFullName = Path.Combine(rootPath, uri);
        string assetName = Path.GetFileName(uri);
        string assetDirectoryName = Path.GetDirectoryName(assetFullName);
        string patten = assetName + withExt;
        if (withExt.Equals(string.Empty))
        {
            patten = assetName + ".*";
        }

        if (!Directory.Exists(assetDirectoryName))
        {
            Debug.LogWarning(assetDirectoryName + " is not found.");
            return null;
        }

        string[] files = Directory.GetFiles(assetDirectoryName, patten);
        int len = Application.dataPath.Length - "Assets".Length;
        foreach (var item in files)
        {
            if (!withExt.Equals(string.Empty))
                return AssetDatabase.LoadAssetAtPath<T>(item.Substring(len));

            string ext = Path.GetExtension(item).ToLower();
            if (ext.Equals(".meta")) continue;

            if (typeof(T) == typeof(Texture2D) ||
                typeof(T) == typeof(Sprite))
            {
                if (ext.Equals(".png") || ext.Equals(".jpeg") || ext.Equals(".bmp") ||
                    ext.Equals(".jpg") || ext.Equals(".tga") || ext.Equals(".gif"))
                {
                    return AssetDatabase.LoadAssetAtPath<T>(item.Substring(len));
                }
            }
            else if (typeof(T) == typeof(GameObject))
            {
                if (ext.Equals(".prefab"))
                {
                    return AssetDatabase.LoadAssetAtPath<T>(item.Substring(len));
                }
            }
            else if (typeof(T) == typeof(TextAsset))
            {
                if (ext.Equals(".bytes") || ext.Equals(".xml") || ext.Equals(".json"))
                {
                    return AssetDatabase.LoadAssetAtPath<T>(item.Substring(len));
                }
            }
            else if (typeof(T) == typeof(AudioClip))
            {
                if (ext.Equals(".mp3") || ext.Equals(".wav") || ext.Equals(".ogg") ||
                    ext.Equals(".mod") || ext.Equals("aif") || ext.Equals(".xm"))
                {
                    return AssetDatabase.LoadAssetAtPath<T>(item.Substring(len));
                }
            }
            else if (typeof(T) == typeof(Material))
            {
                return AssetDatabase.LoadAssetAtPath<T>(item.Substring(len));
            }
            else if (typeof(T) == typeof(SpriteAtlas))
            {
                return AssetDatabase.LoadAssetAtPath<T>(item.Substring(len));
            }
        }

        return null;
#else
            return Resources.Load<T>(uri);
#endif
    }

    public static T[] LoadAllAsset<T>(string uri, string withExt = "", string resRoot = "BundleResources")
        where T : UnityEngine.Object
    {
#if UNITY_EDITOR
        string rootPath = Path.Combine(Application.dataPath, resRoot);
        string assetFullName = Path.Combine(rootPath, uri);
        string assetName = Path.GetFileName(uri);
        string assetDirectoryName = Path.GetDirectoryName(assetFullName);
        string patten = assetName + withExt;
        if (withExt.Equals(string.Empty))
        {
            patten = assetName + ".*";
        }

        if (!Directory.Exists(assetDirectoryName))
        {
            Debug.LogWarning(assetDirectoryName + " is not found.");
            return null;
        }

        UnityEngine.Object[] assets = null;
        T[] result = null;
        string[] files = Directory.GetFiles(assetDirectoryName, patten);
        int len = Application.dataPath.Length - "Assets".Length;
        foreach (var item in files)
        {
            if (!withExt.Equals(string.Empty))
            {
                assets = AssetDatabase.LoadAllAssetsAtPath(item.Substring(len));
                break;
            }

            string ext = Path.GetExtension(item).ToLower();
            if (ext.Equals(".meta")) continue;

            if (typeof(T) == typeof(Texture2D) ||
                typeof(T) == typeof(Sprite))
            {
                if (ext.Equals(".png") || ext.Equals(".jpeg") || ext.Equals(".bmp") ||
                    ext.Equals(".jpg") || ext.Equals(".tga") || ext.Equals(".gif"))
                {
                    assets = AssetDatabase.LoadAllAssetsAtPath(item.Substring(len));
                    break;
                }
            }
            else if (typeof(T) == typeof(GameObject))
            {
                if (ext.Equals(".prefab"))
                {
                    assets = AssetDatabase.LoadAllAssetsAtPath(item.Substring(len));
                    break;
                }
            }
            else if (typeof(T) == typeof(TextAsset))
            {
                if (ext.Equals(".bytes"))
                {
                    assets = AssetDatabase.LoadAllAssetsAtPath(item.Substring(len));
                    break;
                }
            }
            else if (typeof(T) == typeof(AudioClip))
            {
                if (ext.Equals(".mp3") || ext.Equals(".wav") || ext.Equals(".ogg") ||
                    ext.Equals(".mod") || ext.Equals("aif") || ext.Equals(".xm"))
                {
                    assets = AssetDatabase.LoadAllAssetsAtPath(item.Substring(len));
                    break;
                }
            }
        }

        if (null != assets)
            result = assets.OfType<T>().ToArray();

        return result;
#else
                return Resources.LoadAll<T>(uri);
#endif
    }

    public static T LoadSubAsset<T>(string uri, string subAssets, string withExt = "",
        string resRoot = "BundleResources") where T : UnityEngine.Object
    {
        T[] assets = null;
        assets = LoadAllAsset<T>(uri, withExt, resRoot);
        if (null == assets) return null;
        foreach (var asset in assets)
        {
            if (asset.name.Equals(subAssets))
                return asset;
        }

        return null;
    }


    public static void PrintLogToFile(string Content, string fileName)
    {
        string path = Application.dataPath;
        StreamWriter sw = new StreamWriter(path + "\\" + fileName, true);
        //开始写入
        sw.WriteLine(Content);
        //清空缓冲区
        sw.Flush();
        //关闭流
        sw.Close();
    }


    public static string GetFileLength(string filePath)
    {
        if (!FileExists(filePath))
        {
            return "0";
        }

        long fileLength = 0;
        using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        {
            fileLength = fs.Length;
            fs.Close();
            fs.Dispose();
        }

        return fileLength.ToString();
    }

    public static string GetFileContent(string filePath)
    {
        if (!FileExists(filePath))
        {
            return "";
        }

        string fileContent = "";
        using (var fsRead = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        {
            int fsLen = (int) fsRead.Length;
            byte[] heByte = new byte[fsLen];
            int r = fsRead.Read(heByte, 0, heByte.Length);
            fileContent = System.Text.Encoding.UTF8.GetString(heByte);
            fsRead.Close();
            fsRead.Dispose();
        }

        return fileContent;
    }

    /// <summary>
    /// 计算字符串的MD5值
    /// </summary>
    public static string md5(string source)
    {
        MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
        byte[] data = System.Text.Encoding.UTF8.GetBytes(source);
        byte[] md5Data = md5.ComputeHash(data, 0, data.Length);
        md5.Clear();

        string destString = "";
        for (int i = 0; i < md5Data.Length; i++)
        {
            destString += System.Convert.ToString(md5Data[i], 16).PadLeft(2, '0');
        }

        destString = destString.PadLeft(32, '0');
        return destString;
    }
}