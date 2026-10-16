using System.Net.Http.Json;
using MiniCommerce.Contracts.Agent;

namespace MiniCommerce.Web.Services.Agent;

public class AgentService(HttpClient httpClient) : IAgentService
{
    public async Task<AgentPromptResponse> SendAsync(AgentPromptRequest request, CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsJsonAsync("agent", request, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<AgentPromptResponse>(cancellationToken)
            ?? new AgentPromptResponse();
    }
}
