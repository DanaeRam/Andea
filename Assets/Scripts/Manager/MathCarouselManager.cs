using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MathCarouselManager : MonoBehaviour
{
    [Header("Carrusel")]
    public Image carouselImage;
    public Sprite[] levelSprites;

    [Header("Escena intermedia")]
    [SerializeField] private string introWalkSceneName = "BasicoTienda";

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

    private int currentIndex = 0;
    private int currentLesson = 0;

    private void Start()
    {
        UpdateCarousel();
        UpdateRibbonText();
        UpdateVisibleLessonButtons();
        UpdatePanelButtonsText();

        if (lessonPanel != null)
            lessonPanel.SetActive(false);

        if (playButton != null)
        {
            playButton.onClick.RemoveAllListeners();
            playButton.onClick.AddListener(PlaySelectedLesson);
        }

        if (closePanelButton != null)
        {
            closePanelButton.onClick.RemoveAllListeners();
            closePanelButton.onClick.AddListener(CloseLessonPanel);
        }
    }

    public void NextImage()
    {
        if (levelSprites == null || levelSprites.Length == 0) return;

        currentIndex = (currentIndex + 1) % levelSprites.Length;

        UpdateCarousel();
        UpdateRibbonText();
        UpdateVisibleLessonButtons();
        CloseLessonPanel();
    }

    public void PreviousImage()
    {
        if (levelSprites == null || levelSprites.Length == 0) return;

        currentIndex--;

        if (currentIndex < 0)
            currentIndex = levelSprites.Length - 1;

        UpdateCarousel();
        UpdateRibbonText();
        UpdateVisibleLessonButtons();
        CloseLessonPanel();
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
            panelTitleText.text = "Lección " + currentLesson;

        if (descriptionLessonText == null) return;

        switch (currentIndex)
        {
            case 0:
                switch (currentLesson)
                {
                    case 1:
                        descriptionLessonText.text = "Explora la lección 1 del nivel básico: Sumas.";
                        break;
                    case 2:
                        descriptionLessonText.text = "Explora la lección 2 del nivel básico: Restas.";
                        break;
                    case 3:
                        descriptionLessonText.text = "Explora la lección 3 del nivel básico: Sumas y restas.";
                        break;
                    default:
                        descriptionLessonText.text = "Lección del nivel básico.";
                        break;
                }
                break;

            case 1:
                switch (currentLesson)
                {
                    case 1:
                        descriptionLessonText.text = "Explora la lección 1 del nivel intermedio: Multiplicaciones.";
                        break;
                    case 2:
                        descriptionLessonText.text = "Explora la lección 2 del nivel intermedio: Divisiones.";
                        break;
                    default:
                        descriptionLessonText.text = "Lección del nivel intermedio.";
                        break;
                }
                break;

            case 2:
                switch (currentLesson)
                {
                    case 1:
                        descriptionLessonText.text = "Explora la lección 1 del nivel avanzado: Sumas y multiplicaciones.";
                        break;
                    case 2:
                        descriptionLessonText.text = "Explora la lección 2 del nivel avanzado: Sumas y divisiones.";
                        break;
                    case 3:
                        descriptionLessonText.text = "Explora la lección 3 del nivel avanzado: Operaciones combinadas.";
                        break;
                    default:
                        descriptionLessonText.text = "Lección del nivel avanzado.";
                        break;
                }
                break;
        }
    }

    public void CloseLessonPanel()
    {
        if (lessonPanel != null)
            lessonPanel.SetActive(false);
    }

    public void PlaySelectedLesson()
    {
        if (currentLesson <= 0)
        {
            Debug.LogError("No hay lección seleccionada.");
            return;
        }

        string targetSceneName = GetLessonSceneName();

        if (string.IsNullOrEmpty(targetSceneName))
        {
            Debug.LogError("No se encontró escena para esta lección.");
            return;
        }

        MathSceneTransitionData.currentRound = 0;
        MathSceneTransitionData.maxRounds = 6;
        MathSceneTransitionData.exitMode = false;

        MathSceneTransitionData.targetSceneName = targetSceneName;

        SceneManager.LoadScene(introWalkSceneName);
    }

    private string GetLessonSceneName()
    {
        switch (currentIndex)
        {
            case 0: // Básico
                switch (currentLesson)
                {
                    case 1: return "FormulasBasico";
                    case 2: return "BasicoRestas";
                    case 3: return "SumayResta";
                }
                break;

            case 1: // Intermedio
                switch (currentLesson)
                {
                    case 1: return "Multiplicacion";
                    case 2: return "Division";
                }
                break;

            case 2: // Avanzado
                switch (currentLesson)
                {
                    case 1: return "SumayMultiplicacion";
                    case 2: return "SumayDivision";
                    case 3: return "OperacionesCombinadas";
                }
                break;
        }

        return "";
    }
}