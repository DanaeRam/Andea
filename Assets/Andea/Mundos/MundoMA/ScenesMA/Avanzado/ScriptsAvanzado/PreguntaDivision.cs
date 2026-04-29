using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PreguntaDivision : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("UI")]
    [SerializeField] private GameObject quizPanel;
    [SerializeField] private TMP_Text questionText;
    [SerializeField] private TMP_InputField answerInput;
    [SerializeField] private Button submitButton;
    [SerializeField] private TMP_Text feedbackText;

    [Header("Camera")]
    [SerializeField] private CameraZoom2D cameraZoom;
    [SerializeField] private float quizZoom = 3f;
    [SerializeField] private float fallbackNormalZoom = 5f;

    [Header("Time")]
    [SerializeField] private float slowMotionScale = 0.25f;

    private int correctAnswer;
    private PlayerControlLock currentPlayerControl;
    private float previousZoom = 5f;
    private bool quizActive = false;
    private bool answerRegistered = false;

    private void Start()
    {
        if (quizPanel != null)
            quizPanel.SetActive(false);

        if (submitButton != null)
        {
            submitButton.onClick.RemoveAllListeners();
            submitButton.onClick.AddListener(CheckAnswer);
        }
        else
        {
            Debug.LogError("MathQuizManager: falta asignar Submit Button");
        }

        if (quizPanel == null) Debug.LogError("MathQuizManager: falta asignar Quiz Panel");
        if (questionText == null) Debug.LogError("MathQuizManager: falta asignar Question Text");
        if (answerInput == null) Debug.LogError("MathQuizManager: falta asignar Answer Input");
        if (feedbackText == null) Debug.LogError("MathQuizManager: falta asignar Feedback Text");
        if (cameraZoom == null) Debug.LogWarning("MathQuizManager: falta asignar Camera Zoom");
    }

    public void StartQuiz(GameObject player)
    {
        Debug.Log("MathQuizManager -> StartQuiz con: " + player.name);

        if (quizActive) return;

        currentPlayerControl = player.GetComponent<PlayerControlLock>();

        if (currentPlayerControl == null)
        {
            Debug.LogError("MathQuizManager: el jugador no tiene PlayerControlLock");
            return;
        }

        quizActive = true;
        answerRegistered = false;

        SaveCurrentZoom();
        currentPlayerControl.LockForQuiz();
        ActivateQuizMode();
        GenerateQuestion();

        if (quizPanel != null)
            quizPanel.SetActive(true);

        if (feedbackText != null)
            feedbackText.text = "";

        if (answerInput != null)
        {
            answerInput.text = "";
            answerInput.ActivateInputField();
        }
    }

    private void SaveCurrentZoom()
    {
        if (cameraZoom != null && cameraZoom.cam != null)
            previousZoom = cameraZoom.cam.orthographicSize;
        else
            previousZoom = fallbackNormalZoom;
    }

    private void GenerateQuestion()
    {
         // Generamos el resultado primero (para asegurar división exacta)
        int resultado = Random.Range(1, 11); // respuestas de 1 a 10

        int divisor = Random.Range(1, 11); // divisor de 1 a 10

        int dividendo = resultado * divisor; // asegura división exacta

        correctAnswer = resultado;

        if (questionText != null)
            questionText.text = $"¿Cuánto es {dividendo} ÷ {divisor}?";

        Debug.Log("Pregunta generada. Respuesta correcta: " + correctAnswer);
    }

    public void CheckAnswer()
    {
        if (!quizActive) return;
        if (answerInput == null) return;

        if (!int.TryParse(answerInput.text, out int playerAnswer))
        {
            if (feedbackText != null)
            {
                feedbackText.text = "Respuesta no válida";
                feedbackText.color = Color.yellow;
            }

            answerInput.ActivateInputField();
            return;
        }

        if (playerAnswer == correctAnswer)
        {
            RegisterAnswer(true);

            if (feedbackText != null)
            {
                feedbackText.text = "Correcto";
                feedbackText.color = Color.green;
            }

            Invoke(nameof(EndQuizSuccess), 0.5f);
        }
        else
        {
            RegisterAnswer(false);

            if (feedbackText != null)
            {
                feedbackText.text = "Falso";
                feedbackText.color = Color.red;
            }

            if (currentPlayerControl != null)
                currentPlayerControl.LockFailedAnswer();

            answerInput.text = "";
            answerInput.ActivateInputField();
        }
    }

    private void RegisterAnswer(bool isCorrect)
    {
        if (answerRegistered) return;

        answerRegistered = true;

        if (isCorrect)
        {
            MathRewardSessionData.RegisterCorrect();
        }
        else
        {
            MathRewardSessionData.RegisterWrong();
        }
    }

    private void EndQuizSuccess()
    {
        Debug.Log("Respuesta correcta. Cerrando quiz.");

        quizActive = false;

        if (quizPanel != null)
            quizPanel.SetActive(false);

        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;

        if (cameraZoom != null)
            cameraZoom.SetZoom(previousZoom);

        if (currentPlayerControl != null)
        {
            currentPlayerControl.UnlockAfterQuiz();
            Debug.Log("Jugador desbloqueado");
        }
        else
        {
            Debug.LogError("currentPlayerControl es null al terminar el quiz");
        }
    }

    public void ForceCloseQuiz()
    {
        quizActive = false;

        if (quizPanel != null)
            quizPanel.SetActive(false);

        Time.timeScale = 1f;
        Time.fixedDeltaTime = 0.02f;

        if (cameraZoom != null)
            cameraZoom.SetZoom(previousZoom);

        if (currentPlayerControl != null)
            currentPlayerControl.ForceUnlock();

        Debug.Log("Quiz cerrado forzadamente");
    }

    private void ActivateQuizMode()
    {
        Time.timeScale = slowMotionScale;
        Time.fixedDeltaTime = 0.02f * Time.timeScale;

        if (cameraZoom != null)
            cameraZoom.SetZoom(quizZoom);
    }
}
