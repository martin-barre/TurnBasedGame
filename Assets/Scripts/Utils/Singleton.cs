using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Component
{
    private static T instance;

    public static T Instance {
        get {
            if(instance == null) {
                instance = FindObjectOfType<T>();
                if(instance == null) {
                    GameObject newGameObject = new GameObject();
                    newGameObject.name = typeof(T).ToString();
                    instance = newGameObject.AddComponent<T>();
                }
            }
            return instance;
        }
    }
    
    protected virtual void Awake() {
        instance = this as T;
    }
}
