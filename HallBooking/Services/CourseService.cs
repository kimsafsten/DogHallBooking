using HallBooking.Data;
using HallBooking.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text;
using static HallBooking.Services.BookingService;
using static System.Net.WebRequestMethods;

namespace HallBooking.Services
{
    public class CourseService
    {
        public async Task<int> CreateCourseAsync(string title, string instructorName, 
            string description, int capacity, decimal pricePerParticipant)
        {
            using var context = new AppDbContext();
            var course = new Course()
            {
                Title = title,
                InstructorName = instructorName,
                Description = description,
                Capacity = capacity,
                PricePerParticipant = pricePerParticipant
            };

            context.Courses.Add(course);
            await context.SaveChangesAsync();

            return course.Id;
        }

        public async Task UpdateCourseAsync(int courseId, string title, string instructorName, 
            string description, int capacity, decimal pricePerParticipant)
        {
            using var context = new AppDbContext();
            var course = await context.Courses
                .FirstOrDefaultAsync(c => c.Id == courseId);

            if (course == null)
                throw new InvalidOperationException("Kunde inte hitta kursen");

            course.Title = title;
            course.InstructorName = instructorName;
            course.Description = description;
            course.Capacity = capacity;
            course.PricePerParticipant = pricePerParticipant;

            await context.SaveChangesAsync();
        }
        
        public async Task RemoveCourseAsync(int courseId)
        {
            using var context = new AppDbContext();
            var course = await context.Courses
                .FirstOrDefaultAsync(c => c.Id == courseId);

            if (course == null)
                throw new InvalidOperationException("Kunde inte hitta kursen");

            context.Courses.Remove(course);
            await context.SaveChangesAsync();
        }
        
        public async Task EnrollMemberAsync(int courseId, int memberId)
        {
            using var context = new AppDbContext();
            var course = await context.Courses
                .Include(c => c.Enrollments)
                .FirstOrDefaultAsync(c => c.Id == courseId);

            if (course == null)
                throw new InvalidOperationException("Kunde inte hitta kursen");

            var memberExists = await context.Members.AsNoTracking().AnyAsync(m => m.Id == memberId);
                if (!memberExists) throw new InvalidOperationException("Användaren kunte inte hittas");

            var alreadyEnrolled = await context.CourseEnrollments
                .AsNoTracking()
                .AnyAsync(e => e.CourseId == courseId && e.MemberId == memberId);

            if (alreadyEnrolled) throw new InvalidOperationException("Användaren är redan anmäld");

            if (course.Enrollments.Count >= course.Capacity)
                throw new InvalidOperationException("Kursen är full");

            context.CourseEnrollments.Add(new CourseEnrollment
            {
                CourseId = courseId,
                MemberId = memberId,
            });

            await context.SaveChangesAsync();
        }

        public async Task RemoveEnrollmentAsync(int courseId, int memberId)
        {
            using var context = new AppDbContext();
            var enrollment = await context.CourseEnrollments
                .FirstOrDefaultAsync(e => e.CourseId == courseId && e.MemberId == memberId);

            if (enrollment == null)
                throw new InvalidOperationException("Kunde inte hitta någon anmälan");

            context.CourseEnrollments.Remove(enrollment);
            await context.SaveChangesAsync();
        }
        
        public async Task<int> GetEnrollmentCountForCourseAsync(int courseId)
        {
            using var context = new AppDbContext();
            return await context.CourseEnrollments
                .AsNoTracking()
                .CountAsync(e => e.CourseId == courseId);
        }

        public async Task<int> CreateSessionAsync(int courseId, DateTime start, DateTime end)
        {
            using var context = new AppDbContext();
            if (end <= start)
                throw new InvalidOperationException("Sluttiden måste vara efter starttiden");

            if (TimeOnly.FromDateTime(start) < HallRules.OpenTime)
                throw new InvalidOperationException($"Hallen öppnar {HallRules.OpenTime:HH:mm}");

            if (TimeOnly.FromDateTime(end) > HallRules.CloseTime)
                throw new InvalidOperationException($"Hallen stänger {HallRules.CloseTime:HH:mm}");

            var courseExists = await context.Courses
                .AsNoTracking()
                .AnyAsync(c => c.Id == courseId);

            if (!courseExists)
                throw new InvalidOperationException("Kunde inte hitta kursen");

            var newSession = new CourseSession()
            {
                CourseId = courseId,
                StartTime = start,
                EndTime = end
            };

            var overlapsAnySession = await context.CourseSessions
                .AsNoTracking()
                .AnyAsync(s => start < s.EndTime && end > s.StartTime);

            if (overlapsAnySession)
                throw new InvalidOperationException("Kurstillfället krockar med ett annat kurstillfälle");

            var overlapsHallRental = await context.HallRentals
                .AsNoTracking()
                .Where(r => r.Status == HallRentalStatus.Booked)
                .AnyAsync(r => start < r.EndTime && end > r.StartTime);

            if (overlapsHallRental)
                throw new InvalidOperationException("Kurstillfället krockar med en bokad tid");

            context.CourseSessions.Add(newSession);
            await context.SaveChangesAsync();

            return newSession.Id;
        }

        public async Task RemoveSessionAsync(int courseId, int sessionId)
        {
            using var context = new AppDbContext();
            var courseExists = await context.Courses
                .AsNoTracking()
                .AnyAsync(c => c.Id == courseId);

            if (!courseExists) 
                throw new InvalidOperationException("Kursen finns inte");

            var session = await context.CourseSessions
                .FirstOrDefaultAsync(c => c.Id == sessionId && c.CourseId == courseId);

            if (session == null)
                throw new InvalidOperationException("Kurstillfället finns inte");

            if (session.StartTime <= DateTime.Now)
                throw new InvalidOperationException("Kan inte ta bort ett tillfälle som påbörjats eller varit");

            context.CourseSessions.Remove(session);
            await context.SaveChangesAsync();
        }

        public async Task<List<CoursesOverviewView>> GetCoursesOverviewAsync()
        {
            using var context = new AppDbContext();
            return await context.Courses
                .AsNoTracking()
                .Select(c => new CoursesOverviewView(
                    c.Id,
                    c.Title,
                    c.Capacity,
                    context.CourseSessions.Count(s => s.CourseId == c.Id),
                    context.CourseEnrollments.Count(e => e.CourseId == c.Id)
                ))
                .ToListAsync();
        }

        public async Task<CourseWithSessionView> GetCourseWithSessions(int courseId) 
        {
            using var context = new AppDbContext();
            var result = await context.Courses
                .AsNoTracking()
                .Where(c => c.Id == courseId)
                .Select(c => new CourseWithSessionView(
                    c.Id,
                    c.Title,
                    c.InstructorName,
                    c.Description,
                    c.Capacity,
                    c.PricePerParticipant,
                    c.Sessions
                        .OrderBy(s => s.StartTime)
                        .Select(s => new CourseSessionView(
                            s.Id,
                            s.StartTime,
                            s.EndTime
                        ))
                        .ToList()
                 ))
                .FirstOrDefaultAsync();

            if (result == null)
                throw new InvalidOperationException("Kunde inte hitta kursen");

            return result;
        }

        public async Task<CourseFullDetailsView> GetDetailedCourseById(int courseId)
        {
            using var context = new AppDbContext();
            var result = await context.Courses
                .AsNoTracking()
                .Where(c => c.Id == courseId)
                .Select(c => new CourseFullDetailsView(
                    c.Id,
                    c.Title,
                    c.InstructorName,
                    c.Description,
                    c.Capacity,
                    c.PricePerParticipant,
                    c.Sessions
                        .OrderBy(s => s.StartTime)
                        .Select(s => new CourseSessionView(
                            s.Id,
                            s.StartTime,
                            s.EndTime
                        ))
                        .ToList(),
                    c.Enrollments
                        .OrderBy(e => e.Member.Name)
                        .Select(e => new CourseParticipantView(
                            e.MemberId,
                            e.Member.Name,
                            e.Member.Email
                        ))
                        .ToList()
                ))
                .FirstOrDefaultAsync();

            if (result == null)
                throw new InvalidOperationException("Kunde inte hitta kursen");

            return result;
        }

        public record CoursesOverviewView(
            int Id,
            string Title,
            int Capacity,
            int SessionCount,
            int EnrollmentCount
            );

        public record CourseWithSessionView(
            int id,
            string Title,
            string InstructorName,
            string Description,
            int Capacity,
            decimal pricePerParticipant,
            List<CourseSessionView> Sessions
            );

        public record CourseSessionView(
            int Id,
            DateTime StartTime,
            DateTime EndTime
            );
        
        public record CourseFullDetailsView(
            int Id,
            string Title,
            string InstructorName,
            string Description,
            int Capacity,
            decimal pricePerParticipant,
            List<CourseSessionView> Sessions,
            List<CourseParticipantView> Participants
            );

        public record CourseParticipantView(
            int Id,
            string Name,
            string Email
            );

       
    }
}
