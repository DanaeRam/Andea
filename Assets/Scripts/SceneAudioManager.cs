using UnityEngine;

public class SceneAudioManager : MonoBehaviour
{
    [Header("Música de esta escena")]
    public AudioClip musicaEscena;

    [Header("Configuración")]
    [Range(0f, 1f)] public float volumen = 0.5f;
    public bool reproducirAlIniciar = true;
    public bool repetir = true;

    private AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.clip = musicaEscena;
        audioSource.volume = volumen;
        audioSource.loop = repetir;
        audioSource.playOnAwake = false;
    }

    private void Start()
    {
        if (reproducirAlIniciar && musicaEscena != null)
        {
            audioSource.Play();
        }
    }

    public void DetenerMusica()
    {
        audioSource.Stop();
    }

    public void CambiarMusica(AudioClip nuevaMusica)
    {
        audioSource.Stop();
        audioSource.clip = nuevaMusica;
        audioSource.Play();
    }
}