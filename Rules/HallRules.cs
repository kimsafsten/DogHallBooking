namespace HallBooking.Services
{
    // Business rules for hall rentals
    internal static class HallRules
    {
        public static readonly TimeOnly OpenTime = new(8, 0);
        public static readonly TimeOnly CloseTime = new(22, 0);
        public const int SlotMinutes = 60;
        public const decimal HourlyPrice = 280m;
    }

}
