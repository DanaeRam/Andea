using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TiendaItemUI : MonoBehaviour
{
    [Header("UI")]
    public Image iconoItem;
    public TextMeshProUGUI textoNombre;
    public TextMeshProUGUI textoCosto;
    public Button botonComprar;

    [Header("Texto del botón o estado")]
    public TextMeshProUGUI textoEstado;

    private TiendaSimple.ItemData itemData;
    private TiendaSimple tienda;

    public void Configurar(TiendaSimple.ItemData data, TiendaSimple tiendaRef)
    {
        itemData = data;
        tienda = tiendaRef;

        if (textoNombre != null)
            textoNombre.text = data.nombre;

        if (textoCosto != null)
            textoCosto.text = data.costo.ToString();

        if (botonComprar != null)
        {
            botonComprar.onClick.RemoveAllListeners();
            botonComprar.onClick.AddListener(Comprar);
            botonComprar.interactable = true;
        }

        SetDisponible();
    }

    private void Comprar()
    {
        if (tienda == null || itemData == null)
        {
            Debug.LogWarning("Falta tienda o itemData en " + gameObject.name);
            return;
        }

        tienda.IntentarComprar(itemData);
    }

    public void BloquearBoton()
    {
        if (botonComprar != null)
            botonComprar.interactable = false;
    }

    public void SetComprado()
    {
        if (botonComprar != null)
            botonComprar.interactable = false;

        if (textoEstado != null)
            textoEstado.text = "Comprado";
    }

    public void SetDisponible()
    {
        if (botonComprar != null)
            botonComprar.interactable = true;

        if (textoEstado != null)
            textoEstado.text = "Comprar";
    }
}