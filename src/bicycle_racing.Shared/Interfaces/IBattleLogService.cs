using System;
using MagicOnion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bicycle_racing.Shared.Models.Entities;

namespace bicycle_racing.Shared.Interfaces
{
    public interface IBattleLogService : IService<IBattleLogService>
    {

        //プレイヤーごとの対戦履歴を登録するAPI
        UnaryResult<Task> RegistBattleLogAsync(int ButtleId, int playerId, int rank);

        //id指定で対戦履歴を取得するAPI
        UnaryResult<BattleLog> GetBattleLogAsync(int id);
        //対戦履歴一覧を取得するAPI
        UnaryResult<BattleLog[]> GetAllBattleLogAsync();

        //id指定で対戦履歴を更新するAPI
        UnaryResult<BattleLog> UpdateBattleLogAsync(int id, int rank);
    }
}