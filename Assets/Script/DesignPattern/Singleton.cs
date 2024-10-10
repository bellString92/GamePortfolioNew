using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _inst = null;
    public static T Instance
    {
        get
        {
            _inst = FindObjectOfType<T>();
            if (_inst == null)
            {
                GameObject obj = new GameObject();
                obj.name = typeof(T).ToString();
                _inst = obj.AddComponent<T>();
            }
            return _inst;
        }
    }

    protected void Initialize()
    {
        if (_inst != null)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
