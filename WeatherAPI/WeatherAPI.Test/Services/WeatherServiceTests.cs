using FluentAssertions;
using Moq;
using WeatherAPI.Application.Dtos.WeatherCity.Response;
using WeatherAPI.Application.Interfaces.Repositories;
using WeatherAPI.Application.Interfaces.Utils;
using WeatherAPI.Application.Services;

namespace WeatherAPI.Test.Services
{
    public class WeatherServiceTests
    {
        private readonly Mock<IWeatherRepository> _mockWeatherRepository;
        private readonly Mock<IForeCast5Repository> _mockForecastRepository;
        private readonly Mock<IParallelRequestProcessor> _mockParallelRequest;
        private readonly Mock<ICachingUtil> _mockCache;
        private readonly WeatherService _weatherService;

        public WeatherServiceTests()
        {
            _mockWeatherRepository = new Mock<IWeatherRepository>();
            _mockForecastRepository = new Mock<IForeCast5Repository>();
            _mockParallelRequest = new Mock<IParallelRequestProcessor>();
            _mockCache = new Mock<ICachingUtil>();

            _weatherService = new WeatherService(
                _mockWeatherRepository.Object,
                _mockForecastRepository.Object,
                _mockParallelRequest.Object,
                _mockCache.Object
            );
        }

        [Fact]
        public async Task GetWeatherCities_Should_Return_Cities_When_CityName_Is_Provided()
        {
            // Arrange
            var cityName = "Cancún";
            var cityCacheKey = $"GetWeatherCities_{cityName}";
            var mockCityResponse = new List<GetWeatherCityResponseDto>
            {
                new GetWeatherCityResponseDto { city_name = "Cancún", lat = "21.161908", @long = "-86.8515279", popularity = "0.0470989330134478" }
            };

            // Configuración del caché para evitar valores preexistentes
            _mockCache.Setup(cache => cache.TryGet(cityCacheKey, out It.Ref<object>.IsAny)).Returns(false);

            // Configuración del repositorio para devolver la respuesta simulada
            _mockWeatherRepository.Setup(repo => repo.GetWeatherCitiesAsync<GetWeatherCityResponseDto>(cityName))
                .ReturnsAsync(mockCityResponse);

            // Configuración de las llamadas en paralelo
            _mockParallelRequest.Setup(parallel => parallel.ProcessInParallel(It.IsAny<IEnumerable<GetWeatherCityResponseDto>>(), It.IsAny<Func<GetWeatherCityResponseDto, Task<GetWeatherCityResponseDto>>>(), 5))
                .ReturnsAsync(mockCityResponse);

            // Captura de la llamada al método Add usando un Callback
            _mockCache.Setup(cache => cache.Add(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<TimeSpan?>()))
                .Callback<string, object, TimeSpan?>((key, value, duration) =>
                {
                    // Verificaciones en el Callback
                    key.Should().Be(cityCacheKey);  // Verifica que la clave sea la esperada
                    value.Should().BeEquivalentTo(mockCityResponse);  // Verifica que el valor coincide con la respuesta simulada
                    duration.Should().BeNull();  // Verifica que duration sea null si no se especifica
                });

            // Act
            var result = await _weatherService.GetWeatherCities(cityName);

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().HaveCount(1);
            result.First().city_name.Should().Be("Cancún");

            // Verificación de que Add fue llamado una vez
            _mockCache.Verify(cache => cache.Add(It.IsAny<string>(), It.IsAny<object>(), It.IsAny<TimeSpan?>()), Times.Once);
        }

        [Fact]
        public async Task GetWeatherCities_Should_Call_ProcessInParallel_When_CityName_Is_Provided()
        {
            // Arrange
            var cityName = "Cancún";
            var cityCacheKey = $"GetWeatherCities_{cityName}";
            var mockCityResponse = new List<GetWeatherCityResponseDto>
            {
                new GetWeatherCityResponseDto { city_name = "Cancún", lat = "21.161908", @long = "-86.8515279", popularity = "0.0470989330134478" }
            };

            // Configuración del caché
            _mockCache.Setup(cache => cache.TryGet(cityCacheKey, out It.Ref<object>.IsAny)).Returns(false);
            _mockWeatherRepository.Setup(repo => repo.GetWeatherCitiesAsync<GetWeatherCityResponseDto>(cityName))
                .ReturnsAsync(mockCityResponse);

            // Configuración del método ProcessInParallel
            _mockParallelRequest.Setup(parallel => parallel.ProcessInParallel(It.IsAny<IEnumerable<GetWeatherCityResponseDto>>(), It.IsAny<Func<GetWeatherCityResponseDto, Task<GetWeatherCityResponseDto>>>(), 5))
                .Callback<IEnumerable<GetWeatherCityResponseDto>, Func<GetWeatherCityResponseDto, Task<GetWeatherCityResponseDto>>, int>((cities, processFunc, _) =>
                {
                    cities.Should().NotBeNull();
                    cities.Should().HaveCount(1);
                    cities.First().city_name.Should().Be("Cancún");
                    processFunc.Should().NotBeNull();  // Verifica que la función de procesamiento no sea nula
                })
                .ReturnsAsync(mockCityResponse);

            // Act
            var result = await _weatherService.GetWeatherCities(cityName);

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.First().city_name.Should().Be("Cancún");

            // Verificación de que ProcessInParallel fue llamado
            _mockParallelRequest.Verify(parallel => parallel.ProcessInParallel(It.IsAny<IEnumerable<GetWeatherCityResponseDto>>(), It.IsAny<Func<GetWeatherCityResponseDto, Task<GetWeatherCityResponseDto>>>(), 5), Times.Once);
        }

        [Fact]
        public async Task GetWeatherCities_Should_Handle_Exception_When_Repository_Fails()
        {
            // Arrange
            var cityName = "Cancún";
            var cityCacheKey = $"GetWeatherCities_{cityName}";

            // Configuración: El repositorio lanza una excepción
            _mockWeatherRepository.Setup(repo => repo.GetWeatherCitiesAsync<GetWeatherCityResponseDto>(cityName))
                .ThrowsAsync(new Exception("Repository failure"));

            // Act & Assert: Verifica que se maneje la excepción correctamente
            await Assert.ThrowsAsync<Exception>(async () => await _weatherService.GetWeatherCities(cityName));

            // Verificación de que no se haya llamado al servicio en paralelo
            _mockParallelRequest.Verify(parallel => parallel.ProcessInParallel(It.IsAny<IEnumerable<GetWeatherCityResponseDto>>(), It.IsAny<Func<GetWeatherCityResponseDto, Task<GetWeatherCityResponseDto>>>(), 5), Times.Never);
        }

        [Fact]
        public async Task GetWeatherCities_Should_Return_Cached_Cities_When_Already_Cached()
        {
            // Arrange
            var cityName = "Cancún";
            var cityCacheKey = $"GetWeatherCities_{cityName}";
            var cachedCities = new List<GetWeatherCityResponseDto>
            {
                new GetWeatherCityResponseDto { city_name = "Cancún", lat = "21.161908", @long = "-86.8515279", popularity = "0.0470989330134478" }
            };

            // Configuramos TryGet para devolver true y usar Returns para devolver los datos almacenados en caché
            _mockCache.Setup(cache => cache.TryGet(cityCacheKey, out It.Ref<object>.IsAny))
                .Returns(true);

            // Simulamos que la cache devuelve los datos directamente
            _mockCache.Setup(cache => cache.TryGet(It.IsAny<string>(), out It.Ref<object>.IsAny))
                .Returns((string key, out object data) =>
                {
                    data = cachedCities;
                    return true;
                });

            // Act
            var result = await _weatherService.GetWeatherCities(cityName);

            // Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().HaveCount(1);
            result.First().city_name.Should().Be("Cancún");

            // Verificación de que no se llamó al repositorio ni al servicio paralelo
            _mockWeatherRepository.Verify(repo => repo.GetWeatherCitiesAsync<GetWeatherCityResponseDto>(cityName), Times.Never);
            _mockParallelRequest.Verify(parallel => parallel.ProcessInParallel(It.IsAny<IEnumerable<GetWeatherCityResponseDto>>(), It.IsAny<Func<GetWeatherCityResponseDto, Task<GetWeatherCityResponseDto>>>(), 5), Times.Never);
        }
    }
}
