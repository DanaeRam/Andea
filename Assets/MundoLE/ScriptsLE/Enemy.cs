using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Vida")]
    public int vidaMaxima = 3;

    [Header("Animación")]
    public Animator animator;

    [Header("Destrucción")]
    public float tiempoAntesDeDestruir = 1f;

    [Header("Ataque")]
    public float rangoAtaque = 1.2f;
    public int danio = 1;
    public float tiempoEntreAtaques = 1.5f;
    public LayerMask playerLayer;

    private int vidaActual;
    private bool muerto = false;
    private bool puedeAtacar = true;
    private Transform jugador;

    private void Start()
    {
        vidaActual = vidaMaxima;

        if (animator == null)
            animator = GetComponent<Animator>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            jugador = playerObj.transform;
    }

    private void Update()
    {
        if (muerto) return;

        IntentarAtacar();
    }

    public void RecibirDanio(int cantidad)
    {
        if (muerto) return;

        vidaActual -= cantidad;

        if (vidaActual > 0)
        {
            if (animator != null)
                animator.SetTrigger("hurt");
        }
        else
        {
            Morir();
        }
    }

   private void Morir()
    {
        if (muerto) return;

        muerto = true;

        if (GameManager.instancia != null)
        {
            GameManager.instancia.RegistrarEnemigoDerrotado();
        }

        if (animator != null)
            animator.SetTrigger("dead");

        Destroy(gameObject, tiempoAntesDeDestruir);
    }
    private void IntentarAtacar()
    {
        if (!puedeAtacar || jugador == null) return;

        float distancia = Vector2.Distance(transform.position, jugador.position);

        if (distancia <= rangoAtaque)
        {
            AtacarJugador();
        }
    }

    private void AtacarJugador()
    {
        puedeAtacar = false;

        if (animator != null)
            animator.SetTrigger("attack");

        Collider2D jugadorDetectado = Physics2D.OverlapCircle(
            transform.position,
            rangoAtaque,
            playerLayer
        );

        if (jugadorDetectado != null)
        {
            PlayerHealth vidaJugador = jugadorDetectado.GetComponent<PlayerHealth>();

            if (vidaJugador != null)
            {
                vidaJugador.RecibirDanio(danio);
            }
        }

        Invoke(nameof(ReiniciarAtaque), tiempoEntreAtaques);
    }

    private void ReiniciarAtaque()
    {
        puedeAtacar = true;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, rangoAtaque);
    }
}