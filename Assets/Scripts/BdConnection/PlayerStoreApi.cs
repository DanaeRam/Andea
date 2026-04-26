using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerStoreApi : MonoBehaviour
{
    public static PlayerStoreApi Instance;

    [SerializeField] private string comprarItemUrl = "https://andea.vercel.app/api/tienda/comprar";

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
    public class CompraRequest
    {
        public string codigo;
        public string idRecompensa;
        public int costo;
    }

    [Serializable]
    public class CompraResponse
    {
        public bool ok;
        public long jugador_id;
        public int puntos_totales;
        public int runas;
        public int puntos_residuales;
        public string error;
    }

    public IEnumerator ComprarItem(
        string idRecompensa,
        int costo,
        Action<CompraResponse> onSuccess,
        Action<string> onError)
    {
        string codigo = PlayerPrefs.GetString("PlayerCode", "");

        if (string.IsNullOrEmpty(codigo))
        {
            onError?.Invoke("No hay código de jugador.");
            yield break;
        }

        CompraRequest payload = new CompraRequest
        {
            codigo = codigo,
            idRecompensa = idRecompensa,
            costo = costo
        };

        string json = JsonUtility.ToJson(payload);

        using UnityWebRequest request = new UnityWebRequest(comprarItemUrl, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        Debug.Log("STORE API JSON enviado: " + json);
        Debug.Log("STORE API respuesta: " + request.downloadHandler.text);

        if (request.result != UnityWebRequest.Result.Success)
        {
            onError?.Invoke("Error de conexión.");
            yield break;
        }

        CompraResponse response = JsonUtility.FromJson<CompraResponse>(request.downloadHandler.text);

        if (response == null || !response.ok)
        {
            onError?.Invoke(response?.error ?? "Error al comprar.");
            yield break;
        }

        onSuccess?.Invoke(response);
    }
}