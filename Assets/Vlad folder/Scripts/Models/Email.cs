using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[System.Serializable]
public class Email
{
    public static Email EmptyEmail => new Email();
    public string title = "";
    public string sender = "";
    public string[] recivers = new string[0];
    public DateTime? date = null;
    public string body = "";

    private Email() { }
    public Email(string text)
    {
        var line = text.Split('\n');
        title = line[0];
        sender = line[1];
        recivers = line[2].Split(',').Select(x => x.Trim()).ToArray();

        //Debug.Log("Date: " + line[3]);
        var newDate = GameBehaviour.Instance.ReplaceKeyWorld(line[3].Trim()).ToString().Trim();
        //Debug.Log("new date: " + newDate);
        DateTime dateValue;
        if (!DateTime.TryParseExact(newDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateValue))
            DateTime.TryParseExact(newDate, "dd.MM.yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dateValue);
        date = dateValue;

        StringBuilder sb = new StringBuilder();
        for (int i = 4; i < line.Length; i++)
        {
            _ = GameBehaviour.Instance.ReplaceKeyWorld(line[i], sb);
            sb.Append('\n');
        }
        body = sb.ToString();
    }
}
