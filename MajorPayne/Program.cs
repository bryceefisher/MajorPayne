using Discord;
using Discord.WebSocket;
using DiscordBot.Service;

namespace DiscordBot;

public class Program
{
    //This is the Discord token from the bot - (Replace this with your Discord bot token)

    


    //Init

    private static void Main()
    {
        MainAsync().GetAwaiter().GetResult();
    }


    //Main Program

    public static async Task MainAsync()
    {
            var configuration = new ConfigurationBuilder()
            .AddEnvironmentVariables(prefix: "CG_")
            .AddJsonFile("appsettings.json")
            .Build();
        
            
        //Creates a config with specified gateway intents
        var config = new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.MessageContent
        };

        // Create a new Discord client
        var client = new DiscordSocketClient(config);

        // Log messages to the console
        client.Log += Log;

        // Handle messages received
        client.MessageReceived += HandleCommand;

        // Login to Discord
        await client.LoginAsync(TokenType.Bot, configuration["Settings:Discord:token"]);

        // Start the client
        await client.StartAsync();

        // Block this program until it is closed
        await Task.Delay(-1);
    }
    


    //This method is called whenever a message is received


    private static async Task HandleCommand(SocketMessage message)
    {
        var success = true;
        var responseText = string.Empty;

        await Log(new LogMessage(LogSeverity.Info, nameof(HandleCommand),
            "New Message incoming..."));

        // Check if the message starts with one of these commands
        switch (message.Content)
        {
            case { } chat when chat.StartsWith("!chat"):
                await Log(new LogMessage(LogSeverity.Info, nameof(HandleCommand),
                    "Received !chat command: " + message.Content));
                (success, responseText) = await OpenAiService.ChatGpt(message);
                break;
            case { } image when image.StartsWith("!image"):
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

    //This method logs messages to the console

    private static Task Log(LogMessage msg)
    {
        Console.WriteLine(msg.ToString());
        return Task.CompletedTask;
    }
}