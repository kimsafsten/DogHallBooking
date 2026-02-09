using HallBooking.Entities;
using HallBooking.Helpers;
using HallBooking.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HallBooking.UI.Admin
{
    public class PaymentView
    {
        public static async Task Start(PaymentService paymentService)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("== Betalningar ==");
                Console.WriteLine("1. Visa väntande betalningar");
                Console.WriteLine("2. Visa misslyckade betalningar");
                Console.WriteLine("3. Visa betalningar för bokning av hall (hall Id)");
                Console.WriteLine("4. Ändra betalningsstatus (med Id)");
                Console.WriteLine("0. Tillbaka");
                Console.Write("Välj: ");

                var choice = ConsoleHelper.ReadInt();
                switch (choice)
                {
                    case 0:
                        return;

                    case 1:
                        await ShowPaymentsByStatus(PaymentStatus.Pending, paymentService);
                        break;

                    case 2:
                        await ShowPaymentsByStatus(PaymentStatus.Failed, paymentService);
                        break;

                    case 3:
                        await ShowPaymentHallRental(paymentService);
                        break;

                    case 4:
                        await UpdatePaymentStatus(paymentService);
                        break;

                    default:
                        ConsoleHelper.Pause("Ogiltigt val");
                        break;
                }
            }
        }
        private static async Task ShowPaymentsByStatus(PaymentStatus status, PaymentService paymentService)
        {
            Console.Clear();
            Console.WriteLine($"== Betalningar: {status} ==");

            try
            {
                var payments = await paymentService.GetPaymentsByStatusAsync(status);

                if (payments.Count == 0)
                {
                    ConsoleHelper.Pause("Kunde inte hittta några betalningar");
                    return;
                }

                foreach (var p in payments)
                {
                    var source = p.HallRentalId != null
                        ? $"HallRentalId: {p.HallRentalId}"
                        : $"CourseEnrollmentId: {p.CourseEnrollmentId}";

                    Console.WriteLine($"PaymentId: {p.Id} | {p.MemberName} (Id:{p.MemberId}) " +
                        $"| {p.Amount} kr | {p.Status} | {p.CreatedAt:yyyy-MM-dd HH:mm} | {source}");
                }
                ConsoleHelper.Pause();
            }
            catch (Exception ex)
            {
                ConsoleHelper.Pause($"Kunde inte hämta betalningar: {ex.Message}");
            }
        }
        private static async Task ShowPaymentHallRental(PaymentService paymentService)
        {
            Console.Clear();
            Console.WriteLine($"== Betalningar för hallbokning ==");
            Console.WriteLine("0 = tillbaka");
            Console.Write("BokningsId: ");
            var rentalId = ConsoleHelper.ReadInt();
            if (rentalId == 0) return;

            try
            {
                var payment = await paymentService.GetPaymentsForHallRentalAsync(rentalId);

                if (payment.Count == 0)
                {
                    ConsoleHelper.Pause("Kunde inte hittta någon betalning för den bokningen");
                    return;
                }

                foreach (var p in payment)
                {
                    Console.WriteLine($"PaymentId: {p.Id} | {p.MemberName} | {p.Amount} kr " +
                        $"| {p.Status} | {p.CreatedAt:yyyy-MM-dd HH:mm}");
                }
                ConsoleHelper.Pause();
            }
            catch (Exception ex)
            {
                ConsoleHelper.Pause($"Kunde inte hämta betalningar: {ex.Message}");
            }
        }
        private static async Task UpdatePaymentStatus(PaymentService paymentService)
        {
            Console.Clear();
            Console.WriteLine($"== Ändra betalningsstatus ==");
            Console.WriteLine("0 = tillbaka");
            Console.Write("BetalningsId: ");
            var paymentId = ConsoleHelper.ReadInt();
            if (paymentId == 0) return;

            Console.WriteLine("Välj ny status:");
            Console.WriteLine("1. Pending");
            Console.WriteLine("2. Paid");
            Console.WriteLine("3. Failed");
            Console.WriteLine("4. Refunded");
            Console.Write("Välj: ");

            var choice = ConsoleHelper.ReadInt();
            PaymentStatus newStatus;

            switch (choice)
            {
                case 1: newStatus = PaymentStatus.Pending; break;
                case 2: newStatus = PaymentStatus.Paid; break;
                case 3: newStatus = PaymentStatus.Failed; break;
                case 4: newStatus = PaymentStatus.Refunded; break;
                default:
                    ConsoleHelper.Pause("Ogiltigt val");
                    return;
            }

            try
            {
                await paymentService.UpdatePaymentStatusAsync(paymentId, newStatus);
                ConsoleHelper.Pause("Betalningsstatus är uppdaterad");
            }
            catch (Exception ex)
            {
                ConsoleHelper.Pause($"Kunde inte uppdatera betalning: {ex.Message}");
            }
        }
    }
}
