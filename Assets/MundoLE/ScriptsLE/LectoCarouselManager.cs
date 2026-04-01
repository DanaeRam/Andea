using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LectoCarouselManager : MonoBehaviour
{
    [Header("Carrusel")]
    public Image carouselImage;
    public Sprite[] levelSprites; // 0 = Basico, 1 = Intermedio, 2 = Avanzado

    [Header("Ribbon")]
    public TextMeshProUGUI ribbonText;

    [Header("Botones")]
    public Button buttonL1;
    public Button buttonL2;

    private int currentIndex = 0;

    private void Start()
    {
        UpdateCarousel();
        UpdateRibbonText();
        UpdateButtons();
    }

    public void NextImage()
    {
        if (levelSprites == null || levelSprites.Length == 0)
            return;

        currentIndex = (currentIndex + 1) % levelSprites.Length;
        UpdateCarousel();
        UpdateRibbonText();
        UpdateButtons();
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
        UpdateButtons();
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

    private void UpdateButtons()
    {
        // Por ahora solo existe una lección, así que:
        // ButtonL1 activo
        // ButtonL2 oculto
        if (buttonL1 != null)
            buttonL1.gameObject.SetActive(true);

        if (buttonL2 != null)
            buttonL2.gameObject.SetActive(false);
    }

    public void OpenLesson1()
    {
        switch (currentIndex)
        {
            case 0:
                SceneManager.LoadScene("BasicoL1N1");
                break;

            case 1:
                SceneManager.LoadScene("IntermedioL1N1");
                break;

            case 2:
                SceneManager.LoadScene("AvanzadoL1N1");
                break;
        }
    }

    public void OpenLesson2()
    {
        // Lo dejamos preparado para después.
        Debug.Log("La Lección 2 aún no está disponible.");
    }
}