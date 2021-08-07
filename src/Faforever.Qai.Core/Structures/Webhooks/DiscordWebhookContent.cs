
using Newtonsoft.Json;

namespace Faforever.Qai.Core.Structures.Webhooks
{
    /// <summary>
    /// Represents a partial implmenetation of the JSON data needed to send to a Discord Webhook.
    /// Does not include the payload_json, file, and allowed_mentions fields.
    /// </summary>
    public struct DiscordWebhookContent
    {
        /// <summary>
        /// Content for the message. 2000 char limit, required if not using DiscordEmbedsJson property.
        /// </summary>
        [JsonProperty("content")]
        public string? Content { get; set; }
        /// <summary>
        /// Overwrite the username of the webhook. Not Required.
        /// </summary>
        [JsonProperty("username")]
        public string? Username { get; set; }
        /// <summary>
        /// Overwrite the avatar of the webhook. Not Required.
        /// </summary>
        [JsonProperty("avatar_url")]
        public string? AvatarUrl { get; set; }
        /// <summary>
        /// Is the message Text to Speech?
        /// </summary>
        [JsonProperty("tts")]
        public bool? TextToSpeech { get; set; }
        /// <summary>
        /// See https://discord.com/developers/docs/resources/channel#embed-object for embed object structure. Supports 10 Embed objects in a JSON array.
        /// Not Required.
        /// </summary>
        [JsonProperty("embeds")]
        public string? DiscordEmbedsJson { get; set; }
    }
}
