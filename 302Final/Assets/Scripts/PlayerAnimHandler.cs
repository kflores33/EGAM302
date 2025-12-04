using UnityEngine;
using UnityEngine.Events;

public class PlayerAnimHandler : MonoBehaviour
{
    public enum ParryQueueAnimEvent
    {
        CanQueueInput, // queue player input (can be another parry 
        CanPlayQueued // play animation associated with queued input
    }
    public ParryQueueAnimEvent currentParryReactEvent;

    public UnityEvent OnCanQueueInput;
    public UnityEvent OnCanPlayQueued;

    void HandleParryAnimEvent(int currentState)
    {
        currentParryReactEvent = (ParryQueueAnimEvent)currentState;

        switch (currentParryReactEvent)
        {
            case ParryQueueAnimEvent.CanQueueInput:
                OnCanQueueInput?.Invoke();
                break;
            case ParryQueueAnimEvent.CanPlayQueued:
                OnCanPlayQueued?.Invoke();
                break;
        }
    }

    public UnityEvent<bool> OnApplyRunLoop;
    void RunProgress(int canLoop)
    {
        bool loop = canLoop == 1 ? true : false;

        OnApplyRunLoop?.Invoke(loop);
    }

    public UnityEvent<bool> OnToggleOvercommitListener;
    void ToggleOvercommitListener(int canOvercommit)
    {
        bool acceptOvercommit = canOvercommit == 1 ? true : false;

        OnToggleOvercommitListener?.Invoke(acceptOvercommit);
    }
 }
