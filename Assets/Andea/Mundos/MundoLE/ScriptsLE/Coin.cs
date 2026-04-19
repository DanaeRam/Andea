using UnityEngine;

public class Coin : MonoBehaviour
{
    [Header("Animación")]
    public Animator animator;

    [Header("Configuración")]
    public int valor = 1;
    public float tiempoAntesDeDestruir = 0.4f;

    [Header("Sonido")]
    public AudioClip sonidoRecoger;
    [Range(0f, 1f)] public float volumen = 1f;

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

            if (col != null)
                col.enabled = false;

            if (sonidoRecoger != null && Camera.main != null)
                AudioSource.PlayClipAtPoint(sonidoRecoger, Camera.main.transform.position, volumen);

            if (GameManager.instancia != null)
                GameManager.instancia.SumarMoneda(valor);

            if (animator != null)
                animator.SetTrigger("collect");

            Destroy(gameObject, tiempoAntesDeDestruir);
        }
    }
}