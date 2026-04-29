using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SMGalleryLauncher : MonoBehaviour
{
    [Header("Escenas de salud mental")]
    public string painting1SceneName = "SMPainting1";
    public string painting2SceneName = "SMPainting2";
    public string painting3SceneName = "SMPainting3";
    public string painting4SceneName = "SMPainting4";
    public string storySceneName = "StoryPainting1";

    [Header("Escena principal")]
    public string mainSceneName = "MainScene";

    [Header("Configuración")]
    public bool abrirPrimeraNoCompletada = true;

    [Header("Claves de progreso local")]
    public string puzzle1Key = "SMPainting1";
    public string puzzle2Key = "SMPainting2";
    public string puzzle3Key = "SMPainting3";
    public string puzzle4Key = "SMPainting4";

    public void StartGalleryFromBeginning()
    {
        StartCoroutine(CargarProgresoYEntrar());
    }

    private IEnumerator CargarProgresoYEntrar()
    {
        yield return StartCoroutine(CargarProgresoDesdeBaseDeDatos());

        if (abrirPrimeraNoCompletada)
        {
            OpenFirstIncompletePainting();
        }
        else
        {
            OpenPainting1();
        }
    }

    private IEnumerator CargarProgresoDesdeBaseDeDatos()
    {
        if (PlayerLessonsApi.Instance == null)
        {
            Debug.LogWarning("No existe PlayerLessonsApi. Se usará progreso local de SM.");
            yield break;
        }

        Debug.Log("Cargando progreso de Salud Mental desde base de datos...");

        yield return StartCoroutine(
            PlayerLessonsApi.Instance.ObtenerEstadoLecciones(
                "SM",
                onSuccess: (data) =>
                {
                    // Reiniciamos las claves locales de SM antes de aplicar lo que viene de Supabase.
                    PlayerPrefs.SetInt(puzzle1Key, 0);
                    PlayerPrefs.SetInt(puzzle2Key, 0);
                    PlayerPrefs.SetInt(puzzle3Key, 0);
                    PlayerPrefs.SetInt(puzzle4Key, 0);

                    foreach (var leccion in data.lecciones)
                    {
                        if (!leccion.completada)
                            continue;

                        switch (leccion.leccion_id)
                        {
                            case "SM_LECCION_01":
                                PlayerPrefs.SetInt(puzzle1Key, 1);
                                break;

                            case "SM_LECCION_02":
                                PlayerPrefs.SetInt(puzzle2Key, 1);
                                break;

                            case "SM_LECCION_03":
                                PlayerPrefs.SetInt(puzzle3Key, 1);
                                break;

                            case "SM_LECCION_04":
                                PlayerPrefs.SetInt(puzzle4Key, 1);
                                break;
                        }
                    }

                    PlayerPrefs.Save();

                    Debug.Log(
                        "Progreso SM cargado. " +
                        "P1: " + PlayerPrefs.GetInt(puzzle1Key, 0) +
                        " | P2: " + PlayerPrefs.GetInt(puzzle2Key, 0) +
                        " | P3: " + PlayerPrefs.GetInt(puzzle3Key, 0) +
                        " | P4: " + PlayerPrefs.GetInt(puzzle4Key, 0)
                    );
                },
                onError: (error) =>
                {
                    Debug.LogError("Error al cargar progreso SM: " + error);
                }
            )
        );
    }

    private void OpenFirstIncompletePainting()
    {
        bool p1 = PlayerPrefs.GetInt(puzzle1Key, 0) == 1;
        bool p2 = PlayerPrefs.GetInt(puzzle2Key, 0) == 1;
        bool p3 = PlayerPrefs.GetInt(puzzle3Key, 0) == 1;
        bool p4 = PlayerPrefs.GetInt(puzzle4Key, 0) == 1;

        if (!p1)
        {
            OpenPainting1();
            return;
        }

        if (!p2)
        {
            OpenPainting2();
            return;
        }

        if (!p3)
        {
            OpenPainting3();
            return;
        }

        if (!p4)
        {
            OpenPainting4();
            return;
        }

        SceneManager.LoadScene(storySceneName);
    }

    public void OpenPainting1()
    {
        SaveCurrentLesson("SM_LECCION_01", "SaludMental", "Salud Mental Leccion 1");
        SceneManager.LoadScene(painting1SceneName);
    }

    public void OpenPainting2()
    {
        SaveCurrentLesson("SM_LECCION_02", "SaludMental", "Salud Mental Leccion 2");
        SceneManager.LoadScene(painting2SceneName);
    }

    public void OpenPainting3()
    {
        SaveCurrentLesson("SM_LECCION_03", "SaludMental", "Salud Mental Leccion 3");
        SceneManager.LoadScene(painting3SceneName);
    }

    public void OpenPainting4()
    {
        SaveCurrentLesson("SM_LECCION_04", "SaludMental", "Salud Mental Leccion 4");
        SceneManager.LoadScene(painting4SceneName);
    }

    public void BackToMainScene()
    {
        SceneManager.LoadScene(mainSceneName);
    }

    private void SaveCurrentLesson(string lessonId, string levelName, string lessonName)
    {
        PlayerPrefs.SetString("CurrentWorldCode", "SM");
        PlayerPrefs.SetString("CurrentLessonId", lessonId);
        PlayerPrefs.SetString("CurrentLevelName", levelName);
        PlayerPrefs.SetString("CurrentLessonName", lessonName);
        PlayerPrefs.Save();

        Debug.Log("SM lección seleccionada: " + lessonId);
    }
}