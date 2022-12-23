using System.Net;
using Discord.WebSocket;
using Newtonsoft.Json;
using RestSharp;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace DiscordBot.Service;

public class OpenAiService
{
//Url to the ChatGpt Api
    private const string ChatGptApiUrl = "https://api.openai.com/v1/completions";

//Url to the
    private const string DalleApiUrl = "https://api.openai.com/v1/images/generations";


//The method uses the RestClient class to send a request to the ChatGPT API, passing the user's message as the
//prompt and sends the response into the Chat
    internal static async Task<Tuple<bool, string>> ChatGpt(SocketMessage message)
    {
        var configuration = new ConfigurationBuilder()
            .AddEnvironmentVariables(prefix: "CG_")
            .AddJsonFile("appsettings.json")
            .Build();
        // Create a new RestClient instance
        var client = new RestClient(ChatGptApiUrl);

        // Create a new RestRequest instance
        var request = new RestRequest("", Method.Post);

        // Set the request headers
        request.AddHeader("Content-Type", "application/json");
        request.AddHeader("Authorization", $"Bearer {configuration["Settings:Openai:key"]}");


        // Create the request data
        var data = new
        {
            // The prompt is everything after the !chat command
            model = "text-davinci-003",
            prompt = message.Content[5..] + " but write it as if you were a drill sergeant",
            max_tokens = 256
        };
        Console.WriteLine(data);

        var jsonData = JsonSerializer.Serialize(data);

        // Add the request data to the request body
        request.AddJsonBody(jsonData);

        // Send the request and get the response
        var response = await client.ExecuteAsync(request);

        // Holds the response from the API.
        string responseText;
        var success = true;
        // Check the status code of the response
        Console.WriteLine("response.content: " + response.Content);
        if (response.Content != null && response.StatusCode == HttpStatusCode.OK)
        {
            // Get the response text from the API
            responseText = JsonConvert.DeserializeObject<dynamic>(response.Content)?["choices"][0]["text"] ??
                           "Could not deserialize response from ChatGPT Api!";
        }
        else
        {
            // Get the ErrorMessage from the API
            responseText = response.ErrorMessage ?? string.Empty;
            success = false;
        }

        // Send the response to the Discord chat
        await message.Channel.SendMessageAsync(responseText);
        return new Tuple<bool, string>(success, responseText);
    }

//The method uses the RestClient class to send a request to the Dall-E API, passing the user's message as the
//prompt and sends an image to the Chat
    internal static async Task<Tuple<bool, string>> DallE(SocketMessage message)
    {
        var configuration = new ConfigurationBuilder()
            .AddEnvironmentVariables(prefix: "CG_")
            .AddJsonFile("appsettings.json")
            .Build();
        
        // Create a new RestClient instance
        var client = new RestClient(DalleApiUrl);

        // Create a new RestRequest instance
        var request = new RestRequest("", Method.Post);

        // Set the request headers
        request.AddHeader("Content-Type", "application/json");
        request.AddHeader("Authorization", $"Bearer {configuration["Settings:Openai:key"]}");

        // Create the request data
        var data = new
        {
            // The prompt is everything after the !image command
            //model = "image-alpha-001",
            prompt = message.Content[5..],
            n = 1,
            size = "1024x1024"
        };

        var jsonData = JsonSerializer.Serialize(data);

        // Add the request data to the request body
        request.AddJsonBody(jsonData);

        // Send the request and get the response
        var response = await client.ExecuteAsync(request);

        // Holds the response from the API.
        string responseText;
        var success = false;
        // Check the status code of the response
        if (response.Content != null && response.StatusCode == HttpStatusCode.OK)
        {
            // Get the image URL from the API response
            var imageUrl = JsonConvert.DeserializeObject<dynamic>(response.Content)?["data"][0]["url"];
            responseText = $"Here is the generated image: {imageUrl}";
            success = true;
        }
        else
        {
            // Get the ErrorMessage from the API
            responseText = response.ErrorMessage ?? string.Empty;
        }

        // Send the response to the Discord chat
        await message.Channel.SendMessageAsync(responseText);

        return new Tuple<bool, string>(success, responseText);
    }
}