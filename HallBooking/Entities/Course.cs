
namespace HallBooking.Entities
{
    public class Course
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string InstructorName { get; set; } = "";
        public string Description { get; set; } = "";
        public int Capacity { get; set; }
        public decimal PricePerParticipant { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<CourseEnrollment> Enrollments { get; set; } = [];
        public List<CourseSession> Sessions { get; set; } = [];
    }
}
