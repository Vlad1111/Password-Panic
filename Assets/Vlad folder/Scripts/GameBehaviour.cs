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

    private int[] daysInMonths = new[] { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
    private string[] monthsOfTheYear = new[] { "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };

    private int ParseNumber(string nr)
    {
        nr = GetKeyValue(nr);

        int sign = 1;
        if(nr[0] == '-')
        {
            sign = -1;
            nr = nr.Substring(1);
        }
        int x = 0;
        for (int i = 0; i < nr.Length; i++)
            if (nr[i] >= '0' && nr[i] <= '9')
                x = x * 10 + nr[i] - '0';
        //Debug.Log("Number: " + nr + " = " + x);
        return x * sign;
    }

    private int ComputeEcuation(string ecuation)
    {
        var rez = 0;
        var parts = ecuation.Split('+');
        if(parts.Length > 1)
        {
            foreach (var part in parts)
                rez += ParseNumber(part);
            return rez;
        }
        parts = ecuation.Split('-');
        if (parts.Length > 1)
        {
            rez = ParseNumber(parts[0]);
            for(int i=1; i < parts.Length; i++)
                rez -= ParseNumber(parts[i]);
            return rez;
        }
        return ParseNumber(ecuation);
    }

    public string GetKeyValue(string key)
    {
        if(key[0] == 'd' && key[1] == ':')
        {
            var parts = key.Substring(2).Split(',');
            //foreach (var part in parts)
            //    Debug.Log("Part " + part);
            var day = ComputeEcuation(parts[0]);
            var month = ComputeEcuation(parts[1]) - 1;
            var year = ComputeEcuation(parts[2]);
            for(int tr = 0; tr < 100 && (month < 0 || month > 11 || day < 1 || day > daysInMonths[month]); tr++)
            {
                if(month < 0)
                {
                    month = 12 + month;
                    year--;
                }
                if(month > 11)
                {
                    month -= 12;
                    year++;
                }
                if (day < 1)
                {
                    month--;
                    if (month < 0)
                    {
                        month = 12 + month;
                        year--;
                    }
                    day = daysInMonths[month] + day;
                }
                if(day > daysInMonths[month])
                {
                    day = day - daysInMonths[month];
                    month++;
                }
            }
            if(parts.Length > 3)
            {
                List<string> values = new List<string>();
                if(parts[3].Contains('d')) values.Add(day.ToString());
                if(parts[3].Contains('m') && parts[3].Contains('n')) values.Add((month + 1).ToString());
                else if(parts[3].Contains('m')) values.Add(monthsOfTheYear[month]);
                if(parts[3].Contains('y'))values.Add(year.ToString());

                return string.Join(" ", values.ToArray());
            }
            return day + "." + (month + 1) + "." + year;
        }
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
