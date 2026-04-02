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

    [Header("Botones principales del menú")]
    public Button buttonL1;
    public Button buttonL2;

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

    private void Start()
    {
        UpdateCarousel();
        UpdateRibbonText();
        UpdateMainButtons();

        if (lessonPanel != null)
            lessonPanel.SetActive(false);

        UpdatePanelButtonsText();
    }

    public void NextImage()
    {
        if (levelSprites == null || levelSprites.Length == 0)
            return;

        currentIndex = (currentIndex + 1) % levelSprites.Length;

        UpdateCarousel();
        UpdateRibbonText();

        if (lessonPanel != null && lessonPanel.activeSelf)
        {
            UpdateLessonPanelContent(1);
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
            UpdateLessonPanelContent(1);
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

    private void UpdateMainButtons()
    {
        if (buttonL1 != null)
        {
            buttonL1.gameObject.SetActive(true);
            buttonL1.interactable = true;
        }

        if (buttonL2 != null)
        {
            buttonL2.gameObject.SetActive(true);
            buttonL2.interactable = false;
        }
    }

    private void UpdatePanelButtonsText()
    {
        if (learnButtonText != null)
            learnButtonText.text = "Aprender";

        if (playButtonText != null)
            playButtonText.text = "Jugar";
    }

    public void OpenLesson1Panel()
    {
        if (lessonPanel == null) return;

        lessonPanel.SetActive(true);
        UpdateLessonPanelContent(1);
    }

    public void OpenLesson2Panel()
    {
        Debug.Log("La lección 2 aún no está disponible.");
    }

    private void UpdateLessonPanelContent(int lessonNumber)
    {
        if (panelTitleText != null)
        {
            panelTitleText.text = "Lección 1";
        }

        if (descriptionLessonText != null)
        {
            switch (currentIndex)
            {
                case 0:
                    descriptionLessonText.text = "Explora el nivel básico de matemáticas y comienza con retos sencillos.";
                    break;
                case 1:
                    descriptionLessonText.text = "Explora el nivel intermedio de matemáticas y fortalece tus habilidades.";
                    break;
                case 2:
                    descriptionLessonText.text = "Explora el nivel avanzado de matemáticas y resuelve retos más complejos.";
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
                Debug.Log("Aprender - Matemáticas Básico");
                break;
            case 1:
                Debug.Log("Aprender - Matemáticas Intermedio");
                break;
            case 2:
                Debug.Log("Aprender - Matemáticas Avanzado");
                break;
        }
    }

    public void OnPlayButton()
    {
        switch (currentIndex)
        {
            case 0:
                SceneManager.LoadScene("BasicoMAL1N1");
                break;
            case 1:
                SceneManager.LoadScene("IntermedioMAL1N1");
                break;
            case 2:
                SceneManager.LoadScene("AvanzadoMAL1N1");
                break;
        }
    }
}
