using System.Collections.Generic;
using UnityEngine;

public class LevelAssembler : Singleton<LevelAssembler>
{

    private List<LevelID> possibleLevels = new List<LevelID>();
    [SerializeField]
    private float probabilityToAdd;

    private List<float> GetProbabilityList(List<LevelID> _possibleLevels)
    {
        List<float> probabilities = new List<float>();
        ScriptableLevelChunk levelChunk;

        for (int i = 0; i < _possibleLevels.Count; i++)
        {
            levelChunk = ResourceSystem.Instance.GetLevelChunk(_possibleLevels[i]);
            float probability = levelChunk.Probability;
            probabilities.Add(probability);
        }

        return probabilities;
    }
    
    private int GetWeightedRandomIndex(List<float> probabilities)
    {
        float totalProbability = 0;

        for (int i = 0; i < probabilities.Count; i++)
        {
            totalProbability += probabilities[i];
        }

        float randomValue = Random.Range(0, totalProbability);

        for (int i = 0; i < probabilities.Count; i++)
        {
            randomValue -= probabilities[i];

            if (randomValue <= 0)
            {
                return i;
            }
        }

        return probabilities.Count - 1;
    }

    public void CreateLevelChunk(LevelID _nextLevelID, Transform parentObject)
    {
        ScriptableLevelChunk scriptableLevelChunk = ResourceSystem.Instance.GetLevelChunk(_nextLevelID);
        GameObject GO =  Instantiate(scriptableLevelChunk.LevelPrefab, parentObject);
        GO.transform.SetParent(parentObject);
        AddProbability(_nextLevelID, probabilityToAdd);
            
        // We change the references with the chunk just spawned
        possibleLevels.Clear();
        possibleLevels = new List<LevelID>(scriptableLevelChunk.InGamePossibleNeighbour);
       
    }

    private void AddProbability(LevelID _nextLevelID, float probabilityToAdd)
    {
        foreach (LevelID levelID in possibleLevels)
        {
            if (levelID != _nextLevelID)
            {
                // This increases the probability for all the levels that are not picked
                ResourceSystem.Instance.GetLevelChunk(levelID).Probability += probabilityToAdd;
            }
            else if (levelID == _nextLevelID)
            {
                // This resets the probability when we spawn a level
                ScriptableLevelChunk thisLevel = ResourceSystem.Instance.GetLevelChunk(levelID);
                thisLevel.Probability = thisLevel.InGameProbability;
            }
        }
    }


    public LevelID CreateChunk(Transform parentObject)
    {
        List<float> probabilities = GetProbabilityList(possibleLevels);
        int index = GetWeightedRandomIndex(probabilities);
        LevelID _levelID = possibleLevels[index];
        
        CreateLevelChunk(_levelID, parentObject);


        return _levelID;
    }


}
