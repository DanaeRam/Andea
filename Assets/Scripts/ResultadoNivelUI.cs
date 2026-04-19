using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ResultadoNivelUI : MonoBehaviour
{
    [Header("Textos")]
    public TextMeshProUGUI textoPuntaje;
    public TextMeshProUGUI textoMonedas;
    public TextMeshProUGUI textoSobrantes;

    [Header("Botón")]
    public Button botonContinuar;

    [Header("Escena de respaldo")]
    public string escenaRespaldo = "BasicoL1N2";

    private void Start()
    {
        MostrarResultados();

        if (botonContinuar != null)
        {
            botonContinuar.onClick.RemoveAllListeners();
            botonContinuar.onClick.AddListener(Continuar);
            Debug.Log("Botón continuar conectado correctamente.");
        }
        else
        {
            Debug.LogError("No se asignó el botón continuar en ResultadoNivelUI.");
        }
    }

    private void MostrarResultados()
    {
        if (GameManager.instancia == null)
        {
            Debug.LogError("No existe GameManager al mostrar resultados.");
            return;
        }

        if (textoPuntaje != null)
            textoPuntaje.text = "Puntos obtenidos: " + GameManager.instancia.ultimoPuntajeGanado;

        if (textoMonedas != null)
            textoMonedas.text = "Monedas ganadas: " + GameManager.instancia.ultimasMonedasGanadas;

        if (textoSobrantes != null)
            textoSobrantes.text = "Puntos sobrantes: " + GameManager.instancia.puntosSobrantes;
    }

    public void Continuar()
{
    Debug.Log("Se presionó el botón Continuar.");

    if (GameManager.instancia != null)
    {
        int index = GameManager.instancia.siguienteNivelIndex;

        Debug.Log("Siguiente nivel guardado: " + index);
        Debug.Log("Total escenas en Build Settings: " + SceneManager.sceneCountInBuildSettings);

        if (index >= 0 && index < SceneManager.sceneCountInBuildSettings)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(index);
            Debug.Log("Cargando directamente la escena: " + scenePath);

            SceneManager.LoadScene(index, LoadSceneMode.Single);
        }
        else
        {
            Debug.LogError("Índice de escena inválido: " + index);
        }
    }
    else
    {
        Debug.LogError("No existe GameManager.");
    }
}
}