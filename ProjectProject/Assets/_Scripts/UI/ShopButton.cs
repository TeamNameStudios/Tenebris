using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShopButton : MonoBehaviour
{

    [SerializeField]
    public PowerUpEnum id;
    [SerializeField]
    public int level;
    [SerializeField]
    public int pageCost;
    [SerializeField]
    public TextMeshProUGUI text;

    public void UpdateUI(ScriptablePowerUp powerUp)
    {
        id = powerUp.ID;
        level = powerUp.Level;
        pageCost = powerUp.PageCost;    
        text.text = powerUp.PageCost + "pages";

    }
}
