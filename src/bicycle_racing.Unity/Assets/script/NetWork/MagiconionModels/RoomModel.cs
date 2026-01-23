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

public class RoomModel : BaseModel, IRoomHubReceiver
{
    private GrpcChannelx channel;
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

    //　MagicOnion接続処理
    public async UniTask ConnectAsync()
    {
        channel = GrpcChannelx.ForAddress(ServerURL);
        roomHub = await StreamingHubClient.
             ConnectAsync<IRoomHub, IRoomHubReceiver>(channel, this);
        this.ConnectionId = await roomHub.GetConnectionId();
    }
    //　MagicOnion切断処理
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

    public async UniTask JoinAsync( int userId,int StageId)
    {
        JoinedUser[] users = await roomHub.JoinAsync(userId, StageId);
        foreach (JoinedUser user in users)
        {

            if(user == null) continue;
            userTable[user.ConnectionId] = user;  //保持

            if (OnJoinedUser != null)
            {
                
                
                OnJoinedUser(user);
            }
        }

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


