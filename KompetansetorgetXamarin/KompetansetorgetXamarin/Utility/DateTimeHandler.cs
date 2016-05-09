using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KompetansetorgetXamarin
{
    public class DateTimeHandler
    {

        //yyyyMMddHHmmss
        public static string MakeDateString(long input)
        {
            string uneditedString = input.ToString();
            string year = uneditedString.Substring(2, 2);
            string month = uneditedString.Substring(4, 2);
            string day = uneditedString.Substring(6, 2);            
            string dateString = day + "." + month + "." + year;
            return dateString;
        }

        public static string MakeDateTimeString(long input)
        {
            string uneditedString = input.ToString();
            string year = uneditedString.Substring(2, 2);
            string month = uneditedString.Substring(4, 2);
            string day = uneditedString.Substring(6, 2);
            string hour = uneditedString.Substring(8, 2);
            string minute = uneditedString.Substring(10, 2);

            string dateTimeString = day + "." + month + "." + year + " " + hour + ":" + minute;
            return dateTimeString;
            
        }

        /// <summary>
        /// Removes the milliseconds of a datetime, which is needed due to "lack" of precicion in SQLITE.
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DateTime TrimMilliseconds(DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second, 0);
        }
    }
}
