using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EventManager
{
    private static EventManager instance;
    
    public static EventManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new EventManager();

                instance.Init();
            }
            return instance;
        }
    }

    private Dictionary<string, Action> eventDictionary;
    
    private Dictionary<string, Action<object>> paramEventDictionary;

    private EventManager() { }

    private void Init()
    {
        if (eventDictionary == null)
        {
            eventDictionary = new Dictionary<string, Action>();
            paramEventDictionary = new Dictionary<string, Action<object>>();
        }
    }

    public void StartListening(string eventName, Action listener)
    {
        Action thisListener = null;

        if (Instance.eventDictionary.TryGetValue(eventName, out thisListener))
        {
            thisListener += listener;
        }
        else
        {
            thisListener = listener;
            Instance.eventDictionary.Add(eventName, thisListener);
        }
    }

    public void StopListening(string eventName, Action listener)
    {
        if (instance == null)
        {
            return;
        }

        Action thisListener = null;

        if (Instance.eventDictionary.TryGetValue(eventName, out thisListener))
        {
            thisListener -= listener;
        }
    }

    public void TriggerEvent(string eventName)
    {
        if (Instance.eventDictionary.ContainsKey(eventName))
        {
            eventDictionary[eventName]?.Invoke();
        }
    }

    
    
    public void StartListening(string eventName, Action<object> listener)
    {
        Action<object> thisListener = null;

        if (Instance.paramEventDictionary.TryGetValue(eventName, out thisListener))
        {
            thisListener += listener;
        }
        else
        {
            thisListener = listener;
            Instance.paramEventDictionary.Add(eventName, thisListener);
        }
    }

    public void StopListening(string eventName, Action<object> listener)
    {
        if (instance == null)
        {
            return;
        }

        Action<object> thisListener = null;

        if (Instance.paramEventDictionary.TryGetValue(eventName, out thisListener))
        {
            thisListener -= listener;
        }
    }

    public void TriggerEvent(string eventName, object data)
    {
        if (Instance.paramEventDictionary.ContainsKey(eventName))
        {
            paramEventDictionary[eventName]?.Invoke(data);
        }
    }
}
