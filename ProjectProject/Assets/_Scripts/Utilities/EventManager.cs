using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EventManager<T>
{
    private static EventManager<T> instance;
    
    public static EventManager<T> Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new EventManager<T>();

                instance.Init();
            }
            return instance;
        }
    }

    private Dictionary<string, Action> eventDictionary;
    
    private Dictionary<string, Action<T>> paramEventDictionary;

    private EventManager() { }

    private void Init()
    {
        if (eventDictionary == null)
        {
            eventDictionary = new Dictionary<string, Action>();
            paramEventDictionary = new Dictionary<string, Action<T>>();
        }
    }

    public void StartListening(string eventName, Action listener)
    {
         
        Action thisListener = null;

        if (eventDictionary.ContainsKey(eventName))
        {
            eventDictionary[eventName] += listener;
        }
        else
        {
           
            thisListener += listener;
            Instance.eventDictionary.Add(eventName, thisListener);
        }
    }

    public void StopListening(string eventName, Action listener)
    {
        if (instance == null)
        {
            return;
        }

        //Action thisListener = null;

        if (eventDictionary.ContainsKey(eventName))
        {
            eventDictionary[eventName] -= listener;
        }
    }

    public void TriggerEvent(string eventName)
    {
        if (Instance.eventDictionary.ContainsKey(eventName))
        {
            eventDictionary[eventName]?.Invoke();
        }
    }

    
    
    public void StartListening(string eventName, Action<T> listener)
    {
        Action<T> thisListener = null;
        
        if (paramEventDictionary.ContainsKey(eventName))
        {
            paramEventDictionary[eventName] += listener;
        
        }
        else
        {
            thisListener += listener;
           paramEventDictionary.Add(eventName, thisListener);
        }
    }

    public void StopListening(string eventName, Action<T> listener)
    {
        if (instance == null)
        {
            return;
        }

        //Action<T> thisListener = null;

        if (paramEventDictionary.ContainsKey(eventName))
        {
            paramEventDictionary[eventName] -= listener;
        }
    }

    public void TriggerEvent(string eventName, T data)
    {
        if (paramEventDictionary.ContainsKey(eventName))
        {
            paramEventDictionary[eventName]?.Invoke(data);
        }
    }
}
