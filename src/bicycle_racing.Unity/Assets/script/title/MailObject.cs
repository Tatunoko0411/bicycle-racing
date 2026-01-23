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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       

       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetMail()
    {
        netWorkManager = GameObject.Find("NetWorkManager").GetComponent<NetWorkManager>();
        if (mailType == MailType.FriendRequest)
        {
            Button.gameObject.SetActive(true);
            SetEvent();
        }

        sendUserText.text = netWorkManager.GetUserName(SendID).ToString();
    }


    public void approveFriend()
    {
        netWorkManager.RegistFriend(SendID);
        netWorkManager.RemoveMail(DateID);
        Destroy(gameObject);
    }

    public void DeleteMail()
    {
        netWorkManager.RemoveMail(DateID);
        Destroy(gameObject);
    }

    public void SetEvent()
    {
        eventTrigger = Button.gameObject.GetComponent<EventTrigger>();
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerClick;
        entry.callback.AddListener((eventDate) => { approveFriend(); });
        eventTrigger.triggers.Add(entry);
    }



}
