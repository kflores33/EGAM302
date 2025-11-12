using UnityEngine;
using UnityEngine.Events;

public class AnimEventHandler : MonoBehaviour
{
    public UnityEvent SwingEnded;
    void OnSwingEnd()
    {
        SwingEnded?.Invoke();
    }

    public UnityEvent CanPlayQueued;
    void OnCanPlayQueued()
    {
        CanPlayQueued?.Invoke();
    }

    public UnityEvent IdleStarted;
    void OnIdleStart()
    {
        IdleStarted?.Invoke();
    }
}
