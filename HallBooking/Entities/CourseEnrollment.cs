

namespace HallBooking.Entities
{
    public class CourseEnrollment
    {
        public int Id { get; set; }
        public int MemberId { get; set; }
        public Member Member { get; set; } = null!;
        public int CourseId { get; set; }
        public Course Course { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public List<Payment> Payments { get; set; } = [];
    }
}
