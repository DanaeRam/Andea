using UnityEngine;

public class InventarioPanelUI : MonoBehaviour
{
    public GameObject panelInventario;

    private void Start()
    {
        if (panelInventario != null)
            panelInventario.SetActive(false);
    }

    public void AbrirInventario()
    {
        if (panelInventario != null)
            panelInventario.SetActive(true);
    }

    public void CerrarInventario()
    {
        if (panelInventario != null)
            panelInventario.SetActive(false);
    }
}