using System.Linq;

using Faforever.Qai.Core.Structures.ReplayReview;

using Newtonsoft.Json;

using NUnit.Framework;

namespace Faforever.Qai.Core.Tests.ReplayReview
{
    public class ReplayReviewPostFormatterTest
    {
        private static ReplayReviewRequest Request() => new()
        {
            PlayerId = 4242,
            Login = "Rhiza",
            ReplayId = 22334455,
            Map = "Setons Clutch",
            GameMode = "faf",
            Faction = "UEF",
            Rating = "1100",
            PlayedAt = "2026-09-05T19:12:00Z",
            Goal = "I want to know why my eco stalls at eight minutes.",
            Struggle = "I never have mass for the second land factory."
        };

        [Test]
        public void DeserializesTheLobbyServersWireFormat()
        {
            var request = JsonConvert.DeserializeObject<ReplayReviewRequest>(
                """
                {
                    "player_id": 4242,
                    "login": "Rhiza",
                    "replay_id": 22334455,
                    "map": "Setons Clutch",
                    "game_mode": "faf",
                    "faction": "UEF",
                    "rating": "1100",
                    "played_at": "2026-09-05T19:12:00Z",
                    "goal": "Why does my eco stall?",
                    "struggle": "",
                    "requested_at": "2026-09-06T10:00:00+00:00"
                }
                """);

            Assert.That(request, Is.Not.Null);
            Assert.Multiple(() =>
            {
                Assert.That(request!.PlayerId, Is.EqualTo(4242));
                Assert.That(request.Login, Is.EqualTo("Rhiza"));
                Assert.That(request.ReplayId, Is.EqualTo(22334455));
                Assert.That(request.GameMode, Is.EqualTo("faf"));
                Assert.That(request.IsValid, Is.True);
            });
        }

        [Test]
        public void TitleNamesThePlayerTheMapAndTheMode()
        {
            var post = ReplayReviewPostFormatter.Format(Request());

            Assert.That(post.Title, Is.EqualTo("Rhiza on Setons Clutch (faf)"));
        }

        [Test]
        public void TitleSurvivesMissingContext()
        {
            var request = Request();
            request.Map = null;
            request.GameMode = "";

            var post = ReplayReviewPostFormatter.Format(request);

            Assert.That(post.Title, Is.EqualTo("Rhiza"));
        }

        [Test]
        public void BodyLinksTheReplayByIdRatherThanByAnythingTheClientSent()
        {
            var post = ReplayReviewPostFormatter.Format(Request());

            Assert.That(
                post.Body,
                Does.Contain("https://replay.faforever.com/22334455"));
        }

        [Test]
        public void BodyNamesThePlayerTheLobbyAuthenticated()
        {
            var post = ReplayReviewPostFormatter.Format(Request());

            Assert.That(post.Body, Does.Contain("**Player:** Rhiza"));
        }

        [Test]
        public void BodyLeavesOutFieldsTheClientCouldNotFill()
        {
            var request = Request();
            request.Faction = null;
            request.Rating = "  ";
            request.Struggle = "";

            var post = ReplayReviewPostFormatter.Format(request);

            Assert.Multiple(() =>
            {
                Assert.That(post.Body, Does.Not.Contain("Faction"));
                Assert.That(post.Body, Does.Not.Contain("Rating"));
                Assert.That(post.Body, Does.Not.Contain("struggled with"));
                Assert.That(post.Body, Does.Contain("would like help with"));
            });
        }

        [Test]
        public void TitleStaysInsideDiscordsLimit()
        {
            var request = Request();
            request.Map = new string('m', 400);

            var post = ReplayReviewPostFormatter.Format(request);

            Assert.That(
                post.Title,
                Has.Length.LessThanOrEqualTo(ReplayReviewPostFormatter.MaxTitleLength));
        }

        [Test]
        public void BodyStaysInsideDiscordsLimit()
        {
            var request = Request();
            request.Goal = new string('g', 1000);
            request.Struggle = new string('s', 1000);

            var post = ReplayReviewPostFormatter.Format(request);

            Assert.That(
                post.Body,
                Has.Length.LessThanOrEqualTo(ReplayReviewPostFormatter.MaxBodyLength));
        }

        [TestCase(0, "Rhiza", 22334455, "help", TestName = "No player id")]
        [TestCase(4242, null, 22334455, "help", TestName = "No login")]
        [TestCase(4242, "Rhiza", 0, "help", TestName = "No replay")]
        [TestCase(4242, "Rhiza", 22334455, "   ", TestName = "No goal")]
        public void RequestsMissingTheEssentialsAreNotValid(
            int playerId, string login, int replayId, string goal)
        {
            var request = new ReplayReviewRequest
            {
                PlayerId = playerId,
                Login = login,
                ReplayId = replayId,
                Goal = goal
            };

            Assert.That(request.IsValid, Is.False);
        }

        [Test]
        public void PlayerWrittenTextIsShownAsWritten()
        {
            // The consumer does not interpret what a player typed; it is the
            // posting side that keeps it from pinging anyone.
            var request = Request();
            request.Goal = "@everyone please help";

            var post = ReplayReviewPostFormatter.Format(request);

            Assert.That(post.Body, Does.Contain("@everyone please help"));
            Assert.That(post.Body.Split('\n').Last(), Does.Contain("FAF client"));
        }
    }
}
