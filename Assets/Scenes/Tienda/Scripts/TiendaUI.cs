using UnityEngine;

public class TiendaUI : MonoBehaviour
{
    public GameObject fondoPrincipal;
    public GameObject panelArticulos;
    public GameObject panelDialogo;
    public GameObject botonViejo;

    [Header("Mensaje tienda")]
    public ShopItemButton[] botonesTienda;

    private void Start()
    {
        if (fondoPrincipal != null)
            fondoPrincipal.SetActive(true);

        if (panelArticulos != null)
            panelArticulos.SetActive(false);

        if (panelDialogo != null)
            panelDialogo.SetActive(false);
    }

    public void AbrirArticulos()
    {
        if (fondoPrincipal != null)
            fondoPrincipal.SetActive(false);

        if (botonViejo != null)
            botonViejo.SetActive(false);

        if (panelDialogo != null)
            panelDialogo.SetActive(false);

        LimpiarMensajesTienda();

        if (panelArticulos != null)
            panelArticulos.SetActive(true);
    }

    public void CerrarArticulos()
    {
        LimpiarMensajesTienda();

        if (panelArticulos != null)
            panelArticulos.SetActive(false);

        if (fondoPrincipal != null)
            fondoPrincipal.SetActive(true);

        if (botonViejo != null)
            botonViejo.SetActive(true);
    }

    public void AbrirDialogoViejo()
    {
        if (panelDialogo != null)
            panelDialogo.SetActive(true);
    }

    public void CerrarDialogoViejo()
    {
        if (panelDialogo != null)
            panelDialogo.SetActive(false);
    }

    private void LimpiarMensajesTienda()
    {
        if (botonesTienda == null) return;

        for (int i = 0; i < botonesTienda.Length; i++)
        {
            if (botonesTienda[i] != null)
                botonesTienda[i].LimpiarMensajeInstantaneo();
        }
    }
}