using JetBrains.Annotations;
using UnityEngine;

public class DestroyOnAnimEvent : MonoBehaviour
{
    public void DestroyParent()
    {
        GameObject parent = gameObject.transform.parent.gameObject;
        Destroy(parent);
    }
}
