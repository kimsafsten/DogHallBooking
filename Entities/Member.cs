using System;
using System.Collections.Generic;
using System.Text;

namespace HallBooking.Entities
{
    public class Member
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public List<HallRental> HallRentals { get; set; } = [];
        public List<CourseEnrollment> CourseEnrollments { get; set; } = [];
        public List<Payment> Payments { get; set; } = [];

    }
}
