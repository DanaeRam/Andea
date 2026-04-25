using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FormulaGame : MonoBehaviour
{
    [Header("Fórmulas y componentes")]
    public GameObject formula1; // Formula1
    public GameObject formula2; // Formula2
    public GameObject formula3; // Formula3
    public TextMeshProUGUI textFormula1; // TextFormula1
    public TextMeshProUGUI textFormula2; // TextFormula2
    public TextMeshProUGUI textFormula3; // TextFormula3

    public Button button1_1, button1_2, button1_3; // Buttons en Formula1
    public Button button2_1, button2_2, button2_3; // Buttons en Formula2
    public Button button3_1, button3_2, button3_3; // Buttons en Formula3

    public Image iconResult1, iconResult2, iconResult3; // Iconos para los resultados de las fórmulas
    public TextMeshProUGUI textRetro; // Texto de retroalimentación

    // Comentado: Cargar las imágenes de las fórmulas y los iconos
    // public Image imageFormula1, imageFormula2, imageFormula3; // Imágenes para las fórmulas
    // public Sprite[] potionSprites; // Sprite array para las pociones

    private int correctAnswer;
    private int currentFormulaIndex = 0;
    private bool isCorrectAnswer;

    void Start()
    {
        // Comentado para probar lo demás
        // potionSprites = Resources.LoadAll<Sprite>("MundoMA/Resources");

        // Temporarily set to empty to avoid errors while testing
        Debug.Log("Número de sprites cargados (comentado): 0");

        ShowNextFormula();
    }

    // Mostrar la siguiente fórmula en la secuencia
    void ShowNextFormula()
    {
        if (currentFormulaIndex >= 3)
        {
            // Final del juego, muestra mensaje de felicitación
            textRetro.text = "¡Felicidades, completaste todos los ejercicios!";
            return;
        }

        // Ocultar todas las fórmulas
        formula1.SetActive(false);
        formula2.SetActive(false);
        formula3.SetActive(false);

        // Mostrar la fórmula actual
        if (currentFormulaIndex == 0) formula1.SetActive(true);
        if (currentFormulaIndex == 1) formula2.SetActive(true);
        if (currentFormulaIndex == 2) formula3.SetActive(true);

        // Comentado: Asignar las imágenes de las fórmulas
        // SetRandomPotionForFormula();

        // Generar los números aleatorios para la suma
        int num1 = Random.Range(1, 10);
        int num2 = Random.Range(1, 10);
        int result = num1 + num2;

        // Asignar el texto de la fórmula
        if (currentFormulaIndex == 0) textFormula1.text = $"{num1} + {num2}";
        if (currentFormulaIndex == 1) textFormula2.text = $"{num1} + {num2}";
        if (currentFormulaIndex == 2) textFormula3.text = $"{num1} + {num2}";

        // Almacenar la respuesta correcta
        correctAnswer = result;

        // Generar respuestas incorrectas y mezclarlas
        int[] answers = new int[3];
        answers[0] = correctAnswer;
        answers[1] = correctAnswer + Random.Range(1, 3); // Respuesta incorrecta
        answers[2] = correctAnswer + Random.Range(1, 3); // Otra respuesta incorrecta

        // Mezclar las respuestas
        ShuffleArray(answers);

        // Asignar respuestas a los botones de forma aleatoria
        if (currentFormulaIndex == 0)
        {
            button1_1.GetComponentInChildren<TextMeshProUGUI>().text = answers[0].ToString();
            button1_2.GetComponentInChildren<TextMeshProUGUI>().text = answers[1].ToString();
            button1_3.GetComponentInChildren<TextMeshProUGUI>().text = answers[2].ToString();
        }
        if (currentFormulaIndex == 1)
        {
            button2_1.GetComponentInChildren<TextMeshProUGUI>().text = answers[0].ToString();
            button2_2.GetComponentInChildren<TextMeshProUGUI>().text = answers[1].ToString();
            button2_3.GetComponentInChildren<TextMeshProUGUI>().text = answers[2].ToString();
        }
        if (currentFormulaIndex == 2)
        {
            button3_1.GetComponentInChildren<TextMeshProUGUI>().text = answers[0].ToString();
            button3_2.GetComponentInChildren<TextMeshProUGUI>().text = answers[1].ToString();
            button3_3.GetComponentInChildren<TextMeshProUGUI>().text = answers[2].ToString();
        }

        // Asignar los listeners a los botones para comprobar las respuestas
        if (currentFormulaIndex == 0)
        {
            button1_1.onClick.RemoveAllListeners();
            button1_2.onClick.RemoveAllListeners();
            button1_3.onClick.RemoveAllListeners();

            button1_1.onClick.AddListener(() => CheckAnswer(0));
            button1_2.onClick.AddListener(() => CheckAnswer(1));
            button1_3.onClick.AddListener(() => CheckAnswer(2));
        }
        if (currentFormulaIndex == 1)
        {
            button2_1.onClick.RemoveAllListeners();
            button2_2.onClick.RemoveAllListeners();
            button2_3.onClick.RemoveAllListeners();

            button2_1.onClick.AddListener(() => CheckAnswer(0));
            button2_2.onClick.AddListener(() => CheckAnswer(1));
            button2_3.onClick.AddListener(() => CheckAnswer(2));
        }
        if (currentFormulaIndex == 2)
        {
            button3_1.onClick.RemoveAllListeners();
            button3_2.onClick.RemoveAllListeners();
            button3_3.onClick.RemoveAllListeners();

            button3_1.onClick.AddListener(() => CheckAnswer(0));
            button3_2.onClick.AddListener(() => CheckAnswer(1));
            button3_3.onClick.AddListener(() => CheckAnswer(2));
        }
    }

    // Función que verifica la respuesta seleccionada
    void CheckAnswer(int buttonIndex)
    {
        int selectedAnswer = int.Parse(GetButtonText(buttonIndex));

        // Si la respuesta es correcta
        if (selectedAnswer == correctAnswer)
        {
            textRetro.text = "¡Correcto!";

            // Esperar 5 segundos antes de pasar a la siguiente fórmula
            StartCoroutine(WaitForNextFormula(5));
        }
        else
        {
            textRetro.text = "Intenta otra vez.";
        }
    }

    // Esperar un momento antes de pasar a la siguiente fórmula
    IEnumerator WaitForNextFormula(int waitTime)
    {
        yield return new WaitForSeconds(waitTime); // Espera de 5 segundos
        currentFormulaIndex++;
        ShowNextFormula();
    }

    // Función para obtener el texto del botón
    string GetButtonText(int index)
    {
        if (currentFormulaIndex == 0)
            return button1_1.GetComponentInChildren<TextMeshProUGUI>().text;
        if (currentFormulaIndex == 1)
            return button2_1.GetComponentInChildren<TextMeshProUGUI>().text;
        if (currentFormulaIndex == 2)
            return button3_1.GetComponentInChildren<TextMeshProUGUI>().text;

        return string.Empty;
    }

    // Función para mezclar las respuestas
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