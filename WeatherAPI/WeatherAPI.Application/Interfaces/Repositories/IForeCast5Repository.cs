namespace WeatherAPI.Application.Interfaces.Repositories
{
    public interface IForeCast5Repository
    {
        Task<IEnumerable<T>> GetWeatherForCityAsync<T>(string lat, string lon);
    }
}
