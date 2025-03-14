namespace WeatherAPI.Infrastructure.Models.ReservamosWeather.Response
{
    public class GetReservamosWeatherCityResponse
    {
        public int id { get; set; }
        public string slug { get; set; }
        public string city_slug { get; set; }
        public string display { get; set; }
        public string ascii_display { get; set; }
        public string city_name { get; set; }
        public string city_ascii_name { get; set; }
        public string state { get; set; }
        public string country { get; set; }
        public string lat { get; set; }
        public string @long { get; set; }
        public string result_type { get; set; }
        public string popularity { get; set; }
        public double sort_criteria { get; set; }
    }
}
