using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialHUD : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public List<string> lines;
    public float textSpeed;

    private int index;

    private void OnEnable()
    {
        EventManager<string>.Instance.StartListening("onPlayDialogue", SetDialogueLine);
    }

    void Update()
    {
        if (Input.anyKeyDown && TutorialGameController.Instance.state == GameState.TUTORIAL)
        {
            if (textComponent.text == lines[index])
            {
                NextLine();
            }
            else
            {
                StopAllCoroutines();
                textComponent.text = lines[index];
                TutorialGameController.Instance.t = 0;
            }
        }
    }

    void StartDialogue()
    {
        textComponent.text = string.Empty;
        index = 0;
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        foreach (char c in lines[index].ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSecondsRealtime(textSpeed);
        }
    }

    void NextLine()
    {
        if (index < lines.Count - 1)
        {
            index++;
            textComponent.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        else
        {
            //gameObject.SetActive(false);
            textComponent.text = string.Empty;
            TutorialGameController.Instance.t = 0;
            EventManager<GameState>.Instance.TriggerEvent("onStateChanged", GameState.IDLE);
        }
    }

    private void SetDialogueLine(string name)
    {
        TutorialDialogueScriptable SO = ResourceSystem.Instance.GetDialogueLines(name);
        lines.Clear();
        lines = new List<string>(SO.lines);
        StartDialogue();
    }
}
