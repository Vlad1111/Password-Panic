using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class PasswordRuleCheck
{
    public string text;
    public abstract bool Check(string text);
    public override string ToString()
    {
        return text;
    }
}
public class PasswordRuleCheck_MinLenght : PasswordRuleCheck
{
    public int lenght;
    public PasswordRuleCheck_MinLenght(int lenght)
    {
        this.lenght = lenght;
        this.text = "The password needs to me at least " + lenght + " character long";
    }

    public override bool Check(string text) => text.Length >= lenght;
}
public class PasswordRuleCheck_ContainsWords : PasswordRuleCheck
{
    public string[] words;
    public bool isCaseSensitive;
    private bool hasChangedKeyWords = false;
    public PasswordRuleCheck_ContainsWords(string[] words, bool isCaseSensitive)
    {
        this.words = words;
        this.isCaseSensitive = isCaseSensitive;
        this.text = "The password needs to contains one of the following words: ";
        for (int i = 0; i < words.Length; i++)
        {
            if (i != 0)
                this.text += ", ";
            this.text += this.words[i];
        }
        if (this.isCaseSensitive)
            this.text += "\nIt needs to be case sensitive";
        else
        {
            this.text += "\nIt does not need to be case sensitive";
            for (int i = 0; i < this.words[0].Length; i++)
            {
                this.words[i] = this.words[i].ToLower();
            }
        }
    }

    public override bool Check(string text)
    {
        if(!hasChangedKeyWords)
        {
            for(int i=0; i < words.Length; i++)
                this.words[i] = GameBehaviour.Instance.ReplaceKeyWorld(this.words[i]).ToString();
            hasChangedKeyWords = true;
        }
        if(!isCaseSensitive)
            text = text.ToLower();
        foreach (var word in words)
            if (text.Contains(word))
                return true;
        return false;
    }
}
public class PasswordRuleCheck_CheckDigitsSum : PasswordRuleCheck
{
    public int sum;
    public int comparator;
    public PasswordRuleCheck_CheckDigitsSum(int sum, int comparator)
    {
        this.sum = sum;
        this.comparator = comparator;
        if (comparator == 0)
            this.text = "The sum of the digits must be equal with " + sum;
        else if(comparator > 0)
            this.text = "The sum of the digits must be bigger then " + sum;
        else
            this.text = "The sum of the digits must be less then " + sum;
    }

    public override bool Check(string text)
    {
        int allSum = 0;
        foreach (var c in text)
            if (c >= '0' && c <= '9')
                allSum += c - '0';
        //Debug.Log("The digits sum is " + allSum + " " + sum + " " + allSum.CompareTo(this.sum));
        return allSum.CompareTo(this.sum) == comparator;
    }
}
public class PasswordRuleCheck_CheckRegex : PasswordRuleCheck
{
    public string regex;
    private Regex rg;
    public PasswordRuleCheck_CheckRegex(string regex)
    {
        this.regex = regex;
        this.text = "the new password mathces the regex pattern " + regex;
        rg = new Regex(regex);
    }

    public override bool Check(string text)
    {
        return rg.IsMatch(text);
    }
}
public class PasswordRuleCheck_RomanNuberSum : PasswordRuleCheck
{
    public int sum;
    public int comparator;
    private int lastSum = 0;
    public PasswordRuleCheck_RomanNuberSum(int sum, int comparator)
    {
        this.sum = sum;
        this.comparator = comparator;
        if (comparator == 0)
            this.text = "The sum of the roman digits must be equal with " + sum;
        else if (comparator == 1)
            this.text = "The sum of the roman digits must be bigger then " + sum;
        else if (comparator == -1)
            this.text = "The sum of the roman digits must be less then " + sum;
        else if (comparator == 2)
            this.text = "The sum of the roman digits must be divided by " + sum;
        this.text += "\nThis also include the lower diggits";
    }

    public override bool Check(string text)
    {
        text = text.ToLower();
        int allSum = 0;
        foreach (var c in text)
            switch (c)
            {
                case 'i':
                    allSum += 1;
                    break;
                case 'v':
                    allSum += 5;
                    break;
                case 'x':
                    allSum += 10;
                    break;
                case 'l':
                    allSum += 50;
                    break;
                case 'c':
                    allSum += 100;
                    break;
                case 'd':
                    allSum += 500;
                    break;
                case 'm':
                    allSum += 1000;
                    break;
                default:
                    break;
            }
        lastSum = allSum;
        if (comparator >= -1 && comparator <= 1)
            return allSum.CompareTo(this.sum) == comparator;
        else if (comparator == 2)
            return allSum % this.sum == 0;
        return true;
    }

    public override string ToString()
    {
        return text + "\nCurrent sum is " + lastSum;
    }
}
public class LaserWindowBehaviour : MonoBehaviour
{
    public Transform laserWindowParent;

    [Space(20)]
    public Transform enterPasswordParent;
    public TMP_InputField enterPasswordField;

    [Space(20)]
    public Transform changePasswordParent;
    public TMP_InputField newPasswordField;
    public Transform newPasswordErrorParent;
    public Transform newPasswordErrorItem;

    [Space(20)]
    public Transform laserControllParent;
    public Image laserChargeImage;
    public Sprite[] laserChargeSprites;
    public TMP_Text errorLaserText;
    private float laserCarge = 0;
    private float laserChargeDirection = 0;

    public PasswordRuleCheck[] passwordChecks = new PasswordRuleCheck[]
    {
        new PasswordRuleCheck_MinLenght(5),
        new PasswordRuleCheck_CheckRegex(".*[A-Z].*")
        {
            text = "The massowrd must contain a Upper case letter"
        },
        new PasswordRuleCheck_ContainsWords(new string[] { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" }, false)
        {
            text = "The password needs to contain a month of the year"
        },
        new PasswordRuleCheck_CheckDigitsSum(10, 1),
        new PasswordRuleCheck_ContainsWords(new string[] { "Apple", "Mango", "Kiwi", "Pineapple", "Tomato", "Orange" }, true),
        new PasswordRuleCheck_ContainsWords(new string[] { "[pet name]" }, true)
        {
            text = "The password must contain the name of the pet"
        },
        new PasswordRuleCheck_CheckRegex(".*(?=[MDCLXVI])M*(C[MD]|D?C{0,3})(X[CL]|L?X{0,3})(I[XV]|V?I{0,3}).*")
        {
            text = "The password must contain a roman number"
        },
        new PasswordRuleCheck_RomanNuberSum(5000, 1),
        new PasswordRuleCheck_CheckRegex(".*2(0\\d{2}|100).*")
        {
            text = "The password must contain an year in the 21 century" //2001 - 2100
        },
        new PasswordRuleCheck_CheckDigitsSum(20, -1),
        new PasswordRuleCheck_RomanNuberSum(9, 2),
        new PasswordRuleCheck_ContainsWords(new string[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" }, false)
        {
            text = "The password needs to contain a day of the week"
        },
    };

    private void Awake()
    {
        enterPasswordField.onSubmit.AddListener((x) => { CheckPasswordEntered(); });
        newPasswordField.onSubmit.AddListener((x) => { SetNewPassword(); });
    }

    private void Start()
    {
        CheckNewPassword();
    }

    private void HideAll()
    {
        enterPasswordParent.gameObject.SetActive(false);
        changePasswordParent.gameObject.SetActive(false);
        laserControllParent.gameObject.SetActive(false);
    }
    public void Open()
    {
        laserWindowParent.gameObject.SetActive(true);
        HideAll();

        var foundPassword = GameBehaviour.GetGlobalValue(GameVariableKeys.FoundPassword.ToString()) > 0.5f;
        var didResetPassword = GameBehaviour.GetGlobalValue(GameVariableKeys.PasswordReseted.ToString()) > 0.5f;

        if (!foundPassword)
        {
            enterPasswordParent.gameObject.SetActive(true);
        }
        else if (!didResetPassword)
        {
            changePasswordParent.gameObject.SetActive(true);
            CheckNewPassword();
        }
        else
        {
            var isLaserCharged = GameBehaviour.GetGlobalValue(GameVariableKeys.PasswordReseted.ToString()) > 0.5f;
            if (isLaserCharged)
            {
                laserCarge = laserChargeSprites.Length - 0.5f;
            }
            else laserCarge = 0;
            laserControllParent.gameObject.SetActive(true);
        }
    }

    public void Close()
    {
        laserWindowParent.gameObject.SetActive(false);
    }

    public void CheckPasswordEntered()
    {
        if (enterPasswordField.text == GameBehaviour.Instance.laserPassword)
        {
            HideAll();
            GameBehaviour.SetGlobalValue(GameVariableKeys.FoundPassword.ToString(), 1);
            changePasswordParent.gameObject.SetActive(true);
        }
    }

    private int lastRuledPassed = -1;
    public void CheckNewPassword()
    {
        var newPass = newPasswordField.text;
        var checkCount = 0;
        for (int i = 0; i < passwordChecks.Length; i++)
        {
            if (!passwordChecks[i].Check(newPass))
            {
                if (i == lastRuledPassed)
                {
                    var child = newPasswordErrorParent.GetChild(i);
                    child.GetComponentInChildren<TMP_Text>().text = passwordChecks[i].ToString();
                    return;
                }
                lastRuledPassed = i;
                break;
            }
            else
            {
                checkCount++;
            }
        }
        if (checkCount == passwordChecks.Length)
            lastRuledPassed = passwordChecks.Length;
        foreach (Transform child in newPasswordErrorParent)
            Destroy(child.gameObject);
        for (int i = 0; i <= lastRuledPassed && i < passwordChecks.Length; i++)
        {
            var rule = passwordChecks[i];
            var item = Instantiate(newPasswordErrorItem, newPasswordErrorParent);
            var text = item.GetComponentInChildren<TMP_Text>();
            text.text = rule.ToString();
            if (i == lastRuledPassed)
            {
                var img = item.GetComponent<Image>();
                img.color = new Color(0.7f, 0, 0);
                break;
            }
        }
    }

    public void SetNewPassword()
    {
        CheckNewPassword();
        if(lastRuledPassed == passwordChecks.Length)
        {
            HideAll();
            laserControllParent.gameObject.SetActive(true);
            GameBehaviour.SetGlobalValue(GameVariableKeys.PasswordReseted.ToString(), 1);
        }
    }

    public void ChargeLaser()
    {
        laserChargeDirection = 1;
    }

    public void ArmLaser()
    {
        if(GameBehaviour.GetGlobalValue(GameVariableKeys.LaserCharged.ToString()) > 0.5)
        {
            errorLaserText.text = "The Laser is ready to fire";
        }
        else
        {
            errorLaserText.text = "Battery low. Please recharge";
        }
    }

    private void Update()
    {
        int laserStage = (int)laserCarge;
        if(laserStage != laserChargeSprites.Length - 1)
        {
            int stage2 = laserChargeDirection < 0 ? -laserStage % 2 : ((int)(Time.time * 4)) % 2;
            laserChargeImage.sprite = laserChargeSprites[laserStage + stage2];

            laserCarge += laserChargeDirection * Time.deltaTime;
            if(laserCarge < 0)
            {
                laserCarge = 0;
                laserChargeDirection = 0;
            }
        }
        else
        {
            if (GameBehaviour.GetGlobalValue(GameVariableKeys.LaserCanBeCharged.ToString()) < 0.5)
            {
                errorLaserText.text = "An error ocured while charging the laser\n" +
                                        "The wire pannel is now opened\n" +
                                        "Please connect the wires back together";
                laserChargeDirection = -6;
                laserCarge -= 1;
                GameBehaviour.SetGlobalValue(GameVariableKeys.WireNeedRepering.ToString(), 1);
            }
            else
            {
                GameBehaviour.SetGlobalValue(GameVariableKeys.LaserCharged.ToString(), 1);
                errorLaserText.text = "Laser fully charged";
            }
        }
    }
}
