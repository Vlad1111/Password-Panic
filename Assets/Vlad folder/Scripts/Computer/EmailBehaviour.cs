using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EmailBehaviour : MonoBehaviour
{
    public static EmailBehaviour Instance;
    private void Awake()
    {
        Instance = this;
    }

    public int nrEmailPerSearch = 3;
    public List<Email> emails = new List<Email>();
    public Transform emailWindowParent;
    public Transform emailsListParent;
    public Transform emailListItem;
    public TMP_InputField searchBar;
    public TMP_Text emailSubject;
    public TMP_Text emailDetails;
    public TMP_Text emailBody;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("LoadAllEmails");

        searchBar.onSubmit.AddListener((x) => { Search(); });
        searchBar.SetTextWithoutNotify("");
    }

    public void LoadAllEmails()
    {
        var allEmails = Resources.LoadAll<TextAsset>("Emails");
        emails = allEmails.Select(x => new Email(x.ToString())).ToList();

        ShowEmail(Email.EmptyEmail);
        Search();
    }

    public void Close()
    {
        emailWindowParent.gameObject.SetActive(false);
        SoundManager.Instance.PlayClip("Click");
    }

    public void Open()
    {
        emailWindowParent.gameObject.SetActive(true);
        SoundManager.Instance.PlayClip("Click");
    }

    private List<Email> SearchEmailListFor(string world)
    {
        if(world == "")
            return emails;

        List<Email> filteredemails = new List<Email>();
        foreach(var email in emails)
            if(email.body.Contains(world))
                filteredemails.Add(email);

        return filteredemails;
    }

    public void Search()
    {
        var searchedEmailes = SearchEmailListFor(searchBar.text);
        searchedEmailes.Sort((x, y) => y.date.Value.CompareTo(x.date.Value));

        foreach (Transform child in emailsListParent)
            Destroy(child.gameObject);

        for (int i = 0; i < searchedEmailes.Count() && i < nrEmailPerSearch; i++)
        {
            var email = searchedEmailes[i];

            var item = Instantiate(emailListItem, emailsListParent);
            var button = item.GetComponent<Button>();
            button.onClick.AddListener(() => { ShowEmail(email); });

            var subject = item.GetChild(0).GetComponent<TMP_Text>();
            subject.text = email.title;
            var details = item.GetChild(1).GetComponent<TMP_Text>();
            details.text = "from: " + email.sender + "\n" + email.date.Value.ToString("dd.MM.yyy");
        }
    }

    public void ShowEmail(Email email)
    {
        emailSubject.text = email.title;
        var recivers = "";
        for (int i = 0; i < email.recivers.Count(); i++)
        {
            if (i != 0)
                recivers += ", ";
            recivers += email.recivers[i];
        }
        emailDetails.text = "from: " + email.sender + "\n" +
                    "cc: " + recivers + "\n" +
                    (email.date == null ? "" : "send: " + email.date.Value.ToString("dd.MM.yyy"));
        var body = email.body;
        if (searchBar != null && searchBar.text != "")
            body = body.Replace(searchBar.text, "<u>" + searchBar.text + "</u>");
        emailBody.text = body;
    }
}
