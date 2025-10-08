using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class DetailHoverPanel : MonoBehaviour
{
    public UniversalValues universalValues;

    [SerializeField]GameObject hoverPanelObj;
    [SerializeField]TMP_Text textBox;
    public float targetPositionOffsetX;
    public float targetPositionOffsetY;

    public void Start()
    {
        HideHoverPanel();
    }

    private void Update()
    {
        if (this.gameObject == isActiveAndEnabled)
        {
            // offset the position of the box from the cursor position
            Vector2 targetPos = Input.mousePosition;
            //targetPos.x += targetPositionOffsetX;

            // if position is too close to the edge of the screen, keep it from exceeding the boundaries.
            gameObject.transform.position = targetPos;
        }
    }

    public void ShowHoverPanel(string text)
    {
        textBox.text = text;
        hoverPanelObj.SetActive(true);
    }
    public void HideHoverPanel()
    {
        hoverPanelObj.SetActive(false);
    }

    #region Weapon Text
    public string GetInfoWeapon(WeaponBehavior weapon)
    {
        string WeaponName = weapon.weaponData.wpnname;
        int currentLevelInt = weapon.currentLevel + 1;
        string currentLevel = currentLevelInt.ToString();
        string atkStrength = weapon.weaponData.levels[weapon.currentLevel].attack_strength.ToString();
        string weight = weapon.weaponData.levels[weapon.currentLevel].weight.ToString();
        string atkRate = universalValues.WeightTypes[weight].AttackRate.ToString();

        return WeaponToString(WeaponName, currentLevel, atkStrength, weight, atkRate);
    }
    string WeaponToString(string WeaponName, string currentLevel, string atkStrength, string weight, string atkRate)
    {
        return $"<b>{WeaponName}</b>{Environment.NewLine}Level <b>{currentLevel}</b>{Environment.NewLine}Attack Strength: <b>{atkStrength}</b>" +
            $"{Environment.NewLine}{Environment.NewLine}Weight Class: <b>{weight}</b> (Attacks every <b>{atkRate}</b> seconds)";
    }
#endregion
}
