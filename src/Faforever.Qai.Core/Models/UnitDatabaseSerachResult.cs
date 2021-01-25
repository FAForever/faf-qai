using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DSharpPlus.Entities;

using Newtonsoft.Json;

namespace Faforever.Qai.Core.Models
{
	public class UnitDatabaseSerachResult
	{
		[JsonProperty("Id")]
		public string Id { get; set; }
		[JsonProperty("Description")]
		public string Description { get; set; }
		[JsonProperty("General")]
		public UnitGeneralData GeneralData { get; set; }
		[JsonProperty("StrategicIconName")]
		public string StrategicIconName { get; set; }

		public string GetStrategicIconUrl()
			=> UnitDbUtils.GetStrategicIconUrl(StrategicIconName);

		public string GetUnitImageUrl()
			=> UnitDbUtils.GetUnitImageUrl(Id);

		public string GetUnitDatabaseUrl()
			=> $"{UnitDbUtils.UnitPageBase}{Id}";

		public DiscordColor GetFactionColor()
			=> GeneralData.FactionName switch
				{
					"UEF" => UnitColors.EUF,
					"Cybran" => UnitColors.Cybran,
					"Aeon" => UnitColors.Aeon,
					"Seraphim" => UnitColors.Sera,
					_ => DiscordColor.Gray,
				};
	}

	public class UnitGeneralData
	{
		[JsonProperty("FactionName")]
		public string FactionName { get; set; }
		[JsonProperty("UnitName")]
		public string? UnitName { get; set; }
	}

	public static class UnitColors
	{
		public static DiscordColor Aeon { get; } = new(0x4caf50);
		public static DiscordColor EUF { get; } = new(0x2196f3);
		public static DiscordColor Cybran { get; } = new(0xf44336);
		public static DiscordColor Sera { get; } = new(0xffc107);
	}
}
