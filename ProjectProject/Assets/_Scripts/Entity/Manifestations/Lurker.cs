using UnityEngine;

public class Lurker : Manifestation
{
    private enum LurkerState
    {
        IDLE,
        CHASING,
        FALLING
    }
    private LurkerState state = LurkerState.IDLE;

    private Transform player;
    private CapsuleCollider2D capsuleCollider;
    private LineRenderer lr;
    [SerializeField]
    private float fallTime;
    [SerializeField]
    private float startFallVelocity;
    [SerializeField]
    private float startFallVelocityIncrement;
    [SerializeField]
    private float upDownVelocity;
    [SerializeField]
    private float upDownAmplitude;

    private float fallVelocity;
    private float elapsedTime = 0;
    private float landingPoint;
    private Vector2 startPos;


    protected override void OnEnable()
    {
        base.OnEnable();
        EventManager<bool>.Instance.StartListening("onLevelUp", LevelUp);
        state = LurkerState.IDLE;
        player = null;
        elapsedTime = 0;
        fallVelocity = startFallVelocity;
    }

    private void Awake()
    {
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        lr = GetComponent<LineRenderer>();
    }

    public override void Update()
    {
        base.Update();

        switch (state)
        {
            case LurkerState.IDLE:
                FindPlayer();
                startPos = transform.position;
                fallVelocity = startFallVelocity;
                if (destructionCO != null)
                {
                    StopCoroutine(destructionCO);
                }
                break;

            case LurkerState.CHASING:
                Vector2 pos = transform.position;
                pos.x = player.position.x + 2.5f;
                float y = Mathf.PingPong(Time.time * upDownVelocity, upDownAmplitude) * 2;
                pos.y = y + startPos.y - 4;
                transform.position = pos;
                elapsedTime += Time.deltaTime;

                if (elapsedTime >= fallTime - 0.5f)
                {
                    landingPoint = FindLandingPoint();
                    lr.enabled = true;
                    lr.SetPosition(0, (transform.position - Vector3.up));
                    lr.SetPosition(1, new Vector2(transform.position.x, landingPoint));
                    if (elapsedTime >= fallTime)
                    {
                        state = LurkerState.FALLING;
                    }
                }
                break;

            case LurkerState.FALLING:
                Vector2 position = transform.position;
                position.y += Vector2.down.y * fallVelocity * Time.deltaTime;
                lr.SetPosition(0, (transform.position - Vector3.up));
                lr.SetPosition(1, new Vector2(transform.position.x, landingPoint));
                if (transform.position.y <= landingPoint)
                {
                    lr.enabled = false;
                    fallVelocity = 0;
                    position.y = landingPoint + capsuleCollider.size.y / 2;
                    destructionCO = StartCoroutine(AutoDestruction());
                }

                transform.position = position;
                break;
        }
    }

    private void FindPlayer()
    {
        RaycastHit2D hit = Physics2D.CapsuleCast(transform.position - new Vector3(0, 2, 0), capsuleCollider.size, capsuleCollider.direction, 0, Vector2.down);
        if (hit.transform != null)
        {
            if (hit.transform.GetComponent<Player>())
            {
                player = hit.transform;
                state = LurkerState.CHASING;
                PlayClip();
            }
        }
    }

    private float FindLandingPoint()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, Vector2.down);
        for (int i = 0; i < hits.Length; i++)
        {
            if (!hits[i].transform.GetComponent<Player>() && !hits[i].transform.GetComponent<Manifestation>() && !hits[i].transform.GetComponent<Collectible>())
            {
                return hits[i].point.y;
            }
        }

        return -50;
    }

    private void LevelUp(bool value)
    {
        startFallVelocity += startFallVelocityIncrement;
    }
}
