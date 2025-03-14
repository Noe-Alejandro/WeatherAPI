using WeatherAPI.Application.Dtos.WeatherCity.Response;

namespace WeatherAPI.Application.Interfaces.Services
{
    public interface IWeatherService
    {
        Task<IEnumerable<GetWeatherCityResponseDto>> GetWeatherCities(string q);
    }
}
