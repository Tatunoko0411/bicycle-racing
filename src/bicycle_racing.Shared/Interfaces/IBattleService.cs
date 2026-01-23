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
    public interface IBattleService : IService<IBattleService>
    {
        //対戦履歴を登録するAPI
        UnaryResult<int> RegistBattleAsync(int stageId);

        //id指定で対戦履歴を取得するAPI
        UnaryResult<Battle> GetBattleAsync(int id);

        //対戦履歴一覧を取得するAPI
        UnaryResult<Battle[]> GetAllBattleAsync();

    }
}