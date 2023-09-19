using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class MenuButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private Vector3 originalScale;
    private TextMeshProUGUI text;
    private VertexGradient originalColor;
    [SerializeField] private VertexGradient targetColor;
    [SerializeField] private Color pressedColor;
    
    private void Awake()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
        originalScale = text.transform.localScale;
        originalColor = text.colorGradient;
    }

    private void OnEnable()
    {
        text.colorGradient = originalColor;
        text.color = Color.white;
        text.transform.localScale = originalScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        text.colorGradient = targetColor;
        EventManager<SoundEnum>.Instance.TriggerEvent("onPlayClip", SoundEnum.mouseHoveringSound);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        text.colorGradient = originalColor;    }

    public void OnPointerClick(PointerEventData eventData)
    {
        EventManager<SoundEnum>.Instance.TriggerEvent("onPlayClip", SoundEnum.buttonSound);
        text.transform.localScale -= new Vector3(.15f, .15f, .15f);
        text.color = pressedColor;
    }
}
