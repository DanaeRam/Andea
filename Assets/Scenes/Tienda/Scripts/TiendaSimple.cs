using UnityEngine;

public class TiendaSimple : MonoBehaviour
{
    public GameObject itemPrefab;
    public Transform content;

    void Start()
    {
        for (int i = 0; i < 5; i++)
        {
            Instantiate(itemPrefab, content);
        }
    }
}