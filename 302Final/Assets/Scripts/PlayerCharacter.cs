using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCharacter : MonoBehaviour
{
    public enum ParryState
    {
        None, // no parry being performed
        Anticipating, // player has initiated parry input, waiting for attack (automatically times out after a short duration)
        Blocking, // player is successfully parrying an attack (state ends when animation finishes)
        Overcommit // (optional) player is following through with parry after a successful block (cannot parry additional attacks during this state)
    }
    public ParryState currentParryState = ParryState.None;

    bool isPerfectParry;

    // parry ability
    public void OnInitialParry()
    {
        // when player performs parry input
        // set parry state to InitialParry
    }
    public void OnOvercommitParry()
    {

    }

    private void Update()
    {
        
    }
}
