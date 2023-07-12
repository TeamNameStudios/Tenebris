using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManifestationsFactory : Helper.MonoSingleton<ManifestationsFactory>
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
