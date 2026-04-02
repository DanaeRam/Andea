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

    [Header("Escena de aprender")]
    public string learnSceneName = "MathLearnScene";

    [Header("Escenas de jugar por nivel y lección")]
    public string basicLesson1Scene = "BasicoL1";
    public string basicLesson2Scene = "BasicoL2";

    public string intermediateLesson1Scene = "IntermedioL1";
    public string intermediateLesson2Scene = "IntermedioL2";

    public string advancedLesson1Scene = "AvanzadoL1";
    public string advancedLesson2Scene = "AvanzadoL2";

    private int currentIndex = 0;   // 0 = Básico, 1 = Intermedio, 2 = Avanzado
    private int selectedLesson = 1; // 1 o 2

    private void Start()
    {
        UpdateCarousel();
        UpdateRibbonText();
        UpdateMainButtons();

        if (lessonPanel != null)
            lessonPanel.SetActive(false);

        UpdatePanelButtonsText();

        if (closePanelButton != null)
            closePanelButton.onClick.AddListener(CloseLessonPanel);

        if (learnButton != null)
            learnButton.onClick.AddListener(OnLearnButton);

        if (playButton != null)
            playButton.onClick.AddListener(OnPlayButton);

        if (buttonL1 != null)
            buttonL1.onClick.AddListener(OpenLesson1Panel);

        if (buttonL2 != null)
            buttonL2.onClick.AddListener(OpenLesson2Panel);
    }

    public void NextImage()
    {
        if (levelSprites == null || levelSprites.Length == 0)
            return;

        currentIndex = (currentIndex + 1) % levelSprites.Length;

        UpdateCarousel();
        UpdateRibbonText();
        UpdateMainButtons();

        if (lessonPanel != null && lessonPanel.activeSelf)
            UpdateLessonPanelContent(selectedLesson);
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
        UpdateMainButtons();

        if (lessonPanel != null && lessonPanel.activeSelf)
            UpdateLessonPanelContent(selectedLesson);
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
            buttonL2.interactable = true;
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

        selectedLesson = 1;
        lessonPanel.SetActive(true);
        UpdateLessonPanelContent(selectedLesson);
    }

    public void OpenLesson2Panel()
    {
        if (lessonPanel == null) return;

        selectedLesson = 2;
        lessonPanel.SetActive(true);
        UpdateLessonPanelContent(selectedLesson);
    }

    private void UpdateLessonPanelContent(int lessonNumber)
    {
        if (panelTitleText != null)
            panelTitleText.text = "Lección " + lessonNumber;

        if (descriptionLessonText != null)
        {
            switch (currentIndex)
            {
                case 0:
                    descriptionLessonText.text = lessonNumber == 1
                        ? "Lección 1 del nivel básico de matemáticas."
                        : "Lección 2 del nivel básico de matemáticas.";
                    break;

                case 1:
                    descriptionLessonText.text = lessonNumber == 1
                        ? "Lección 1 del nivel intermedio de matemáticas."
                        : "Lección 2 del nivel intermedio de matemáticas.";
                    break;

                case 2:
                    descriptionLessonText.text = lessonNumber == 1
                        ? "Lección 1 del nivel avanzado de matemáticas."
                        : "Lección 2 del nivel avanzado de matemáticas.";
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
        if (!string.IsNullOrEmpty(learnSceneName))
        {
            SceneManager.LoadScene(learnSceneName);
        }
        else
        {
            Debug.LogWarning("No se asignó la escena de Learn.");
        }
    }

    public void OnPlayButton()
    {
        string sceneToLoad = GetPlaySceneName(currentIndex, selectedLesson);

        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogWarning("No se encontró una escena válida para Play.");
        }
    }

    private string GetPlaySceneName(int levelIndex, int lessonNumber)
    {
        switch (levelIndex)
        {
            case 0:
                return lessonNumber == 1 ? basicLesson1Scene : basicLesson2Scene;

            case 1:
                return lessonNumber == 1 ? intermediateLesson1Scene : intermediateLesson2Scene;

            case 2:
                return lessonNumber == 1 ? advancedLesson1Scene : advancedLesson2Scene;

            default:
                return "";
        }
    }
}