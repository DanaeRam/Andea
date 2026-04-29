using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainSceneSessionUI : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI playerNameText;
    public Button logoutButton;

    [Header("Escena de login")]
    public string loginSceneName = "MainMenu";

    [Header("Panel Config")]
    public GameObject panelConfig;
    public Button buttonConfig;
    public Button buttonCerrarConfig;

    [Header("Panel Help")]
    public GameObject panelHelpMainScene;
    public Button buttonHelp;
    public Button buttonCerrarHelp;

    private void Start()
    {
        MostrarNombreJugador();

        if (logoutButton != null)
        {
            logoutButton.onClick.RemoveAllListeners();
            logoutButton.onClick.AddListener(CerrarSesion);
        }

        if (buttonConfig != null)
        {
            buttonConfig.onClick.RemoveAllListeners();
            buttonConfig.onClick.AddListener(AbrirConfig);
        }

        if (buttonCerrarConfig != null)
        {
            buttonCerrarConfig.onClick.RemoveAllListeners();
            buttonCerrarConfig.onClick.AddListener(CerrarConfig);
        }

        if (buttonHelp != null)
        {
            buttonHelp.onClick.RemoveAllListeners();
            buttonHelp.onClick.AddListener(AbrirHelp);
        }

        if (buttonCerrarHelp != null)
        {
            buttonCerrarHelp.onClick.RemoveAllListeners();
            buttonCerrarHelp.onClick.AddListener(CerrarHelp);
        }

        if (panelConfig != null)
            panelConfig.SetActive(false);

        if (panelHelpMainScene != null)
            panelHelpMainScene.SetActive(false);
    }

    public void AbrirConfig()
    {
        if (panelConfig != null)
            panelConfig.SetActive(true);
    }

    public void CerrarConfig()
    {
        if (panelConfig != null)
            panelConfig.SetActive(false);
    }

    public void AbrirHelp()
    {
        if (panelHelpMainScene != null)
            panelHelpMainScene.SetActive(true);
    }

    public void CerrarHelp()
    {
        if (panelHelpMainScene != null)
            panelHelpMainScene.SetActive(false);
    }

    private void MostrarNombreJugador()
    {
        string playerNickname = PlayerPrefs.GetString("PlayerNickname", "");
        string playerFullName = PlayerPrefs.GetString("PlayerFullName", "");
        string playerName = PlayerPrefs.GetString("PlayerName", "");

        if (playerNameText == null)
            return;

        if (!string.IsNullOrEmpty(playerNickname))
            playerNameText.text = "Hola, " + playerNickname;
        else if (!string.IsNullOrEmpty(playerFullName))
            playerNameText.text = "Hola, " + playerFullName;
        else if (!string.IsNullOrEmpty(playerName))
            playerNameText.text = "Hola, " + playerName;
        else
            playerNameText.text = "Hola, Explorador";
    }

    public void CerrarSesion()
    {
        PlayerPrefs.DeleteKey("PlayerName");
        PlayerPrefs.DeleteKey("PlayerNickname");
        PlayerPrefs.DeleteKey("PlayerFullName");
        PlayerPrefs.DeleteKey("PlayerCode");

        PlayerPrefs.DeleteKey("EquippedSwordId");
        PlayerPrefs.DeleteKey("EquippedSwordName");
        PlayerPrefs.DeleteKey("EquippedSwordPlayerCode");

        PlayerPrefs.DeleteKey("SMPainting1");
        PlayerPrefs.DeleteKey("SMPainting2");
        PlayerPrefs.DeleteKey("SMPainting3");
        PlayerPrefs.DeleteKey("SMPainting4");

        PlayerPrefs.DeleteKey("CurrentWorldCode");
        PlayerPrefs.DeleteKey("CurrentLessonId");
        PlayerPrefs.DeleteKey("CurrentLevelName");
        PlayerPrefs.DeleteKey("CurrentLessonName");

        PlayerPrefs.Save();

        if (GameManager.instancia != null)
            Destroy(GameManager.instancia.gameObject);

        if (PlayerProgressApi.Instance != null)
            Destroy(PlayerProgressApi.Instance.gameObject);

        if (PlayerLessonsApi.Instance != null)
            Destroy(PlayerLessonsApi.Instance.gameObject);

        SceneManager.LoadScene(loginSceneName);
    }
}