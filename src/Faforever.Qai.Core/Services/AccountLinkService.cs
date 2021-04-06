using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Faforever.Qai.Core.Database;
using Faforever.Qai.Core.Structures;
using Faforever.Qai.Core.Structures.Link;

using Microsoft.EntityFrameworkCore;

namespace Faforever.Qai.Core.Services
{
	public class AccountLinkService
	{
		public enum LinkStatus
		{
			Ready,
			Waiting,
			Invalid
		}

		public ConcurrentDictionary<string, LinkState> LinkingController { get; init; }

		private readonly QAIDatabaseModel _database;

		public AccountLinkService(QAIDatabaseModel database)
		{
			_database = database;

			LinkingController = new();
		}

		public async Task<string> Start(ulong discordId, string discordUsername)
		{
			if (await _database.FindAsync<AccountLink>(discordId) is not null)
				throw new DiscordIdAlreadyMatchedException($"The ID {discordId} is already linked.");

			var token = Guid.NewGuid().ToString();

			LinkingController[token] = new (token, discordId, discordUsername, new Timer((x) =>
			{
				_ = LinkingController.TryRemove(token, out _);
			},
			null, TimeSpan.FromSeconds(30), Timeout.InfiniteTimeSpan));

			return token;
		}

		public bool VerifyDiscord(string token, ulong accountId)
		{
			if (LinkingController.TryGetValue(token, out var link))
			{
				return link.DiscordId == accountId;
			}
			else
				throw new TokenNotFoundException("An expired or invalid token was provided.");
		}

		public bool BindFafUser(string token, int fafId, string fafUsername)
		{
			if (_database.AccountLinks.AsNoTracking().FirstOrDefault(x => x.FafId == fafId) is not null)
				throw new FafIdAlreadyMatchedException($"The ID {fafId} is already linked.");

			if (LinkingController.TryGetValue(token, out var old))
			{
				LinkingController[token] = new(token, old.DiscordId, old.DiscordUsername,
					fafId, fafUsername, new Timer((x) =>
					{
						_ = LinkingController.TryRemove(token, out _);
					}, 
					null, TimeSpan.FromSeconds(30), Timeout.InfiniteTimeSpan));

				return true;
			}
			else
				throw new TokenNotFoundException("An expired or invalid token was provided.");
		}

		public LinkStatus GetLinkStatus(string token)
		{
			if (LinkingController.TryGetValue(token, out var link))
			{
				if (link.LinkReady)
					return LinkStatus.Ready;
				else return LinkStatus.Waiting;
			}

			return LinkStatus.Invalid;
		}

		public async Task<AccountLink> FinalizeLink(string token)
		{
			if (LinkingController.TryRemove(token, out var state))
			{
				var disp = state.ExparationTimer.DisposeAsync();

				var link = new AccountLink()
				{
					DiscordUsername = state.DiscordUsername,
					DisocrdId = state.DiscordId,
					FafUsername = state.FafUsername,
					FafId = state.FafId ?? throw new Exception("Failed to find a valid FAF ID to save.")
				};

				await _database.AddAsync(link);
				await _database.SaveChangesAsync();

				await disp;

				return link;
			}
			else
				throw new TokenNotFoundException("An expired or invalid token was provided.");
		}

		/// <summary>
		/// Thrown when a Discord User ID is attempted to be linked to a new FAF account after it has already been linked.
		/// </summary>
		public class DiscordIdAlreadyMatchedException : Exception
		{
			public DiscordIdAlreadyMatchedException() : base() { }
			public DiscordIdAlreadyMatchedException(string? message) : base(message) { }
			public DiscordIdAlreadyMatchedException(string? message, Exception? innerException) : base(message, innerException) { }
			public DiscordIdAlreadyMatchedException(SerializationInfo info, StreamingContext context) : base(info, context) { }
		}

		/// <summary>
		/// Thrown when a Faf User ID is attempted to be linked to a Discord account after it has already been linked.
		/// </summary>
		public class FafIdAlreadyMatchedException : Exception
		{
			public FafIdAlreadyMatchedException() : base() { }
			public FafIdAlreadyMatchedException(string? message) : base(message) { }
			public FafIdAlreadyMatchedException(string? message, Exception? innerException) : base(message, innerException) { }
			public FafIdAlreadyMatchedException(SerializationInfo info, StreamingContext context) : base(info, context) { }
		}

		/// <summary>
		/// Thrown when a bind is attempted to be made with a token that is no longer valid.
		/// </summary>
		public class TokenNotFoundException : Exception
		{
			public TokenNotFoundException() : base() { }
			public TokenNotFoundException(string? message) : base(message) { }
			public TokenNotFoundException(string? message, Exception? innerException) : base(message, innerException) { }
			public TokenNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context) { }
		}
	}
}
