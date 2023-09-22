using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ControlsButton : MonoBehaviour, IPointerClickHandler
{
    public ActionKeys actionKey;

    public void OnPointerClick(PointerEventData eventData)
    {
        KeymapController.Instance.StartRebind(actionKey);
    }
}
