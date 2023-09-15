using UnityEngine;

public class Chaser : Manifestation
{
    private enum ChaserState
    {
        IDLE,
        DESCENDING,
        CHASING,
        ATTACKING
    }

    private ChaserState state = ChaserState.IDLE;
    
    private Transform player;
    private CapsuleCollider2D capsuleCollider;

    [Tooltip("Distance to cover before the chaser starts chasing the player")]
    [SerializeField] private float minDistance;
    private float chaseVelocity;
    [Tooltip("Starting velocity of the chaser")]
    [SerializeField] private float startVelocity;
    [SerializeField] private float startVelocityIncrement;
    [Tooltip("Keep this value small, ex. 0.02")]
    [SerializeField] private float velocityAcceleration;
    [Tooltip("How close the chaser must get to the player before attacking it")]
    [SerializeField] private float attackDistance;


    //Velocity of the chaser's attack
    private float attackVelocity;
    private float groundHeight;
    private Vector3 attackDirection;

    protected override void OnEnable()
    {
        base.OnEnable();
        EventManager<bool>.Instance.StartListening("onLevelUp", LevelUp);
        player = null;
        state = ChaserState.IDLE;
        chaseVelocity = startVelocity;
    }

    private void Awake()
    {
        capsuleCollider = GetComponent<CapsuleCollider2D>();

    }

    public override void Update()
    {
        base.Update();
        switch (state)
        {
            case ChaserState.IDLE:
                FindPlayer();
                if (player != null && Mathf.Abs(transform.position.x - player.position.x) > minDistance)
                {
                    Vector2 pos = transform.position;
                    pos.y = groundHeight + capsuleCollider.size.y / 2;
                    transform.position = pos;
                    state = ChaserState.DESCENDING;
                    PlayClip();
                }
                break;

            case ChaserState.DESCENDING:
                    
                if (transform.position.y - groundHeight <= 1.2f )
                {
                    state = ChaserState.CHASING;
                }
                break;

            case ChaserState.CHASING:

                Vector2 position = transform.position;
                position.x += Vector2.right.x * chaseVelocity * Time.deltaTime;
                transform.position = position;
                chaseVelocity += velocityAcceleration;
                
                if (Mathf.Abs(transform.position.x - player.position.x) <= attackDistance)
                {
                    attackDirection = (player.position - transform.position).normalized;
                    attackVelocity = chaseVelocity + 10;
                    state = ChaserState.ATTACKING;
                }
                break;
            
            case ChaserState.ATTACKING:

                transform.position += attackDirection * attackVelocity * Time.deltaTime;
                StartCoroutine(AutoDestruction());
                break;
        }
    }

    private void FindPlayer()
    {
        RaycastHit2D[] hits = Physics2D.CapsuleCastAll(transform.position, capsuleCollider.size, capsuleCollider.direction, 0, Vector2.down);

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].transform.GetComponent<Player>())
            {
                player = hits[i].transform;
            }
            else
            {

                groundHeight = hits[i].point.y;
            }
        }
        
        if (groundHeight == 0 && player != null)
        {
            groundHeight = player.transform.position.y;
        }
    }

    private void LevelUp(bool value)
    {
        startVelocity += startVelocityIncrement;
    }
}
