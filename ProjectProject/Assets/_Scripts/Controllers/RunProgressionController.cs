using System;
using UnityEngine;

public class RunProgressionController : MonoBehaviour
{
    private bool hasIncrementedMinutes = true;
    private bool hasIncrementedSeconds = true;

    [SerializeField] private float seconds;
    [SerializeField] private float minutes;

    [SerializeField] private float easyProbIncrement;
    [SerializeField] private float easyProbCap;
    [SerializeField] private float middleProbIncrement;
    [SerializeField] private float middleProbCap;
    [SerializeField] private float hardProbIncrement;
    [SerializeField] private float hardProbCap;
    [SerializeField] private float insaneProbIncrement;
    [SerializeField] private float insaneProbCap;

    private void Start()
    {
        ResourceSystem.Instance.RemoveNeighbourByDifficulty(LevelDifficulty.HARD);
        ResourceSystem.Instance.RemoveNeighbourByDifficulty(LevelDifficulty.INSANITY);
    }

    private void Update()
    {
        if (GameController.Instance.State == GameState.PLAYING)
        {
            ManageRun(GameController.Instance.Timer);
        }
    }

    private void ManageRun(TimeSpan timespan)
    {
        ManageRunBySeconds(timespan);
        ManageRunByMinutes(timespan);
    }

    private void ManageRunBySeconds(TimeSpan timeSpan)
    {
        if (timeSpan.Seconds % seconds == 0 && !hasIncrementedSeconds)
        {
            if (timeSpan.Seconds == 30 && timeSpan.Minutes == 0)
            {
                ResourceSystem.Instance.RestoreNeighbour();
            }
            EventManager<bool>.Instance.TriggerEvent("onLevelUp", true);
            hasIncrementedSeconds = true;
        }
        else if (timeSpan.Seconds % seconds != 0 && hasIncrementedSeconds)
        {
            hasIncrementedSeconds = false;
        }
    }

    private void ManageRunByMinutes(TimeSpan timeSpan)
    {
        if (timeSpan.Minutes % minutes == 0 && !hasIncrementedMinutes)
        {
            hasIncrementedMinutes = true;
            ResourceSystem.Instance.ChangeAllBaseProbability(-easyProbIncrement, easyProbCap, -middleProbIncrement, middleProbCap, hardProbIncrement, hardProbCap, insaneProbIncrement, insaneProbCap);
        }
        else if (timeSpan.Minutes % minutes != 0 && hasIncrementedMinutes)
        {
            hasIncrementedMinutes = false;
        }
    }
}
