using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MagicOnion;
using UnityEngine;

namespace bicycle_racing.Shared.Interfaces.StreamingHubs
{
    /// <summary>
    /// サーバーからクライアントへの通知関連
    /// </summary>
    public interface IRoomHubReceiver
    {
        // [クライアントに実装]
        // [サーバーから呼び出す]

        // ユーザーの入室通知
        void OnJoin(JoinedUser user);

        //ユーザーの退出通知
        void OnLeave(Guid ID);

        void OnMove(Guid ID,Vector3 pos,Quaternion rot);
    }

}
