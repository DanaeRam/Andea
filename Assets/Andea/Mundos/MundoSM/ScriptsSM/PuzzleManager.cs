using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    public PuzzleSlot[] slots;
    public GalleryProgressManager galleryProgressManager;

    private bool puzzleAlreadyCompleted = false;

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

    public void CheckPuzzleCompletion()
    {
        if (puzzleAlreadyCompleted)
            return;

        if (IsPuzzleComplete())
        {
            puzzleAlreadyCompleted = true;
            Debug.Log("¡Rompecabezas completado!");

            if (galleryProgressManager != null)
            {
                galleryProgressManager.MarkCurrentPuzzleCompleted();
            }
        }
    }
}