using HallBooking.Entities;
using HallBooking.Helpers;
using HallBooking.Services;
using HallBooking.UI.Admin.Course;

namespace HallBooking.UI.Admin
{
    public class AdminView
    {

        public static async Task Start(
            MemberService memberService,
            CourseService courseService,
            BookingService bookingService,
            PaymentService paymentService)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Välkommen till adminsidan");
                Console.WriteLine("1. Medlemmar");
                Console.WriteLine("2. Hantera kurser");
                Console.WriteLine("3. Hallbokningar");
                Console.WriteLine("4. Betalningar");
                Console.WriteLine("0. Logga ut");
                var choice = ConsoleHelper.ReadInt();

                switch (choice)
                {
                    case 0:
                        return;

                    case 1:
                        await ShowMembersView.Start(memberService);
                        break;

                    case 2:
                        await CoursesView.Start(courseService);
                        break;

                    case 3:
                        await HallRentalView.Start(bookingService);
                        break;

                    case 4:
                        await PaymentView.Start(paymentService);
                        break;

                    default:
                        ConsoleHelper.Pause("Ogiltligt val");
                        break;

                }
            }
        }

      
        
        
    }

}
