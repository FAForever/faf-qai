using System.Text.Json.Serialization;

namespace Faforever.Qai.Core.Structures.Link
{
    [method: JsonConstructor]
    public class FafUser(FafUserData data)
    {
        [JsonPropertyName("data")]
        public FafUserData Data { get; internal set; } = data;
    }

    [method: JsonConstructor]
    public class FafUserData(FafUserAttributes attributes)
    {
        [JsonPropertyName("attributes")]
        public FafUserAttributes Attributes { get; internal set; } = attributes;
    }

    [method: JsonConstructor]
    public class FafUserAttributes(int userId, string username)
    {
        [JsonPropertyName("userId")]
        public int UserId { get; internal set; } = userId;
        [JsonPropertyName("userName")]
        public string UserName { get; internal set; } = username;
    }
}
