// Don't use Global AND file scope usings.
//using Discord;
//using Discord.WebSocket;
//using DiscordBot.Service;

using MajorPayne.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MajorPayne;

public class Program
{
    // since Program is already set up as a class, configuration and setup of objects should be done in a constructor.
    // However seeing as the fields arn't setup for that, I suppose we do everything asyncrenously,
    //withoout persisting any state in the class. Which means we have to instantiate our services inline.
    // however note that this should not ALL take place in your Main.

    //this pattern is a Lambda. Similar to a python Lambda but with drastically different syntax.
    static void Main()
        => MainAsync().GetAwaiter().GetResult();

    #region Main Task
    public static async Task MainAsync()
    {
        /* Stolen from the interwebs somewhere reputable:
         
         * The var keyword was created to handle declarations when the type is not known, 
         * such as generic types, lambdas, and query expressions. 
         * If you already know the type of a variable, you must declare that explicitly. 
         
         * Remember, if you don’t declare a variable explicitly, the compiler must do extra work to determine the type. 
         * While the cost of this operation may not be significant, it’s just unnecessary burden on the compiler.
         */
        //this is an indicator that the programmer is not familar with C#
        //var configuration = new ConfigurationBuilder()

        //this interface will act the way you might want the var keyword to act in the previous attempt.
        // it represents a key, value pair of application configuration properties.
        // it knows how to configurew and retrieve envirnment variable.
        // It also works with more that just this basic ConfigurationBuilder.
        // Anything that inherits the IConfiguration interface.
        IConfiguration configuration = new ConfigurationBuilder()
        .AddEnvironmentVariables(prefix: "!")
        .AddJsonFile("appsettings.json")
        .Build();

        var discordConfig = new DiscordSocketConfig
        {
            // either list these two intents or each individual.
            GatewayIntents = GatewayIntents.All | GatewayIntents.GuildMembers
        };

        // we will hook into events in another file,
        //client.MessageReceived += HandleCommand;

        //instead of including this in every file,
        //we will just instantiate the logging service and forget about the rest.
        //client.Log += Log;

        //but first we need to set up our service container and provider.

        // Create a new Discord client
        //var client = new DiscordSocketClient(discordConfig);
        DiscordSocketClient client = new(discordConfig);

        // if you have a service that needs instantiating before it goes in the container,
        // add it like this.
        InteractionService interactions = new(client);

        //This object is responsible for tracking, intatiating and passing in services that we register.
        IServiceProvider services = new ServiceCollection()
            .AddSingleton(configuration)
            .AddSingleton<OpenAIAPI>(new OpenAIAPI(
                configuration["Settings:OpenAi:key"])) // from json file
            .AddSingleton<DiscordSocketClient>(client)
            .AddSingleton<InteractionHandler>()
            .BuildServiceProvider();
        // this call at the end of the chain, builds a serviceProvider containing the collection that we have set up.
        // this is one reason why you might not want to use var here.
        // It's not clear at a glance of the signature, that it actually produces a provider not a collection.


        // InteractionHandler is initialized in this way because
        // 1. it has a bunch of specific parameters that I don't want to deal with.
        // 2. the actions that take place in initializeAsync need to happen at a specifc spot in the flow of the program.
        // when a service is added to the collection without being instatiated, the provider will instantiate it for use, even populate the parameters given that they are also in the collection.
        await services.GetRequiredService<InteractionHandler>().InitializeAsync();
        // Login to Discord
        await client.LoginAsync(TokenType.Bot, configuration["Settings:Discord:token"]);

        // Start the client
        await client.StartAsync();

        // Block this program until it is closed
        await Task.Delay(-1);
    }
    #endregion
    //This method logs messages to the console
    // Logging service
    //private static Task Log(LogMessage msg)
    //{
    //    Console.WriteLine(msg.ToString());
    //    return Task.CompletedTask;
    //}

    internal static bool IsDebug()
    {
        throw new NotImplementedException();
    }
}