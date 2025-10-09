using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponShopSlot : MonoBehaviour
{
    public WeaponScriptable weaponData;
    public ShopWeapon priceInfo;

    // UI Elements
    public Image image;
    public TMP_Text WeaponName;
    public TMP_Text WeaponType;
    public TMP_Text AttackStrength;
    public TMP_Text Price;
    public Button PurchaseButton;

    public Color canPurchase;
    public Color notEnough;

    bool canBuy;
    private void Update()
    {
        if (SaveManager.instance.BloodAccumulated >= priceInfo.price)
        {
            PurchaseButton.image.color = canPurchase;
            canBuy = true;
        }
        else
        {
            PurchaseButton.image.color = notEnough;
            canBuy=false;
        }
    }

    public void TryToBuy()
    {
        if (canBuy)
        {
            // purchase 
            SaveManager.instance.GainBlood(-priceInfo.price);
            ShopInvManager.instance.OnWeaponBought(priceInfo.weapon_id);
            Destroy(this.gameObject);
        }
        else
        {
            Debug.Log("womp womp. dont have enough blood");
        }
    }

    public void Initialize(ShopWeapon weapon)
    {
        priceInfo = weapon;
        weaponData = weapon.weaponData;
        if (weaponData == null) { Debug.LogError("WeaponShopSlot missing WeaponScriptable reference"); return; }

        WeaponName.text = weaponData.wpnname;
        WeaponType.text = weaponData.levels[0].weight;
        Price.text = $"{weapon.price.ToString("#.00")} mL";
        AttackStrength.text = $"Attack Strength: {weaponData.levels[0].attack_strength}";
    }
}
