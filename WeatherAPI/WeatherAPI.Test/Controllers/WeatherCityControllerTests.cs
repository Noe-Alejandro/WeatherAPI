using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using WeatherAPI.Application.Dtos.WeatherCity.Response;
using WeatherAPI.Application.Interfaces.Services;
using WeatherAPI.Controllers.v1;
using WeatherAPI.Domain.Models.Response;

namespace WeatherAPI.Test.Controllers
{
    public class WeatherCityControllerTests
    {
        private readonly Mock<IWeatherService> _mockWeatherService;
        private readonly WeatherCityController _controller;

        public WeatherCityControllerTests()
        {
            _mockWeatherService = new Mock<IWeatherService>();
            _controller = new WeatherCityController(_mockWeatherService.Object);
        }

        [Fact]
        public async Task GetWeatherForCities_Should_Return_200_When_Success()
        {
            // Arrange
            var cityName = "Cancún";
            var mockCityResponse = new List<GetWeatherCityResponseDto>
            {
                new GetWeatherCityResponseDto
                {
                    city_name = "Cancún",
                    lat = "21.161908",
                    @long = "-86.8515279",
                    popularity = "0.0470989330134478"
                }
            };

            _mockWeatherService.Setup(service => service.GetWeatherCities(cityName))
                .ReturnsAsync(mockCityResponse);

            // Act
            var result = await _controller.GetWeatherForCities(cityName);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult?.StatusCode.Should().Be(200);

            var apiResponse = okResult?.Value as ApiResponse<IEnumerable<GetWeatherCityResponseDto>>;
            apiResponse?.Data.Should().BeEquivalentTo(mockCityResponse);
        }

        [Fact]
        public async Task GetWeatherForCities_Should_Return_500_When_Exception_Occurs()
        {
            // Arrange
            var cityName = "Cancún";
            _mockWeatherService.Setup(service => service.GetWeatherCities(cityName))
                .ThrowsAsync(new Exception("Repository failure"));

            // Act
            var result = await _controller.GetWeatherForCities(cityName);

            // Assert
            result.Should().BeOfType<StatusCodeResult>();
            var objectResult = result as StatusCodeResult;
            objectResult?.StatusCode.Should().Be(500);
        }
    }
}
