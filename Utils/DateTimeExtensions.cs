using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuartzCryptoTools.Utils
{
    internal class DateTimeExtensions
    {
        // 参考: https://stackoverflow.com/questions/11895382/net-datetime-to-dos-date-32-bit-conversion
        public static DateTime ToDateTime(int dosDateTime)
        {
            var date = (dosDateTime & 0xFFFF0000) >> 16;
            var time = (dosDateTime & 0x0000FFFF);

            var year = (date >> 9) + 1980;
            var month = (date & 0x01e0) >> 5;
            var day = date & 0x1F;
            var hour = time >> 11;
            var minute = (time & 0x07e0) >> 5;
            var second = (time & 0x1F) * 2;

            return new DateTime((int)year, (int)month, (int)day, (int)hour, (int)minute, (int)second);
        }

        public static int ToDOSDate(DateTime dateTime)
        {
            var years = dateTime.Year - 1980;
            var months = dateTime.Month;
            var days = dateTime.Day;
            var hours = dateTime.Hour;
            var minutes = dateTime.Minute;
            var seconds = dateTime.Second;

            var date = (years << 9) | (months << 5) | days;
            var time = (hours << 11) | (minutes << 5) | (seconds >> 1);

            return (date << 16) | time;
        }
    }
}
