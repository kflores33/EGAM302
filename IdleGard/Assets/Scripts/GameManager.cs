using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else { Destroy(this); }
    }

    // tell spawner when to spawn
    // house references & inter-script functions
    // manage game state

    private void Update()
    {

    }
}
