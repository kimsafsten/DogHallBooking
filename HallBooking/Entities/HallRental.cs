 using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace HallBooking.Entities
{
    public class HallRental
    {
        public int Id { get; set; }
        public int MemberId { get; set; }
        public Member Member { get; set; } = null!;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public HallRentalStatus Status { get; set; }
        public decimal HourlyPrice { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<Payment> Payments { get; set; } = [];

        [NotMapped]
        // Calculated value; not stored in DB (price is calculated from time span and hourly rate)
        public decimal TotalPrice 
            => ((int)(EndTime- StartTime).TotalHours) * HourlyPrice;
    }
    public enum HallRentalStatus
    {
        Booked = 0,
        Cancelled = 1
    }
}
