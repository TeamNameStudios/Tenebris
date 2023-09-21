
using System.Collections.Generic;
using UnityEngine;
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
        EventManager<int>.Instance.StartListening("SaveTotalPage", SaveTotalPages);
        EventManager<float>.Instance.StartListening("SaveBestDistance", SaveBestDistance);
        EventManager<string>.Instance.StartListening("SaveBestTime", SaveBestTime);
        EventManager<List<PowerUp>>.Instance.StartListening("SavePowerUp", SavePowerUp);
        EventManager<int>.Instance.StartListening("SaveTutorialFlag", SaveTutorialFlag);
        EventManager<float>.Instance.StartListening("SaveMasterVolume", SaveMasterVolume);
        EventManager<float>.Instance.StartListening("SaveMusicVolume", SaveMusicVolume);
        EventManager<float>.Instance.StartListening("SaveEffectVolume", SaveEffectVolume);
        EventManager<bool>.Instance.StartListening("LoadData", LoadData);
        EventManager<bool>.Instance.StartListening("LoadAudioData", LoadAudioData);
    }
    
    private void OnDisable()
    {
        EventManager<int>.Instance.StopListening("SaveTotalPage", SaveTotalPages);
        EventManager<float>.Instance.StopListening("SaveBestDistance", SaveBestDistance);
        EventManager<string>.Instance.StopListening("SaveBestTime", SaveBestTime);
        EventManager<List<PowerUp>>.Instance.StopListening("SavePowerUp", SavePowerUp);
        EventManager<int>.Instance.StopListening("SaveTutorialFlag", SaveTutorialFlag);
        EventManager<float>.Instance.StopListening("SaveMasterVolume", SaveMasterVolume);
        EventManager<float>.Instance.StopListening("SaveMusicVolume", SaveMusicVolume);
        EventManager<float>.Instance.StopListening("SaveEffectVolume", SaveEffectVolume);
        EventManager<bool>.Instance.StopListening("LoadData", LoadData);
        EventManager<bool>.Instance.StopListening("LoadAudioData", LoadAudioData);
    }

    public void LoadData(bool loading)
    {
        LoadTotalPages();
        LoadPowerUp();
        LoadBestDistance();
        LoadBestTime();
        LoadTutorialFlag();
    }

    public void SaveMasterVolume(float count)
    {
        if (count < 0)
            return;
        PlayerPrefs.SetFloat("MasterVolume", count);
        PlayerPrefs.Save();
    }

    public float LoadMasterVolume()
    {
        return PlayerPrefs.GetFloat("MasterVolume", 0);
    }

    public void SaveMusicVolume(float count)
    {
        if (count < 0)
            return;
        PlayerPrefs.SetFloat("MusicVolume", count);
        PlayerPrefs.Save();
    }

    public float LoadMusicVolume()
    {
       return PlayerPrefs.GetFloat("MusicVolume", 0);
    }

    public void SaveEffectVolume(float count)
    {
        if (count < 0)
            return;
        PlayerPrefs.SetFloat("EffectVolume", count);
        PlayerPrefs.Save();
    }

    public float LoadEffectVolume()
    {
        return PlayerPrefs.GetFloat("EffectVolume", 0);
    }

    public void LoadAudioData(bool loading)
    {

        List<float> audioVolume = new List<float>
        {
            LoadMasterVolume(),
            LoadMusicVolume(),
            LoadEffectVolume()
        };
        EventManager<List<float>>.Instance.TriggerEvent("onLoadAudioData", audioVolume);
    }

    public void ResetData()
    {
        PlayerPrefs.DeleteAll();
        SaveTutorialFlag(0);// The tutorial won't be reset
        if (File.Exists(@"" + Application.persistentDataPath + _powerUpListFileName))
        {
            File.Delete(@"" + Application.persistentDataPath + _powerUpListFileName);
        }
        LoadData(true);
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

    public void SaveBestTime(string value)
    {
        if (value == null)
            return;
        PlayerPrefs.SetString("BestTime", value);
        PlayerPrefs.Save();
    }

    public void LoadBestTime()
    {
        string bestTime = PlayerPrefs.GetString("BestTime", "00:00");
        EventManager<string>.Instance.TriggerEvent("onBestTimeLoaded", bestTime);
    }

    public void SaveTutorialFlag(int tutorialFlag)
    {
        PlayerPrefs.SetInt("TutorialFlag", tutorialFlag);
        PlayerPrefs.Save();
    }

    public void LoadTutorialFlag()
    {
        int tutorialFlag = PlayerPrefs.GetInt("TutorialFlag", 1);
        EventManager<int>.Instance.TriggerEvent("onTutorialFlagLoaded", tutorialFlag);
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
            SerializablePowerUp serializedPowerUp = new SerializablePowerUp(new PowerUp(enumList[i],0));
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
        {
            InitPowerUpFile();
            return;
        }
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