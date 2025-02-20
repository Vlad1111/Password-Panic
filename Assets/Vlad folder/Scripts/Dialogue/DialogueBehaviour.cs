using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueBehaviour : MonoBehaviour
{
    public static DialogueBehaviour Instance;
    private void Awake()
    {
        Instance = this;
    }

    public LineDisplayController lineController;
    public Transform lineParent;

    public List<string> lines = new List<string>();
    public List<string> commands = new List<string>();
    public int currentLine = 0;
    private void Start()
    {
        lineParent.gameObject.SetActive(false);
    }

    private void ComputeCommand(string command)
    {
        if (command == "") return;
    }

    public void LineEnded()
    {
        if(currentLine >= lines.Count - 1)
        {
            GameBehaviour.Instance.DialogueChengedState(false);
            lineParent.gameObject.SetActive(false);
            return;
        }
        currentLine++;
        ComputeCommand(commands[currentLine]);
        lineController.showLine(lines[currentLine]);
    }

    private void StartDialogue()
    {
        lineController.showLine(lines[currentLine]);
        lineParent.gameObject.SetActive(true);
        GameBehaviour.Instance.DialogueChengedState(true);
    }

    public void ShowLine(string line)
    {
        lines = new List<string>(new[] {line});
        commands = new List<string>(new[] { "" });
        currentLine = 0;
        StartDialogue();
    }

    public void ShowDialogue(string[] linesCommands)
    {
        commands.Clear();
        lines.Clear();

        bool alreadyAddedCommand = false;
        foreach(string l in linesCommands)
        {
            if (l.Length > 2 && l[0]=='(' && l[l.Length-2]==')')
            {
                var c = l.Substring(1, l.Length - 2);
                commands.Add(c);
                alreadyAddedCommand = true;
            }
            else
            {
                lines.Add(l);
                if(!alreadyAddedCommand)
                    commands.Add("");
                alreadyAddedCommand = false;
            }
        }
        
    }
}
