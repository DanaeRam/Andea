using UnityEngine;

public class FlagTrigger : MonoBehaviour
{
    [Header("Canvas del quiz")]
    public GameObject quizCanvas;

    [Header("Jugador")]
    public PlayerController playerController;

    private bool activated = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (activated) return;

        if (other.CompareTag("Player"))
        {
            activated = true;

            if (quizCanvas != null)
                quizCanvas.SetActive(true);

            if (playerController != null)
                playerController.enabled = false;
        }
    }
}