using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mapi
{
    public interface IUserDataInteractor
    {
        // Properties 
        List<UserData> Users { get; }

        // Methods
        List<UserData> SearchUsersByName(string name);
        bool SearchForCollision(string name);
        UserData FindUser(string name);
        Task UpdateAllUsers();
        Task<UserData> GetUserAsync();
        Task<UserData> InsertUserAsync(UserData user);
        Task ChangeString(string id, string property, string newImgName);
        Task ChangeArray(string id, string property, string[] array);
    }
}
