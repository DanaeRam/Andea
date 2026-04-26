using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LectoGameSessionManager : MonoBehaviour
{
    public static LectoGameSessionManager Instance;

    [Header("Banco de escenas por nivel")]
    public LevelSceneBank basicoSceneBank;
    public LevelSceneBank intermedioSceneBank;
    public LevelSceneBank avanzadoSceneBank;

    [Header("Configuración de partida")]
    public int roundsPerSession = 6;
    public string quizSceneName = "SceneQuizLE";
    public string resultsSceneName = "ResultadosNivel";

    [Header("Estado actual")]
    public string currentLevelName;
    public string currentLessonName;
    public int currentRoundIndex = 0;

    private List<string> selectedScenesThisSession = new List<string>();
    private List<LessonQuestionData> selectedQuestionsThisSession = new List<LessonQuestionData>();

    public LessonQuestionData CurrentQuestion
    {
        get
        {
            if (currentRoundIndex < 0 || currentRoundIndex >= selectedQuestionsThisSession.Count)
                return null;

            return selectedQuestionsThisSession[currentRoundIndex];
        }
    }

    public string CurrentPlatformScene
    {
        get
        {
            if (currentRoundIndex < 0 || currentRoundIndex >= selectedScenesThisSession.Count)
                return null;

            return selectedScenesThisSession[currentRoundIndex];
        }
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void StartLessonSession(string levelName, string lessonName)
    {
        currentLevelName = levelName;
        currentLessonName = lessonName;
        currentRoundIndex = 0;

        selectedScenesThisSession.Clear();
        selectedQuestionsThisSession.Clear();

        LevelSceneBank sceneBank = GetSceneBank(levelName);
        string jsonPath = GetQuestionBankJsonPath(levelName, lessonName);

        if (sceneBank == null)
        {
            Debug.LogError("No se encontró banco de escenas para el nivel: " + levelName);
            return;
        }

        if (string.IsNullOrEmpty(jsonPath))
        {
            Debug.LogError("No se encontró ruta JSON para la lección: " + lessonName);
            return;
        }

        LessonQuestionBankJson questionBank = QuestionBankJsonLoader.LoadQuestionBank(jsonPath);

        if (questionBank == null)
        {
            Debug.LogError("No se pudo cargar el banco de preguntas JSON.");
            return;
        }

        if (sceneBank.platformSceneNames == null || sceneBank.platformSceneNames.Length < roundsPerSession)
        {
            Debug.LogError("No hay suficientes escenas para armar la partida.");
            return;
        }

        if (questionBank.questions == null || questionBank.questions.Length < roundsPerSession)
        {
            Debug.LogError("No hay suficientes preguntas para armar la partida.");
            return;
        }

        if (GameManager.instancia != null)
            GameManager.instancia.IniciarNuevaSesion();

        selectedScenesThisSession = GetRandomUniqueScenes(sceneBank.platformSceneNames, roundsPerSession);
        selectedQuestionsThisSession = GetRandomUniqueQuestions(questionBank.questions, roundsPerSession);

        SceneManager.LoadScene(selectedScenesThisSession[currentRoundIndex]);
    }

    public void LoadQuizSceneForCurrentRound()
    {
        SceneManager.LoadScene(quizSceneName);
    }

    public void GoToNextRoundOrResults()
    {
        currentRoundIndex++;

        if (currentRoundIndex >= roundsPerSession)
        {
            StartCoroutine(FinalizarSesionYMostrarResultados());
        }
        else
        {
            SceneManager.LoadScene(selectedScenesThisSession[currentRoundIndex]);
        }
    }

    private IEnumerator FinalizarSesionYMostrarResultados()
    {
        if (GameManager.instancia != null)
        {
            yield return StartCoroutine(
                GameManager.instancia.CompletarNivelCoroutine()
            );
        }

        SceneManager.LoadScene(resultsSceneName);
    }

    private LevelSceneBank GetSceneBank(string levelName)
    {
        switch (levelName)
        {
            case "Basico": return basicoSceneBank;
            case "Intermedio": return intermedioSceneBank;
            case "Avanzado": return avanzadoSceneBank;
            default: return null;
        }
    }

    private string GetQuestionBankJsonPath(string levelName, string lessonName)
    {
        if (levelName == "Basico" && lessonName == "Prueba1")
            return "QuestionsBank/BasicoL1";

        if (levelName == "Basico" && lessonName == "Prueba2")
            return "QuestionsBank/BasicoL2";

        if (levelName == "Basico" && lessonName == "Prueba3")
            return "QuestionsBank/BasicoL3";

        if (levelName == "Intermedio" && lessonName == "Identificar sustantivos, verbos y adjetivos")
            return "QuestionsBank/IntermedioL1";

        if (levelName == "Intermedio" && lessonName == "Sinónimos y Antónimos")
            return "QuestionsBank/IntermedioL2";

        if (levelName == "Avanzado" && lessonName == "Uso correcto de signos de puntuación")
            return "QuestionsBank/AvanzadoL1";

        if (levelName == "Avanzado" && lessonName == "Comprensión Lectora")
            return "QuestionsBank/AvanzadoL2";

        if (levelName == "Avanzado" && lessonName == "Identificar la idea principal")
            return "QuestionsBank/AvanzadoL3";

        if (levelName == "Avanzado" && lessonName == "Inferencias de un texto")
            return "QuestionsBank/AvanzadoL4";

        return null;
    }

    private List<string> GetRandomUniqueScenes(string[] source, int amount)
    {
        List<string> pool = new List<string>(source);
        Shuffle(pool);
        return pool.GetRange(0, amount);
    }

    private List<LessonQuestionData> GetRandomUniqueQuestions(LessonQuestionData[] source, int amount)
    {
        List<LessonQuestionData> pool = new List<LessonQuestionData>(source);
        Shuffle(pool);
        return pool.GetRange(0, amount);
    }

    private void Shuffle<T>(List<T> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int randomIndex = Random.Range(i, list.Count);
            T temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
}