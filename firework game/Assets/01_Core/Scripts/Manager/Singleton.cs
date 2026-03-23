using UnityEngine;

/// <summary>
/// 泛型单例基类（Unity专用，所有管理器继承此类）
/// </summary>
/// <typeparam name="T">要做成单例的管理器类型</typeparam>
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    // 静态实例：全局唯一的管理器实例
    private static T _instance;

    // 线程锁：防止多线程创建多个实例（新手可忽略，保留即可）
    private static readonly object _lock = new object();

    // 公开的实例访问入口（外部通过 Instance 调用）
    public static T Instance
    {
        get
        {
            lock (_lock)
            {
                // 如果实例为空，自动查找场景中的管理器
                if (_instance == null)
                {
                    _instance = FindObjectOfType<T>();

                    // 场景中没有则自动创建（避免手动拖组件）
                    if (_instance == null)
                    {
                        GameObject singletonObj = new GameObject(typeof(T).Name);
                        _instance = singletonObj.AddComponent<T>();
                        // 切换场景不销毁，保证全局唯一
                        DontDestroyOnLoad(singletonObj);
                    }
                }
                return _instance;
            }
        }
    }

    // 初始化：保证实例唯一
    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // 销毁重复的管理器（防止场景里拖多个）
            Destroy(gameObject);
        }
    }
}