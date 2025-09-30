

using System.Threading.Channels;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Agents.OpenAI;
using OpenAI.Assistants;
using workout.logic.Options;

public class ModelService : IModelService
{
    private readonly ILogger<ModelService> logger;
    private readonly OpenAIAssistantAgent _agent;
    private readonly IServiceProvider _serviceProvider;

    public ModelService(AssistantClient assistantClient, ILogger<ModelService> logger, Settings settings, IServiceProvider serviceProvider)
    {
        var OpenAISettings = settings.openAISettings;
        _serviceProvider = serviceProvider;
        this.logger = logger;
        var assistant = assistantClient.CreateAssistant(OpenAISettings.Model, GetAssistantOptions());
        _agent = new OpenAIAssistantAgent(assistant, assistantClient, GetPlugins());
    }


    public async Task AskSomething(Channel<string> channel, Message message)
    {

    }

    private AssistantCreationOptions GetAssistantOptions()
    {
        var instructions = File.ReadAllText("prompt.txt");
        return new()
        {
            Instructions = instructions,
            Name = "coach",
        };
    }

    private IList<KernelPlugin> GetPlugins()
    {
        return new List<KernelPlugin>()
        {
            KernelPluginFactory.CreateFromType<WorkoutsPlugin>(serviceProvider:_serviceProvider),
        };
    }
}