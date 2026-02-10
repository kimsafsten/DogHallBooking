using HallBooking.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HallBooking.Services.BookingService;

namespace HallBooking.Helpers
{
    public static class BookingHelper
    {
        public static void ValidateHallRentalTime(DateTime startTime, DateTime endTime)
        {
            if (endTime <= startTime)
                throw new InvalidOperationException("EndTime must be after StartTime");

            bool startOnHour = startTime.Minute == 0 && startTime.Second == 0 && startTime.Millisecond == 0;
            bool endOnHour = endTime.Minute == 0 && endTime.Second == 0 && endTime.Millisecond == 0;

            if (!startOnHour || !endOnHour)
                throw new InvalidOperationException("Hall rentals must start and end on whole hour");

            if (TimeOnly.FromDateTime(startTime) < HallRules.OpenTime)
                throw new InvalidOperationException($"Hall opens at {HallRules.OpenTime:HH:mm}");

            if (TimeOnly.FromDateTime(endTime) > HallRules.CloseTime)
                throw new InvalidOperationException($"Hall closes at {HallRules.CloseTime:HH:mm}");

            var minutes = (endTime - startTime).TotalMinutes;
            if (minutes < 60 || minutes % 60 != 0)
                throw new InvalidOperationException("Hall rentals must be booked in full hours");
        }

        public static bool Overlaps(DateTime startA, DateTime endA, DateTime startB, DateTime endB)
           => startA < endB && endA > startB;

        public static bool OverlapsAny(DateTime start, DateTime end, IEnumerable<BlockedTimesView> blocked)
            => blocked.Any(b => Overlaps(start, end, b.StartTime, b.EndTime));
    }
}
