using System.Collections;
using UnityEngine;

public class ProjectileBehavior : MonoBehaviour
{
    public float startDelay = 0.3f;
    public float speed = 16f;
    public Vector3 direction;

    public float lifetime = 10f;

    bool canGo = false;

    private void Start()
    {
        StartCoroutine(WaitToMove());
    }

    private void Update()
    {
        if (canGo) { 
            transform.Translate(direction.normalized * speed * Time.deltaTime);
            lifetime -= Time.deltaTime;
            if (lifetime <= 0f)
            {
                Destroy(gameObject);
            }
        }
    }

    // inherit speed and direction from shooter

    IEnumerator WaitToMove()
    {
        yield return new WaitForSeconds(startDelay);
        canGo = true;
    }
}
