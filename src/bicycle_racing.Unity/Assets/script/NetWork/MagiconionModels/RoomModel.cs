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
    public Action<Guid,Vector3,Quaternion> OnMoveUser { get; set; }

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
        if (roomHub != null) await roomHub.DisposeAsync();
        if (channel != null) await channel.ShutdownAsync();
        roomHub = null; channel = null;
    }

    //　破棄処理 
    async void OnDestroy()
    {
        DisconnectAsync();
    }

    public async UniTask JoinAsync(string roomName, int userId)
    {
        JoinedUser[] users = await roomHub.JoinAsync(roomName, userId);
        foreach (JoinedUser user in users)
        {
            userTable[user.ConnectionId] = user;  //保持

            if (OnJoinedUser != null)
            {
                
                
                OnJoinedUser(user);
            }
        }

    }

    public async UniTask LeaveAsync()
    {
       await roomHub.LeaveAsync();     
    }

    public async UniTask MoveAsync(Vector3 pos,Quaternion rot)
    {
        await roomHub.MoveAsync(pos,rot);
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
    public void OnMove(Guid ID,Vector3 pos,Quaternion rot)
    {
        JoinedUser user;

        userTable.TryGetValue(ID, out user);

        if (OnMoveUser != null)
        {
            OnMoveUser(user.ConnectionId, pos, rot);
        }
    }


}


