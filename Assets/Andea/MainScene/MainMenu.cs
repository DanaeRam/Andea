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

    [Header("Botón volver")]
    public Button backButton;

    [Header("Configuración")]
    public string mainSceneName = "MainScene";
    public string welcomeSceneName = "WelcomeScene";
    public string apiUrl = "https://andea.vercel.app/api/validar-codigo";

    private bool loginDirecto = false;

    private void Start()
    {
        if (panelMenuInicial != null)
            panelMenuInicial.SetActive(true);

        if (nameCodePanel != null)
            nameCodePanel.SetActive(false);

        if (inputFieldCode != null)
            inputFieldCode.gameObject.SetActive(false);

        if (acceptCodeButton != null)
            acceptCodeButton.gameObject.SetActive(false);

        if (stateText != null)
            stateText.text = "";

        if (buttonNewPlayer != null)
        {
            buttonNewPlayer.onClick.RemoveAllListeners();
            buttonNewPlayer.onClick.AddListener(OpenNameCodePanel);
        }

        if (buttonLogin != null)
        {
            buttonLogin.onClick.RemoveAllListeners();
            buttonLogin.onClick.AddListener(OpenCodeOnlyPanel);
        }

        if (acceptNameButton != null)
        {
            acceptNameButton.onClick.RemoveAllListeners();
            acceptNameButton.onClick.AddListener(ConfirmName);
        }

        if (acceptCodeButton != null)
        {
            acceptCodeButton.onClick.RemoveAllListeners();
            acceptCodeButton.onClick.AddListener(ConfirmCode);
        }

        if (backButton != null)
        {
            backButton.onClick.RemoveAllListeners();
            backButton.onClick.AddListener(GoBackToMenu);
        }
    }

    public void OpenNameCodePanel()
    {
        loginDirecto = false;

        if (panelMenuInicial != null)
            panelMenuInicial.SetActive(false);

        if (nameCodePanel != null)
            nameCodePanel.SetActive(true);

        if (titleText != null)
            titleText.text = "Ingresa el nombre del niño";

        if (inputFieldName != null)
        {
            inputFieldName.gameObject.SetActive(true);
            inputFieldName.text = "";
        }

        if (acceptNameButton != null)
            acceptNameButton.gameObject.SetActive(true);

        if (inputFieldCode != null)
        {
            inputFieldCode.gameObject.SetActive(false);
            inputFieldCode.text = "";
        }

        if (acceptCodeButton != null)
            acceptCodeButton.gameObject.SetActive(false);

        if (stateText != null)
            stateText.text = "";
    }

    public void OpenCodeOnlyPanel()
    {
        loginDirecto = true;

        if (panelMenuInicial != null)
            panelMenuInicial.SetActive(false);

        if (nameCodePanel != null)
            nameCodePanel.SetActive(true);

        if (titleText != null)
            titleText.text = "Ingresa tu código de jugador";

        if (inputFieldName != null)
            inputFieldName.gameObject.SetActive(false);

        if (acceptNameButton != null)
            acceptNameButton.gameObject.SetActive(false);

        if (inputFieldCode != null)
        {
            inputFieldCode.gameObject.SetActive(true);
            inputFieldCode.text = "";
        }

        if (acceptCodeButton != null)
            acceptCodeButton.gameObject.SetActive(true);

        if (stateText != null)
            stateText.text = "";
    }

    public void ConfirmName()
    {
        if (inputFieldName == null)
            return;

        string playerName = inputFieldName.text.Trim();

        if (string.IsNullOrEmpty(playerName))
        {
            if (titleText != null)
                titleText.text = "Por favor, ingresa el nombre del niño";

            return;
        }

        PlayerPrefs.SetString("PlayerName", playerName);

        if (titleText != null)
            titleText.text = "Ingresa el código del jugador";

        if (inputFieldName != null)
            inputFieldName.gameObject.SetActive(false);

        if (acceptNameButton != null)
            acceptNameButton.gameObject.SetActive(false);

        if (inputFieldCode != null)
            inputFieldCode.gameObject.SetActive(true);

        if (acceptCodeButton != null)
            acceptCodeButton.gameObject.SetActive(true);

        if (stateText != null)
            stateText.text = "";
    }

    public void ConfirmCode()
    {
        if (inputFieldCode == null)
            return;

        string playerCode = inputFieldCode.text.Trim();

        if (string.IsNullOrEmpty(playerCode))
        {
            if (titleText != null)
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

        using UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");
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
            string nombreCompleto = ExtraerValorJson(response, "nombre_completo");

            // Limpiar datos de espada del usuario anterior antes de guardar el nuevo jugador.
            LimpiarEspadaEquipadaLocal();

            PlayerPrefs.SetString("PlayerCode", playerCode);

            if (!string.IsNullOrEmpty(nombreCompleto))
            {
                PlayerPrefs.SetString("PlayerFullName", nombreCompleto);
                PlayerPrefs.SetString("PlayerName", nombreCompleto);
            }

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

    private void LimpiarEspadaEquipadaLocal()
    {
        PlayerPrefs.DeleteKey("EquippedSwordId");
        PlayerPrefs.DeleteKey("EquippedSwordName");
        PlayerPrefs.DeleteKey("EquippedSwordPlayerCode");
    }

    private string ExtraerValorJson(string json, string clave)
    {
        string patron = "\"" + clave + "\":\"";
        int inicio = json.IndexOf(patron);

        if (inicio == -1)
            return "";

        inicio += patron.Length;
        int fin = json.IndexOf("\"", inicio);

        if (fin == -1)
            return "";

        return json.Substring(inicio, fin - inicio);
    }

    public void GoBackToMenu()
    {
        loginDirecto = false;

        if (nameCodePanel != null)
            nameCodePanel.SetActive(false);

        if (panelMenuInicial != null)
            panelMenuInicial.SetActive(true);

        if (stateText != null)
            stateText.text = "";
    }
}