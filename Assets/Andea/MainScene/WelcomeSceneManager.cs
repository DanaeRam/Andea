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

    [Header("Configuración")]
    public string mainSceneName = "MainScene";
    
    public float delayBeforePopup = 3f;

    private int currentMessageIndex = 0;

    private string[] messages =
    {
        "Bienvenido, explorador…",
        "Has encontrado el Santuario de Andea, este no es un lugar común…",
        "Es un portal que conecta tres mundos mágicos:",
        "El mundo de las historias, el mundo de las emociones y el mundo de los números.",
        "Pero algo extraño está ocurriendo…\nla magia de estos mundos está desapareciendo.",
        "Solo alguien valiente, curioso y con gran imaginación puede ayudar a restaurarlos.",
        "¿Te gustaría comenzar la aventura?"
    };

    private void Start()
    {
        skipButton.SetActive(false);
        welcomePanel.SetActive(false);

        skipButton.GetComponent<Button>().onClick.AddListener(SkipToMainScene);

        prevButton.onClick.AddListener(PreviousMessage);
        nextButton.onClick.AddListener(NextMessage);
        startAdventureButton.onClick.AddListener(StartAdventure);

        startAdventureButton.gameObject.SetActive(false);

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

        // Mostrar botón solo en el último mensaje
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

    public void StartAdventure()
    {
        SceneManager.LoadScene(mainSceneName);
    }

    public void SkipToMainScene()
    {
        SceneManager.LoadScene(mainSceneName);
    }
}