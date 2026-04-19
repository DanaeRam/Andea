using TMPro;
using UnityEngine;

public class GameManagerUIBinder : MonoBehaviour
{
    [Header("Textos de UI de esta escena")]
    public TextMeshProUGUI textoMonedas;
    public TextMeshProUGUI textoVidas;
    public TextMeshProUGUI textoPuntos;
    public TextMeshProUGUI textoMonedasTienda;

    private void Start()
    {
        if (GameManager.instancia == null)
        {
            Debug.LogError("No existe GameManager en la escena.");
            return;
        }

        GameManager.instancia.AsignarTextosUI(
            textoMonedas,
            textoVidas,
            textoPuntos,
            textoMonedasTienda
        );
    }
}