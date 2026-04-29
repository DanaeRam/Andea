using UnityEngine;
using UnityEngine.UI;

public class BotonEspadasExplosion : MonoBehaviour
{
    [Header("Botón que activa el efecto")]
    public Button botonEspadas;

    [Header("Partículas de explosión")]
    public ParticleSystem explosionFX;

    [Header("Objeto donde aparecerá la explosión")]
    public RectTransform puntoExplosion;

    private void Start()
    {
        if (botonEspadas == null)
            botonEspadas = GetComponent<Button>();

        if (botonEspadas != null)
            botonEspadas.onClick.AddListener(ReproducirExplosion);

        if (explosionFX != null)
            explosionFX.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    public void ReproducirExplosion()
    {
        if (explosionFX == null)
        {
            Debug.LogWarning("No asignaste el Particle System de explosión.");
            return;
        }

        if (puntoExplosion != null)
            explosionFX.transform.position = puntoExplosion.position;
        else
            explosionFX.transform.position = transform.position;

        explosionFX.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        explosionFX.Play();

        Debug.Log("Explosión de espadas activada.");
    }
}