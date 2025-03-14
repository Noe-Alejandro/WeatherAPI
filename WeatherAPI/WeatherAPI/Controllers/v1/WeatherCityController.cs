using Microsoft.AspNetCore.Mvc;
using WeatherAPI.Application.Dtos.WeatherCity.Response;
using WeatherAPI.Application.Interfaces.Services;
using WeatherAPI.Domain.Models.Response;

namespace WeatherAPI.Controllers.v1
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class WeatherCityController : ControllerBase
    {
        private readonly IWeatherService _weatherService;

        public WeatherCityController(IWeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        [HttpGet(Name = "Get cities with weather")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<GetWeatherCityResponseDto>>), 200)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetWeatherForCities([FromQuery] string? q = null)
        {
            try
            {
                var response = await _weatherService.GetWeatherCities(q);
                return Ok(ApiResponse<IEnumerable<GetWeatherCityResponseDto>>.CreateSuccessResponse(response));
            }
            catch (Exception)
            {
                return StatusCode(500);
            }
        }
    }
}
