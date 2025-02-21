using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NotepadBehaviour : MonoBehaviour
{
    //private const string NotepadKey = "__SAVED_NOTEPAD_TEXT__";

    public Transform notepadParent;
    public TMP_InputField textArea;

    private void Start()
    {
        textArea.text = "";// PlayerPrefs.GetString(NotepadKey);
    }

    public void Open()
    {
        SoundManager.Instance.PlayClip("Click");
        notepadParent.gameObject.SetActive(true);
    }

    public void Close()
    {
        SoundManager.Instance.PlayClip("Click");
        notepadParent.gameObject.SetActive(false);
    }

    public void SaveText()
    {
        //PlayerPrefs.SetString(NotepadKey, textArea.text);
    }
}
