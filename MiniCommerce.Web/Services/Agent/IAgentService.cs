using MiniCommerce.Contracts.Agent;

namespace MiniCommerce.Web.Services.Agent;

public interface IAgentService
{
    Task<AgentPromptResponse> SendAsync(AgentPromptRequest request, CancellationToken cancellationToken);
}
