using System.Net;
using Newtonsoft.Json;
using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace WeatherPluginDemo.Plugins
{
    public class WeatherPlugin
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        public WeatherPlugin(string apiKey)
        {
            if (string.IsNullOrEmpty(apiKey))
            {
                throw new ArgumentNullException(nameof(apiKey));
            }
            _apiKey = apiKey;
            _httpClient = new HttpClient(){
                BaseAddress = new Uri("https://api.openweathermap.org/data/2.5/") 
            };
        }

        [KernelFunction]
        [Description("Get weather data by city name")]
        public async Task<WeatherInfo?> GetWeatherDataByCityNameAsync([Description("city")]string city)
        {   
            var request = $"weather?q={city}&appid={_apiKey}&units=metric";
            var apiResponse = await GetApiResponse(request);
            if(apiResponse == null)
            {
                return null;
            }
            return apiResponse;
        }

        private async Task<WeatherInfo?> GetApiResponse(string request)
        {
            WeatherInfo weatherData;
            try 
            {
                var response = await _httpClient.GetAsync(request);
                
                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    return null;
                }
                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new HttpRequestException("Failed to authenticat to weather API");
                }
                var json = await response.Content.ReadAsStringAsync();

                dynamic? weather = JsonConvert.DeserializeObject(json) ?? throw new Exception("Failed to deserialize weather data");
                weatherData = new WeatherInfo(){Description=weather.weather[0].description, Temperature=weather.main.temp, City=weather.name, Country=weather.sys.country};
            }
            catch(Exception weatherApiException)
            {
                throw new Exception("Error fetching data from OpenWeather API.", weatherApiException);
            }

            return weatherData;
        }
    }
}
