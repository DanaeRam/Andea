using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ResultadoNivelUI : MonoBehaviour
{
    [Header("Textos principales")]
    public TextMeshProUGUI textoPuntaje;
    public TextMeshProUGUI textoMonedas;
    public TextMeshProUGUI textoSobrantes;

    [Header("Textos de penalización")]
    public TextMeshProUGUI textoMuertes;
    public TextMeshProUGUI textoRespuestasIncorrectas;
    public TextMeshProUGUI textoPenalizacion;

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

        GameManager gm = GameManager.instancia;

        if (textoPuntaje != null)
            textoPuntaje.text = "Puntos obtenidos: " + gm.ultimoPuntajeGanado;

        if (textoMonedas != null)
            textoMonedas.text = "Runas ganadas: " + gm.ultimasMonedasGanadas;

        if (textoSobrantes != null)
            textoSobrantes.text = "Puntos sobrantes: " + gm.puntosSobrantes;

        if (textoMuertes != null)
            textoMuertes.text = "Muertes: " + gm.muertesNivel + "  (-" + (gm.muertesNivel * 3) + " puntos)";

        if (textoRespuestasIncorrectas != null)
            textoRespuestasIncorrectas.text = "Respuestas incorrectas: " + gm.respuestasIncorrectasNivel + "  (-" + (gm.respuestasIncorrectasNivel * 2) + " puntos)";

        if (textoPenalizacion != null)
            textoPenalizacion.text = "Penalización total: -" + gm.penalizacionUltimoNivel + " puntos";
    }

    public void Continuar()
    {
        SceneManager.LoadScene(escenaSalida);
    }
}