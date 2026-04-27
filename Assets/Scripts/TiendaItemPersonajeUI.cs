using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TiendaItemPersonajeUI : MonoBehaviour
{
    [Header("Datos del personaje")]
    [SerializeField] private string nombrePersonaje;
    [SerializeField] private int precioPersonaje;
    [SerializeField] private Sprite imagenPersonaje;

    [Header("Referencias UI")]
    [SerializeField] private Image iconoPersonaje;
    [SerializeField] private TMP_Text textoNombre;
    [SerializeField] private TMP_Text textoPrecio;
    [SerializeField] private Button botonComprar;

    [Header("Texto de estado general")]
    [SerializeField] private TMP_Text textoEstadoTienda;

    [Header("Configuración")]
    [SerializeField] private bool comprado = false;

    private void Awake()
    {
        if (botonComprar != null)
        {
            botonComprar.onClick.AddListener(ComprarPersonaje);
        }
    }

    private void Start()
    {
        ActualizarUI();
    }

    private void OnDestroy()
    {
        if (botonComprar != null)
        {
            botonComprar.onClick.RemoveListener(ComprarPersonaje);
        }
    }

    private void ActualizarUI()
    {
        if (iconoPersonaje != null)
        {
            iconoPersonaje.sprite = imagenPersonaje;
            iconoPersonaje.preserveAspect = true;
        }

        if (textoNombre != null)
        {
            textoNombre.text = nombrePersonaje;
        }

        if (textoPrecio != null)
        {
            textoPrecio.text = precioPersonaje.ToString() + " oro";
        }

        if (botonComprar != null)
        {
            botonComprar.interactable = !comprado;

            TMP_Text textoBoton = botonComprar.GetComponentInChildren<TMP_Text>();

            if (textoBoton != null)
            {
                textoBoton.text = comprado ? "Comprado" : "Comprar";
            }
        }
    }

    private void ComprarPersonaje()
    {
        if (comprado)
        {
            if (textoEstadoTienda != null)
            {
                textoEstadoTienda.text = "Este personaje ya fue comprado.";
            }

            return;
        }

        comprado = true;

        if (textoEstadoTienda != null)
        {
            textoEstadoTienda.text = "Compraste: " + nombrePersonaje;
        }

        ActualizarUI();

        Debug.Log("Personaje comprado: " + nombrePersonaje);
    }
}