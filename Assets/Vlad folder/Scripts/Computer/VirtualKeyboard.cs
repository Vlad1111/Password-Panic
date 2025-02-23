using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class KeyClass
{
    public char name;
    public Button button;
    public TMP_Text text;
    public char key;
}
public class VirtualKeyboard : MonoBehaviour
{
    public static VirtualKeyboard Instance;

    private void Awake()
    {
        Instance = this;
    }

    public List<string> keysChar = new List<string>(new string[]
    {
        "11!",
        "22@",
        "33#",
        "44$",
        "55%",
        "66^",
        "77&",
        "88*",
        "99(",
        "00)",
        "--_",
        "==+",
        "[[{",
        "]]}",
        ";;:",
        "''\"",
        "\\\\|",
        ",,<",
        "..>",
        "//?",
    });

    private int shift = 0;
    public List<KeyClass> keys = new List<KeyClass>();
    private TMP_InputField inputField = null;

    public void Start()
    {
        Awake();

        Debug.Log(keys.Count);
        if(keys.Count == 0)
        {
            var buttons = GetComponentsInChildren<Button>();
            foreach (var button in buttons)
            {
                if (button.name.Length == 1)
                {
                    var newButton = button;
                    var tmp_text = newButton.transform.GetChild(0).GetComponent<TMP_Text>();
                    var keyClass = new KeyClass()
                    {
                        name = newButton.name[0],
                        button = newButton,
                        key = newButton.name[0],
                        text = tmp_text
                    };
                    keys.Add(keyClass);
                    button.onClick.AddListener(() => OnKeyPressed(keyClass));
                }
            }
            UpdateKeyText();
        }
    }

    public void UpdateKeyText()
    {
        foreach(var key in keys)
        {
            var ch = keysChar.FirstOrDefault((x) => x[0] == key.name);
            if (ch == default(string))
            {
                if (shift == 1)
                    key.key = (char)(key.name - 'a' + 'A');
                else key.key = key.name;
            }
            else key.key = ch[1 + shift];

            key.text.text  = key.key.ToString();
        }
    }

    public void OpenForInputField(TMP_InputField impF)
    {
        inputField = impF;
        gameObject.SetActive(true);
    }

    public void OnKeyPressed(KeyClass key)
    {
        if (inputField == null) return;

        inputField.text = inputField.text + key.key;
    }

    public void OnSpace()
    {
        OnKeyPressed(new KeyClass() { key = ' ' });
    }

    public void Enter()
    {
        OnKeyPressed(new KeyClass() { key = '\n' });
    }

    public void Delete()
    {
        if (inputField == null) return;

        inputField.text = inputField.text.Substring(0, inputField.text.Length - 1);
    }

    public void Shift()
    {
        shift = (shift + 1) % 2;
        UpdateKeyText();
    }

    public void CloseKeyboard()
    {
        transform.gameObject.SetActive(false);
    }
}
