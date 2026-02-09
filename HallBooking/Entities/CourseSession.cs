using System;
using System.Collections.Generic;
using System.Text;

namespace HallBooking.Entities
{
    public class CourseSession
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public Course Course { get; set; } = null!;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public DateTime CreatedAt { get; set; }
        
        public DateTime BlockStart =>
            // Used for conflict checks: normalize to full-hour start
            new DateTime(StartTime.Year, StartTime.Month, StartTime.Day
                , StartTime.Hour, 0, 0, StartTime.Kind);

        public DateTime BlockEnd
        {
            get
            {
                // Used for conflict checks: round up to next hour unless already on the hour
                var endHour = new DateTime(EndTime.Year, EndTime.Month, EndTime.Day, EndTime.Hour, 0, 0, EndTime.Kind);
                bool onHour = EndTime.Minute == 0 && EndTime.Second == 0 && EndTime.Millisecond == 0;
                return onHour ? endHour : endHour.AddHours(1);
            }
        }
    }
}
