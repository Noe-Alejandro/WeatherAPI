using AutoMapper;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RestSharp;
using WeatherAPI.Application.Interfaces.Repositories;
using WeatherAPI.Infrastructure.Models.ForeCast.Response;

namespace WeatherAPI.Infrastructure.Repositories
{
    public class ForeCast5Repository : IForeCast5Repository
    {
        private readonly IMapper _mapper;
        private readonly RestClient _client;
        private readonly string _url;
        private readonly string _appid;

        public ForeCast5Repository(IConfiguration configuration, IMapper mapper)
        {
            _mapper = mapper;
            _url = configuration.GetValue<string>("ForeCast:BaseUrl") ?? "";
            _appid = configuration.GetValue<string>("ForeCast:APPID") ?? "";
            _client = new RestClient(_url);
        }

        public async Task<IEnumerable<T>> GetWeatherForCityAsync<T>(string lat, string lon)
        {
            try
            {
                var request = new RestRequest("", Method.Get);
                request.AddQueryParameter("lat", lat);
                request.AddQueryParameter("lon", lon);
                request.AddQueryParameter("appid", _appid);

                var response = await _client.ExecuteAsync(request);

                if (response.IsSuccessful)
                {
                    var weatherData = JsonConvert.DeserializeObject<GetForeCast5Response>(response.Content);

                    if (weatherData != null)
                    {
                        var mappedResponse = _mapper.Map<List<T>>(weatherData.list);
                        return mappedResponse;
                    }

                    throw new Exception("Datos de clima inválidos.");
                }
                else
                {
                    throw new Exception("Error al obtener los datos del clima");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al obtener los datos de la ciudad: {ex.Message}");
            }
        }
    }
}
