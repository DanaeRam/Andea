using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerLessonsApi : MonoBehaviour
{
    public static PlayerLessonsApi Instance;

    [Header("Endpoints API")]
    [SerializeField] private string obtenerEstadoLeccionesUrl = "https://andea.vercel.app/api/lecciones/estado";
    [SerializeField] private string completarLeccionUrl = "https://andea.vercel.app/api/lecciones/completar";

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
    public class EstadoLeccionesRequest
    {
        public string codigo;
        public string mundo;
    }

    [Serializable]
    public class LeccionEstado
    {
        public string leccion_id;
        public string mundo;
        public string nivel;
        public int numero;
        public string nombre;
        public bool completada;
        public bool desbloqueada;
    }

    [Serializable]
    public class EstadoLeccionesResponse
    {
        public bool ok;
        public LeccionEstado[] lecciones;
        public string error;
    }

    [Serializable]
    public class CompletarLeccionRequest
    {
        public string codigo;
        public string leccionId;
        public string mundo;
    }

    [Serializable]
    public class CompletarLeccionResponse
    {
        public bool ok;
        public string leccion_id;
        public string mensaje;
        public string error;
    }

    public IEnumerator ObtenerEstadoLecciones(
        Action<EstadoLeccionesResponse> onSuccess,
        Action<string> onError)
    {
        string mundoActual = PlayerPrefs.GetString("CurrentWorldCode", "LE");

        if (string.IsNullOrEmpty(mundoActual))
            mundoActual = "LE";

        yield return ObtenerEstadoLecciones(mundoActual, onSuccess, onError);
    }

    public IEnumerator ObtenerEstadoLecciones(
        string mundo,
        Action<EstadoLeccionesResponse> onSuccess,
        Action<string> onError)
    {
        string codigo = PlayerPrefs.GetString("PlayerCode", "");

        if (string.IsNullOrEmpty(codigo))
        {
            onError?.Invoke("No hay código de jugador.");
            yield break;
        }

        if (string.IsNullOrEmpty(mundo))
            mundo = "LE";

        mundo = mundo.Trim().ToUpper();

        EstadoLeccionesRequest payload = new EstadoLeccionesRequest
        {
            codigo = codigo,
            mundo = mundo
        };

        string json = JsonUtility.ToJson(payload);

        using UnityWebRequest request = new UnityWebRequest(obtenerEstadoLeccionesUrl, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.timeout = timeoutSegundos;
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        string respuesta = request.downloadHandler != null
            ? request.downloadHandler.text
            : "";

        Debug.Log("LECCIONES JSON enviado: " + json);
        Debug.Log("LECCIONES respuesta: " + respuesta);

        if (request.result != UnityWebRequest.Result.Success)
        {
            onError?.Invoke("Error al obtener lecciones: " + request.error);
            yield break;
        }

        EstadoLeccionesResponse response = null;

        try
        {
            response = JsonUtility.FromJson<EstadoLeccionesResponse>(respuesta);
        }
        catch (Exception e)
        {
            onError?.Invoke("Error al parsear estado de lecciones: " + e.Message);
            yield break;
        }

        if (response == null || !response.ok)
        {
            onError?.Invoke(response?.error ?? "Error al obtener estado de lecciones.");
            yield break;
        }

        onSuccess?.Invoke(response);
    }

    // Versión de compatibilidad.
    // Antes mandaba siempre LE. Ahora usa CurrentWorldCode para evitar errores con MA o SM.
    public IEnumerator CompletarLeccion(
        string leccionId,
        Action<CompletarLeccionResponse> onSuccess,
        Action<string> onError)
    {
        string mundoActual = PlayerPrefs.GetString("CurrentWorldCode", "LE");

        if (string.IsNullOrEmpty(mundoActual))
            mundoActual = "LE";

        mundoActual = mundoActual.Trim().ToUpper();

        yield return CompletarLeccion(mundoActual, leccionId, onSuccess, onError);
    }

    // Versión correcta para LE, MA y SM.
    public IEnumerator CompletarLeccion(
        string mundo,
        string leccionId,
        Action<CompletarLeccionResponse> onSuccess,
        Action<string> onError)
    {
        string codigo = PlayerPrefs.GetString("PlayerCode", "");

        if (string.IsNullOrEmpty(codigo))
        {
            onError?.Invoke("No hay código de jugador.");
            yield break;
        }

        if (string.IsNullOrEmpty(mundo))
            mundo = PlayerPrefs.GetString("CurrentWorldCode", "LE");

        if (string.IsNullOrEmpty(mundo))
            mundo = "LE";

        mundo = mundo.Trim().ToUpper();

        if (string.IsNullOrEmpty(leccionId))
        {
            onError?.Invoke("No hay ID de lección.");
            yield break;
        }

        CompletarLeccionRequest payload = new CompletarLeccionRequest
        {
            codigo = codigo,
            leccionId = leccionId,
            mundo = mundo
        };

        string json = JsonUtility.ToJson(payload);

        using UnityWebRequest request = new UnityWebRequest(completarLeccionUrl, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.timeout = timeoutSegundos;
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        string respuesta = request.downloadHandler != null
            ? request.downloadHandler.text
            : "";

        Debug.Log("COMPLETAR LECCIÓN JSON enviado: " + json);
        Debug.Log("COMPLETAR LECCIÓN respuesta: " + respuesta);

        if (request.result != UnityWebRequest.Result.Success)
        {
            onError?.Invoke("Error al completar lección: " + request.error);
            yield break;
        }

        CompletarLeccionResponse response = null;

        try
        {
            response = JsonUtility.FromJson<CompletarLeccionResponse>(respuesta);
        }
        catch (Exception e)
        {
            onError?.Invoke("Error al parsear completar lección: " + e.Message);
            yield break;
        }

        if (response == null || !response.ok)
        {
            onError?.Invoke(response?.error ?? "Error al completar lección.");
            yield break;
        }

        onSuccess?.Invoke(response);
    }
}