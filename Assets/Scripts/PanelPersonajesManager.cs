using UnityEngine;
using UnityEngine.UI;

public class PanelPersonajesManager : MonoBehaviour
{
    [Header("Contenedor")]
    public RectTransform contenidoItems;

    [Header("Personajes")]
    public Sprite[] spritesPersonajes;

    [Header("Configuración visual")]
    public Vector2 tamañoCelda = new Vector2(90, 90);
    public Vector2 espacioEntreItems = new Vector2(15, 15);
    public int columnas = 5;

    [Header("Opciones")]
    public bool crearAlIniciar = true;
    public bool limpiarAntesDeCrear = true;

    private void Start()
    {
        if (crearAlIniciar)
        {
            CrearPersonajes();
        }
    }

    public void CrearPersonajes()
    {
        if (contenidoItems == null)
        {
            Debug.LogError("Falta asignar ContenidoItems en el PanelPersonajesManager.");
            return;
        }

        if (spritesPersonajes == null || spritesPersonajes.Length == 0)
        {
            Debug.LogWarning("No agregaste sprites de personajes.");
            return;
        }

        PrepararGrid();

        if (limpiarAntesDeCrear)
        {
            LimpiarContenido();
        }

        for (int i = 0; i < spritesPersonajes.Length; i++)
        {
            CrearItemPersonaje(spritesPersonajes[i], i);
        }
    }

    private void PrepararGrid()
    {
        GridLayoutGroup grid = contenidoItems.GetComponent<GridLayoutGroup>();

        if (grid == null)
        {
            grid = contenidoItems.gameObject.AddComponent<GridLayoutGroup>();
        }

        grid.cellSize = tamañoCelda;
        grid.spacing = espacioEntreItems;
        grid.startCorner = GridLayoutGroup.Corner.UpperLeft;
        grid.startAxis = GridLayoutGroup.Axis.Horizontal;
        grid.childAlignment = TextAnchor.UpperLeft;
        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = columnas;

        ContentSizeFitter fitter = contenidoItems.GetComponent<ContentSizeFitter>();

        if (fitter == null)
        {
            fitter = contenidoItems.gameObject.AddComponent<ContentSizeFitter>();
        }

        fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;
    }

    private void LimpiarContenido()
    {
        for (int i = contenidoItems.childCount - 1; i >= 0; i--)
        {
            Destroy(contenidoItems.GetChild(i).gameObject);
        }
    }

    private void CrearItemPersonaje(Sprite sprite, int indice)
    {
        if (sprite == null)
        {
            Debug.LogWarning("Hay un sprite vacío en la posición: " + indice);
            return;
        }

        GameObject item = new GameObject("Personaje_" + sprite.name);
        item.transform.SetParent(contenidoItems, false);

        RectTransform rect = item.AddComponent<RectTransform>();
        rect.localScale = Vector3.one;

        Image imagen = item.AddComponent<Image>();
        imagen.sprite = sprite;
        imagen.preserveAspect = true;
        imagen.raycastTarget = true;
        imagen.color = Color.white;

        Button boton = item.AddComponent<Button>();
        boton.targetGraphic = imagen;
        boton.interactable = true;

        int indiceLocal = indice;
        boton.onClick.AddListener(() => SeleccionarPersonaje(indiceLocal));
    }

    private void SeleccionarPersonaje(int indice)
    {
        Debug.Log("Seleccionaste personaje: " + spritesPersonajes[indice].name);
    }
}