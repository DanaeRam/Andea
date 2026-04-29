using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

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

    private Animator animator;
    private bool moving;
    private bool finished;
    private bool savingProgress;

    private void Start()
    {
        animator = GetComponent<Animator>();

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
        if (!moving || finished)
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

        if (string.IsNullOrEmpty(worldCode))
            worldCode = "MA";

        worldCode = worldCode.Trim().ToUpper();

        Debug.Log(
            "MA lección completada. Intentando guardar progreso -> mundo: " +
            worldCode +
            " | leccionId: " +
            lessonId
        );

        if (syncLessonProgress)
        {
            if (PlayerLessonsApi.Instance == null)
            {
                Debug.LogWarning("No existe PlayerLessonsApi. No se pudo guardar progreso MA.");
            }
            else if (string.IsNullOrEmpty(lessonId))
            {
                Debug.LogWarning("No hay CurrentLessonId guardado. No se pudo guardar progreso MA.");
            }
            else
            {
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
        }

        MathSceneTransitionData.currentRound = 0;
        MathSceneTransitionData.exitMode = false;
        MathSceneTransitionData.targetSceneName = "";

        savingProgress = false;

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