using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class WeaponInvSlot : MonoBehaviour
{
    public WeaponScriptable weaponData;
    public OwnedWeapon saveData;

    public bool weaponIsActive;

    // UI Elements
    public Image image;
    public TMP_Text WeaponName;
    public TMP_Text WeaponType;
    public TMP_Text WeaponLevel;
    public Slider EXPBar;
    public TMP_Text LevelProgress;

    public Image BKGDPanel;

    public Color panelActive;
    public Color panelInactive;

    private void Start()
    {
        if (weaponData == null) { Debug.LogError("WeaponInvSlot missing WeaponScriptable reference"); return; }

        // weapon level stuff later
    }
    private void Update()
    {
        if (weaponIsActive)
        {
            BKGDPanel.color = panelActive;
        }
        else { BKGDPanel.color = panelInactive; }

        EXPBar.value = saveData.killCount;
    }

    public void Initialize(OwnedWeapon weapon)
    {
        saveData = weapon;
        weaponData = PlayerInvManager.instance.weaponDatabase.GetWeaponById(weapon.weapon_id);

        WeaponName.text = weaponData.wpnname;
        WeaponType.text = weaponData.levels[0].weight;
        WeaponLevel.text = (weapon.currentLevel + 1).ToString();

        EXPBar.maxValue = weaponData.levels[weapon.currentLevel].kills_to_level;
        EXPBar.minValue = 0;
    }
}
