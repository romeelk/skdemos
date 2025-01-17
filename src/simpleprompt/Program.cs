using Microsoft.SemanticKernel;
using Microsoft.Extensions.Configuration;

class Program
{
    static async Task Main(string[] args)
    {
        var settings = ReadSettings();
        // Initialize the kernel
        var kernel = Kernel.CreateBuilder()
            .AddAzureOpenAIChatCompletion(
                deploymentName: settings.deploymentName,
                endpoint: settings.endpoint,
                apiKey: settings.apiKey)
            .Build();

        // Create a simple prompt
        const string promptTemplate = """
        <message role="system">You are a trivia bot. Return a fun fact about the input in json format using the key of term being asked.</message>
        <message role ="user">Tell me a fun fact about {{$input}}</message>
        """;
        var promptFunction = kernel.CreateFunctionFromPrompt(promptTemplate);

        // Run the prompt
        var result = await kernel.InvokeAsync(promptFunction, new() { ["input"] = "cat" });
        
        Console.WriteLine(result);
    }
    private static (string deploymentName, string endpoint, string apiKey) ReadSettings()
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        return (
            config["AzureOpenAI:DeploymentName"] ?? throw new Exception("DeploymentName not found"),
            config["AzureOpenAI:Endpoint"] ?? throw new Exception("Endpoint not found"),
            config["AzureOpenAI:ApiKey"] ?? throw new Exception("ApiKey not found")
        );
    }
}