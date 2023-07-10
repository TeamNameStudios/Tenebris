using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TentacleAttack : MonoBehaviour
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
    public Rigidbody2D rb2d;
    internal bool hook;

    public float tt { get; }
    
    Collider coll;

    public GameObject box;

    public void TentacleInit(Vector2 spawn)
    {
        initialPos = spawn.x - 6;
        Vector2 pos = new Vector2(initialPos, spawn.y);
        tentaclePrefab.transform.position = pos;
        finalPos = initialPos + tentacleLenght;
        isAlive = true;
        currentTimer = 0;
        tentaclePrefab.SetActive(true);
        defaultPosition = tentaclePrefab.transform.position;
    }

    private void OnEnable()
    {
        rb2d = rb2d.GetComponent<Rigidbody2D>();
        rb2d.WakeUp();
        Debug.Log($"The rigid body is kinematic: {rb2d.isKinematic}");

        coll = GetComponent<BoxCollider>();
    }

    private void Start()
    {
     
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pushable"))
            box = other.gameObject;
        if (other.CompareTag("Hookable"))
            EventManager<Vector2>.Instance.TriggerEvent("hook", other.transform.position);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Pushable"))
            box = collision.gameObject;
        if (collision.CompareTag("Hookable"))
            EventManager<Vector2>.Instance.TriggerEvent("hook", collision.transform.position);
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (hook)
        {
            currentTimer -= Time.fixedDeltaTime;

            float t = animCurve.Evaluate(currentTimer);
            float lerp = Mathf.Lerp(initialPos, finalPos, t);

            Vector2 posX = tentaclePrefab.transform.position;
            posX.x = lerp;
            tentaclePrefab.transform.position = posX;

            if (currentTimer <= 0.2f)
            {
                isAlive = false;
                hook = false;
            }
        }

        else if (!isAlive)
        {
            EventManager<bool>.Instance.TriggerEvent("atkAgn", true);
            tentaclePrefab.transform.position = defaultPosition;
            tentaclePrefab.SetActive(false);
            box = null;
        }

        else if (currentTimer >= aliveTimer)
            isAlive = false;

        else 
        {
            currentTimer += Time.fixedDeltaTime;
            float t = animCurve.Evaluate(currentTimer);
            float lerp = Mathf.Lerp(initialPos, finalPos, t);

            Vector2 posX = tentaclePrefab.transform.position;
            float posDelta = lerp - posX.x;
            posX.x = lerp;
            tentaclePrefab.transform.position = posX;

            if (box != null)
            {
                Vector2 posBoxX = box.transform.position;
                posBoxX.x += posDelta;
                box.transform.position = posBoxX;
            }
        }

        

    }

}
