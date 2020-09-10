using UnityEngine;

public abstract class SingletonBase<T> : MonoBehaviour where T : class
{
    /// <summary>
    /// SingletoneBase instance back field
    /// </summary>
    private static T _instance = null;
    /// <summary>
    /// SingletoneBase instance
    /// </summary>
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType(typeof(T)) as T;
                if (_instance == null)
                    Debug.LogWarning("SingletoneBase<T>: Could not found GameObject of type " + typeof(T).Name);
            }
            return _instance;
        }
    }
}
