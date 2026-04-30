using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class QuizLevelManager : MonoBehaviour
{
    [Header("Pregunta")]
    public TextMeshProUGUI questionText;

    [Header("Textos de respuestas")]
    public TextMeshProUGUI button1Text;
    public TextMeshProUGUI button2Text;
    public TextMeshProUGUI button3Text;
    public TextMeshProUGUI button4Text;

    [Header("Botones")]
    public Button Button1;
    public Button Button2;
    public Button Button3;
    public Button Button4;

    [Header("Feedback")]
    public GameObject FeedbackBox;
    public TextMeshProUGUI FeedbackText;
    public float feedbackDuration = 1.5f;

    [Header("Configuración")]
    public int maxAttempts = 3;

    private bool answered = false;
    private QuizAttemptSystem attemptSystem;
    private LessonQuestionData currentQuestionData;

    private void Start()
    {
        attemptSystem = new QuizAttemptSystem(maxAttempts);

        if (FeedbackBox != null)
            FeedbackBox.SetActive(false);

        if (FeedbackText != null)
            FeedbackText.text = "";

        LoadCurrentQuestion();
        SetupButtons();
    }

    private void LoadCurrentQuestion()
    {
        if (LectoGameSessionManager.Instance == null)
        {
            Debug.LogError("No existe LectoGameSessionManager.");
            return;
        }

        currentQuestionData = LectoGameSessionManager.Instance.CurrentQuestion;

        if (currentQuestionData == null)
        {
            Debug.LogError("No hay pregunta actual.");
            return;
        }

        if (questionText != null)
            questionText.text = currentQuestionData.questionText;

        if (button1Text != null)
            button1Text.text = currentQuestionData.option1;

        if (button2Text != null)
            button2Text.text = currentQuestionData.option2;

        if (button3Text != null)
            button3Text.text = currentQuestionData.option3;

        if (button4Text != null)
            button4Text.text = currentQuestionData.option4;
    }

    private void SetupButtons()
    {
        if (Button1 != null)
        {
            Button1.onClick.RemoveAllListeners();
            Button1.onClick.AddListener(() => CheckAnswer(1));
        }

        if (Button2 != null)
        {
            Button2.onClick.RemoveAllListeners();
            Button2.onClick.AddListener(() => CheckAnswer(2));
        }

        if (Button3 != null)
        {
            Button3.onClick.RemoveAllListeners();
            Button3.onClick.AddListener(() => CheckAnswer(3));
        }

        if (Button4 != null)
        {
            Button4.onClick.RemoveAllListeners();
            Button4.onClick.AddListener(() => CheckAnswer(4));
        }
    }

    private void CheckAnswer(int selectedOption)
    {
        if (answered) return;

        if (attemptSystem != null && !attemptSystem.CanAnswer())
            return;

        answered = true;

        if (FeedbackBox != null)
            FeedbackBox.SetActive(true);

        bool isCorrect = selectedOption == currentQuestionData.correctOption;

        if (isCorrect)
        {
            if (FeedbackText != null)
                FeedbackText.text = currentQuestionData.correctFeedback;

            StartCoroutine(LoadNextAfterDelay());
        }
        else
        {
            HandleWrongAnswer();
        }
    }

    private void HandleWrongAnswer()
    {
        attemptSystem.RegisterWrongAnswer();

        if (GameManager.instancia != null)
            GameManager.instancia.RegistrarRespuestaIncorrectaNivel();

        if (attemptSystem.CanAnswer())
        {
            if (FeedbackText != null)
                FeedbackText.text = currentQuestionData.wrongFeedback + "\nTe queda " + attemptSystem.RemainingAttempts + " intento.";

            StartCoroutine(AllowNextAttemptAfterDelay());
        }
        else
        {
            if (FeedbackText != null)
                FeedbackText.text = currentQuestionData.wrongFeedback + "\nFallaste de nuevo. Intenta otra vez esta pregunta.";

            StartCoroutine(RestartQuestionAfterDelay());
        }
    }

    private IEnumerator AllowNextAttemptAfterDelay()
    {
        yield return new WaitForSeconds(feedbackDuration);

        answered = false;

        if (FeedbackBox != null)
            FeedbackBox.SetActive(false);

        if (FeedbackText != null)
            FeedbackText.text = "";
    }

    private IEnumerator LoadNextAfterDelay()
    {
        yield return new WaitForSeconds(feedbackDuration);

        if (LectoGameSessionManager.Instance != null)
        {
            LectoGameSessionManager.Instance.GoToNextRoundOrResults();
        }
    }

    private IEnumerator RestartQuestionAfterDelay()
    {
        yield return new WaitForSeconds(feedbackDuration);

        answered = false;

        if (FeedbackBox != null)
            FeedbackBox.SetActive(false);

        if (FeedbackText != null)
            FeedbackText.text = "";
    }
}