using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Helper
{
    #region Design Patterns
    // MONOSINGLETON
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {
        private static T instance;
    
        public static T Instance { get { return instance; } }
    
        protected virtual void Awake()
        {
            if (instance != null)
            {
                Debug.LogError($"Instance of this singleton {(T)this} already exist, deleting.");
                Destroy(gameObject);
            }
            else
            {
                DontDestroyOnLoad(gameObject);
                instance = (T)this;
            }
        }
    }

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

    IEnumerator WaitCoroutine(float _time)
    {
        yield return new WaitForSeconds(_time);
    }

    #endregion
}

