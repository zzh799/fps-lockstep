using FairyGUI;
using UnityEngine;

namespace Manager
{
    public class AssetBundleUISource : IUISource
    {
        public AssetBundleUISource(string packageName)
        {
            this.fileName = packageName;
        }

        public string fileName { get; set; }

        public bool loaded
        {
            get => _loaded;
            private set => _loaded = value;
        }

        public bool _loaded = false;

        public void Load(UILoadCallback callback)
        {
            //TODO:从AB包加载
            try
            {
                UIPackage.AddPackage("Assets/BundleResources/UI/" + fileName);
                loaded = true;
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Add Package:{fileName} Error：{ex.Message}");
            }
            callback.Invoke();
        }

        public void Cancel()
        {
            throw new System.NotImplementedException();
        }
    }
}