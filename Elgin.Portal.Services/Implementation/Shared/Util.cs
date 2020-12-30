using System;
using System.Collections.Generic;
using System.Text;

namespace Elgin.Portal.Services.Implementation.Shared
{
    public static class Util
    {
        public static DateTime PegaHoraBrasilia() => TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.FindSystemTimeZoneById("E. South America Standard Time"));
    }
}
