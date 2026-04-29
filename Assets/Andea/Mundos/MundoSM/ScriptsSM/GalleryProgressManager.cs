using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GalleryProgressManager : MonoBehaviour
{
    [Header("Datos del puzzle actual")]
    public string currentPuzzleKey;
    public string nextSceneName;
    public string storySceneName = "StoryPainting1";

    [Header("Sincronización con base de datos")]
    public bool syncWithDatabase = true;
    public string worldCode = "SM";
    public string currentLessonId = "SM_LECCION_01";

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

    [Header("Claves de progreso local")]
    public string puzzle1Key = "SMPainting1";
    public string puzzle2Key = "SMPainting2";
    public string puzzle3Key = "SMPainting3";
    public string puzzle4Key = "SMPainting4";

    private bool syncInProgress = false;

    private void Start()
    {
        if (string.IsNullOrEmpty(worldCode))
            worldCode = "SM";

        worldCode = worldCode.Trim().ToUpper();

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
        Debug.Log("ENTRÓ A MarkCurrentPuzzleCompleted en: " + currentPuzzleKey);

        if (string.IsNullOrEmpty(currentPuzzleKey))
        {
            Debug.LogError("Current Puzzle Key está vacío en GalleryProgressManager.");
            return;
        }

        if (string.IsNullOrEmpty(worldCode))
            worldCode = "SM";

        worldCode = worldCode.Trim().ToUpper();

        if (string.IsNullOrEmpty(currentLessonId))
        {
            Debug.LogError("Current Lesson Id está vacío en GalleryProgressManager.");
            return;
        }

        PlayerPrefs.SetInt(currentPuzzleKey, 1);

        PlayerPrefs.SetString("CurrentWorldCode", worldCode);
        PlayerPrefs.SetString("CurrentLessonId", currentLessonId);
        PlayerPrefs.SetString("CurrentLevelName", "SaludMental");
        PlayerPrefs.SetString("CurrentLessonName", currentPuzzleKey);
        PlayerPrefs.Save();

        Debug.Log("Puzzle completado localmente: " + currentPuzzleKey);
        Debug.Log("SM lección seleccionada: " + currentLessonId);

        if (syncWithDatabase)
        {
            StartCoroutine(SyncLessonCompletedToDatabase());
        }
        else
        {
            Debug.LogWarning("Sync With Database está desactivado. No se guardará en Supabase.");
        }

        OpenPanel();
    }

    private IEnumerator SyncLessonCompletedToDatabase()
    {
        Debug.Log("Entró a SyncLessonCompletedToDatabase.");

        if (syncInProgress)
        {
            Debug.LogWarning("No se sincroniza porque syncInProgress ya está en true.");
            yield break;
        }

        if (PlayerLessonsApi.Instance == null)
        {
            Debug.LogWarning("No existe PlayerLessonsApi. No se pudo sincronizar progreso SM.");
            yield break;
        }

        if (string.IsNullOrEmpty(worldCode))
            worldCode = "SM";

        worldCode = worldCode.Trim().ToUpper();

        if (string.IsNullOrEmpty(currentLessonId))
        {
            Debug.LogWarning("Current Lesson Id está vacío en GalleryProgressManager.");
            yield break;
        }

        string playerCode = PlayerPrefs.GetString("PlayerCode", "");
        if (string.IsNullOrEmpty(playerCode))
        {
            Debug.LogWarning("No hay PlayerCode guardado. No se pudo sincronizar progreso SM.");
            yield break;
        }

        syncInProgress = true;

        Debug.Log("PlayerCode actual: " + playerCode);
        Debug.Log("Intentando guardar progreso SM -> mundo: " + worldCode + " | leccionId: " + currentLessonId);

        yield return StartCoroutine(
            PlayerLessonsApi.Instance.CompletarLeccion(
                worldCode,
                currentLessonId,
                onSuccess: (data) =>
                {
                    Debug.Log("Progreso SM guardado en base de datos: " + data.leccion_id);
                },
                onError: (error) =>
                {
                    Debug.LogError("Error al guardar progreso SM: " + error);
                }
            )
        );

        syncInProgress = false;
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
        if (img == null)
            return;

        img.color = completed
            ? new Color(1f, 1f, 1f, 1f)
            : new Color(0.25f, 0.25f, 0.25f, 0.45f);
    }

    public void OnBottomButton()
    {
        bool allCompleted =
            PlayerPrefs.GetInt(puzzle1Key, 0) == 1 &&
            PlayerPrefs.GetInt(puzzle2Key, 0) == 1 &&
            PlayerPrefs.GetInt(puzzle3Key, 0) == 1 &&
            PlayerPrefs.GetInt(puzzle4Key, 0) == 1;

        Debug.Log(
            "BottomButton presionado. allCompleted: " + allCompleted +
            " | currentPuzzleKey: " + currentPuzzleKey +
            " | nextSceneName: " + nextSceneName +
            " | storySceneName: " + storySceneName
        );

        if (allCompleted)
        {
            if (string.IsNullOrEmpty(storySceneName))
            {
                Debug.LogError("Story Scene Name está vacío.");
                return;
            }

            SceneManager.LoadScene(storySceneName);
            return;
        }

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
        else
        {
            Debug.LogError("Next Scene Name está vacío en " + currentPuzzleKey);
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