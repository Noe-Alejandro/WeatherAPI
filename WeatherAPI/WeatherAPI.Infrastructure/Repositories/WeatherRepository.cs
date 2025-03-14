using AutoMapper;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RestSharp;
using WeatherAPI.Application.Interfaces.Repositories;
using WeatherAPI.Infrastructure.Models.ReservamosWeather.Response;

namespace WeatherAPI.Infrastructure.Repositories
{
    public class WeatherRepository : IWeatherRepository
    {
        private readonly IMapper _mapper;
        private readonly RestClient _client;
        private readonly string _url;

        public WeatherRepository(IConfiguration configuration, IMapper mapper)
        {
            _mapper = mapper;
            _url = configuration.GetValue<string>("Reservamos:WeatherApiUrl") ?? "";
            _client = new RestClient(_url);
        }

        public async Task<IEnumerable<T>> GetWeatherCitiesAsync<T>(string cityName)
        {
            var request = new RestRequest("", Method.Get);
            if (!string.IsNullOrEmpty(cityName))
            {
                request.AddParameter("q", cityName, ParameterType.QueryString);
            }

            var response = await _client.ExecuteAsync(request);
            var popularCitiesResponse = JsonConvert.DeserializeObject<IEnumerable<GetReservamosWeatherCityResponse>>(response.Content ?? "")
                ?.Where(city => city.result_type.Equals("city"));

            var mappedResponse = _mapper.Map<IEnumerable<T>>(popularCitiesResponse);
            return mappedResponse;
        }
    }
}
