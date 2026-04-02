using UnityEngine;

public class SwordDamage : MonoBehaviour
{
    public int damage = 10;
    [SerializeField] private string enemyTag = "Enemy";

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(enemyTag) && other.GetComponentInParent<EnemyHealth>() == null)
            return;

        Debug.Log("La espada tocó a: " + other.name);

        EnemyHealth enemy = other.GetComponent<EnemyHealth>();

        if (enemy == null)
        {
            enemy = other.GetComponentInParent<EnemyHealth>();
        }

        if (enemy != null)
        {
            Debug.Log("EnemyHealth encontrado, aplicando daño");
            enemy.TakeDamage(damage);
        }
        else
        {
            Debug.Log("No encontré EnemyHealth en este objeto ni en su padre");
        }
    }
}