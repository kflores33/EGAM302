using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShopInvManager : MonoBehaviour
{
    public static ShopInvManager instance;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this);

        ownedWeaponDB = SaveManager.instance.RuntimeWeapons; // make sure the correct thing is referenced
    }
    public OwnedWeaponDB ownedWeaponDB;
    public WeaponDatabase weaponDatabase;

    // ummmm. actually why did i make this script again?
    // oh right. 
    // when a weapon is bought, remove the weapon from the shop list and add it to the owned weapon list

    public void OnWeaponBought(string weapon_id)
    {
        if (weaponDatabase == null) return;

        // check if owned 
        if (ownedWeaponDB.GetWeapon(weapon_id) == null) // check if the weapon isn't already owned
        {
            WeaponScriptable weaponToAdd = weaponDatabase.GetWeaponById(weapon_id);
            ownedWeaponDB.AddWeapon(weaponToAdd); // add to owned weapon list
            
            ShopWeapon weaponToRemove = ownedWeaponDB.GetShopWeapon(weapon_id);
            ownedWeaponDB.RemoveShopWeapon(weaponToRemove); // remove from shop list

            GeneralUIHandler.instance.PopulateWeaponShop(); // refresh shop UI
            GeneralUIHandler.instance.PopulateWeaponInventory(); // refresh inventory UI
        }
    }

    public void AddRandomWeaponToShop()
    {
        // old method, which could get stuck in an infinite loop if all weapons were owned/shop (also just inefficient)
        //WeaponScriptable randomWeapon = weaponDatabase.GetRandomWeapon();
        //if (randomWeapon == null) return;

        //int attempts = 0;
        //while (attempts < 66)
        //{
        //    if (ownedWeaponDB.ownedWeapons.Any(o => o.weapon_id == randomWeapon.weapon_id) || ownedWeaponDB.shopWeapons.Any(s => s.weapon_id == randomWeapon.weapon_id))
        //    {
        //        AddRandomWeaponToShop(); // try again
        //        return;
        //    }
        //}

        // Get all possible weapons from the database
        List<WeaponScriptable> allWeapons = weaponDatabase.allWeapons; 

        // Filter out weapons already owned or already in shop
        var availableWeapons = allWeapons
            .Where(w => // "keep only the elements that meet the following conditions"
                !ownedWeaponDB.ownedWeapons.Any(o => o.weapon_id == w.weapon_id) &&
                !ownedWeaponDB.shopWeapons.Any(s => s.weapon_id == w.weapon_id)) // check if the weapon is NOT in either list
            .ToList(); // convert this to a new list

        if (availableWeapons.Count == 0)
        {
            Debug.LogWarning("No available weapons left to add to the shop!");
            return;
        }

        // Pick a random one from the filtered list
        WeaponScriptable randomWeapon = availableWeapons[Random.Range(0, availableWeapons.Count)];

        ownedWeaponDB.AddShopWeapon(randomWeapon);
        ShopWeapon shopWpn = ownedWeaponDB.GetShopWeapon(randomWeapon.weapon_id);
        GeneralUIHandler.instance.UpdateWeaponShop(shopWpn);
    }
}
