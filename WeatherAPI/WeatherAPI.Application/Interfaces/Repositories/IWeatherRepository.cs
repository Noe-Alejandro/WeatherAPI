namespace WeatherAPI.Application.Interfaces.Repositories
{
    public interface IWeatherRepository
    {
        Task<IEnumerable<T>> GetWeatherCitiesAsync<T>(string cityName);
    }
}
