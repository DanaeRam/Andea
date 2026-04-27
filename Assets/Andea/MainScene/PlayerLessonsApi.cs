using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerLessonsApi : MonoBehaviour
{
    public static PlayerLessonsApi Instance;

    [Header("Endpoints API")]
    [SerializeField] private string obtenerEstadoLeccionesUrl =
        "https://andea.vercel.app/api/lecciones/estado";

    [SerializeField] private string completarLeccionUrl =
        "https://andea.vercel.app/api/lecciones/completar";

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
    }

    [Serializable]
    public class LeccionEstado
    {
        public string leccion_id;
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
        string codigo = PlayerPrefs.GetString("PlayerCode", "");

        if (string.IsNullOrEmpty(codigo))
        {
            onError?.Invoke("No hay código de jugador.");
            yield break;
        }

        EstadoLeccionesRequest payload = new EstadoLeccionesRequest
        {
            codigo = codigo
        };

        string json = JsonUtility.ToJson(payload);

        using UnityWebRequest request = new UnityWebRequest(obtenerEstadoLeccionesUrl, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        string respuesta = request.downloadHandler.text;

        Debug.Log("LECCIONES JSON enviado: " + json);
        Debug.Log("LECCIONES respuesta: " + respuesta);

        if (request.result != UnityWebRequest.Result.Success)
        {
            onError?.Invoke("Error al obtener lecciones: " + request.error);
            yield break;
        }

        EstadoLeccionesResponse response =
            JsonUtility.FromJson<EstadoLeccionesResponse>(respuesta);

        if (response == null || !response.ok)
        {
            onError?.Invoke(response?.error ?? "Error al obtener estado de lecciones.");
            yield break;
        }

        onSuccess?.Invoke(response);
    }

    public IEnumerator CompletarLeccion(
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

        CompletarLeccionRequest payload = new CompletarLeccionRequest
        {
            codigo = codigo,
            leccionId = leccionId
        };

        string json = JsonUtility.ToJson(payload);

        using UnityWebRequest request = new UnityWebRequest(completarLeccionUrl, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        string respuesta = request.downloadHandler.text;

        Debug.Log("COMPLETAR LECCIÓN JSON enviado: " + json);
        Debug.Log("COMPLETAR LECCIÓN respuesta: " + respuesta);

        if (request.result != UnityWebRequest.Result.Success)
        {
            onError?.Invoke("Error al completar lección: " + request.error);
            yield break;
        }

        CompletarLeccionResponse response =
            JsonUtility.FromJson<CompletarLeccionResponse>(respuesta);

        if (response == null || !response.ok)
        {
            onError?.Invoke(response?.error ?? "Error al completar lección.");
            yield break;
        }

        onSuccess?.Invoke(response);
    }
}