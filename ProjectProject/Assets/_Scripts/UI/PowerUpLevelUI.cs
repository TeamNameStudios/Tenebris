using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class PowerUpLevelUI : MonoBehaviour
{
    public int level;
    public List<Image> powerLevel = new List<Image>();


    public void UpdateUI(int _level, Sprite Empty, Sprite Complete )
    {

     
            level = _level - 1;
            for (int i = 0; i < powerLevel.Count; i++)
            {
                if (i < level)
                {
                    powerLevel[i].sprite = Complete;
                }
                else
                {
                    powerLevel[i].sprite = Empty;
                }

            }
        

    }
}
