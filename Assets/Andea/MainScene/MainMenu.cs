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

    private bool loginDirecto = false;

    private void Start()
    {
        panelMenuInicial.SetActive(true);
        nameCodePanel.SetActive(false);

        inputFieldCode.gameObject.SetActive(false);
        acceptCodeButton.gameObject.SetActive(false);

        if (stateText != null)
            stateText.text = "";

        if (buttonNewPlayer != null)
            buttonNewPlayer.onClick.AddListener(OpenNameCodePanel);

        if (buttonLogin != null)
            buttonLogin.onClick.AddListener(OpenCodeOnlyPanel);

        if (acceptNameButton != null)
            acceptNameButton.onClick.AddListener(ConfirmName);

        if (acceptCodeButton != null)
            acceptCodeButton.onClick.AddListener(ConfirmCode);

        if (backButton != null)
            backButton.onClick.AddListener(GoBackToMenu);
    }

    public void OpenNameCodePanel()
    {
        loginDirecto = false;

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
            stateText.text = "";
    }

    public void OpenCodeOnlyPanel()
    {
        loginDirecto = true;

        panelMenuInicial.SetActive(false);
        nameCodePanel.SetActive(true);

        titleText.text = "Ingresa tu código de jugador";

        inputFieldName.gameObject.SetActive(false);
        acceptNameButton.gameObject.SetActive(false);

        inputFieldCode.gameObject.SetActive(true);
        acceptCodeButton.gameObject.SetActive(true);

        inputFieldCode.text = "";

        if (stateText != null)
            stateText.text = "";
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
            stateText.text = "";
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

    private IEnumerator ValidateCode(string playerCode)
    {
        if (stateText != null)
            stateText.text = "Validando código...";

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
                stateText.text = "Error de conexión con el servidor";

            yield break;
        }

        string response = request.downloadHandler.text;
        Debug.Log("Respuesta API: " + response);

        if (response.Contains("\"valid\":true"))
        {
            PlayerPrefs.SetString("PlayerCode", playerCode);
            PlayerPrefs.Save();

            if (GameManager.instancia != null)
            {
                GameManager.instancia.SolicitarProgresoInicialSiExisteCodigo();
            }
            else
            {
                Debug.LogWarning("No existe GameManager al validar el código.");
            }

            if (stateText != null)
                stateText.text = "Código correcto";

            yield return new WaitForSeconds(1f);

            if (loginDirecto)
                SceneManager.LoadScene(mainSceneName);
            else
                SceneManager.LoadScene(welcomeSceneName);
        }
        else
        {
            if (stateText != null)
                stateText.text = "Código incorrecto";
        }
    }

    public void GoBackToMenu()
    {
        loginDirecto = false;

        nameCodePanel.SetActive(false);
        panelMenuInicial.SetActive(true);

        if (stateText != null)
            stateText.text = "";
    }
}