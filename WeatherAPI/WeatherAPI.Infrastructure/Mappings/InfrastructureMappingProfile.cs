using AutoMapper;
using WeatherAPI.Application.Dtos.Shared;
using WeatherAPI.Application.Dtos.WeatherCity.Response;
using WeatherAPI.Infrastructure.Mappings.Converter;
using WeatherAPI.Infrastructure.Models.ForeCast.Response;
using WeatherAPI.Infrastructure.Models.ReservamosWeather.Response;

namespace WeatherAPI.Infrastructure.Mappings
{
    public class InfrastructureMappingProfile : Profile
    {
        public InfrastructureMappingProfile()
        {
            CreateMap<GetReservamosWeatherCityResponse, GetWeatherCityResponseDto>();

            CreateMap<List<ForeCast5List>, List<DayForecastDto>>()
                .ConvertUsing<WeatherDataToDayForecastDtoConverter>();
        }
    }
}
