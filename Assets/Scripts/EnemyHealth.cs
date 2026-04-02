using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int health = 100;
    public float deathGravity = 3f;
    public float destroyAfter = 2f;

    private Rigidbody2D rb;
    private Collider2D col;
    private bool isDead = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        health -= damage;

        if (health <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;

        MonoBehaviour[] scripts = GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in scripts)
        {
            if (script != this)
            {
                script.enabled = false;
            }
        }

        if (col != null)
        {
            col.isTrigger = true;
        }

        if (rb != null)
        {
            rb.constraints = RigidbodyConstraints2D.None;
            rb.gravityScale = deathGravity;
            rb.linearVelocity = new Vector2(0f, -2f);
            rb.angularVelocity = 200f;
        }

        Destroy(gameObject, destroyAfter);
    }
}