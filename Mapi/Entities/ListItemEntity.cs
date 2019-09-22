namespace Mapi
{
    public class ListItemEntity
    {
        public string Username { get; set; }
        public string Picture { get; set; }
        public string TypeOfAction { get; private set; }

        public ListItemEntity(string username, string picture, bool isFollower)
        {
            Username = username;
            if (!isFollower)
                TypeOfAction = "+";
            else
                TypeOfAction = "-";
            Picture = picture;
        }

        public override string ToString()
        {
            return Username;
        }
    }
}
