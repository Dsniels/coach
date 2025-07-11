

using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Channels;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using workout.abstractions.Entities;
using workout.abstractions.Interfaces;

public class ModelService : IModelService
{
    private readonly IChatClient _client;
    private readonly ILogger<ModelService> logger;
    private readonly IWorkoutRepository _workoutRepository;

    public ModelService(IChatClient client, ILogger<ModelService> logger, IWorkoutRepository workoutRepository)
    {
        _client = client;
        this.logger = logger;
        _workoutRepository = workoutRepository;
    }

    private ChatOptions GetChatOptions()
    {
        return new ChatOptions
        {
            Tools = [
                AIFunctionFactory.Create(async (IList<WorkoutDto> dtos)=>{
                    try
                    {
                    var res = 0;
                    foreach(var dto in dtos){
                        var workout = new Workout{
                            Ejercicio = dto.Ejercicio,
                            Sets = dto.Sets,
                            Repeticiones = dto.Repeticiones,
                        };
                        workout.UserId = "testss";
                        res += await _workoutRepository.CreateWorkoutAsync(workout);
                    }
                    return res;

                    }
                    catch (System.Exception e)
                    {
                        logger.LogError(e.Message);
                        return 0;
                    }
                },"crear_entrenamiento","Creates a new workout ONLY when the user requests to create, add, or generate a workout . Do not call this for general questions. Creates a workout with these required fields: Ejercicio (string), Repeticiones (int), Sets (int), if sets are not provided set default to 1, al finalizar solo di los ejercicios agregados y da una recomendacion si consideras que fueron pocas repeticiones y poco peso")
            ],

        };
    }


    public async Task AskSomething(Channel<string> channel, Message message)
    {
        logger.LogInformation("Sending...");
        logger.LogInformation(message.Content);
        var messages = new List<ChatMessage> { new ChatMessage(ChatRole.User, message.Content) };
        try
        {

            await foreach (var stream in _client.GetStreamingResponseAsync(messages, GetChatOptions()))
            {
                await channel.Writer.WriteAsync(stream.Text);
            }
        }
        finally
        {
            channel.Writer.Complete();

        }
    }
}