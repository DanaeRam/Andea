using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GalleryProgressManager : MonoBehaviour
{
    [Header("Datos del puzzle actual")]
    public string currentPuzzleKey;   // Ej: SMBasicoL1P1
    public string nextSceneName;      // Ej: SMBasicoL1P2
    public bool isLastPuzzle = false;

    [Header("Panel principal")]
    public GameObject progressPanel;

    [Header("Fondo difuminado")]
    public GameObject backgroundProgressPanel;

    [Header("Botones")]
    public Button progressButton;
    public Button closeButton;
    public Button bottomButton;

    [Header("Textos")]
    public TextMeshProUGUI panelTitleText;
    public TextMeshProUGUI bottomButtonText;

    [Header("Miniaturas")]
    public Image painting1;
    public Image painting2;
    public Image painting3;
    public Image painting4;

    [Header("Claves de progreso")]
    public string puzzle1Key = "SMBasicoL1P1";
    public string puzzle2Key = "SMBasicoL1P2";
    public string puzzle3Key = "SMBasicoL1P3";
    public string puzzle4Key = "SMBasicoL1P4";

    private void Start()
    {
        if (progressPanel != null)
            progressPanel.SetActive(false);

        if (backgroundProgressPanel != null)
            backgroundProgressPanel.SetActive(false);

        RefreshPanel();
        RegisterButtons();
    }

    private void RegisterButtons()
    {
        if (progressButton != null)
        {
            progressButton.onClick.RemoveAllListeners();
            progressButton.onClick.AddListener(OpenPanel);
        }

        if (closeButton != null)
        {
            closeButton.onClick.RemoveAllListeners();
            closeButton.onClick.AddListener(ClosePanel);
        }

        if (bottomButton != null)
        {
            bottomButton.onClick.RemoveAllListeners();
            bottomButton.onClick.AddListener(OnBottomButton);
        }
    }

    public void OpenPanel()
    {
        RefreshPanel();

        if (backgroundProgressPanel != null)
        {
            backgroundProgressPanel.SetActive(true);
            backgroundProgressPanel.transform.SetAsLastSibling();
        }

        if (progressPanel != null)
        {
            progressPanel.SetActive(true);
            progressPanel.transform.SetAsLastSibling();
        }
    }

    public void ClosePanel()
    {
        if (progressPanel != null)
            progressPanel.SetActive(false);

        if (backgroundProgressPanel != null)
            backgroundProgressPanel.SetActive(false);
    }

    public void MarkCurrentPuzzleCompleted()
    {
        PlayerPrefs.SetInt(currentPuzzleKey, 1);
        PlayerPrefs.Save();

        OpenPanel();
    }

    public void RefreshPanel()
    {
        bool p1 = PlayerPrefs.GetInt(puzzle1Key, 0) == 1;
        bool p2 = PlayerPrefs.GetInt(puzzle2Key, 0) == 1;
        bool p3 = PlayerPrefs.GetInt(puzzle3Key, 0) == 1;
        bool p4 = PlayerPrefs.GetInt(puzzle4Key, 0) == 1;

        UpdatePaintingColor(painting1, p1);
        UpdatePaintingColor(painting2, p2);
        UpdatePaintingColor(painting3, p3);
        UpdatePaintingColor(painting4, p4);

        bool allCompleted = p1 && p2 && p3 && p4;

        if (panelTitleText != null)
            panelTitleText.text = "Pinturas";

        if (bottomButtonText != null)
            bottomButtonText.text = allCompleted ? "Ver historia" : "Siguiente pintura";
    }

    private void UpdatePaintingColor(Image img, bool completed)
    {
        if (img == null) return;

        img.color = completed
            ? Color.white
            : new Color(0.4f, 0.4f, 0.4f, 1f);
    }

    public void OnBottomButton()
    {
        bool allCompleted =
            PlayerPrefs.GetInt(puzzle1Key, 0) == 1 &&
            PlayerPrefs.GetInt(puzzle2Key, 0) == 1 &&
            PlayerPrefs.GetInt(puzzle3Key, 0) == 1 &&
            PlayerPrefs.GetInt(puzzle4Key, 0) == 1;

        if (allCompleted)
        {
            Debug.Log("Aquí después irá Ver historia");
            return;
        }

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }

    public void ResetGalleryProgress()
    {
        PlayerPrefs.DeleteKey(puzzle1Key);
        PlayerPrefs.DeleteKey(puzzle2Key);
        PlayerPrefs.DeleteKey(puzzle3Key);
        PlayerPrefs.DeleteKey(puzzle4Key);
        PlayerPrefs.Save();

        RefreshPanel();
        Debug.Log("Progreso de galería reiniciado.");
    }
}