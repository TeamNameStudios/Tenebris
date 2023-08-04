using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp
{
    public PowerUpEnum ID;
    public int Level;
    public PowerUp(PowerUpEnum _ID, int _Level)
    {
        ID = _ID;
        Level = _Level;
    }
    public PowerUp(SerializablePowerUp serializablePowerUp)
    {
        ID = serializablePowerUp.ID;
        Level = serializablePowerUp.Level;
    }
}

