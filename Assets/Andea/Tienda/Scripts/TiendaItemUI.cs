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

    private TiendaSimple.ItemData itemData;
    private TiendaSimple tienda;

    public void Configurar(TiendaSimple.ItemData data, TiendaSimple tiendaRef)
    {
        itemData = data;
        tienda = tiendaRef;

        if (iconoItem != null)
            iconoItem.sprite = data.icono;

        if (textoNombre != null)
            textoNombre.text = data.nombre;

        if (textoCosto != null)
            textoCosto.text = data.costo.ToString();

        if (botonComprar != null)
        {
            botonComprar.onClick.RemoveAllListeners();
            botonComprar.onClick.AddListener(Comprar);
        }
    }

    private void Comprar()
    {
        if (tienda != null && itemData != null)
        {
            tienda.IntentarComprar(itemData);
        }
    }
}