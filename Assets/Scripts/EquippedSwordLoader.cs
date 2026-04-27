using UnityEngine;

public class EquippedSwordLoader : MonoBehaviour
{
    [System.Serializable]
    public class SwordEntry
    {
        public string itemId;
        public GameObject swordObject;
    }

    [SerializeField] private SwordEntry[] swords;

    private const string EquippedWeaponKey = "equipped_weapon_id";

    private void Start()
    {
        string equippedId = PlayerPrefs.GetString(EquippedWeaponKey, "");

        for (int i = 0; i < swords.Length; i++)
        {
            if (swords[i].swordObject != null)
                swords[i].swordObject.SetActive(swords[i].itemId == equippedId);
        }
    }
}