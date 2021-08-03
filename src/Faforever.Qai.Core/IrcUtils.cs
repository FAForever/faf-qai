using System.Collections.Generic;
using System.Text;

namespace Faforever.Qai.Core
{
	public static class IrcUtils
	{
		private static readonly IDictionary<char, char> quotedChars = new Dictionary<char, char>{
			{'\0', '0'},
			{'\n', 'n'},
			{'\r', 'r'}
		};

		private const char ctcpEscapeChar = '\x5C';
		private const char lowLevelEscapeChar = '\x10';
		private const char taggedDataDelimeterChar = '\x001';

		public static string ActionMessage(string text)
		{
			return taggedDataDelimeterChar + LowLevelQuote(CtcpQuote("ACTION " + text)) + taggedDataDelimeterChar;
		}

		private static string CtcpQuote(string value)
		{
			return Quote(value, ctcpEscapeChar);
		}

		private static string LowLevelQuote(string value)
		{
			return Quote(value, lowLevelEscapeChar);
		}

		private static string Quote(string value, char escapeChar)
		{
			var textBuilder = new StringBuilder(value.Length * 2);

			for (var i = 0; i < value.Length; i++)
			{
				if (quotedChars.TryGetValue(value[i], out var curQuotedChar) || value[i] == escapeChar)
				{
					textBuilder.Append(escapeChar);
					textBuilder.Append(curQuotedChar);
				}
				else
				{
					textBuilder.Append(value[i]);
				}
			}

			return textBuilder.ToString();
		}
	}

}