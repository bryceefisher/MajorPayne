
namespace MajorPayne.Services;

public class LoggingService
{

    // making these readonly, private field is the best performance option.
    // While properties are more flexable, Public and show up on intellisense,
    // they do have a little bit more overhead.
    readonly DiscordSocketClient _client;
    readonly InteractionService _command;
    readonly OpenAIAPI _ai;

    public LoggingService(DiscordSocketClient client, InteractionService command, OpenAIAPI ai)
    {
        _client = client;
        _command = command;
        _ai = ai;

        // Hooks,
        // These listen for Events provided by the Discord api.
        client.Log += LogAsync;
        command.Log += LogAsync;
        client.MessageReceived += Client_MessageReceived;

        async Task Client_MessageReceived(SocketMessage arg)
        {

            Console.WriteLine("Recieved: " + arg.Content);
            //await File.AppendAllTextAsync("./Prompts/convo.txt", arg.Content);
            await Task.CompletedTask;
        }

        async Task LogAsync(LogMessage message)
        {
            if (message.Exception is CommandException cmdException)
            {
                Console.WriteLine($"[Command/{message.Severity}] {cmdException.Command.Aliases[0]}"
                    + $" failed to execute in {cmdException.Context.Channel}.");
                Console.WriteLine(cmdException);
            }
            else
                Console.WriteLine($"[General/{message.Severity}] {message}");

            await Task.CompletedTask;
        }
    }

    private Task Command_SlashCommandExecuted(SlashCommandInfo arg1, IInteractionContext arg2, Discord.Interactions.IResult arg3)
    {
        Console.WriteLine($"{arg1.Name} was triggered");
        return Task.CompletedTask;
    }
    // gets anoying.
    //async Task Client_PresenceUpdated(SocketUser arg1, SocketPresence arg2, SocketPresence arg3)
    //{
    //    //Console.WriteLine($"{arg1.Username} is now {arg3.Status} but was previously {arg2.Status}");
    //        await File.AppendAllTextAsync("./Prompts/log.txt", $"{arg1.Username} is now {arg3.Status}");
    //    await Task.CompletedTask;
    //}
}

