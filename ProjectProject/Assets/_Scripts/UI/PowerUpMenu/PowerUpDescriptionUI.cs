using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;

public class PowerUpDescriptionUI : MonoBehaviour
{
    [SerializeField]
    string powerUpDescription;
    [SerializeField]
    private TextMeshProUGUI text;

    public void UpdateUI(ScriptablePowerUp powerUp)
    {
        if(powerUp == null)
        {
            text.text = "You reached the max power!!!";
        }
        else
        {
            text.text = powerUpDescription + " " + powerUp.PowerUpPercentage + " % ";
        }
       
    }
}
