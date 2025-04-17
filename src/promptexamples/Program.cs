using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Plugins.Core;


IConfiguration configuration = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables()
    .AddCommandLine(args)
    .Build();

static (string? azoaEndpoint, string? azoaApiKey, string? aoaiModelId, string? azoaDeployedModel) GetConfig(IConfiguration config)
{
    var azoaEndpoint = config["azoaEndpoint"];
    var azoaApikey = config["azoaApiKey"];
    var azoaModelId = config["azoaModelId"];
    var azoaDeployedModel = config["azoaDeployedModel"];
    return (azoaEndpoint, azoaApikey, azoaModelId, azoaDeployedModel);
}

var (azoaEndpoint, azoaApikey, azoaModel, azoaDeployedModel) = GetConfig(configuration);

var builder = Kernel.CreateBuilder();
builder.Services.AddAzureOpenAIChatCompletion(
    deploymentName: azoaDeployedModel,
    endpoint: azoaEndpoint,
    apiKey: azoaApikey,
    modelId: azoaModel);

builder.Plugins.AddFromType<TimePlugin>();
builder.Plugins.AddFromType<ConversationSummaryPlugin>();

var kernel = builder.Build();

try
{
    // await PromptWithHistory(kernel);
    // await PromptTravelPersona(kernel);
    // await PromptSystemMessage(kernel);
    //await ZeroShotPrompt(kernel);
    //await FewShotPrompt(kernel);
    await ChainOfThought(kernel);
}
catch (Exception exception)
{
    System.Console.WriteLine($"Oops an error occured {exception.ToString()}");
}

static async Task PromptWithHistory(Kernel kernel)
{
    string language = "Python";
    string history = @"I am keen programming student wanting to know more about python";
    // Assign a persona to the prompt
    string prompt = @$"
    You are AI Python programming assistant. You are helpful, creative, 
    and very friendly. Consider the prograammers's background:
    ${history}

    Create a list of helpful phrases and words in 
    ${language} a programmer would find useful.

    Group phrases by category. Include common Python 
    statementss. Display the statements in the following format: 
    Python:

    Begin with: 'Here are some statments in ${language} 
    you may find helpful:' 
    and end with: 'I hope this helps you Python Learning'";

    Console.WriteLine($"Prompt history: {history}");
    Console.WriteLine($"Prompt:{prompt}");
    var result = await kernel.InvokePromptAsync(prompt);
    Console.WriteLine($"Prompt response: {result}");
}

static async Task PromptTravelPersona(Kernel kernel)
{
    string input = @"I'm planning an anniversary trip with my 
    spouse. We like hiking, mountains, and beaches. Our 
    travel budget is $15000";

    string prompt = @$"
    The following is a conversation with an AI travel 
    assistant. The assistant is helpful, creative, and 
    very friendly.

    <message role=""user"">Can you give me some travel 
    destination suggestions?</message>

    <message role=""assistant"">Of course! Do you have a 
    budget or any specific activities in mind?</message>

    <message role=""user"">${input}</message>";

    Console.WriteLine($"Prompt :{prompt}");
    var result = await kernel.InvokePromptAsync(prompt);
    Console.WriteLine($"Prompt response with Travel Persona Role: {result}");
}

static async Task PromptSystemMessage(Kernel kernel)
{
    var input = @$"I want to learn the Javascript language. Please summarise 
    for me a learning path";

    string prompt = @$"
    <message role=""system"">The following is a conversation with an AI Programming 
    assistant. The assistant is helpful
    very friendly.</message>

    <message role=""user"">${input}</message>";

    Console.WriteLine($"Prompt {prompt}");
    var result = await kernel.InvokePromptAsync(prompt);
    Console.WriteLine($"Prompot response: {result}");
}

/*
*  Unguided with no context given
*/
static async Task ZeroShotPrompt(Kernel kernel)
{
    string request = "I want to get my expense done today";
    string prompt = $"""
    Instructions: What is the intent of this request?
    If you don't know the intent, don't guess; instead respond with "Unknown".
    Choices: CreateExpense, MakePayment, SendExpense,  Unknown.
    User Input: {request}
    Intent: 
    """;
    Console.WriteLine($"Prompt {prompt}");
    var result = await kernel.InvokePromptAsync(prompt);
    Console.WriteLine($"Prompot response: {result}");
}

/*
*  Guided with specific examples with expected inputs and outputs
*/
static async Task FewShotPrompt(Kernel kernel)
{
    string request = "When i get back from the conference i need to sort out my hotel and flight expense";
   string prompt = $"""
Instructions: What is the intent of this request?
If you don't know the intent, don't guess; instead respond with "Unknown".
Choices: CreateExpense, MakePayment, SendExpense,, Unknown.

User Input: Can you please approve my patment for my expense claim
Intent: MakePayment

User Input: I need to claim omy Hotel payment
Intent: CreateExpense

User Input: {request}
Intent:
""";
    Console.WriteLine($"Prompt {prompt}");
    var result = await kernel.InvokePromptAsync(prompt);
    Console.WriteLine($"Prompot response: {result}");
}

/*
* LLMs do not really reason like Humans and can be thrown by simple logical reasoning tasks.
 Chain of thought is a approach to guide the LLM to break down the prompt ask into a series of steps.
*/
static async Task ChainOfThought(Kernel kernel)
{
   string prompt = $"What is the value of 3 + 4 + 19 - 12?";
    Console.WriteLine($"Prompt {prompt}");
    var kernelArgs = new KernelArguments{
        {"temperature",   1.8}
    };

    var result = await kernel.InvokePromptAsync(prompt,kernelArgs);
    Console.WriteLine($"Prompot response: {result}");
}