using System;
using UnityEngine;

namespace FGUFW.Core
{
    static public class DateTimeHelper
    {
        static public long UnixMilliseconds(this DateTime dateTime)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return Convert.ToInt64((dateTime.ToUniversalTime() - epoch).TotalMilliseconds);
        }
    }
}