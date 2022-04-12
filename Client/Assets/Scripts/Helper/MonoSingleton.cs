using UnityEngine;

public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    public bool global = true;
    private static T mInstance = null;

    public static T Instance
    {
        get
        {
            if (mInstance == null)
            {
                mInstance = GameObject.FindObjectOfType(typeof(T)) as T;
                if (mInstance == null)
                {
                    GameObject go = new GameObject(typeof(T).Name);
                    mInstance = go.AddComponent<T>();
                    
                    GameObject parent = GameObject.Find("GameManager");
                    if (parent != null)
                    {
                        go.transform.parent = parent.transform;
                    }
                }
            }

            return mInstance;
        }
    }

    /*
     * 没有任何实现的函数，用于保证MonoSingleton在使用前已创建
     */
    public void Startup()
    {

    }

    private void Awake()
    {
        if (global)
        {
            if (mInstance != null && mInstance != this.gameObject.GetComponent<T>())
            {
                Destroy(this.gameObject);
                return;
            }
            DontDestroyOnLoad(this.gameObject);
            mInstance = this.gameObject.GetComponent<T>();
        }
        this.Init();
    }

    protected virtual void Init()
    {
  
    }

    public void DestroySelf()
    {
        Dispose();
        mInstance = null;
        Destroy(gameObject);
    }

    public virtual void Dispose()
    { 
    }

}