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

        //移動通知
        void OnMove(Guid ID,Vector3 pos,Quaternion rot,Vector3 scale);
 
        //アイテム設置通知
        void OnPutItem(Guid ID,Vector3 pos ,int ItemType);

        //チェックポイント通過通知
        void OnPassCheck(Guid ID);

        //ゴール通知
        void OnGoal(Guid ID);

        //マッチ完了通知
        void OnConfirmed(int BattleId);

        //スタート通知
        void OnStart();
    }

}
