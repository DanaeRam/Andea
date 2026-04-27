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

        if (itemData != null)
        {
            itemData.idRecompensa = itemData.idRecompensa.Trim();
            itemData.nombre = itemData.nombre.Trim();
        }

        AutoAsignarReferencias();
        DesactivarScriptsViejos();

        if (textoNombre != null)
            textoNombre.text = itemData.nombre;

        if (textoCosto != null)
            textoCosto.text = itemData.costo.ToString();

        if (botonComprar != null)
        {
            botonComprar.onClick.RemoveAllListeners();
            botonComprar.onClick.AddListener(Comprar);
            botonComprar.interactable = true;
        }

        SetDisponible();

        Debug.Log(
            "TiendaItemUI configurado: " +
            gameObject.name +
            " -> [" +
            itemData.idRecompensa +
            "] | " +
            itemData.nombre
        );
    }

    private void AutoAsignarReferencias()
    {
        if (botonComprar == null)
            botonComprar = GetComponentInChildren<Button>(true);

        if (textoEstado == null && botonComprar != null)
            textoEstado = botonComprar.GetComponentInChildren<TextMeshProUGUI>(true);

        if (textoEstado == null)
        {
            TextMeshProUGUI[] textos = GetComponentsInChildren<TextMeshProUGUI>(true);

            foreach (TextMeshProUGUI texto in textos)
            {
                if (texto.text.Contains("Comprar") ||
                    texto.text.Contains("Comprado") ||
                    texto.text.Contains("Equipada") ||
                    texto.text.Contains("Equipado") ||
                    texto.text.Contains("No disponible"))
                {
                    textoEstado = texto;
                    break;
                }
            }
        }

        if (textoNombre == null)
        {
            TextMeshProUGUI[] textos = GetComponentsInChildren<TextMeshProUGUI>(true);

            foreach (TextMeshProUGUI texto in textos)
            {
                string nombreObjeto = texto.gameObject.name.ToLower();

                if (nombreObjeto.Contains("nombre"))
                {
                    textoNombre = texto;
                    break;
                }
            }
        }

        if (textoCosto == null)
        {
            TextMeshProUGUI[] textos = GetComponentsInChildren<TextMeshProUGUI>(true);

            foreach (TextMeshProUGUI texto in textos)
            {
                string nombreObjeto = texto.gameObject.name.ToLower();

                if (nombreObjeto.Contains("precio") || nombreObjeto.Contains("costo"))
                {
                    textoCosto = texto;
                    break;
                }
            }
        }
    }

    private void DesactivarScriptsViejos()
    {
        MonoBehaviour[] scripts = GetComponentsInChildren<MonoBehaviour>(true);

        foreach (MonoBehaviour script in scripts)
        {
            if (script == null)
                continue;

            if (script == this)
                continue;

            if (script.GetType().Name == "ShopItemButton")
            {
                script.enabled = false;
                Debug.Log("Desactivado script viejo ShopItemButton en: " + script.gameObject.name);
            }
        }
    }

    private void Comprar()
    {
        if (tienda == null || itemData == null)
        {
            Debug.LogWarning("TiendaItemUI: falta tienda o itemData en " + gameObject.name);
            return;
        }

        itemData.idRecompensa = itemData.idRecompensa.Trim();

        Debug.Log(
            "BOTÓN PRESIONADO: " +
            gameObject.name +
            " -> [" +
            itemData.idRecompensa +
            "] | " +
            itemData.nombre
        );

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

        CambiarTextosViejos("Comprado");
    }

    public void SetDisponible()
    {
        if (botonComprar != null)
            botonComprar.interactable = true;

        if (textoEstado != null)
            textoEstado.text = "Comprar";

        CambiarTextosViejos("Comprar");
    }

    public void SetNoDisponible()
    {
        if (botonComprar != null)
            botonComprar.interactable = false;

        if (textoEstado != null)
            textoEstado.text = "No disponible";

        CambiarTextosViejos("No disponible");
    }

    private void CambiarTextosViejos(string nuevoTexto)
    {
        TextMeshProUGUI[] textos = GetComponentsInChildren<TextMeshProUGUI>(true);

        foreach (TextMeshProUGUI texto in textos)
        {
            if (texto == null)
                continue;

            if (texto.text.Contains("Equipada") ||
                texto.text.Contains("Equipado") ||
                texto.text.Contains("Comprado") ||
                texto.text.Contains("No disponible"))
            {
                texto.text = nuevoTexto;
            }
        }
    }
}