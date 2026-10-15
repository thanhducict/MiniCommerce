using Microsoft.Agents.AI;

namespace Agent.Api.Agents;

// Keeps one long-lived conversation session per (agent, user) so follow-up
// prompts continue the same conversation instead of starting from scratch.
public interface IAgentSessionStore
{
    Task<AgentSession> GetOrCreateAsync(
        Guid userGuid,
        Func<CancellationToken, Task<AgentSession>> factory,
        CancellationToken cancellationToken = default);
}
