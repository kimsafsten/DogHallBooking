using HallBooking.Helpers;
using HallBooking.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HallBooking.UI.Member
{
    public class BookHallView
    {
        public static async Task Start(int memberId, BookingService bookingService)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("== Visa lediga halltider ==");

                var day = ConsoleHelper.ReadDateOnly("Ange datum (YYYY-MM-DD): ");

                List<BookingService.AvailableTimesView> slots;
                try
                {
                    slots = await bookingService.GetAvailableTimeSlotsAsync(day);
                }
                catch (Exception ex)
                {
                    ConsoleHelper.Pause($"Kunde inte hämta lediga tider: {ex.Message}");
                    return;
                }
                Console.Clear();
                Console.WriteLine($"Lediga tider för {day:yyyy-MM-dd}:");

                if (slots.Count == 0)
                {
                    Console.WriteLine("Inga lediga tider den dagen");
                }

                for (int i = 0; i < slots.Count; i++)
                {
                    Console.WriteLine($"{i + 1}. {slots[i].StartTime:HH:mm} - {slots[i].EndTime:HH:mm}");
                }
                Console.WriteLine();

                Console.WriteLine("1) Boka en av tiderna");
                Console.WriteLine("2) Välj ny dag");
                Console.WriteLine("0. Tillbaka");
                Console.Write("Välj: ");

                var action = ConsoleHelper.ReadInt();

                switch (action)
                {
                    case 0:
                        return;

                    case 2:
                        continue;

                    case 1:
                        if (slots.Count == 0)
                        {
                            ConsoleHelper.Pause("Det finns inga lediga tider den dagen");
                            continue;
                        }

                        await BookFromSlots(memberId, bookingService, slots);
                        return;

                    default:
                        ConsoleHelper.Pause("Ogiltigt val");
                        break;
                }
            }
        }

        private static async Task BookFromSlots(
            int memberId,
            BookingService bookingService,
            List<BookingService.AvailableTimesView> slots)
        {
            Console.Write("Välj starttid (nummer): ");
            var startChoice = ConsoleHelper.ReadInt();

            if (startChoice < 1 || startChoice > slots.Count)
            {
                ConsoleHelper.Pause("Ogiltigt val");
                return;
            }

            var startIndex = startChoice - 1;

            Console.Write("Hur många timmar vill du boka? (1..): ");
            var hours = ConsoleHelper.ReadInt();

            if (hours < 1)
            {
                ConsoleHelper.Pause("Du måste boka minst en timme");
                return;
            }

            if (startIndex + hours - 1 >= slots.Count)
            {
                ConsoleHelper.Pause("Det finns inte tillräckligt många lediga tider från den starttiden");
                return;
            }

            for (int i = startIndex; i < startIndex + hours - 1; i++)
            {
                if (slots[i].EndTime != slots[i + 1].StartTime)
                {
                    ConsoleHelper.Pause("Valda tider är inte sammanhängande");
                    return;
                }
            }

            var start = slots[startIndex].StartTime;
            var end = slots[startIndex + hours - 1].EndTime;

            try
            {
                await bookingService.CreateHallRentalAsync(memberId, start, end);
                ConsoleHelper.Pause($"Bokning skapad: {start:HH:mm}-{end:HH:mm}");
                return;
            }
            catch (Exception ex)
            {
                ConsoleHelper.Pause($"Kunde inte hämta/boka lediga tider: {ex.Message}");
            }
        }
    }
}
