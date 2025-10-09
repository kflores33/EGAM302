using UnityEngine;
using TMPro;

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

    public GameObject damagePopup;
    public float dmgPopupVariance = 10;

    public void SpawnDamagePopup(Vector3 position, float damage)
    {
        // convert position to screenspace: https://stackoverflow.com/questions/73332594/convert-world-space-gameobject-position-to-screen-space-canvas-ui-position
        Vector3 screenPos = Camera.main.WorldToScreenPoint(position);
        Vector3 spawnPos = new Vector2(screenPos.x + Random.Range(-dmgPopupVariance, dmgPopupVariance), screenPos.y + Random.Range(-dmgPopupVariance, dmgPopupVariance));

        GameObject DamagePopupInstance = Instantiate(damagePopup, spawnPos, Quaternion.identity, this.transform);

        DamagePopupInstance.transform.GetChild(0).GetComponent<TMP_Text>().SetText(damage.ToString());
    }

    // populate weapon inventory based on owned weapons
}
