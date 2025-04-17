namespace WeatherPlugin_UnitTests;
using WeatherPluginDemo.Plugins;
using NUnit.Framework;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Given_NoApiKey_When_CreatingWeatherPlugin_Then_ThrowsArgumentException()
    {   
        string apiKey = string.Empty;
        Assert.Throws<ArgumentNullException>(() => new WeatherPlugin(apiKey));
    }

    [Test]
    public async Task Given_InvalidKey_Then_ThrowsException()
    {
        ReadEnvironment.LoadEnvironmentVariables();
        var apiKey = "Invalid";
        var api = new WeatherPlugin(apiKey);
        Assert.ThrowsAsync<Exception>(async () => await api.GetWeatherDataByCityNameAsync("London"));
    }
    [Test]
    public async Task Given_CountryName_Then_ReturnsWeatherData()
    {
        ReadEnvironment.LoadEnvironmentVariables();
        var apiKey = System.Environment.GetEnvironmentVariable("OPENWEATHERMAP_API_KEY");
        var weatherPlugin = new WeatherPlugin(apiKey);
        var weatherData = await weatherPlugin.GetWeatherDataByCityNameAsync("London");

        Assert.IsNotNull(weatherData?.Description);
        Assert.That(weatherData?.City, Is.EqualTo("London"));
        Assert.That(weatherData?.Country, Is.EqualTo("GB"));
    }

    [Test]
    public async Task Given_InvalidCityName_Then_ReturnsNoWeatherData()
    {
        ReadEnvironment.LoadEnvironmentVariables();
        var apiKey = System.Environment.GetEnvironmentVariable("OPENWEATHERMAP_API_KEY");
        var weatherPlugin = new WeatherPlugin(apiKey);
        var weatherData = await weatherPlugin.GetWeatherDataByCityNameAsync("UnknownCity");

        Assert.IsNull(weatherData);
    }
}
