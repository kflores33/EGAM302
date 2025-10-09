using UnityEngine;
using TMPro;
using System.Collections;

public class GeneralUIHandler : MonoBehaviour
{
    public static GeneralUIHandler instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else { Destroy(this); }
    }
    void Start()
    {
        StartCoroutine(WaitForLoad());
    }

    IEnumerator WaitForLoad()
    {
        // Wait until SaveManager has loaded and has data
        yield return new WaitUntil(() => SaveManager.instance.RuntimeWeapons != null);
        yield return new WaitUntil(() => SaveManager.instance.RuntimeWeapons != null &&
                                         SaveManager.instance.RuntimeWeapons.ownedWeapons.Count > 0); // wait until there's at least one weapon ready to display

        PopulateWeaponInventory();
    }

    public GameObject damagePopup;
    public float dmgPopupVariance = 10;

    public void SpawnDamagePopup(Vector3 position, float damage)
    {
        // convert position to screenspace: https://stackoverflow.com/questions/73332594/convert-world-space-gameobject-position-to-screen-space-canvas-ui-position
        Vector3 screenPos = Camera.main.WorldToScreenPoint(position);
        Vector3 spawnPos = new Vector2(screenPos.x + UnityEngine.Random.Range(-dmgPopupVariance, dmgPopupVariance), screenPos.y + UnityEngine.Random.Range(-dmgPopupVariance, dmgPopupVariance));

        GameObject DamagePopupInstance = Instantiate(damagePopup, spawnPos, Quaternion.identity, this.transform);

        DamagePopupInstance.transform.GetChild(0).GetComponent<TMP_Text>().SetText(damage.ToString());
    }

    public TMP_Text WaveCounterTxt;
    public void UpdateWaveCounter(int waveNum)
    {
        WaveCounterTxt.text = $"Wave {waveNum.ToString()}";
    }

    public TMP_Text BloodCounterTxt;
    public void UpdateBloodCounter(float bloodNum)
    {
        // round blood num to 2 decimal places
        BloodCounterTxt.text = $"{bloodNum.ToString("#.00")} mL";
    }

    // populate weapon inventory based on owned weapons
    #region Weapon Inventory UI
    public GameObject WeaponInvSlotPrefab;
    public Transform WeaponInvParent;

    public void PopulateWeaponInventory()
    {
        // clear existing slots
        foreach (Transform child in WeaponInvParent)
        {
            Destroy(child.gameObject);
        }
        foreach (var weapon in PlayerInvManager.instance.ownedWeaponDB.ownedWeapons)
        {
            GameObject slot = Instantiate(WeaponInvSlotPrefab, WeaponInvParent);
            slot.GetComponent<WeaponInvSlot>().Initialize(weapon);
        }
    }
    #endregion
}
