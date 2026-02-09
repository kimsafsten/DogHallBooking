using HallBooking.Helpers;
using HallBooking.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HallBooking.UI.Admin
{
    public class ShowMembersView
    {
        public static async Task Start(MemberService memberService)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("== Visa medlemmar ==");

                List<MemberService.MemberListView> members;
                try
                {
                    members = await memberService.GetMembersAsync();
                }
                catch (Exception ex)
                {
                    ConsoleHelper.Pause($"Kunde inte hämta medlemmar: {ex.Message}");
                    return;
                }
                if (members.Count == 0)
                {
                    ConsoleHelper.Pause("Det finns inga medlemmar");
                    return;
                }

                for (int i = 0; i < members.Count; i++)
                {
                    var m = members[i];
                    Console.WriteLine($"{i + 1}. {m.Name} (Id: {m.Id}, {m.Email})");
                }

                Console.WriteLine();
                Console.WriteLine("0. Tillbaka");
                Console.Write("Välj en medlem(nummer): ");
                var choice = ConsoleHelper.ReadInt();

                if (choice == 0) return;

                if (choice < 1 || choice > members.Count)
                {
                    ConsoleHelper.Pause("Ogiltigt val");
                    continue;
                }

                var selectedMemberId = members[choice - 1].Id;
                var selMemberName = members[choice - 1].Name;

                await ShowMemberDetails(selectedMemberId, selMemberName, memberService);
            }
        }
        private static async Task ShowMemberDetails(int memberId, string memberName, MemberService memberService)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine($"Du visar nu {memberName} (Id: {memberId})");
                Console.WriteLine("0. Tillbaka");
                Console.WriteLine("1. Visa detaljer");
                Console.Write("Välj: ");
                var choice = ConsoleHelper.ReadInt();

                switch (choice)
                {
                    case 0:
                        return;

                    case 1:
                        var member = await memberService.GetMemberByIdAsync(memberId);
                        if (member is null)
                        {
                            ConsoleHelper.Pause("Ingen medlem hittades");
                            return;
                        }
                        Console.WriteLine($"Id: {member.Id}");
                        Console.WriteLine($"Namn: {member.Name}");
                        Console.WriteLine($"Epost: {member.Email}");
                        Console.WriteLine($"Skapad: {member.CreatedAt}");
                        ConsoleHelper.Pause();
                        break;

                    default:
                        ConsoleHelper.Pause("Ogiltigt val");
                        break;
                }
            }
        }
    }
}
