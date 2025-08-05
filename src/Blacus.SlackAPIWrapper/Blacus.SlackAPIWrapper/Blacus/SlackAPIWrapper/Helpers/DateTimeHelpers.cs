using System;

namespace Blacus.SlackAPIWrapper.Helpers
{

    public static class DateTimeHelpers
    { 

        public static DateTime ToDateTimeFromUnixTimeStamp(this long unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dateTime;
        }

    }

}
