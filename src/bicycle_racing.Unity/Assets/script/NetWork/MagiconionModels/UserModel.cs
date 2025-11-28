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
    public async UniTask<bool> RegistUserAsync(string name)
    {
        var channel = GrpcChannelx.ForAddress(ServerURL);
        var client = MagicOnionClient.Create<IUserService>(channel);
        try
        {  //登録成功
            userId = await client.RegistUserAsync(name);
            return true;
        }
        catch (RpcException e)
        {  //登録失敗
            Debug.Log(e);
            return false;
        }
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
        var result = await client.GetUserAsync(id);

        return result;
    }

    public async UnaryResult<User> UpdateUser(int id, string name)
    {
        var channel = GrpcChannelx.ForAddress(ServerURL);
        var client = MagicOnionClient.Create<IUserService>(channel);
        var result = await client.UpdateUserAsync(id, name);

        return result;
    }
}

