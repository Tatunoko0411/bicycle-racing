using UnityEngine;
using static MailObject;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using bicycle_racing.Shared.Models.Entities;

public class FriendObject : MonoBehaviour
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

    public User friendDate;

    NetWorkManager netWorkManager;

    /// <summary>
    /// フレンド情報の配置
    /// </summary>
    /// <param name="FrirendID">フレンドのユーザID</param>
    public async void SetFriend(int FrirendID)
    {
        netWorkManager = GameObject.Find("NetWorkManager").GetComponent<NetWorkManager>();

        //フレンドのユーザ情報取得
        string name = await netWorkManager.GetUserName(FrirendID);
        friendDate = await netWorkManager.GetUser(name);

        state = (State)friendDate.State;

        //マッチング中じゃない場合はボタン非表示
        if (state == State.Matching)
        {
            Button.gameObject.SetActive(true);
            SetEvent();
        }
        else
        {
            Button.gameObject.SetActive(false);
        }


        NameText.text = name;
        switch (friendDate.State)
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


    /// <summary>
    /// フレンドのマッチに合流
    /// </summary>
    public async void FriendMatch()
    {
       netWorkManager.SetRoom(friendDate.Stage_id,friendDate.Room_name);

        Initiate.Fade("MatcingScene", Color.black,1.5f);
    }


    /// <summary>
    /// 合流ボタンのイベント設定
    /// </summary>
    public void SetEvent()
    {
        eventTrigger = Button.gameObject.GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((eventDate) => { FriendMatch(); });
        eventTrigger.triggers.Add(entry);
    }

}
