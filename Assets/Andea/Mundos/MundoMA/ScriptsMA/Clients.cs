using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using TMPro;
using UnityEngine.UI;

public class Clients : MonoBehaviour
{
    [Header("Entrada")]
    public Vector3 enterStartPosition = new Vector3(-8f, -3f, 0f);

    [Header("Centro")]
    public Vector3 centerPosition = new Vector3(0f, -3f, 0f);

    [Header("Salida")]
    public Vector3 exitTargetPosition = new Vector3(8f, -3f, 0f);

    [Header("Movimiento")]
    public float speed = 2f;
    public float waitBeforeLesson = 4f;
    public float waitBeforeExit = 1f;

    [Header("Escena de regreso")]
    public string carouselSceneName = "CarouselMA";

    [Header("Guardar progreso")]
    public bool syncLessonProgress = true;

    [Header("Recompensas")]
    public bool giveReward = true;

    [Header("Panel resultados MA")]
    public GameObject resultsPanel;
    public TextMeshProUGUI resultsTitleText;
    public TextMeshProUGUI correctText;
    public TextMeshProUGUI wrongText;
    public TextMeshProUGUI questionPointsText;
    public TextMeshProUGUI bonusText;
    public TextMeshProUGUI totalPointsText;
    public TextMeshProUGUI runesText;
    public Button continueButton;

    private Animator animator;
    private bool moving;
    private bool finished;
    private bool savingProgress;
    private bool showingResults;

    private void Start()
    {
        animator = GetComponent<Animator>();

        if (resultsPanel != null)
            resultsPanel.SetActive(false);

        if (continueButton != null)
        {
            continueButton.onClick.RemoveAllListeners();
            continueButton.onClick.AddListener(ContinueToCarousel);
        }

        if (MathSceneTransitionData.exitMode)
        {
            transform.position = centerPosition;

            if (animator != null)
                animator.SetBool("isWalking", false);

            StartCoroutine(StartExitFlow());
        }
        else
        {
            transform.position = enterStartPosition;
            FaceDirection(centerPosition);

            moving = true;

            if (animator != null)
                animator.SetBool("isWalking", true);
        }
    }

    private void Update()
    {
        if (!moving || finished || showingResults)
            return;

        if (MathSceneTransitionData.exitMode)
            MoveTo(exitTargetPosition, true);
        else
            MoveTo(centerPosition, false);
    }

    private void MoveTo(Vector3 destination, bool returnToCarousel)
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            destination,
            speed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, destination) >= 0.01f)
            return;

        transform.position = destination;
        moving = false;
        finished = true;

        if (animator != null)
            animator.SetBool("isWalking", false);

        if (returnToCarousel)
        {
            MathSceneTransitionData.exitMode = false;

            if (MathSceneTransitionData.currentRound < MathSceneTransitionData.maxRounds)
            {
                Debug.Log(
                    "MA cliente terminado. Ronda actual: " +
                    MathSceneTransitionData.currentRound +
                    " / " +
                    MathSceneTransitionData.maxRounds
                );

                SceneManager.LoadScene("BasicoTienda");
            }
            else
            {
                StartCoroutine(FinalizarLeccionMA());
            }
        }
        else
        {
            StartCoroutine(OpenLessonAfterDelay());
        }
    }

    private IEnumerator StartExitFlow()
    {
        yield return new WaitForSeconds(waitBeforeExit);

        FaceDirection(exitTargetPosition);

        moving = true;
        finished = false;

        if (animator != null)
            animator.SetBool("isWalking", true);
    }

    private IEnumerator OpenLessonAfterDelay()
    {
        yield return new WaitForSeconds(waitBeforeLesson);

        if (string.IsNullOrEmpty(MathSceneTransitionData.targetSceneName))
        {
            Debug.LogError("No se asignó ninguna escena destino.");
            yield break;
        }

        Debug.Log("MA abriendo escena de preguntas: " + MathSceneTransitionData.targetSceneName);

        SceneManager.LoadScene(MathSceneTransitionData.targetSceneName);
    }

    private IEnumerator FinalizarLeccionMA()
    {
        if (savingProgress)
            yield break;

        savingProgress = true;

        string worldCode = PlayerPrefs.GetString("CurrentWorldCode", "MA");
        string lessonId = PlayerPrefs.GetString("CurrentLessonId", "");
        bool yaEstabaCompletada = PlayerPrefs.GetInt("CurrentLessonAlreadyCompleted", 0) == 1;

        if (string.IsNullOrEmpty(worldCode))
            worldCode = "MA";

        worldCode = worldCode.Trim().ToUpper();

        Debug.Log(
            "MA lección completada. Intentando guardar progreso -> mundo: " +
            worldCode +
            " | leccionId: " +
            lessonId +
            " | yaEstabaCompletada: " +
            yaEstabaCompletada
        );

        if (syncLessonProgress)
        {
            yield return StartCoroutine(GuardarProgresoLeccion(worldCode, lessonId));
        }

        int totalPuntosGanados = 0;
        int runasGeneradas = 0;

        if (giveReward && !yaEstabaCompletada)
        {
            yield return StartCoroutine(GuardarRecompensaMA(
                lessonId,
                (puntos, runas) =>
                {
                    totalPuntosGanados = puntos;
                    runasGeneradas = runas;
                }
            ));
        }
        else
        {
            Debug.Log("La lección MA ya estaba completada. No se vuelven a dar puntos.");
        }

        MathSceneTransitionData.currentRound = 0;
        MathSceneTransitionData.exitMode = false;
        MathSceneTransitionData.targetSceneName = "";

        savingProgress = false;

        MostrarPanelResultados(yaEstabaCompletada, totalPuntosGanados, runasGeneradas);
    }

    private IEnumerator GuardarProgresoLeccion(string worldCode, string lessonId)
    {
        if (PlayerLessonsApi.Instance == null)
        {
            Debug.LogWarning("No existe PlayerLessonsApi. No se pudo guardar progreso MA.");
            yield break;
        }

        if (string.IsNullOrEmpty(lessonId))
        {
            Debug.LogWarning("No hay CurrentLessonId guardado. No se pudo guardar progreso MA.");
            yield break;
        }

        yield return StartCoroutine(
            PlayerLessonsApi.Instance.CompletarLeccion(
                worldCode,
                lessonId,
                onSuccess: (data) =>
                {
                    Debug.Log("Progreso MA guardado en base de datos: " + data.leccion_id);
                },
                onError: (error) =>
                {
                    Debug.LogError("Error al guardar progreso MA: " + error);
                }
            )
        );
    }

    private IEnumerator GuardarRecompensaMA(string lessonId, System.Action<int, int> onFinished)
    {
        int totalPuntos = MathRewardSessionData.TotalPoints;

        if (totalPuntos <= 0)
        {
            onFinished?.Invoke(0, 0);
            yield break;
        }

        if (PlayerProgressApi.Instance == null)
        {
            Debug.LogWarning("No existe PlayerProgressApi. No se pudieron guardar puntos MA.");
            onFinished?.Invoke(totalPuntos, 0);
            yield break;
        }

        string motivo = "Lección MA completada: " + lessonId;

        yield return StartCoroutine(
            PlayerProgressApi.Instance.AgregarPuntos(
                totalPuntos,
                motivo,
                onSuccess: (data) =>
                {
                    Debug.Log(
                        "Puntos MA guardados. Puntos ganados: " +
                        totalPuntos +
                        " | Runas generadas: " +
                        data.runas_generadas
                    );

                    onFinished?.Invoke(totalPuntos, data.runas_generadas);
                },
                onError: (error) =>
                {
                    Debug.LogError("Error al guardar puntos MA: " + error);
                    onFinished?.Invoke(totalPuntos, 0);
                }
            )
        );
    }

    private void MostrarPanelResultados(bool yaEstabaCompletada, int totalPuntosGanados, int runasGeneradas)
    {
        showingResults = true;

        if (resultsPanel != null)
            resultsPanel.SetActive(true);

        if (resultsTitleText != null)
            resultsTitleText.text = yaEstabaCompletada
                ? "Lección completada de nuevo"
                : "¡Lección completada!";

        if (correctText != null)
            correctText.text = "Respuestas correctas: " + MathRewardSessionData.correctAnswers;

        if (wrongText != null)
            wrongText.text = "Respuestas incorrectas: " + MathRewardSessionData.wrongAnswers;

        if (questionPointsText != null)
            questionPointsText.text =
                "Puntos por respuestas: " +
                MathRewardSessionData.TotalQuestionPoints;

        if (bonusText != null)
            bonusText.text = yaEstabaCompletada
                ? "Bonus por completar: 0"
                : "Bonus por completar: " + MathRewardSessionData.completionBonus;

        if (totalPointsText != null)
        {
            if (yaEstabaCompletada)
                totalPointsText.text = "Total ganado: 0\nEsta lección ya estaba completada.";
            else
                totalPointsText.text = "Total ganado: " + totalPuntosGanados;
        }

        if (runesText != null)
            runesText.text = "Runas ganadas: " + runasGeneradas;
    }

    public void ContinueToCarousel()
    {
        SceneManager.LoadScene(carouselSceneName);
    }

    private void FaceDirection(Vector3 destination)
    {
        if (destination.x > transform.position.x)
        {
            transform.localScale = new Vector3(
                -Mathf.Abs(transform.localScale.x),
                transform.localScale.y,
                transform.localScale.z
            );
        }
        else
        {
            transform.localScale = new Vector3(
                Mathf.Abs(transform.localScale.x),
                transform.localScale.y,
                transform.localScale.z
            );
        }
    }
}