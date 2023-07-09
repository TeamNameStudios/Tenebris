using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartFadeIn : MonoBehaviour
{
    public float timer;
    public AnimationCurve fadeIn;

    private float _activeTimer = 0;
    private Image col;

    private void Start()
    {
        col = GetComponent<Image>();
    }

    private void FixedUpdate()
    {
        _activeTimer += Time.fixedDeltaTime;

        Color colFade = col.color;
        colFade.a = fadeIn.Evaluate(_activeTimer);
        col.color = colFade;

        if (_activeTimer >= timer)
            gameObject.SetActive(false);
    }

}
