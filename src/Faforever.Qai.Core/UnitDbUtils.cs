namespace Faforever.Qai.Core
{
    public static class UnitDbUtils
    {
        public const string UnitDatabase = "https://unitdb.faforever.com";
        public static string UnitApi
        {
            get
            {
                return $"{UnitDatabase}/api.php";
            }
        }
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

        public static string GetStrategicIconUrl(string stratIconType)
            => $"{UnitImages}/strategic/{stratIconType}_rest.png";
    }
}
