using UnityEngine;

public class TiendaLoadSync : MonoBehaviour
{
    private void Start()
    {
        if (GameManager.instancia != null)
        {
            GameManager.instancia.ForzarRecargaProgresoServidor();
            Debug.Log("Tienda: recargando progreso del servidor.");
        }
        else
        {
            Debug.LogWarning("Tienda: no existe GameManager.");
        }
    }
}