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

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (placedCorrectly) return;

        originalParent = transform.parent;
        canvasGroup.blocksRaycasts = false;

        transform.SetParent(canvas.transform);
        rectTransform.sizeDelta = bigSize;
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
            transform.SetParent(originalParent);
            rectTransform.sizeDelta = smallSize;
        }
    }

    public void SnapToSlot(Transform slotTransform)
    {
        placedCorrectly = true;

        transform.SetParent(slotTransform, false);

        RectTransform rt = GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);

        rt.anchoredPosition = Vector2.zero;
        rt.localRotation = Quaternion.identity;
        rt.localScale = Vector3.one;
        rt.sizeDelta = bigSize;

        canvasGroup.blocksRaycasts = true;
    }

    public void ReturnToPanel()
    {
        transform.SetParent(originalParent);
        rectTransform.sizeDelta = smallSize;
        rectTransform.localScale = Vector3.one;
        canvasGroup.blocksRaycasts = true;
    }
}