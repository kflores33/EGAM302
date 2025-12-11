using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem.XR.Haptics;

public class ProjectileBehavior : MonoBehaviour
{
    public float startDelay = 0.6f;
    public float speed = 16f;
    public float parriedSpeedMultiplier = 5;
    public float parriedSpeed => speed * parriedSpeedMultiplier;
    public Vector3 direction;

    public float lifetime = 10f;

    public int damageToDeal = 20;

    bool canGo = false;

    public enum ObjectState
    {
        None,
        Blocked,
        Parried
    }
    public ObjectState currentState;

    private void Start()
    {
        StartCoroutine(WaitToMove());
    }

    private void Update()
    {
        if (canGo) 
        {
            if (currentState == ObjectState.None)
            {
                transform.Translate(direction.normalized * speed * Time.deltaTime);
            }
            else if (currentState == ObjectState.Parried)
            {
                transform.Translate(direction.normalized * parriedSpeed * Time.deltaTime, Space.World);
            }
        }

        lifetime -= Time.deltaTime;
        if (lifetime <= 0f)
        {
            Destroy(gameObject);
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
        currentState = ObjectState.Blocked;

        StartCoroutine(OnBlockCoroutine(1));
    }
    public void OnParried(Vector3 newDirection)
    {
        canGo = true;
        currentState = ObjectState.Parried;

        Debug.Log($"new direction: {newDirection}");

        newDirection.y = 0f;
        direction = newDirection;
    }

    IEnumerator OnBlockCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        Destroy(gameObject);
    }
}
