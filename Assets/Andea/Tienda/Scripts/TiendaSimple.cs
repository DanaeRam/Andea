using System.Collections;
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

    [Header("Fondo del mensaje general")]
    public GameObject fondoMensajeTienda;

    [Header("Ventana de espadas")]
    public GameObject panelEspadas;
    public GameObject botonMenuPrincipal;

    [Header("Tiempo visible del mensaje")]
    public float duracionMensaje = 2.5f;

    private Coroutine ocultarMensajeCoroutine;

    private void Awake()
    {
        OcultarMensajeInstantaneo();
    }

    private void OnEnable()
    {
        OcultarMensajeInstantaneo();
    }

    private IEnumerator Start()
    {
        ConfigurarItems();

        if (panelEspadas != null)
            panelEspadas.SetActive(false);

        if (botonMenuPrincipal != null)
            botonMenuPrincipal.SetActive(true);

        OcultarMensajeInstantaneo();

        LimpiarTextosEquipadosEnEscena();

        yield return null;

        LimpiarTextosEquipadosEnEscena();
        OcultarMensajeInstantaneo();
    }

    private void ConfigurarItems()
    {
        foreach (ItemData item in items)
        {
            AutoAsignarItemUI(item);

            if (item.itemUI != null)
                item.itemUI.Configurar(item, this);
            else
                Debug.LogWarning("Falta asignar ItemUI en: " + item.idRecompensa);
        }
    }

    private void AutoAsignarItemUI(ItemData item)
    {
        if (item == null || string.IsNullOrEmpty(item.idRecompensa))
            return;

        int numeroEspada = ObtenerNumeroEspada(item.idRecompensa);

        if (numeroEspada <= 0)
            return;

        string nombreEsperado = "ItemEspada" + numeroEspada;

        if (item.itemUI == null || item.itemUI.gameObject.name != nombreEsperado)
        {
            TiendaItemUI encontrado = BuscarItemUIEnEscena(nombreEsperado);

            if (encontrado != null)
                item.itemUI = encontrado;
        }
    }

    private int ObtenerNumeroEspada(string id)
    {
        string limpio = id.Replace("ESPADA_", "");

        if (int.TryParse(limpio, out int numero))
            return numero;

        return -1;
    }

    private TiendaItemUI BuscarItemUIEnEscena(string nombreObjeto)
    {
        TiendaItemUI[] todos = Resources.FindObjectsOfTypeAll<TiendaItemUI>();

        foreach (TiendaItemUI ui in todos)
        {
            if (ui == null)
                continue;

            if (!ui.gameObject.scene.IsValid())
                continue;

            if (ui.gameObject.name == nombreObjeto)
                return ui;
        }

        return null;
    }

    private void LimpiarTextosEquipadosEnEscena()
    {
        TextMeshProUGUI[] textos = Resources.FindObjectsOfTypeAll<TextMeshProUGUI>();

        foreach (TextMeshProUGUI texto in textos)
        {
            if (texto == null)
                continue;

            if (!texto.gameObject.scene.IsValid())
                continue;

            if (texto.text.Contains("Equipada"))
                texto.text = texto.text.Replace("Equipada", "Comprar");

            if (texto.text.Contains("Equipado"))
                texto.text = texto.text.Replace("Equipado", "Comprar");
        }
    }

    public void AbrirPanelEspadas()
    {
        if (panelEspadas != null)
            panelEspadas.SetActive(true);

        if (botonMenuPrincipal != null)
            botonMenuPrincipal.SetActive(false);

        OcultarMensajeInstantaneo();
    }

    public void CerrarPanelEspadas()
    {
        if (panelEspadas != null)
            panelEspadas.SetActive(false);

        if (botonMenuPrincipal != null)
            botonMenuPrincipal.SetActive(true);

        OcultarMensajeInstantaneo();
    }

    public void IntentarComprar(ItemData item)
    {
        if (item == null)
        {
            MostrarMensaje("Item inválido.");
            return;
        }

        if (string.IsNullOrEmpty(item.idRecompensa))
        {
            MostrarMensaje("Esta espada no tiene ID.");
            return;
        }

        if (PlayerStoreApi.Instance == null)
        {
            MostrarMensaje("Error: PlayerStoreApi no existe en la escena.");
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
                MostrarMensaje(error);

                if (item.itemUI != null)
                {
                    if (error.Contains("Ya tienes"))
                        item.itemUI.SetComprado();
                    else if (error.Contains("Recompensa no encontrada") || error.Contains("inactiva"))
                        item.itemUI.SetNoDisponible();
                    else
                        item.itemUI.SetDisponible();
                }
            }
        ));
    }

    public void MostrarMensaje(string mensaje)
    {
        if (textoEstadoTienda != null)
        {
            textoEstadoTienda.text = mensaje;
            textoEstadoTienda.gameObject.SetActive(true);
        }

        if (fondoMensajeTienda != null)
            fondoMensajeTienda.SetActive(true);

        Debug.Log("Mensaje tienda: " + mensaje);

        if (ocultarMensajeCoroutine != null)
            StopCoroutine(ocultarMensajeCoroutine);

        ocultarMensajeCoroutine = StartCoroutine(OcultarMensajeDespues());
    }

    private IEnumerator OcultarMensajeDespues()
    {
        yield return new WaitForSeconds(duracionMensaje);

        OcultarMensajeInstantaneo();
    }

    public void OcultarMensajeInstantaneo()
    {
        if (ocultarMensajeCoroutine != null)
        {
            StopCoroutine(ocultarMensajeCoroutine);
            ocultarMensajeCoroutine = null;
        }

        if (textoEstadoTienda != null)
        {
            textoEstadoTienda.text = "";
            textoEstadoTienda.gameObject.SetActive(false);
        }

        if (fondoMensajeTienda != null)
            fondoMensajeTienda.SetActive(false);
    }
}