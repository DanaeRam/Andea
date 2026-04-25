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

    [Header("Escena de salida")]
    public string escenaSalida = "MainScene";

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip sonidoResultados;

    private void Start()
    {
        MostrarResultados();

        AudioListener.pause = false;

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (audioSource != null && sonidoResultados != null)
            audioSource.PlayOneShot(sonidoResultados);
        else
            Debug.LogWarning("Falta AudioSource o sonidoResultados en ResultadoNivelUI.");

        if (botonContinuar != null)
        {
            botonContinuar.onClick.RemoveAllListeners();
            botonContinuar.onClick.AddListener(Continuar);
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
            textoMonedas.text = "Runas ganadas: " + GameManager.instancia.ultimasMonedasGanadas;

        if (textoSobrantes != null)
            textoSobrantes.text = "Puntos sobrantes: " + GameManager.instancia.puntosSobrantes;
    }

    public void Continuar()
    {
        SceneManager.LoadScene(escenaSalida);
    }
}