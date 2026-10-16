using Agent.Api.SessionStore;
using Google.GenAI;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using MiniCommerce.Contracts.Agent;

var builder = WebApplication.CreateBuilder(args);

// The agent is stateless, so build it once and share it across all users.
builder.Services.AddSingleton<ChatClientAgent>(_ =>
{
    var apiKey = Environment.GetEnvironmentVariable("Gemini__ApiKey")
        ?? throw new InvalidOperationException("Gemini__ApiKey is not configured.");
    const string model = "gemini-3.5-flash-lite";

    return new ChatClientAgent(
        new Client(vertexAI: false, apiKey: apiKey).AsIChatClient(model),
        instructions: "You are a good assistant.",
        tools:
        [
        ]);
});

// Sessions are per user and must outlive a single request, so keep them in a singleton store.
builder.Services.AddSingleton<IAgentSessionStore, InMemoryAgentSessionStore>();

builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseHttpsRedirection();

app.MapPost("/agent", async (
    AgentPromptRequest request,
    ChatClientAgent agent,
    IAgentSessionStore sessionStore,
    CancellationToken cancellationToken) =>
{
    if (string.IsNullOrWhiteSpace(request?.Prompt))
    {
        return Results.BadRequest("Prompt is required.");
    }

    if (request.UserGuid == Guid.Empty)
    {
        return Results.BadRequest("UserGuid is required.");
    }

    var chatMessage = new ChatMessage(ChatRole.User, request.Prompt);

    var chatClientAgentRunOptions = new ChatClientAgentRunOptions
    {
        ChatOptions = new ChatOptions
        {
            Reasoning = new ReasoningOptions
            {
                Effort = ReasoningEffort.Low,
                Output = ReasoningOutput.Full
            },
            MaxOutputTokens = 1024
        }
    };

    var session = await sessionStore.GetOrCreateAsync(
        request.UserGuid,
        async ct => await agent.CreateSessionAsync(ct),
        cancellationToken);
    var response = await agent.RunAsync(chatMessage, session, chatClientAgentRunOptions, cancellationToken: cancellationToken);

    return Results.Ok(new AgentPromptResponse {
        Response = response.ToString() ?? string.Empty,
        InputToken = response.Usage?.InputTokenCount ?? 0,
        OutputToken = response.Usage?.OutputTokenCount ?? 0,
        ReasoningToken = response.Usage?.ReasoningTokenCount ?? 0
    });
});

app.MapHealthChecks("/health");
app.Run();
