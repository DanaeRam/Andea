using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class Pausa : MonoBehaviour
{
    public bool estaPausado = false;

    private VisualElement pausa;
    private Button btnContinuar;
    private Button btnMenu;

    [SerializeField] private string nombreEscenaMenu = "MenuPrincipal";

    void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        pausa = root.Q<VisualElement>("Pausa");
        pausa.style.visibility = Visibility.Hidden;

        btnContinuar = root.Q<Button>("BotonContinuar");
        btnMenu = root.Q<Button>("BotonMenu");

        if (btnContinuar != null)
            btnContinuar.clicked += Pausar;

        if (btnMenu != null)
            btnMenu.clicked += IrAlMenu;
    }

    void OnDisable()
    {
        if (btnContinuar != null)
            btnContinuar.clicked -= Pausar;

        if (btnMenu != null)
            btnMenu.clicked -= IrAlMenu;
    }

    public void Pausar()
    {
        estaPausado = !estaPausado;
        pausa.style.visibility = estaPausado ? Visibility.Visible : Visibility.Hidden;

        Time.timeScale = estaPausado ? 0f : 1f;
        AudioListener.pause = estaPausado;
    }

    public void IrAlMenu()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        SceneManager.LoadScene(nombreEscenaMenu);
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Pausar();
        }
    }
}