using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ComputerBehaviour : MonoBehaviour
{
    public Animator LaserAnimator;
    public EmailBehaviour emailBehaviour;

    public float scale = 0;
    private float speed;

    private string[] profanaties = new[]
    {
        "fuck",
        "shit",
        "cunt",
        "vagina",
        "penis",
        "dick",
        "nigger",
        "retarded",
        "bitch",
        "bastard",
        "clint",
        "whore",
        "piss",
        "fucker",
        "shitting",
        "ass",
        "arse",
        "wanker",
        "nob"
    };

    private void Start()
    {
        var all = GetComponentsInChildren<TMP_InputField>(true);
        foreach (var a in all)
        {
            var input = a;
            a.onValueChanged.AddListener((x) => { CheckForProfanaty(input); });
        }
    }

    private void OnEnable()
    {
        OpenComputer(); 
    }
    public void OpenComputer()
    {
        scale = 1;
        speed = 5;
        transform.localScale = new Vector3(1, 0, 1);
        gameObject.SetActive(true);
    }

    public void CloseComputer()
    {
        scale = 0.001f;
        speed = -10;
        transform.localScale = new Vector3(1, 1, 1);
        ScreenBahaviour.Instance.SetCameraLocation("Desk");
        SoundManager.Instance.PlayClip("Click");

        if(GameBehaviour.GetGlobalValue(GameVariableKeys.LaserCharged.ToString()) >= 0.5f)
            if(GameBehaviour.GetGlobalValue("Animation_LaserArmed") < 0.5f)
            {
                LaserAnimator.Play("LaserArm");
                GameBehaviour.SetGlobalValue("Animation_LaserArmed", 1);
            }
    }

    private void CheckForProfanaty(TMP_InputField field)
    {
        var text = field.text;
        bool wasChanged = false;
        foreach (var profanaty in profanaties)
        {
            //Debug.Log(text + " " + profanaty + " " + text.Contains(profanaty));
            if (text.Contains(profanaty))
            {
                wasChanged = true;
                text = text.Replace(profanaty, "");
            }
        }
        if(wasChanged)
            field.SetTextWithoutNotify(text);
    }

    private void Update()
    {
        if(scale > 0)
        {
            transform.localScale = new Vector3(1, 1 - scale, 1);
            scale -= Time.deltaTime * speed;
            if(scale <= 0)
            {
                scale = -1;
                transform.localScale = new Vector3(1, 1, 1);
            }
            if(scale > 1)
            {
                scale = -1;
                transform.localScale = new Vector3(1, 0, 1);
                gameObject.SetActive(false);
            }
        }
    }
}
