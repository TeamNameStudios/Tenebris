using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Helper
{
    #region Design Patterns


    #region POOLS
    // OBJECT POOL W\ GAMEOBJECTS
    public class ObjectPool
    {
        public GameObject basePrefab;

        private Queue<GameObject> objectPool;

        public string PrefabName => basePrefab.name;

        public ObjectPool(GameObject prefab, int poolSize)
        {
            basePrefab = prefab;
            objectPool = new Queue<GameObject>();

            for (int i = 0; i < poolSize; i++)
            {
                GameObject obj = MonoBehaviour.Instantiate(prefab);
                obj.name = PrefabName; // to remove the (Clone) label
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }
        }

        public GameObject GetObject()
        {
            if (objectPool.Count > 0)
            {
                GameObject obj = objectPool.Dequeue();
                return obj;
            }
            else
            {
                GameObject obj = MonoBehaviour.Instantiate(basePrefab);
                obj.name = PrefabName; // to remove the (Clone) label
                return obj;
            }
        }

        public void ReturnObject(GameObject obj)
        {
            obj.SetActive(false);
            objectPool.Enqueue(obj);
        }
    }

    // GENERIC POOL
    public class GenericObjectPool<T> where T : Component
    {
        public T basePrefab;

        private Queue<T> objectPool;

        public GenericObjectPool(T prefab, int poolSize)
        {
            basePrefab = prefab;
            objectPool = new Queue<T>();

            for (int i = 0; i < poolSize; i++)
            {
                var obj = MonoBehaviour.Instantiate(prefab);
                obj.gameObject.SetActive(false);
                objectPool.Enqueue(obj);
            }
        }

        public T GetObject()
        {
            if (objectPool.Count > 0)
            {
                T obj = objectPool.Dequeue();
                return obj;
            }
            else
            {
                T obj = MonoBehaviour.Instantiate(basePrefab);
                return obj;
            }
        }

        public void ReturnObject(T obj)
        {
            obj.gameObject.SetActive(false);
            objectPool.Enqueue(obj);
        }
    }

    #endregion
    // FACTORY W\ POOLS OF GAMEOBJECTS (we ask for the GO name, maybe try to implement the asking with an enum (?))
    public class Factory : MonoBehaviour
    {
        public int poolSize;
        public GameObject[] prefabs;

        protected Dictionary<string, Helper.ObjectPool> pools;

        private void Start()
        {
            InitPools();
        }

        private void InitPools()
        {
            pools = new Dictionary<string, Helper.ObjectPool>();

            foreach (GameObject go in prefabs)
                pools.Add(go.name, new Helper.ObjectPool(go, poolSize));
        }

        public GameObject CreateObject(string name, Vector3 position)
        {
            return CreateObject(name, position, Quaternion.identity);
        }

        public GameObject CreateObject(string objectName, Vector3 position, Quaternion rotation, bool setActive = true)
        {
            if (!pools.ContainsKey(objectName))
            {
                Debug.LogWarning(string.Format("Could not find a pooled object with name '{0}'", name));
                return null;
            }

            GameObject instance = pools[objectName].GetObject();
            instance.transform.position = position;
            instance.transform.rotation = rotation;
            instance.SetActive(setActive);


            return instance;
        }

        public bool ReturnObject(GameObject instance)
        {
            if (!pools.ContainsKey(instance.name))
            {
                Debug.LogWarning(string.Format("Could not find a pooled object with name '{0}'", name));
                return false;
            }

            pools[instance.name].ReturnObject(instance);

            return true;
        }
    }

    // FACTORY AND OBJ POOLING WITH GENERICS

    public class GenericFactory<T> : MonoBehaviour where T : Component
    {
        public int poolSize;
        public T[] prefabs;

        protected Dictionary<string, Helper.GenericObjectPool<T>> pools;

        private void Start()
        {
            InitPools();
        }

        private void InitPools()
        {
            pools = new Dictionary<string, Helper.GenericObjectPool<T>>();

            foreach (T go in prefabs)
                pools.Add(go.gameObject.name, new Helper.GenericObjectPool<T>(go, poolSize));
        }

        public T CreateObject(string name, Vector3 position)
        {
            return CreateObject(name, position, Quaternion.identity);
        }

        public T CreateObject(string objectName, Vector3 position, Quaternion rotation, bool setActive = true)
        {
            if (!pools.ContainsKey(objectName))
            {
                return null;
            }

            T instance = pools[objectName].GetObject();
            instance.transform.position = position;
            instance.transform.rotation = rotation;
            instance.gameObject.SetActive(setActive);


            return instance;
        }

        public bool ReturnObject(T instance)
        {
            if (!pools.ContainsKey(instance.gameObject.name))
            {
                return false;
            }

            pools[instance.gameObject.name].ReturnObject(instance);

            return true;
        }
    }

    
    #endregion

    #region Utility
    public static T RandomArrayValue<T>(List<T> array)
    {
        if (array.Count <= 0)
            return default;
    
        return array[UnityEngine.Random.Range(0, array.Count)];
    }
    
    public static T RandomArrayValue<T>(T[] array)
    {
        if (array.Length <= 0)
            return default;
    
        return array[UnityEngine.Random.Range(0, array.Length)];
    }
    
    public static void Shuffle<T>(IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = UnityEngine.Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }

    public static IEnumerator WaitCoroutine(float _time)
    {
        yield return new WaitForSeconds(_time);
    }

    /// <summary>
    /// Destroy all child objects of this transform (Unintentionally evil sounding).
    /// Use it like so:
    /// <code>
    /// transform.DestroyChildren();
    /// </code>
    /// </summary>
    public static void DestroyChildren(this Transform t)
    {
        foreach (Transform child in t) Object.Destroy(child.gameObject);
    }
    
    #endregion

}

/// <summary>
/// A static instance is similar to a singleton, but instead of destroying any new
/// instances, it overrides the current instance. This is handy for resetting the state
/// and saves you doing it manually
/// </summary>
public abstract class StaticInstance<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get; protected set; }
    protected virtual void Awake() 
    {
        Instance = this as T;
    }

    protected virtual void OnApplicationQuit()
    {
        Instance = null;
        Destroy(gameObject);
    }
}

/// <summary>
/// This transforms the static instance into a basic singleton. This will destroy any new
/// versions created, leaving the original instance intact
/// </summary>
public abstract class Singleton<T> : StaticInstance<T> where T : MonoBehaviour
{
    protected override void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        base.Awake();
    }
}

/// <summary>
/// Finally we have a persistent version of the singleton. This will survive through scene
/// loads. Perfect for system classes which require stateful, persistent data. Or audio sources
/// where music plays through loading screens, etc
/// </summary>
public abstract class PersistentSingleton<T> : Singleton<T> where T : MonoBehaviour
{
    protected override void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        else
        {
            Instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
    }
}