using HallBooking.Helpers;
using HallBooking.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HallBooking.UI.Admin.Course
{
    public class CoursesView
    {
        public static async Task Start(CourseService courseService)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("== Hantera kurser ==");
                Console.WriteLine("1. Visa kurser / välj kurs");
                Console.WriteLine("2. Lägg till kurs");
                Console.WriteLine("0. Tillbaka");
                Console.Write("Välj: ");
                var choice = ConsoleHelper.ReadInt();

                switch (choice)
                {
                    case 0:
                        return;

                    case 1:
                        var courseId = await SelectCourse(courseService);
                        if (courseId != null)
                            await AdminCourseView.Start(courseId.Value, courseService);
                        break;

                    case 2:
                        await CreateNewCourse(courseService);
                        break;

                    default:
                        ConsoleHelper.Pause("Ogiltigt val");
                        break;
                }
            }
        }
        private static async Task<int?> SelectCourse(CourseService courseService)
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
                return null;
            }

            if (courses.Count == 0)
            {
                ConsoleHelper.Pause("Det finns inga kurser för tillfället");
                return null;
            }

            for (int i = 0; i < courses.Count; i++)
            {
                var c = courses[i];

                Console.WriteLine($"{i + 1}. {c.Title} (Id: {c.Id}) Tillfällen: {c.SessionCount}, Anmälda: {c.EnrollmentCount}/{c.Capacity}");
            }

            Console.WriteLine();
            Console.WriteLine("0. Tillbaka");
            Console.Write("Välj en kurs (nummer): ");

            var choice = ConsoleHelper.ReadInt();

            if (choice == 0)
                return null;

            if (choice < 1 || choice > courses.Count)
            {
                ConsoleHelper.Pause("Ogiltigt val");
                return null;
            }

            return courses[choice - 1].Id;

        }
        private static async Task CreateNewCourse(CourseService courseService)
        {
            Console.Clear();
            Console.WriteLine("== Lägg till en kurs ==");

            Console.Write("Kursnamn: ");
            var title = (Console.ReadLine() ?? "").Trim();
            if (string.IsNullOrWhiteSpace(title))
            {
                ConsoleHelper.Pause("Titel måste anges.");
                return;
            }

            Console.Write("Instruktör: ");
            var instructor = (Console.ReadLine() ?? "").Trim();
            if (string.IsNullOrWhiteSpace(instructor))
            {
                ConsoleHelper.Pause("Instruktör måste anges.");
                return;
            }

            Console.Write("Beskrivning: ");
            var description = (Console.ReadLine() ?? "").Trim();
            if (string.IsNullOrWhiteSpace(description))
            {
                ConsoleHelper.Pause("Beskrivning måste fyllas i.");
                return;
            }

            Console.Write("Antal deltagare: ");
            var capacity = ConsoleHelper.ReadInt();
            if (capacity <= 0)
            {
                ConsoleHelper.Pause("Det måste finnas minst en kursplats.");
                return;
            }

            var price = ConsoleHelper.ReadDecimal("Pris per deltagare: ");

            try
            {
                await courseService.CreateCourseAsync(title, instructor, description, capacity, price);
                ConsoleHelper.Pause($"Kursen \"{title}\" är skapad");
            }
            catch (Exception ex)
            {
                ConsoleHelper.Pause($"Kunde inte skapa kursen: {ex.Message}");
            }
        }
       
    }
}
