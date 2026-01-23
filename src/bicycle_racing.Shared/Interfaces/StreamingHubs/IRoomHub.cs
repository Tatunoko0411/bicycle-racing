using MagicOnion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace bicycle_racing.Shared.Interfaces.StreamingHubs
{
    /// <summary>
    /// クライアントから呼び出す処理を実装するクラス用インターフェース
    /// </summary>
    public interface IRoomHub : IStreamingHub<IRoomHub, IRoomHubReceiver>
    {
        // [サーバーに実装]
        // [クライアントから呼び出す]

        // ユーザー入室
        Task<JoinedUser[]> JoinAsync(int userId,int StageId);

        Task StartAsync(int StageId);

        Task<JoinedUser[]> JoinFriendAsync(int userId,int StageId, string roomNAme);
        // サーバー退出
        Task LeaveAsync();

        //ユーザーの移動
        Task MoveAsync(Vector3 pos,Quaternion rot,Vector3 scale);

        //アイテム設置
        Task PutItemAsync(Vector3 pos,int ItemType);

        //チェックポイント通過
        Task PassCheckAsync();

        //ゴール
        Task GoalAsync(int rank);

        //準備完了
        Task ReadyAsync();

        Task<Guid> GetConnectionId();

       


    }

}
