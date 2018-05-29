using System;
using System.Collections.Generic;
using System.Text;
using LibraryData.Models;

namespace LibraryServices
{
    class DataHelpers
    {
        public static IEnumerable<string> HumanizeBizHours(IEnumerable<BranchHours> branchHourses)
        {
            var hours = new List<string>();

            foreach (var time in branchHourses)
            {
                var day = HumanizeDay(time.DayOfWeek);
                var openTime = HumanizeTime(time.OpenTime);
                var closeTime = HumanizeTime(time.CloseTime);

                var timeEntry = $"{day} {openTime} to {closeTime}";
                hours.Add(timeEntry);
            }

            return hours;
        }

        public static object HumanizeTime(int time)
        {
            return TimeSpan.FromHours(time).ToString("hh':'mm");
        }

        public static object HumanizeDay(int DayOfWeek)
        {
            //substract 1 because data starts with 1 and enum with 0 index
            return Enum.GetName(typeof(DayOfWeek), DayOfWeek - 1);
        }
    }
}
