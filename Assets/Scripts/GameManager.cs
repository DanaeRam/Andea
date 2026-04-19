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

    [Header("Recompensas globales")]
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

    [Header("UI")]
    public TextMeshProUGUI textoMonedas;
    public TextMeshProUGUI textoVidas;
    public TextMeshProUGUI textoPuntos;
    public TextMeshProUGUI textoMonedasTienda;

    private LevelRewardSystem rewardSystem;

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
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    private void Start()
    {
        ActualizarTodoElTexto();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == nombreEscenaResultados)
        {
            return;
        }

        ReiniciarDatosDelNivel();
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
        SincronizarRecompensas();
    }

    public void RegistrarEnemigoDerrotado()
    {
        if (rewardSystem == null)
            rewardSystem = new LevelRewardSystem();

        rewardSystem.RegisterEnemyDefeated();
        SincronizarRecompensas();
    }

    public void CompletarNivel()
    {
        if (rewardSystem == null)
            rewardSystem = new LevelRewardSystem();

        rewardSystem.CompleteLevel(true);

        ultimoPuntajeGanado = rewardSystem.LastPointsEarned;
        ultimasMonedasGanadas = rewardSystem.LastShopCoinsEarned;
        puntosSobrantes = rewardSystem.LastRemainingPoints;

        SincronizarRecompensas();

        rewardSystem.ResetLevelProgress();
    }

    public void CompletarActividad()
    {
        if (rewardSystem == null)
            rewardSystem = new LevelRewardSystem();

        rewardSystem.CompleteActivity(true);

        ultimoPuntajeGanado = rewardSystem.LastPointsEarned;
        ultimasMonedasGanadas = rewardSystem.LastShopCoinsEarned;
        puntosSobrantes = rewardSystem.LastRemainingPoints;

        SincronizarRecompensas();

        rewardSystem.ResetLevelProgress();
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

        ActualizarTodoElTexto();
    }

    private void SincronizarRecompensas()
    {
        puntos = rewardSystem.TotalPoints;
        monedasTienda = rewardSystem.ShopCoins;
        puntosNivelActual = rewardSystem.CurrentLevelPoints;

        ActualizarTextoRecompensas();
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
            textoMonedasTienda.text = ": " + monedasTienda;
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
}