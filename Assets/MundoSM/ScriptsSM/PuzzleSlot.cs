using UnityEngine;
using UnityEngine.EventSystems;

public class PuzzleSlot : MonoBehaviour, IDropHandler
{
    public int slotIndex;

    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("Se soltó algo en el slot " + slotIndex);

        PuzzlePiece piece = eventData.pointerDrag.GetComponent<PuzzlePiece>();

        if (piece != null)
        {
            Debug.Log("La pieza tiene índice " + piece.correctIndex);

            if (piece.correctIndex == slotIndex)
            {
                Debug.Log("Pieza correcta, se pega");
                piece.SnapToSlot(transform);
            }
            else
            {
                Debug.Log("Pieza incorrecta, regresa");
                piece.ReturnToPanel();
            }
        }
    }
}