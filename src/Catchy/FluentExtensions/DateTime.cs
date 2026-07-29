namespace Catchy.FluentExtensions
{
    public static class DateTime
    {
        public static TimeSpan Seconds(this int seconds)
        {
            return TimeSpan.FromSeconds(seconds);
        }

        public static TimeSpan Minutes(this int minutes)
        {
            return TimeSpan.FromMinutes(minutes);
        }

        public static TimeSpan Hours(this int hours)
        {
            return TimeSpan.FromHours(hours);
        }

        public static TimeSpan Milliseconds(this int milliseconds)
        {
            return TimeSpan.FromMilliseconds(milliseconds);
        }
    }
}

