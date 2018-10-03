using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueOuput : MonoBehaviour, IOutputabble
{
    [SerializeField] Text dialogueText;

    void Awake()
    {
        GameOuput.SetOutput(this);
    }

    public void Output(string output)
    {
        dialogueText.text = output;
    }
}
