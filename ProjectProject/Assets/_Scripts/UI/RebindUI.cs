using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class RebindUI : MonoBehaviour
{
    public ActionKeys key;
    public TextMeshProUGUI jumpText;
    public TextMeshProUGUI dashText;
    public TextMeshProUGUI grappleText;
    public GameObject WaitForInputPanel;

    private void OnEnable()
    {
        EventManager<bool>.Instance.StartListening("onRebindCompleted", RebindCompleted);
        EventManager<bool>.Instance.StartListening("onRebindFailed", RebindFailed);
    }

    private void OnDisable()
    {
        EventManager<bool>.Instance.StopListening("onRebindCompleted", RebindCompleted);
        EventManager<bool>.Instance.StopListening("onRebindFailed", RebindFailed);
    }

    private void Start()
    {
        UpdateControlsText(true);
    }

    private void UpdateControlsText(bool value)
    {
        jumpText.text = KeymapController.Instance.jumpKey.ToString();
        dashText.text = KeymapController.Instance.dashKey.ToString();
        grappleText.text = KeymapController.Instance.grappleKey.ToString();
    }

    private void RebindCompleted(bool value)
    {
        WaitForInputPanel.SetActive(false);
        UpdateControlsText(true);
    }

    private void RebindFailed(bool value)
    {
        UpdateControlsText(true);
        StartCoroutine(FadeCO());
    }

    private IEnumerator FadeCO()
    {
        WaitForInputPanel.GetComponent<Image>().color = new Color(1, 0, 0, .2f);
        yield return new WaitForSeconds(.2f);
        WaitForInputPanel.GetComponent<Image>().color = new Color (0,0,0,0);
        WaitForInputPanel.SetActive(false);
    }
}
