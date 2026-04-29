using System.Collections.Generic;
using UnityEngine;

public class ShufflePieces : MonoBehaviour
{
    [Header("Slots del panel derecho")]
    public List<RectTransform> pieceSlots = new List<RectTransform>();

    [Header("Piezas")]
    public List<RectTransform> pieces = new List<RectTransform>();

    [Header("Tamaño pequeño de las piezas")]
    public Vector2 smallSize = new Vector2(100, 100);

    private void Start()
    {
        Shuffle_Pieces();
    }

    public void Shuffle_Pieces()
    {
        List<RectTransform> availableSlots = new List<RectTransform>(pieceSlots);

        foreach (RectTransform piece in pieces)
        {
            if (availableSlots.Count == 0) return;

            int randomIndex = Random.Range(0, availableSlots.Count);
            RectTransform selectedSlot = availableSlots[randomIndex];

            piece.SetParent(selectedSlot, false);

            piece.anchorMin = new Vector2(0.5f, 0.5f);
            piece.anchorMax = new Vector2(0.5f, 0.5f);
            piece.pivot = new Vector2(0.5f, 0.5f);

            piece.anchoredPosition = Vector2.zero;
            piece.localRotation = Quaternion.identity;
            piece.localScale = Vector3.one;
            piece.sizeDelta = smallSize;

            availableSlots.RemoveAt(randomIndex);
        }
    }
}