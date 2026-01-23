using bicycle_racing.Shared.Models.Entities;
using MagicOnion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bicycle_racing.Shared.Interfaces
{
    public interface IMailService: IService<IMailService>
    {
        //メール作成API
        UnaryResult<int> CreateMailAsync(int SendId,int ReceiveId,int Type,string content);
        //メールステータス更新API
        UnaryResult<Mail> ChangeStateMailAsync(int ID,int state);
        //メール取得API
        UnaryResult<Mail[]> GetMailAsync(int playerId);
        //メール削除API
        UnaryResult<bool> RemoveMailAsync(int ID);
    }
}
