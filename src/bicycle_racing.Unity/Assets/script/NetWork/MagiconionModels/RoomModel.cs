using Cysharp.Threading.Tasks;
using MagicOnion.Client;
using MagicOnion;
using bicycle_racing.Shared.Interfaces.StreamingHubs;
using System;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using UnityEngine.UI;
using System.Threading.Tasks;
using NUnit.Framework;
using System.Collections.Generic;
using Unity.Mathematics;
using static UnityEngine.Rendering.DebugUI.Table;
using Grpc.Net.Client;

public class RoomModel : BaseModel, IRoomHubReceiver
{
    private GrpcChannel channel;
    private IRoomHub roomHub;

    //　接続ID
    public Guid ConnectionId { get; set; }

    //　ユーザー接続通知
    public Action<JoinedUser> OnJoinedUser { get; set; }

    //　ユーザー切断通知
    public Action<JoinedUser> OnLeavedUser { get; set; }

    //ユーザーの移動通知
    public Action<Guid,Vector3,Quaternion,Vector3> OnMoveUser { get; set; }
    //ユーザーのアイテム設置通知
    public Action<Guid,Vector3,int>OnPutItemUser { get; set; }

   //ユーザーのチェックポイント通過通知
    public Action<Guid> OnPassCheckPoint { get; set; }
   //ユーザーのゴール通知
    public Action<Guid> OnGoalUser { get; set; }
    //マッチング確定通知
    public Action<int> OnMenberConfirmed { get; set; }
    //ゲーム開始通知
    public Action OnStartGame { get; set; }
    public Dictionary<Guid, JoinedUser> userTable { get; set; } = new Dictionary<Guid, JoinedUser>();


    // MagicOnion接続処理
    public async UniTask ConnectAsync()
    {
        try
        {
            Debug.Log("ConnectAsync start");// ★修正: 共通プロバイダーから、設定済みのチャンネルをもらうだけ！

            channel = GrpcChannelProvider.GetChannel();
            Debug.Log("Connecting to Hub...");

            try
            {
                // 2. 画像にある通り、第4引数（option）に渡します
                // 第3引数の host は省略（null）するため、名前付き引数を使うのが安全です
                this.roomHub = await MagicOnion.Client.StreamingHubClient.ConnectAsync<IRoomHub, IRoomHubReceiver>(
                                                                                                                   channel,
                                                                                                                   this,
                                                                                                                   option: commonCallOptions
                                                                                                                   );

                Debug.Log("★Success: Connected with CallOptions Headers!");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"ConnectAsync failed: {e}");
            }

            Debug.Log("ConnectAsync Success!");

            this.ConnectionId = await roomHub.GetConnectionId();
            Debug.Log(ConnectionId);
           // myObject = GameObject.Find("Player").gameObject;
        }
        catch (Exception ex)
        {
            Debug.LogError("ConnectAsync failed: " + ex);
        }
    }

    public async UniTask DisconnectAsync()
    {
        
        if (roomHub != null) { 
        

            await roomHub.DisposeAsync(); 
        }
        if (channel != null) await channel.ShutdownAsync();
        roomHub = null; channel = null;
    }

    //　破棄処理 
    async void OnDestroy()
    {
        
        DisconnectAsync();
    }


    public async UniTask JoinAsync(int userId, int StageId)
    {
        JoinedUser[] users = await roomHub.JoinAsync(userId, StageId);
        // Debug.Log($"ルーム名: {roomName}");

        // ルーム全員分の情報が来るので、自分以外のオブジェクトを生成する
        foreach (var user in users)
        {
            userTable[user.ConnectionId] = user;

            if (OnJoinedUser != null)
            {
                OnJoinedUser(user);
                Debug.Log($"接続ID: {user.ConnectionId}, ユーザーID: {user.UserData.Id}, ユーザー名: {user.UserData.Name}");
            }
        }

        // 接続できたら自分の位置情報を共有する
        InvokeRepeating("MoveAsync", 0, 0.1f);
    }


    public async UniTask JoinFriendAsync(int userId, int StageId,string RoomName)
    {
        JoinedUser[] users = await roomHub.JoinFriendAsync(userId, StageId,RoomName);
        foreach (JoinedUser user in users)
        {
            userTable[user.ConnectionId] = user;  //保持

            if (OnJoinedUser != null)
            {


                OnJoinedUser(user);
            }
        }

    }

    public async UniTask StartGameAsync(int stageId)
    {
        await roomHub.StartAsync(stageId);
    }
    public async UniTask ReadyAsync()
    {
        await roomHub.ReadyAsync();
    }

    public async UniTask LeaveAsync()
    {
       await roomHub.LeaveAsync();
    }


    public async UniTask MoveAsync(Vector3 pos,Quaternion rot, Vector3 scale)
    {
        await roomHub.MoveAsync(pos,rot,scale);
    }

    public async UniTask PutItemAsync(Vector3 pos, int ItemType)
    {
        await roomHub.PutItemAsync(pos,ItemType);
    }

    public async UniTask PassCheckAsync()
    { 
        await roomHub.PassCheckAsync();
    }

    public async UniTask OnGoalAsync(int rank)
    {
        await roomHub.GoalAsync(rank);
    }

    //　入室通知 (IRoomHubReceiverインタフェースの実装)
    public void OnJoin(JoinedUser user)
    {
        userTable[user.ConnectionId] = user;
        if (OnJoinedUser != null)
        {
            OnJoinedUser(user);
        }
    }

    //　退出通知 (IRoomHubReceiverインタフェースの実装)
    public void OnLeave(Guid ID)
    {
        Debug.Log($"消すよ：{ID}");

        JoinedUser user;
            
        userTable.TryGetValue(ID,out user);

        if (OnLeavedUser != null)
        {
            OnLeavedUser(user);
        }
        
    }

    // 移動通知(IRoomHubReceiverインタフェースの実装)
    public void OnMove(Guid ID,Vector3 pos,Quaternion rot,Vector3 scale)
    {
        JoinedUser user;

        userTable.TryGetValue(ID, out user);

        if (OnMoveUser != null)
        {
            OnMoveUser(user.ConnectionId, pos, rot,scale);
        }
    }

    public void OnPutItem(Guid ID, Vector3 pos, int ItemType)
    {
        JoinedUser user;

        userTable.TryGetValue(ID, out user);

        if (OnPutItemUser != null)
        {
            OnPutItemUser(user.ConnectionId, pos, ItemType);
        }
    }

    public void OnPassCheck(Guid ID)
    {
        JoinedUser user;
        userTable.TryGetValue(ID, out user);

        if (OnMoveUser != null)
        {
            OnPassCheckPoint(user.ConnectionId);
        }
    }

    public  void OnGoal(Guid ID)
    {
        JoinedUser user;
        userTable.TryGetValue(ID, out user);

        if (OnMoveUser != null)
        {
            OnGoalUser(user.ConnectionId);
        }
    }

    public void OnConfirmed(int BattleId)
    {
        OnMenberConfirmed(BattleId);
    }

    public void OnStart()
    {
        OnStartGame();
    }
}


