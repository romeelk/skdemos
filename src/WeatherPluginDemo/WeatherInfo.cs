using System;

namespace WeatherPluginDemo;

public class WeatherInfo
{
    public string? City { get; set; }
    public double Lat { get; set; }
    public double Lon { get; set; }
    public double Temperature { get; set; }
    public string? Description { get; set; }
    public string? Country { get; set; }
}
