using MagicOnion.Server;
using MagicOnion;
using bicycle_racing.Shared.Interfaces;
using bicycle_racing.Shared.Models.Entities;
using bicycle_racing.Server.Models.Contexts;
using System.Xml.Linq;
using System.Security.Policy;

namespace realtime_game.Server.Services
{
    public class UserService : ServiceBase<IUserService>, IUserService
    {

        public async UnaryResult<int> RegistUserAsync(string name)
        {
            using var context = new GameDbContext();
            //バリデーションチェック(名前登録済みかどうか)
            if (context.Users.Count() > 0 &&
                  context.Users.Where(user => user.Name == name).Count() > 0)
            {
                throw new ReturnStatusException(Grpc.Core.StatusCode.InvalidArgument, "");
            }

            //テーブルにレコードを追加
            User user = new User();
            user.Name = name;
            int rnd = new Random().Next(100000, 999999);
            user.Token = rnd.ToString();
            user.Created_at = DateTime.Now;
            user.Updated_at = DateTime.Now;
            context.Users.Add(user);
            await context.SaveChangesAsync();
            return user.Id;
        }

        public async UnaryResult<User> GetUserAsync(int id)
        {

            using var context = new GameDbContext();

            User user = context.Users.Where(user => user.Id == id).First();


            return user;
        }

        public async UnaryResult<User[]> GetAllUserAsync()
        {
            
            using var context = new GameDbContext();

            User[] users = context.Users.ToArray();


            return users;
        }

        public async UnaryResult<User> UpdateUserAsync(int id, string name)
        {
            using var context = new GameDbContext();
            User user = context.Users.Where(user => user.Id == id).First();
            user.Name = name;
            await context.SaveChangesAsync();



            return user;
        }

     
    }

}
