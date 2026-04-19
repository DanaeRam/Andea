using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instancia;

    [Header("Monedas")]
    public int monedas = 0;

    [Header("Vidas")]
    public int vidas = 3;

    [Header("UI")]
    public TextMeshProUGUI textoMonedas;
    public TextMeshProUGUI textoVidas;

    private void Awake()
    {
        if (instancia == null)
        {
            instancia = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        ActualizarTextoMonedas();
        ActualizarTextoVidas();
    }

    public void SumarMoneda(int cantidad = 1)
    {
        monedas += cantidad;
        ActualizarTextoMonedas();
    }

    public void RestarVida(int cantidad = 1)
    {
        vidas -= cantidad;

        if (vidas < 0)
            vidas = 0;

        ActualizarTextoVidas();
    }

    private void ActualizarTextoMonedas()
    {
        if (textoMonedas != null)
        {
            textoMonedas.text = ": " + monedas;
        }
    }

    private void ActualizarTextoVidas()
    {
        if (textoVidas != null)
        {
            textoVidas.text = ": " + vidas;
        }
    }

    public void ReiniciarVidas(int cantidad)
    {
        vidas = cantidad;
        ActualizarTextoVidas();
    }

}