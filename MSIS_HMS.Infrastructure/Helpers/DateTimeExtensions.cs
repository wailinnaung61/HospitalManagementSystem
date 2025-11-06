using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MSIS_HMS.Infrastructure.Helpers
{
    public static class DateTimeExtensions
    {
        public static string ToDateString(this DateTime date)
        {
            return date.ToString("yyyy-MM-dd");
        }

        public static string ToDateTimeString(this DateTime datetime)
        {
            return datetime.ToString("yyyy-MM-dd hh:mmtt");
        }

        public static string GetCurrentDateString()
        {
            return DateTime.Now.ToString("yyyy-MM-dd");
        }

        public static string GetCurrentDateTimeString()
        {
            return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss").Replace(' ', 'T');
        }

        
        public static SelectList GetDaysOfWeekSelectList(int? SelectedDay = null)
        {
            var daysOfWeek = Enum.GetValues(typeof(DayOfWeek))
                .OfType<DayOfWeek>()
                //.OrderBy(day => day < DayOfWeek.Monday)
                .Select(x => new SelectListItem(x.ToString(), ((int)x).ToString()));     
            var selectList = new SelectList(daysOfWeek, "Value", "Text", SelectedDay);
            return selectList;
        }
    }
}
