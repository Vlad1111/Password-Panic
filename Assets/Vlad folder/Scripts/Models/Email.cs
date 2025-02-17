using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        date = DateTime.Parse(line[3].Trim());
        StringBuilder sb = new StringBuilder();
        for (int i = 4; i < line.Length; i++)
        {
            _ = GameBehaviour.Instance.ReplaceKeyWorld(line[i], sb);
            sb.Append('\n');
        }
        body = sb.ToString();
    }
}
