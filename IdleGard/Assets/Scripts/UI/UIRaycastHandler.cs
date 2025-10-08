
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class UIRaycastHandler : MonoBehaviour 
{
    public static UIRaycastHandler instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else { Destroy(this); }
    }

    [SerializeField]GraphicRaycaster raycaster;
    [SerializeField]EventSystem eventSystem;
    PointerEventData eventData;

    public DetailHoverPanel detailHoverPanel;

    private void Start()
    {
        if (raycaster == null)
        {
            if ( GetComponent<GraphicRaycaster>() ==null)
            {
                Debug.LogError("Cannot find reference to GraphicRaycaster");
            }
            raycaster = GetComponent<GraphicRaycaster>();
        }
        if (eventSystem == null)
        {
            if (GetComponent<EventSystem>() == null)
            {
                Debug.LogError("Cannot find reference to EventSystem");
            }
            eventSystem = GetComponent<EventSystem>();
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // new pointer event
            eventData = new PointerEventData(eventSystem);
            // set pointer event position to mouse position
            eventData.position = Input.mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();
            raycaster.Raycast(eventData, results);

            foreach (RaycastResult result in results) 
            {
                GameObject hitObject = result.gameObject;
                // below is where different raycast behaviors can be defined.
                if (hitObject.GetComponent<WeaponInvSlot>() != null) { 

                    Debug.Log($"hit {hitObject}");

                    if (!hitObject.GetComponent<WeaponInvSlot>().weaponIsActive)
                    {                   
                        SpawnerBehavior.instance.SpawnWeapon(Input.mousePosition, hitObject.GetComponent<WeaponInvSlot>().weaponData, hitObject.GetComponent<WeaponInvSlot>());
                        hitObject.GetComponent<WeaponInvSlot>().weaponIsActive = true;
                    }

                    break;
                }
            }
        }

        // CURSOR HOVER 
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hitInfo))
        {
            if (hitInfo.collider == null) { detailHoverPanel.HideHoverPanel() ; return; }

            if (hitInfo.collider.GetComponentInParent<WeaponBehavior>() != null)
            {
                Debug.Log("weapon found. hover box active");
                WeaponBehavior wpn = hitInfo.collider.GetComponentInParent<WeaponBehavior>();
                detailHoverPanel.ShowHoverPanel(detailHoverPanel.GetInfoWeapon(wpn));
            }
            else { detailHoverPanel.HideHoverPanel(); }
        }
    }
}
