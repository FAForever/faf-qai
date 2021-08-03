using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Faforever.Qai.Core.Extensions
{
	public static class ListExtensions
	{
		private static readonly Random _rand = new();

		public static T Random<T>(this List<T> list)
			=> list[_rand.Next(0, list.Count)];
	}
}
