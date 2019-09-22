using Newtonsoft.Json;

namespace Mapi
{
    public class UserData
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "token")]
        public string Token { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "visitedPolygons")]
        public string[] VisitedPolygons { get; set; }

        [JsonProperty(PropertyName = "img")]
        public string Img { get; set; }

        [JsonProperty(PropertyName = "followers")]
        public string[] Followers { get; set; }

        [JsonProperty(PropertyName = "followings")]
        public string[] Followings { get; set; }
    }
}
