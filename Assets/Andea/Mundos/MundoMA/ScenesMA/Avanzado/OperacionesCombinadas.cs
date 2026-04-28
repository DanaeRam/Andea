using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class OperacionesCombinadas : MonoBehaviour
{
    [Header("Fórmulas y componentes")]
    public GameObject formula1;
    public GameObject formula2;
    public GameObject formula3;
    public TextMeshProUGUI textFormula1;
    public TextMeshProUGUI textFormula2;
    public TextMeshProUGUI textFormula3;

    [Header("Imágenes de fórmulas")]
    public Image imageFormula1;
    public Image imageFormula2;
    public Image imageFormula3;

    [Header("Banco de pociones")]
    public Sprite[] potionSprites;

    [Header("Iconos")]
    public Sprite rightIcon;
    public Sprite wrongIcon;

    public Button button1_1, button1_2, button1_3;
    public Button button2_1, button2_2, button2_3;
    public Button button3_1, button3_2, button3_3;

    public Image iconResult1, iconResult2, iconResult3;
    public TextMeshProUGUI textRetro;

    private int correctAnswer;
    private int currentFormulaIndex = 0;
    private bool isCorrectAnswer;
    private List<Sprite> availablePotions;

    private Coroutine hideIconCoroutine;

    void Start()
    {
        availablePotions = new List<Sprite>(potionSprites);

        iconResult1.gameObject.SetActive(false);
        iconResult2.gameObject.SetActive(false);
        iconResult3.gameObject.SetActive(false);

        ShowNextFormula();
    }

    void ShowNextFormula()
    {
        if (currentFormulaIndex >= 3)
        {
            StartCoroutine(FinishLesson());
            return;
        }

        if (currentFormulaIndex == 0)
        {
            formula1.SetActive(true);
            formula2.SetActive(false);
            formula3.SetActive(false);

            imageFormula1.sprite = GetRandomPotion();
        }

        if (currentFormulaIndex == 1)
        {
            formula1.SetActive(true);
            formula2.SetActive(true);
            formula3.SetActive(false);

            imageFormula2.sprite = GetRandomPotion();
        }

        if (currentFormulaIndex == 2)
        {
            formula1.SetActive(true);
            formula2.SetActive(true);
            formula3.SetActive(true);

            imageFormula3.sprite = GetRandomPotion();
        }

int a = 0;
int b = 0;
int c = 0;
int result = 0;

bool isValid = false;

while (!isValid)
{
    int operationType = Random.Range(0, 4);

    // 0 = suma/resta
    // 1 = suma/multiplicación
    // 2 = suma/división
    // 3 = resta/multiplicación o resta/división

    if (operationType == 0)
    {
        // a + b - c
        a = Random.Range(0, 10);
        b = Random.Range(0, 10);
        c = Random.Range(0, 10);

        int partial = a + b;

        if (partial >= c)
        {
            result = partial - c;
            SetFormulaText($"{a} + {b} - {c}");
            isValid = true;
        }
    }

    if (operationType == 1)
    {
        // a + b × c
        a = Random.Range(0, 10);
        b = Random.Range(0, 10);
        c = Random.Range(0, 10);

        result = a + (b * c);
        SetFormulaText($"{a} + {b} × {c}");
        isValid = true;
    }

    if (operationType == 2)
    {
        // a + b ÷ c
        int divisor = Random.Range(1, 10);
        int quotient = Random.Range(1, 10);
        int dividend = divisor * quotient;

        a = Random.Range(0, 10);
        b = dividend;
        c = divisor;

        result = a + quotient;
        SetFormulaText($"{a} + {b} ÷ {c}");
        isValid = true;
    }

    if (operationType == 3)
    {
        bool useMultiplication = Random.value > 0.5f;

        if (useMultiplication)
        {
            // a × b - c
            a = Random.Range(0, 10);
            b = Random.Range(0, 10);
            c = Random.Range(0, 10);

            int partial = a * b;

            if (partial >= c)
            {
                result = partial - c;
                SetFormulaText($"{a} × {b} - {c}");
                isValid = true;
            }
        }
        else
        {
            // a ÷ b - c
            int divisor = Random.Range(1, 10);
            int quotient = Random.Range(1, 10);
            int dividend = divisor * quotient;

            a = dividend;
            b = divisor;
            c = Random.Range(0, 10);

            int partial = quotient;

            if (partial >= c)
            {
                result = partial - c;
                SetFormulaText($"{a} ÷ {b} - {c}");
                isValid = true;
            }
        }
    }
}

correctAnswer = result;

void SetFormulaText(string formula)
{
    if (currentFormulaIndex == 0) textFormula1.text = formula;
    if (currentFormulaIndex == 1) textFormula2.text = formula;
    if (currentFormulaIndex == 2) textFormula3.text = formula;
}

        int[] answers = new int[3];
        answers[0] = correctAnswer;
        answers[1] = correctAnswer + 1;
        answers[2] = correctAnswer - 1;

        ShuffleArray(answers);

        if (currentFormulaIndex == 0)
        {
            button1_1.GetComponentInChildren<TextMeshProUGUI>().text = answers[0].ToString();
            button1_2.GetComponentInChildren<TextMeshProUGUI>().text = answers[1].ToString();
            button1_3.GetComponentInChildren<TextMeshProUGUI>().text = answers[2].ToString();

            button1_1.onClick.RemoveAllListeners();
            button1_2.onClick.RemoveAllListeners();
            button1_3.onClick.RemoveAllListeners();

            button1_1.onClick.AddListener(() => CheckAnswer(0));
            button1_2.onClick.AddListener(() => CheckAnswer(1));
            button1_3.onClick.AddListener(() => CheckAnswer(2));
        }

        if (currentFormulaIndex == 1)
        {
            button2_1.GetComponentInChildren<TextMeshProUGUI>().text = answers[0].ToString();
            button2_2.GetComponentInChildren<TextMeshProUGUI>().text = answers[1].ToString();
            button2_3.GetComponentInChildren<TextMeshProUGUI>().text = answers[2].ToString();

            button2_1.onClick.RemoveAllListeners();
            button2_2.onClick.RemoveAllListeners();
            button2_3.onClick.RemoveAllListeners();

            button2_1.onClick.AddListener(() => CheckAnswer(0));
            button2_2.onClick.AddListener(() => CheckAnswer(1));
            button2_3.onClick.AddListener(() => CheckAnswer(2));
        }

        if (currentFormulaIndex == 2)
        {
            button3_1.GetComponentInChildren<TextMeshProUGUI>().text = answers[0].ToString();
            button3_2.GetComponentInChildren<TextMeshProUGUI>().text = answers[1].ToString();
            button3_3.GetComponentInChildren<TextMeshProUGUI>().text = answers[2].ToString();

            button3_1.onClick.RemoveAllListeners();
            button3_2.onClick.RemoveAllListeners();
            button3_3.onClick.RemoveAllListeners();

            button3_1.onClick.AddListener(() => CheckAnswer(0));
            button3_2.onClick.AddListener(() => CheckAnswer(1));
            button3_3.onClick.AddListener(() => CheckAnswer(2));
        }
    }

IEnumerator FinishLesson()
{
    textRetro.text = "¡Felicidades, completaste todos los ejercicios!";

    yield return new WaitForSeconds(2f);

    MathSceneTransitionData.currentRound++;

    MathSceneTransitionData.exitMode = true;

    SceneManager.LoadScene("BasicoTienda");
}
    Sprite GetRandomPotion()
    {
        if (availablePotions == null || availablePotions.Count == 0)
        {
            Debug.LogWarning("No hay pociones disponibles en potionSprites.");
            return null;
        }

        int index = Random.Range(0, availablePotions.Count);
        Sprite selectedPotion = availablePotions[index];

        availablePotions.RemoveAt(index);

        return selectedPotion;
    }

    void CheckAnswer(int buttonIndex)
    {
        int selectedAnswer = int.Parse(GetButtonText(buttonIndex));

        if (selectedAnswer == correctAnswer)
        {
            textRetro.text = "¡Correcto!";
            isCorrectAnswer = true;

            ShowCurrentIcon(rightIcon);

            DisableCurrentButtons();

            StartCoroutine(WaitForNextFormula(2));
        }
        else
        {
            textRetro.text = "Intenta otra vez.";
            isCorrectAnswer = false;

            ShowCurrentIcon(wrongIcon);

            if (hideIconCoroutine != null)
            {
                StopCoroutine(hideIconCoroutine);
            }

            hideIconCoroutine = StartCoroutine(HideCurrentIconAfterSeconds(2));
        }
    }

    void ShowCurrentIcon(Sprite icon)
{
    if (currentFormulaIndex == 0)
    {
        iconResult1.gameObject.SetActive(true);
        iconResult1.sprite = icon;
    }

    if (currentFormulaIndex == 1)
    {
        iconResult2.gameObject.SetActive(true);
        iconResult2.sprite = icon;
    }

    if (currentFormulaIndex == 2)
    {
        iconResult3.gameObject.SetActive(true);
        iconResult3.sprite = icon;
    }
}

IEnumerator HideCurrentIconAfterSeconds(float seconds)
{
    int formulaIndexAtClick = currentFormulaIndex;

    yield return new WaitForSeconds(seconds);

    if (formulaIndexAtClick == 0)
    {
        iconResult1.gameObject.SetActive(false);
    }

    if (formulaIndexAtClick == 1)
    {
        iconResult2.gameObject.SetActive(false);
    }

    if (formulaIndexAtClick == 2)
    {
        iconResult3.gameObject.SetActive(false);
    }
}

    void DisableCurrentButtons()
    {
        if (currentFormulaIndex == 0)
        {
            button1_1.interactable = false;
            button1_2.interactable = false;
            button1_3.interactable = false;
        }

        if (currentFormulaIndex == 1)
        {
            button2_1.interactable = false;
            button2_2.interactable = false;
            button2_3.interactable = false;
        }

        if (currentFormulaIndex == 2)
        {
            button3_1.interactable = false;
            button3_2.interactable = false;
            button3_3.interactable = false;
        }
    }

    IEnumerator WaitForNextFormula(int waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        currentFormulaIndex++;
        ShowNextFormula();
    }

    string GetButtonText(int index)
    {
        if (currentFormulaIndex == 0)
        {
            if (index == 0) return button1_1.GetComponentInChildren<TextMeshProUGUI>().text;
            if (index == 1) return button1_2.GetComponentInChildren<TextMeshProUGUI>().text;
            if (index == 2) return button1_3.GetComponentInChildren<TextMeshProUGUI>().text;
        }

        if (currentFormulaIndex == 1)
        {
            if (index == 0) return button2_1.GetComponentInChildren<TextMeshProUGUI>().text;
            if (index == 1) return button2_2.GetComponentInChildren<TextMeshProUGUI>().text;
            if (index == 2) return button2_3.GetComponentInChildren<TextMeshProUGUI>().text;
        }

        if (currentFormulaIndex == 2)
        {
            if (index == 0) return button3_1.GetComponentInChildren<TextMeshProUGUI>().text;
            if (index == 1) return button3_2.GetComponentInChildren<TextMeshProUGUI>().text;
            if (index == 2) return button3_3.GetComponentInChildren<TextMeshProUGUI>().text;
        }

        return string.Empty;
    }

    void ShuffleArray(int[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            int tmp = array[i];
            int randomIndex = Random.Range(i, array.Length);
            array[i] = array[randomIndex];
            array[randomIndex] = tmp;
        }
    }
}