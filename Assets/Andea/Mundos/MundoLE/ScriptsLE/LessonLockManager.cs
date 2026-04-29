using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LessonLockManager : MonoBehaviour
{
    [System.Serializable]
    public class LessonButtonData
    {
        [Header("Datos de la lección")]
        public string leccionId;
        public string nivel;

        [Header("UI")]
        public Button botonLeccion;
        public GameObject candado;
        public TextMeshProUGUI textoEstado;
    }

    [Header("Botones de lecciones")]
    public List<LessonButtonData> lecciones = new List<LessonButtonData>();

    [Header("Mensaje general")]
    public TextMeshProUGUI textoMensaje;

    private void Start()
    {
        CargarBloqueos();
    }

    public void CargarBloqueos()
    {
        if (PlayerLessonsApi.Instance == null)
        {
            MostrarMensaje("Error: no existe PlayerLessonsApi.");
            return;
        }

        StartCoroutine(PlayerLessonsApi.Instance.ObtenerEstadoLecciones(
            (data) =>
            {
                AplicarEstadoLecciones(data.lecciones);
            },
            (error) =>
            {
                MostrarMensaje(error);
            }
        ));
    }

    private void AplicarEstadoLecciones(PlayerLessonsApi.LeccionEstado[] estadoServidor)
    {
        foreach (var itemUI in lecciones)
        {
            PlayerLessonsApi.LeccionEstado estado = BuscarEstado(itemUI.leccionId, estadoServidor);

            if (estado == null)
            {
                Debug.LogWarning("No se encontró estado para: " + itemUI.leccionId);
                BloquearLeccion(itemUI, "No encontrada");
                continue;
            }

            if (estado.desbloqueada)
            {
                DesbloquearLeccion(itemUI, estado.completada);
            }
            else
            {
                BloquearLeccion(itemUI, "Bloqueada");
            }
        }

        MostrarMensaje("");
    }

    private PlayerLessonsApi.LeccionEstado BuscarEstado(
        string leccionId,
        PlayerLessonsApi.LeccionEstado[] estadoServidor)
    {
        foreach (var estado in estadoServidor)
        {
            if (estado.leccion_id == leccionId)
                return estado;
        }

        return null;
    }

    private void DesbloquearLeccion(LessonButtonData itemUI, bool completada)
    {
        if (itemUI.botonLeccion != null)
            itemUI.botonLeccion.interactable = true;

        if (itemUI.candado != null)
            itemUI.candado.SetActive(false);

        if (itemUI.textoEstado != null)
            itemUI.textoEstado.text = completada ? "Completada" : "";
    }

    private void BloquearLeccion(LessonButtonData itemUI, string mensaje)
    {
        if (itemUI.botonLeccion != null)
            itemUI.botonLeccion.interactable = false;

        if (itemUI.candado != null)
            itemUI.candado.SetActive(true);

        if (itemUI.textoEstado != null)
            itemUI.textoEstado.text = mensaje;
    }

    private void MostrarMensaje(string mensaje)
    {
        if (textoMensaje != null)
            textoMensaje.text = mensaje;

        if (!string.IsNullOrEmpty(mensaje))
            Debug.Log("LessonLockManager: " + mensaje);
    }
}
