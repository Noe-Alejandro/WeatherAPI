namespace WeatherAPI.Infrastructure.Utils
{
    public class GenericListUtil<T>
    {
        public List<T> Items { get; set; }

        public GenericListUtil(IEnumerable<T> items)
        {
            Items = new List<T>(items);
        }
    }
}
