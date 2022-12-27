using Discord;
using Microsoft.VisualBasic;

namespace MajorPayne.Handlers;

public class InteractionHandler
{
    // Private Fields
    private readonly DiscordSocketClient _client;
    private readonly InteractionService _interacts;
    private readonly IServiceProvider _provider;
    private readonly IConfiguration _config;

    public InteractionHandler(DiscordSocketClient client, InteractionService interaction, IServiceProvider provider, IConfiguration config)
    {
        _client = client;
        _provider = provider;
        _interacts = interaction;
        _config = config;
    }

    public async Task InitializeAsync()
    {
        // Process when the client is ready, so we can register our commands.
        _client.Ready += ReadyAsync;
        _interacts.Log += LogAsync;
        // Add the public modules that inherit InteractionModuleBase<T> to the InteractionService
        await _interacts.AddModulesAsync(Assembly.GetEntryAssembly(), _provider);

        // Process the InteractionCreated payloads to execute Interactions commands
        _client.InteractionCreated += HandleInteraction;
        _client.MessageCommandExecuted += _client_MessageCommandExecuted;
    }
    // TODO write this
    private async Task _client_MessageCommandExecuted(SocketMessageCommand message)
    {
        try
        {
            // Create an execution context that matches the generic type parameter of your InteractionModuleBase<T> modules.
            var context = new SocketInteractionContext(_client, message);

            // Execute the incoming command.
            var result = await _interacts.ExecuteCommandAsync(context, _provider);
            Console.WriteLine(result.ToString());
            if (!result.IsSuccess)
                Console.WriteLine(message);

        switch (message)
        {
            case { } chat when chat.StartsWith("!chat"):
                await Log(new LogMessage(LogSeverity.Info, nameof(HandleCommand),
                    "Received !chat command: " + message.Content));
                (success, responseText) = await OpenAiService.ChatGpt(message);
                break;
            case { }
image when image.StartsWith("!image"):
                await Log(new LogMessage(LogSeverity.Info, nameof(HandleCommand),
                    "Received !image command: " + message.Content));
                (success, responseText) = await OpenAiService.DallE(message);


                break;
            default:
                await Log(new LogMessage(LogSeverity.Info, nameof(HandleCommand),
                    "No command found, normal message" + message));
                break;
        }

        if (!success)
            await Log(new LogMessage(LogSeverity.Warning, nameof(HandleCommand),
                "Error with one of the request to the Apis!"));

        if (!string.IsNullOrEmpty(responseText))
            await Log(new LogMessage(LogSeverity.Info, nameof(HandleCommand),
                "Response: " + responseText));
    }
}

private Task LogAsync(LogMessage log)
{
    Console.WriteLine(log);
    return Task.CompletedTask;
}

private async Task ReadyAsync()
{
    // Context & Slash commands can be automatically registered, but this process needs to happen after the client enters the READY state.
    // Since Global Commands take around 1 hour to register, we should use a test guild to instantly update and test our commands.
    if (Program.IsDebug())
        await _interacts.RegisterCommandsToGuildAsync(000000000000); // TODO change this
    else
        await _interacts.RegisterCommandsGloballyAsync(true);
}

private async Task HandleInteraction(SocketInteraction interaction)
{
    try
    {
        // Create an execution context that matches the generic type parameter of your InteractionModuleBase<T> modules.
        var context = new SocketInteractionContext(_client, interaction);

        // Execute the incoming command.
        var result = await _interacts.ExecuteCommandAsync(context, _provider);
        Console.WriteLine(result.ToString());
        if (!result.IsSuccess)
            switch (result.Error)
            {
                case InteractionCommandError.UnmetPrecondition:
                    // implement
                    break;
                default:
                    break;
            }
    }
    catch
    {
        // If Slash Command execution fails it is most likely that the original interaction acknowledgement will persist. It is a good idea to delete the original
        // response, or at least let the user know that something went wrong during the command execution.
        if (interaction.Type is InteractionType.ApplicationCommand)
            await interaction.GetOriginalResponseAsync().ContinueWith(async (msg) => await msg.Result.DeleteAsync());
    }
}
}
// private static async Task HandleCommand(SocketMessage message)
//    {
//        // This is bad practice.
//        //var success = true;

//        var responseText = string.Empty;

//await Log(new LogMessage(LogSeverity.Info, nameof(HandleCommand),
//            "New Message incoming..."));
        //TODO move this to a handler.
        // Check if the message starts with one of these commands
        // this should not be handled in the Program file, little lone the MainTask.
 
