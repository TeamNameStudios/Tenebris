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

    private void Awake()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
        originalScale = transform.localScale;
        originalColor = text.color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale -= new Vector3(.15f,.15f,.15f);
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = originalScale;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        EventManager<SoundEnum>.Instance.TriggerEvent("onPlayClip", SoundEnum.buttonSound);
        StartCoroutine(ChangeColor());
    }

    private IEnumerator ChangeColor()
    {
        text.color = targetColor;
        yield return new WaitForSeconds(.4f);
        text.color = originalColor;
    }
}
