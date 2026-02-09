using System;
using System.Collections.Generic;
using System.Text;

namespace HallBooking.Entities
{
    public class Payment
    {
        public int Id { get; set; }
        public int MemberId { get; set; }
        public Member Member { get; set; } = null!;
        public int? HallRentalId { get; set; }
        public HallRental? HallRental { get; set; }
        public int? CourseEnrollmentId { get; set; }
        public CourseEnrollment? CourseEnrollment { get; set; }
        public decimal Amount { get; set; }
        public PaymentStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
    public enum PaymentStatus
    {
        Pending = 0,
        Paid = 1,
        Failed = 2,
        Refunded = 3
    }
}
