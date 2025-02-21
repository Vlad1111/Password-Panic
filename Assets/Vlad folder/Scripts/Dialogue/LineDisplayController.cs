using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class LineDisplayController : MonoBehaviour
{
    [System.Serializable]
    public class LineVoiceSoundData
    {
        public string name;
        public int voiceIndex;
        public float voiceTemper = 1;
    }
    public TMP_Text lineText;
    public UnityEvent onLineEnd;
    [SerializeField] private float textSpeed;
    private float initialSpeed = 0.04f;
    private int initialSize = -1;

    //private bool wasParentActive = true;
    //private bool wasOneLine = false;
    public string line;
    private string[] segments;
    private string savedText = "";
    private Stack<string> endTags = new Stack<string>();
    private int curentLetter = 0;
    private int curentSegmentLetter = 0;
    private int curentSegment = 0;
    private float timeSinceLastLetter = 0;

    private float autoPlayTime = -1;


    private void Awake()
    {
        initialSpeed = textSpeed;
    }
    public List<string> SplitIntoTags(string line)
    {
        List<string> subtgs = new List<string>();
        int lst = 0;
        for (int i = 0; i < line.Length; i++)
        {
            if (line[i] == '<')
            {
                if (i != 0 && line[i - 1] == '\\')
                    continue;
                int curr = i;
                while (i < line.Length && line[i] != '>')
                    i++;
                if (i < line.Length && line[i] == '>')
                {
                    if (curr > lst)
                    {
                        subtgs.Add(line.Substring(lst, curr - lst));
                    }
                    subtgs.Add(line.Substring(curr, i - curr + 1));
                    lst = i + 1;
                }
            }
        }
        subtgs.Add(line.Substring(lst, line.Length - lst));
        subtgs.Remove("");
        return subtgs;
    }

    private void preprocessLine(string line)
    {
        var s = line;
        for (int i = 0; i < s.Length; i++)
            if (s[i] == '[')
            {
                if (i != 0 && s[i - 1] == '\\')
                    continue;
                int nr = 1;
                int p = i + 1;
                while (p < s.Length && nr > 0)
                {
                    if (s[p] == '[') nr++;
                    else if (s[p] == ']') nr--;
                    p++;
                }
                if (p - 1 < s.Length && s[p - 1] == ']')
                {
                    var sus = s.Substring(i + 1, p - i - 2);
                    var newS = sus;// GameBahaviour.instance.getSubtituteString(sus);
                    s = s.Replace("[" + sus + "]", newS);
                    i += newS.Length - 1;
                }
                else break;
            }
        s = s.Replace("\\<", "<");
        line = s.Replace("\\[", "[");

        segments = SplitIntoTags(line).ToArray();
        this.line = line;
    }
    public void showLine(string line, bool notTheLastLine = false, bool wasOneLine = false)
    {
        //MenuController.instance.addInteracteblesLock("line");
        //wasParentActive = gameObject.activeSelf || notTheLastLine;
        //this.wasOneLine = wasOneLine;
        gameObject.SetActive(true);
        preprocessLine(line);
        endTags.Clear();
        savedText = "";
        lineText.text = "";
        curentLetter = 0;
        curentSegment = 0;
        curentSegmentLetter = 0;
    }
    public void endLine()
    {
        //MenuController.instance.removeInteracteblesLock("line");
        //if (wasParentActive == false)
        //    gameObject.SetActive(wasParentActive);
        if(onLineEnd != null)
            onLineEnd.Invoke();
    }

    public void hideLine()
    {
        //MenuController.instance.removeInteracteblesLock("line");
        gameObject.SetActive(false);
        setSize(1);
        setSpeed(1);
    }

    private bool wasFirePressed = false;

    private void VerifyCurentSegmentForTagAndAdd()
    {
        while (curentSegment < segments.Length && segments[curentSegment][0] == '<')
        {
            if (segments[curentSegment].Last() != '>')
                break;
            if(segments[curentSegment][1] == '/' || segments[curentSegment][1] == '\\')
            {
                savedText += endTags.Pop();
            }
            else
            {
                savedText += segments[curentSegment];
                endTags.Push("</" + segments[curentSegment].Substring(1));
            }
            curentSegment++;
        }
    }

    private void nextLetter()
    {
        VerifyCurentSegmentForTagAndAdd();
        float speed = textSpeed;// (skip.isOn ? textSpeed / 30 : textSpeed);
        if (timeSinceLastLetter < speed)
        {
            timeSinceLastLetter += Time.deltaTime;
        }
        else
        {
            //voiceSounds[voiceIndex].Play(1f * lineText.fontSize / initialSize, voiceTemper);
            int nrLetter = (int)(Time.deltaTime / speed);
            if (nrLetter < 1)
                nrLetter = 1;
            //for (int k = 0; k < nrLetter && curentLetter < line.Length; k++)
            //{
            //    lineText.text += line[curentLetter];
            //    curentLetter++;
            //}
            //timeSinceLastLetter = 0;
            //Debug.Log(nrLetter + " " + curentSegment + " " + curentLetter);
            while(nrLetter > 0)
            {
                if (curentSegment >= segments.Length)
                {
                    lineText.text = line;
                    break;
                }
                for (int k = 0; nrLetter > 0 && curentSegmentLetter < segments[curentSegment].Length; k++)
                {
                    //lineText.text += segments[curentSegment][curentSegmentLetter];
                    curentSegmentLetter++;
                    nrLetter--;
                }

                string middle = segments[curentSegment].Substring(0, curentSegmentLetter);
                lineText.text = savedText + middle + string.Join("", endTags.ToArray());

                if (nrLetter > 0)
                {
                    savedText += segments[curentSegment];
                    curentSegment++;
                    curentSegmentLetter = 0;
                    VerifyCurentSegmentForTagAndAdd();
                }
            }
            curentLetter = lineText.text.Length;
            timeSinceLastLetter = 0;
        }
    }

    internal void SetOneAutoPlayLineTime(float time)
    {
        autoPlayTime = time;
    }

    private bool? lastVerifyIsScreenPressedValue = null;
    private bool IsScreenPressed()
    {
        if(lastVerifyIsScreenPressedValue != null)
            return lastVerifyIsScreenPressedValue.Value;
        if (Input.GetAxis("Fire1") > 0.1)
        {
            //Ray ray = Camera.main.ScreenPointToRay();
            var poz = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var hitInfo = Physics2D.Raycast(poz, Vector2.zero);
            if (hitInfo)
            {
                //Debug.Log(hitInfo.collider.gameObject.tag);
                if (hitInfo.collider.gameObject.tag == "CaracterAltView")
                {
                    lastVerifyIsScreenPressedValue = false;
                    return false;
                }
            }
        }
        lastVerifyIsScreenPressedValue = Input.GetAxis("Fire1") > 0.1 || Input.GetAxis("Jump") > 0.1;
        return lastVerifyIsScreenPressedValue.Value;
    }

    internal void setSpeed(float speed)
    {
        textSpeed = speed * initialSpeed;
    }

    public void SetGlobalSpeed(float globalSpeed)
    {
        initialSpeed = globalSpeed;
        textSpeed = globalSpeed;
    }

    public void setSize(float size)
    {
        //if(initialSize < 0)
        //    initialSize = lineText.fontSize;
        int s = (int)(size * initialSize);
        if (size != 0 && s == 0)
            s = 1;
        lineText.fontSize = s;
    }

    void Update()
    {
        //if (MenuController.instance.IsMenuOpened())
        //    return;
        if (curentLetter < line.Length)
        {
            if (IsScreenPressed())
            {
                if (!wasFirePressed)// || skip.isOn )
                {
                    lineText.text = line;
                    curentLetter = line.Length;
                }
                wasFirePressed = true;
            }
            else
            {
                wasFirePressed = false;
                nextLetter();
            }
        }
        else if (curentLetter >= line.Length)
        {
            bool autoPlay = false;
            if (autoPlayTime >= 0)
            {
                autoPlayTime -= Time.deltaTime * textSpeed;
                if (autoPlayTime < 0)
                {
                    autoPlayTime = -1;
                    autoPlay = true;
                }
            }
            if (IsScreenPressed() || autoPlay)// || skip.isOn)
            {
                if (!wasFirePressed)// || skip.isOn)
                {
                    curentLetter++;
                    autoPlayTime = -1;
                    endLine();
                }
                wasFirePressed = true;
            }
            else wasFirePressed = false;
        }
        else endLine();
        lastVerifyIsScreenPressedValue = null;
    }
}
