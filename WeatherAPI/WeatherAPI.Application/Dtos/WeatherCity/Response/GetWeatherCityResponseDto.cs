using WeatherAPI.Application.Dtos.Shared;

namespace WeatherAPI.Application.Dtos.WeatherCity.Response
{
    public class GetWeatherCityResponseDto
    {
        public int id { get; set; }
        public string city_name { get; set; }
        public string lat { get; set; }
        public string @long { get; set; }
        public string popularity { get; set; }
        public IEnumerable<DayForecastDto> foreCast { get; set; }
    }
}
