using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine.Events;
using Unity.Cinemachine;

public class UIManager : MonoBehaviour
{
    [SerializeField] GameObject BossHP;
    [SerializeField] GameObject PlayerHP;

    [SerializeField] TMP_Text dialogueText;
    [SerializeField] GameObject dialoguePanel;

    public List<string> IntroDialogue = new List<string>();

    public int lastLineIndex;

    public UnityEvent<int> cameraSwitch;
    public UnityEvent onLastDialogueLine;

    private void Start()
    {
        BossHP.SetActive(false);
        PlayerHP.SetActive(false);

        dialogueText.text = IntroDialogue[0];
    }

    private void Update()
    {
        if (PlayerInputManager.Instance.UIClickPressed)
        {
            if(IntroDialogue.Count > lastLineIndex + 1)
            {
                if (lastLineIndex == 3)
                {
                    cameraSwitch?.Invoke(2);
                }

                lastLineIndex = lastLineIndex + 1;
                dialogueText.text = IntroDialogue[lastLineIndex];
            }
            else
            {
                cameraSwitch?.Invoke(3);
                onLastDialogueLine?.Invoke();
            }
        }
    }

    public void StartFight()
    {
        BossHP.SetActive(true);
        PlayerHP.SetActive(true);
        dialoguePanel.SetActive(false);
    }
}
