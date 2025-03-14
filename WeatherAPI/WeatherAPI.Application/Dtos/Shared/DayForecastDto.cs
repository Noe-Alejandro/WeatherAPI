namespace WeatherAPI.Application.Dtos.Shared
{
    public class DayForecastDto
    {
        public required string Date { get; set; }
        public required MinMaxTemperature Temperature { get; set; }
        public required List<WeatherByDateTime> WeatherByDateTime { get; set; }
    }

    public class WeatherByDateTime
    {
        public DateTime DateTime { get; set; }
        public required TemperatureDto Temperature { get; set; }
        public required string WeatherCondition { get; set; }
        public required string Icon { get; set; }
    }

    public class TemperatureDto : MinMaxTemperature
    {
        public double CurrentTemperature { get; set; }
    }

    public class MinMaxTemperature
    {
        public double MinTemperature { get; set; }
        public double MaxTemperature { get; set; }
    }
}
