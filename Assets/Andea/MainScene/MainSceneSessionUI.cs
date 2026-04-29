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

    private void Start()
    {
        MostrarNombreJugador();

        if (logoutButton != null)
        {
            logoutButton.onClick.RemoveAllListeners();
            logoutButton.onClick.AddListener(CerrarSesion);
        }

                // Abrir panel config
        if (buttonConfig != null)
        {
            buttonConfig.onClick.RemoveAllListeners();
            buttonConfig.onClick.AddListener(AbrirConfig);
        }

        // Cerrar panel config
        if (buttonCerrarConfig != null)
        {
            buttonCerrarConfig.onClick.RemoveAllListeners();
            buttonCerrarConfig.onClick.AddListener(CerrarConfig);
        }

        // Asegurar que inicia cerrado
        if (panelConfig != null)
        {
            panelConfig.SetActive(false);
        }
    }

        public void AbrirConfig()
        {
            if (panelConfig != null)
            {
                panelConfig.SetActive(true);
            }
        }

        public void CerrarConfig()
        {
            if (panelConfig != null)
            {
                panelConfig.SetActive(false);
            }
        }



    private void MostrarNombreJugador()
    {
        // Primero intenta usar el nombre real que viene de la base de datos
        string playerFullName = PlayerPrefs.GetString("PlayerFullName", "");

        // Como respaldo, usa el nombre local escrito por el usuario
        string playerName = PlayerPrefs.GetString("PlayerName", "");

        if (playerNameText != null)
        {
            if (!string.IsNullOrEmpty(playerFullName))
            {
                playerNameText.text = "Hola, " + playerFullName;
            }
            else if (!string.IsNullOrEmpty(playerName))
            {
                playerNameText.text = "Hola, " + playerName;
            }
            else
            {
                playerNameText.text = "Hola, Explorador";
            }
        }
    }

    public void CerrarSesion()
    {
        PlayerPrefs.DeleteKey("PlayerName");
        PlayerPrefs.DeleteKey("PlayerFullName");
        PlayerPrefs.DeleteKey("PlayerCode");
        PlayerPrefs.Save();

        if (GameManager.instancia != null)
        {
            Destroy(GameManager.instancia.gameObject);
        }

        if (PlayerProgressApi.Instance != null)
        {
            Destroy(PlayerProgressApi.Instance.gameObject);
        }

        SceneManager.LoadScene(loginSceneName);
    }
}

