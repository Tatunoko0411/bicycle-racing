using bicycle_racing.Shared.Models.Entities;
using MagicOnion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bicycle_racing.Shared.Interfaces
{
    public interface IFriendService : IService<IFriendService>
    {
        //フレンド登録API
        UnaryResult<int> RegistFriendAsync(int Id1, int Id2);


        //ユーザーID指定でフレンド取得するAPI
        UnaryResult<Friend[]> GetFriendAsync(int Id);

        //id指定でユーザー情報を取得するAPI
        UnaryResult<Task> RemoveFriendAsync(int id);
    }
}
