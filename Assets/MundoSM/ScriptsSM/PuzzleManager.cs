using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    public PuzzleSlot[] slots;

    public bool IsPuzzleComplete()
    {
        foreach (PuzzleSlot slot in slots)
        {
            if (slot.transform.childCount == 0)
                return false;

            PuzzlePiece piece = slot.transform.GetChild(0).GetComponent<PuzzlePiece>();
            if (piece == null || piece.correctIndex != slot.slotIndex)
                return false;
        }

        return true;
    }
}