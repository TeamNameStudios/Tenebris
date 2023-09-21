using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PowerUpMenu : MonoBehaviour
{
    [SerializeField]
    public List<ShopButton> buttonList = new List<ShopButton>();
    [SerializeField]
    public List<PowerUpDescriptionUI> descriptionList = new List<PowerUpDescriptionUI>();
    [SerializeField]
    public List<PowerUpLevelUI> PowerUpLevel = new List<PowerUpLevelUI>();

    [SerializeField]
    public TextMeshProUGUI TotalPagesUI;
    [SerializeField]
    public int totalPages;
    [SerializeField]
    public Sprite empty;
    [SerializeField]
    public Sprite full;
    List<PowerUp> actualPowerUps = new List<PowerUp>();
    [SerializeField]
    private GameObject ConfirmPanel;

    private ShopButton buttonClicked;

    public void OnEnable()
    {
        EventManager<int>.Instance.StartListening("onTotalPageLoaded", ManageTotalPage);
        EventManager<List<PowerUp>>.Instance.StartListening("onPowerUpLoaded", ManagePowerUp);
        EventManager<bool>.Instance.TriggerEvent("LoadData", true);
    }

    public void OnDisable()
    {
        EventManager<int>.Instance.StopListening("onTotalPageLoaded", ManageTotalPage);
        EventManager<List<PowerUp>>.Instance.StopListening("onPowerUpLoaded", ManagePowerUp);
    }

    public void OpenConfirmPanel(int ButtonId)
    {
        ShopButton _buttonClicked = buttonList[ButtonId];
        if (_buttonClicked.pageCost <= totalPages)
        {
            buttonClicked = _buttonClicked;
            ConfirmPanel.SetActive(true);
        }
    }

    public void CloseConfirmPanel()
    {
        ConfirmPanel.SetActive(false);
    }

    public void BuyPowerUp()
    {
        if (buttonClicked.pageCost <= totalPages)
        {
            List<PowerUp> newList = new List<PowerUp>();
            for(int i = 0; i < actualPowerUps.Count; i++)
            {
                if (actualPowerUps[i].ID == buttonClicked.id && actualPowerUps[i].Level <= 5)
                {
                    newList.Add(new PowerUp(actualPowerUps[i].ID, actualPowerUps[i].Level + 1));
                    totalPages = totalPages - buttonClicked.pageCost;
                    EventManager<int>.Instance.TriggerEvent("SaveTotalPage", totalPages);
                }
                else
                {
                    newList.Add(actualPowerUps[i]);
                }
            }
            EventManager<List<PowerUp>>.Instance.TriggerEvent("SavePowerUp", newList);
        }
        EventManager<bool>.Instance.TriggerEvent("LoadData", true);
        ConfirmPanel.SetActive(false);
        buttonClicked = null;
    }

    private void ManagePowerUp(List<PowerUp> powerUps)
    {
        actualPowerUps = powerUps;
        for (int i = 0; i < powerUps.Count; i++) {
            ScriptablePowerUp scriptablePowerUp = ResourceSystem.Instance.GetPowerUp(powerUps[i].ID, powerUps[i].Level+1);
            buttonList[i].UpdateUI(scriptablePowerUp);
            descriptionList[i].UpdateUI(scriptablePowerUp);
            PowerUpLevel[i].UpdateUI(powerUps[i].Level + 1, empty,full);
        }
    }

    private void ManageTotalPage(int _totalPages) {
        totalPages = _totalPages;
        TotalPagesUI.text = "Pages:"+totalPages;
    }
}
