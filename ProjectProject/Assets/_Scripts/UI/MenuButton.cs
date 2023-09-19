using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class MenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private Vector3 originalScale;
    private TextMeshProUGUI text;
    private Color originalColor;
    [SerializeField] private Color targetColor;
    [SerializeField] private Color pressedColor;
    
    private void Awake()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
        originalScale = text.transform.localScale;
        originalColor = text.color;
    }

    private void OnEnable()
    {
        text.color = originalColor;
        text.transform.localScale = originalScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        text.color = targetColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        text.color = originalColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        EventManager<SoundEnum>.Instance.TriggerEvent("onPlayClip", SoundEnum.buttonSound);
        text.transform.localScale -= new Vector3(.15f, .15f, .15f);
        text.color = pressedColor;
    }
}
