using Cysharp.Runtime.Multicast;
using MagicOnion.Server.Hubs;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using bicycle_racing.Server.Models.Contexts;
using bicycle_racing.Shared.Interfaces.StreamingHubs;
using System.Collections.Concurrent;

namespace bicycle_racing.Server.StreamingHubs
{
    public class RoomContextRepository(IMulticastGroupProvider groupProvider)
    {
        private readonly ConcurrentDictionary<string, RoomContext> contexts =
            new ConcurrentDictionary<string, RoomContext>();

        public int MaxUsers = 2;
        public int MinUsers = 2;


        // ルームコンテキストの作成
        public RoomContext CreateContext(string roomName,int StageId)
        {
            var context = new RoomContext(groupProvider, roomName,StageId);
            contexts[roomName] = context;
            return context;
        }
        // ルームコンテキストの取得
        public RoomContext GetContext(string roomName)
        {
            if (!contexts.ContainsKey(roomName))
            {
                return null;
            }
            return contexts[roomName];
        }

        // ルームコンテキストの削除
        public void RemoveContext(string roomName)
        {
            if (contexts.Remove(roomName, out var RoomContext))
            {
                RoomContext?.Dispose();
            }
        }

        public RoomContext FindContext(int stageId)
        {
            foreach (var item in contexts)
            {
                if(!item.Value.isStart)
                {

                    if (item.Value.StageID == stageId)
                    {
                        return item.Value;
                    }
                }
            }

            //全ルームがスタートしていたら新規作成
            return CreateContext(Guid.NewGuid().ToString(),stageId);
        }
    }
}


