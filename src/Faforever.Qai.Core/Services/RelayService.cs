using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using Faforever.Qai.Core.Database;

using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace Faforever.Qai.Core.Services
{
	// Test Webhook URL
	// https://discord.com/api/webhooks/773214715328725012/Kj0P_CMUooloZTyfIOP37QD1GLuXp_LVRS3ax2FqxT5mP2dOvLoP_xMZiJ8L6Sf0jYgq

	public class RelayService
	{
		private readonly QAIDatabaseModel _database;


		public RelayService(QAIDatabaseModel database)
		{
			this._database = database;


		}

		public bool AddRelay(ulong discordGuild, ulong discordChannel, string ircChannel)
		{
			throw new NotImplementedException();
		}

		public bool RemoveRelat(ulong discordGuild, ulong discordChannel)
		{
			throw new NotImplementedException();
		}

		public bool SendFromDiscord(ulong discordChannel, string author, string message)
		{
			throw new NotImplementedException();
		}

		public bool SendFromIRC(string ircChannel, string author, string message)
		{
			throw new NotImplementedException();
		}
	}
}
