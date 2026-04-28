using UnityEngine;
using UnityEngine.SceneManagement;

public class SMGalleryLauncher : MonoBehaviour
{
    [Header("Escena inicial de la galería")]
    public string firstPuzzleSceneName = "SMPainting1";

    [Header("Claves de progreso")]
    public string puzzle1Key = "SMPainting1";
    public string puzzle2Key = "SMPainting2";
    public string puzzle3Key = "SMPainting3";
    public string puzzle4Key = "SMPainting4";

    public void StartGalleryFromBeginning()
    {
        PlayerPrefs.DeleteKey(puzzle1Key);
        PlayerPrefs.DeleteKey(puzzle2Key);
        PlayerPrefs.DeleteKey(puzzle3Key);
        PlayerPrefs.DeleteKey(puzzle4Key);
        PlayerPrefs.Save();

        SceneManager.LoadScene(firstPuzzleSceneName);
    }
}