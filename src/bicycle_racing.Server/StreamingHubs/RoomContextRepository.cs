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

        // ルームコンテキストの作成
        public RoomContext CreateContext(string roomName)
        {
            var context = new RoomContext(groupProvider, roomName);
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
    }
}


