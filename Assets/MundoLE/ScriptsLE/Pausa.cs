using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;


public class Pausa : MonoBehaviour
{
     public bool estaPausado = false;
    private VisualElement pausa;
    private Button btnContinuar;
    void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;
        pausa = root.Q<VisualElement>("Pausa");
        pausa.style.visibility = Visibility.Hidden;

        btnContinuar = root.Q<Button>("BotonContinuar");
        btnContinuar.clicked += Pausar;
    }

    void OnDisable()
    {
        btnContinuar.clicked -= Pausar;
    }
    public void Pausar()
    {
        estaPausado = !estaPausado;
        pausa.style.visibility = estaPausado ? Visibility.Visible : Visibility.Hidden;

        Time.timeScale = estaPausado ? 0 : 1;
    }
    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            Pausar();
        }
    }
}
