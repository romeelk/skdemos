namespace WeatherPlugin_UnitTests;

public static class ReadEnvironment {

    public static void LoadEnvironmentVariables() 
    {
        // Load the .env file
        var envFile = File.ReadAllLines(".env");

        // split the lines by the '=' character
        foreach (var line in envFile)
        {
            var splitLine = line.Split('=',StringSplitOptions.RemoveEmptyEntries);
            Environment.SetEnvironmentVariable(splitLine[0], splitLine[1]);
        }
    }
}