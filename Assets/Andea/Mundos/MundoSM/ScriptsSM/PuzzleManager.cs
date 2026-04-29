using UnityEngine;

public class PuzzleManager : MonoBehaviour
{
    [Header("Contenedores")]
    public Transform boardPanel;
    public Transform piecesPanel;

    [Header("Progreso")]
    public GalleryProgressManager galleryProgressManager;

    private PuzzleSlot[] slots;
    private PuzzlePiece[] pieces;

    private bool puzzleAlreadyCompleted = false;

    private void Start()
    {
        SetupPuzzle();
    }

    private void SetupPuzzle()
    {
        slots = boardPanel.GetComponentsInChildren<PuzzleSlot>(true);
        pieces = piecesPanel.GetComponentsInChildren<PuzzlePiece>(true);

        for (int i = 0; i < slots.Length; i++)
        {
            slots[i].slotIndex = i;
            slots[i].puzzleManager = this;
        }

        for (int i = 0; i < pieces.Length; i++)
        {
            pieces[i].correctIndex = i;
            pieces[i].SetPlacedCorrectly(false);
        }

        if (slots.Length != pieces.Length)
        {
            Debug.LogWarning("El número de slots y piezas no coincide.");
        }

        Debug.Log("Slots encontrados: " + slots.Length);
        Debug.Log("Piezas encontradas: " + pieces.Length);
    }

    public bool IsPuzzleComplete()
    {
        if (slots == null || slots.Length == 0)
            return false;

        foreach (PuzzleSlot slot in slots)
        {
            if (slot.transform.childCount == 0)
                return false;

            PuzzlePiece piece = slot.transform.GetChild(0).GetComponent<PuzzlePiece>();

            if (piece == null)
                return false;

            if (piece.correctIndex != slot.slotIndex)
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