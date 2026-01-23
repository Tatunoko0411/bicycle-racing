using bicycle_racing.Server.Models.Contexts;
using bicycle_racing.Shared.Interfaces;
using bicycle_racing.Shared.Models.Entities;
using MagicOnion;
using MagicOnion.Server;
using Org.BouncyCastle.Asn1.X509;
using System.Xml.Linq;

namespace bicycle_racing.Server.Services
{
    public class FriendService : ServiceBase<IFriendService>, IFriendService
    {
        //フレンド登録API
        public async UnaryResult<int> RegistFriendAsync(int Id1, int Id2)
        {
            using var context = new GameDbContext();

            //テーブルにレコードを追加
            Friend friend = new Friend();

            friend.Player_id_01 = Id1;
            friend.Player_id_02 = Id2;
            friend.Created_at = DateTime.Now;
            friend.Updated_at = DateTime.Now;
          
            context.Friends.Add(friend);
            await context.SaveChangesAsync();
            return friend.Id;
        }

        //ユーザーID指定でフレンド取得するAPI
        public async UnaryResult<Friend[]> GetFriendAsync(int Id)
        {
            using var context = new GameDbContext();

            Friend[] friends = context.Friends.Where(friend => (friend.Player_id_02 == Id || friend.Player_id_01 == Id)).ToArray();
           
            return friends;
        }

        //id指定でユーザー情報を取得するAPI
        public async UnaryResult<Task> RemoveFriendAsync(int id)
        {
            using var context = new GameDbContext();

            Friend friend = context.Friends.Where(friend => (friend.Id == id)).First();

            context.Friends.Remove(friend);

            await context.SaveChangesAsync();
            return Task.CompletedTask;
        }
    }
}
