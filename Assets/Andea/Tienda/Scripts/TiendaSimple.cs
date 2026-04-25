using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TiendaSimple : MonoBehaviour
{
    [System.Serializable]
    public class ItemData
    {
        public string nombre;
        public int costo;
        public Sprite icono;
    }

    [Header("Prefab y contenedor")]
    public GameObject itemPrefab;
    public Transform content;

    [Header("Artículos de tienda")]
    public List<ItemData> items = new List<ItemData>();

    [Header("Mensajes")]
    public TextMeshProUGUI textoEstadoTienda;

    private void Start()
    {
        GenerarItems();
    }

    private void GenerarItems()
    {
        if (itemPrefab == null || content == null)
        {
            Debug.LogError("TiendaSimple: falta itemPrefab o content.");
            return;
        }

        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < items.Count; i++)
        {
            GameObject nuevoItem = Instantiate(itemPrefab, content);
            TiendaItemUI itemUI = nuevoItem.GetComponent<TiendaItemUI>();

            if (itemUI == null)
            {
                Debug.LogError("El prefab de tienda no tiene TiendaItemUI.");
                continue;
            }

            itemUI.Configurar(items[i], this);
        }
    }

    public void IntentarComprar(ItemData item)
    {
        if (GameManager.instancia == null)
        {
            MostrarMensaje("No existe GameManager.");
            return;
        }

        bool comprado = GameManager.instancia.GastarMonedasTienda(item.costo);

        if (comprado)
        {
            MostrarMensaje("Compraste: " + item.nombre);
        }
        else
        {
            MostrarMensaje("No tienes suficientes runas.");
        }
    }

    public void MostrarMensaje(string mensaje)
    {
        if (textoEstadoTienda != null)
            textoEstadoTienda.text = mensaje;
    }
}