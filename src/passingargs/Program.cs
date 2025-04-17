using Microsoft.SemanticKernel;
using Microsoft.Extensions.Configuration;

class Program
{
    static async Task Main(string[] args)
    {
        var (deploymentName, endpoint, apiKey) = ReadSettings();
        // Initialize the kernel
        var kernel = Kernel.CreateBuilder()
            .AddAzureOpenAIChatCompletion(
                deploymentName: deploymentName,
                endpoint: endpoint,
                apiKey: apiKey)
            .Build();

        // Create a simple prompt
        var prompt = """
                    You are a helpful knowledge base about world cities.
                    Keep the summary to 2 facts.
                    Give some interesting facts about {{$city}}
                    """;

        // Run the prompt
        var arguments = new KernelArguments { ["city"] = "London" };
        var cityFunction = kernel.CreateFunctionFromPrompt(prompt);

        var result = await kernel.InvokeAsync(cityFunction, arguments);
    
        Console.WriteLine(result);

        await ProcessImagePrompt(kernel);
    }

    static async Task ProcessImagePrompt(Kernel kernel)
    {
        var prompt = "You are a image assitant. Describe the image in {{$imageData}}";
        var arguments = new KernelArguments(new Dictionary<string, object?>
        {
            {"imageData", "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAoAAAAKCAYAAACNMs+9AAAAAXNSR0IArs4c6QAAACVJREFUKFNj/KTO/J+BCMA4iBUyQX1A0I10VAizCj1oMdyISyEAFoQbHwTcuS8AAAAASUVORK5CYII="}
        });
        var imagePromptFunction = kernel.CreateFunctionFromPrompt(prompt);
        var response = await kernel.InvokeAsync(imagePromptFunction, arguments);

        Console.WriteLine($"Response: {response}");
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