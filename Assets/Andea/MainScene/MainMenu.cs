using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    [Header("Paneles")]
    public GameObject panelMenuInicial;
    public GameObject nameCodePanel;

    [Header("Botones menú inicial")]
    public Button buttonLogin;
    public Button buttonNewPlayer;

    [Header("Panel nombre y código")]
    public TextMeshProUGUI titleText;
    public TMP_InputField inputFieldName;
    public Button acceptNameButton;
    public TMP_InputField inputFieldCode;
    public Button acceptCodeButton;

    [Header("Mensaje de estado")]
    public TextMeshProUGUI stateText;

    [Header("Botón Volver")]
    public Button backButton;  

    [Header("Configuración")]
    public string mainSceneName = "MainScene";
    public string welcomeSceneName = "WelcomeScene";
    public string apiUrl = "https://andea.vercel.app/api/validar-codigo";

    private void Start()
    {
        panelMenuInicial.SetActive(true);
        nameCodePanel.SetActive(false);

        inputFieldCode.gameObject.SetActive(false);
        acceptCodeButton.gameObject.SetActive(false);

        if (stateText != null)
        {
            stateText.text = "";
        }

        buttonNewPlayer.onClick.AddListener(OpenNameCodePanel);
        acceptNameButton.onClick.AddListener(ConfirmName);
        acceptCodeButton.onClick.AddListener(ConfirmCode);

        if (buttonLogin != null)
        {
            buttonLogin.onClick.AddListener(OnLoginPressed);
        }

        if (backButton != null)  // Asegúrate de que el botón "Back" esté asignado en el Inspector
        {
            backButton.onClick.AddListener(GoBackToMenu);  // Conecta el botón de regreso
        }
    }

    public void OpenNameCodePanel()
    {
        panelMenuInicial.SetActive(false);
        nameCodePanel.SetActive(true);

        titleText.text = "Ingresa el nombre del niño";

        inputFieldName.gameObject.SetActive(true);
        acceptNameButton.gameObject.SetActive(true);

        inputFieldCode.gameObject.SetActive(false);
        acceptCodeButton.gameObject.SetActive(false);

        inputFieldName.text = "";
        inputFieldCode.text = "";

        if (stateText != null)
        {
            stateText.text = "";
        }
    }

    public void ConfirmName()
    {
        string playerName = inputFieldName.text.Trim();

        if (string.IsNullOrEmpty(playerName))
        {
            titleText.text = "Por favor, ingresa el nombre del niño";
            return;
        }

        PlayerPrefs.SetString("PlayerName", playerName);

        titleText.text = "Ingresa el código del jugador";

        inputFieldName.gameObject.SetActive(false);
        acceptNameButton.gameObject.SetActive(false);

        inputFieldCode.gameObject.SetActive(true);
        acceptCodeButton.gameObject.SetActive(true);

        if (stateText != null)
        {
            stateText.text = "";
        }
    }

    public void ConfirmCode()
    {
        string playerCode = inputFieldCode.text.Trim();

        if (string.IsNullOrEmpty(playerCode))
        {
            titleText.text = "Por favor, ingresa el código del jugador";
            return;
        }

        StartCoroutine(ValidateCode(playerCode));
    }

    IEnumerator ValidateCode(string playerCode)
    {
        if (stateText != null)
        {
            stateText.text = "Validando código...";
        }

        string json = "{\"codigo\":\"" + playerCode + "\"}";

        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);

        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error de conexión: " + request.error);

            if (stateText != null)
            {
                stateText.text = "Error de conexión con el servidor";
            }

            yield break;
        }

        string response = request.downloadHandler.text;
        Debug.Log("Respuesta API: " + response);

        if (response.Contains("\"valid\":true"))
        {
            PlayerPrefs.SetString("PlayerCode", playerCode);

            if (stateText != null)
            {
                stateText.text = "Código correcto";
            }

            yield return new WaitForSeconds(1f);
            SceneManager.LoadScene(welcomeSceneName);
        }
        else
        {
            if (stateText != null)
            {
                stateText.text = "Código incorrecto";
            }
        }
    }

    public void OnLoginPressed()
    {
        SceneManager.LoadScene(mainSceneName);
    }

    public void GoBackToMenu()
    {
        nameCodePanel.SetActive(false);  // Desactiva el panel de código
        panelMenuInicial.SetActive(true);  // Activa el panel del menú inicial
    }
}