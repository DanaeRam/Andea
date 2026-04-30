using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerInventoryApi : MonoBehaviour
{
    public static PlayerInventoryApi Instance;

    [Header("Endpoints API")]
    [SerializeField] private string obtenerInventarioUrl =
        "https://andea.vercel.app/api/inventario/obtener";

    [SerializeField] private string equiparInventarioUrl =
        "https://andea.vercel.app/api/inventario/equipar";

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
    public class InventarioRequest
    {
        public string codigo;
    }

    [Serializable]
    public class InventarioItem
    {
        public string recompensa_id;
        public string nombre;
        public string tipo;
        public string sprite_key;
        public bool equipada;
    }

    [Serializable]
    public class InventarioResponse
    {
        public bool ok;
        public InventarioItem[] items;
        public string error;
    }

    [Serializable]
    public class EquiparRequest
    {
        public string codigo;
        public string idRecompensa;
    }

    [Serializable]
    public class EquiparResponse
    {
        public bool ok;
        public string recompensa_id;
        public string nombre;
        public string error;
    }

    public IEnumerator ObtenerInventario(
        Action<InventarioResponse> onSuccess,
        Action<string> onError)
    {
        string codigo = PlayerPrefs.GetString("PlayerCode", "");

        if (string.IsNullOrEmpty(codigo))
        {
            onError?.Invoke("No hay código de jugador.");
            yield break;
        }

        InventarioRequest payload = new InventarioRequest
        {
            codigo = codigo
        };

        string json = JsonUtility.ToJson(payload);

        using UnityWebRequest request = new UnityWebRequest(obtenerInventarioUrl, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        string respuesta = request.downloadHandler.text;
        Debug.Log("INVENTARIO respuesta: " + respuesta);

        if (request.result != UnityWebRequest.Result.Success)
        {
            onError?.Invoke("Error al cargar inventario: " + request.error);
            yield break;
        }

        InventarioResponse response = JsonUtility.FromJson<InventarioResponse>(respuesta);

        if (response == null || !response.ok)
        {
            onError?.Invoke(response?.error ?? "Error al obtener inventario.");
            yield break;
        }

        onSuccess?.Invoke(response);
    }

    public IEnumerator EquiparEspada(
        string idRecompensa,
        Action<EquiparResponse> onSuccess,
        Action<string> onError)
    {
        string codigo = PlayerPrefs.GetString("PlayerCode", "");

        if (string.IsNullOrEmpty(codigo))
        {
            onError?.Invoke("No hay código de jugador.");
            yield break;
        }

        EquiparRequest payload = new EquiparRequest
        {
            codigo = codigo,
            idRecompensa = idRecompensa
        };

        string json = JsonUtility.ToJson(payload);

        using UnityWebRequest request = new UnityWebRequest(equiparInventarioUrl, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        string respuesta = request.downloadHandler.text;
        Debug.Log("EQUIPAR JSON enviado: " + json);
        Debug.Log("EQUIPAR respuesta: " + respuesta);

        if (request.result != UnityWebRequest.Result.Success)
        {
            onError?.Invoke("Error al equipar: " + request.error);
            yield break;
        }

        EquiparResponse response = JsonUtility.FromJson<EquiparResponse>(respuesta);

        if (response == null || !response.ok)
        {
            onError?.Invoke(response?.error ?? "Error al equipar espada.");
            yield break;
        }

        onSuccess?.Invoke(response);
    }
}