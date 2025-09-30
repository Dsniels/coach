using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Client.RP;
using OpenAI.Realtime;
using workout.abstractions.Entities;

public class ConversationService
{
    private readonly RealtimeClient realtimeClient;
    private readonly Settings settings;
    private readonly ILogger<ConversationService> _logger;
    private RealtimeSession _session;

    public ConversationService(RealtimeClient realtimeClient, Settings settings, ILogger<ConversationService> logger)
    {
        this.realtimeClient = realtimeClient;
        this.settings = settings;
        _logger = logger;
    }

    public async Task Conversation()
    {
        _logger.LogInformation("Deployment {}", settings.openAISettings.Model);
        _logger.LogInformation("Endpoint {}", settings.openAISettings.Endpoint);
        _logger.LogInformation("Key {}", settings.openAISettings.ApiKey);
        _session = await realtimeClient.StartConversationSessionAsync(settings.openAISettings.Model,  cancellationToken: CancellationToken.None);


        ConversationFunctionTool tool = new("save_workout")
        {
            Description = "call this when the user ask you to save a workout",
            Parameters = BinaryData.FromObjectAsJson(new Workout() { }),
        };

        _session.ConfigureSession(new()
        {
            Tools = { tool },

            InputTranscriptionOptions = new()
            {
                Model = settings.openAISettings.Model
            }
        });
        SpeakerOutput speaker = new();
        await foreach (var update in _session.ReceiveUpdatesAsync())
        {
            if (update is ConversationSessionStartedUpdate)
            {
                _ = Task.Run(async () =>
                {
                    _logger.LogInformation("Listening");
                    Microphone microphone = Microphone.Start();
                    await _session.SendInputAudioAsync(microphone);
                });
            }

            if (update is OutputDeltaUpdate deltaUpdate)
            {
                _logger.LogInformation("Transcript: {transcript}", deltaUpdate.AudioTranscript);
                speaker.Queue(deltaUpdate.AudioBytes);
            }

            if (update is ConversationSessionStartedUpdate startedUpdate)
            {
                speaker.Clear();
            }
        }


    }
}