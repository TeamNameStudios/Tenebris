using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class ControlsButton : MenuButton, IPointerClickHandler
{
    public ActionKeys actionKey;
    public TextMeshProUGUI controlText;
    public GameObject WaitForInputPanel;

    public override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        WaitForInputPanel.SetActive(true);
        controlText.text = "WAITING FOR INPUT...";
        KeymapController.Instance.StartRebind(actionKey);
    }
}
