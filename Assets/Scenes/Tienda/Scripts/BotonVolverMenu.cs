using UnityEngine;
using UnityEngine.SceneManagement;

public class BotonVolverMenu : MonoBehaviour
{
    [SerializeField] private string nombreEscenaMenu = "MainScene";

    public void IrAlMenu()
    {
        Time.timeScale = 1f;
        AudioListener.pause = false;
        SceneManager.LoadScene(nombreEscenaMenu);
    }
}