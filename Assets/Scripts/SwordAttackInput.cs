using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class SwordAttackInput : MonoBehaviour
{
    private Animator anim;
    private Collider2D swordCollider;
    private PlayerControlLock controlLock;

    [SerializeField] private float attackDuration = 0.2f;
    private bool isAttacking = false;

    private void Start()
    {
        anim = GetComponent<Animator>();
        swordCollider = GetComponent<Collider2D>();
        controlLock = GetComponentInParent<PlayerControlLock>();

        if (swordCollider != null)
            swordCollider.enabled = false;
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

        yield return new WaitForSeconds(attackDuration);

        if (swordCollider != null)
            swordCollider.enabled = false;

        isAttacking = false;
    }
}