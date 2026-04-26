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

        Debug.Log("STORE API JSON enviado: " + json);

        yield return request.SendWebRequest();

        string responseText = request.downloadHandler.text;

        Debug.Log("STORE API status: " + request.responseCode);
        Debug.Log("STORE API result: " + request.result);
        Debug.Log("STORE API error: " + request.error);
        Debug.Log("STORE API respuesta: " + responseText);

        if (request.result != UnityWebRequest.Result.Success)
        {
            if (!string.IsNullOrEmpty(responseText))
            {
                CompraResponse errorResponse = JsonUtility.FromJson<CompraResponse>(responseText);

                if (errorResponse != null && !string.IsNullOrEmpty(errorResponse.error))
                {
                    onError?.Invoke(errorResponse.error);
                }
                else
                {
                    onError?.Invoke("Error al comprar.");
                }
            }
            else
            {
                onError?.Invoke("Error de conexión: " + request.error);
            }

            yield break;
        }

        if (string.IsNullOrEmpty(responseText))
        {
            onError?.Invoke("La API respondió vacío.");
            yield break;
        }

        CompraResponse response = JsonUtility.FromJson<CompraResponse>(responseText);

        if (response == null)
        {
            onError?.Invoke("No se pudo leer la respuesta de la API.");
            yield break;
        }

        if (!response.ok)
        {
            onError?.Invoke(response.error ?? "Error al comprar.");
            yield break;
        }

        onSuccess?.Invoke(response);
    }
}