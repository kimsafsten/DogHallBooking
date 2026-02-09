using HallBooking.Helpers;
using HallBooking.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HallBooking.UI.Admin.Course
{
    public class AdminCourseView
    {
        public static async Task Start(int courseId, CourseService courseService)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"== Hantera kurs (Id: {courseId}) ==");
                Console.WriteLine("1. Visa detaljer");
                Console.WriteLine("2. Lägg till kurstillfälle");
                Console.WriteLine("3. Ta bort kurstillfälle");
                Console.WriteLine("4. Ta bort en anmälan");
                Console.WriteLine("5. Ändra en kurs");
                Console.WriteLine("0. Tillbaka");
                Console.Write("Välj: ");

                var choice = ConsoleHelper.ReadInt();

                switch (choice)
                {
                    case 0:
                        return;

                    case 1:
                        await ShowCourseDetails(courseId, courseService);
                        break;

                    case 2:
                        await AddCourseSession(courseId, courseService);
                        break;

                    case 3:
                        await RemoveCourseSession(courseId, courseService);
                        break;

                    case 4:
                        await RemoveCourseEnrollment(courseId, courseService);
                        break;

                    case 5:
                        await UpdateCourse(courseId, courseService);
                        break;

                    default:
                        ConsoleHelper.Pause("Ogiltigt val");
                        break;
                }
            }
        }
        private static async Task ShowCourseDetails(int courseId, CourseService courseService)
        {
            Console.Clear();
            CourseService.CourseFullDetailsView course;
            try
            {
                course = await courseService.GetDetailedCourseById(courseId);
            }
            catch (Exception ex)
            {
                ConsoleHelper.Pause($"Kunde inte hämta kursdetaljer: {ex.Message}");
                return;
            }

            int enrollmentCountFromDb;
            try
            {
                enrollmentCountFromDb = await courseService.GetEnrollmentCountForCourseAsync(courseId);
            }
            catch (Exception ex)
            {
                ConsoleHelper.Pause($"Kunde inte beräkna antal anmälda: {ex.Message}");
                return;
            }

            Console.WriteLine($"== {course.Title} (Id: {course.Id}) ==");
            Console.WriteLine($"Instruktör: {course.InstructorName}");
            Console.WriteLine($"Pris per deltagare: {course.pricePerParticipant}");
            Console.WriteLine($"Platser: {course.Capacity}");
            Console.WriteLine($"Antal anmälda: {enrollmentCountFromDb}/{course.Capacity}");
            Console.WriteLine($"Beskrivning: {course.Description}");
            Console.WriteLine();

            Console.WriteLine("== Tillfällen ==");
            if (course.Sessions.Count == 0)
            {
                Console.WriteLine("Inga tillfällen planerade");
            }
            else
            {
                foreach (var s in course.Sessions)
                {
                    Console.WriteLine($"- SessionId: {s.Id} | {s.StartTime:yyyy-MM-dd HH:mm} - {s.EndTime:HH:mm}");
                }
            }
            Console.WriteLine();

            Console.WriteLine("== Deltagare ==");
            if (course.Participants.Count == 0)
            {
                Console.WriteLine("- Inga deltagare anmälda");
            }
            else
            {
                foreach (var p in course.Participants)
                {
                    Console.WriteLine($"- MemberId: {p.Id} | {p.Name} ({p.Email})");
                }
            }

            Console.WriteLine();
            ConsoleHelper.Pause();
        }
        private static async Task AddCourseSession(int courseId, CourseService courseService)
        {
            Console.Clear();
            Console.WriteLine("== Lägg till kurstillfälle ==");

            var day = ConsoleHelper.ReadDateOnly("Datum (YYYY-MM-DD): ");
            var startTime = ConsoleHelper.ReadTimeOnly("Starttid (HH:mm): ");
            var endTime = ConsoleHelper.ReadTimeOnly("Sluttid (HH:mm): ");

            var start = day.ToDateTime(startTime);
            var end = day.ToDateTime(endTime);

            try
            {
                var sessionId = await courseService.CreateSessionAsync(courseId, start, end);
                ConsoleHelper.Pause($"Tillfälle skapat! SessionId {sessionId}");
            }
            catch (Exception ex)
            {
                ConsoleHelper.Pause($"Kunde inte lägga till tillfälle: {ex.Message}");
            }
        }
        private static async Task RemoveCourseSession(int courseId, CourseService courseService)
        {
            Console.Clear();
            Console.WriteLine("== Ta bort kurstillfälle ==");

            Console.Write("Ange sessionsId att ta bort (0 = avbryt): ");
            var sessionId = ConsoleHelper.ReadInt();

            if (sessionId == 0)
                return;

            try
            {
                await courseService.RemoveSessionAsync(courseId, sessionId);
                ConsoleHelper.Pause("Kurstillfället är borttaget");
            }
            catch (Exception ex)
            {
                ConsoleHelper.Pause($"Kunde inte ta bort tillfället: {ex.Message}");
            }
        }
        private static async Task RemoveCourseEnrollment(int courseId, CourseService courseService)
        {
            Console.Clear();
            Console.WriteLine("== Ta bort en anmälan ==");

            Console.Write("Ange medlemsId att ta bort från kursen (0 = avbryt): ");
            var memberId = ConsoleHelper.ReadInt();

            if (memberId == 0)
                return;

            try
            {
                await courseService.RemoveEnrollmentAsync(courseId, memberId);
                ConsoleHelper.Pause("Anmälan är borttagen");
            }
            catch (Exception ex)
            {
                ConsoleHelper.Pause($"Kunde inte ta bort anmälan: {ex.Message}");
            }
        }
        private static async Task UpdateCourse(int courseId, CourseService courseService)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("== Ändra en kurs ==");

                CourseService.CourseWithSessionView course;
                try
                {
                    course = await courseService.GetCourseWithSessions(courseId);
                }
                catch (Exception ex)
                {
                    ConsoleHelper.Pause($"Kunde inte hämta kursen: {ex.Message}");
                    return;
                }

                Console.WriteLine($"Nuvarande titel      : {course.Title}");
                Console.WriteLine($"Nuvarande instruktör : {course.InstructorName}");
                Console.WriteLine($"Nuvarande beskrivning: {course.Description}");
                Console.WriteLine($"Nuvarande platser    : {course.Capacity}");
                Console.WriteLine($"Nuvarande pris       : {course.pricePerParticipant}");
                Console.WriteLine();
                Console.WriteLine("Tryck Enter för att behålla nuvarande värde.");
                Console.WriteLine();

                Console.Write("Ny titel: ");
                var titleInput = (Console.ReadLine() ?? "").Trim();
                var newTitle = string.IsNullOrWhiteSpace(titleInput) ? course.Title : titleInput;

                Console.Write("Ny instruktör: ");
                var instructorInput = (Console.ReadLine() ?? "").Trim();
                var newInstructor = string.IsNullOrWhiteSpace(instructorInput) ? course.InstructorName : instructorInput;

                Console.Write("Ny beskrivning: ");
                var descInput = (Console.ReadLine() ?? "").Trim();
                var newDescription = string.IsNullOrWhiteSpace(descInput) ? course.Description : descInput;

                int newCapacity = course.Capacity;
                while (true)
                {
                    Console.Write($"Nya platser [{course.Capacity}]: ");
                    var capInput = (Console.ReadLine() ?? "").Trim();

                    if (string.IsNullOrWhiteSpace(capInput))
                        break;

                    if (int.TryParse(capInput, out var cap) && cap > 0)
                    {
                        newCapacity = cap;
                        break;
                    }

                    Console.WriteLine("Skriv ett giltigt heltal större än 0, eller tryck Enter för att behålla.");
                }

                decimal newPrice = course.pricePerParticipant;
                while (true)
                {
                    Console.Write($"Nytt pris per deltagare [{course.pricePerParticipant}]: ");
                    var priceInput = (Console.ReadLine() ?? "").Trim();

                    if (string.IsNullOrWhiteSpace(priceInput))
                        break;

                    priceInput = priceInput.Replace(',', '.');

                    if (decimal.TryParse(
                        priceInput,
                        System.Globalization.NumberStyles.Number,
                        System.Globalization.CultureInfo.InvariantCulture,
                        out var price) && price > 0)
                    {
                        newPrice = price;
                        break;
                    }

                    Console.WriteLine("Skriv ett giltigt tal större än 0, eller tryck Enter för att behålla.");
                }

                try
                {
                    await courseService.UpdateCourseAsync(
                        courseId,
                        newTitle,
                        newInstructor,
                        newDescription,
                        newCapacity,
                        newPrice);

                    ConsoleHelper.Pause("Kursen är uppdaterad");
                    return;
                }
                catch (Exception ex)
                {
                    ConsoleHelper.Pause($"Kunde inte uppdatera kursen: {ex.Message}");
                }
            }
        }
    }
}
