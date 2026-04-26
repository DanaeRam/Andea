using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TiendaSimple : MonoBehaviour
{
    [System.Serializable]
    public class ItemData
    {
        public string idRecompensa;
        public string nombre;
        public int costo;
        public TiendaItemUI itemUI;
    }

    [Header("Artículos de tienda")]
    public List<ItemData> items = new List<ItemData>();

    [Header("Mensaje general de tienda")]
    public TextMeshProUGUI textoEstadoTienda;

    private void Start()
    {
        ConfigurarItems();

        if (textoEstadoTienda != null)
            textoEstadoTienda.text = "";
    }

    private void ConfigurarItems()
    {
        foreach (ItemData item in items)
        {
            if (item.itemUI != null)
                item.itemUI.Configurar(item, this);
            else
                Debug.LogWarning("Falta ItemUI en: " + item.idRecompensa);
        }
    }

    public void IntentarComprar(ItemData item)
    {
        if (item == null)
        {
            MostrarMensaje("Item inválido.");
            return;
        }

        if (PlayerStoreApi.Instance == null)
        {
            MostrarMensaje("Error de conexión.");
            return;
        }

        if (item.itemUI != null)
            item.itemUI.BloquearBoton();

        StartCoroutine(PlayerStoreApi.Instance.ComprarItem(
            item.idRecompensa,
            item.costo,
            onSuccess: (data) =>
            {
                if (GameManager.instancia != null)
                {
                    GameManager.instancia.ActualizarProgresoServidor(
                        data.jugador_id,
                        data.puntos_totales,
                        data.runas,
                        data.puntos_residuales
                    );
                }

                MostrarMensaje("Compraste: " + item.nombre);

                if (item.itemUI != null)
                    item.itemUI.SetComprado();
            },
            onError: (error) =>
            {
                MostrarMensaje("⚠ " + error);

                if (item.itemUI != null)
                    item.itemUI.SetDisponible();
            }
        ));
    }

    public void MostrarMensaje(string mensaje)
    {
        if (textoEstadoTienda != null)
            textoEstadoTienda.text = mensaje;

        Debug.Log("Mensaje tienda: " + mensaje);
    }
}