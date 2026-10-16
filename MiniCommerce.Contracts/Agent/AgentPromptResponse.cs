namespace MiniCommerce.Contracts.Agent;

/// <summary>Response returned by the agent API.</summary>
public sealed record AgentPromptResponse
{
    public string Response { get; init; } = string.Empty;
    public long InputToken { get; init; }
    public long OutputToken { get; init; }
    public long TotalToken => InputToken + OutputToken;
    public long ReasoningToken { get; init; }
};