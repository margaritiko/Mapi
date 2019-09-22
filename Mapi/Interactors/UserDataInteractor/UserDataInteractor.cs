using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Azure.Documents;

namespace Mapi
{
    public partial class UserDataInteractor: IUserDataInteractor
    {
        static UserDataInteractor defaultInstance = new UserDataInteractor();

        private Uri collectionLink = UriFactory.CreateDocumentCollectionUri(Constants.databaseId, Constants.collectionId);

        private DocumentClient client;

        private UserDataInteractor()
        {
            client = new DocumentClient(new System.Uri(Constants.accountURL), Constants.accountKey);
        }

        public static UserDataInteractor DefaultManager
        {
            get
            {
                return defaultInstance;
            }
        }

        /// <summary>
        /// All users in CDB.
        /// </summary>
        /// <value>All users.</value>
        public List<UserData> Users { get; private set; }

        public List<UserData> SearchUsersByName(string name)
        {
            List<UserData> searchResult = new List<UserData>();
            foreach (var user in Users)
                if (user.Name.Contains(name) && user.Name != App.mainUser.Name)
                    searchResult.Add(user);

            return searchResult;
        }

        /// <summary>
        /// Searchs for collision.
        /// </summary>
        /// <returns><c>true</c>, if for collision was searched, <c>false</c> otherwise.</returns>
        /// <param name="name">Name.</param>
        public bool SearchForCollision(string name)
        {
            foreach (var user in Users)
                if (user.Name == name)
                    return true;

            return false;
        }

        /// <summary>
        /// Finds the user.
        /// </summary>
        /// <returns>The user.</returns>
        /// <param name="name">Name.</param>
        public UserData FindUser(string name)
        {
            List<UserData> searchResult = new List<UserData>();
            foreach (var user in Users)
                if (user.Name == name)
                    return user;

            return null;
        }

        /// <summary>
        /// Updates all users.
        /// </summary>
        /// <returns>All users.</returns>
        public async Task UpdateAllUsers()
        {
            try
            {
                var query = client.CreateDocumentQuery<UserData>(collectionLink, new FeedOptions { MaxItemCount = -1 }).AsDocumentQuery();

                Users = new List<UserData>();
                while (query.HasMoreResults)
                {
                    Users.AddRange(await query.ExecuteNextAsync<UserData>());
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(@"ERROR {0}", e.Message);
            }
        }

        /// <summary>
        /// Gets current user.
        /// </summary>
        /// <returns>The user if exists.</returns>
        public async Task<UserData> GetUserAsync()
        {
            try
            {
                var query = client.CreateDocumentQuery<UserData>(collectionLink, new FeedOptions { MaxItemCount = -1 }).AsDocumentQuery();

                Users = new List<UserData>();
                while (query.HasMoreResults)
                {
                    Users.AddRange(await query.ExecuteNextAsync<UserData>());
                }

                foreach (var user in Users)
                    if (user.Token == App.userId)
                        return user;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(@"ERROR {0}", e.Message);
                return null;
            }
            return null;
        }

        /// <summary>
        /// Inserts new user with given data.
        /// </summary>
        /// <returns>New user.</returns>
        /// <param name="user">User.</param>
        public async Task<UserData> InsertUserAsync(UserData user)
        {
            try
            {
                var result = await client.CreateDocumentAsync(collectionLink, user);
                user.Id = result.Resource.Id;
                Users.Add(user);
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(@"ERROR {0}", e.Message);
            }
            return user;
        }

        /// <summary>
        /// All changes with user here.
        /// </summary>
        /// <returns>The item async.</returns>
        /// <param name="id">Identifier.</param>
        public async Task ChangeString(string id, string property, string newImgName)
        {
            try
            {
                List<Document> users = client.CreateDocumentQuery<Document>(collectionLink).ToList();
                Document user = null;
                for (int i = 0; i < users.Count; ++i)
                    if (users[i].Id == id)
                    {
                        user = users[i];
                        break;
                    }

                // Update some properties on the found resource
                user.SetPropertyValue(property, newImgName);

                // Now persist these changes to the database by replacing the original resource
                Document updated = await client.ReplaceDocumentAsync(user);

            }
            catch (Exception e)
            {
                Console.Error.WriteLine(@"ERROR {0}", e.Message);
            }
        }

        public async Task ChangeArray(string id, string property, string[] array)
        {
            try
            {
                List<Document> users = client.CreateDocumentQuery<Document>(collectionLink).ToList();
                Document user = null;
                for (int i = 0; i < users.Count; ++i)
                    if (users[i].Id == id)
                    {
                        user = users[i];
                        break;
                    }

                // Update some properties on the found resource
                user.SetPropertyValue(property, array);

                // Now persist these changes to the database by replacing the original resource
                Document updated = await client.ReplaceDocumentAsync(user);

            }
            catch (Exception e)
            {
                Console.Error.WriteLine(@"ERROR {0}", e.Message);
            }
        }
    }
}
