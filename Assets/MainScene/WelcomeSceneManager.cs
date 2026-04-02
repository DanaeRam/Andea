using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class WelcomeSceneManager : MonoBehaviour
{
    [Header("Botón saltar")]
    public GameObject skipButton;

    [Header("Panel de bienvenida")]
    public GameObject welcomePanel;
    public TextMeshProUGUI messageText;
    public Button prevButton;
    public Button nextButton;
    public Button startAdventureButton;

    [Header("Panel de nombre y código")]
    public GameObject nameCodePanel;
    public TextMeshProUGUI titleText;
    public TMP_InputField inputFieldName;
    public Button acceptNameButton;
    public TMP_InputField inputFieldCode;
    public Button acceptCodeButton;

    [Header("Configuración")]
    public string mainSceneName = "EscenaPrincipal";
    public float delayBeforePopup = 3f;

    private int currentMessageIndex = 0;

    private string[] messages =
    {
        "Bienvenido, explorador…",
        "Has encontrado el Santuario de Andea, no es un lugar común…",
        "Es un portal que conecta tres mundos mágicos: ",
        "El mundo de las historias, el mundo de las emociones y el mundo de los números.",
        "Pero algo extraño está ocurriendo…\nla magia de estos mundos está desapareciendo.",
        "Solo alguien valiente, curioso y con gran imaginación puede ayudar a restaurarlos.",
        "¿Te gustaría comenzar la aventura?"
    };

    private void Start()
    {
        skipButton.SetActive(false);
        welcomePanel.SetActive(false);
        nameCodePanel.SetActive(false);
        
        skipButton.GetComponent<Button>().onClick.AddListener(SkipToMainScene);
        startAdventureButton.gameObject.SetActive(false);

        inputFieldCode.gameObject.SetActive(false);
        acceptCodeButton.gameObject.SetActive(false);

        prevButton.onClick.AddListener(PreviousMessage);
        nextButton.onClick.AddListener(NextMessage);
        startAdventureButton.onClick.AddListener(OpenNamePanel);

        acceptNameButton.onClick.AddListener(ConfirmName);
        acceptCodeButton.onClick.AddListener(ConfirmCode);

        StartCoroutine(ShowWelcomePanelAfterDelay());
    }

    IEnumerator ShowWelcomePanelAfterDelay()
    {
        yield return new WaitForSeconds(delayBeforePopup);

        welcomePanel.SetActive(true);
        skipButton.SetActive(true); 

        currentMessageIndex = 0;
        UpdateMessage();
    }

    void UpdateMessage()
    {
        messageText.text = messages[currentMessageIndex];

        prevButton.gameObject.SetActive(currentMessageIndex > 0);
        nextButton.gameObject.SetActive(currentMessageIndex < messages.Length - 1);

        // Solo mostrar el botón de iniciar aventura en el último mensaje
        startAdventureButton.gameObject.SetActive(currentMessageIndex == messages.Length - 1);
    }

    public void NextMessage()
    {
        if (currentMessageIndex < messages.Length - 1)
        {
            currentMessageIndex++;
            UpdateMessage();
        }
    }

    public void PreviousMessage()
    {
        if (currentMessageIndex > 0)
        {
            currentMessageIndex--;
            UpdateMessage();
        }
    }

    public void OpenNamePanel()
    {
        welcomePanel.SetActive(false);
        nameCodePanel.SetActive(true);

        titleText.text = "Ingresa el nombre del niño";
        inputFieldName.gameObject.SetActive(true);
        acceptNameButton.gameObject.SetActive(true);

        inputFieldCode.gameObject.SetActive(false);
        acceptCodeButton.gameObject.SetActive(false);

        inputFieldName.text = "";
        inputFieldCode.text = "";
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

        titleText.text = "Ingresa el código de jugador";

        inputFieldName.gameObject.SetActive(false);
        acceptNameButton.gameObject.SetActive(false);

        inputFieldCode.gameObject.SetActive(true);
        acceptCodeButton.gameObject.SetActive(true);
    }

    public void ConfirmCode()
    {
        string playerCode = inputFieldCode.text.Trim();

        if (string.IsNullOrEmpty(playerCode))
        {
            titleText.text = "Por favor, ingresa el código de jugador";
            return;
        }

        PlayerPrefs.SetString("PlayerCode", playerCode);

        SceneManager.LoadScene(mainSceneName);
    }

    public void SkipToMainScene()
    {
        SceneManager.LoadScene(mainSceneName);
    }
}