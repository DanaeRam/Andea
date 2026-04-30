using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip sonidoSalto;
    public AudioClip sonidoAtaque;

    [Header("Movimiento")]
    public float velocidad = 5f;

    [Header("Salto")]
    public float fuerzaSalto = 12f;
    public int saltosMaximos = 1;
    public ParticleSystem particulaSalto;

    [Header("Wall Jump")]
    public float fuerzaWallJumpX = 7f;
    public float fuerzaWallJumpY = 12f;

    [Header("Detección")]
    public LayerMask capaSuelo;
    public float distanciaSuelo = 0.15f;
    public float distanciaPared = 0.1f;

    [Header("Ataque")]
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public LayerMask enemyLayers;
    public int attackDamage = 1;
    public float attackCooldown = 0.5f;

    [Header("Animación")]
    public Animator animator;

    private Rigidbody2D rigidBody;
    private BoxCollider2D boxCollider;

    private bool mirandoDerecha = true;
    private int saltosRestantes;
    private bool puedeAtacar = true;

    private void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();

        if (animator == null)
            animator = GetComponent<Animator>();

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        saltosRestantes = saltosMaximos;
    }

    private void Update()
    {
        ProcesarSalto();
        ProcesarAtaque();
        ActualizarAnimaciones();
    }

    private void FixedUpdate()
    {
        ProcesarMovimiento();
    }

    private bool EstaEnSuelo()
    {
        return Physics2D.BoxCast(
            boxCollider.bounds.center,
            boxCollider.bounds.size,
            0f,
            Vector2.down,
            distanciaSuelo,
            capaSuelo
        );
    }

    private bool TocaParedDerecha()
    {
        return Physics2D.BoxCast(
            boxCollider.bounds.center,
            boxCollider.bounds.size,
            0f,
            Vector2.right,
            distanciaPared,
            capaSuelo
        );
    }

    private bool TocaParedIzquierda()
    {
        return Physics2D.BoxCast(
            boxCollider.bounds.center,
            boxCollider.bounds.size,
            0f,
            Vector2.left,
            distanciaPared,
            capaSuelo
        );
    }

    private void crearParticulaSalto()
    {
        particulaSalto.Play();
    }
    private void ProcesarSalto()
    {
        bool enSuelo = EstaEnSuelo();
        bool paredDerecha = TocaParedDerecha();
        bool paredIzquierda = TocaParedIzquierda();
        bool tocandoPared = paredDerecha || paredIzquierda;

        if (enSuelo)
        {
            saltosRestantes = saltosMaximos;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (enSuelo && saltosRestantes > 0)
            {
                saltosRestantes--;

                rigidBody.linearVelocity = new Vector2(rigidBody.linearVelocity.x, 0f);
                rigidBody.AddForce(Vector2.up * fuerzaSalto, ForceMode2D.Impulse);

                animator.SetTrigger("jump");
                crearParticulaSalto();

                if (audioSource != null && sonidoSalto != null)
                    audioSource.PlayOneShot(sonidoSalto);
            }
            else if (!enSuelo && tocandoPared)
            {
                float direccionSalida = paredDerecha ? -1f : 1f;

                mirandoDerecha = direccionSalida > 0;
                ActualizarOrientacionVisual();

                rigidBody.linearVelocity = Vector2.zero;
                rigidBody.AddForce(
                    new Vector2(direccionSalida * fuerzaWallJumpX, fuerzaWallJumpY),
                    ForceMode2D.Impulse
                );

                animator.SetTrigger("jump");
                crearParticulaSalto();

                if (audioSource != null && sonidoSalto != null)
                    audioSource.PlayOneShot(sonidoSalto);
            }
        }
    }

    private void ProcesarAtaque()
    {
        if (Input.GetKeyDown(KeyCode.X) && puedeAtacar)
        {
            Atacar();
        }
    }

    private void Atacar()
    {
        puedeAtacar = false;

        animator.SetTrigger("attack");

        if (audioSource != null && sonidoAtaque != null)
            audioSource.PlayOneShot(sonidoAtaque);

        if (attackPoint != null)
        {
            Collider2D[] enemigosGolpeados = Physics2D.OverlapCircleAll(
                attackPoint.position,
                attackRange,
                enemyLayers
            );

            foreach (Collider2D enemigo in enemigosGolpeados)
            {
                Enemy enemyScript = enemigo.GetComponent<Enemy>();

                if (enemyScript != null)
                {
                    enemyScript.RecibirDanio(attackDamage);
                }
            }
        }

        Invoke(nameof(ReiniciarAtaque), attackCooldown);
    }

    private void ReiniciarAtaque()
    {
        puedeAtacar = true;
    }

    private void ProcesarMovimiento()
    {
        if (!puedeAtacar)
        {
            rigidBody.linearVelocity = new Vector2(0f, rigidBody.linearVelocity.y);
            return;
        }

        bool paredFrontal =
            (mirandoDerecha && TocaParedDerecha()) ||
            (!mirandoDerecha && TocaParedIzquierda());

        float velocidadX = mirandoDerecha ? velocidad : -velocidad;

        rigidBody.linearVelocity = paredFrontal
            ? new Vector2(0f, rigidBody.linearVelocity.y)
            : new Vector2(velocidadX, rigidBody.linearVelocity.y);
    }

    private void ActualizarOrientacionVisual()
    {
        Vector3 escala = transform.localScale;
        escala.x = mirandoDerecha ? Mathf.Abs(escala.x) : -Mathf.Abs(escala.x);
        transform.localScale = escala;
    }

    private void ActualizarAnimaciones()
    {
        bool enSuelo = EstaEnSuelo();

        animator.SetBool("isGrounded", enSuelo);
        animator.SetFloat("velocidadY", rigidBody.linearVelocity.y);

        float velocidadHorizontal = Mathf.Abs(rigidBody.linearVelocity.x);
        animator.SetFloat("speed", velocidadHorizontal > 0.1f ? 1f : 0f);
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}