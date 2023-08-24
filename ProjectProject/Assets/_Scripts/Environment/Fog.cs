using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fog : MonoBehaviour
{
    //private float playerVelocity;
    //[SerializeField] private float fogSmoothness;
    //[SerializeField] private Vector2 fogSpeed;
    //[SerializeField] private float fogSize1;
    //[SerializeField] private Color fogColor1;
    //[SerializeField] private float fogSize2;
    //[SerializeField] private Color fogColor2;

    //private Material fog;

    //private void Awake()
    //{
    //    fog = GetComponent<SpriteRenderer>().material;
    //    fog.SetColor("_Color", fogColor1);
    //    fog.SetColor("_Color2", fogColor2);
    //}

    //private void Update()
    //{
    //    fog.SetVector("_FogSpeed", new Vector4(fogSpeed.x, fogSpeed.y, 0, 0));
    //    fog.SetFloat("_FogSize", fogSize1);
    //    fog.SetFloat("_FogSize2", fogSize2);
    //}

    //private void OnEnable()
    //{
    //    EventManager<float>.Instance.StartListening("onPlayerChangeXVelociy", SetVelocity);
    //}

    //private void OnDisable()
    //{
    //    EventManager<float>.Instance.StopListening("onPlayerChangeXVelociy", SetVelocity);
    //}

    //private void SetVelocity(float _velocity)
    //{
    //    playerVelocity = _velocity / fogSmoothness;
    //    fog.SetVector("_FogSpeed2", new Vector4(playerVelocity, 0, 0, 0));
    //}   
}
