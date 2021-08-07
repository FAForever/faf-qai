using System;

namespace Faforever.Qai.Core.Extensions
{
    public static class DateTimeExtensions
    {
        public static string ToIso8601(this DateTime dt) => dt.ToString("yyyy-MM-ddTHH:mm:ssZ");
    }
}
