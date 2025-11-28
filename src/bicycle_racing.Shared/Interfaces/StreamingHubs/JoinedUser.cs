using MessagePack;
using bicycle_racing.Shared.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace bicycle_racing.Shared.Interfaces.StreamingHubs
{
    /// <summary>
    /// 接続済みユーザー
    /// </summary>
    [MessagePackObject]
    public class JoinedUser
    {
        [Key(0)]
        public Guid ConnectionId { get; set; }// 接続ID
        [Key(1)]
        public User UserData { get; set; }// ユーザー情報
        [Key(2)]
        public int JoinOrder { get; set; } // 参加順番
    }

}
