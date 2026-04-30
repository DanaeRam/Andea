using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopItemButton : MonoBehaviour
{
    [Header("Identidad del item")]
    [SerializeField] private string itemId = "espada_1";
    [SerializeField] private string itemNombre = "Espada";

    [Header("Costo")]
    [SerializeField] private int costo = 120;

    [Header("UI del item")]
    [SerializeField] private Button botonComprar;
    [SerializeField] private TMP_Text textoBoton;
    [SerializeField] private TMP_Text textoPrecio;

    [Header("Mensaje general")]
    [SerializeField] private TMP_Text textoEstadoTienda;
    [SerializeField] private GameObject fondoMensaje;
    [SerializeField] private float duracionMensaje = 10f;

    [Header("Opcional")]
    [SerializeField] private GameObject selloComprado;
    [SerializeField] private bool equiparAutomaticamenteAlComprar = true;

    private Coroutine rutinaMensaje;

    private string PlayerPrefsKeyComprado => "shop_bought_" + itemId;
    private const string EquippedWeaponKey = "equipped_weapon_id";

    private void Start()
    {
        if (botonComprar == null)
            botonComprar = GetComponent<Button>();

        if (textoPrecio != null)
            textoPrecio.text = costo.ToString();

        if (botonComprar != null)
            botonComprar.onClick.AddListener(OnClickBoton);

        LimpiarMensajeInstantaneo();
        RefrescarEstadoVisual();
    }

    private void OnEnable()
    {
        LimpiarMensajeInstantaneo();
        RefrescarEstadoVisual();
    }

    private void OnDisable()
    {
        LimpiarMensajeInstantaneo();
    }

    private void OnClickBoton()
    {
        if (YaComprado())
        {
            Equipar();
            return;
        }

        Comprar();
    }

    public void Comprar()
    {
        if (GameManager.instancia == null)
        {
            MostrarMensajeTemporal("No existe GameManager en la escena.");
            return;
        }

        if (!GameManager.instancia.TieneMonedasTiendaSuficientes(costo))
        {
            MostrarMensajeTemporal("Monedas insuficientes");
            return;
        }

        bool compraExitosa = GameManager.instancia.GastarMonedasTienda(costo);

        if (!compraExitosa)
        {
            MostrarMensajeTemporal("Monedas insuficientes");
            return;
        }

        MarcarComoComprado();

        if (equiparAutomaticamenteAlComprar)
            EquiparInterno();

        MostrarMensajeTemporal($"{itemNombre} comprada con éxito.");
        RefrescarEstadoVisual();
    }

    public void Equipar()
    {
        if (!YaComprado())
        {
            MostrarMensajeTemporal($"Primero compra {itemNombre}.");
            return;
        }

        EquiparInterno();
        MostrarMensajeTemporal($"{itemNombre} equipada.");
        RefrescarEstadoVisual();
    }

    private void EquiparInterno()
    {
        PlayerPrefs.SetString(EquippedWeaponKey, itemId);
        PlayerPrefs.Save();
    }

    private bool YaComprado()
    {
        return PlayerPrefs.GetInt(PlayerPrefsKeyComprado, 0) == 1;
    }

    private bool EstaEquipada()
    {
        return PlayerPrefs.GetString(EquippedWeaponKey, "") == itemId;
    }

    private void MarcarComoComprado()
    {
        PlayerPrefs.SetInt(PlayerPrefsKeyComprado, 1);
        PlayerPrefs.Save();
    }

    private void RefrescarEstadoVisual()
    {
        bool comprado = YaComprado();
        bool equipada = EstaEquipada();

        if (textoBoton != null)
        {
            if (!comprado)
                textoBoton.text = "Comprar";
            else if (equipada)
                textoBoton.text = "Equipada";
            else
                textoBoton.text = "Equipar";
        }

        if (botonComprar != null)
            botonComprar.interactable = true;

        if (selloComprado != null)
            selloComprado.SetActive(comprado);
    }

    private void MostrarMensajeTemporal(string mensaje)
    {
        if (textoEstadoTienda == null)
        {
            Debug.Log(mensaje);
            return;
        }

        textoEstadoTienda.text = mensaje;

        if (fondoMensaje != null)
            fondoMensaje.SetActive(true);

        if (rutinaMensaje != null)
            StopCoroutine(rutinaMensaje);

        rutinaMensaje = StartCoroutine(LimpiarMensajeDespuesDeTiempo());
    }

    public void LimpiarMensajeInstantaneo()
    {
        if (rutinaMensaje != null)
        {
            StopCoroutine(rutinaMensaje);
            rutinaMensaje = null;
        }

        if (textoEstadoTienda != null)
            textoEstadoTienda.text = "";

        if (fondoMensaje != null)
            fondoMensaje.SetActive(false);
    }

    private IEnumerator LimpiarMensajeDespuesDeTiempo()
    {
        yield return new WaitForSeconds(duracionMensaje);

        if (textoEstadoTienda != null)
            textoEstadoTienda.text = "";

        if (fondoMensaje != null)
            fondoMensaje.SetActive(false);

        rutinaMensaje = null;
    }
}