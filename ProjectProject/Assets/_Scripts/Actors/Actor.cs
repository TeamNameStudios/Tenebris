using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    public enum ActorState
    {
        Idle,
        Moving,
        Jumping,
        Dead,
    }

    public ActorState State { get; private set; }
 
}
