using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Setting
{
    public float masterVolume = 0.5f;
    public float musicVolue = 0.5f;
    public float sfxVolume = 0.5f;
}
public class SettingsWindow : MonoBehaviour
{
    private const string SettinKey = "__SAVED_SETTING__";

    public Transform settingParent;
    public Slider masterVolue;
    public Slider musicVolue;
    public Slider sfxVolue;
    public Setting setting;

    public void Start()
    {
        if(PlayerPrefs.HasKey(SettinKey))
        {
            setting = JsonUtility.FromJson<Setting>(PlayerPrefs.GetString(SettinKey));
        }
        else setting = new Setting();

        masterVolue.SetValueWithoutNotify(setting.masterVolume);
        musicVolue.SetValueWithoutNotify(setting.musicVolue);
        sfxVolue.SetValueWithoutNotify(setting.sfxVolume);

        SoundManager.Instance.MasterVolume = setting.masterVolume;
        SoundManager.Instance.MusicVolume = setting.musicVolue;
        SoundManager.Instance.SFXVolume = setting.sfxVolume;
        SoundManager.Instance.RecalculateVolume();
    }

    public void Open()
    {
        SoundManager.Instance.PlayClip("Click");
        settingParent.gameObject.SetActive(true);
    }

    public void Close()
    {
        SoundManager.Instance.PlayClip("Click");
        settingParent.gameObject.SetActive(false);
    }

    public void OnMasteChange()
    {
        setting.masterVolume = masterVolue.value;
        PlayerPrefs.SetString(SettinKey, JsonUtility.ToJson(setting));

        SoundManager.Instance.MasterVolume = setting.masterVolume;
        SoundManager.Instance.RecalculateVolume();
    }

    public void OnMusicChange()
    {
        setting.musicVolue = musicVolue.value;
        PlayerPrefs.SetString(SettinKey, JsonUtility.ToJson(setting));

        SoundManager.Instance.MusicVolume = setting.musicVolue;
        SoundManager.Instance.RecalculateVolume();
    }

    public void OnSfxChange()
    {
        setting.sfxVolume = sfxVolue.value;
        PlayerPrefs.SetString(SettinKey, JsonUtility.ToJson(setting));

        SoundManager.Instance.SFXVolume = setting.sfxVolume;
        SoundManager.Instance.RecalculateVolume();
    }
}
