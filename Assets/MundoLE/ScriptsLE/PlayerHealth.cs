using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Vida")]
    public int vidaMaxima = 5;

    [Header("Animación")]
    public Animator animator;

    [Header("Respawn")]
    public float tiempoAntesDeRespawn = 2f;

    private int vidaActual;
    private bool muerto = false;
    private Vector3 posicionInicial;

    private void Start()
    {
        vidaActual = vidaMaxima;
        posicionInicial = transform.position;

        if (animator == null)
            animator = GetComponent<Animator>();
    }

    public void RecibirDanio(int cantidad)
    {
        if (muerto) return;

        vidaActual -= cantidad;

        Debug.Log("Vida jugador: " + vidaActual);

        if (vidaActual <= 0)
        {
            Morir();
        }
    }

    private void Morir()
    {
        if (muerto) return;

        muerto = true;

        if (animator != null)
            animator.SetTrigger("dead");

        PlayerController controller = GetComponent<PlayerController>();
        if (controller != null)
            controller.enabled = false;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        StartCoroutine(RespawnCoroutine());
    }

    private IEnumerator RespawnCoroutine()
    {
        yield return new WaitForSeconds(tiempoAntesDeRespawn);

        transform.position = posicionInicial;

        vidaActual = vidaMaxima;
        muerto = false;

        if (animator != null)
        {
            animator.Rebind();
            animator.Update(0f);
        }

        PlayerController controller = GetComponent<PlayerController>();
        if (controller != null)
            controller.enabled = true;
    }
}