using HallBooking.Helpers;
using HallBooking.Services;
using HallBooking.UI.Admin;
using HallBooking.UI.Member;

namespace HallBooking.UI
{
    public class StartView
    {
        //Password for login to AdminView
        static string adminPassword = "admin26";
        public async Task RunAsync(
            MemberService memberService,
            CourseService courseService,
            BookingService bookingService,
            PaymentService paymentService)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Välkommen till Skånes Hundcenter");
                Console.WriteLine("Välj vad du vill göra");
                Console.WriteLine("1. Logga in som medlem");
                Console.WriteLine("2. Logga in som admin");
                Console.WriteLine("0. Avsluta programmet");
                var choice = ConsoleHelper.ReadInt();

                switch (choice)
                {
                    case 0:
                        return;

                    case 1:
                        var memberId = await MemberLogin(memberService);
                        if (memberId != null)
                        {
                            await MemberView.Start(memberId.Value, memberService, courseService, bookingService);
                        }
                        break;

                    case 2:
                        await AdminLogin(memberService, courseService, bookingService, paymentService);
                        break;

                    default:
                        ConsoleHelper.Pause("Ogiltligt val.");
                        break;
                }
            }
        }
    
        private static async Task<int?> MemberLogin(MemberService memberService)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("Logga in som medlem");
                Console.Write("Ange din epost (tom rad = tillbaka): ");

                var email = (Console.ReadLine() ?? "").Trim();
                if (string.IsNullOrWhiteSpace(email)) return null;
                email = email.Trim().ToLowerInvariant();

                var member = await memberService.GetMemberByEmailAsync(email);
                if (member != null)
                {
                    ConsoleHelper.Pause($"Inloggad som: {member.Name} ({member.Email})");
                    return member.Id;
                }

                Console.WriteLine("Hittade ingen medelm med den eposten");
                Console.WriteLine("1. Försök igen");
                Console.WriteLine("2. Skapa ny medlem");
                Console.WriteLine("0. Tillbaka till startmenyn");

                var choice = ConsoleHelper.ReadInt();
                if (choice == 0) return null;
                if (choice == 1) continue;
                if (choice == 2)
                {
                    var newId = await CreateMember(memberService, email);
                    if (newId != null) return newId.Value;
                }

            }
        }

        private static async Task<int?> CreateMember(MemberService memberService, string? email)
        {
            Console.Clear();
            Console.WriteLine("Skapa medlem");
            Console.Write("Namn: ");

            var name = (Console.ReadLine() ?? "").Trim();
            if (string.IsNullOrWhiteSpace(name))
            {
                ConsoleHelper.Pause("Namn måste anges.");
                return null;
            }

            Console.WriteLine("Fyll i epost, om den är korrekt, tryck enter");
            var finalEmail = ConsoleHelper.ReadEmail("Epost", email);

            try
            {
                var id = await memberService.CreateMemberAsync(name, finalEmail);
                ConsoleHelper.Pause($"Medelm är skapad! Du är nu inloggad");
                return id;
            }
            catch (Exception ex)
            {
                ConsoleHelper.Pause($"Kunde inte skapa medlem: {ex.Message}");
                return null;
            }
        }

        private static async Task AdminLogin(
            MemberService memberService, 
            CourseService courseService, 
            BookingService bookingService,
            PaymentService paymentService)
        {
            Console.Clear();
            Console.WriteLine("Fyll i ditt lösenord för att kommma till adminsidan, (0 för att återgå)");
            Console.Write("Lösenord: ");
            var input = (Console.ReadLine() ?? "").Trim().ToLowerInvariant();

            if (input == "0")
                return;

            if (input == adminPassword)
            {
                await AdminView.Start(memberService, courseService, bookingService, paymentService);
                return;
            }

            ConsoleHelper.Pause("Felaktigt lösenord");
            
        }

    }
}

