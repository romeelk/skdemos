using System.Net.Http.Json;
using Newtonsoft.Json.Linq;
using Microsoft.SemanticKernel;
using Microsoft.Extensions.Configuration;

using WeatherPluginDemo;
using WeatherPluginDemo.Plugins;
using Microsoft.SemanticKernel.Connectors.OpenAI;

Console.WriteLine("Plugin that can call OpenWeatherMap API to get weather information");

try
{
    var settings = ReadSettings();
    
    var kernel =  Kernel.CreateBuilder()
        .AddAzureOpenAIChatCompletion(
            deploymentName: settings.deploymentName,
            endpoint: settings.endpoint,
            apiKey: settings.apiKey)
    .Build();

    Console.WriteLine("Reading city coordinates from file");

    var weatherApi = new WeatherPlugin(settings.weatherApiKey);

    var weatherInfo = weatherApi.GetWeatherDataByCityNameAsync("London").GetAwaiter().GetResult();

    Console.WriteLine("Calling Weather API directly to get weather information for London");

    Console.WriteLine("Current temperature in London is: " + weatherInfo?.Temperature + "°C");

    var locations = ReadCîtyCoordinates();

    foreach(var item in locations)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"City: {item.Value.City}, Lat: {item.Value.Lat}, Lon: {item.Value.Lon}, Country: {item.Value.Country}");
    }

    Console.WriteLine("Loading WeatherPlugin into the kernel");

    kernel.Plugins.AddFromObject(new WeatherPlugin(settings.weatherApiKey),nameof(WeatherPlugin));

    var promptQuery = @"What is the current Weather in {{$city}}. Use the WeatherPlugin";

    var promptFunction = kernel.CreateFunctionFromPrompt(promptQuery,new OpenAIPromptExecutionSettings(){ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions});
    
    Console.WriteLine("Enter a city to query the LLM for current weather");
    var city = Console.ReadLine();
    var result = await kernel.InvokeAsync(promptFunction,new KernelArguments() { ["city"] = city});
    Console.WriteLine(result);
}
catch (HttpRequestException ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}

static async Task<WeatherInfo> GetWeather(double lat, double lon)
{
    /*https://api.openweathermap.org/data/2.5/weather?lat=44.34&lon=10.99&appid={API key}*/
    var apiKey = System.Environment.GetEnvironmentVariable("OPENWEATHERMAP_API_KEY");
    HttpClient client = new();
    client.BaseAddress = new Uri("http://api.openweathermap.org/");
    var response = await client.GetAsync($"data/2.5/weather?lat={lat}&lon={lon}&appid={apiKey}&units=metric");
    var weather =  await response.Content.ReadAsStringAsync();
    var parsedWeatherInfo = JObject.Parse(weather);
    var weatherInfo = new WeatherInfo() {
        Lat = parsedWeatherInfo["coord"]?["lat"]?.Value<double>() ?? 0,
        Lon = parsedWeatherInfo["coord"]?["lon"]?.Value<double>() ?? 0,
        Temperature = parsedWeatherInfo["main"]?["temp"]?.Value<double>() ?? 0,
        Description = parsedWeatherInfo["weather"]?.FirstOrDefault()?["description"]?.Value<string>() ?? "No description available"
    };

    return weatherInfo;
}

static async Task<(double, double)> GetLocationCoordinates(string location)
{
    List<Location> locations = [];
    var apiKey = System.Environment.GetEnvironmentVariable("OPENWEATHERMAP_API_KEY");
    /*http://api.openweathermap.org/geo/1.0/direct?q={city name},{state code},{country code}&limit={limit}&appid={API key}*/
    HttpClient client = new();
    client.BaseAddress = new Uri("http://api.openweathermap.org/");
    var response = await client.GetAsync($"geo/1.0/direct?q={location}&limit=1&appid={apiKey}");

    if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
    {
        throw new HttpRequestException(HttpRequestError.UserAuthenticationError.ToString());
    }
    else if (response.StatusCode == System.Net.HttpStatusCode.OK)
    {
        locations = await response.Content.ReadFromJsonAsync<List<Location>>();
        Console.WriteLine("Retrieved location coordinates for city: " + location);
    }
    // Get the location coordinates
    return (locations[0].Lat, locations[0].Lon);
}

static Dictionary<string,Location> ReadCîtyCoordinates()
{
    var fileReader = new StreamReader("locations.csv");
    var locations = new Dictionary<string,Location>();

    fileReader.ReadLine(); // Skip the header column
    while(!fileReader.EndOfStream)
    {
        var line = fileReader.ReadLine();
        var values = line?.Split(',');
        if (!double.TryParse(values[2], out double lat) || !double.TryParse(values[3], out double lon))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error parsing coordinates for city: {values[0]}");
            continue;
        }
        var location = new Location() { City = values[0] ?? "unknown", Country = values[1] ?? "unknown", Lat = lat, Lon = lon };
        locations.Add(location.City,location);
    }
    return locations;
}

static (string deploymentName, string endpoint, string apiKey, string weatherApiKey) ReadSettings()
{
    var config = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .Build();

    return (
        config["AzureOpenAI:DeploymentName"] ?? throw new Exception("DeploymentName not found"),
        config["AzureOpenAI:Endpoint"] ?? throw new Exception("Endpoint not found"),
        config["AzureOpenAI:ApiKey"] ?? throw new Exception("ApiKey not found"),
        config["OpenWeatherMap:ApiKey"] ?? throw new Exception("OpenWeatherMap ApiKey not found")
    );
}