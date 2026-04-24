using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instancia;

    [Header("Monedas normales del nivel")]
    public int monedas = 0;

    [Header("Vidas")]
    public int vidas = 3;
    public int vidasIniciales = 3;

    [Header("Recompensas locales del nivel")]
    public int puntos = 0;
    public int monedasTienda = 0;
    public int puntosNivelActual = 0;

    [Header("Resultado del último nivel")]
    public int ultimoPuntajeGanado = 0;
    public int ultimasMonedasGanadas = 0;
    public int puntosSobrantes = 0;

    [Header("Siguiente nivel")]
    public int siguienteNivelIndex = -1;

    [Header("Escena de resultados")]
    public string nombreEscenaResultados = "ResultadosNivel";

    [Header("Progreso sincronizado con servidor")]
    public long jugadorId = 0;
    public int puntosTotalesServidor = 0;
    public int runasServidor = 0;
    public int puntosResidualesServidor = 0;
    public bool progresoServidorCargado = false;

    [Header("UI")]
    public TextMeshProUGUI textoMonedas;
    public TextMeshProUGUI textoVidas;
    public TextMeshProUGUI textoPuntos;
    public TextMeshProUGUI textoMonedasTienda;

    private LevelRewardSystem rewardSystem;
    private bool sincronizandoProgreso = false;

    private void Awake()
    {
        if (instancia == null)
        {
            instancia = this;
            DontDestroyOnLoad(gameObject);

            rewardSystem = new LevelRewardSystem();
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (instancia == this)
            SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        ActualizarTodoElTexto();
        StartCoroutine(CargarProgresoInicialSiExisteCodigo());
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == nombreEscenaResultados)
            return;

        ReiniciarDatosDelNivel();
    }

    private IEnumerator CargarProgresoInicialSiExisteCodigo()
    {
        yield return null;

        string codigoJugador = PlayerPrefs.GetString("PlayerCode", "");

        if (string.IsNullOrEmpty(codigoJugador))
        {
            Debug.Log("No hay PlayerCode guardado todavía.");
            yield break;
        }

        if (PlayerProgressApi.Instance == null)
        {
            Debug.LogWarning("PlayerProgressApi no existe en la escena.");
            yield break;
        }

        yield return StartCoroutine(
            PlayerProgressApi.Instance.ObtenerProgreso(
                onSuccess: (data) =>
                {
                    if (data != null && data.ok)
                    {
                        ActualizarProgresoServidor(
                            data.jugador_id,
                            data.puntos_totales,
                            data.runas,
                            data.puntos_residuales
                        );

                        Debug.Log("Progreso inicial cargado correctamente.");
                    }
                    else
                    {
                        Debug.LogWarning("No se pudo cargar el progreso inicial.");
                    }
                },
                onError: (error) =>
                {
                    Debug.LogError("Error al cargar progreso inicial: " + error);
                }
            )
        );
    }

    private void ReiniciarDatosDelNivel()
    {
        monedas = 0;
        vidas = vidasIniciales;

        if (rewardSystem == null)
            rewardSystem = new LevelRewardSystem();

        rewardSystem.ResetLevelProgress();
        puntosNivelActual = 0;

        ActualizarTodoElTexto();
    }

    public void SumarMoneda(int cantidad = 1)
    {
        monedas += cantidad;
        ActualizarTextoMonedas();
    }

    public void RestarVida(int cantidad = 1)
    {
        vidas -= cantidad;

        if (vidas < 0)
            vidas = 0;

        ActualizarTextoVidas();
    }

    public void ReiniciarVidas(int cantidad)
    {
        vidas = cantidad;
        ActualizarTextoVidas();
    }

    public void RegistrarMonedaRecolectada()
    {
        if (rewardSystem == null)
            rewardSystem = new LevelRewardSystem();

        rewardSystem.RegisterCollectedCoin();
        SincronizarRecompensasLocales();
    }

    public void RegistrarEnemigoDerrotado()
    {
        if (rewardSystem == null)
            rewardSystem = new LevelRewardSystem();

        rewardSystem.RegisterEnemyDefeated();
        SincronizarRecompensasLocales();
    }

    public void CompletarNivel()
    {
        if (rewardSystem == null)
            rewardSystem = new LevelRewardSystem();

        bool completado = rewardSystem.CompleteLevel(true);

        if (!completado)
            return;

        ultimoPuntajeGanado = rewardSystem.LastPointsEarned;
        ultimasMonedasGanadas = rewardSystem.LastShopCoinsEarned;
        puntosSobrantes = rewardSystem.LastRemainingPoints;

        SincronizarRecompensasLocales();

        if (ultimoPuntajeGanado > 0)
        {
            StartCoroutine(SincronizarPuntosConServidor(ultimoPuntajeGanado, "nivel_completado"));
        }

        rewardSystem.ResetLevelProgress();
        puntosNivelActual = 0;
        ActualizarTodoElTexto();
    }

    public void CompletarActividad()
    {
        if (rewardSystem == null)
            rewardSystem = new LevelRewardSystem();

        bool completado = rewardSystem.CompleteActivity(true);

        if (!completado)
            return;

        ultimoPuntajeGanado = rewardSystem.LastPointsEarned;
        ultimasMonedasGanadas = rewardSystem.LastShopCoinsEarned;
        puntosSobrantes = rewardSystem.LastRemainingPoints;

        SincronizarRecompensasLocales();

        if (ultimoPuntajeGanado > 0)
        {
            StartCoroutine(SincronizarPuntosConServidor(ultimoPuntajeGanado, "actividad_completada"));
        }

        rewardSystem.ResetLevelProgress();
        puntosNivelActual = 0;
        ActualizarTodoElTexto();
    }

    private IEnumerator SincronizarPuntosConServidor(int puntosGanados, string motivo)
    {
        if (sincronizandoProgreso)
        {
            Debug.LogWarning("Ya hay una sincronización en curso.");
            yield break;
        }

        if (PlayerProgressApi.Instance == null)
        {
            Debug.LogError("PlayerProgressApi no existe en la escena.");
            yield break;
        }

        string codigoJugador = PlayerPrefs.GetString("PlayerCode", "");
        if (string.IsNullOrEmpty(codigoJugador))
        {
            Debug.LogError("No hay PlayerCode guardado.");
            yield break;
        }

        sincronizandoProgreso = true;

        yield return StartCoroutine(
            PlayerProgressApi.Instance.AgregarPuntos(
                puntosGanados,
                motivo,
                onSuccess: (data) =>
                {
                    if (data != null && data.ok)
                    {
                        ActualizarProgresoServidor(
                            data.jugador_id,
                            data.puntos_totales,
                            data.runas,
                            data.puntos_residuales
                        );

                        ultimasMonedasGanadas = data.runas_generadas;

                        Debug.Log("Puntos sincronizados correctamente con servidor.");
                    }
                    else
                    {
                        Debug.LogError("La API respondió con error al agregar puntos.");
                    }
                },
                onError: (error) =>
                {
                    Debug.LogError("Error al sincronizar puntos: " + error);
                }
            )
        );

        sincronizandoProgreso = false;
    }

    private void SincronizarRecompensasLocales()
    {
        puntos = rewardSystem.TotalPoints;
        puntosNivelActual = rewardSystem.CurrentLevelPoints;

        // El saldo real de tienda ahora viene del servidor
        monedasTienda = runasServidor;

        ActualizarTextoRecompensas();
    }

    public void ActualizarProgresoServidor(long nuevoJugadorId, int puntosTotales, int runas, int residuales)
    {
        jugadorId = nuevoJugadorId;
        puntosTotalesServidor = puntosTotales;
        runasServidor = runas;
        puntosResidualesServidor = residuales;
        progresoServidorCargado = true;

        monedasTienda = runasServidor;
        puntos = puntosResidualesServidor;

        ActualizarTodoElTexto();
    }

    public void GuardarSiguienteNivel(int index)
    {
        siguienteNivelIndex = index;
        Debug.Log("Siguiente nivel guardado: " + siguienteNivelIndex);
    }

    public void CargarSiguienteNivel()
    {
        Debug.Log("Intentando cargar siguiente nivel: " + siguienteNivelIndex);

        if (siguienteNivelIndex >= 0 && siguienteNivelIndex < SceneManager.sceneCountInBuildSettings)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(siguienteNivelIndex);
            Debug.Log("Cargando escena: " + scenePath);
            SceneManager.LoadScene(siguienteNivelIndex, LoadSceneMode.Single);
        }
        else
        {
            Debug.LogError("Índice de siguiente nivel inválido: " + siguienteNivelIndex);
        }
    }

    public void ResetearRecompensasGlobales()
    {
        if (rewardSystem == null)
            rewardSystem = new LevelRewardSystem();

        rewardSystem.Reset();

        puntos = 0;
        monedasTienda = 0;
        puntosNivelActual = 0;
        ultimoPuntajeGanado = 0;
        ultimasMonedasGanadas = 0;
        puntosSobrantes = 0;

        puntosTotalesServidor = 0;
        runasServidor = 0;
        puntosResidualesServidor = 0;
        progresoServidorCargado = false;

        ActualizarTodoElTexto();
    }

    private void ActualizarTodoElTexto()
    {
        ActualizarTextoMonedas();
        ActualizarTextoVidas();
        ActualizarTextoRecompensas();
    }

    private void ActualizarTextoMonedas()
    {
        if (textoMonedas != null)
            textoMonedas.text = ": " + monedas;
    }

    private void ActualizarTextoVidas()
    {
        if (textoVidas != null)
            textoVidas.text = ": " + vidas;
    }

    private void ActualizarTextoRecompensas()
    {
        if (textoPuntos != null)
            textoPuntos.text = ": " + puntosNivelActual;

        if (textoMonedasTienda != null)
            textoMonedasTienda.text = ": " + runasServidor;
    }

    public void AsignarTextosUI(
        TextMeshProUGUI nuevoTextoMonedas,
        TextMeshProUGUI nuevoTextoVidas,
        TextMeshProUGUI nuevoTextoPuntos,
        TextMeshProUGUI nuevoTextoMonedasTienda)
    {
        textoMonedas = nuevoTextoMonedas;
        textoVidas = nuevoTextoVidas;
        textoPuntos = nuevoTextoPuntos;
        textoMonedasTienda = nuevoTextoMonedasTienda;

        ActualizarTodoElTexto();
    }

    // =========================
    // RUNAS / TIENDA
    // =========================

    public bool TieneMonedasTiendaSuficientes(int costo)
    {
        return runasServidor >= costo;
    }

    public bool GastarMonedasTienda(int costo)
    {
        // Este método luego deberá llamar una API de compra real.
        // Por ahora solo evita usar PlayerPrefs como fuente oficial.
        if (costo < 0)
            return false;

        if (runasServidor < costo)
            return false;

        runasServidor -= costo;
        monedasTienda = runasServidor;
        ActualizarTodoElTexto();
        return true;
    }

    public void AgregarMonedasTienda(int cantidad)
    {
        if (cantidad <= 0)
            return;

        runasServidor += cantidad;
        monedasTienda = runasServidor;
        ActualizarTodoElTexto();
    }

    public int ObtenerMonedasTienda()
    {
        return runasServidor;
    }
}