

using System.Threading.Channels;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using workout.logic.Options;

public class ModelService : IModelService
{
    private readonly IChatClient _client;
    private readonly Tools _tools;
    private readonly ILogger<ModelService> logger;

    public ModelService(IChatClient client, ILogger<ModelService> logger, Tools tools)
    {
        _tools = tools;
        _client = client;
        this.logger = logger;
    }

    private ChatOptions GetChatOptions()
    {
        return new ChatOptions
        {
            AllowMultipleToolCalls=true,
            Tools = [
                AIFunctionFactory.Create(
                    _tools.CreateWorkout(),
                    "create_workout",
                    "Creates a new workout ONLY when the user requests to create, add, or generate a workout . Do not call this for general questions. Creates a workout with these required fields: Ejercicio (string), Repeticiones (int), Sets (int), if sets are not provided set default to 1, al finalizar solo di los ejercicios agregados y da una recomendacion si consideras que fueron pocas repeticiones y poco peso"
                    ),
                AIFunctionFactory.Create(
                    _tools.GetWorkOutByDate(),
                    "get_workout_by_date",
                    "Usa esta funcion cuando el usuario te pregunte informacion sobre algun ejercicio que realizo en cierta fecha, el usuario puede decirte la fecha completa o un dia relativo al dia de hoy en dado caso tu saca que dia es hoy y continua con la funcion, al final si la funcion retorna null hazle saber al usuario que no encontro registros con esa fecha o con ese nombre, si la funcion regresa informacion dasela al usuario."
                )

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