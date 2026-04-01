using UnityEngine;

public class Coin : MonoBehaviour
{
    [Header("Animación")]
    public Animator animator;

    [Header("Configuración")]
    public int valor = 1;
    public float tiempoAntesDeDestruir = 0.4f;

    private bool recogida = false;
    private CircleCollider2D col;

    private void Start()
    {
        col = GetComponent<CircleCollider2D>();

        if (animator == null)
            animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (recogida) return;

        if (other.CompareTag("Player"))
        {
            recogida = true;

            // Desactiva el collider para evitar múltiples activaciones
            if (col != null)
                col.enabled = false;

            // Sumar al contador
            if (GameManager.instancia != null)
                GameManager.instancia.SumarMoneda(valor);

            // Activar animación
            if (animator != null)
                animator.SetTrigger("collect");
            // Destruir después de la animación
            Destroy(gameObject, tiempoAntesDeDestruir);
        }
    }
}