using Cysharp.Threading.Tasks;
using Grpc.Net.Client;
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
using Grpc.Core.Interceptors;



public class MailModel : BaseModel
{
    /// <summary>
    /// メール登録
    /// </summary>
    /// <param name="SendId">送信したユーザーID</param>
    /// <param name="ReceiveId">受け取るユーザーID</param>
    /// <param name="Type">メールのタイプ</param>
    /// <param name="content">メールの内容</param>
    /// <returns>登録されたメールのID</returns>
    public async UniTask<int> CreateMailAsync(int SendId, int ReceiveId, int Type, string content)
    {
        var channel = GrpcChannelProvider.GetChannel();
        var invoker = channel.Intercept(new GameIdInterceptor("ge202402"));
        var client = MagicOnionClient.Create<IMailService>(invoker);
        var result = await client.CreateMailAsync(SendId,ReceiveId,Type,content);

        return result;
    }

    /// <summary>
    /// メールのステータス変更
    /// </summary>
    /// <param name="ID">メールのID</param>
    /// <param name="state">変更後のステータス</param>
    /// <returns>変更後のメール情報</returns>
    public async UniTask<Mail> ChangeStateMailAsync(int ID,int state)
    {
        var channel = GrpcChannelProvider.GetChannel();
        var invoker = channel.Intercept(new GameIdInterceptor("ge202402"));
        var client = MagicOnionClient.Create<IMailService>(invoker);
        var result = await client.ChangeStateMailAsync(ID,state);

        return result;
    }

    /// <summary>
    /// メール取得
    /// </summary>
    /// <param name="playerID">自分のユーザーID</param>
    /// <returns>取得したメール情報</returns>
    public async UniTask<Mail[]> GetMailAysnc(int playerID)
    {
        var channel = GrpcChannelProvider.GetChannel();
        var invoker = channel.Intercept(new GameIdInterceptor("ge202402"));
        var client = MagicOnionClient.Create<IMailService>(invoker);
        var result = await client.GetMailAsync(playerID);

        return result;
    }

    /// <summary>
    /// メール削除
    /// </summary>
    /// <param name="ID">メールID</param>
    /// <returns>完了可否</returns>
    public async UniTask<bool> RemoveMailAsync(int ID)
    {
        var channel = GrpcChannelProvider.GetChannel();
        var invoker = channel.Intercept(new GameIdInterceptor("ge202402"));
        var client = MagicOnionClient.Create<IMailService>(invoker);
        var result = await client.RemoveMailAsync(ID);

        return result;
    }
}
