using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerProgressApi : MonoBehaviour
{
    public static PlayerProgressApi Instance;

    [Header("Endpoints API")]
    [SerializeField] private string obtenerProgresoUrl = "https://andea.vercel.app/api/progreso/obtener-progreso";
    [SerializeField] private string agregarPuntosUrl = "https://andea.vercel.app/api/progreso/agregar-puntos";

    [Header("Configuración")]
    [SerializeField] private int timeoutSegundos = 15;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [Serializable]
    public class ObtenerProgresoRequest
    {
        public string codigo;
    }

    [Serializable]
    public class AgregarPuntosRequest
    {
        public string codigo;
        public int puntos_ganados;
        public string motivo;
    }

    [Serializable]
    public class ProgresoResponse
    {
        public bool ok;
        public long jugador_id;
        public int puntos_totales;
        public int runas;
        public int puntos_residuales;
        public int runas_generadas;
        public string error;
    }

    public IEnumerator ObtenerProgreso(Action<ProgresoResponse> onSuccess, Action<string> onError)
    {
        string codigo = PlayerPrefs.GetString("PlayerCode", "");

        if (string.IsNullOrEmpty(codigo))
        {
            onError?.Invoke("No se encontró PlayerCode en PlayerPrefs.");
            yield break;
        }

        ObtenerProgresoRequest payload = new ObtenerProgresoRequest
        {
            codigo = codigo
        };

        yield return StartCoroutine(EnviarRequestJson(
            url: obtenerProgresoUrl,
            payload: payload,
            onSuccess: onSuccess,
            onError: onError,
            nombreOperacion: "obtener progreso"
        ));
    }

    public IEnumerator AgregarPuntos(int puntosGanados, string motivo, Action<ProgresoResponse> onSuccess, Action<string> onError)
    {
        string codigo = PlayerPrefs.GetString("PlayerCode", "");

        if (string.IsNullOrEmpty(codigo))
        {
            onError?.Invoke("No se encontró PlayerCode en PlayerPrefs.");
            yield break;
        }

        if (puntosGanados <= 0)
        {
            onError?.Invoke("puntosGanados debe ser mayor que 0.");
            yield break;
        }

        AgregarPuntosRequest payload = new AgregarPuntosRequest
        {
            codigo = codigo,
            puntos_ganados = puntosGanados,
            motivo = motivo
        };

        yield return StartCoroutine(EnviarRequestJson(
            url: agregarPuntosUrl,
            payload: payload,
            onSuccess: onSuccess,
            onError: onError,
            nombreOperacion: "agregar puntos"
        ));
    }

    private IEnumerator EnviarRequestJson<TRequest>(
        string url,
        TRequest payload,
        Action<ProgresoResponse> onSuccess,
        Action<string> onError,
        string nombreOperacion)
    {
        string json = JsonUtility.ToJson(payload);

        using UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.timeout = timeoutSegundos;
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        string responseText = request.downloadHandler != null
            ? request.downloadHandler.text
            : "";

        if (request.result != UnityWebRequest.Result.Success)
        {
            onError?.Invoke($"Error HTTP al {nombreOperacion}: {request.error} | Respuesta: {responseText}");
            yield break;
        }

        if (string.IsNullOrWhiteSpace(responseText))
        {
            onError?.Invoke($"La respuesta de {nombreOperacion} es vacía.");
            yield break;
        }

        ProgresoResponse response = null;

        try
        {
            response = JsonUtility.FromJson<ProgresoResponse>(responseText);
        }
        catch (Exception e)
        {
            onError?.Invoke($"Error al parsear JSON de {nombreOperacion}: {e.Message} | Respuesta: {responseText}");
            yield break;
        }

        if (response == null)
        {
            onError?.Invoke($"No se pudo convertir la respuesta de {nombreOperacion}. Respuesta: {responseText}");
            yield break;
        }

        if (!response.ok)
        {
            string mensajeError = string.IsNullOrEmpty(response.error)
                ? $"La API devolvió ok=false en {nombreOperacion}."
                : response.error;

            onError?.Invoke($"{nombreOperacion}: {mensajeError}");
            yield break;
        }

        onSuccess?.Invoke(response);
    }
}