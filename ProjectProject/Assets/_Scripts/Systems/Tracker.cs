using UnityEngine;

public class Tracker : Singleton<Tracker>
{
    [SerializeField] public float distance;
    private float maxX = -Mathf.Infinity;
    private float actualDistance;
    private float playerDistance;
    private bool isBestDistanceGraveSpawned;
    public float bestDistance;
    [SerializeField] GameObject bestDistanceMarker;

    private void OnEnable()
    {
        EventManager<float>.Instance.StartListening("onPlayerChangeXVelociy", Move);
        EventManager<float>.Instance.StartListening("onBestDistanceLoaded", LoadBestDistance);
    }

    private void OnDisable()
    {
        EventManager<float>.Instance.StartListening("onPlayerChangeXVelociy", Move);
        EventManager<float>.Instance.StopListening("onBestDistanceLoaded", LoadBestDistance);
    }

    protected override void Awake()
    {
        base.Awake();
        playerDistance = 0;
        distance = 0;
        transform.position = new Vector3(0, 0, 0);
        isBestDistanceGraveSpawned = false;
    }

    private void Update()
    {
        if (GameController.Instance.State == GameState.PLAYING && GameController.Instance.IsTutorial == 0)
        {
            playerDistance += actualDistance;
            distance = Mathf.RoundToInt(playerDistance);

            if (distance > maxX)
            {
                maxX = distance;
            }

            EventManager<float>.Instance.TriggerEvent("UpdateDistanceCount", maxX);
        }
       
        if (GameController.Instance.State == GameState.LOSING && GameController.Instance.IsTutorial == 0)
        {
            if (maxX > bestDistance)
            {
                bestDistance = maxX;
                EventManager<float>.Instance.TriggerEvent("onBestDistanceUpdated", bestDistance);
            }

            EventManager<float>.Instance.TriggerEvent("SaveBestDistance", bestDistance);
        }

        // SPAWN BEST DISTANCE MARKER

        if (GameController.Instance.State == GameState.PLAYING && !isBestDistanceGraveSpawned && bestDistance > 0 && GameController.Instance.IsTutorial == 0)
        {
            if (bestDistance > 100)
            {
                if (distance > bestDistance - 100)
                {
                    Instantiate(bestDistanceMarker, new Vector3(100, 0, 0), Quaternion.identity);
                    isBestDistanceGraveSpawned = true;
                }
            }
            else if (bestDistance < 100)
            {
                Instantiate(bestDistanceMarker, new Vector3(bestDistance, 0, 0), Quaternion.identity);
                isBestDistanceGraveSpawned = true;
            }
        }
    }

    private void Move(float _velocity)
    {
        actualDistance = _velocity * Time.deltaTime;
    }

    private void LoadBestDistance(float _bestDistance)
    {
        bestDistance = _bestDistance;
    }
}
