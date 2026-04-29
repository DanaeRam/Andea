using UnityEngine;

public class ImagenFlotanteEnLugar : MonoBehaviour
{
    [Header("Movimiento horizontal")]
    public float amplitudX = 18f;
    public float velocidadX = 1.5f;

    [Header("Movimiento vertical suave")]
    public float amplitudY = 8f;
    public float velocidadY = 1.2f;

    [Header("Pulso de tamaño")]
    public float fuerzaPulso = 0.035f;
    public float velocidadPulso = 1.4f;

    [Header("Debug")]
    public bool mostrarDebug = false;

    private RectTransform rect;
    private Vector2 posicionBase;
    private Vector3 escalaBase;
    private float tiempo;
    private bool animando;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        GuardarBase();
    }

    private void Start()
    {
        IniciarMovimiento();
    }

    private void OnEnable()
    {
        IniciarMovimiento();
    }

    private void Update()
    {
        if (!animando || rect == null)
            return;

        tiempo += Time.unscaledDeltaTime;

        float x = Mathf.Sin(tiempo * velocidadX) * amplitudX;
        float y = Mathf.Sin(tiempo * velocidadY) * amplitudY;
        float pulso = 1f + Mathf.Sin(tiempo * velocidadPulso) * fuerzaPulso;

        rect.anchoredPosition = posicionBase + new Vector2(x, y);
        rect.localScale = escalaBase * pulso;
    }

    private void GuardarBase()
    {
        if (rect == null)
            rect = GetComponent<RectTransform>();

        posicionBase = rect.anchoredPosition;
        escalaBase = rect.localScale;
    }

    public void IniciarMovimiento()
    {
        if (rect == null)
            rect = GetComponent<RectTransform>();

        gameObject.SetActive(true);
        enabled = true;

        GuardarBase();
        tiempo = 0f;
        animando = true;

        if (mostrarDebug)
            Debug.Log("Movimiento iniciado en: " + gameObject.name);
    }

    public void DetenerMovimiento()
    {
        animando = false;

        if (mostrarDebug)
            Debug.Log("Movimiento detenido en: " + gameObject.name);
    }
}