using System;

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

        public static long GetMapSize(this long i)
        {
            var b = (i / 5) / 10;

            var digits = Math.Floor(Math.Log10(b) + 1);
            var nearest = (int)Math.Pow(10, digits - 1);

            return (b + 5 * nearest / 10) / nearest * nearest;
        }

        public static string GetFaction(this int factionId)
            => factionId switch
            {
                1 => "U",
                2 => "A",
                3 => "C",
                4 => "S",
                _ => ""
            };
    }
}
