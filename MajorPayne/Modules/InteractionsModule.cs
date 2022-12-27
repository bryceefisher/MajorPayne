// For the time being, we will keep all types of commands in this file. This may change if too many commands pile up in here, or we sttart to notice some clear seperating boundries that we can sort by.

using Discord.Net;

namespace MajorPayne.Modules;

public class InteractionsModule : InteractionModuleBase<SocketInteractionContext>
{
    readonly InteractionService _commands;
    readonly IConfiguration _config;
    readonly OpenAIAPI _openAi;

    //OpenAiAPI is the Library we use to communicate with the AI model. It has enough functionality,
    // so that there is no reason for us to need to create any kind of wrapper around it. We will just use the library.
    public InteractionsModule(IConfiguration config, InteractionService commands, OpenAIAPI openAi)
    {
        _commands = commands;
        _config = config;
        _openAi = openAi;
       
    }
    #region Slash Commands
    // ============= Add more command / interactions here =============
    [SlashCommand("ping", "ping your bot")]
    public async Task GreetUserAsync()
            => await RespondAsync(text: $":ping Y'rself Y'all: It took me {Context.Client.Latency}ms to respond to you!", ephemeral: true);

    #endregion
}

