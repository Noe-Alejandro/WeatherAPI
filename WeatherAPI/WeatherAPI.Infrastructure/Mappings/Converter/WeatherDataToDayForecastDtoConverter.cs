using AutoMapper;
using WeatherAPI.Application.Dtos.Shared;
using WeatherAPI.Infrastructure.Models.ForeCast.Response;

namespace WeatherAPI.Infrastructure.Mappings.Converter
{
    public class WeatherDataToDayForecastDtoConverter : ITypeConverter<List<ForeCast5List>, List<DayForecastDto>>
    {
        public List<DayForecastDto> Convert(List<ForeCast5List> source, List<DayForecastDto> destination, ResolutionContext context)
        {
            var groupedByDay = source
                .GroupBy(forecasts => DateTime.Parse(forecasts.dt_txt).Date)
                .Select(forecastDay => new DayForecastDto
                {
                    Date = forecastDay.Key.ToString("yyyy-MM-dd"),
                    Temperature = new TemperatureDto
                    {
                        MinTemperature = forecastDay.Min(forecastByTime => forecastByTime.main.temp_min),
                        MaxTemperature = forecastDay.Max(forecastByTime => forecastByTime.main.temp_max)
                    },
                    WeatherByDateTime = forecastDay.Select(forecastByTime => new WeatherByDateTime
                    {
                        DateTime = DateTime.Parse(forecastByTime.dt_txt),
                        WeatherCondition = forecastByTime.weather.FirstOrDefault().description,
                        Icon = forecastByTime.weather.FirstOrDefault()?.icon,
                        Temperature = new TemperatureDto
                        {
                            CurrentTemperature = forecastByTime.main.temp,
                            MinTemperature = forecastByTime.main.temp_min,
                            MaxTemperature = forecastByTime.main.temp_max
                        }
                    }).ToList()
                })
                .ToList();

            return groupedByDay;
        }
    }
}
