using UnityEngine;

public class CameraZoom2D : MonoBehaviour
{
    public Camera cam;
    public float zoomSpeed = 5f;

    private float targetSize;

    private void Start()
    {
        if (cam == null)
            cam = Camera.main;

        if (cam != null)
            targetSize = cam.orthographicSize;
    }

    private void Update()
    {
        if (cam == null) return;

        cam.orthographicSize = Mathf.Lerp(
            cam.orthographicSize,
            targetSize,
            Time.unscaledDeltaTime * zoomSpeed
        );
    }

    public void SetZoom(float size)
    {
        targetSize = size;
    }
}