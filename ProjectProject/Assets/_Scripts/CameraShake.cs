using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : Singleton<CameraShake>
{
    [SerializeField] private float duration;
    [SerializeField] private float magnitude;

    private void OnEnable()
    {
        EventManager<bool>.Instance.StartListening("onHit", Shake);
    }

    private void OnDisable()
    {
        EventManager<bool>.Instance.StopListening("onHit", Shake);
    }

    private IEnumerator ShakeCO()
    {
        Vector3 originalPos = transform.position;

        float elapsedTime = 0;

        while (elapsedTime < duration)
        {
            float xOffset = Random.Range(-.2f, .2f) * magnitude;

            Vector3 pos = transform.position;
            pos.x = xOffset;
            transform.position = pos;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.position = originalPos;
    }

    private void Shake(bool value)
    {
        StartCoroutine(ShakeCO());
    }
}
