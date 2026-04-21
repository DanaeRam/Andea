using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIHoverPulseEffect : MonoBehaviour, 
    IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Escala")]
    public float hoverScale = 1.08f;
    public float pressedScale = 0.94f;
    public float animationSpeed = 10f;

    [Header("Pulso automático")]
    public bool usePulse = true;
    public float pulseAmount = 0.04f;
    public float pulseSpeed = 2f;

    [Header("Color opcional")]
    public bool changeColor = true;
    public Color normalColor = Color.white;
    public Color hoverColor = new Color(1f, 0.95f, 0.65f, 1f);

    private RectTransform rectTransform;
    private Graphic graphic;

    private Vector3 originalScale;
    private float targetScale = 1f;
    private bool isHovering = false;
    private bool isPressed = false;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        graphic = GetComponent<Graphic>();

        if (rectTransform != null)
            originalScale = rectTransform.localScale;

        if (graphic != null && changeColor)
            graphic.color = normalColor;
    }

    private void Update()
    {
        if (rectTransform == null) return;

        float finalScale = targetScale;

        if (usePulse && !isPressed)
        {
            float pulse = Mathf.Sin(Time.unscaledTime * pulseSpeed) * pulseAmount;
            finalScale += pulse;
        }

        Vector3 desiredScale = originalScale * finalScale;

        rectTransform.localScale = Vector3.Lerp(
            rectTransform.localScale,
            desiredScale,
            Time.unscaledDeltaTime * animationSpeed
        );
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovering = true;
        targetScale = hoverScale;

        if (graphic != null && changeColor)
            graphic.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovering = false;
        isPressed = false;
        targetScale = 1f;

        if (graphic != null && changeColor)
            graphic.color = normalColor;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
        targetScale = pressedScale;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
        targetScale = isHovering ? hoverScale : 1f;
    }
}