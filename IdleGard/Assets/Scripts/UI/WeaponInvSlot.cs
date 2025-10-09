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

        if (saveData != null)
        {
            EXPBar.value = saveData.killCount;

            if (saveData.currentLevel >= weaponData.levels.Count - 1)
            {
                LevelProgress.text = "MAX LEVEL";
                EXPBar.value = EXPBar.maxValue;
            }
            else
                LevelProgress.text = $"{saveData.killCount}/{weaponData.levels[saveData.currentLevel].kills_to_level} Kills until Level {saveData.currentLevel + 2}";

            WeaponLevel.text = $"Level {saveData.currentLevel + 1}";
        }
    }

    public void Initialize(OwnedWeapon weapon)
    {
        saveData = weapon;
        weaponData = weapon.weaponData;

        WeaponName.text = weaponData.wpnname;
        WeaponType.text = weaponData.levels[0].weight;
        WeaponLevel.text = $"Level {weapon.currentLevel + 1}";

        if (weapon.currentLevel >= weaponData.levels.Count - 1)
        {
            EXPBar.maxValue = weaponData.levels[2].kills_to_level;
        }
        else EXPBar.maxValue = weaponData.levels[weapon.currentLevel].kills_to_level;

        EXPBar.minValue = 0;
    }
}
