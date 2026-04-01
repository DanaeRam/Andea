using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Vida")]
    public int vidaMaxima = 3;

    [Header("Animación")]
    public Animator animator;

    private int vidaActual;
    private bool muerto = false;

    private void Start()
    {
        vidaActual = vidaMaxima;

        if (animator == null)
            animator = GetComponent<Animator>();
    }

    public void RecibirDanio(int cantidad)
    {
        if (muerto) return;

        vidaActual -= cantidad;

        if (vidaActual <= 0)
        {
            Morir();
        }
    }

    private void Morir()
    {
        muerto = true;

        if (animator != null)
            animator.SetTrigger("dead");

        Destroy(gameObject, 1f);
    }
}