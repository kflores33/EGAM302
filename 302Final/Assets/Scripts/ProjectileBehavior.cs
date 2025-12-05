using System.Collections;
using UnityEngine;

public class ProjectileBehavior : MonoBehaviour
{
    public float startDelay = 0.6f;
    public float speed = 16f;
    public Vector3 direction;

    public float lifetime = 10f;

    bool canGo = false;

    bool isBlocked = false;

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

    public void OnBlock()
    {
        canGo = false;
        isBlocked = true; 

        StartCoroutine(OnBlockCoroutine(1));
    }

    IEnumerator OnBlockCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}
