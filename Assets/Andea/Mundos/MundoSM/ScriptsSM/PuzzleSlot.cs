using UnityEngine;
using UnityEngine.EventSystems;

public class PuzzleSlot : MonoBehaviour, IDropHandler
{
    public int slotIndex;
    public PuzzleManager puzzleManager;

    public void OnDrop(PointerEventData eventData)
    {
        PuzzlePiece piece = eventData.pointerDrag.GetComponent<PuzzlePiece>();

        if (piece != null)
        {
            if (piece.correctIndex == slotIndex)
            {
                piece.SnapToSlot(transform);

                if (puzzleManager != null)
                {
                    puzzleManager.CheckPuzzleCompletion();
                }
            }
            else
            {
                piece.ReturnToPanel();
            }
        }
    }
}