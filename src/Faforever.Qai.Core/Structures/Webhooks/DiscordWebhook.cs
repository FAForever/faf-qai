using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Text;

namespace Faforever.Qai.Core.Structures.Webhooks
{
	public class DiscordWebhook : IEquatable<DiscordWebhook>
	{
		public ulong Id { get; set; }
		public string Token { get; set; }
		public ulong ChannelId { get; set; }

		public string WebhookUrl
		{
			get
			{
				return $"https://discordapp.com/api/webhooks/{Id}/{Token}";
			}
		}

		public DiscordWebhook()
			: this(0, "", 0) { }

		public DiscordWebhook(ulong id, string token, ulong channelId)
		{
			this.Id = id;
			this.Token = token;
			this.ChannelId = channelId;
		}

		public bool Equals([AllowNull] DiscordWebhook other)
		{
			return other?.Id.Equals(Id) ?? false;
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}

		public override bool Equals(object? obj)
		{
			if (obj is DiscordWebhook)
				return Equals(obj as DiscordWebhook);
			else return base.Equals(obj);
		}
	}
}
