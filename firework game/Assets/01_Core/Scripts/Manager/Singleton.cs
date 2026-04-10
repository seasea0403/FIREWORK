using UnityEngine;

/// <summary>
/// 通用 MonoBehaviour 单例基类，确保在场景中只存在一个实例。
/// </summary>
/// <typeparam name="T">派生类类型</typeparam>
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    // 当前单例实例
    private static T _instance;

    // 锁定对象，保证多线程访问安全
    private static readonly object _lock = new object();

    // 全局访问点
    public static T Instance
    {
        get
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<T>();

                    if (_instance == null)
                    {
                        GameObject singletonObj = new GameObject(typeof(T).Name);
                        _instance = singletonObj.AddComponent<T>();
                        DontDestroyOnLoad(singletonObj);
                    }
                }
                return _instance;
            }
        }
    }

    // Awake 时确保唯一实例
    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}