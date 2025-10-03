using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class WeaponInvSlot : MonoBehaviour
{
    public WeaponScriptable weaponData;

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

        WeaponName.text = weaponData.wpnname;

        //if (weaponData.type.Count >1) WeaponType.text = $"{weaponData.type[0]}, {weaponData.type[1]}";
        //else WeaponType.text = weaponData.type[0];

        WeaponType.text = weaponData.levels[0].weight;

        // weapon level stuff later
    }
    private void Update()
    {
        if (weaponIsActive)
        {
            BKGDPanel.color = panelActive;
        }
        else { BKGDPanel.color = panelInactive; }

    }
}
