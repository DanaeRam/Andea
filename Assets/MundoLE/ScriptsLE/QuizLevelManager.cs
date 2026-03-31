using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class QuizLevelManager : MonoBehaviour
{
    [Header("Botones")]
    public Button Button1; // Respuesta correcta
    public Button Button2; // Respuesta incorrecta
    public Button Button3; // Respuesta incorrecta
    public Button Button4; // Respuesta incorrecta

    [Header("Feedback")]
    public GameObject FeedbackBox;
    public TextMeshProUGUI FeedbackText;
    public float feedbackDuration = 1.5f;

    [Header("Configuración")]
    public bool Button1IsCorrect = true;

    private int attempts = 0;
    private bool answered = false;

    private void Start()
    {
        if (FeedbackBox != null)
            FeedbackBox.SetActive(false);

        if (FeedbackText != null)
            FeedbackText.text = "";

        if (Button1 != null) Button1.onClick.AddListener(() => CheckAnswer(Button1IsCorrect));
        if (Button2 != null) Button2.onClick.AddListener(() => CheckAnswer(false));
        if (Button3 != null) Button3.onClick.AddListener(() => CheckAnswer(false));
        if (Button4 != null) Button4.onClick.AddListener(() => CheckAnswer(false));
    }

    private void CheckAnswer(bool isCorrect)
    {
        if (answered) return;

        answered = true;

        if (FeedbackBox != null)
            FeedbackBox.SetActive(true);

        if (isCorrect)
        {
            if (FeedbackText != null)
                FeedbackText.text = "¡Correcto!";

            StartCoroutine(LoadNextLevelAfterDelay());
        }
        else
        {
            attempts++;

            if (attempts == 1)
            {
                if (FeedbackText != null)
                    FeedbackText.text = "Respuesta incorrecta. Intenta otra vez.";

                StartCoroutine(SecondChance());
            }
            else
            {
                if (FeedbackText != null)
                    FeedbackText.text = "Fallaste de nuevo. Reiniciando...";

                StartCoroutine(RestartLevelAfterDelay());
            }
        }
    }

    private IEnumerator SecondChance()
    {
        yield return new WaitForSeconds(feedbackDuration);

        answered = false;

        if (FeedbackBox != null)
            FeedbackBox.SetActive(false);

        if (FeedbackText != null)
            FeedbackText.text = "";
    }

    private IEnumerator LoadNextLevelAfterDelay()
    {
        yield return new WaitForSeconds(feedbackDuration);

        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;

        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextSceneIndex);
        }
        else
        {
            Debug.Log("No hay un siguiente nivel configurado en Build Settings.");
        }
    }

    private IEnumerator RestartLevelAfterDelay()
    {
        yield return new WaitForSeconds(feedbackDuration);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}