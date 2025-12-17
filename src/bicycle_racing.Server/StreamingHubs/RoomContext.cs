using Cysharp.Runtime.Multicast;
using bicycle_racing.Shared.Interfaces.StreamingHubs;
using bicycle_racing.Server.StreamingHubs;

namespace bicycle_racing.Server.StreamingHubs
{
    public class RoomContext : IDisposable
    {
        public Guid Id { get; } // ルームID
        public string Name { get; } // ルーム名
        public IMulticastSyncGroup<Guid, IRoomHubReceiver> Group { get; } // グループ
        public Dictionary<Guid, RoomUserData> RoomUserDataList { get; } =
            new Dictionary<Guid, RoomUserData>(); // ユーザデータ一覧

        public bool isStart = false;

        public int StageID;

        public int BattleId;

        //その他、ルームのデータとして保存したいものをフィールドに追加していく
        // コンストラクタ
        public RoomContext(IMulticastGroupProvider groupProvider, string roomName,int StageId)
        {
            Id = Guid.NewGuid();  	//ルーム毎のデータにIDを付けておく
            Name = roomName;    //ルーム名をフィールドに保存
           Group = 			//グループを作成
            groupProvider.GetOrAddSynchronousGroup<Guid, IRoomHubReceiver>(roomName);
           isStart = false;
           StageID = StageId;
        }
        public void Dispose()
        {
            Group.Dispose();
        }
    }
}
