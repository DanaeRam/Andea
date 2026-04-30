using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class SwordAttackInput : MonoBehaviour
{
    private Animator anim;
    private Collider2D swordCollider;
    private PlayerControlLock controlLock;

    [Header("Visual de la espada")]
    [SerializeField] private SpriteRenderer swordVisual;

    [SerializeField] private float attackDuration = 0.2f;
    private bool isAttacking = false;

    private void Start()
    {
        anim = GetComponentInParent<Animator>();
        swordCollider = GetComponent<Collider2D>();
        controlLock = GetComponentInParent<PlayerControlLock>();

        if (swordCollider != null)
            swordCollider.enabled = false;

        if (swordVisual != null)
            swordVisual.enabled = true;
    }

    private void Update()
    {
        if (controlLock != null && !controlLock.canAttack)
            return;

        if (Keyboard.current != null && Keyboard.current.xKey.wasPressedThisFrame && !isAttacking)
        {
            StartCoroutine(AttackCoroutine());
        }
    }

    private IEnumerator AttackCoroutine()
    {
        isAttacking = true;

        if (anim != null)
            anim.SetTrigger("Attack");

        if (swordCollider != null)
            swordCollider.enabled = true;

        if (swordVisual != null)
            swordVisual.enabled = false;

        yield return new WaitForSeconds(attackDuration);

        if (swordCollider != null)
            swordCollider.enabled = false;

        if (swordVisual != null)
            swordVisual.enabled = true;

        isAttacking = false;
    }
}