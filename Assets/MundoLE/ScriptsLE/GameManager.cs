using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instancia;

    [Header("Contador")]
    public int monedas = 0;

    [Header("UI")]
    public TextMeshProUGUI textoMonedas;

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
    }

    public void SumarMoneda(int cantidad = 1)
    {
        monedas += cantidad;
        ActualizarTextoMonedas();
    }

    private void ActualizarTextoMonedas()
    {
        if (textoMonedas != null)
        {
            textoMonedas.text = ": " + monedas;
        }
    }
}