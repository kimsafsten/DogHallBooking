using HallBooking.Helpers;
using HallBooking.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HallBooking.UI.Member
{
    public class MemberCoursesView
    {
        public static async Task Start(int memberId, CourseService courseService)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("== Kurser ==");

                List<CourseService.CoursesOverviewView> courses;

                try
                {
                    courses = await courseService.GetCoursesOverviewAsync();
                }
                catch (Exception ex)
                {
                    ConsoleHelper.Pause($"Det gick inte att hämta kurser: {ex.Message}");
                    return;
                }

                if (courses.Count == 0)
                {
                    ConsoleHelper.Pause("Det finns inga kurser för tillfället");
                    return;
                }

                var displayNumber = 1;
                var activeCourses = new List<CourseService.CoursesOverviewView>();

                foreach (var c in courses)
                {
                    if (c.SessionCount <= 0)
                        continue;

                    if (c.EnrollmentCount >= c.Capacity)
                        continue;

                    activeCourses.Add(c);

                    var spotsLeft = c.Capacity - c.EnrollmentCount;

                    Console.WriteLine($"{displayNumber}. {c.Title} (Platser lediga: {spotsLeft}/{c.Capacity}, Tillfällen: {c.SessionCount})");

                    displayNumber++;
                }

                if (activeCourses.Count == 0)
                {
                    ConsoleHelper.Pause("Det finns inga aktiva kurser för tillfället");
                    return;
                }

                Console.WriteLine("0. Tillbaka till medlemssidan");
                Console.Write("Välj en kurs (nummer): ");

                var choice = ConsoleHelper.ReadInt();

                if (choice == 0)
                    return;

                if (choice < 1 || choice > activeCourses.Count)
                {
                    ConsoleHelper.Pause("Ogiltigt val");
                    continue; ;
                }

                var selectedCourse = activeCourses[choice - 1];
                var courseId = selectedCourse.Id;

                await GetCourseWithSession(courseId, memberId, courseService);
            }
        }

        private static async Task GetCourseWithSession(int courseId, int memberId, CourseService courseService)
        {
            while (true)
            {
                Console.Clear();
                CourseService.CourseWithSessionView course;
                try
                {
                    course = await courseService.GetCourseWithSessions(courseId);
                }
                catch (Exception ex)
                {
                    ConsoleHelper.Pause($"Kunde inte hämta kursdetaljer: {ex.Message}");
                    return;
                }

                Console.WriteLine($"== {course.Title}==");
                Console.WriteLine($"Instruktör: {course.InstructorName}");
                Console.WriteLine($"Pris: {course.pricePerParticipant}");
                Console.WriteLine($"Platser: {course.Capacity}");
                Console.WriteLine($"Beskrivning: {course.Description}");
                Console.WriteLine();

                Console.WriteLine("Tillfällen: ");
                if (course.Sessions.Count == 0)
                    Console.WriteLine("- Inga tillfällen planerade.");
                else
                    foreach (var s in course.Sessions)
                    {
                        Console.WriteLine($"{s.StartTime:yyyy-MM-dd HH:mm} - {s.EndTime:HH:mm}");
                    }

                Console.WriteLine("1. Tillbaka till kurslistan");
                Console.WriteLine("2. Anmäl till den aktuella kursen");

                var choice = ConsoleHelper.ReadInt();

                if (choice == 1) return;
                if (choice == 2)
                {
                    try
                    {
                        await courseService.EnrollMemberAsync(courseId, memberId);
                        ConsoleHelper.Pause("Du är nu anmäld till kursen, betalningsinformation skickas via epost");
                        return;
                    }
                    catch (Exception ex)
                    {
                        ConsoleHelper.Pause($"Kunde inte anmäla: {ex.Message}");
                    }
                }
                else
                {
                    ConsoleHelper.Pause("Ogiltigt val");
                }
            }
        }
    }
}
