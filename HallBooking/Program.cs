using HallBooking.Data;
using HallBooking.Services;
using HallBooking.UI;
using Microsoft.EntityFrameworkCore;

namespace HallBooking
{
    class Program
    {
        static async Task Main(string[] args)
        {
            

            var memberService = new MemberService();
            var courseService = new CourseService();
            var bookingService = new BookingService();
            var paymentService = new PaymentService();

            var menu = new StartView();

            await menu.RunAsync(memberService, courseService, bookingService, paymentService);
        }
    }
}
