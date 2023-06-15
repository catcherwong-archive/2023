using Azure.AI.OpenAI;
using Azure;
using AzureOpenAISample;

string endpoint = Utils.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT");
string key = Utils.GetEnvironmentVariable("AZURE_OPENAI_KEY");

OpenAIClient client = new(new Uri(endpoint), new AzureKeyCredential(key));

Console.WriteLine("======normal========");
await Normal(client);
Console.WriteLine("======normal========");

Console.WriteLine("======stream========");
await Stream(client);
Console.WriteLine("======stream========");

Console.WriteLine("ok");

static async Task Normal(OpenAIClient client)
{
    var chatCompletionsOptions = new ChatCompletionsOptions()
    {
        Messages =
        {
            new ChatMessage(ChatRole.System, "You are a helpful assistant."),
            new ChatMessage(ChatRole.User, "帮忙写一个招聘.NET开发工程师的JD，包含岗位职责，任职要求以及福利"),
        },
        MaxTokens = 1000
    };

    Response<ChatCompletions> response = await client.GetChatCompletionsAsync(
        deploymentOrModelName: "cw-gpt35",
        chatCompletionsOptions);

    Console.WriteLine(response.Value.Choices[0].Message.Content);
}

static async Task Stream(OpenAIClient client)
{
    var chatCompletionsOptions = new ChatCompletionsOptions()
    {
        Messages =
        {
            new ChatMessage(ChatRole.System, "You are a helpful assistant."),
            new ChatMessage(ChatRole.User, "帮忙写一个招聘.NET开发工程师的JD，包含岗位职责，任职要求以及福利"),
        },
        MaxTokens = 1000
    };

    Response<StreamingChatCompletions> response = await client.GetChatCompletionsStreamingAsync(
        deploymentOrModelName: "cw-gpt35",
        chatCompletionsOptions);
    using StreamingChatCompletions streamingChatCompletions = response.Value;

    await foreach (StreamingChatChoice choice in streamingChatCompletions.GetChoicesStreaming())
    {
        await foreach (ChatMessage message in choice.GetMessageStreaming())
        {
            Console.Write(message.Content);
        }
        Console.WriteLine();
    }
}