using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Dynamic;
using System.Text;

namespace Faforever.Qai.Core.Structures.Webhooks
{
	public class DiscordWebhookData : IEquatable<DiscordWebhookData>
	{
		public ulong Id { get; set; }
		public string Token { get; set; }

		public string WebhookUrl
		{
			get
			{
				return $"https://discordapp.com/api/webhooks/{Id}/{Token}";
			}
		}

		public DiscordWebhookData()
			: this(0, "") { }

		public DiscordWebhookData(ulong id, string token)
		{
			this.Id = id;
			this.Token = token;
		}

		public bool Equals([AllowNull] DiscordWebhookData other)
		{
			return other?.Id.Equals(Id) ?? false;
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		}

		public override bool Equals(object? obj)
		{
			if (obj is DiscordWebhookData)
				return Equals(obj as DiscordWebhookData);
			else return base.Equals(obj);
		}
	}
}
