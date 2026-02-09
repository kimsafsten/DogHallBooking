using HallBooking.Data;
using HallBooking.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HallBooking.Services.BookingService;

namespace HallBooking.Services
{
    public class PaymentService
    {
        
        public async Task UpdatePaymentStatusAsync(int paymentId, PaymentStatus newStatus)
        {
            using var context = new AppDbContext();
            var payment = await context.Payments.FirstOrDefaultAsync(p => p.Id == paymentId);

            if (payment == null)
                throw new InvalidOperationException("Betalningen hittades inte");

            payment.Status = newStatus;
            await context.SaveChangesAsync();
        }

        public async Task<List<PaymentListView>> GetPaymentsForHallRentalAsync(int hallRentalId)
        {
            using var context = new AppDbContext();
            return await context.Payments
                .AsNoTracking()
                .Include(p => p.Member)
                .Where(p => p.HallRentalId == hallRentalId)
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new PaymentListView(
                    p.Id,
                    p.MemberId,
                    p.Member.Name,
                    p.Amount,
                    p.Status,
                    p.CreatedAt,
                    p.HallRentalId,
                    p.CourseEnrollmentId
                    ))
                .ToListAsync();
        }

        public async Task<List<PaymentListView>> GetPaymentsByStatusAsync(PaymentStatus status)
        {
            using var context = new AppDbContext();
            return await context.Payments
                .AsNoTracking()
                .Include(p => p.Member)
                .Where(p => p.Status == status)
                .OrderByDescending(p => p.CreatedAt)
                .Select(p => new PaymentListView(
                    p.Id,
                    p.MemberId,
                    p.Member.Name,
                    p.Amount,
                    p.Status,
                    p.CreatedAt,
                    p.HallRentalId,
                    p.CourseEnrollmentId
                    ))
                .ToListAsync();

        }

        public record PaymentListView(
          int Id,
          int MemberId,
          string MemberName,
          decimal Amount,
          PaymentStatus Status,
          DateTime CreatedAt,
          int? HallRentalId,
          int? CourseEnrollmentId
          );

    }
}
