
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class DataManager : Singleton<DataManager>
{
    private string _powerUpListFileName = "/power-up.dat";

    private void Start()
    {
        if (!File.Exists(@"" + Application.persistentDataPath + _powerUpListFileName))
        {
            InitPowerUpFile();
        }
            
    }
    private void OnEnable()
    {
        EventManager<int>.Instance.StartListening("SavePage", SavePages);
        EventManager<int>.Instance.StartListening("SaveTotalPage", SaveTotalPages);
        EventManager<float>.Instance.StartListening("SaveBestDistance", SaveBestDistance);
        EventManager<List<PowerUp>>.Instance.StartListening("SavePowerUp", SavePowerUp);
        EventManager<bool>.Instance.StartListening("LoadData", LoadData);
    }
    private void OnDisable()
    {
        EventManager<int>.Instance.StopListening("SavePage", SavePages);
        EventManager<int>.Instance.StopListening("SaveTotalPage", SaveTotalPages);
        EventManager<float>.Instance.StopListening("SaveBestDistance", SaveBestDistance);
        EventManager<List<PowerUp>>.Instance.StopListening("SavePowerUp", SavePowerUp);
        EventManager<bool>.Instance.StopListening("LoadData", LoadData);
    }

    public void LoadData(bool loading)
    {
        LoadPages();
        LoadTotalPages();
        LoadPowerUp();
        LoadBestDistance();
    }

    public void SavePages(int count)
    {
        if (count < 0)
            return;
        PlayerPrefs.SetInt("Pages", count);
        PlayerPrefs.Save();
    }

    public void LoadPages()
    {
        int numberOfPages =  PlayerPrefs.GetInt("Pages", 0);
        EventManager<int>.Instance.TriggerEvent("onPageLoaded", numberOfPages);
    }


    public void SaveTotalPages(int count)
    {
        if (count < 0)
            return;
        PlayerPrefs.SetInt("TotalPages", count);
        PlayerPrefs.Save();
    }

    public void LoadTotalPages()
    {
        int numberOfPages = PlayerPrefs.GetInt("TotalPages", 0);
        EventManager<int>.Instance.TriggerEvent("onTotalPageLoaded", numberOfPages);
    }

    public void SaveBestDistance(float count)
    {
        if (count < 0)
            return;
        PlayerPrefs.SetFloat("BestDistance", count);
        PlayerPrefs.Save();
    }

    public void LoadBestDistance()
    {
        float bestDistance = PlayerPrefs.GetFloat("BestDistance", 0);
        EventManager<float>.Instance.TriggerEvent("onBestDistanceLoaded", bestDistance);
    }

    #region POWERUPS
    public void SavePowerUp(List<PowerUp> dataList)
    {
        // JSON Creation
        List<SerializablePowerUp> listSerializedPowerUp = new List<SerializablePowerUp>();
        for(int i = 0;i< dataList.Count;i++)
        {
            SerializablePowerUp serializedPowerUp = new SerializablePowerUp(dataList[i]);
            listSerializedPowerUp.Add(serializedPowerUp);
        }

        SerializableList<SerializablePowerUp> listObj = new SerializableList<SerializablePowerUp>(listSerializedPowerUp);
        string json = JsonUtility.ToJson(listObj);

        // Save json string to file
        if (File.Exists(@"" + Application.persistentDataPath + _powerUpListFileName))
        {
            File.Delete(@"" + Application.persistentDataPath + _powerUpListFileName);
        }
        StreamWriter writer = new StreamWriter(Application.persistentDataPath + _powerUpListFileName);
        writer.WriteLine(json);
        writer.Close();
    }

    private void InitPowerUpFile()
    {
        List<PowerUpEnum> enumList = ResourceSystem.Instance.GetInitsPowerUp();
        List<SerializablePowerUp> listSerializedPowerUp = new List<SerializablePowerUp>();
        for (int i = 0; i < enumList.Count; i++)
        {
            SerializablePowerUp serializedPowerUp = new SerializablePowerUp(new PowerUp(enumList[i],1));
            listSerializedPowerUp.Add(serializedPowerUp);
        }

        SerializableList<SerializablePowerUp> listObj = new SerializableList<SerializablePowerUp>(listSerializedPowerUp);
        string json = JsonUtility.ToJson(listObj);
        StreamWriter writer = new StreamWriter(Application.persistentDataPath + _powerUpListFileName);
        writer.WriteLine(json);
        writer.Close();
    }

    public void LoadPowerUp()
    {
        // Read from file the json string
        if (!File.Exists(@"" + Application.persistentDataPath + _powerUpListFileName))
            return;
        StreamReader reader = new StreamReader(Application.persistentDataPath + _powerUpListFileName);
        string json = reader.ReadToEnd();
        reader.Close();
        List<SerializablePowerUp> serializedList = JsonUtility.FromJson<SerializableList<SerializablePowerUp>>(json).serializableList;
        List <PowerUp> powerUpList = new List<PowerUp>();
        for (int i = 0; i < serializedList.Count; i++) {
            PowerUp powerUp = new PowerUp(serializedList[i]);
            powerUpList.Add(powerUp);
        }
        EventManager<List<PowerUp>>.Instance.TriggerEvent("onPowerUpLoaded", powerUpList);
    }
    #endregion
}

[System.Serializable]
public class SerializableList<T>
{
    public List<T> serializableList;
    public SerializableList(List<T> list) => this.serializableList = list;
}



[System.Serializable]
public class SerializablePowerUp
{
    public PowerUpEnum ID;
    public int Level;
    public SerializablePowerUp(PowerUp powerUp)
    {
        ID = powerUp.ID;
        Level = powerUp.Level;
    }
}