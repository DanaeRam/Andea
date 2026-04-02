using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class EnemyPusherCombo2D : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform player;
    [SerializeField] private string playerTag = "Player";

    [Header("Proximity Push (constante)")]
    [SerializeField] private float pushRange = 1.2f;
    [SerializeField] private float pushForce = 12f;
    [SerializeField] private float pushUp = 0f;

    [Header("Contact Knockback (golpe)")]
    [SerializeField] private float knockbackForce = 10f;
    [SerializeField] private float knockUp = 2f;
    [SerializeField] private float hitCooldown = 0.35f;

    [Header("Safety")]
    [SerializeField] private float maxPlayerSpeed = 14f;

    private Rigidbody2D enemyRb;
    private Rigidbody2D playerRb;
    private PlayerControlLock playerControlLock;
    private float nextHitTime;

    private void Awake()
    {
        enemyRb = GetComponent<Rigidbody2D>();
        enemyRb.bodyType = RigidbodyType2D.Dynamic;
        enemyRb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    private void Start()
    {
        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag(playerTag);
            if (p != null) player = p.transform;
        }

        if (player != null)
        {
            playerRb = player.GetComponent<Rigidbody2D>();
            playerControlLock = player.GetComponent<PlayerControlLock>();
        }
    }

    private void FixedUpdate()
    {
        if (player == null || playerRb == null) return;

        if (playerControlLock != null && playerControlLock.isInQuiz)
            return;

        Vector2 enemyPos = enemyRb.position;
        Vector2 playerPos = playerRb.position;

        Vector2 delta = playerPos - enemyPos;
        float dist = delta.magnitude;

        float chaseRange = 10f;
        float stopDistance = 0.8f;
        float chaseSpeed = 3f;

        if (dist <= chaseRange && dist > stopDistance)
        {
            float dirX = Mathf.Sign(delta.x);
            Vector2 next = new Vector2(enemyPos.x + dirX * chaseSpeed * Time.fixedDeltaTime, enemyPos.y);
            enemyRb.MovePosition(next);
        }

        if (dist <= pushRange)
        {
            Vector2 dir = delta.normalized;

            playerRb.AddForce(new Vector2(dir.x * pushForce, pushUp), ForceMode2D.Force);

            if (playerRb.linearVelocity.magnitude > maxPlayerSpeed)
                playerRb.linearVelocity = playerRb.linearVelocity.normalized * maxPlayerSpeed;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!collision.collider.CompareTag(playerTag)) return;
        if (playerRb == null) return;
        if (Time.time < nextHitTime) return;
        if (playerControlLock != null && playerControlLock.isInQuiz) return;

        nextHitTime = Time.time + hitCooldown;

        Vector2 dir = ((Vector2)collision.transform.position - (Vector2)transform.position).normalized;

        playerRb.linearVelocity = new Vector2(playerRb.linearVelocity.x, 0f);
        playerRb.AddForce(new Vector2(dir.x * knockbackForce, knockUp), ForceMode2D.Impulse);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, pushRange);
    }
}
