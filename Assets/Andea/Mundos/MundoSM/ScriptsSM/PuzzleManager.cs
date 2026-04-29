using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    [Header("Slots del rompecabezas")]
    public PuzzleSlot[] slots;

    [Header("Progreso de galería")]
    public GalleryProgressManager galleryProgressManager;

    private bool puzzleAlreadyCompleted = false;

    public bool IsPuzzleComplete()
    {
        if (slots == null || slots.Length == 0)
        {
            Debug.LogError("PuzzleManager: No hay slots asignados.");
            return false;
        }

        foreach (PuzzleSlot slot in slots)
        {
            if (slot == null)
            {
                Debug.LogError("PuzzleManager: Hay un slot vacío en el array.");
                return false;
            }

            if (slot.transform.childCount == 0)
            {
                Debug.Log("PuzzleManager: Falta pieza en slot " + slot.slotIndex);
                return false;
            }

            PuzzlePiece piece = slot.transform.GetChild(0).GetComponent<PuzzlePiece>();

            if (piece == null)
            {
                Debug.LogError("PuzzleManager: El hijo del slot " + slot.slotIndex + " no tiene PuzzlePiece.");
                return false;
            }

            if (piece.correctIndex != slot.slotIndex)
            {
                Debug.Log(
                    "PuzzleManager: Pieza incorrecta en slot " + slot.slotIndex +
                    ". Pieza correctIndex: " + piece.correctIndex
                );

                return false;
            }
        }

        return true;
    }

    public void CheckPuzzleCompletion()
    {
        Debug.Log("PuzzleManager: Revisando si el rompecabezas está completo...");

        if (puzzleAlreadyCompleted)
        {
            Debug.Log("PuzzleManager: El rompecabezas ya estaba marcado como completado.");
            return;
        }

        if (!IsPuzzleComplete())
        {
            Debug.Log("PuzzleManager: El rompecabezas todavía no está completo.");
            return;
        }

        puzzleAlreadyCompleted = true;
        Debug.Log("PuzzleManager: ¡Rompecabezas completado!");

        if (galleryProgressManager == null)
        {
            Debug.LogError("PuzzleManager: GalleryProgressManager NO está asignado en el Inspector.");
            return;
        }

        Debug.Log("PuzzleManager: Llamando a GalleryProgressManager.MarkCurrentPuzzleCompleted().");
        galleryProgressManager.MarkCurrentPuzzleCompleted();
    }

    public void ResetPuzzleCompletedState()
    {
        puzzleAlreadyCompleted = false;
        Debug.Log("PuzzleManager: Estado de completado reiniciado.");
    }
}