using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class GameBehaviour : MonoBehaviour
{
    public static GameBehaviour Instance;

    private static Dictionary<string, float> globalGameValues = new Dictionary<string, float>();
    private void Awake()
    {
        Instance = this;
        foreach (var pair in randomValues)
        {
            randomValuesDictionary.Add(pair.key, pair.possibleValues[Random.Range(0, pair.possibleValues.Length)]);
        }
    }

    public static float GetGlobalValue(string key)
    {
        if(globalGameValues.ContainsKey(key))
            return globalGameValues[key];
        return 0;
    }

    public static void SetGlobalValue(string key, float value)
    {
        globalGameValues[key] = value;
    }

    public static void AddToGlobalValue(string key, float value)
    {
        globalGameValues[key] = GetGlobalValue(key) + value;
    }


    [System.Serializable]
    public class RandomValues
    {
        public string key;
        public string[] possibleValues;
    }

    public float remaingTime;
    public TMP_Text countdownDisplyText;
    public string laserPassword;
    public List<RandomValues> randomValues;
    private Dictionary<string, string> randomValuesDictionary = new Dictionary<string, string>();

    // Start is called before the first frame update
    void Start()
    {
        laserPassword = ReplaceKeyWorld(laserPassword).ToString();
    }

    public string GetKeyValue(string key)
    {
        if (randomValuesDictionary.ContainsKey(key))
        {
            return randomValuesDictionary[key];
        }
        return key;
    }

    public StringBuilder ReplaceKeyWorld(string text, StringBuilder sb = null)
    {
        if(sb == null) sb = new StringBuilder();


        int last = 0;
        for (int j = 0; j < text.Length; j++)
        {
            if (text[j] == '[')
            {
                if (last != j)
                    sb.Append(text.Substring(last, j - last));
                last = j;
            }
            else if (text[j] == ']')
            {
                var key = text.Substring(last + 1, j - last - 1);
                sb.Append(GetKeyValue(key));
                last = j + 1;
            }
        }
        if(last < text.Length - 1)
            sb.Append(text.Substring(last, text.Length - last));

        return sb;
    }

    // Update is called once per frame
    void Update()
    {
        countdownDisplyText.text = ((int)(remaingTime / 60)) + ":";
        int seconds = ((int)remaingTime) % 60;
        if (seconds < 10)
            countdownDisplyText.text += "0" + seconds;
        else
            countdownDisplyText.text += seconds;
        remaingTime -= Time.deltaTime;
    }
}
