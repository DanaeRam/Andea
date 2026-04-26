using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LectoCarouselManager : MonoBehaviour
{
    [Header("Carrusel")]
    public Image carouselImage;
    public Sprite[] levelSprites; // 0 = Básico, 1 = Intermedio, 2 = Avanzado

    [Header("Ribbon superior")]
    public TextMeshProUGUI ribbonText;

    [Header("Contenedores de botones por nivel")]
    public GameObject botonesBasico;
    public GameObject botonesIntermedio;
    public GameObject botonesAvanzado;

    [Header("Panel de lección")]
    public GameObject lessonPanel;
    public TextMeshProUGUI panelTitleText;
    public TextMeshProUGUI descriptionLessonText;

    [Header("Botones dentro del panel")]
    public Button closePanelButton;
    public Button learnButton;
    public Button playButton;
    public TextMeshProUGUI learnButtonText;
    public TextMeshProUGUI playButtonText;

    private int currentIndex = 0;   // 0 = Básico, 1 = Intermedio, 2 = Avanzado
    private int currentLesson = 0;  // Lección seleccionada dentro del nivel actual

    private void Start()
    {
        UpdateCarousel();
        UpdateRibbonText();
        UpdateVisibleLessonButtons();
        UpdatePanelButtonsText();

        if (lessonPanel != null)
            lessonPanel.SetActive(false);
    }

    public void NextImage()
    {
        if (levelSprites == null || levelSprites.Length == 0)
            return;

        currentIndex = (currentIndex + 1) % levelSprites.Length;

        UpdateCarousel();
        UpdateRibbonText();
        UpdateVisibleLessonButtons();

        if (lessonPanel != null && lessonPanel.activeSelf)
        {
            lessonPanel.SetActive(false);
        }
    }

    public void PreviousImage()
    {
        if (levelSprites == null || levelSprites.Length == 0)
            return;

        currentIndex--;

        if (currentIndex < 0)
            currentIndex = levelSprites.Length - 1;

        UpdateCarousel();
        UpdateRibbonText();
        UpdateVisibleLessonButtons();

        if (lessonPanel != null && lessonPanel.activeSelf)
        {
            lessonPanel.SetActive(false);
        }
    }

    private void UpdateCarousel()
    {
        if (carouselImage != null && levelSprites != null && levelSprites.Length > 0)
        {
            carouselImage.sprite = levelSprites[currentIndex];
            carouselImage.preserveAspect = true;
        }
    }

    private void UpdateRibbonText()
    {
        if (ribbonText == null) return;

        switch (currentIndex)
        {
            case 0:
                ribbonText.text = "Nivel Básico";
                break;
            case 1:
                ribbonText.text = "Nivel Intermedio";
                break;
            case 2:
                ribbonText.text = "Nivel Avanzado";
                break;
        }
    }

    private void UpdateVisibleLessonButtons()
    {
        if (botonesBasico != null)
            botonesBasico.SetActive(currentIndex == 0);

        if (botonesIntermedio != null)
            botonesIntermedio.SetActive(currentIndex == 1);

        if (botonesAvanzado != null)
            botonesAvanzado.SetActive(currentIndex == 2);
    }

    private void UpdatePanelButtonsText()
    {
        if (learnButtonText != null)
            learnButtonText.text = "Aprender";

        if (playButtonText != null)
            playButtonText.text = "Jugar";
    }

    public void OpenLessonPanel(int lessonNumber)
    {
        currentLesson = lessonNumber;

        if (lessonPanel != null)
            lessonPanel.SetActive(true);

        UpdateLessonPanelContent();
    }

    private void UpdateLessonPanelContent()
    {
        if (panelTitleText != null)
        {
            panelTitleText.text = "Lección " + currentLesson;
        }

        if (descriptionLessonText != null)
        {
            switch (currentIndex)
            {
                case 0: // Básico
                    switch (currentLesson)
                    {
                        case 1:
                            descriptionLessonText.text = "Explora la lección 1 del nivel básico y comienza tu aventura de lecto-escritura.";
                            break;
                        case 2:
                            descriptionLessonText.text = "Explora la lección 2 del nivel básico y sigue practicando habilidades iniciales.";
                            break;
                        case 3:
                            descriptionLessonText.text = "Explora la lección 3 del nivel básico y refuerza lo aprendido.";
                            break;
                        default:
                            descriptionLessonText.text = "Lección del nivel básico.";
                            break;
                    }
                    break;

                case 1: // Intermedio
                    switch (currentLesson)
                    {
                        case 1:
                            descriptionLessonText.text = "Explora la lección 1 del nivel intermedio: Identificar sustantivos, verbos y adjetivos.";
                            break;
                        case 2:
                            descriptionLessonText.text = "Explora la lección 2 del nivel intermedio: Sinónimos y Antónimos.";
                            break;
                        default:
                            descriptionLessonText.text = "Lección del nivel intermedio.";
                            break;
                    }
                    break;

                case 2: // Avanzado
                    switch (currentLesson)
                    {
                        case 1:
                            descriptionLessonText.text = "Explora la lección 1 del nivel avanzado: Uso correcto de signos de puntuación.";
                            break;
                        case 2:
                            descriptionLessonText.text = "Explora la lección 2 del nivel avanzado: Comprensión Lectora.";
                            break;
                        case 3:
                            descriptionLessonText.text = "Explora la lección 3 del nivel avanzado: Identificar la idea principal.";
                            break;
                        case 4:
                            descriptionLessonText.text = "Explora la lección 4 del nivel avanzado: Inferencias de un texto.";
                            break;
                        default:
                            descriptionLessonText.text = "Lección del nivel avanzado.";
                            break;
                    }
                    break;
                            }
        }
    }

    public void CloseLessonPanel()
    {
        if (lessonPanel != null)
            lessonPanel.SetActive(false);
    }

    public void OnLearnButton()
    {
        switch (currentIndex)
        {
            case 0:
                switch (currentLesson)
                {
                    case 1:
                        Debug.Log("Aprender - Básico Lección 1");
                        break;
                    case 2:
                        Debug.Log("Aprender - Básico Lección 2");
                        break;
                    case 3:
                        Debug.Log("Aprender - Básico Lección 3");
                        break;
                }
                break;

            case 1:
                switch (currentLesson)
                {
                    case 1:
                        Debug.Log("Aprender - Intermedio Lección 1");
                        break;
                    case 2:
                        Debug.Log("Aprender - Intermedio Lección 2");
                        break;
                }
                break;

            case 2:
                switch (currentLesson)
                {
                    case 1:
                        Debug.Log("Aprender - Avanzado Lección 1");
                        break;
                    case 2:
                        Debug.Log("Aprender - Avanzado Lección 2");
                        break;
                    case 3:
                        Debug.Log("Aprender - Avanzado Lección 3");
                        break;
                    case 4:
                        Debug.Log("Aprender - Avanzado Lección 4");
                        break;
                }
                break;
        }
    }

 public void OnPlayButton()
{
    switch (currentIndex)
    {
        case 0:
            // Básico
            switch (currentLesson)
            {
                case 1:
                    // Lección 1: Identificar sustantivos, verbos y adjetivos
                    if (LectoGameSessionManager.Instance != null)
                    {
                        LectoGameSessionManager.Instance.StartLessonSession(
                            "Basico",
                            "Identificar sustantivos, verbos y adjetivos1"
                        );
                    }
                    break;

                case 2:
                    // Lección 2: Sinónimos y Antónimos
                    if (LectoGameSessionManager.Instance != null)
                    {
                        LectoGameSessionManager.Instance.StartLessonSession(
                            "Basico",
                            "Identificar sustantivos, verbos y adjetivos2"
                        );
                    }
                    break;

                case 3:
                    // Lección 3: Identificar la idea principal
                    if (LectoGameSessionManager.Instance != null)
                    {
                        LectoGameSessionManager.Instance.StartLessonSession(
                            "Basico",
                            "Identificar sustantivos, verbos y adjetivos3"
                        );
                    }
                    break;

                default:
                    Debug.LogError("Lección no configurada para el nivel basico.");
                    break;
            }
            break;

        case 1:
            // Intermedio
            switch (currentLesson)
            {
                case 1:
                    // Lección 1: Identificar sustantivos, verbos y adjetivos
                    if (LectoGameSessionManager.Instance != null)
                    {
                        LectoGameSessionManager.Instance.StartLessonSession(
                            "Intermedio",
                            "Identificar sustantivos, verbos y adjetivos"
                        );
                    }
                    break;

                case 2:
                    // Lección 2: Sinónimos y Antónimos
                    if (LectoGameSessionManager.Instance != null)
                    {
                        LectoGameSessionManager.Instance.StartLessonSession(
                            "Intermedio",
                            "Sinónimos y Antónimos"
                        );
                    }
                    break;

                default:
                    Debug.LogError("Lección no configurada para el nivel intermedio.");
                    break;
            }
            break;

        case 2:
            // Avanzado
            switch (currentLesson)
            {
                case 1:
                    // Lección 1: Uso correcto de signos de puntuación
                    if (LectoGameSessionManager.Instance != null)
                    {
                        LectoGameSessionManager.Instance.StartLessonSession(
                            "Avanzado",
                            "Uso correcto de signos de puntuación"
                        );
                    }
                    break;

                case 2:
                    // Lección 2: Comprensión Lectora
                    if (LectoGameSessionManager.Instance != null)
                    {
                        LectoGameSessionManager.Instance.StartLessonSession(
                            "Avanzado",
                            "Comprensión Lectora"
                        );
                    }
                    break;

                case 3:
                    // Lección 3: Identificar la idea principal
                    if (LectoGameSessionManager.Instance != null)
                    {
                        LectoGameSessionManager.Instance.StartLessonSession(
                            "Avanzado",
                            "Identificar la idea principal"
                        );
                    }
                    break;

                case 4:
                    // Lección 4: Inferencias de un texto
                    if (LectoGameSessionManager.Instance != null)
                    {
                        LectoGameSessionManager.Instance.StartLessonSession(
                            "Avanzado",
                            "Inferencias de un texto"
                        );
                    }
                    break;

                default:
                    Debug.LogError("Lección no configurada para el nivel avanzado.");
                    break;
            }
            break;
    }
}
}