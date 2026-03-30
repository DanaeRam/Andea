using UnityEngine;

public class JumpOnlyPlayer : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidad = 5f;

    [Header("Salto")]
    public float fuerzaSalto = 12f;
    public int saltosMaximos = 1;

    [Header("Wall Jump")]
    public float fuerzaWallJumpX = 7f;
    public float fuerzaWallJumpY = 12f;

    [Header("Detección")]
    public LayerMask capaSuelo;
    public float distanciaSuelo = 0.15f;
    public float distanciaPared = 0.1f;

    private Rigidbody2D rigidBody;
    private BoxCollider2D boxCollider;

    private bool mirandoDerecha = true;
    private int saltosRestantes;

    private void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
        saltosRestantes = saltosMaximos;
    }

    void Update()
    {
        ProcesarSalto();
    }

    private void FixedUpdate()
    {
        ProcesarMovimiento();
    }

    bool EstaEnSuelo()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(
            boxCollider.bounds.center,
            boxCollider.bounds.size,
            0f,
            Vector2.down,
            distanciaSuelo,
            capaSuelo
        );

        return raycastHit.collider != null;
    }

    bool TocaParedDerecha()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(
            boxCollider.bounds.center,
            boxCollider.bounds.size,
            0f,
            Vector2.right,
            distanciaPared,
            capaSuelo
        );

        return raycastHit.collider != null;
    }

    bool TocaParedIzquierda()
    {
        RaycastHit2D raycastHit = Physics2D.BoxCast(
            boxCollider.bounds.center,
            boxCollider.bounds.size,
            0f,
            Vector2.left,
            distanciaPared,
            capaSuelo
        );

        return raycastHit.collider != null;
    }

    void ProcesarSalto()
    {
        bool enSuelo = EstaEnSuelo();
        bool paredDerecha = TocaParedDerecha();
        bool paredIzquierda = TocaParedIzquierda();
        bool tocandoPared = paredDerecha || paredIzquierda;

        // Solo recarga el salto normal cuando toca el suelo
        if (enSuelo)
        {
            saltosRestantes = saltosMaximos;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Salto normal desde suelo
            if (enSuelo && saltosRestantes > 0)
            {
                saltosRestantes--;
                rigidBody.linearVelocity = new Vector2(rigidBody.linearVelocity.x, 0f);
                rigidBody.AddForce(Vector2.up * fuerzaSalto, ForceMode2D.Impulse);
            }
            // Wall jump si está en el aire tocando pared
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
            }
        }
    }

    void ProcesarMovimiento()
    {
        bool enSuelo = EstaEnSuelo();
        bool paredFrontal = (mirandoDerecha && TocaParedDerecha()) || (!mirandoDerecha && TocaParedIzquierda());

        float velocidadX = mirandoDerecha ? velocidad : -velocidad;

        // En suelo: corre automáticamente, pero si hay pared enfrente se detiene
        if (enSuelo)
        {
            if (paredFrontal)
            {
                rigidBody.linearVelocity = new Vector2(0f, rigidBody.linearVelocity.y);
            }
            else
            {
                rigidBody.linearVelocity = new Vector2(velocidadX, rigidBody.linearVelocity.y);
            }
        }
        else
        {
            // En el aire NO sigas empujando fuerte contra la pared
            // así evitamos que se "pegue"
            if (paredFrontal)
            {
                rigidBody.linearVelocity = new Vector2(0f, rigidBody.linearVelocity.y);
            }
            else
            {
                rigidBody.linearVelocity = new Vector2(velocidadX, rigidBody.linearVelocity.y);
            }
        }
    }

    void ActualizarOrientacionVisual()
    {
        Vector3 escala = transform.localScale;

        if (mirandoDerecha)
            escala.x = Mathf.Abs(escala.x);
        else
            escala.x = -Mathf.Abs(escala.x);

        transform.localScale = escala;
    }
}