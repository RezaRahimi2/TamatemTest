using UnityEngine;
public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    private static object _lock = new object();

    public static T Instance
    {
        get
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    var instances = FindObjectsOfType<T>();
                    if (instances.Length > 1)
                    {
                        Debug.LogError("[Singleton] Something went really wrong " +
                                       ", there are too many Singletons; deleting them: ");
                        for (int i = 1; i < instances.Length; i++)
                        {
                            Debug.LogError("Deleting " + instances[i].gameObject.name);
                            Destroy(instances[i].gameObject);
                        }
                        _instance = FindObjectOfType<T>();
                        return _instance;
                    }

                    if (instances.Length > 0)
                        _instance = instances[0];
                }

                return _instance;
            }
        }
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void OnDestroy()
    {
        _instance = null;
    }
}

public abstract class SingletonNonMono<T>  where T : class, new()
{
    private static T _instance;
    private static object _lock = new object();

    public static T Instance
    {
        get
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                        _instance =  _instance = new T();
                        return _instance;
                }
                return _instance;
            }
        }
    }
}