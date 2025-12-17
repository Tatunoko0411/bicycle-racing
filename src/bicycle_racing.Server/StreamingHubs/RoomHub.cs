using MagicOnion.Server.Hubs;
using bicycle_racing.Server.Models.Contexts;
using bicycle_racing.Shared.Models.Entities;
using bicycle_racing.Shared.Interfaces.StreamingHubs;

using UnityEngine;
using realtime_game.Server.Services;

namespace bicycle_racing.Server.StreamingHubs
{
    public class RoomHub(RoomContextRepository roomContextRepository) : StreamingHubBase<IRoomHub,IRoomHubReceiver>, IRoomHub
    {
        private RoomContextRepository roomContextRepos;
        private RoomContext roomContext;
        private BattleService battleService = new BattleService();
        private BattleLogService battleLogService = new BattleLogService();

        //ルームに接続
        public async Task<JoinedUser[]> JoinAsync(int userId,int StageId)
        {
            // 同時に生成しないように排他制御
            lock (roomContextRepos)
            {
                //ルーム検索
                this.roomContext = roomContextRepos.FindContext(StageId);

            }

            // ルームに参加 & ルームを保持
            this.roomContext.Group.Add(this.ConnectionId, Client);

            // DBからユーザー情報取得
            GameDbContext context = new GameDbContext();
            User user = context.Users.Where(user => user.Id == userId).First();

            // 入室済みユーザーのデータを作成
            var joinedUser = new JoinedUser();
            joinedUser.ConnectionId = this.ConnectionId;
            joinedUser.UserData = user;

            // ルームコンテキストにユーザー情報を登録
            var roomUserData = new RoomUserData() { JoinedUser = joinedUser };
            this.roomContext.RoomUserDataList[ConnectionId] = roomUserData;

            if(this.roomContext.RoomUserDataList.Count >= roomContextRepos.MaxUsers)
            {
                this.roomContext.isStart = true;
                Console.WriteLine("参加者がいっぱいになりました");

                int BattleId = await battleService.RegistBattleAsync(StageId);

                this.roomContext.BattleId = BattleId;

                //ゲーム開始通知
                this.roomContext.Group.All.OnConfirmed(BattleId);
            }


            // 自分以外のルーム参加者全員に、ユーザーの入室通知を送信
            this.roomContext.Group.Except([this.ConnectionId]).OnJoin(joinedUser);

            // 入室リクエストをしたユーザーに、参加者の情報をリストで返す
            return this.roomContext.RoomUserDataList.Select(
                f => f.Value.JoinedUser).ToArray();
        }

        // 接続時の処理
        protected override ValueTask OnConnected()
        {
            roomContextRepos = roomContextRepository;
            return default;
        }
        // 切断時の処理
        protected override ValueTask OnDisconnected()
        {
            return default;
        }

        // 接続ID取得
        public Task<Guid> GetConnectionId()
        {
            return Task.FromResult<Guid>(this.ConnectionId);
        }


        public Task ReadyAsync()
        {

            //準備出来たことを自分のRoomUserDataに保存
            var roomUserData = this.roomContext.RoomUserDataList[this.ConnectionId];
            roomUserData.isReady = true;

            //全員準備できたか判定
            bool isReady = true;
            var roomUserDataList = this.roomContext.RoomUserDataList.Values.ToArray();
            foreach (var targetRoomUserData in roomUserDataList)
            {
                if(!targetRoomUserData.isReady)
                {
                    return Task.CompletedTask;
                }
            }

            Console.WriteLine("開始します");
            this.roomContext.Group.All.OnStart();
            return Task.CompletedTask;
        }

        public Task LeaveAsync()
        {
            //　退室したことを全メンバーに通知
            this.roomContext.Group.All.OnLeave(this.ConnectionId);

            //　ルーム内のメンバーから自分を削除
            this.roomContext.Group.Remove(this.ConnectionId);

            //　ルームデータから退室したユーザーを削除
            this.roomContext.RoomUserDataList.Remove(this.ConnectionId);
            if (roomContext.Group.Count() <= 0)
            {
                roomContextRepos.RemoveContext(this.roomContext.Name);
            }
            return Task.CompletedTask;
        }

        // 移動
        public Task MoveAsync(Vector3 pos,Quaternion rot)
        {
            // 位置情報を記録
            this.roomContext.RoomUserDataList[this.ConnectionId].pos = pos;

            // 移動情報を自分以外の全メンバーに通知
            this.roomContext.Group.Except([this.ConnectionId]).OnMove(this.ConnectionId, pos,rot);

            return Task.CompletedTask;
        }

        //チェックポイント通過
        public Task PassCheckAsync()
        { 
            // 位置情報を記録
            this.roomContext.RoomUserDataList[this.ConnectionId].checkCount++;

            // 移動情報を自分以外の全メンバーに通知
            this.roomContext.Group.Except([this.ConnectionId]).OnPassCheck(this.ConnectionId);

            return Task.CompletedTask;

        }

        //ゴール
        public async Task GoalAsync(int rank)
        {
            this.roomContext.RoomUserDataList[this.ConnectionId].checkCount++;

            // 移動情報を自分以外の全メンバーに通知
            this.roomContext.Group.Except([this.ConnectionId]).OnGoal(this.ConnectionId);


            await battleLogService.RegistBattleLogAsync(roomContext.BattleId, 
                this.roomContext.RoomUserDataList[this.ConnectionId].JoinedUser.UserData.Id,
                rank);

           
        }

        

    }


}

