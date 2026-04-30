using UnityEngine;
using UnityEngine.InputSystem;

public class MoverConInputAction : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidadX = 7f;
    public float fuerzaSalto = 10f;

    [Header("Referencias")]
    public SpriteRenderer spriteRenderer;

    private Rigidbody2D rb;
    private Animator animator;

    private float moveX;
    private bool enSuelo = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        if (rb == null)
            Debug.LogError("Falta Rigidbody2D en el personaje.", this);

        if (animator == null)
            Debug.LogError("Falta Animator en el personaje.", this);

        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        moveX = 0f;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
                moveX = -1f;
            else if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
                moveX = 1f;

            if ((Keyboard.current.spaceKey.wasPressedThisFrame || Keyboard.current.wKey.wasPressedThisFrame || Keyboard.current.upArrowKey.wasPressedThisFrame) && enSuelo)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, fuerzaSalto);
                enSuelo = false;
            }
        }

        if (animator != null)
        {
            animator.SetBool("IsWalking", Mathf.Abs(moveX) > 0.1f);
            animator.SetFloat("MoveX", moveX);
        }

        if (spriteRenderer != null)
        {
            if (moveX > 0)
                spriteRenderer.flipX = true;
            else if (moveX < 0)
                spriteRenderer.flipX = false;
        }
    }

    private void FixedUpdate()
    {
        if (rb == null) return;

        rb.linearVelocity = new Vector2(moveX * velocidadX, rb.linearVelocity.y);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            enSuelo = true;
        }
    }
}