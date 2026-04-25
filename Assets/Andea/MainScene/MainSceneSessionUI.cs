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

    private void Start()
    {
        MostrarNombreJugador();

        if (logoutButton != null)
        {
            logoutButton.onClick.RemoveAllListeners();
            logoutButton.onClick.AddListener(CerrarSesion);
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