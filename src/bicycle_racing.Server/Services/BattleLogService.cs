using MagicOnion.Server;
using MagicOnion;
using bicycle_racing.Shared.Interfaces;
using bicycle_racing.Shared.Models.Entities;
using bicycle_racing.Server.Models.Contexts;
using System.Xml.Linq;
using System.Security.Policy;

namespace realtime_game.Server.Services
{
    public class BattleLogService : ServiceBase<IBattleLogService>, IBattleLogService
    {



        //プレイヤーごとの対戦履歴を登録するAPI
        public async UnaryResult<Task> RegistBattleLogAsync(int ButtleId, int playerId, int rank)
        {
            using var context = new GameDbContext();

            //テーブルにレコードを追加
            BattleLog battleLog = new BattleLog();
            battleLog.Battle_id = ButtleId;
            battleLog.Player_id = playerId;
            battleLog.Rank = rank;
            battleLog.Created_at = DateTime.Now;
            battleLog.Updated_at = DateTime.Now;
            context.Battle_Logs.Add(battleLog);
            await context.SaveChangesAsync();
            return Task.CompletedTask;
        }

        //id指定で対戦履歴を取得するAPI
        public async UnaryResult<BattleLog> GetBattleLogAsync(int id)
        {
            using var context = new GameDbContext();

            BattleLog battle = context.Battle_Logs.Where(battle => battle.Id == id).First();


            return battle;
        }

        //対戦履歴一覧を取得するAPI
        public async UnaryResult<BattleLog[]> GetAllBattleLogAsync()
        {
            using var context = new GameDbContext();


            //テーブルにレコードを追加
            BattleLog[] user = context.Battle_Logs.ToArray();

            return user;
        }

        //id指定で対戦履歴を更新するAPI
        public async UnaryResult<BattleLog> UpdateBattleLogAsync(int id, int rank)
        {
            using var context = new GameDbContext();

            BattleLog battleLog = context.Battle_Logs.Where(battleLog => battleLog.Id == id).First();
            battleLog.Rank = rank;
            await context.SaveChangesAsync();



            return battleLog;
        }
    }
}
