using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MailObject : MonoBehaviour
{

    public enum MailType
    {
        None = 0,
        Normal,
        FriendRequest,

    }

    [SerializeField]public Text ContentText;
    [SerializeField]public Text sendUserText;
    [SerializeField]public Text DateText;
    [SerializeField]Button Button;

    public MailType mailType;

    EventTrigger eventTrigger;

    public int SendID;
    public int DateID;

    NetWorkManager netWorkManager;

   

    /// <summary>
    /// メール情報設定
    /// </summary>
    public void SetMail()
    {
        netWorkManager = GameObject.Find("NetWorkManager").GetComponent<NetWorkManager>();
        if (mailType == MailType.FriendRequest)
        {//フレンドリクエストの場合はボタン表示
            Button.gameObject.SetActive(true);
            SetEvent();
        }

        sendUserText.text = netWorkManager.GetUserName(SendID).ToString();
    }

    /// <summary>
    /// フレンド申請の承諾（フレンド追加）
    /// </summary>
    public void approveFriend()
    {
        netWorkManager.RegistFriend(SendID);
        netWorkManager.RemoveMail(DateID);
        Destroy(gameObject);
    }

    /// <summary>
    /// メール削除
    /// </summary>
    public void DeleteMail()
    {
        netWorkManager.RemoveMail(DateID);
        Destroy(gameObject);
    }

    /// <summary>
    /// メールのボタン設定
    /// </summary>
    public void SetEvent()
    {
        eventTrigger = Button.gameObject.GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((eventDate) => { approveFriend(); });
        eventTrigger.triggers.Add(entry);
    }



}
