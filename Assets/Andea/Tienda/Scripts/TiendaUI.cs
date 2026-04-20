using UnityEngine;
using UnityEngine.UI;

public class TiendaUI : MonoBehaviour
{
    public GameObject fondoPrincipal;
    public GameObject panelDialogo;
    public GameObject panelArticulos;
    public GameObject ButtonMain;

    void Start()
    {
        if (fondoPrincipal != null)
            fondoPrincipal.SetActive(true);

        if (panelDialogo != null)
            panelDialogo.SetActive(false);

        if (panelArticulos != null)
            panelArticulos.SetActive(false);
    }

    public void AbrirDialogo()
    {
        if (fondoPrincipal != null) fondoPrincipal.SetActive(false);
        if (panelDialogo != null) panelDialogo.SetActive(true);
        if(ButtonMain != null) ButtonMain.SetActive(false);

    }

    public void CerrarDialogo()
    {
        if (panelDialogo != null) panelDialogo.SetActive(false);
        if (fondoPrincipal != null) fondoPrincipal.SetActive(true);
        if(ButtonMain != null) ButtonMain.SetActive(true);
    }

    public void AbrirArticulos()
    {
        if (panelDialogo != null) panelDialogo.SetActive(false);
        if (panelArticulos != null) panelArticulos.SetActive(true);
        if(ButtonMain != null) ButtonMain.SetActive(false);
    }

    public void CerrarArticulos()
    {
        if (panelArticulos != null) panelArticulos.SetActive(false);
        if (fondoPrincipal != null) fondoPrincipal.SetActive(true);
        if(ButtonMain != null) ButtonMain.SetActive(true);
    }
}