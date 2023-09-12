
using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private VolumeType volumeType;

    private void Start()
    {
        slider.onValueChanged.AddListener(volume => EventManager<float>.Instance.TriggerEvent(volumeType.ToString(), volume));
    }
}
