using bicycle_racing.Shared.Models.Entities;
using DG.Tweening;
using Newtonsoft.Json;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using Unity.Entities.UniversalDelegates;
using UnityEngine;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    public enum MailState
    {
        None = 0,
        Read
    }

    NetWorkManager netWorkManager;

    [SerializeField] GameObject MailObjectPrefab;
    [SerializeField] GameObject MailParent;

    [SerializeField] GameObject UserObjPrefab;
    [SerializeField] GameObject UserParent;

    [SerializeField] GameObject FriendObjPrefab;
    [SerializeField] GameObject FriendParent;

    [SerializeField] InputField userFindField;  

    [SerializeField] GameObject BackButton;

    [SerializeField] InputField UpdateNameField;

    [SerializeField] GameObject TutorialObject;
    [SerializeField] Image TutorialImage;
    [SerializeField] GameObject nextPageButton;
    [SerializeField] GameObject BackPageButton;
    [SerializeField] List<Sprite> TutorialSprites;
    int TutorialCount;

    [SerializeField] Text ID;
    [SerializeField] Text play;
    [SerializeField] Text win;

    [SerializeField] GameObject UserDate;

    [SerializeField] GameObject StartObject;

    [SerializeField] GameObject Map1;
    [SerializeField] GameObject Map2;

    [SerializeField] Transform CenterPos;

    [SerializeField] List<Button> Buttons;

    [SerializeField] GameObject panel;

    bool isChengeState;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        netWorkManager = GameObject.Find("NetWorkManager").GetComponent<NetWorkManager>();


        StartObject.SetActive(true);

        isChengeState = false;


    }

    // Update is called once per frame
    void Update()
    {
        //Startだと取得タイミングが早くてエラーになるので、情報取得確認後、ステータス変更
        if (netWorkManager != null)
        {
            if (netWorkManager.myself != null)
            {
                if (!isChengeState)
                {
                    if (netWorkManager.myself.State != (int)FriendObject.State.Online)
                    {
                        netWorkManager.ChangeState(FriendObject.State.Online);
                        isChengeState=true;
                    }
                }
            }
        }
        

        if (StartObject.activeSelf)
        {
            if(Input.GetMouseButton(0)||Input.GetMouseButton(1))
            {
                StartObject.SetActive(false);
                SEManager.PlaySE(SEManager.SE.click);
            }
        }

        Map1.transform.Rotate(0, 0.3f, 0);
        Map2.transform.Rotate(0, 0.3f, 0);
    }

    public void MoveMtching(int StageId)
    {
        GameManager.StageId = StageId;

       switch(StageId)
        {//選択されたステージを中央に移動
            case 1:
                Map1.transform.DOMove(CenterPos.position,0.5f);
                break;
                case 2:
                Map2.transform.DOMove(CenterPos.position, 0.5f);
                break;
        }

        foreach(Button button in Buttons)
        {
            if (button != null)
            {
                button.gameObject.SetActive(false);
            }

        }

        Initiate.Fade("MatcingScene", Color.black, 0.3f);
 
        SEManager.PlaySE(SEManager.SE.Move);
    }


    /// <summary>
    /// プロフィール表示
    /// </summary>
    public void OpenProfil()
    {
        SEManager.PlaySE(SEManager.SE.click);
        FriendParent.SetActive(false);
        UserParent.SetActive(false);
        userFindField.gameObject.SetActive(false);
        MailParent.SetActive(false);
        BackButton.SetActive(true);
        UserDate.SetActive(true);
        TutorialObject.SetActive(false);


        UpdateNameField.text = netWorkManager.myself.Name;
        ID.text = "ID："+ netWorkManager.myself.Id.ToString();
        play.text = "プレイ回数：" + netWorkManager.myself.Play_count.ToString();
        win.text = "勝利回数：" + netWorkManager.myself.Win_count.ToString();
    }

    /// <summary>
    /// ユーザ検索語のUI切り替え
    /// </summary>
    public async void FindUser()
    {
        SEManager.PlaySE(SEManager.SE.click);
        FriendParent.SetActive(false);
        UserParent.SetActive(true);
        BackButton.SetActive(true);
        UserDate.SetActive(false);
        User user =  await netWorkManager.GetUser(userFindField.text);
        TutorialObject.SetActive(false);

        foreach (Transform child in UserParent.transform)
        {
            Destroy(child.gameObject);
        }

        if (user != null)
        {
            GameObject gameObject = Instantiate(UserObjPrefab,
                UserParent.transform.position,
                Quaternion.identity,
                UserParent.transform);

           UserObject userObject = gameObject.GetComponent<UserObject>();

            userObject.SetUser(user.Id);
            
        }
    }

    /// <summary>
    /// フレンド申請送信
    /// </summary>
    /// <param name="id"></param>
    public async void SendFriendRequest(int id)
    {
       netWorkManager.SendMail(id,(int)MailObject.MailType.FriendRequest,
            $"{netWorkManager.myself.Name}からのフレンドリクエストを許可しますか？");

        SEManager.PlaySE(SEManager.SE.click);
    }

    /// <summary>
    /// メール取得＆UI変更
    /// </summary>
    public async void GetMail()
    {
        SEManager.PlaySE(SEManager.SE.click);
        FriendParent.SetActive(false);
        UserParent.SetActive(false);
        MailParent.SetActive(true);
        userFindField.gameObject.SetActive(false);
        BackButton.SetActive(true);
        UserDate.SetActive(false);
        TutorialObject.SetActive(false);
        panel.SetActive(true);


        //情報取得後オブジェクト生成
        Mail[] mails = await netWorkManager.GetMailAsync();

        foreach (Transform child in MailParent.transform)
        {
            Destroy(child.gameObject);
        }

        if (mails != null)
        {
            foreach (Mail mail in mails)
            {
                GameObject mailobj = Instantiate(MailObjectPrefab,
                      MailParent.transform.position,
                      Quaternion.identity,
                      MailParent.transform);

                MailObject mailObject = mailobj.GetComponent<MailObject>();

                mailObject.ContentText.text = mail.Content;
                mailObject.DateText.text = "受信日：" + mail.Created_at.ToString();
                mailObject.SendID = mail.send_player_id;
                mailObject.DateID = mail.Id;
                mailObject.mailType = (MailObject.MailType)mail.Type;

                mailObject.SetMail();
            }
        }

    }

    /// <summary>
    /// フレンド欄の表示
    /// </summary>
    public async void GetFriend()
    {
        SEManager.PlaySE(SEManager.SE.click);
        FriendParent.SetActive(true);
        UserParent.SetActive(false);
        MailParent.SetActive(false);
        userFindField.gameObject.SetActive(true);
        BackButton.SetActive(true);
        UserDate.SetActive(false);
        TutorialObject.SetActive(false);
        panel.SetActive(true);

        //情報取得後オブジェクト生成
        Friend[] friends = await netWorkManager.GetFriends();

        foreach(Transform child in FriendParent.transform)
        {
            Destroy(child.gameObject);
        }

        if (friends.Length > 0)
        {

            foreach (Friend friend in friends)
            {
                GameObject obj = Instantiate(FriendObjPrefab,
                    FriendParent.transform.position,
                    Quaternion.identity,
                    FriendParent.transform);

                FriendObject friendObject = obj.GetComponent<FriendObject>();

                if (friend.Player_id_02 != netWorkManager.myUserId)
                {
                    friendObject.SetFriend(friend.Player_id_02);
                }
                else
                {
                    friendObject.SetFriend(friend.Player_id_01);
                }


            }
        }
    }

    /// <summary>
    /// チュートリアル表示
    /// </summary>
    public void SetTutorial()
    {
        SEManager.PlaySE(SEManager.SE.click);
        FriendParent.SetActive(false);
        UserParent.SetActive(false);
        MailParent.SetActive(false);
        BackButton.SetActive(true);
        userFindField.gameObject.SetActive(false);
        UserDate.SetActive(false);


        TutorialObject.SetActive(true);
        

        TutorialCount = 0;
        TutorialImage.sprite = TutorialSprites[TutorialCount];
        BackPageButton.SetActive(false);
        nextPageButton.SetActive(true);
        
    }

    /// <summary>
    /// チュートリアル内の表示切り替え
    /// </summary>
    /// <param name="value"></param>
    public void ChengeTutorialCount(int value)
    {
        TutorialCount += value;
        TutorialImage.sprite = TutorialSprites[TutorialCount];

        if (TutorialCount <= 0)
        {
            BackPageButton.SetActive(false);
            nextPageButton.SetActive(true);
        }
        else if (TutorialCount >= TutorialSprites.Count -1)
        {
            BackPageButton.SetActive(true);
            nextPageButton.SetActive(false);
        }
        else
        {
            BackPageButton.SetActive(true);
            nextPageButton.SetActive(true);
        }
    }

    /// <summary>
    /// メイン画面に切り替え
    /// </summary>
    public void BackMain()
    {
        SEManager.PlaySE(SEManager.SE.click);
        FriendParent.SetActive(false);
        UserParent.SetActive(false);
        MailParent.SetActive(false);
        BackButton.SetActive(false);
        userFindField.gameObject.SetActive(false);
        UserDate.SetActive(false);
        TutorialObject.SetActive(false);
        panel.SetActive(false);
    }


    /// <summary>
    /// ユーザ名更新
    /// </summary>
    public void UpdateName()
    {
        SEManager.PlaySE(SEManager.SE.click);
        netWorkManager.UpdateName(UpdateNameField.text);
    }

    /// <summary>
    ///スタート画面に遷移
    /// </summary>
    public void BackStart()
    {
        StartObject.SetActive(true);
        SEManager.PlaySE(SEManager.SE.click);
    }


}
