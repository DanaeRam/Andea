using UnityEngine;

public class FlagTrigger : MonoBehaviour
{
    [Header("Jugador")]
    public PlayerController playerController;

    private bool activated = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (activated) return;

        if (other.CompareTag("Player"))
        {
            activated = true;

            if (playerController != null)
                playerController.enabled = false;

            if (LectoGameSessionManager.Instance != null)
            {
                Debug.Log("Meta alcanzada. Cargando escena de pregunta...");
                LectoGameSessionManager.Instance.LoadQuizSceneForCurrentRound();
            }
            else
            {
                Debug.LogError("No existe LectoGameSessionManager en la escena.");
            }
        }
    }
}