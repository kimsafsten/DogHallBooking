using HallBooking.Data;
using HallBooking.Entities;
using HallBooking.Enums;
using HallBooking.Helpers;
using Microsoft.EntityFrameworkCore;


namespace HallBooking.Services
{
    public partial class BookingService
    {
        
        
        public async Task<int> CreateHallRentalAsync(int memberId, DateTime startTime, DateTime endTime) 
        {
            using var context = new AppDbContext();
            BookingHelper.ValidateHallRentalTime(startTime, endTime);

            var memberExists = await context.Members
                .AsNoTracking()
                .AnyAsync(m => m.Id == memberId);

            if (!memberExists) throw new InvalidOperationException("Kunde inte hitta användare");

            // Explicit transaction to ensure hall rental and payment are created atomically
            await using var tx = await context.Database.BeginTransactionAsync();
            try
            {
                await EnsureNoHallRentalOverlapsAsync(startTime, endTime);
                await EnsureNoSessionOverlapsHallRentalAsync(startTime, endTime);

                var hours = (int)((endTime - startTime).TotalHours);
                var hourlyPrice = HallRules.HourlyPrice;
                var totalPrice = hours * hourlyPrice;

                var rental = new HallRental() 
                {
                    MemberId = memberId,
                    StartTime = startTime,
                    EndTime = endTime,
                    Status = HallRentalStatus.Booked,
                    HourlyPrice = hourlyPrice
                };

                context.HallRentals.Add(rental);
                await context.SaveChangesAsync();

                var payment = new Payment()
                {
                    MemberId = memberId,
                    Amount = totalPrice,
                    Status = PaymentStatus.Pending,
                    HallRentalId = rental.Id
                };

                context.Payments.Add(payment);
                await context.SaveChangesAsync();

                await tx.CommitAsync();
                return rental.Id;
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }

        }

        public async Task UpdateHallRentalAsync(int hallRentalId, DateTime newStartTime, DateTime newEndTime)
        {
            using var context = new AppDbContext();
            BookingHelper.ValidateHallRentalTime(newStartTime, newEndTime);
            
            var rental = await context.HallRentals
                .FirstOrDefaultAsync(r => r.Id == hallRentalId);

            if (rental == null)
                throw new InvalidOperationException("Kunde inte hitta bokning");

            if (rental.Status != HallRentalStatus.Booked)
                throw new InvalidOperationException("Endast bokade tider kan uppdateras");

            await EnsureNoHallRentalOverlapsAsync(newStartTime, newEndTime, ignoreHallRentalId: hallRentalId);

            await EnsureNoSessionOverlapsHallRentalAsync(newStartTime, newEndTime);

            var oldTotal = rental.TotalPrice;

            rental.StartTime = newStartTime;
            rental.EndTime = newEndTime;
            rental.HourlyPrice = HallRules.HourlyPrice;

            var newTotal = rental.TotalPrice;

            if (newTotal != oldTotal)
            {
                context.Payments.Add(new Payment
                {
                    MemberId = rental.MemberId,
                    Amount = newTotal,
                    Status = PaymentStatus.Pending,
                    HallRentalId = rental.Id
                });
            }

            await context.SaveChangesAsync();

        }
        // Soft-delete: keep record for history/audit, but it should no longer block time slots.
        public async Task CancelHallRentalAsync(int hallRentalId)
        {
            using var context = new AppDbContext();
            var rental = await context.HallRentals
                .Include(r => r.Payments)
                .FirstOrDefaultAsync(r => r.Id == hallRentalId);

            if (rental == null)
                throw new InvalidOperationException("Bokningen kunde inte hittas");

            if (rental.Status == HallRentalStatus.Cancelled)
                return;

            rental.Status = HallRentalStatus.Cancelled;

            var latestPayment = rental.Payments
                .OrderByDescending(r => r.CreatedAt)
                .FirstOrDefault();

            if (latestPayment != null)
            {
                if (latestPayment.Status == PaymentStatus.Paid)
                {
                    latestPayment.Status = PaymentStatus.Refunded;
                }
                else if (latestPayment.Status == PaymentStatus.Pending)
                {
                    latestPayment.Status = PaymentStatus.Failed;
                }
            }

            await context.SaveChangesAsync();
        }

        public async Task<List<BlockedTimesView>> GetBookedTimeBlockAsync(DateOnly day)
        {
            using var context = new AppDbContext();
            var from = day.ToDateTime(new TimeOnly(0, 0));
            var to = from.AddDays(1);

            var rentals = await context.HallRentals
                .AsNoTracking()
                .Include(r => r.Member)
                .Where(r => r.Status == HallRentalStatus.Booked)
                .Where(r => r.StartTime < to && r.EndTime > from)
                .OrderBy(r => r.StartTime)
                .ToListAsync();

            var rentalBlocks = rentals.Select(r => new BlockedTimesView(
                StartTime: r.StartTime,
                EndTime: r.EndTime,
                Type: TimeBlockType.HallRental,
                SourceId: r.Id,
                Label: $"Bokad: {r.Member.Name}"
                ));

            var sessions = await context.CourseSessions
                .AsNoTracking()
                .Include(s => s.Course)
                .Where(s => s.StartTime < to && s.EndTime > from)
                .OrderBy(s => s.StartTime)
                .ToListAsync();

            // Sessions are rounded to whole hours (block start/end) to match rental slot resolution.
            var sessionBlocks = sessions.Select(s =>
            {
                var blockStart = s.BlockStart;
                var blockEnd = s.BlockEnd;

                if (blockStart < from) blockStart = from;
                if (blockEnd > to) blockEnd = to;

                return new BlockedTimesView(
                    StartTime: blockStart,
                    EndTime: blockEnd,
                    Type: TimeBlockType.CourseSession,
                    SourceId: s.Id,
                    Label: $"Kurs: {s.Course.Title} ({s.StartTime:HH:mm}-{s.EndTime:HH:mm})"
                    );
            });

            return rentalBlocks
                .Concat(sessionBlocks)
                .OrderBy(s => s.StartTime)
                .ThenBy(s => s.EndTime)
                .ToList();
        }

        public async Task<List<AvailableTimesView>> GetAvailableTimeSlotsAsync(DateOnly day)
        {
            using var context = new AppDbContext();
            var open = day.ToDateTime(HallRules.OpenTime);
            var close = day.ToDateTime(HallRules.CloseTime);

            var blocked = await GetBookedTimeBlockAsync(day);

            var result = new List<AvailableTimesView>();

            var cursor = open;
            while (cursor.AddMinutes(HallRules.SlotMinutes) <= close)
            {
                var slotStart = cursor;
                var slotEnd = cursor.AddMinutes(HallRules.SlotMinutes);

                if (!BookingHelper.OverlapsAny(slotStart, slotEnd, blocked))
                    result.Add(new AvailableTimesView(slotStart, slotEnd));

                cursor = cursor.AddMinutes(HallRules.SlotMinutes);
            }
            return result;

        }

        public async Task<HallRentalDetailsView?> GetDetailsForHallRentalAsync(int hallRentalId)
        {
            using var context = new AppDbContext();
            var rental = await context.HallRentals
                .AsNoTracking()
                .Include(r => r.Member)
                .Include(r => r.Payments)
                .FirstOrDefaultAsync(r => r.Id == hallRentalId);

            if (rental == null)
                return null;

            var latestPayment = rental.Payments
                .OrderByDescending(p => p.CreatedAt)
                .FirstOrDefault();

            return new HallRentalDetailsView(
                Id: rental.Id,
                MemberId: rental.MemberId,
                MemberName: rental.Member.Name,
                StartTime: rental.StartTime,
                EndTime: rental.EndTime,
                Status: rental.Status,
                TotalPrice: rental.TotalPrice,
                CreatedAt: rental.CreatedAt,
                LatestPaymentStatus: latestPayment?.Status
                );
        }

        public async Task<List<HallRentalOverviewView>> GetUpcomingHallRentalsAsync()
        {
            using var context = new AppDbContext();
            var now = DateTime.Now;

            return await context.HallRentals
                .AsNoTracking()
                .Include(r => r.Member)
                .Where(r => r.Status == HallRentalStatus.Booked)
                .Where(r => r.EndTime > now)
                .OrderBy(r => r.StartTime)
                .Select(r => new HallRentalOverviewView(
                    r.Id,
                    r.MemberId,
                    r.Member.Name,
                    r.StartTime,
                    r.EndTime,
                    r.Status,
                    ((int)(r.EndTime - r.StartTime).TotalHours) * r.HourlyPrice
                    ))
                .ToListAsync();
        }
        public record BlockedTimesView(
            DateTime StartTime,
            DateTime EndTime,
            TimeBlockType Type,
            int SourceId,
            string Label
            );

        public record AvailableTimesView(
            DateTime StartTime,
            DateTime EndTime
            );

        public record HallRentalDetailsView(
            int Id,
            int MemberId,
            string MemberName,
            DateTime StartTime,
            DateTime EndTime,
            HallRentalStatus Status,
            decimal TotalPrice,
            DateTime CreatedAt,
            PaymentStatus? LatestPaymentStatus
            );

        public record HallRentalOverviewView(
            int Id,
            int MemberId,
            string MemberName,
            DateTime StartTime,
            DateTime EndTime,
            HallRentalStatus Status,
            decimal TotalPrice
            );

        

        private async Task EnsureNoHallRentalOverlapsAsync(DateTime startTime, DateTime endTime, int? ignoreHallRentalId = null)
        {
            using var context = new AppDbContext();
            var rentalsQuery = context.HallRentals
                .AsNoTracking()
                .Where(r => r.Status == HallRentalStatus.Booked);

            if (ignoreHallRentalId.HasValue)
                rentalsQuery = rentalsQuery.Where(r => r.Id != ignoreHallRentalId.Value);

            var overlaps = await rentalsQuery
                .AnyAsync(r => startTime < r.EndTime && endTime > r.StartTime);

            if (overlaps)
                throw new InvalidOperationException("Tiden krockar med en bokad tid");

        }

        private async Task EnsureNoSessionOverlapsHallRentalAsync(DateTime start, DateTime end)
        {
            using var context = new AppDbContext();
            var sessions = await context.CourseSessions
                .AsNoTracking()
                .Where(s => s.StartTime < end && s.EndTime > start)
                .ToListAsync();

            foreach (var s in sessions)
            {
                if (BookingHelper.Overlaps(start, end, s.BlockStart, s.BlockEnd))
                    throw new InvalidOperationException("Tiden krockar med ett kurstillfälle");
            }
        }
    }
}
