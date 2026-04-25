using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FormulaGame : MonoBehaviour
{
    [Header("Fórmulas y componentes")]
    public GameObject formula1;
    public GameObject formula2;
    public GameObject formula3;
    public TextMeshProUGUI textFormula1;
    public TextMeshProUGUI textFormula2;
    public TextMeshProUGUI textFormula3;

    public Button button1_1, button1_2, button1_3;
    public Button button2_1, button2_2, button2_3;
    public Button button3_1, button3_2, button3_3;

    public Image iconResult1, iconResult2, iconResult3;
    public TextMeshProUGUI textRetro;

    private int correctAnswer;
    private int currentFormulaIndex = 0;
    private bool isCorrectAnswer;

    void Start()
    {
        Debug.Log("Número de sprites cargados (comentado): 0");
        ShowNextFormula();
    }

    void ShowNextFormula()
    {
        if (currentFormulaIndex >= 3)
        {
            textRetro.text = "¡Felicidades, completaste todos los ejercicios!";
            return;
        }

        formula1.SetActive(false);
        formula2.SetActive(false);
        formula3.SetActive(false);

        if (currentFormulaIndex == 0) formula1.SetActive(true);
        if (currentFormulaIndex == 1) formula2.SetActive(true);
        if (currentFormulaIndex == 2) formula3.SetActive(true);

        int num1 = Random.Range(1, 10);
        int num2 = Random.Range(1, 10);
        int result = num1 + num2;

        if (currentFormulaIndex == 0) textFormula1.text = $"{num1} + {num2}";
        if (currentFormulaIndex == 1) textFormula2.text = $"{num1} + {num2}";
        if (currentFormulaIndex == 2) textFormula3.text = $"{num1} + {num2}";

        correctAnswer = result;

        int[] answers = new int[3];
        answers[0] = correctAnswer;
        answers[1] = correctAnswer + Random.Range(1, 3);
        answers[2] = correctAnswer + Random.Range(3, 5);

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

    void CheckAnswer(int buttonIndex)
    {
        int selectedAnswer = int.Parse(GetButtonText(buttonIndex));

        if (selectedAnswer == correctAnswer)
        {
            textRetro.text = "¡Correcto!";
            isCorrectAnswer = true;

            // Comentado para pruebas
            // if (currentFormulaIndex == 0) iconResult1.sprite = Resources.Load<Sprite>("RightIcon");
            // if (currentFormulaIndex == 1) iconResult2.sprite = Resources.Load<Sprite>("RightIcon");
            // if (currentFormulaIndex == 2) iconResult3.sprite = Resources.Load<Sprite>("RightIcon");

            StartCoroutine(WaitForNextFormula(5));
        }
        else
        {
            textRetro.text = "Intenta otra vez.";
            isCorrectAnswer = false;

            // Comentado para pruebas
            // if (currentFormulaIndex == 0) iconResult1.sprite = Resources.Load<Sprite>("WrongIcon");
            // if (currentFormulaIndex == 1) iconResult2.sprite = Resources.Load<Sprite>("WrongIcon");
            // if (currentFormulaIndex == 2) iconResult3.sprite = Resources.Load<Sprite>("WrongIcon");
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

    void SetRandomPotionForFormula()
    {
        // Comentado para pruebas
        // Sprite randomPotion = potionSprites[Random.Range(0, potionSprites.Length)];

        // if (currentFormulaIndex == 0) imageFormula1.sprite = randomPotion;
        // if (currentFormulaIndex == 1) imageFormula2.sprite = randomPotion;
        // if (currentFormulaIndex == 2) imageFormula3.sprite = randomPotion;
    }
}