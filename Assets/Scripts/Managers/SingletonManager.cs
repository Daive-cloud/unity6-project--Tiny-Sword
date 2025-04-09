using UnityEditor.Localization.Plugins.Google;
using UnityEngine;

public class SingletonManager<T> : MonoBehaviour where T : MonoBehaviour
{
    private protected virtual void Awake()
    {
        T[] managers = FindObjectsByType<T>(FindObjectsSortMode.None);

        if(managers.Length > 1)
        {
            Destroy(gameObject);
            return;
        }
    }

    public static T Get()
    {
        var tag = typeof(T).Name;
        GameObject objectManager = GameObject.FindWithTag(tag);

        if(objectManager != null)
        {
            return objectManager.GetComponent<T>();
        }
        else{
            GameObject go = new(tag);
            go.tag = tag;
            return go.AddComponent<T>();
        }
    }
}
