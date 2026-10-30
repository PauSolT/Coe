using UnityEngine;

public class BaseSingleton<T> : MonoBehaviour
    where T : Component
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                var objs = FindObjectsByType(typeof(T), FindObjectsSortMode.None) as T[];
                if (objs.Length > 0)
                    _instance = objs[0];
                if (objs.Length > 1)
                {
                    Log.Info("There is more than one " + typeof(T).Name + " in the scene.", LogCategory.System, level: LogLevel.Warn);
                }
                if (_instance == null)
                {
                    GameObject obj = new();
                    obj.hideFlags = HideFlags.HideAndDontSave;
                    _instance = obj.AddComponent<T>();
                }
            }
            return _instance;
        }
    }
}


public class BaseSingletonSingletonPersistent<T> : MonoBehaviour
    where T : Component
{
    public static T Instance { get; private set; }

    public virtual void Awake()
    {
        if (Instance == null)
        {
            Instance = this as T;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}