using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TiendaDialogoAnciano : MonoBehaviour
{
    [Header("Panel de diálogo")]
    [SerializeField] private GameObject panelDialogo;

    [Header("Botones")]
    [SerializeField] private Button botonAnciano;
    [SerializeField] private Button botonCerrarDialogo;

    [Header("Texto del diálogo")]
    [SerializeField] private TMP_Text textoDialogo;

    [TextArea(2, 5)]
    [SerializeField] private string mensajeAnciano = "Bienvenido, viajero. Aquí encontrarás objetos para fortalecer tu aventura.";

    private void Awake()
    {
        if (botonAnciano != null)
        {
            botonAnciano.onClick.AddListener(AbrirDialogoAnciano);
        }

        if (botonCerrarDialogo != null)
        {
            botonCerrarDialogo.onClick.AddListener(CerrarDialogo);
        }
    }

    private void Start()
    {
        if (panelDialogo != null)
        {
            panelDialogo.SetActive(false);
        }
    }

    public void AbrirDialogoAnciano()
    {
        if (panelDialogo == null)
        {
            Debug.LogError("No asignaste PanelDialogo.");
            return;
        }

        panelDialogo.SetActive(true);
        panelDialogo.transform.SetAsLastSibling();

        if (textoDialogo != null)
        {
            textoDialogo.text = mensajeAnciano;
        }

        Debug.Log("Diálogo del anciano abierto.");
    }

    public void CerrarDialogo()
    {
        if (panelDialogo != null)
        {
            panelDialogo.SetActive(false);
        }
    }
}