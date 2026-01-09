using System.IO;
using UnityEngine;

public class Settings : MonoBehaviour
{
    #region SINGLETON INSTANCE
    private static Settings _instance;

    public static Settings Instance
    {
        get
        {
            if (_instance == null && !ApplicationQuitting)
            {
                _instance = FindFirstObjectByType<Settings>();
                if (_instance == null)
                {
                    GameObject newInstance = new GameObject("Singleton_SoccerSettings");
                    _instance = newInstance.AddComponent<Settings>();
                }
            }

            return _instance;
        }
    }

    public static bool Exists
    {
        get { return _instance != null; }
    }

    public static bool ApplicationQuitting = false;
    protected virtual void OnApplicationQuit()
    {
        ApplicationQuitting = true;
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject); if (_instance == null) _instance = this;
        else if (_instance != this) Destroy(gameObject);
    }
    private void OnDestroy()
    {
        if (_instance == this) _instance = null;
    }
    #endregion SINGLETON INSTANCE

    public readonly string m_DataPath = Path.Combine(Directory.GetParent(Application.dataPath).FullName, "Data");
}
