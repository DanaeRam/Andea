using UnityEngine;

public class QuizDivision : MonoBehaviour
{
    [SerializeField] private PreguntaDivision quizManager;
    private bool alreadyTriggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("EnemyQuizTrigger detectó: " + other.name);

        if (alreadyTriggered) return;

        PlayerControlLock playerLock = other.GetComponentInParent<PlayerControlLock>();

        if (playerLock == null)
        {
            return;
        }

        GameObject player = playerLock.gameObject;

        if (!player.CompareTag("Player"))
        {
            return;
        }

        if (quizManager == null)
        {
            Debug.LogError("EnemyQuizTrigger: quizManager no está asignado");
            return;
        }

        Debug.Log("El jugador entró al trigger");
        alreadyTriggered = true;
        quizManager.StartQuiz(player);
    }

    public void ResetTrigger()
    {
        alreadyTriggered = false;
    }
}

