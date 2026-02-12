using Cysharp.Threading.Tasks;
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
using Grpc.Net.Client;
using Grpc.Core;
using Grpc.Core.Interceptors;

public class UserModel : BaseModel
{
    private int userId;  //登録ユーザーID


    /// <summary>
    /// ユーザ登録
    /// </summary>
    /// <param name="name">ユーザ名</param>
    /// <returns>ユーザID</returns>
    public async UnaryResult<int> RegistUser(string name)
    {
        var channel = GrpcChannelProvider.GetChannel();
        var invoker = channel.Intercept(new GameIdInterceptor("ge202402"));
        var client = MagicOnionClient.Create<IUserService>(invoker);
        var result = await client.RegistUserAsync(name);

        LogManager.SetLogText(result.ToString());
        return result;
    }


    /// <summary>
    /// ユーザーの対戦履歴更新
    /// </summary>
    /// <param name="id">プレイヤーID</param>
    /// <param name="play">プレイ回数</param>
    /// <param name="win">勝利回数</param>
    /// <returns>ユーザー情報</returns>

    public async UnaryResult<User> UpdateUserCountAsync(int id, int play, int win)
    {
        var channel = GrpcChannelProvider.GetChannel();
        var invoker = channel.Intercept(new GameIdInterceptor("ge202402"));
        var client = MagicOnionClient.Create<IUserService>(invoker);
        var result = await client.UpdateUserCountAsync(id,play,win);

        return result;
    }

    /// <summary>
    /// ユーザ取得（ID指定）
    /// </summary>
    /// <param name="id">取得したいユーザID</param>
    /// <returns>ユーザ情報</returns>
    public async UnaryResult<User> GetUser(int id)
    {
        var channel = GrpcChannelProvider.GetChannel();
        var invoker = channel.Intercept(new GameIdInterceptor("ge202402"));
        var client = MagicOnionClient.Create<IUserService>(invoker);
        var result = await client.GetUserAsyncWithID(id);

        return result;
    }


    /// <summary>
    /// ユーザ取得（名前指定）
    /// </summary>
    /// <param name="name">取得したいユーザ名</param>
    /// <returns>ユーザ名</returns>
    public async UnaryResult<User> GetUser(string name)
    {
        var channel = GrpcChannelProvider.GetChannel();
        var invoker = channel.Intercept(new GameIdInterceptor("ge202402"));
        var client = MagicOnionClient.Create<IUserService>(invoker);
        var result = await client.GetUserAsyncWithName(name);

        return result;
    }

    /// <summary>
    /// ユーザ名更新
    /// </summary>
    /// <param name="id">ユーザID</param>
    /// <param name="name">更新後の名前</param>
    /// <returns>更新後のユーザ情報</returns>
    public async UnaryResult<User> UpdateUser(int id, string name)
    {
        var channel = GrpcChannelProvider.GetChannel();
        var invoker = channel.Intercept(new GameIdInterceptor("ge202402"));
        var client = MagicOnionClient.Create<IUserService>(invoker);
        var result = await client.UpdateUserAsync(id, name);

        return result;
    }

    /// <summary>
    /// フレンド登録
    /// </summary>
    /// <param name="Id1">自身のID</param>
    /// <param name="Id2">フレンドにするほかユーザのID</param>
    /// <returns>データベース上のID</returns>
    public async UnaryResult<int> RegistFriend(int Id1,int Id2)
    {
        var channel = GrpcChannelProvider.GetChannel();
        var invoker = channel.Intercept(new GameIdInterceptor("ge202402"));
        var client = MagicOnionClient.Create<IFriendService>(invoker);
        var result = await client.RegistFriendAsync(Id1,Id2);

        return result;
    }

    /// <summary>
    /// フレンド情報取得
    /// </summary>
    /// <param name="Id">自身のID</param>
    /// <returns>フレンド情報</returns>
    public async UnaryResult<Friend[]> GetFriend(int Id)
    {
        var channel = GrpcChannelProvider.GetChannel();
        var invoker = channel.Intercept(new GameIdInterceptor("ge202402"));
        var client = MagicOnionClient.Create<IFriendService>(invoker);
        var result = await client.GetFriendAsync(Id);

        return result;
    }


    /// <summary>
    /// フレンド削除
    /// </summary>
    /// <param name="Id">フレンドID</param>
    /// <returns>削除成功可否</returns>
    public async UnaryResult<Task> RmoveFriend(int Id)
    {
        var channel = GrpcChannelProvider.GetChannel();
        var invoker = channel.Intercept(new GameIdInterceptor("ge202402"));
        var client = MagicOnionClient.Create<IFriendService>(invoker);
        var result = await client.RemoveFriendAsync(Id);

        return result;
    }

    /// <summary>
    /// ユーザのステータス変更
    /// </summary>
    /// <param name="id">ユーザID</param>
    /// <param name="State">ステータス情報</param>
    /// <returns>更新後のユーザ情報</returns>
    public async UnaryResult<User> ChangeState(int id,int State)
    {
        var channel = GrpcChannelProvider.GetChannel();
        var invoker = channel.Intercept(new GameIdInterceptor("ge202402"));
        var client = MagicOnionClient.Create<IUserService>(invoker);
        var result = await client.ChangeStateUserAsync(id, State);

        return result;
    }

    /// <summary>
    /// ユーザの接続情報更新
    /// </summary>
    /// <param name="id">ユーザID</param>
    /// <param name="StageId">ステージID</param>
    /// <param name="name">ルーム名</param>
    /// <returns>更新後のユーザ情報</returns>
    public async UnaryResult<User> SetRoomeName(int id,int StageId,string name)
    {
        var channel = GrpcChannelProvider.GetChannel();
        var invoker = channel.Intercept(new GameIdInterceptor("ge202402"));
        var client = MagicOnionClient.Create<IUserService>(invoker);
        var result = await client.SetRoomNameUserAsync(id,StageId,name);

        return result;
    }
}

