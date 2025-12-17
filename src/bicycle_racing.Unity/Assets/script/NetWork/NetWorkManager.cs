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

public class NetWorkManager : MonoBehaviour
{
    [SerializeField] GameObject characterPrefab;
    [SerializeField] InputField roomNameField;
    [SerializeField] InputField PlayerIdField;
    [SerializeField] Text info;
    Dictionary<Guid, GameObject> characterList = new Dictionary<Guid, GameObject>();

    RoomModel roomModel;
    UserModel userModel;

    int myUserId = 1;
    User myself;

    int battleId = 0;   

    float waitTime = 0;

    bool isJoin = false;

    [SerializeField]BikeController bikeController;


    public void Awake()
    {
        DontDestroyOnLoad(this);
    }

    async void Start()
    {
        roomModel = GetComponent<RoomModel>();
        userModel = GetComponent<UserModel>();

        //ユーザーが入室した時にOnJoinedUserメソッドを実行するよう、モデルに登録しておく
        roomModel.OnJoinedUser += this.OnJoinedUser;
        roomModel.OnLeavedUser += this.OnLeavedUser;
        roomModel.OnMoveUser += this.OnMoveUser;
        roomModel.OnPassCheckPoint += this.OnPassCheckPoint;
        roomModel.OnGoalUser += this.OnGoalUser;
        roomModel.OnMenberConfirmed += this.OnMenberConfirmed;
        roomModel.OnStartGame += this.OnStartGame;
        //接続
        await roomModel.ConnectAsync();

        try
        {
            // ユーザー情報を取得
            myself = await userModel.GetUser(myUserId);

            Debug.Log("取得成功");
        }
        catch (Exception e)
        {
            Debug.Log("RegistUser failed");
            Debug.LogException(e);
        }

        

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


    public async void JoinRoom(int StageId)
    {
        int ID = myUserId;

        //入室
        Debug.Log("入室処理開始");
        await roomModel.JoinAsync(ID,StageId);
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
                    isJoin = true;

                }
                else
                {

                    GameObject characterObject = Instantiate(characterPrefab);  //インスタンス生成
                    characterObject.transform.position = StartPos;
                    characterObject.transform.rotation =  new Quaternion(0, -90, 0, 0);
                    BikeController bike = characterObject.GetComponent<BikeController>();


                 

                    characterList[joinedUser.ConnectionId] = characterObject;//フィールドで保持
                }

                cnt++;
            }
        }
    }

    public async void Ready()
    {
        await roomModel.ReadyAsync();
    }


    public async void LeaveRoom()
    {
        await roomModel.LeaveAsync();
        isJoin = false;
    }

    public async void Move()
    {
        await roomModel.MoveAsync(bikeController.gameObject.transform.position, bikeController.gameObject.transform.rotation);
    }

    public async void PassCheck()
    {
        await roomModel.PassCheckAsync();
    }

    public async void Goal(int rank)
    {
        await roomModel.OnGoalAsync(rank);
    }

    //ユーザーが入室した時の処理
    private void OnJoinedUser(JoinedUser user)
    {
        // すでに表示済みのユーザーは追加しない
        if (characterList.ContainsKey(user.ConnectionId))
        {
            return;
        }

        // 自分は追加しない
        if (user.ConnectionId == roomModel.ConnectionId)
        {
            //info.text = ($"接続ID：{user.ConnectionId} " +
            //   $"ユーザーID:{user.UserData.Id}" +
            //   $" ユーザー名：{user.UserData.Name}");

            isJoin = true;
            return;
           
        }

        //GameObject characterObject = Instantiate(characterPrefab);  //インスタンス生成
        //characterObject.transform.position = new Vector3(bikeController.transform.position.x+10,bikeController.transform.position.y,bikeController.transform.position.x);
        //BikeController bike = characterObject.GetComponent<BikeController>();
       
        //characterList[user.ConnectionId] = characterObject;  //フィールドで保持
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

        Debug.Log("削除");
    }



    // 自分以外のユーザーの移動を反映
    private void OnMoveUser(Guid connectionId, Vector3 pos, Quaternion rot)
    {
        // いない人は移動できない
        if (!characterList.ContainsKey(connectionId))
        {
            Debug.Log("いないよぉ！");
            return;
        }

        // DOTweenを使うことでなめらかに動く！
        characterList[connectionId].transform.DOMove(pos, 0.1f);
        characterList[connectionId].transform.rotation = new Quaternion(rot.x,rot.y,rot.z,rot.w);
    
    }


    //自分以外のユーザーのチェックポイント状況を反映
    private void OnPassCheckPoint(Guid connectionId)
    {
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
        BikeController bike = characterList[connectionId].GetComponent<BikeController>();

        if (!bike.controllingBike)
        {
            bike.isGoal = true;

            Debug.Log($"{connectionId}がゴール通過");
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
