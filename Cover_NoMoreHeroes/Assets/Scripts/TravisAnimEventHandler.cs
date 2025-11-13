using UnityEngine;
using UnityEngine.Events;

public class TravisAnimEventHandler : MonoBehaviour
{
    public enum QueueAnimEvent
    {
        CanQueueCombo,
        CanPlayQueued,
        CanPlayNonCombo
    }
    public QueueAnimEvent currentQueueEvent;
    //public QueueAnimEvent previousQueueEvent;

    public UnityEvent OnCanQueueCombo;
    public UnityEvent OnCanPlayQueued;
    public UnityEvent OnCanPlayNonCombo;

    void HandleQueueAnimEvent(int currentState)
    {
        currentQueueEvent = (QueueAnimEvent)currentState;

        switch (currentQueueEvent)
        {
            case QueueAnimEvent.CanQueueCombo:
                OnCanQueueCombo?.Invoke();
                //Debug.Log("Attack Animation Started");
                break;
            case QueueAnimEvent.CanPlayQueued:
                OnCanPlayQueued?.Invoke();
                //Debug.Log("Next Attack can Play");
                break;
            case QueueAnimEvent.CanPlayNonCombo:
                OnCanPlayNonCombo?.Invoke();
                //Debug.Log("Combo dropped, can play 1st attack");
                break;
        }
    }

    public UnityEvent<float> OnApplyMoveForce;
    void MoveForceAnimEvent(float moveForce)
    {
        OnApplyMoveForce?.Invoke(moveForce);
        Debug.Log("Applying Move Force: " + moveForce);
    }

    public UnityEvent<bool> OnApplyRunLoop;
    void RunProgress(int canLoop)
    {
        bool loop = canLoop == 1 ? true : false;

        OnApplyRunLoop?.Invoke(loop);
    }
}
