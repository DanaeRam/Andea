using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventarioEspadasUI : MonoBehaviour
{
    [System.Serializable]
    public class EspadaInventarioUI
    {
        public string idRecompensa;
        public GameObject objetoEspada;
        public Button botonEspada;
        public TextMeshProUGUI textoEstado;
    }

    [Header("Espadas del inventario")]
    public List<EspadaInventarioUI> espadas = new List<EspadaInventarioUI>();

    [Header("Mensaje general")]
    public TextMeshProUGUI textoMensaje;

    private void Start()
    {
        OcultarTodas();
        ConfigurarBotones();
        CargarInventario();
    }

    private void OcultarTodas()
    {
        foreach (var espada in espadas)
        {
            if (espada.objetoEspada != null)
                espada.objetoEspada.SetActive(false);

            if (espada.textoEstado != null)
                espada.textoEstado.text = "";
        }
    }

    private void ConfigurarBotones()
    {
        foreach (var espada in espadas)
        {
            if (espada.botonEspada != null)
            {
                string id = espada.idRecompensa;
                espada.botonEspada.onClick.RemoveAllListeners();
                espada.botonEspada.onClick.AddListener(() => EquiparEspada(id));
            }
        }
    }

    private void CargarInventario()
    {
        if (PlayerInventoryApi.Instance == null)
        {
            Debug.LogError("No existe PlayerInventoryApi.");
            return;
        }

        StartCoroutine(PlayerInventoryApi.Instance.ObtenerInventario(
            (data) =>
            {
                MostrarEspadasCompradas(data.items);
            },
            (error) =>
            {
                Debug.LogError("Error inventario: " + error);
            }
        ));
    }

    private void MostrarEspadasCompradas(PlayerInventoryApi.InventarioItem[] itemsComprados)
    {
        OcultarTodas();

        foreach (var item in itemsComprados)
        {
            foreach (var espada in espadas)
            {
                if (espada.idRecompensa == item.recompensa_id)
                {
                    if (espada.objetoEspada != null)
                        espada.objetoEspada.SetActive(true);

                    if (espada.textoEstado != null)
                        espada.textoEstado.text = item.equipada ? "Equipada" : "";

                    if (item.equipada)
                    {
                        PlayerPrefs.SetString("EquippedSwordId", item.recompensa_id);
                        PlayerPrefs.SetString("EquippedSwordName", item.nombre);
                        PlayerPrefs.Save();

                        AplicarEspadaAlJugador(item.recompensa_id);
                    }
                }
            }
        }
    }

    public void EquiparEspada(string idRecompensa)
    {
        if (PlayerInventoryApi.Instance == null)
        {
            MostrarMensaje("Error de conexión.");
            return;
        }

        StartCoroutine(PlayerInventoryApi.Instance.EquiparEspada(
            idRecompensa,
            (data) =>
            {
                MostrarMensaje(data.nombre + " equipada.");

                PlayerPrefs.SetString("EquippedSwordId", data.recompensa_id);
                PlayerPrefs.SetString("EquippedSwordName", data.nombre);
                PlayerPrefs.Save();

                AplicarEspadaAlJugador(data.recompensa_id);

                foreach (var espada in espadas)
                {
                    if (espada.textoEstado != null)
                        espada.textoEstado.text = "";

                    if (espada.idRecompensa == data.recompensa_id && espada.textoEstado != null)
                        espada.textoEstado.text = "Equipada";
                }
            },
            (error) =>
            {
                MostrarMensaje(error);
            }
        ));
    }

    private void AplicarEspadaAlJugador(string idRecompensa)
    {
        PlayerSwordManager swordManager = FindFirstObjectByType<PlayerSwordManager>();

        if (swordManager != null)
        {
            swordManager.ApplySwordById(idRecompensa);
        }
        else
        {
            Debug.Log("No hay PlayerSwordManager en esta escena. Se guardó la espada para aplicarla después.");
        }
    }

    private void MostrarMensaje(string mensaje)
    {
        if (textoMensaje != null)
            textoMensaje.text = mensaje;

        Debug.Log("Inventario: " + mensaje);
    }
}
