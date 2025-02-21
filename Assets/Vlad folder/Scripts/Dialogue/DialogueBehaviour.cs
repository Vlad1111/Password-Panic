using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class SpriteAnimation
{
    [System.Serializable]
    public class SpriteAnimationFrame
    {
        public Sprite sprite;
        public float time;
    }
    public string name;
    public SpriteAnimationFrame[] frames;
}
public class DialogueBehaviour : MonoBehaviour
{
    public static DialogueBehaviour Instance;
    private void Awake()
    {
        Instance = this;
    }

    public LineDisplayController lineController;
    public Transform lineParent;

    [Space(20)]
    public List<string> lines = new List<string>();
    public List<string> commands = new List<string>();
    public int currentLine = 0;

    [Space(20)]
    public Image characterImage;
    public SpriteAnimation[] spriteAnimation;
    private int animationIndex = -1;
    private int frameIndex = 0;
    private float frameTime = 0;

    [Space(20)]
    public Transform endParent;
    public Transform playAgainParent;
    public Image endImage;
    public SpriteAnimation goodEndAnimation;
    public SpriteAnimation badEndAnimation;
    private int endIndex = -1;
    private int endFrameIndex = 0;
    private float endFrameTime = 0;

    private void Start()
    {
        //lineParent.gameObject.SetActive(false);
    }

    private void ComputeCommand(string command)
    {
        if (command == "") return;
        var cmds = command.Split(' ');
        switch (cmds[0].ToLower())
        {
            case "auto":
                lineController.SetOneAutoPlayLineTime(float.Parse(cmds[1]));
                break;
            case "speed":
                lineController.setSpeed(float.Parse(cmds[1]));
                break;
            case "frame":
                animationIndex = -1;
                for (int i = 0; i < spriteAnimation.Length; i++)
                    if (spriteAnimation[i].name == cmds[1])
                    {
                        animationIndex = i;
                        frameIndex = 0;
                        frameTime = 0;
                        break;
                    }
                break;
        }
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
        animationIndex = -1;
        currentLine = 0;
        ComputeCommand(commands[currentLine]);
        lineController.showLine(lines[currentLine]);
        lineParent.gameObject.SetActive(true);
        GameBehaviour.Instance.DialogueChengedState(true);
    }

    public void ShowLine(string line)
    {
        lines = new List<string>(new[] {line});
        commands = new List<string>(new[] { "" });
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
                var c = l.Substring(1, l.Length - 3);
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
        StartDialogue();
    }

    public void ShowDialogueFromFile(string file)
    {
        var path = Path.Join("Dialogue", file);
        var txt = Resources.Load<TextAsset>(path);
        if (txt != null)
            ShowDialogue(txt.ToString().Split('\n'));
    }

    public void PlayGoodEnding()
    {
        GameBehaviour.Instance.StopTimmer();

        endIndex = 1;
        endFrameIndex = 0;
        endFrameTime = 0;

        endParent.gameObject.SetActive(true);
    }

    public void PlayBadEnding()
    {
        GameBehaviour.Instance.StopTimmer();

        endIndex = 2;
        endFrameIndex = 0;
        endFrameTime = 0;

        endParent.gameObject.SetActive(true);
    }

    private void Update()
    {
        if (animationIndex < 0)
        {
            characterImage.sprite = null;
            characterImage.color = new Color(1, 1, 1, 0);
        }
        else
        {
            characterImage.sprite = spriteAnimation[animationIndex].frames[frameIndex].sprite;
            characterImage.color = new Color(1, 1, 1, 1);
            if (spriteAnimation[animationIndex].frames.Length != 1)
            {
                frameTime += Time.deltaTime;
                if (frameTime > spriteAnimation[animationIndex].frames[frameIndex].time)
                {
                    frameTime -= spriteAnimation[animationIndex].frames[frameIndex].time;
                    frameIndex++;
                    if (frameIndex >= spriteAnimation[animationIndex].frames.Length)
                    {
                        frameIndex = 0;
                    }
                }
            }
        }

        if(endIndex > 0)
        {
            var end = endIndex == 1 ? goodEndAnimation : badEndAnimation;
            if(endFrameIndex < end.frames.Length)
            {
                endImage.sprite = end.frames[endFrameIndex].sprite;
                endFrameTime += Time.deltaTime;
                if(endFrameTime > end.frames[endFrameIndex].time)
                {
                    endFrameTime -= end.frames[endFrameIndex].time;
                    endFrameIndex++;
                }
            }
            else
            {
                endFrameIndex = 0;
                playAgainParent.gameObject.SetActive(true);
            }
        }
    }
}
