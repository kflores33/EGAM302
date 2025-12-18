using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    public static GameManager instance { get; private set; }

    public UnityEvent onIntroEnd;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this);
        }
    }

    public enum GameState
    {
        Intro,
        Gameplay
    }
    public GameState currentGameState = GameState.Intro;

    private void Start()
    {
        PlayerInputManager.Instance.DisableGameplayInput();
    }

    public void OnIntroEnd()
    {
        currentGameState = GameState.Intro;

        onIntroEnd?.Invoke();
    }
}
