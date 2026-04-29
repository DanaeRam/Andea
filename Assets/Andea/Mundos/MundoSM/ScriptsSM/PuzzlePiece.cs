using UnityEngine;
using UnityEngine.EventSystems;

public class PuzzlePiece : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public int correctIndex;

    private Transform originalParent;
    private RectTransform rectTransform;
    private Canvas canvas;
    private CanvasGroup canvasGroup;

    public Vector2 smallSize = new Vector2(100, 100);
    public Vector2 bigSize = new Vector2(256, 256);

    private bool placedCorrectly = false;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public bool IsPlacedCorrectly()
    {
        return placedCorrectly;
    }

    public void SetPlacedCorrectly(bool value)
    {
        placedCorrectly = value;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (placedCorrectly) return;

        originalParent = transform.parent;

        canvasGroup.blocksRaycasts = false;

        transform.SetParent(canvas.transform, true);
        rectTransform.sizeDelta = bigSize;
        rectTransform.localScale = Vector3.one;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (placedCorrectly) return;

        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (placedCorrectly) return;

        canvasGroup.blocksRaycasts = true;

        if (transform.parent == canvas.transform)
        {
            ReturnToPanel();
        }
    }

public void SnapToSlot(Transform slotTransform)
{
    placedCorrectly = true;

    transform.SetParent(slotTransform, false);

    rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
    rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
    rectTransform.pivot = new Vector2(0.5f, 0.5f);

    rectTransform.anchoredPosition = Vector2.zero;
    rectTransform.localRotation = Quaternion.identity;
    rectTransform.localScale = Vector3.one;

    RectTransform slotRect = slotTransform.GetComponent<RectTransform>();
    rectTransform.sizeDelta = slotRect.sizeDelta;

    canvasGroup.blocksRaycasts = true;
}

    public void ReturnToPanel()
    {
        if (originalParent == null) return;

        transform.SetParent(originalParent, false);

        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);

        rectTransform.anchoredPosition = Vector2.zero;
        rectTransform.localRotation = Quaternion.identity;
        rectTransform.localScale = Vector3.one;
        rectTransform.sizeDelta = smallSize;

        canvasGroup.blocksRaycasts = true;
    }
}