using System.Net.Http.Json;
using Newtonsoft.Json.Linq;
using WeatherPlugin;

Console.WriteLine("Plugin that can call OpenWeatherMap API to get weather information");

try
{
    Console.WriteLine("Reading city coordinates from file");

    if(Environment.GetEnvironmentVariable("OPENWEATHERMAP_API_KEY") == null)
    {
        Console.WriteLine("Please set the OPENWEATHERMAP_API_KEY environment variable");
        return;
    }
    var locations = ReadCîtyCoordinates();

    foreach(var item in locations)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"City: {item.Value.City}, Lat: {item.Value.Lat}, Lon: {item.Value.Lon}, Country: {item.Value.Country}");
    }
    Console.WriteLine("Attempt to lookup city coordinates for London and then weather");

    if(locations.TryGetValue("London", out Location? value))
    {
        Console.WriteLine($"City: {value.City}, Lat: {value.Lat}, Lon: {value.Lon}, Country: {value.Country}");

        var weather = await GetWeather(value.Lat, value.Lon);

        Console.WriteLine(weather);
    }
    else
    {
        Console.WriteLine("City not found in the list of locations");
    }  
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
        Temparture = parsedWeatherInfo["main"]?["temp"]?.Value<double>() ?? 0,
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