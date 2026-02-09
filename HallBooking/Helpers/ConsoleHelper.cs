using System.Globalization;

namespace HallBooking.Helpers
{
    public static class ConsoleHelper
    {
        public static int ReadInt()
        {
            while (true)
            {
                var input = Console.ReadLine();
                if (int.TryParse(input, out var value))
                    return value;

                Console.WriteLine("Skriv ett nummer: ");
            }
        }
        public static void Pause(string? message = null)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                Console.WriteLine(message);
                Console.WriteLine();
            }

            Console.WriteLine("Tryck Enter för att fortsätta");
            Console.ReadLine();
        }
        public static string ReadEmail(string prompt, string? defaultEmail = null)
        {
            while (true)
            {
                if (!string.IsNullOrWhiteSpace(defaultEmail))
                    Console.Write($"{prompt} [{defaultEmail}]: ");
                else
                    Console.Write(prompt);

                var input = (Console.ReadLine() ?? "").Trim();

                var email = string.IsNullOrWhiteSpace(input)
                    ? defaultEmail
                    : input;

                email = (email ?? "").Trim().ToLowerInvariant();

                if (string.IsNullOrWhiteSpace(email))
                {
                    Console.WriteLine("Epost måste anges.");
                    continue;
                }

                if (!email.Contains("@"))
                {
                    Console.WriteLine("Eposten verkar inte vara giltig.");
                    continue;
                }

                return email;
            }
        }
        public static DateOnly ReadDateOnly(string prompt)
        {
            while (true)
            {
                Console.WriteLine(prompt);
                var input = (Console.ReadLine() ?? "").Trim();

                if (DateOnly.TryParse(input, out var date))
                    return date;

                Console.WriteLine("Skriv ett giltligt datum, t.ex. 2026-01-29");
            }
        }
        public static decimal ReadDecimal(string prompt)
        {
            while (true)
            {
                Console.WriteLine(prompt);
                var input = (Console.ReadLine() ?? "").Trim();

                if (string.IsNullOrWhiteSpace(input))
                {
                    Console.WriteLine("Värde måste anges");
                    continue;
                }

                input = input.Replace(',', '.');

                if (decimal.TryParse(
                    input,
                    NumberStyles.Number,
                    CultureInfo.InvariantCulture,
                    out var value))
                {
                    if (value <= 0)
                    {
                        Console.WriteLine("Värdet måste vara större än 0");
                        continue;
                    }

                    return value;
                }
                Console.WriteLine("Skriv ett giltigt decimaltal (t.ex. 1495,00).");
            }
        }
        public static TimeOnly ReadTimeOnly(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                var input = (Console.ReadLine() ?? "").Trim();

                if (TimeOnly.TryParse(input, out var time))
                    return time;

                Console.WriteLine("Skriv en giltig tid, t.ex. 17:00");
            }
        }
    }
}
