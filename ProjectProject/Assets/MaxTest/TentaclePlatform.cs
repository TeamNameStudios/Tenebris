using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TentaclePlatform : MonoBehaviour
{
    public GameObject tentaclePrefab;
    private float initialPos;
    private float finalPos;
    public float aliveTimer;
    internal float currentTimer;
    private bool isAlive;
    public AnimationCurve animCurve;
    public float tentacleLenght;
    private Vector2 defaultPosition;
    public Collider2D coll2D;

 

    public void TentacleInit(Vector2 spawnPos)
    {
        tentaclePrefab.SetActive(true);
        coll2D = GetComponent<Collider2D>();
        spawnPos.y -= transform.localScale.y / 2;
        transform.position = spawnPos;
        initialPos = spawnPos.y;
        finalPos = initialPos + tentacleLenght;
        isAlive = true;
        currentTimer = 0;
        defaultPosition = tentaclePrefab.transform.position;
    }

    private void Start()
    {
     
    }

    // Update is called once per frame
    private void FixedUpdate()
    {

        currentTimer += Time.fixedDeltaTime;
        if(isAlive && currentTimer < aliveTimer)
        {
            float t = animCurve.Evaluate(currentTimer);
            float lerp = Mathf.Lerp(initialPos, finalPos, t);

            Vector2 posY = tentaclePrefab.transform.position;
            posY.y = lerp;
            tentaclePrefab.transform.position = posY;
        }
        else if(!isAlive)
        {
            EventManager<bool>.Instance.TriggerEvent("finishTentacle", true);
            tentaclePrefab.transform.position = defaultPosition;
            tentaclePrefab.SetActive(false);
        }
        else if(currentTimer >= aliveTimer)
            isAlive = false;
        

    }

}