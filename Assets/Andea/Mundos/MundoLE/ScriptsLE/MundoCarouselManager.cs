using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MundoCarouselManager : MonoBehaviour
{
    public enum PlayMode
    {
        LectoRandom,     // Para LE: usa LectoGameSessionManager
        FixedScene,      // Para MA: carga flujo de escena fija
        None             // Para mundos sin botón Jugar
    }

    [Header("Mundo")]
    public string worldCode = "LE";

    [Header("Tipo de juego")]
    public PlayMode playMode = PlayMode.LectoRandom;

    [Tooltip("Si está apagado, todos los botones aparecen desbloqueados.")]
    public bool usarBloqueoPorProgreso = true;

    [Header("Carrusel")]
    public Image carouselImage;
    public Sprite[] levelSprites;

    [Header("Ribbon superior")]
    public TextMeshProUGUI ribbonText;

    [Header("Contenedores de botones por nivel")]
    public GameObject botonesBasico;
    public GameObject botonesIntermedio;
    public GameObject botonesAvanzado;

    [Header("Panel de lección")]
    public GameObject lessonPanel;
    public TextMeshProUGUI panelTitleText;
    public TextMeshProUGUI descriptionLessonText;

    [Header("Botones dentro del panel")]
    public Button closePanelButton;
    public Button learnButton;
    public Button playButton;
    public TextMeshProUGUI learnButtonText;
    public TextMeshProUGUI playButtonText;

    [Header("Bloqueo por progreso")]
    public Button[] botonesBasicoLecciones;
    public Button[] botonesIntermedioLecciones;
    public Button[] botonesAvanzadoLecciones;

    [Header("Candados opcionales")]
    public GameObject[] candadosBasico;
    public GameObject[] candadosIntermedio;
    public GameObject[] candadosAvanzado;

    [Header("Mensaje de bloqueo")]
    public TextMeshProUGUI textoMensajeBloqueo;

    [Header("Escena de aprendizaje")]
    public string lessonSceneName = "LessonScene";

    [Header("Matemáticas - Escena intermedia")]
    public string mathIntroSceneName = "BasicoTienda";
    public int mathMaxRounds = 6;

    [Header("Escenas fijas - Básico")]
    public string[] escenasBasico;

    [Header("Escenas fijas - Intermedio")]
    public string[] escenasIntermedio;

    [Header("Escenas fijas - Avanzado")]
    public string[] escenasAvanzado;

    private int currentIndex = 0;
    private int currentLesson = 0;

    private Dictionary<string, PlayerLessonsApi.LeccionEstado> estadoLecciones =
        new Dictionary<string, PlayerLessonsApi.LeccionEstado>();

    private void Start()
    {
        NormalizarWorldCode();

        UpdateCarousel();
        UpdateRibbonText();
        UpdateVisibleLessonButtons();
        UpdatePanelButtonsText();
        RegisterPanelButtons();

        if (lessonPanel != null)
            lessonPanel.SetActive(false);

        LimpiarMensajeBloqueo();
        CargarEstadoLecciones();
    }

    private void NormalizarWorldCode()
    {
        if (string.IsNullOrEmpty(worldCode))
            worldCode = "LE";

        worldCode = worldCode.Trim().ToUpper();
    }

    private void RegisterPanelButtons()
    {
        if (closePanelButton != null)
        {
            closePanelButton.onClick.RemoveAllListeners();
            closePanelButton.onClick.AddListener(CloseLessonPanel);
        }

        if (learnButton != null)
        {
            learnButton.onClick.RemoveAllListeners();
            learnButton.onClick.AddListener(OnLearnButton);
        }

        if (playButton != null)
        {
            playButton.onClick.RemoveAllListeners();
            playButton.onClick.AddListener(OnPlayButton);
        }
    }

    public void NextImage()
    {
        if (levelSprites == null || levelSprites.Length == 0)
            return;

        currentIndex = (currentIndex + 1) % levelSprites.Length;

        UpdateCarousel();
        UpdateRibbonText();
        UpdateVisibleLessonButtons();
        AplicarBloqueosVisuales();
        CloseLessonPanel();
        LimpiarMensajeBloqueo();
    }

    public void PreviousImage()
    {
        if (levelSprites == null || levelSprites.Length == 0)
            return;

        currentIndex--;

        if (currentIndex < 0)
            currentIndex = levelSprites.Length - 1;

        UpdateCarousel();
        UpdateRibbonText();
        UpdateVisibleLessonButtons();
        AplicarBloqueosVisuales();
        CloseLessonPanel();
        LimpiarMensajeBloqueo();
    }

    private void UpdateCarousel()
    {
        if (carouselImage != null && levelSprites != null && levelSprites.Length > 0)
        {
            carouselImage.sprite = levelSprites[currentIndex];
            carouselImage.preserveAspect = true;
        }
    }

    private void UpdateRibbonText()
    {
        if (ribbonText == null)
            return;

        string mundoTexto;

        if (worldCode == "MA")
            mundoTexto = "Matemáticas";
        else if (worldCode == "LE")
            mundoTexto = "Lecto-escritura";
        else if (worldCode == "SM")
            mundoTexto = "Salud mental";
        else
            mundoTexto = worldCode;

        switch (currentIndex)
        {
            case 0:
                ribbonText.text = mundoTexto + "\nNivel Básico";
                break;

            case 1:
                ribbonText.text = mundoTexto + "\nNivel Intermedio";
                break;

            case 2:
                ribbonText.text = mundoTexto + "\nNivel Avanzado";
                break;
        }
    }

    private void UpdateVisibleLessonButtons()
    {
        if (botonesBasico != null)
            botonesBasico.SetActive(currentIndex == 0);

        if (botonesIntermedio != null)
            botonesIntermedio.SetActive(currentIndex == 1);

        if (botonesAvanzado != null)
            botonesAvanzado.SetActive(currentIndex == 2);
    }

    private void UpdatePanelButtonsText()
    {
        if (learnButtonText != null)
            learnButtonText.text = "Aprender";

        if (playButtonText != null)
            playButtonText.text = "Jugar";

        if (playButton != null)
            playButton.gameObject.SetActive(playMode != PlayMode.None);
    }

    private void CargarEstadoLecciones()
    {
        if (!usarBloqueoPorProgreso)
        {
            DesbloquearTodosLosBotones();
            return;
        }

        if (PlayerLessonsApi.Instance == null)
        {
            Debug.LogWarning("No existe PlayerLessonsApi. No se aplicarán bloqueos desde servidor.");
            AplicarBloqueosVisuales();
            return;
        }

        StartCoroutine(PlayerLessonsApi.Instance.ObtenerEstadoLecciones(
            worldCode,
            (data) =>
            {
                estadoLecciones.Clear();

                foreach (var leccion in data.lecciones)
                {
                    estadoLecciones[leccion.leccion_id] = leccion;
                }

                AplicarBloqueosVisuales();
            },
            (error) =>
            {
                Debug.LogError("Error al obtener estado de lecciones de " + worldCode + ": " + error);
                MostrarMensajeBloqueo("No se pudo cargar el progreso de lecciones.");
                AplicarBloqueosVisuales();
            }
        ));
    }

    private void AplicarBloqueosVisuales()
    {
        if (!usarBloqueoPorProgreso)
        {
            DesbloquearTodosLosBotones();
            return;
        }

        ConfigurarBotonesNivel(botonesBasicoLecciones, candadosBasico, "BASICO");
        ConfigurarBotonesNivel(botonesIntermedioLecciones, candadosIntermedio, "INTERMEDIO");
        ConfigurarBotonesNivel(botonesAvanzadoLecciones, candadosAvanzado, "AVANZADO");
    }

    private void ConfigurarBotonesNivel(Button[] botones, GameObject[] candados, string prefijo)
    {
        if (botones == null)
            return;

        for (int i = 0; i < botones.Length; i++)
        {
            string leccionId = ConstruirLessonId(prefijo, i + 1);
            bool desbloqueada = EstaLeccionDesbloqueada(leccionId);

            if (botones[i] != null)
                botones[i].interactable = desbloqueada;

            if (candados != null && i < candados.Length && candados[i] != null)
                candados[i].SetActive(!desbloqueada);
        }
    }

    private void DesbloquearTodosLosBotones()
    {
        ActivarBotones(botonesBasicoLecciones, candadosBasico);
        ActivarBotones(botonesIntermedioLecciones, candadosIntermedio);
        ActivarBotones(botonesAvanzadoLecciones, candadosAvanzado);
    }

    private void ActivarBotones(Button[] botones, GameObject[] candados)
    {
        if (botones != null)
        {
            foreach (Button boton in botones)
            {
                if (boton != null)
                    boton.interactable = true;
            }
        }

        if (candados != null)
        {
            foreach (GameObject candado in candados)
            {
                if (candado != null)
                    candado.SetActive(false);
            }
        }
    }

    private string ConstruirLessonId(string prefijo, int numero)
    {
        string baseId = prefijo + "_" + numero.ToString("00");

        if (worldCode == "LE")
            return baseId;

        return worldCode + "_" + baseId;
    }

    private bool EstaLeccionDesbloqueada(string leccionId)
    {
        if (!usarBloqueoPorProgreso)
            return true;

        if (estadoLecciones.TryGetValue(leccionId, out PlayerLessonsApi.LeccionEstado estado))
            return estado.desbloqueada;

        return leccionId.Contains("BASICO");
    }

    private bool EstaLeccionCompletada(string leccionId)
    {
        if (estadoLecciones.TryGetValue(leccionId, out PlayerLessonsApi.LeccionEstado estado))
            return estado.completada;

        return false;
    }

    public void OpenLessonPanel(int lessonNumber)
    {
        currentLesson = lessonNumber;

        string leccionId = GetCurrentLessonId();

        if (!EstaLeccionDesbloqueada(leccionId))
        {
            MostrarMensajeDeBloqueoPorNivel();
            return;
        }

        LimpiarMensajeBloqueo();

        if (lessonPanel != null)
            lessonPanel.SetActive(true);

        UpdateLessonPanelContent();
    }

    private void UpdateLessonPanelContent()
    {
        string leccionId = GetCurrentLessonId();

        if (panelTitleText != null)
        {
            if (EstaLeccionCompletada(leccionId))
                panelTitleText.text = "Lección " + currentLesson + " - Completada";
            else
                panelTitleText.text = "Lección " + currentLesson;
        }

        if (descriptionLessonText != null)
            descriptionLessonText.text = GetCurrentLessonDescription();
    }

    private string GetCurrentLessonDescription()
    {
        if (worldCode == "MA")
            return GetMathLessonDescription();

        if (worldCode == "SM")
            return "Explora la lección " + currentLesson + " de salud mental.";

        switch (currentIndex)
        {
            case 0:
                switch (currentLesson)
                {
                    case 1:
                        return "Explora la lección 1 del nivel básico y comienza tu aventura de lecto-escritura.";
                    case 2:
                        return "Explora la lección 2 del nivel básico y sigue practicando habilidades iniciales.";
                    case 3:
                        return "Explora la lección 3 del nivel básico y refuerza lo aprendido.";
                    default:
                        return "Lección del nivel básico.";
                }

            case 1:
                switch (currentLesson)
                {
                    case 1:
                        return "Explora la lección 1 del nivel intermedio: Identificar sustantivos, verbos y adjetivos.";
                    case 2:
                        return "Explora la lección 2 del nivel intermedio: Sinónimos y Antónimos.";
                    default:
                        return "Lección del nivel intermedio.";
                }

            case 2:
                switch (currentLesson)
                {
                    case 1:
                        return "Explora la lección 1 del nivel avanzado: Uso correcto de signos de puntuación.";
                    case 2:
                        return "Explora la lección 2 del nivel avanzado: Comprensión Lectora.";
                    case 3:
                        return "Explora la lección 3 del nivel avanzado: Identificar la idea principal.";
                    case 4:
                        return "Explora la lección 4 del nivel avanzado: Inferencias de un texto.";
                    default:
                        return "Lección del nivel avanzado.";
                }
        }

        return "Lección.";
    }

    private string GetMathLessonDescription()
    {
        switch (currentIndex)
        {
            case 0:
                switch (currentLesson)
                {
                    case 1:
                        return "Explora la lección 1 del nivel básico: Fórmulas básicas.";
                    case 2:
                        return "Explora la lección 2 del nivel básico: Restas.";
                    case 3:
                        return "Explora la lección 3 del nivel básico: Sumas y restas.";
                    default:
                        return "Lección del nivel básico de matemáticas.";
                }

            case 1:
                switch (currentLesson)
                {
                    case 1:
                        return "Explora la lección 1 del nivel intermedio: Multiplicaciones.";
                    case 2:
                        return "Explora la lección 2 del nivel intermedio: Divisiones.";
                    default:
                        return "Lección del nivel intermedio de matemáticas.";
                }

            case 2:
                switch (currentLesson)
                {
                    case 1:
                        return "Explora la lección 1 del nivel avanzado: Sumas y multiplicaciones.";
                    case 2:
                        return "Explora la lección 2 del nivel avanzado: Sumas y divisiones.";
                    case 3:
                        return "Explora la lección 3 del nivel avanzado: Operaciones combinadas.";
                    default:
                        return "Lección del nivel avanzado de matemáticas.";
                }
        }

        return "Lección de matemáticas.";
    }

    public void CloseLessonPanel()
    {
        if (lessonPanel != null)
            lessonPanel.SetActive(false);
    }

    public void OnLearnButton()
    {
        string leccionId = GetCurrentLessonId();

        if (!EstaLeccionDesbloqueada(leccionId))
        {
            MostrarMensajeDeBloqueoPorNivel();
            return;
        }

        string levelName = GetCurrentLevelName();
        string lessonName = GetCurrentLessonName();

        if (string.IsNullOrEmpty(leccionId) ||
            string.IsNullOrEmpty(levelName) ||
            string.IsNullOrEmpty(lessonName))
        {
            Debug.LogError("No se pudo abrir la lección de aprendizaje. Datos inválidos.");
            return;
        }

        GuardarLeccionActual(leccionId, levelName, lessonName);

        SceneManager.LoadScene(lessonSceneName);
    }

    public void OnPlayButton()
    {
        string leccionId = GetCurrentLessonId();

        if (!EstaLeccionDesbloqueada(leccionId))
        {
            MostrarMensajeDeBloqueoPorNivel();
            return;
        }

        string levelName = GetCurrentLevelName();
        string lessonName = GetCurrentLessonName();

        if (string.IsNullOrEmpty(leccionId) ||
            string.IsNullOrEmpty(levelName) ||
            string.IsNullOrEmpty(lessonName))
        {
            Debug.LogError("No se pudo iniciar la lección. Datos inválidos.");
            return;
        }

        GuardarLeccionActual(leccionId, levelName, lessonName);

        switch (playMode)
        {
            case PlayMode.LectoRandom:
                IniciarLectoRandom(levelName, lessonName);
                break;

            case PlayMode.FixedScene:
                CargarEscenaFija();
                break;

            case PlayMode.None:
                Debug.Log("Este mundo no tiene modo Jugar configurado.");
                break;
        }
    }

    private void GuardarLeccionActual(string leccionId, string levelName, string lessonName)
    {
        PlayerPrefs.SetString("CurrentWorldCode", worldCode);
        PlayerPrefs.SetString("CurrentLessonId", leccionId);
        PlayerPrefs.SetString("CurrentLevelName", levelName);
        PlayerPrefs.SetString("CurrentLessonName", lessonName);
        PlayerPrefs.Save();

        Debug.Log(
            "Lección actual guardada -> mundo: " + worldCode +
            " | leccionId: " + leccionId +
            " | nivel: " + levelName +
            " | nombre: " + lessonName
        );
    }

    private void IniciarLectoRandom(string levelName, string lessonName)
    {
        if (LectoGameSessionManager.Instance != null)
        {
            LectoGameSessionManager.Instance.StartLessonSession(levelName, lessonName);
        }
        else
        {
            Debug.LogError("No existe LectoGameSessionManager.");
        }
    }

    private void CargarEscenaFija()
    {
        string sceneName = GetFixedSceneName();

        if (string.IsNullOrEmpty(sceneName))
        {
            Debug.LogError("No hay escena fija configurada para " + GetCurrentLessonId());
            return;
        }

        if (worldCode == "MA")
        {
            if (string.IsNullOrEmpty(mathIntroSceneName))
            {
                Debug.LogError("Math Intro Scene Name está vacío.");
                return;
            }

            MathRewardSessionData.Reset();

            bool yaCompletada = EstaLeccionCompletada(GetCurrentLessonId());
            PlayerPrefs.SetInt("CurrentLessonAlreadyCompleted", yaCompletada ? 1 : 0);
            PlayerPrefs.Save();

            MathSceneTransitionData.currentRound = 0;
            MathSceneTransitionData.maxRounds = mathMaxRounds;
            MathSceneTransitionData.exitMode = false;
            MathSceneTransitionData.targetSceneName = sceneName;

            Debug.Log(
                "MA iniciando tienda. Lección: " + GetCurrentLessonId() +
                " | Escena preguntas: " + sceneName +
                " | Escena intermedia: " + mathIntroSceneName +
                " | Ya completada: " + yaCompletada
            );

            SceneManager.LoadScene(mathIntroSceneName);
            return;
        }

        SceneManager.LoadScene(sceneName);
    }

    private string GetFixedSceneName()
    {
        string[] escenas = null;

        switch (currentIndex)
        {
            case 0:
                escenas = escenasBasico;
                break;

            case 1:
                escenas = escenasIntermedio;
                break;

            case 2:
                escenas = escenasAvanzado;
                break;
        }

        int index = currentLesson - 1;

        if (escenas == null || index < 0 || index >= escenas.Length)
            return "";

        return escenas[index];
    }

    private string GetCurrentLessonId()
    {
        switch (currentIndex)
        {
            case 0:
                return ConstruirLessonId("BASICO", currentLesson);

            case 1:
                return ConstruirLessonId("INTERMEDIO", currentLesson);

            case 2:
                return ConstruirLessonId("AVANZADO", currentLesson);

            default:
                return "";
        }
    }

    private string GetCurrentLevelName()
    {
        switch (currentIndex)
        {
            case 0:
                return "Basico";

            case 1:
                return "Intermedio";

            case 2:
                return "Avanzado";

            default:
                return "";
        }
    }

    private string GetCurrentLessonName()
    {
        if (worldCode == "MA")
        {
            switch (currentIndex)
            {
                case 0:
                    switch (currentLesson)
                    {
                        case 1: return "Matematicas Formulas Basicas";
                        case 2: return "Matematicas Restas";
                        case 3: return "Matematicas Sumas y Restas";
                    }
                    break;

                case 1:
                    switch (currentLesson)
                    {
                        case 1: return "Matematicas Multiplicacion";
                        case 2: return "Matematicas Division";
                    }
                    break;

                case 2:
                    switch (currentLesson)
                    {
                        case 1: return "Matematicas Sumas y Multiplicacion";
                        case 2: return "Matematicas Sumas y Division";
                        case 3: return "Matematicas Operaciones Combinadas";
                    }
                    break;
            }
        }

        if (worldCode == "SM")
        {
            return "Salud Mental Leccion " + currentLesson;
        }

        switch (currentIndex)
        {
            case 0:
                switch (currentLesson)
                {
                    case 1: return "Prueba1";
                    case 2: return "Prueba2";
                    case 3: return "Prueba3";
                }
                break;

            case 1:
                switch (currentLesson)
                {
                    case 1: return "Identificar sustantivos, verbos y adjetivos";
                    case 2: return "Sinónimos y Antónimos";
                }
                break;

            case 2:
                switch (currentLesson)
                {
                    case 1: return "Uso correcto de signos de puntuación";
                    case 2: return "Comprensión Lectora";
                    case 3: return "Identificar la idea principal";
                    case 4: return "Inferencias de un texto";
                }
                break;
        }

        return "";
    }

    private void MostrarMensajeDeBloqueoPorNivel()
    {
        if (currentIndex == 1)
        {
            MostrarMensajeBloqueo("Completa todas las lecciones de Básico para desbloquear Intermedio.");
        }
        else if (currentIndex == 2)
        {
            MostrarMensajeBloqueo("Completa todas las lecciones de Intermedio para desbloquear Avanzado.");
        }
        else
        {
            MostrarMensajeBloqueo("Esta lección está bloqueada.");
        }
    }

    private void MostrarMensajeBloqueo(string mensaje)
    {
        if (textoMensajeBloqueo != null)
            textoMensajeBloqueo.text = mensaje;

        Debug.Log("Bloqueo lección: " + mensaje);
    }

    private void LimpiarMensajeBloqueo()
    {
        if (textoMensajeBloqueo != null)
            textoMensajeBloqueo.text = "";
    }

    public void RefrescarEstadoDesdeServidor()
    {
        CargarEstadoLecciones();
    }
}
