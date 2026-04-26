using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MathCarouselManager : MonoBehaviour
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


}