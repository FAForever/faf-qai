using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Faforever.Qai.Core.Operations
{
	public static class ApiFormatter
	{
		public static string RemoveBadContent(this string input)
		{
			string output = input;
			var bindex = output.IndexOf("<LOC");
			if (bindex >= 0)
			{
				var start = input[..bindex];
				var end = output[(output.IndexOf(">") + 1)..];
				output = start + end;
			}

			return output;
		}
	}
}
