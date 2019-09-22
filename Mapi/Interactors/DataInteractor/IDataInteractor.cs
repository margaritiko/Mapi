using System;
using System.Threading.Tasks;
namespace Mapi
{
    public interface IDataInteractor
    {
        Task AddUserToDBWithName(string name);
        ListItemEntity FindFollowing(string name);
        ListItemEntity FindFollower(string name);
        ListItemEntity ConvertUser(UserData user);
        Task GetUser();
        string GetImage();
        string[] GetVisitedPolygons();
        string[] GetFollowers();
        string[] GetFollowings();
        Task ChangeImageTo(string imgName);
        Task AddNewPolygons(string[] newPolygons);
        bool CheckPolygonForUser(string username, string polygonToCheck);
        bool CheckPolygon(string polygonToCheck);
        Task AddNewPolygon(string newPolygon);
        Task AddFollower(string user, string follower);
        Task AddFollowing(string following);
        Task DeleteFollowing(string following);
        Task DeleteFollower(string user, string follower);
    }
}
