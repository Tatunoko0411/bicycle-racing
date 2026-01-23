using DG.Tweening;
using bicycle_racing.Shared.Interfaces.StreamingHubs;
using bicycle_racing.Shared.Models.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using rayzngames;
using UnityEngine.SceneManagement;
using Newtonsoft.Json;
using System.IO;

[DefaultExecutionOrder(-10)]

public class NetWorkManager : MonoBehaviour
{
    [SerializeField] GameObject characterPrefab;
    [SerializeField] InputField roomNameField;
    [SerializeField] InputField PlayerIdField;
    [SerializeField] Text info;
    Dictionary<Guid, GameObject> characterList = new Dictionary<Guid, GameObject>();
    public Dictionary<Guid,User> GoalPlayerList = new Dictionary<Guid,User>();
    public RoomModel roomModel;
    UserModel userModel;
    MailModel mailModel;

    public int myUserId = 1;
    public User myself;

    int battleId = 0;   

    float waitTime = 0;

    bool isJoin = false;

    [SerializeField]BikeController bikeController;


    public async void Awake()
    {
        LogManager.SetLogText("接続待機中");

        roomModel = GetComponent<RoomModel>();
        userModel = GetComponent<UserModel>();
        mailModel = GetComponent<MailModel>();

        DontDestroyOnLoad(this);

        //接続
        try
        {
            await roomModel.ConnectAsync();
        }
        catch (Exception e)
        {
            LogManager.SetLogText(e.Message);
        }
    }

    async void Start()
    {


        //ユーザーが入室した時にOnJoinedUserメソッドを実行するよう、モデルに登録しておく
        roomModel.OnJoinedUser += this.OnJoinedUser;
        roomModel.OnLeavedUser += this.OnLeavedUser;
        roomModel.OnMoveUser += this.OnMoveUser;
        roomModel.OnPutItemUser += this.OnPutItemUser;
        roomModel.OnPassCheckPoint += this.OnPassCheckPoint;
        roomModel.OnGoalUser += this.OnGoalUser;
        roomModel.OnMenberConfirmed += this.OnMenberConfirmed;
        roomModel.OnStartGame += this.OnStartGame;
        

        bool isSuccess = LoadUserData();
        if (isSuccess)
        {
            SaveUserData();
        }
        else
        {
          myUserId = await userModel.RegistUser(Guid.NewGuid().ToString());
   
            if(myUserId == 0)
            {
                //Debug.Log("RegistUser failed");
                return;
            }
            SaveUserData();
        }


        try
        {
            // ユーザー情報を取得
            myself = await userModel.GetUser(myUserId);

            LogManager.SetLogText("ネットワークの接続に成功しました。");
            ChangeState(FriendObject.State.Online);
        }
        catch (Exception e)
        {
           // LogManager.SetLogText("RegistUser failed");
            Debug.LogException(e);
        }

        

    }

    private void OnApplicationQuit()
    {

        ChangeState(FriendObject.State.Offline);
        SetRoom(0,"");
    }

    private void FixedUpdate()
    {

        if (SceneManager.GetActiveScene().name == "GameScene")
        {
            if (isJoin)
            {
                waitTime++;

                if (waitTime >= 5)
                {
                    Move();
                    waitTime = 0;
                }
            }
        }
    }


    // ユーザー情報を読み込む
    public bool LoadUserData()
    {
        if (!File.Exists(Application.persistentDataPath + "/saveData.json"))
        {
            return false;
        }
        var reader =
                   new StreamReader(Application.persistentDataPath + "/saveData.json");
        string json = reader.ReadToEnd();
        reader.Close();
        SaveData saveData = JsonConvert.DeserializeObject<SaveData>(json);
        myUserId = saveData.UserID;

        return true;
    }

    public void SaveUserData()
    {
        SaveData saveData = new SaveData();

        saveData.UserID = myUserId;

        string json = JsonConvert.SerializeObject(saveData);
        var writer =
                new StreamWriter(Application.persistentDataPath + "/saveData.json");
        writer.Write(json);
        writer.Flush();
        writer.Close();

    }

    public async Task<User> GetUser(string name)
    {
       return await userModel.GetUser(name);
    }

    public async void RegistFriend(int ID)
    {
        await userModel.RegistFriend(myUserId,ID);
    }

    public async void UpdateName(string name)
    {
        await userModel.UpdateUser(myUserId,name);
    }

    public async Task<User> UpdateUserCount(int play,int win)
    {
       return await userModel.UpdateUserCountAsync(myUserId,play,win);
    }

    public async void JoinRoom(int StageId)
    {
        int ID = myUserId;

        if(myself.Room_name != "")
        {
            //ルーム指定で入室
            await roomModel.JoinFriendAsync(ID,StageId,myself.Room_name);
            return;
        }

        //入室
        Debug.Log("入室処理開始");
        await roomModel.JoinAsync(ID,StageId);
    }

    public async void StartGame(int stageID)
    {
        await roomModel.StartGameAsync(stageID);
    }

    public void SetPlayers()
    {
        GameManager gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        if (gameManager != null)
        {


            int cnt = 0;    
            //プレイヤー配置
            foreach (JoinedUser joinedUser in roomModel.userTable.Values)
            {

                // すでに表示済みのユーザーは追加しない
                if (characterList.ContainsKey(joinedUser.ConnectionId))
                {
                    continue;
                }

                Vector3 StartPos = gameManager.StartPoints[cnt].transform.position;
                // 自分は位置のみ設定
                if (joinedUser.ConnectionId == roomModel.ConnectionId)
                {
                    bikeController = GameObject.Find("Bicycle").GetComponent<BikeController>();
                    bikeController.gameObject.transform.position = StartPos;
                    bikeController.gameManager.transform.rotation = new Quaternion(0,-90,0,0);
                    bikeController.NameText.text = joinedUser.UserData.Name;
                    isJoin = true;

                }
                else
                {

                    GameObject characterObject = Instantiate(characterPrefab);  //インスタンス生成
                    characterObject.transform.position = StartPos;
                    characterObject.transform.rotation =  new Quaternion(0, -90, 0, 0);
                    BikeController bike = characterObject.GetComponent<BikeController>();
                    bike.NameText.text = joinedUser.UserData.Name;


                 

                    characterList[joinedUser.ConnectionId] = characterObject;//フィールドで保持
                }

                cnt++;
            }
        }
    }

    public async void SetRoom(int StageID,string name)
    {
       myself = await userModel.SetRoomeName(myUserId,StageID, name);
       GameManager.StageId = myself.Stage_id;
    }

    public async void Ready()
    {
        await roomModel.ReadyAsync();
    }


    public async void LeaveRoom()
    {
        await roomModel.LeaveAsync();
        SetRoom(0,"");
        isJoin = false;
    }

    public async void Move()
    {
        await roomModel.MoveAsync(bikeController.gameObject.transform.position,
            bikeController.gameObject.transform.rotation,
            bikeController.gameObject.transform.localScale
            );
    }

    public async void PutItem()
    {
        await roomModel.PutItemAsync(bikeController.transform.position,
            (int)bikeController.HaveItem);
    }

    public async void PassCheck()
    {
        await roomModel.PassCheckAsync();
    }

    public async void Goal(int rank)
    {
        await roomModel.OnGoalAsync(rank);
    }

    public async void SendMail(int ReceiveId,int type,string content)
    {
        await mailModel.CreateMailAsync(myUserId,ReceiveId,type,content);
    }

    public async void ChangeStateMail(int ID,int state)
    {
        await mailModel.ChangeStateMailAsync(ID,state);
    }

    public async Task<Mail[]> GetMailAsync()
    {
        return await mailModel.GetMailAysnc(myUserId);
    }

    public async void RemoveMail(int ID)
    {
        await mailModel.RemoveMailAsync(ID);
    }

    public async Task<string> GetUserName(int ID)
    {
        User user = await userModel.GetUser(ID);

        return user.Name;
    }

    public async Task<Friend[]> GetFriends()
    {
        return await userModel.GetFriend(myUserId); 
    }

    public async void ChangeState(FriendObject.State state)
    {
       myself =  await userModel.ChangeState(myUserId,(int)state);
    }

    //ユーザーが入室した時の処理
    private void OnJoinedUser(JoinedUser user)
    {

        MatchingManager matchingManager = GameObject.Find("MatchingManager").GetComponent<MatchingManager>();   
        // すでに表示済みのユーザーは追加しない
        if (characterList.ContainsKey(user.ConnectionId))
        {
            return;
        }

        // 自分は追加しない
        if (user.ConnectionId == roomModel.ConnectionId)
        {
            if (user.JoinOrder == 0)
            {
                matchingManager.isHost = true;
                LogManager.SetLogText("入室しました。");
            }

            isJoin = true;
            //return;  
        }

        GameObject obj = Instantiate(characterPrefab,
            matchingManager.PlayerPos[user.JoinOrder].position,
            Quaternion.identity);

        obj.transform.transform.rotation = new Quaternion(0,180,0,0);

        BikeController bike = obj.GetComponent<BikeController>();

        bike.NameText.text = user.UserData.Name;

        matchingManager.waitTime = 0;

        Vector3 direction = Camera.main.transform.position - bike.NameText.transform.position;
        bike.NameText.transform.rotation = Quaternion.LookRotation(-direction);
    }

    //ユーザーが退室した時の処理
    private void OnLeavedUser(JoinedUser user)
    {
        if(user.UserData.Id == myUserId)
        {
            return;
        }

        Destroy(characterList[user.ConnectionId]);
        characterList[user.ConnectionId] = null;

        if (user.JoinOrder + 1 == roomModel.userTable[roomModel.ConnectionId].JoinOrder) 
        {
            MatchingManager matchingManager = GameObject.Find("MatchingManager").GetComponent<MatchingManager>();

            matchingManager.isHost = true;
            matchingManager.waitTime = 0;
        }

        Debug.Log("削除");
    }



    // 自分以外のユーザーの移動を反映
    private void OnMoveUser(Guid connectionId, Vector3 pos, Quaternion rot, Vector3 scale)
    {

        GameObject reusltObj = GameObject.Find("ResultManager");

        if (reusltObj != null)
        {
            return;
        }

        // いない人は移動できない
        if (!characterList.ContainsKey(connectionId))
        {
            Debug.Log("いないよぉ！");
            return;
        }

        // DOTweenを使うことでなめらかに動く！
        characterList[connectionId].transform.DOMove(pos, 0.1f);
        characterList[connectionId].transform.rotation = rot;
        characterList[connectionId].transform.localScale = scale;
    }

    private void OnPutItemUser(Guid connectionId, Vector3 pos, int ItemType)
    {

        GameObject reusltObj = GameObject.Find("ResultManager");

        if (reusltObj != null)
        {
            return;
        }
        BikeController bike;
        GameManager gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        

        

        if (connectionId == roomModel.ConnectionId)
        {
            bike = bikeController;
        }
        else
        {
            bike = characterList[connectionId].GetComponent<BikeController>();
        }
        GameObject putItem = bikeController.BananaPrefab;

        if (ItemType == (int)BikeController.Item.SpeedDown)
        {
            putItem = bike.BananaPrefab;
        }
        else if(ItemType == (int)BikeController.Item.PowerDown)
        {
            putItem = bike.BedPrefab;
        }

        Vector3 PutPos = new Vector3(pos.x, pos.y - 0.5f, pos.z);
        //Instantiate(gameManager.Items[ItemType - 1],
         Instantiate(putItem,
           PutPos - (bike.transform.forward * 2f),
            Quaternion.identity);
    }


    //自分以外のユーザーのチェックポイント状況を反映
    private void OnPassCheckPoint(Guid connectionId)
    {
        GameObject reusltObj = GameObject.Find("ResultManager");

        if (reusltObj != null)
        {
            return;
        }
        BikeController bike = characterList[connectionId].GetComponent<BikeController>();

        if (!bike.controllingBike)
        {
            bike.nowCheckPoint = bike.nowCheckPoint.nextCheckPoint;
            bike.checkCount++;

            Debug.Log($"{connectionId}がチェックポイント通過");
        }
    }

    //自分以外のゴール状況を反映
    private void OnGoalUser(Guid connectionId)
    {
        if (!bikeController.isGoal)
        {
            BikeController bike = characterList[connectionId].GetComponent<BikeController>();

            if (!bike.controllingBike)
            {
                bike.isGoal = true;

                Debug.Log($"{connectionId}がゴール通過");
            }
        }

        JoinedUser goalUser = this.roomModel.userTable[connectionId];

        GoalPlayerList[connectionId] = goalUser.UserData;

        GameObject reusltObj = GameObject.Find("ResultManager");

        if (reusltObj != null)
        {
            ResultManager resultManager = reusltObj.GetComponent<ResultManager>();

            resultManager.SetPlayer(goalUser.UserData);
        }
    }

    private void OnMenberConfirmed(int BattleId)
    {
        MatchingManager matchingManager = GameObject.Find("MatchingManager").GetComponent<MatchingManager>();
        if (matchingManager != null)
        {
            StartCoroutine(matchingManager.StartGame());
            this.battleId = BattleId;
        }
    }

    private void OnStartGame()
    {
        GameManager gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        if (gameManager != null)
        {
            StartCoroutine(gameManager.CountDown());
        }
    }



}
