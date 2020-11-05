using System.Threading.Tasks;
using Faforever.Qai.Core.Models;
using Faforever.Qai.Core.Services;
using Moq;
using Xunit;

namespace Faforever.Qai.Core.Tests {
	public class SimpleCommandParserTest {
		private readonly SimpleCommandParser _parser;
		private readonly string _commandPrefix;
		private readonly Mock<ICommandSource> _commandSource;
		private readonly Mock<IPlayerService> _playerService;
		private readonly FetchPlayerStatsResult _validPlayerFetch;

		public SimpleCommandParserTest() {
			_validPlayerFetch = new FetchPlayerStatsResult {
				Name = "CoolMcGrrr",
				Ranking1v1 = 6000,
				Rating1v1 = 500,
				RankingGlobal = 19000,
				RatingGlobal = 1000
			};

			_commandSource = new Mock<ICommandSource>();
			_playerService = new Mock<IPlayerService>();

			_commandPrefix = "!";
			_parser = new SimpleCommandParser(_commandPrefix, _playerService.Object);
		}

		[Theory]
		[InlineData("Anyone ready for a game?")]
		[InlineData("ratings?! how do you find them?")]
		public async Task ShouldIgnoreMessage(string message) {
			//Arrange
			//Act
			await _parser.HandleMessage(_commandSource.Object, message);

			//Assert
			_commandSource.Verify(x => x.Respond(It.IsAny<string>()), Times.Never);
		}

		[Theory]
		[InlineData("player")]
		[InlineData("ratings")]
		[InlineData("RATINGS")]
		[InlineData("PLAYER")]
		public async Task ShouldRecognizeFetchPlayerAndFailOnNoUsername(string command) {
			//Arrange
			//Act
			await _parser.HandleMessage(_commandSource.Object, $"{_commandPrefix}{command}");

			//Assert
			_commandSource.Verify(x => x.Respond(It.IsAny<string>()), Times.AtLeastOnce);
			_playerService.Verify(x => x.FetchPlayerStats(It.IsAny<string>()), Times.Never);
		}

		[Theory]
		[InlineData("player")]
		[InlineData("ratings")]
		public async Task ShouldFetchPlayerStats(string command) {
			//Arrange
			const string username = "CoolMcGrrr";
			_playerService.Setup(x => x.FetchPlayerStats(username)).ReturnsAsync(_validPlayerFetch);
			//Act
			await _parser.HandleMessage(_commandSource.Object, $"{_commandPrefix}{command} {username}");

			//Assert
			_playerService.Verify(x => x.FetchPlayerStats(username), Times.Once);
		}
	}
}