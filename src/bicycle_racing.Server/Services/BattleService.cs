using MagicOnion.Server;
using MagicOnion;
using bicycle_racing.Shared.Interfaces;
using bicycle_racing.Shared.Models.Entities;
using bicycle_racing.Server.Models.Contexts;
using System.Xml.Linq;
using System.Security.Policy;

namespace realtime_game.Server.Services
{
    public class BattleService : ServiceBase<IBattleService>, IBattleService
    {

        //対戦履歴を登録するAPI
        public async UnaryResult<int> RegistBattleAsync(int stageId)
        {
            using var context = new GameDbContext();
         
            //テーブルにレコードを追加
            Battle battle = new Battle();
            battle.stage_id = stageId;
            battle.Created_at = DateTime.Now;
            battle.Updated_at = DateTime.Now;
            context.Battles.Add(battle);
            await context.SaveChangesAsync();
            return battle.Id;
        }



        //id指定で対戦履歴を取得するAPI
        public async UnaryResult<Battle> GetBattleAsync(int id)
        {
            using var context = new GameDbContext();

            Battle battle = context.Battles.Where(battle => battle.Id == id).First();


            return battle;
        }

        //対戦履歴一覧を取得するAPI
        public async UnaryResult<Battle[]> GetAllBattleAsync()
        {
            using var context = new GameDbContext();
        

            //テーブルにレコードを追加
            Battle[] user = context.Battles.ToArray();

            return user;
        }

    }
}
