using UnityEngine;

public class WeaponBehavior : MonoBehaviour
{
    public WeaponScriptable weaponData; // reference to weapon data scriptable object

    public bool draggable;

    private void Update()
    {
        if (draggable)
        {
            Vector3 draggedPos = Input.mousePosition;
            draggedPos.z = -6;

            transform.position = draggedPos;
        }
    }
}
