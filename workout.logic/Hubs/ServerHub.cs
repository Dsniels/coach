using System;
using System.Runtime.CompilerServices;
using System.Threading.Channels;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using workout.abstractions.Interfaces.Hub;

namespace workout.logic.Hubs;

public class ServerHub : Hub<IClientHub>
{
    private readonly IModelService modelService;
    private readonly ILogger<ServerHub> logger;

    public ServerHub(IModelService modelService, ILogger<ServerHub> logger)
    {
        this.modelService = modelService;
        this.logger = logger;
    }

    public async IAsyncEnumerable<string> Talk(Message message, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var ch = Channel.CreateUnbounded<string>();
        logger.LogInformation(message.Content);

        this.modelService.AskSomething(ch, message);

        await foreach (var stream in ch.Reader.ReadAllAsync(cancellationToken))
        {
            yield return stream;
        }

    }

}
