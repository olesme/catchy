using Catchy;

namespace CatchySourceGenTests.TestTypes
{
    public class Temperature
    {
        public decimal Celsius { get; set; }
        public decimal Fahrenheit => Celsius * 9 / 5 + 32;
    }

    [AssertFor(typeof(Temperature))]
    public static partial class TemperatureAssertions
    {
        [Assertion("be freezing (at or below 0°C)")]
        public static bool IsFreezing(Temperature t) => t.Celsius <= 0;

        [Assertion("be boiling (at or above 100°C)")]
        public static bool IsBoiling(Temperature t) => t.Celsius >= 100;
    }

    public class Humidity
    {
        public int Percent { get; set; }
    }

    public class HumidityReading
    {
        public int Percent { get; set; }
    }

    [AssertFor<Humidity>]
    public static partial class HumidityAssertions
    {
        [Assertion("be comfortable (between 30% and 60%)")]
        public static bool IsComfortable(Humidity h) => h.Percent >= 30 && h.Percent <= 60;
    }

    [AssertFor(typeof(HumidityReading))]
    public static partial class HumidityReadingAssertions
    {
        [Assertion("be humid (at or above 50%)")]
        public static bool IsHumid(HumidityReading h) => h.Percent >= 50;
    }
}
