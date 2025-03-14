namespace WeatherAPI.Application.Interfaces.Utils
{
    public interface ICachingUtil
    {
        bool TryGet(string key, out object? data);
        void Add(string key, object data, TimeSpan? duration = null);
        void Remove(string key);
        void Clear();
    }
}
