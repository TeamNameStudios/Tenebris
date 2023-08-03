
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;

public class DataManager : Singleton<DataManager>
{
    private string _powerUpListFileName = "/power-up.dat";

    private void OnEnable()
    {
        EventManager<int>.Instance.StartListening("SavePage", SavePages);
        EventManager<int>.Instance.StartListening("SaveTotalPage", SaveTotalPages);
        EventManager<int>.Instance.StartListening("SaveBestDistance", SaveBestDistance);
        EventManager<List<PowerUp>>.Instance.StartListening("SavePowerUp", SavePowerUp);
        EventManager<bool>.Instance.StartListening("LoadData", LoadData);
    }
    private void OnDisable()
    {
        EventManager<int>.Instance.StopListening("SavePage", SavePages);
        EventManager<int>.Instance.StopListening("SaveTotalPage", SaveTotalPages);
        EventManager<int>.Instance.StopListening("SaveBestDistance", SaveBestDistance);
        EventManager<List<PowerUp>>.Instance.StopListening("SavePowerUp", SavePowerUp);
        EventManager<bool>.Instance.StartListening("LoadData", LoadData);
    }

    public void LoadData(bool loading)
    {
        LoadPages();
        LoadPowerUp();
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

    public void SaveBestDistance(int count)
    {
        if (count < 0)
            return;
        PlayerPrefs.SetInt("BestDistance", count);
        PlayerPrefs.Save();
    }

    public void LoadBestDistance()
    {
        int bestDistance = PlayerPrefs.GetInt("BestDistance", 0);
        EventManager<int>.Instance.TriggerEvent("onBestDistanceLoaded", bestDistance);
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
    public int PageCost;
    public SerializablePowerUp(PowerUp powerUp)
    {
        ID = powerUp.ID;
        Level = powerUp.Level;
        PageCost = powerUp.PageCost;
    }
}

public enum PowerUpEnum
{

}

public class PowerUp
{
    public PowerUpEnum ID;
    public int Level;
    public int PageCost;
    public PowerUp(PowerUpEnum _ID, int _Level, int _PageCost) { 
        ID = _ID;
        Level = _Level;
        PageCost = _PageCost;
    }
    public PowerUp(SerializablePowerUp serializablePowerUp)
    {
        ID = serializablePowerUp.ID;
        Level = serializablePowerUp.Level;
        PageCost = serializablePowerUp.PageCost;
    }
}