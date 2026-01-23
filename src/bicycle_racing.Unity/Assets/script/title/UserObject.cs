using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using bicycle_racing.Shared.Models.Entities;


public class UserObject : MonoBehaviour
{
    public enum State
    {
        Offline = 0,
        Online,
        Matching,
        Game,
    }

    [SerializeField] public Text NameText;
    [SerializeField] public Text StateText;
    [SerializeField] Button Button;

    public State state;

    EventTrigger eventTrigger;

    User userDate;

    NetWorkManager netWorkManager;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {



    }

    // Update is called once per frame
    void Update()
    {

    }

    public async void SetUser(int UserId)
    {
        netWorkManager = GameObject.Find("NetWorkManager").GetComponent<NetWorkManager>();

        string name = await netWorkManager.GetUserName(UserId);
        userDate = await netWorkManager.GetUser(name);

        state = (State)userDate.State;


        if (userDate.Name != netWorkManager.myself.Name)
        {
            Button.gameObject.SetActive(true);
            SetEvent();
        }
        else
        {
            Button.gameObject.SetActive(false);
        }


        NameText.text = name;
        switch (userDate.State)
        {
            case (int)State.Offline:
                StateText.text = "オフライン";
                break;
            case (int)State.Online:
                StateText.text = "オンライン";
                break;
            case (int)State.Matching:
                StateText.text = "マッチング中";
                break;
            case (int)State.Game:
                StateText.text = "ゲーム中";
                break;
        }


    }


    public async void SendRequest()
    {
        netWorkManager.SendMail(userDate.Id, (int)MailObject.MailType.FriendRequest,
            $"フレンドリクエストだよ");

       
    }

    public void SetEvent()
    {
        eventTrigger = Button.gameObject.GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((eventDate) => { SendRequest(); });
        eventTrigger.triggers.Add(entry);
    }
}
