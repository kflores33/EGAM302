using System.Collections;
using UnityEngine;

public class GroundEruptBehavior : MonoBehaviour
{
    // spawn at player position. 
    // wait a few seconds until actually attacking (i.e. enable trigger)
    // needs to play an animation to indicate...quick flashing color (red?) that speeds up until triggering

    public int damageToDeal = 0;
    public float delayTime = 3;
    public float delayTimeElapsed;

    public GameObject IndicatorToGrow;
    public GameObject IndicatorBounds;

    Vector3 IndicatorStartScale;

    Coroutine waitToDestroyCoroutine;

    Collider col;

    private void Start()
    {
        IndicatorStartScale = IndicatorToGrow.transform.localScale;
        col = GetComponent<Collider>();

        col.enabled = false;
    }

    private void Update()
    {
        if (delayTimeElapsed <= 3)
        {
            delayTimeElapsed += Time.deltaTime;

            float t = delayTimeElapsed / delayTime;
            IndicatorToGrow.transform.localScale = Vector3.Lerp(IndicatorStartScale, IndicatorBounds.transform.localScale, t);
        }
        else
        {
            col.enabled = true;
            if (waitToDestroyCoroutine == null) 
            {
                waitToDestroyCoroutine = StartCoroutine(WaitToDestroy());
            }
        }
    }

    IEnumerator WaitToDestroy()
    {
        yield return new WaitForSeconds(1);

        Destroy(gameObject);
    }

    IEnumerator IndicatorScaleCoroutine(Vector3 targetScale)
    {
        Vector3 startScale = transform.localScale;
        float currentTime = 0.0f;

        do
        {
            // Calculate the interpolation point 't' based on elapsed time
            currentTime += Time.deltaTime;
            float t = currentTime / delayTime;
            // Optional: add an easing function, like Mathf.SmoothStep(0f, 1f, t)

            transform.localScale = Vector3.Lerp(startScale, targetScale, t);

            yield return null; // Wait until the next frame
        } while (currentTime <= delayTime);

        // Ensure the final scale is exactly the target scale
        transform.localScale = targetScale;
    }
}
