using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TutorialHUD : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public List<string> lines;
    public float textSpeed;

    private int index;
    [SerializeField] GameObject dialogueArrow;

    private void OnEnable()
    {
        EventManager<string>.Instance.StartListening("onPlayDialogue", SetDialogueLine);
    }

    private void OnDisable()
    {
        EventManager<string>.Instance.StopListening("onPlayDialogue", SetDialogueLine);
    }

    private void Update()
    {
        if (Input.anyKeyDown && GameController.Instance.State == GameState.TUTORIAL && textComponent != null)
        {
            if (textComponent.text == lines[index])
            {
                NextLine();
            }
            else
            {
                StopAllCoroutines();
                textComponent.text = lines[index];
            }
        }

        if (textComponent != null && index < lines.Count)
        {
            dialogueArrow.SetActive(textComponent.text == lines[index]);
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
            textComponent.text = string.Empty;
            SlowMotionController.Instance.t = 0;
            EventManager<GameState>.Instance.TriggerEvent("onStateChanged", GameState.PLAYING);
        }
    }

    private void SetDialogueLine(string name)
    {
        EventManager<bool>.Instance.TriggerEvent("jumpMovement", false);

        if (textComponent != null)
        {
            ScriptableTutorialDialogue SO = ResourceSystem.Instance.GetDialogueLines(name);
            lines.Clear();
            lines = new List<string>(SO.lines);
            textComponent.transform.localPosition = SO.position;
            StartDialogue();
        }
    }
}
