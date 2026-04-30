using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSwordManager : MonoBehaviour
{
    [System.Serializable]
    public class SwordData
    {
        public string idRecompensa;
        public string nombre;
        public Sprite sprite;
        public int bonusDamage = 0;
    }

    [Header("Referencias del jugador")]
    public PlayerController playerController;
    public SwordDamage swordDamage;
    public SpriteRenderer swordVisual;

    [Header("Daño base del jugador")]
    public int baseDamage = 1;

    [Header("Espadas disponibles")]
    public List<SwordData> swords = new List<SwordData>();
    [Header("Espada por defecto")]
    public string defaultSwordId = "ESPADA_DEFAULT";

    private void Start()
    {
        if (playerController == null)
            playerController = GetComponent<PlayerController>();

        CargarEspadaEquipada();
    }

    private void CargarEspadaEquipada()
    {
        string idGuardado = PlayerPrefs.GetString("EquippedSwordId", "");

        if (!string.IsNullOrEmpty(idGuardado))
        {
            ApplySwordById(idGuardado);
            return;
        }

        if (PlayerInventoryApi.Instance != null)
        {
            StartCoroutine(CargarDesdeServidor());
        }
    }

    private IEnumerator CargarDesdeServidor()
    {
        yield return PlayerInventoryApi.Instance.ObtenerInventario(
            onSuccess: (data) =>
            {
                foreach (var item in data.items)
                {
                    if (item.equipada)
                    {
                        PlayerPrefs.SetString("EquippedSwordId", item.recompensa_id);
                        PlayerPrefs.SetString("EquippedSwordName", item.nombre);
                        PlayerPrefs.Save();

                        ApplySwordById(item.recompensa_id);
                        return;
                    }
                }

                Debug.Log("No hay espada equipada todavía.");
            },
            onError: (error) =>
            {
                Debug.LogWarning("No se pudo cargar espada equipada: " + error);
            }
        );
    }

    public void ApplySwordById(string idRecompensa)
    {
        SwordData sword = swords.Find(s => s.idRecompensa == idRecompensa);

        if (sword == null)
        {
            Debug.LogWarning("No existe SwordData para: " + idRecompensa);
            return;
        }

        int finalDamage = baseDamage + sword.bonusDamage;

        if (swordVisual != null)
            swordVisual.sprite = sword.sprite;

        if (swordDamage != null)
            swordDamage.damage = finalDamage;

        if (playerController != null)
            playerController.attackDamage = finalDamage;

        Debug.Log("Espada aplicada: " + sword.nombre + " | Daño final: " + finalDamage);
    }
}