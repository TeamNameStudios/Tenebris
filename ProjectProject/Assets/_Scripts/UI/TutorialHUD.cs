using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialHUD : MonoBehaviour
{
    [SerializeField] private List<GameObject> tutorialTextList = new List<GameObject>();
    private int tutorialIndex = 0;

    private void Update()
    {
        if (Input.anyKeyDown && TutorialGameController.Instance.state == GameState.TUTORIAL)
        {
            NextDialogue();
        }
    }

    private void NextDialogue()
    {
        if (tutorialIndex + 1 <= tutorialTextList.Count)
        {
            return;
        }

        tutorialTextList[tutorialIndex].SetActive(false);

        tutorialIndex++;

        tutorialTextList[tutorialIndex].SetActive(true);
    }
}
