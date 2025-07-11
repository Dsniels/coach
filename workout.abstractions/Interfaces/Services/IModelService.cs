using System.Threading.Channels;

public interface IModelService
{
    Task AskSomething(Channel<string> channel, Message message);
}