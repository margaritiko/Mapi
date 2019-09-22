using System;
using System.Threading.Tasks;

namespace Mapi
{
    public class DataInteractor: IDataInteractor
    {
        static Random random = new Random();
        // Work with Cosmos DB
        // Data methods
        /// <summary>
        /// Adds the new user.
        /// </summary>
        /// <returns>The user.</returns>
        /// <param name="item">Item.</param>
        async Task AddUser(UserData item)
        {
            await App.manager?.InsertUserAsync(item);
            Console.WriteLine ("005");
        }

        /// <summary>
        /// Adds new user with given name and unique tocken.
        /// </summary>
        /// <param name="name">Username.</param>
        public async Task AddUserToDBWithName(string name)
        {
            Console.WriteLine ("001");
            var randomImg = random.Next(0, 10) + 1;
            Console.WriteLine ("002");
            var todo = new UserData { Token = App.userId, Name = name, Img = $"{randomImg}.png" };
            Console.WriteLine ("003");
            await AddUser(todo);
            Console.WriteLine ("004");
        }

        // Get data methods
        /// <summary>
        /// Finds the following by given name.
        /// </summary>
        /// <returns>The following.</returns>
        /// <param name="name">Name to use in search.</param>
        public ListItemEntity FindFollowing(string name)
        {
            var user = App.manager?.FindUser(name);
            if (user == null)
                return null;
            return new ListItemEntity(user.Name, user.Img, true);
        }

        /// <summary>
        /// Finds the follower by given name.
        /// </summary>
        /// <returns>The follower.</returns>
        /// <param name="name">Name to use in search.</param>
        public ListItemEntity FindFollower(string name)
        {
            var user = App.manager?.FindUser(name);
            if (user == null)
                return null;

            var allFollowings = GetFollowings();
            if (allFollowings != null)
            {
                foreach (var person in allFollowings)
                    if (person == name)
                        return new ListItemEntity(user.Name, user.Img, true);
            }
            return new ListItemEntity(user.Name, user.Img, false);
        }

        /// <summary>
        /// Converts UserCosmosDB user to simple User.
        /// </summary>
        /// <returns>User instance.</returns>
        /// <param name="user">UserCosmosDB user.</param>
        public ListItemEntity ConvertUser(UserData user)
        {
            var allUsers = GetFollowings();
            if (allUsers != null)
            {
                foreach (var person in allUsers)
                    if (person == user.Name)
                        return new ListItemEntity(user.Name, user.Img, true);
            }
            return new ListItemEntity(user.Name, user.Img, false);
        }

        /// <summary>
        /// Gets main user.
        /// </summary>
        /// <returns>The user.</returns>
        async public Task GetUser()
        {
            App.mainUser = await App.manager?.GetUserAsync();
        }

        /// <summary>
        /// Gets the image of main user.
        /// </summary>
        /// <returns>The image.</returns>
        public string GetImage()
        {
            return App.mainUser.Img;
        }

        /// <summary>
        /// Gets all visited polygons of main user.
        /// </summary>
        /// <returns>The visited polygons.</returns>
        public string[] GetVisitedPolygons()
        {
            return App.mainUser.VisitedPolygons;
        }

        /// <summary>
        /// Gets all followers.
        /// </summary>
        /// <returns>The followers.</returns>
        public string[] GetFollowers()
        {
            return App.mainUser.Followers;
        }

        /// <summary>
        /// Gets all followings.
        /// </summary>
        /// <returns>The followings.</returns>
        public string[] GetFollowings()
        {
            return App.mainUser.Followings;
        }

        // Change data methods
        async public Task ChangeImageTo(string imgName)
        {
            await App.manager?.ChangeString(App.mainUser.Id, "img", imgName);
            await App.manager?.UpdateAllUsers();
        }

        async public Task AddNewPolygons(string[] newPolygons)
        {
            string[] currentPolygons = GetVisitedPolygons();
            string[] polygons = new string[currentPolygons.Length + newPolygons.Length];
            for (int i = 0; i < currentPolygons.Length; ++i)
                polygons[i] = currentPolygons[i];
            for (int i = 0; i < newPolygons.Length; ++i)
                polygons[currentPolygons.Length + i] = newPolygons[i];

            await App.manager?.ChangeArray(App.mainUser.Id, "visitedPolygons", polygons);
            await App.manager?.UpdateAllUsers();
        }

        public bool CheckPolygonForUser(string username, string polygonToCheck)
        {
            var user = App.manager?.FindUser(username);
            if (user == null)
                return false;
            string[] currentPolygons = user.VisitedPolygons;
            if (currentPolygons == null)
                return false;

            for (int i = 0; i < currentPolygons.Length; ++i)
                if (currentPolygons[i] == polygonToCheck)
                    return true;

            return false;
        }

        /// <summary>
        /// Checks for existing of given polygon.
        /// </summary>
        /// <returns><c>true</c>, if for existing of given polygon was checked, <c>false</c> otherwise.</returns>
        /// <param name="polygonToCheck">Polygon to check.</param>
        public bool CheckPolygon(string polygonToCheck)
        {
            string[] currentPolygons = GetVisitedPolygons();
            if (currentPolygons == null)
                return false;

            for (int i = 0; i < currentPolygons.Length; ++i)
                if (currentPolygons[i] == polygonToCheck)
                    return true;

            return false;
        }

        /// <summary>
        /// Adds the new polygon.
        /// </summary>
        /// <returns>The new polygon.</returns>
        /// <param name="newPolygon">New polygon.</param>
        async public Task AddNewPolygon(string newPolygon)
        {
            string[] currentPolygons = GetVisitedPolygons();
            string[] polygons;
            if (currentPolygons != null)
                polygons = new string[currentPolygons.Length + 1];
            else
                polygons = new string[1];

            if (currentPolygons != null)
            {
                for (int i = 0; i < currentPolygons.Length; ++i)
                    polygons[i] = currentPolygons[i];
                polygons[currentPolygons.Length] = newPolygon;
            }
            else
                polygons[0] = newPolygon;

            await App.manager?.ChangeArray(App.mainUser.Id, "visitedPolygons", polygons);
            await App.manager?.UpdateAllUsers();
        }

        async public Task AddFollower(string user, string follower)
        {
            var userToEdit = App.manager?.FindUser(user);
            string[] currentFollowers = userToEdit.Followers;
            string[] followers;
            if (currentFollowers != null)
                followers = new string[currentFollowers.Length + 1];
            else
                followers = new string[1];
            for (int i = 0; i < followers.Length - 1; ++i)
                followers[i] = currentFollowers[i];
            followers[followers.Length - 1] = follower;
            await App.manager?.ChangeArray(userToEdit.Id, "followers", followers);
            await App.manager?.UpdateAllUsers();
        }

        async public Task AddFollowing(string following)
        {
            string[] currentFollowings = GetFollowings();
            string[] followings;
            if (currentFollowings != null)
                followings = new string[currentFollowings.Length + 1];
            else
                followings = new string[1];
            for (int i = 0; i < followings.Length - 1; ++i)
                followings[i] = currentFollowings[i];
            followings[followings.Length - 1] = following;

            await App.manager?.ChangeArray(App.mainUser.Id, "followings", followings);
            await App.manager?.UpdateAllUsers();
        }

        async public Task DeleteFollowing(string following)
        {
            string[] currentFollowings = GetFollowings();
            if (currentFollowings == null)
                return;
            if (currentFollowings.Length == 0)
                return;
            string[] followings = new string[currentFollowings.Length - 1];
            for (int i = 0, j = 0; i < currentFollowings.Length; ++i)
                if (currentFollowings[i] != following)
                {
                    followings[j] = currentFollowings[i];
                    j++;
                }

            await App.manager?.ChangeArray(App.mainUser.Id, "followings", followings);
            await App.manager?.UpdateAllUsers();
        }

        async public Task DeleteFollower(string user, string follower)
        {
            var userToEdit = App.manager?.FindUser(user);
            string[] currentFollowers = userToEdit.Followers;
            if (currentFollowers == null)
                return;
            if (currentFollowers.Length == 0)
                return;

            string[] followers = new string[currentFollowers.Length - 1];
            for (int i = 0, j = 0; i < currentFollowers.Length; ++i)
                if (currentFollowers[i] != follower)
                {
                    followers[j] = currentFollowers[i];
                    j++;
                }

            await App.manager?.ChangeArray(userToEdit.Id, "followers", followers);
            await App.manager?.UpdateAllUsers();
        }
    }
}
