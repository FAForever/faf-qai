using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Faforever.Qai.Core
{
	public static class UnitDbUtils
	{
		public const string UnitDatabase = "https://unitdb.faforever.com";
		public static string UnitPageBase
		{
			get
			{
				return $"{UnitDatabase}/index.php?id="; 
			}
		}

		public static string UnitImages
		{
			get
			{
				return $"{UnitDatabase}/res/img";
			}
		}

		public static string GetUnitImageUrl(string unitId)
			=> $"{UnitImages}/preview/{unitId}.png";

		public static string GetStratIconUrl(string stratIconType)
			=> $"{UnitImages}/strategic/{stratIconType}.png";
	}
}
