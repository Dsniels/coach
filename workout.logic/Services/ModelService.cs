

using System.Threading.Channels;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using workout.logic.Options;

public class ModelService : IModelService
{
    private readonly IChatClient _client;
    private readonly Tools _tools;
    private readonly ILogger<ModelService> logger;
    private IList<ChatMessage> History = new List<ChatMessage> {
        new ChatMessage(ChatRole.System,
        $$""" 
                esta es una lista de los ejercicios existentes, basate de esta para saber como se llaman y como se eescriben  los ejercicios que el usuario mencionaEntrenamiento del tren inferior

                Sentadilla (tradicional): flexión de cadera y rodillas para descender tu peso, trabaja piernas y core .
                Sentadilla búlgara: elevas una pierna en un banco o silla, trabaja de forma unilateral .
                Sentadilla estilo sumo: postura amplia enfocada en aductores y abductores .
                Sentadilla sissy: variante exigente que resalta los cuádriceps .
                Pistol squat (sentadilla a una pierna): equilibrio y fuerza en una sola pierna .
                Sentadilla con salto: pliométrica, potencia y quema calórica .
                Sentadilla isométrica: mantener la posición para intensificar esfuerzo .
                Zancada tradicional (lunge): alterna trabajas glúteos, cuádriceps e isquiotibiales .
                Zancadas cruzadas hacia atrás: más énfasis en glúteos .
                Zancadas laterales: focalizan aductores y abductores .
                Zancadas con salto (scissor lunge): potencia, fuerza y quema calórica .
                Glute bridge: elevación de pelvis para glúteos e isquiotibiales .
                Hip thrust: variante intensa del puente, se puede añadir peso .
                Salto del patinador: trabaja piernas, glúteos y core con desplazamiento lateral .
                Bunny hop (salto del conejo): salto simultáneo de ambas piernas en sentadilla .
                Duck walks (marcha de pato): caminar en cuclillas profundas, piernas y core .
                Forward ape: caminas con manos en el suelo y saltas para volver pies a las manos .

                Entrenamiento del tren superior

                Flexiones (push‑ups): clásicas para pectoral, brazos y core .
                Flexiones diamante: manos formando un diamante, enfocadas en pectoral mayor y tríceps .
                Flexiones con palmada: potencia y explosividad .
                Flexiones espartanas: desalineación de manos para trabajar fuerza unilateral .
                Flexiones supinas: con manos orientadas hacia afuera para trabajar más la espalda .
                Flexiones hindúes: combinan pectorales, hombros, tríceps y core .
                Flexiones delfín: inicio en V invertida, hombros y core activados .
                Flexiones verticales invertidas: inversión del cuerpo para trabajar hombros intensamente .
                Superman: extensiones lumbares tumbado para espalda baja .
                Swimming (pilates): similar al “superman”, movimiento controlado para lumbares .
                Fondos (dipping): tríceps usando silla o banco .

                Ejercicios para el core

                Plank (plancha tradicional): estabilización horizontal, trabaja el core en profundidad .
                Plank lateral: énfasis en oblicuos .
                Plank lateral con torsión: agrega rotación para mayor intensidad .
                Plank invertido: activa glúteos, isquiotibiales y core .
                Plank con rodillas al pecho: añade movimiento para mayor gasto calórico .
                Plank lateral dinámico: con movimiento de cadera enfocado en oblicuos .
                Body saw: balanceo de avance y retroceso imitando una sierra .
                Dead bug: tumbado, alterna brazos y piernas para activar toda la zona media .
                Flutter kicks: aleteo de piernas para abdomen inferior .
                Cruce alterno de piernas: lleva una pierna horizontalmente sobre la otra, trabaja oblicuos .
                Ciclista (abdominales cruzados): emula pedaleo, oblicuos y abdomen trabajados .
                Roll to candlestick: similar a movimiento usado en cross‑fit .
                Hundred (Pilates): técnica pilates para activar respiración y core .
                Hollow body (balanceo en hollow): posición intensa para abdomen profundo .

                Ejercicios compuestos y cardio corporal

                Burpees: trabajo de cuerpo completo, cardio y fuerza .
                Mountain climbers (escaladores): core y tren inferior en movimiento constante .
                Burroll (reverse burpees): variante de burpee con movimiento invertido .
                Bear crawl: gateo que exige hombros, brazos y core .
                Broad jump: salto largo horizontal, potencia total .
                Jumping jacks: salto estelar para cardio y coordinación .
                Saltamontes (grasshoppers): variante de escalador desde posición de flexión . 
                """) };

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
            AllowMultipleToolCalls = true,
            Tools = [
                AIFunctionFactory.Create(
                    _tools.CreateWorkout(),
                    "create_workout",
                    "Creates a new workout ONLY when the user requests to create, add, or generate a workout . Do not call this for general questions. Creates a workout with these required fields: Ejercicio (string), Repeticiones (int), Sets (int), if sets are not provided set default to 1, al finalizar solo di los ejercicios agregados y da una recomendacion si consideras que fueron pocas repeticiones y poco peso"
                    ),
                AIFunctionFactory.Create(
                    _tools.GetWorkOutByDate(),
                    "get_workout_by_date",
                    "Usa esta función cuando el usuario pregunte cuántos ejercicios hizo en una fecha específica. El usuario puede mencionar la fecha de forma absoluta (“el 18 de julio de 2025”) o relativa (“ayer”, “el lunes”, “hace tres días”). Convierte fechas relativas a una fecha DateTime absoluta usando el día actual. Si el nombre del ejercicio es ambiguo (“cuántas hice”), trata de inferir a qué se refiere (“flexiones”, “abdominales”, etc.) en base al contexto. Si no encuentras ejercicios, responde amablemente que no hay registros ese día."
                    )

                    ],

        };
    }


    public async Task AskSomething(Channel<string> channel, Message message)
    {
        logger.LogInformation("Sending...");
        logger.LogInformation(message.Content);
        var messages = new ChatMessage(ChatRole.User, message.Content);
        var response = "";
        try
        {
            History.Add(messages);
            await foreach (var stream in _client.GetStreamingResponseAsync(History, GetChatOptions()))
            {
                await channel.Writer.WriteAsync(stream.Text);
                response += stream.Text;
            }
        }
        finally
        {

            channel.Writer.Complete();

            History.Add(new ChatMessage(ChatRole.Assistant, response));
        }
    }
}