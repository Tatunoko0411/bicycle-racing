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



public class MailModel : BaseModel
{

    public async UniTask<int> CreateMailAsync(int SendId, int ReceiveId, int Type, string content)
    {
        var channel = GrpcChannelx.ForAddress(ServerURL);
        var client = MagicOnionClient.Create<IMailService>(channel);
        var result = await client.CreateMailAsync(SendId,ReceiveId,Type,content);

        return result;
    }

    public async UniTask<Mail> ChangeStateMailAsync(int ID,int state)
    {
        var channel = GrpcChannelx.ForAddress(ServerURL);
        var client = MagicOnionClient.Create<IMailService>(channel);
        var result = await client.ChangeStateMailAsync(ID,state);

        return result;
    }

    public async UniTask<Mail[]> GetMailAysnc(int playerID)
    {
        var channel = GrpcChannelx.ForAddress(ServerURL);
        var client = MagicOnionClient.Create<IMailService>(channel);
        var result = await client.GetMailAsync(playerID);

        return result;
    }

    public async UniTask<bool> RemoveMailAsync(int ID)
    {
        var channel = GrpcChannelx.ForAddress(ServerURL);
        var client = MagicOnionClient.Create<IMailService>(channel);
        var result = await client.RemoveMailAsync(ID);

        return result;
    }
}
