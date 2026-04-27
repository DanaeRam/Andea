using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class Pausa : MonoBehaviour
{
    public bool estaPausado = false;

    private UIDocument pausaDocument;
    private VisualElement pausa;
    private Button btnContinuar;
    private Button btnMenu;

    [SerializeField] private string nombreEscenaMenu = "MenuPrincipal";

    [SerializeField] private GameObject quizCanvas;

    void Start()
    {
        pausaDocument = GetComponent<UIDocument>();

        if (pausaDocument == null)
        {
            Debug.LogError("No hay UIDocument en el objeto de Pausa.");
            return;
        }

        PrepararUI();
        OcultarPausaTotalmente();
    }

    private void PrepararUI()
    {
        if (pausaDocument == null) return;

        var root = pausaDocument.rootVisualElement;

        pausa = root.Q<VisualElement>("Pausa");
        btnContinuar = root.Q<Button>("BotonContinuar");
        btnMenu = root.Q<Button>("BotonMenu");

        if (btnContinuar != null)
        {
            btnContinuar.clicked -= Reanudar;
            btnContinuar.clicked += Reanudar;
        }

        if (btnMenu != null)
        {
            btnMenu.clicked -= IrAlMenu;
            btnMenu.clicked += IrAlMenu;
        }
    }

    private bool QuizActivo()
    {
        return quizCanvas != null && quizCanvas.activeInHierarchy;
    }

    public void Pausar()
    {
        if (QuizActivo())
        {
            return;
        }

        if (estaPausado)
            Reanudar();
        else
            MostrarPausa();
    }

    private void MostrarPausa()
    {
        estaPausado = true;

        pausaDocument.enabled = true;
        PrepararUI();

        if (pausa != null)
        {
            pausa.style.display = DisplayStyle.Flex;
            pausa.pickingMode = PickingMode.Position;
        }

        Time.timeScale = 0f;
        AudioListener.pause = true;
    }

    public void Reanudar()
    {
        estaPausado = false;

        Time.timeScale = 1f;
        AudioListener.pause = false;

        OcultarPausaTotalmente();
    }

    private void OcultarPausaTotalmente()
    {
        if (pausa != null)
        {
            pausa.style.display = DisplayStyle.None;
            pausa.pickingMode = PickingMode.Ignore;
        }

        if (pausaDocument != null)
            pausaDocument.enabled = false;
    }

    public void IrAlMenu()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        SceneManager.LoadScene(nombreEscenaMenu);
    }

    void Update()
    {
        if (Keyboard.current == null) return;

        if (QuizActivo())
        {
            if (estaPausado)
                Reanudar();

            OcultarPausaTotalmente();
            return;
        }

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Pausar();
        }
    }
}