using Newtonsoft.Json;

namespace custom_auth.Auth
{
    public class ApiKey
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("ver")]
        public int Version { get; set; }
        [JsonProperty("rid")]
        public int ResourceId { get; set; }
        [JsonProperty("act")]
        public string Actions { get; set; }
        [JsonProperty("exp")]
        public long ExpirationDate { get; set; }
        [JsonProperty("sec")]
        public string Secret { get; set; }
    }
    public class ApiKeyDB
    {
        public string Name { get; set; }
        [JsonIgnore]
        public string Hash { get; set; }
        public ApiKeyState State { get; set; }
    }
    public enum ApiKeyState
    {
        Active,
        Inactive
    }
}
