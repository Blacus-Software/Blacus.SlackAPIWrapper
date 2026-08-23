using System;

namespace Blacus.SlackAPIWrapper.Helpers
{

    public static class DateTimeHelpers
    {

        public static DateTime ToDateTimeFromUnixTimeStamp(this long unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            return DateTime.UnixEpoch.AddSeconds(unixTimeStamp).ToLocalTime();
        }

    }

}
