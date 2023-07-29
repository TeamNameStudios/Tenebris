using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelAssembler : Singleton<LevelAssembler>
{
    [SerializeField] private ScriptableLevelChunk demoLevel;

    [SerializeField] private Vector3 startingPos = new Vector3(-8, 0, 0);

    [SerializeField] private ScriptableLevelChunk lastLevelChunk;

    [SerializeField] private List<LevelID> possibleLevels = new List<LevelID>();

    [SerializeField] private float probabilityToAdd;

    private List<float> GetProbabilityList(List<LevelID> _possibleLevels)
    {
        List<float> probabilities = new List<float>();
        ScriptableLevelChunk levelChunk;
        
        foreach (LevelID level in _possibleLevels)
        {
            levelChunk = ResourceSystem.Instance.GetLevelChunk(level);
            float probability = levelChunk.Probability;
            probabilities.Add(probability);
        }

        return probabilities;
    }
    
    private int GetWeightedRandomIndex(List<float> probabilities)
    {
        float totalProbability = 0;

        foreach (float prob in probabilities)
        {
            totalProbability += prob;
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

    public void CreateLevelChunk(LevelID _nextLevelID, Transform parentObject, bool newLevel = true)
    {
        ScriptableLevelChunk a = ResourceSystem.Instance.GetLevelChunk(_nextLevelID);
        GameObject GO =  Instantiate(a.LevelPrefab, parentObject);
        GO.transform.SetParent(parentObject);
        Debug.Log("Creating " + a.ID.ToString() + " chunk at " + parentObject.transform.position);
        
        if (newLevel)
        {
            AddProbability(_nextLevelID, probabilityToAdd);
            
            // We change the references with the chunk just spawned
            lastLevelChunk = a;
            possibleLevels.Clear();
            possibleLevels = new List<LevelID>(lastLevelChunk.PossibleNeighbour);
        }
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
                thisLevel.Probability = thisLevel.BaseProbability;
            }
        }
    }

    public void Setup(Transform parentObject)
    {
        GameObject GO = Instantiate(demoLevel.LevelPrefab, parentObject);
        GO.transform.SetParent(parentObject);
        lastLevelChunk = demoLevel;
        possibleLevels = new List<LevelID>(lastLevelChunk.PossibleNeighbour);
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
