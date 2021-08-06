using System;
using System.Collections.Generic;

namespace Faforever.Qai.Core.Extensions
{
	public static class ListExtensions
	{
		private static readonly Random _rand = new();

		public static T Random<T>(this List<T> list)
			=> list[_rand.Next(0, list.Count)];
	}
}
