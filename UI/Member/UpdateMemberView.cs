using HallBooking.Helpers;
using HallBooking.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HallBooking.UI.Member
{
    public class UpdateMemberView
    {
        public static async Task Start(int memberId, MemberService memberService)
        {
            while (true)
            {

                Console.Clear();
                Console.WriteLine("== Ändra dina uppgifter ==");
                Console.WriteLine("1. Ändra din epost");
                Console.WriteLine("2. Ändra ditt namn och epost");
                Console.WriteLine("0. Återvänd till medlemssidan");
                Console.Write("Välj: ");

                var choice = ConsoleHelper.ReadInt();
                switch (choice)
                {
                    case 0:
                        return;

                    case 1:
                        await UpdateEmail(memberId, memberService);
                        break;

                    case 2:
                        await UpdateNameAndEmail(memberId, memberService);
                        break;

                    default:
                        ConsoleHelper.Pause("Ogiltigt val");
                        break;
                }
            }
        }

        private static async Task UpdateEmail(int memberId, MemberService memberService)
        {
            Console.Clear();
            Console.WriteLine("== Ändra epost ==");

            var newEmail = ConsoleHelper.ReadEmail("Fyll i din nya epost: ");

            try
            {
                await memberService.UpdateEmailAsync(memberId, newEmail);
                ConsoleHelper.Pause("Eposten är uppdaterad");
            }
            catch (Exception ex)
            {
                ConsoleHelper.Pause($"Kunde inte uppdatera epost: {ex.Message}");
            }
        }

        private static async Task UpdateNameAndEmail(int memberId, MemberService memberService)
        {
            Console.Clear();
            Console.WriteLine("== Ändra namn och epost ==");

            var me = await memberService.GetMemberByIdAsync(memberId);
            if (me == null)
            {
                ConsoleHelper.Pause("Kunde inte hitta medlem");
                return;
            }

            Console.WriteLine($"Nuvarande namn : {me.Name}");
            Console.WriteLine($"Nuvarande epost: {me.Email}");

            Console.WriteLine("Tryck Enter för att behålla nuvarande värde.");

            Console.Write("Nytt namn: ");
            var nameInput = (Console.ReadLine() ?? "").Trim();
            var newName = string.IsNullOrWhiteSpace(nameInput) ? me.Name : nameInput;

            var newEmail = ConsoleHelper.ReadEmail("Ny epost", me.Email);

            try
            {
                await memberService.UpdateMemberAsync(memberId, newName, newEmail);
                ConsoleHelper.Pause("Användaren är uppdaterad");
            }
            catch (Exception ex)
            {
                ConsoleHelper.Pause($"Kunde inte uppdatera: {ex.Message}");
            }
        }
    }
}
