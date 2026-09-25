using System;
using System.Text;

namespace Faforever.Qai.Core.Structures.ReplayReview
{
    /// <summary>
    /// The forum post a <see cref="ReplayReviewRequest"/> turns into.
    /// </summary>
    public readonly record struct ReplayReviewPost(string Title, string Body);

    /// <summary>
    /// Renders a review request into a forum post.
    /// </summary>
    /// <remarks>
    /// Rendering lives here rather than in the client that sent the request,
    /// which is the point of putting structured fields on the queue: how a
    /// review request reads can change without anyone shipping a client.
    /// </remarks>
    public static class ReplayReviewPostFormatter
    {
        /// <summary>Discord's limit on a forum post's name.</summary>
        public const int MaxTitleLength = 100;

        /// <summary>Discord's limit on a plain message's content.</summary>
        public const int MaxBodyLength = 2000;

        public const string ReplayUrlFormat = "https://replay.faforever.com/{0}";

        public static ReplayReviewPost Format(ReplayReviewRequest request)
        {
            return new ReplayReviewPost(FormatTitle(request), FormatBody(request));
        }

        private static string FormatTitle(ReplayReviewRequest request)
        {
            var title = new StringBuilder(request.Login);

            if (!string.IsNullOrWhiteSpace(request.Map))
                title.Append(" on ").Append(request.Map!.Trim());

            if (!string.IsNullOrWhiteSpace(request.GameMode))
                title.Append(" (").Append(request.GameMode!.Trim()).Append(')');

            return Truncate(title.ToString(), MaxTitleLength);
        }

        private static string FormatBody(ReplayReviewRequest request)
        {
            var body = new StringBuilder();

            // The link is built here from the id rather than taken from the
            // request. The request never carries a url, so nothing a player
            // types can decide where this post points.
            body.Append("**Replay:** ")
                .AppendFormat(ReplayUrlFormat, request.ReplayId)
                .Append('\n');

            AppendField(body, "Player", request.Login);
            AppendField(body, "Rating", request.Rating);
            AppendField(body, "Game mode", request.GameMode);
            AppendField(body, "Map", request.Map);
            AppendField(body, "Faction", request.Faction);
            AppendField(body, "Played", request.PlayedAt);

            body.Append("\n**What they would like help with**\n")
                .Append(request.Goal!.Trim())
                .Append('\n');

            if (!string.IsNullOrWhiteSpace(request.Struggle))
            {
                body.Append("\n**What they struggled with**\n")
                    .Append(request.Struggle!.Trim())
                    .Append('\n');
            }

            body.Append("\n*Requested from the FAF client. The player name above is the one the lobby server authenticated.*");

            return Truncate(body.ToString(), MaxBodyLength);
        }

        private static void AppendField(StringBuilder body, string label, string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return;

            body.Append("**").Append(label).Append(":** ").Append(value!.Trim()).Append('\n');
        }

        private static string Truncate(string value, int maxLength)
        {
            if (value.Length <= maxLength)
                return value;

            // One character of the budget goes to the ellipsis so the result
            // reads as cut off rather than as something the player wrote.
            return string.Concat(value.AsSpan(0, maxLength - 1).TrimEnd(), "…");
        }
    }
}
