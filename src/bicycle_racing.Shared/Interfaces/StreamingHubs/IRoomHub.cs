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
        Task<JoinedUser[]> JoinAsync(string roomName, int userId);

        // サーバー退出
        Task LeaveAsync();

        //ユーザーの移動
        Task MoveAsync(Vector3 pos,Quaternion rot);

        Task<Guid> GetConnectionId();
    }

}
