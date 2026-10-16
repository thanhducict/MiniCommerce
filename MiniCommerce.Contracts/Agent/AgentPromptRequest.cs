namespace MiniCommerce.Contracts.Agent;

/// <summary>Request payload for sending a prompt to the agent API.</summary>
public sealed record AgentPromptRequest
{
    public string Prompt { get; init; } = string.Empty;
    public Guid UserGuid { get; init; }
}