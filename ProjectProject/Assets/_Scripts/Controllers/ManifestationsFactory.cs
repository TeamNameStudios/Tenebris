using System.Collections.Generic;
using UnityEngine;

public class ManifestationsFactory : Singleton<ManifestationsFactory>
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

        for (int i = 0; i < prefabs.Length; i++) 
            pools.Add(prefabs[i].name, new Helper.ObjectPool(prefabs[i], poolSize)); 
           
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
