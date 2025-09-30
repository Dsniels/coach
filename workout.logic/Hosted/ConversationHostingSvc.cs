using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class ConversationHostedSvc : BackgroundService
{
    public ConversationHostedSvc(ConversationService conversationService, ILogger<ConversationHostedSvc> logger)
    {
        ConversationService = conversationService;
        Logger = logger;
    }

    public ConversationService ConversationService { get; }
    public ILogger<ConversationHostedSvc> Logger { get; }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await ConversationService.Conversation();
    }
}