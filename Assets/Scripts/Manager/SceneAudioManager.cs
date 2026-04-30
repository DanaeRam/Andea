using UnityEngine;

public class SceneAudioManager : MonoBehaviour
{
    public static SceneAudioManager instancia;

    private AudioSource audioSource;

    private void Awake()
    {
        // Evita duplicados
        if (instancia != null && instancia != this)
        {
            Destroy(gameObject);
            return;
        }

        instancia = this;
        DontDestroyOnLoad(gameObject);

        // Busca el AudioSource ya existente en el objeto
        audioSource = GetComponent<AudioSource>();

        // Si no existe, muestra error para que lo agregues manualmente
        if (audioSource == null)
        {
            Debug.LogError("SceneAudioManager necesita un AudioSource en el mismo objeto.");
            return;
        }
    }
}