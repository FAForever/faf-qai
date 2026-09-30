using Newtonsoft.Json;

namespace Faforever.Qai.Core.Structures.ReplayReview
{
    /// <summary>
    /// A request from a player to have one of their replays reviewed.
    /// </summary>
    /// <remarks>
    /// This is the body the lobby server publishes with routing key
    /// <c>request.replay_review.create</c>. Only the lobby publishes it, and
    /// <see cref="PlayerId"/> and <see cref="Login"/> are stamped there from
    /// the player's authenticated connection - the requesting client never
    /// gets to say who it is. That is the entire reason the request travels
    /// this way instead of straight to Discord.
    ///
    /// Everything else is the player's own words and should be treated as
    /// such: it is displayed, never interpreted.
    /// </remarks>
    public class ReplayReviewRequest
    {
        [JsonProperty("player_id")]
        public int PlayerId { get; set; }

        [JsonProperty("login")]
        public string? Login { get; set; }

        [JsonProperty("replay_id")]
        public int ReplayId { get; set; }

        [JsonProperty("map")]
        public string? Map { get; set; }

        [JsonProperty("game_mode")]
        public string? GameMode { get; set; }

        [JsonProperty("faction")]
        public string? Faction { get; set; }

        [JsonProperty("rating")]
        public string? Rating { get; set; }

        [JsonProperty("played_at")]
        public string? PlayedAt { get; set; }

        [JsonProperty("goal")]
        public string? Goal { get; set; }

        [JsonProperty("struggle")]
        public string? Struggle { get; set; }

        [JsonProperty("requested_at")]
        public string? RequestedAt { get; set; }

        /// <summary>
        /// Whether this request has the four things a post cannot be made
        /// without: who asked, which replay, and what they want to know.
        /// </summary>
        /// <remarks>
        /// The lobby validates all of this before publishing. Checking again
        /// here is not distrust of the lobby but of the queue: a message can
        /// also arrive from a hand-run test or an older publisher, and the
        /// failure mode of not checking is a post with no author.
        /// </remarks>
        [JsonIgnore]
        public bool IsValid =>
            PlayerId > 0
            && ReplayId > 0
            && !string.IsNullOrWhiteSpace(Login)
            && !string.IsNullOrWhiteSpace(Goal);
    }
}
