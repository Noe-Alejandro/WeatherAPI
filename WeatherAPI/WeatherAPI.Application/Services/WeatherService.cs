using WeatherAPI.Application.Dtos.Shared;
using WeatherAPI.Application.Dtos.WeatherCity.Response;
using WeatherAPI.Application.Interfaces.Repositories;
using WeatherAPI.Application.Interfaces.Services;
using WeatherAPI.Application.Interfaces.Utils;

namespace WeatherAPI.Application.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly IWeatherRepository _weatherRepository;
        private readonly IForeCast5Repository _forecastRepository;
        private readonly IParallelRequestProcessor _parallelRequest;
        private readonly ICachingUtil _cache;

        public WeatherService(IWeatherRepository weatherRepository, IForeCast5Repository foreCast5Repository, IParallelRequestProcessor parallelRequest, ICachingUtil cache)
        {
            _weatherRepository = weatherRepository;
            _forecastRepository = foreCast5Repository;
            _parallelRequest = parallelRequest;
            _cache = cache;
        }

        public async Task<IEnumerable<GetWeatherCityResponseDto>> GetWeatherCities(string cityName)
        {
            string cityCacheKey = $"GetWeatherCities_{cityName}";
            if (_cache.TryGet(cityCacheKey, out var cachedCities) && cachedCities is List<GetWeatherCityResponseDto> cities)
            {
                return cities;
            }

            var popularCities = await _weatherRepository.GetWeatherCitiesAsync<GetWeatherCityResponseDto>(cityName);
            var popularCitiesParallel = await _parallelRequest.ProcessInParallel(popularCities, async city =>
            {
                if (city.lat != null && city.@long != null)
                {
                    var forecastByDays = await _forecastRepository.GetWeatherForCityAsync<DayForecastDto>(city.lat, city.@long);
                    city.foreCast = forecastByDays;
                }
                return city;
            }, 5);

            var result = popularCitiesParallel.OrderByDescending(item => item.popularity).ToList();
            _cache.Add(cityCacheKey, result);
            return result;
        }
    }
}
