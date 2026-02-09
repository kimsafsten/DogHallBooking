using HallBooking.Helpers;
using HallBooking.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HallBooking.UI.Admin
{
    public class HallRentalView
    {
        public static async Task Start(BookingService bookingService)
        {
            while (true)
            {

                Console.Clear();
                Console.WriteLine("== Hallbokningar ==");
                Console.WriteLine("1. Visa bokade tider för en dag");
                Console.WriteLine("2. Visa och välj kommande bokningar");
                Console.WriteLine("3. Visa bokningsdetaljer (med Id)");
                Console.WriteLine("4. Ändra hallbokning (med Id)");
                Console.WriteLine("5. Ta bort hallbokning (med Id)");
                Console.WriteLine("0. Tillbaka");
                Console.WriteLine("Välj: ");

                var choice = ConsoleHelper.ReadInt();

                switch (choice)
                {
                    case 0:
                        return;

                    case 1:
                        await ShowBookedBlocksForDay(bookingService);
                        break;

                    case 2:
                        await ShowUpcomingHallRentals(bookingService);
                        break;

                    case 3:
                        await ShowHallRentalDetails(bookingService);
                        break;

                    case 4:
                        await UpdateHallRental(bookingService);
                        break;

                    case 5:
                        await RemoveHallRental(bookingService);
                        break;

                    default:
                        ConsoleHelper.Pause("Ogiltigt val");
                        break;
                }
            }
        }
        private static async Task ShowBookedBlocksForDay(BookingService bookingService)
        {
            Console.Clear();
            Console.WriteLine("== Bokade tider ==");
            var day = ConsoleHelper.ReadDateOnly("Datum (YYYY-MM-DD): ");

            try
            {
                var blocks = await bookingService.GetBookedTimeBlockAsync(day);

                Console.Clear();
                Console.WriteLine($"== Bokade tider för {day:yyyy-MM-dd} ==");

                if (blocks.Count == 0)
                {
                    ConsoleHelper.Pause("Inga bokningar eller kurstillfällen den dagen");
                    return;
                }

                foreach (var b in blocks)
                {
                    Console.WriteLine($"{b.StartTime:HH:mm}-{b.EndTime:HH:mm} [{b.Type}] " +
                        $"Id: {b.SourceId} | {b.Label}");
                }
                ConsoleHelper.Pause();
            }
            catch (Exception ex)
            {
                ConsoleHelper.Pause($"Kunde inte hämta bokade tider: {ex.Message}");
            }
        }
        private static async Task ShowHallRentalDetails(BookingService bookingService)
        {
            Console.Clear();
            Console.WriteLine("== Visa bokningsdetaljer ==");

            Console.Write("Ange bokningsId (0 = avbryt): ");
            var id = ConsoleHelper.ReadInt();
            if (id == 0) return;

            try
            {
                var details = await bookingService.GetDetailsForHallRentalAsync(id);
                if (details == null)
                {
                    ConsoleHelper.Pause("Ingen hallbokning hittades");
                    return;
                }

                Console.WriteLine($"Id: {details.Id}");
                Console.WriteLine($"Medlem: {details.MemberName} (Id: {details.MemberId})");
                Console.WriteLine($"Tid: {details.StartTime:yyyy-MM-dd HH:mm} - {details.EndTime:HH:mm}");
                Console.WriteLine($"Status: {details.Status}");
                Console.WriteLine($"Pris (beräknat): {details.TotalPrice} kr");
                Console.WriteLine($"Senaste betalningsstatus: {details.LatestPaymentStatus?.ToString() ?? "-"}");
                Console.WriteLine($"Skapad: {details.CreatedAt:yyyy-MM-dd HH:mm}");

                ConsoleHelper.Pause();
            }
            catch (Exception ex)
            {
                ConsoleHelper.Pause($"Kunde inte hämta detaljerna: {ex.Message}");
            }
        }
        private static async Task ShowUpcomingHallRentals(BookingService bookingService)
        {
            Console.Clear();
            Console.WriteLine("== Kommande bokningar ==");

            List<BookingService.HallRentalOverviewView> rentals;
            try
            {
                rentals = await bookingService.GetUpcomingHallRentalsAsync();
            }
            catch (Exception ex)
            {
                ConsoleHelper.Pause($"Kunde inte hämta bokningar: {ex.Message}");
                return;
            }

            if (rentals.Count == 0)
            {
                ConsoleHelper.Pause("Det finns inga kommande bokningar");
                return;
            }

            for (int i = 0; i < rentals.Count; i++)
            {
                var r = rentals[i];
                Console.WriteLine($"{i + 1}. Id: {r.Id} | {r.StartTime:yyyy-MM-dd HH:mm}-" +
                    $"{r.EndTime:HH:mm} | {r.MemberName} | {r.TotalPrice} kr");
            }
            Console.WriteLine();
            Console.WriteLine("Tips: använd Id ovan i menyval 3–5.");
            ConsoleHelper.Pause();
        }
        private static async Task UpdateHallRental(BookingService bookingService)
        {
            Console.Clear();
            Console.WriteLine("== Ändra en bokning ==");

            Console.Write("Ange bokningsId (0 = avbryt): ");
            var id = ConsoleHelper.ReadInt();
            if (id == 0) return;

            var day = ConsoleHelper.ReadDateOnly("Nytt datum (YYYY-MM-DD): ");
            var startTime = ConsoleHelper.ReadTimeOnly("Ny starttid (HH:mm): ");
            var endTime = ConsoleHelper.ReadTimeOnly("Ny sluttid (HH:mm): ");

            var start = day.ToDateTime(startTime);
            var end = day.ToDateTime(endTime);

            try
            {
                await bookingService.UpdateHallRentalAsync(id, start, end);
                ConsoleHelper.Pause("Bokningen är uppdaterad");
            }
            catch (Exception ex)
            {
                ConsoleHelper.Pause($"Kunde inte uppdatera bokningen: {ex.Message}");
            }
        }
        private static async Task RemoveHallRental(BookingService bookingService)
        {
            Console.Clear();
            Console.WriteLine("== Avboka bokning ==");

            Console.Write("Ange bokningsId (0 = avbryt): ");
            var id = ConsoleHelper.ReadInt();
            if (id == 0) return;

            try
            {
                await bookingService.CancelHallRentalAsync(id);
                ConsoleHelper.Pause("Tog bort bokningen");
            }
            catch (Exception ex)
            {
                ConsoleHelper.Pause($"Kunde inte ta bort bokningen: {ex.Message}");
            }
        }
    }
}
