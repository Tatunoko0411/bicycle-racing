using Cysharp.Threading.Tasks;
using Grpc.Core;
using MagicOnion.Client;
using MagicOnion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bicycle_racing.Shared.Interfaces;
using UnityEngine;
using bicycle_racing.Shared.Models.Entities;

public class UserModel : BaseModel
{
    private int userId;  //登録ユーザーID
    public async UniTask<int> RegistUserAsync(string name)
    {
        var channel = GrpcChannelx.ForAddress(ServerURL);
        var client = MagicOnionClient.Create<IUserService>(channel);
        try
        {  //登録成功
            userId = await client.RegistUserAsync(name);
            return userId;
        }
        catch (RpcException e)
        {  //登録失敗
            Debug.Log(e);
            return 0;
        }
    }

    public async UnaryResult<User> UpdateUserCountAsync(int id, int play, int win)
    {
        var channel = GrpcChannelx.ForAddress(ServerURL);
        var client = MagicOnionClient.Create<IUserService>(channel);
        var result = await client.UpdateUserCountAsync(id,play,win);

        return result;
    }

    public async UnaryResult<int> RegistUser(string name)
    {
        var channel = GrpcChannelx.ForAddress(ServerURL);
        var client = MagicOnionClient.Create<IUserService>(channel);
        var result = await client.RegistUserAsync(name);

        return result;
    }

    public async UnaryResult<User> GetUser(int id)
    {
        var channel = GrpcChannelx.ForAddress(ServerURL);
        var client = MagicOnionClient.Create<IUserService>(channel);
        var result = await client.GetUserAsyncWithID(id);

        return result;
    }

    public async UnaryResult<User> GetUser(string name)
    {
        var channel = GrpcChannelx.ForAddress(ServerURL);
        var client = MagicOnionClient.Create<IUserService>(channel);
        var result = await client.GetUserAsyncWithName(name);

        return result;
    }

    public async UnaryResult<User> UpdateUser(int id, string name)
    {
        var channel = GrpcChannelx.ForAddress(ServerURL);
        var client = MagicOnionClient.Create<IUserService>(channel);
        var result = await client.UpdateUserAsync(id, name);

        return result;
    }

    public async UnaryResult<int> RegistFriend(int Id1,int Id2)
    {
        var channel = GrpcChannelx.ForAddress(ServerURL);
        var client = MagicOnionClient.Create<IFriendService>(channel);
        var result = await client.RegistFriendAsync(Id1,Id2);

        return result;
    }

    public async UnaryResult<Friend[]> GetFriend(int Id)
    {
        var channel = GrpcChannelx.ForAddress(ServerURL);
        var client = MagicOnionClient.Create<IFriendService>(channel);
        var result = await client.GetFriendAsync(Id);

        return result;
    }

    public async UnaryResult<Task> RmoveFriend(int Id)
    {
        var channel = GrpcChannelx.ForAddress(ServerURL);
        var client = MagicOnionClient.Create<IFriendService>(channel);
        var result = await client.RemoveFriendAsync(Id);

        return result;
    }
    public async UnaryResult<User> ChangeState(int id,int State)
    {
        var channel = GrpcChannelx.ForAddress(ServerURL);
        var client = MagicOnionClient.Create<IUserService>(channel);
        var result = await client.ChangeStateUserAsync(id, State);

        return result;
    }

    public async UnaryResult<User> SetRoomeName(int id,int StageId,string name)
    {
        var channel = GrpcChannelx.ForAddress(ServerURL);
        var client = MagicOnionClient.Create<IUserService>(channel);
        var result = await client.SetRoomNameUserAsync(id,StageId,name);

        return result;
    }
}

