using UnityEngine;
using UnityEngine.UI;

public class TiendaNavegacionPaneles : MonoBehaviour
{
    [Header("Paneles")]
    [SerializeField] private GameObject panelArticulos;
    [SerializeField] private GameObject panelPersonajes;
    [SerializeField] private GameObject panelDialogo;

    [Header("Botones para abrir")]
    [SerializeField] private Button botonAbrirArticulos;
    [SerializeField] private Button botonAbrirPersonajes;

    [Header("Botones para cerrar")]
    [SerializeField] private Button botonCerrarArticulos;
    [SerializeField] private Button botonCerrarPersonajes;

    [Header("Configuración")]
    [SerializeField] private bool ocultarPanelesAlIniciar = true;

    private void Awake()
    {
        if (botonAbrirArticulos != null)
        {
            botonAbrirArticulos.onClick.AddListener(AbrirPanelArticulos);
        }

        if (botonAbrirPersonajes != null)
        {
            botonAbrirPersonajes.onClick.AddListener(AbrirPanelPersonajes);
        }

        if (botonCerrarArticulos != null)
        {
            botonCerrarArticulos.onClick.AddListener(CerrarPanelArticulos);
        }

        if (botonCerrarPersonajes != null)
        {
            botonCerrarPersonajes.onClick.AddListener(CerrarPanelPersonajes);
        }
    }

    private void Start()
    {
        if (ocultarPanelesAlIniciar)
        {
            if (panelArticulos != null)
            {
                panelArticulos.SetActive(false);
            }

            if (panelPersonajes != null)
            {
                panelPersonajes.SetActive(false);
            }

            if (panelDialogo != null)
            {
                panelDialogo.SetActive(false);
            }
        }
    }

    public void AbrirPanelArticulos()
    {
        CerrarTodosLosPaneles();

        if (panelArticulos != null)
        {
            panelArticulos.SetActive(true);
            panelArticulos.transform.SetAsLastSibling();
        }
        else
        {
            Debug.LogError("No asignaste PanelArticulos.");
        }
    }

    public void AbrirPanelPersonajes()
    {
        CerrarTodosLosPaneles();

        if (panelPersonajes != null)
        {
            panelPersonajes.SetActive(true);
            panelPersonajes.transform.SetAsLastSibling();
        }
        else
        {
            Debug.LogError("No asignaste PanelPersonajes.");
        }
    }

    public void CerrarPanelArticulos()
    {
        if (panelArticulos != null)
        {
            panelArticulos.SetActive(false);
        }
    }

    public void CerrarPanelPersonajes()
    {
        if (panelPersonajes != null)
        {
            panelPersonajes.SetActive(false);
        }
    }

    private void CerrarTodosLosPaneles()
    {
        if (panelArticulos != null)
        {
            panelArticulos.SetActive(false);
        }

        if (panelPersonajes != null)
        {
            panelPersonajes.SetActive(false);
        }

        if (panelDialogo != null)
        {
            panelDialogo.SetActive(false);
        }
    }
}