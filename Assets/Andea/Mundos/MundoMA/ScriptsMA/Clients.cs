using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Clients : MonoBehaviour
{
    [Header("Entrada")]
    public Vector3 enterStartPosition = new Vector3(-7f, -1.6f, 0f);

    [Header("Centro")]
    public Vector3 centerPosition = new Vector3(0f, -1.6f, 0f);

    [Header("Salida")]
    public Vector3 exitTargetPosition = new Vector3(7f, -1.6f, 0f);

    [Header("Movimiento")]
    public float speed = 2f;
    public float waitBeforeLesson = 4f;
    public float waitBeforeExit = 1f;

    [Header("Escena de regreso")]
    public string carouselSceneName = "CarouselMA";

    private Animator animator;
    private bool moving;
    private bool finished;

    void Start()
    {
        animator = GetComponent<Animator>();

        if (MathSceneTransitionData.exitMode)
        {
            transform.position = centerPosition;
            animator.SetBool("isWalking", false);

            StartCoroutine(StartExitFlow());
        }
        else
        {
            transform.position = enterStartPosition;
            FaceDirection(centerPosition);

            moving = true;
            animator.SetBool("isWalking", true);
        }
    }

    void Update()
    {
        if (!moving || finished) return;

        if (MathSceneTransitionData.exitMode)
        {
            MoveTo(exitTargetPosition, true);
        }
        else
        {
            MoveTo(centerPosition, false);
        }
    }

    void MoveTo(Vector3 destination, bool returnToCarousel)
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            destination,
            speed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, destination) < 0.01f)
        {
            transform.position = destination;
            moving = false;
            finished = true;

            animator.SetBool("isWalking", false);

        if (returnToCarousel)
        {
            MathSceneTransitionData.exitMode = false;

            if (MathSceneTransitionData.currentRound < MathSceneTransitionData.maxRounds)
            {
                SceneManager.LoadScene("BasicoTienda");
            }
            else
            {
                MathSceneTransitionData.currentRound = 0;
                SceneManager.LoadScene(carouselSceneName);
            }
        }
            else
            {
                StartCoroutine(OpenLessonAfterDelay());
            }
        }
    }

    IEnumerator StartExitFlow()
    {
        yield return new WaitForSeconds(waitBeforeExit);

        FaceDirection(exitTargetPosition);

        moving = true;
        finished = false;
        animator.SetBool("isWalking", true);
    }

    IEnumerator OpenLessonAfterDelay()
    {
        yield return new WaitForSeconds(waitBeforeLesson);

        if (string.IsNullOrEmpty(MathSceneTransitionData.targetSceneName))
        {
            Debug.LogError("No se asignó ninguna escena destino.");
            yield break;
        }

        SceneManager.LoadScene(MathSceneTransitionData.targetSceneName);
    }

    void FaceDirection(Vector3 destination)
    {
        if (destination.x > transform.position.x)
        {
            transform.localScale = new Vector3(
                Mathf.Abs(transform.localScale.x),
                transform.localScale.y,
                transform.localScale.z
            );
        }
        else
        {
            transform.localScale = new Vector3(
                -Mathf.Abs(transform.localScale.x),
                transform.localScale.y,
                transform.localScale.z
            );
        }
    }
}