using HallBooking.Helpers;
using HallBooking.Services;

namespace HallBooking.UI.Member
{
    internal class MemberView
    {

        public static async Task Start(
          int memberId,
          MemberService memberService,
          CourseService courseService,
          BookingService bookingService)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"Medlem (ID: {memberId})");
                Console.WriteLine("1. Ändra dina uppgifter");
                Console.WriteLine("2. Boka hallen");
                Console.WriteLine("3. Visa kurser");
                Console.WriteLine("0. Logga ut");
                var choice = ConsoleHelper.ReadInt();

                switch (choice)
                {
                    case 0:
                        return;

                    case 1:
                        await UpdateMemberView.Start(memberId, memberService);
                        break;

                    case 2:
                        await BookHallView.Start(memberId, bookingService);
                        break;

                    case 3:
                        await MemberCoursesView.Start(memberId, courseService);
                        break;

                    default:
                        ConsoleHelper.Pause("Ogiltigt val");
                        break;
                }
            }
        }

    }
}


