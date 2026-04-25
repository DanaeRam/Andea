using System.Collections;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instancia;

    [Header("Monedas normales de la sesión")]
    public int monedas = 0;

    [Header("Vidas")]
    public int vidas = 3;
    public int vidasIniciales = 3;

    [Header("Recompensas locales de la sesión")]
    public int puntos = 0;
    public int monedasTienda = 0;
    public int puntosNivelActual = 0;

    [Header("Resultado de la última sesión")]
    public int ultimoPuntajeGanado = 0;
    public int ultimasMonedasGanadas = 0;
    public int puntosSobrantes = 0;

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
    private bool progresoInicialSolicitado = false;

    private void Awake()
    {
        if (instancia == null)
        {
            instancia = this;
            DontDestroyOnLoad(gameObject);
            rewardSystem = new LevelRewardSystem();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        ActualizarTodoElTexto();
        SolicitarProgresoInicialSiExisteCodigo();
    }

    // =========================
    // INICIO / SESIÓN
    // =========================

    public void SolicitarProgresoInicialSiExisteCodigo()
    {
        if (progresoInicialSolicitado)
            return;

        string codigoJugador = PlayerPrefs.GetString("PlayerCode", "");

        if (string.IsNullOrEmpty(codigoJugador))
        {
            Debug.Log("Aún no hay PlayerCode, no se solicita progreso todavía.");
            return;
        }

        progresoInicialSolicitado = true;
        StartCoroutine(CargarProgresoInicialSiExisteCodigo());
    }

    private IEnumerator CargarProgresoInicialSiExisteCodigo()
    {
        yield return null;

        string codigoJugador = PlayerPrefs.GetString("PlayerCode", "");

        if (string.IsNullOrEmpty(codigoJugador))
        {
            Debug.Log("No hay PlayerCode guardado todavía.");
            progresoInicialSolicitado = false;
            yield break;
        }

        if (PlayerProgressApi.Instance == null)
        {
            Debug.LogWarning("PlayerProgressApi no existe en la escena.");
            progresoInicialSolicitado = false;
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
                        progresoInicialSolicitado = false;
                    }
                },
                onError: (error) =>
                {
                    Debug.LogError("Error al cargar progreso inicial: " + error);
                    progresoInicialSolicitado = false;
                }
            )
        );
    }

    public void IniciarNuevaSesion()
    {
        monedas = 0;
        vidas = vidasIniciales;

        if (rewardSystem == null)
            rewardSystem = new LevelRewardSystem();
        else
            rewardSystem.Reset();

        puntos = 0;
        puntosNivelActual = 0;

        ultimoPuntajeGanado = 0;
        ultimasMonedasGanadas = 0;
        puntosSobrantes = 0;

        monedasTienda = runasServidor;

        ActualizarTodoElTexto();
        Debug.Log("Nueva sesión iniciada.");
    }

    // =========================
    // PROGRESO LOCAL DE SESIÓN
    // =========================

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

    // =========================
    // CIERRE DE SESIÓN / NIVEL
    // =========================

    public IEnumerator CompletarNivelCoroutine(System.Action onFinished = null)
    {
        if (rewardSystem == null)
            rewardSystem = new LevelRewardSystem();

        bool completado = rewardSystem.CompleteLevel(true);

        if (!completado)
        {
            onFinished?.Invoke();
            yield break;
        }

        ultimoPuntajeGanado = rewardSystem.LastPointsEarned;
        puntosSobrantes = rewardSystem.LastRemainingPoints;
        ultimasMonedasGanadas = 0;

        SincronizarRecompensasLocales();

        if (ultimoPuntajeGanado > 0)
        {
            yield return StartCoroutine(
                SincronizarPuntosConServidor(
                    ultimoPuntajeGanado,
                    "nivel_completado"
                )
            );
        }

        puntosNivelActual = 0;
        ActualizarTodoElTexto();

        onFinished?.Invoke();
    }

    public IEnumerator CompletarActividadCoroutine(System.Action onFinished = null)
    {
        if (rewardSystem == null)
            rewardSystem = new LevelRewardSystem();

        bool completado = rewardSystem.CompleteActivity(true);

        if (!completado)
        {
            onFinished?.Invoke();
            yield break;
        }

        ultimoPuntajeGanado = rewardSystem.LastPointsEarned;
        puntosSobrantes = rewardSystem.LastRemainingPoints;
        ultimasMonedasGanadas = 0;

        SincronizarRecompensasLocales();

        if (ultimoPuntajeGanado > 0)
        {
            yield return StartCoroutine(
                SincronizarPuntosConServidor(
                    ultimoPuntajeGanado,
                    "actividad_completada"
                )
            );
        }

        puntosNivelActual = 0;
        ActualizarTodoElTexto();

        onFinished?.Invoke();
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
        monedasTienda = runasServidor;
        ActualizarTextoRecompensas();
    }

    public void ActualizarProgresoServidor(long nuevoJugadorId, int puntosTotales, int runas, int residuales)
    {
        Debug.Log("ActualizarProgresoServidor -> jugadorId: " + nuevoJugadorId +
                " | puntosTotales: " + puntosTotales +
                " | runas: " + runas +
                " | residuales: " + residuales);

        jugadorId = nuevoJugadorId;
        puntosTotalesServidor = puntosTotales;
        runasServidor = runas;
        puntosResidualesServidor = residuales;
        progresoServidorCargado = true;

        monedasTienda = runasServidor;
        puntos = puntosResidualesServidor;

        ActualizarTodoElTexto();
    }
    // =========================
    // RESETEO GENERAL
    // =========================

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
        monedas = 0;
        vidas = vidasIniciales;

        progresoInicialSolicitado = false;

        ActualizarTodoElTexto();
    }

    // =========================
    // UI
    // =========================

    private void ActualizarTodoElTexto()
    {
        ActualizarTextoMonedas();
        ActualizarTextoVidas();
        ActualizarTextoRecompensas();
    }

    private void ActualizarTextoMonedas()
    {
        if (textoMonedas != null)
            textoMonedas.text = monedas.ToString();
    }

    private void ActualizarTextoVidas()
    {
        if (textoVidas != null)
            textoVidas.text = vidas.ToString();
    }

    private void ActualizarTextoRecompensas()
    {
        if (textoPuntos != null)
            textoPuntos.text = puntosNivelActual.ToString();

        if (textoMonedasTienda != null)
        {
            textoMonedasTienda.text = runasServidor.ToString();
            Debug.Log("UI runas actualizada a: " + runasServidor + " | objeto texto: " + textoMonedasTienda.name);
        }
        else
        {
            Debug.LogWarning("textoMonedasTienda es NULL en GameManager");
        }
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

    public void ForzarRecargaProgresoServidor()
    {
        progresoInicialSolicitado = false;
        SolicitarProgresoInicialSiExisteCodigo();
    }
}