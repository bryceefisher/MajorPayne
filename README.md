# Discord Bot *(with OpenAI [ChatGPT & DALL-E])*

This is a simple Discord bot that uses the OpenAI ChatGPT API to generate responses to messages in a Discord chat. The
bot is written in C# and uses the [Discord.Net](https://github.com/discord-net/Discord.Net) library to interact with the
Discord API and the [RestSharp](https://github.com/restsharp/RestSharp) library to make HTTP requests to the OpenAI API.

## Installation

To use this bot, you will need the following:

- A Discord bot token
- Openai key

Replace Settings:Discord:token and Settings:Openai:key in appsettings.json with your actual Discord bot token and ChatGPT API key, respectively. appsettings.json is located in MajorPayne > bin > Debug > net6.0.

## Usage

To use the bot, send a message in the form of `!chat <message>` or `!image <message>` where `<message>` is the text you
want the bot to generate a response for. For example:

ChatGPT:

`!chat What is the meaning of life?`

DALL-E:

`!image Pixel art where monkeys trying to rob a bank`

The bot will respond with a generated image/text response based on the prompt you provided.

## License

This project is licensed under the MIT License. See [LICENSE](https://github.com/omgitsjan/DiscordBot/blob/main/LICENSE)
for details.
