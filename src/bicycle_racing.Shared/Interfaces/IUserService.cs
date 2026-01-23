using MagicOnion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bicycle_racing.Shared.Models.Entities;

namespace bicycle_racing.Shared.Interfaces
{
    public interface IUserService:IService<IUserService>
    {
        //ユーザーを登録するAPI
        UnaryResult<int> RegistUserAsync(string name);

        //id指定でユーザー情報を取得するAPI
        UnaryResult<User> GetUserAsyncWithID(int id);
        UnaryResult<User> GetUserAsyncWithName(string name);

        //ユーザー一覧を取得するAPI
        UnaryResult<User[]> GetAllUserAsync();

        //id指定でユーザー名を更新するAPI
        UnaryResult<User> UpdateUserAsync(int id,string name);

        //id指定でユーザーのプレイ回数等を更新するAPI
        UnaryResult<User> UpdateUserCountAsync(int id, int play, int win);

        //id指定でユーザーのプレイ状況を更新するAPI
        UnaryResult<User> ChangeStateUserAsync(int id,int state);

        //id指定でユーザーの参加しているルーム名を更新するAPI
        UnaryResult<User> SetRoomNameUserAsync(int id,int StageId,string name);
    }
}
