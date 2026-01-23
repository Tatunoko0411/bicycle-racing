using MagicOnion.Server;
using bicycle_racing.Shared.Interfaces;
using bicycle_racing.Shared.Models.Entities;
using bicycle_racing.Server.Models.Contexts;
using MagicOnion;

namespace bicycle_racing.Server.Services
{
    public class MailService : ServiceBase<IMailService>, IMailService
    {

        //メール作成API
       public async UnaryResult<int> CreateMailAsync(int SendId, int ReceiveId, int Type, string content)
        {
            using var context = new GameDbContext();

            //テーブルにレコードを追加
            Mail mail = new Mail();

            mail.send_player_id = SendId;
            mail.Receive_player_id = ReceiveId;
            mail.Type = Type;
            mail.Content = content;
            mail.state = 0;
            mail.Created_at = DateTime.Now;
            mail.Updated_at = DateTime.Now;
            context.Mails.Add(mail);
            await context.SaveChangesAsync();

            return mail.Id;
        }
        //メールステータス更新API
        public async UnaryResult<Mail> ChangeStateMailAsync(int ID, int state)
        {
            using var context = new GameDbContext();

            Mail mail = context.Mails.Where(mail => mail.Id == ID).First();
            mail.state = state;
            await context.SaveChangesAsync();

            return mail;
        }

        //メール取得API
        public async UnaryResult<Mail[]> GetMailAsync(int playerId)
        {
            using var context = new GameDbContext();

            Mail[] mails = context.Mails.Where(mail => mail.Receive_player_id == playerId).ToArray();


            return mails;
        }
        //メール削除API
        public async UnaryResult<bool> RemoveMailAsync(int ID)
        {
            using var context = new GameDbContext();

            Mail mail = context.Mails.Where(mail => (mail.Id == ID)).First();

            context.Mails.Remove(mail);

            await context.SaveChangesAsync();
            return true;
        }
    }
}
