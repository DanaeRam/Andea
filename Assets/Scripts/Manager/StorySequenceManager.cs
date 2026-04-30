using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StorySequenceManager : MonoBehaviour
{
    [Header("Historia")]
    public TextMeshProUGUI storyText;
    public Button nextButton;

    [Header("Pinturas (Images UI)")]
    public Image painting1;
    public Image painting2;
    public Image painting3;
    public Image painting4;

    [Header("Panel de pregunta")]
    public GameObject questionPanel;
    public TextMeshProUGUI questionText;
    public Button optionAButton;
    public Button optionBButton;
    public Button optionCButton;
    public TextMeshProUGUI feedbackText;

    private int currentStep = 0;

    private void Start()
    {
        if (questionPanel != null)
            questionPanel.SetActive(false);

        if (feedbackText != null)
            feedbackText.text = "";

        nextButton.onClick.RemoveAllListeners();
        nextButton.onClick.AddListener(NextStep);

        optionAButton.onClick.AddListener(OnOptionA);
        optionBButton.onClick.AddListener(OnOptionB);
        optionCButton.onClick.AddListener(OnOptionC);

        ShowStep(0);
    }

    public void NextStep()
    {
        currentStep++;

        if (currentStep <= 2)
            ShowStep(currentStep);
        else if (currentStep == 3)
            ShowQuestion();
    }

    void ShowOnlyPainting(Image active)
    {
        if (painting1 != null) painting1.gameObject.SetActive(false);
        if (painting2 != null) painting2.gameObject.SetActive(false);
        if (painting3 != null) painting3.gameObject.SetActive(false);
        if (painting4 != null) painting4.gameObject.SetActive(false);

        if (active != null)
            active.gameObject.SetActive(true);
    }

    private void ShowStep(int step)
    {
        questionPanel.SetActive(false);
        nextButton.gameObject.SetActive(true);

        switch (step)
        {
            case 0:
                ShowOnlyPainting(painting1);
                storyText.text = "Luis llegó al recreo y vio a todos jugando… pero nadie lo invitó.";
                break;

            case 1:
                ShowOnlyPainting(painting2);
                storyText.text = "Se sentó en una banca. Sentía un nudo en la garganta y no sabía qué hacer.\n\nPensó: 'Tal vez no quieren jugar conmigo…' y comenzó a sentirse triste.";
                break;

            case 2:
                ShowOnlyPainting(painting3);
                storyText.text = "De pronto, una pelota rodó hasta él. Era su oportunidad de hacer algo.";
                break;

            case 4:
                ShowOnlyPainting(painting4);
                storyText.text = "Luis llevó la pelota al grupo y preguntó si podía jugar. Los demás lo invitaron a unirse y se sintió mucho mejor.";
                nextButton.gameObject.SetActive(false);
                break;
        }
    }

    private void ShowQuestion()
    {
        questionPanel.SetActive(true);
        nextButton.gameObject.SetActive(false);

        questionText.text = "¿Qué debería hacer Luis?";

        optionAButton.GetComponentInChildren<TextMeshProUGUI>().text = "Ignorar la pelota y quedarse solo";
        optionBButton.GetComponentInChildren<TextMeshProUGUI>().text = "Llevar la pelota al grupo y preguntar si puede jugar";
        optionCButton.GetComponentInChildren<TextMeshProUGUI>().text = "Enojarse y patear la pelota lejos";

        feedbackText.text = "";
    }

    private void OnOptionA()
    {
        feedbackText.text = "Pista: quedarse solo no ayuda. ¿Qué acción lo acercaría a los demás?";
    }

    private void OnOptionB()
    {
        questionPanel.SetActive(false);
        currentStep = 4;
        ShowStep(4);
    }

    private void OnOptionC()
    {
        feedbackText.text = "Pista: enojarse podría empeorar todo. Piensa en una opción positiva.";
    }
}