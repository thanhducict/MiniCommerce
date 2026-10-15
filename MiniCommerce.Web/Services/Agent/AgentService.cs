using System.Net.Http.Json;
using MiniCommerce.Web.Models.Agent;

namespace MiniCommerce.Web.Services.Agent;

public class AgentService(HttpClient httpClient) : IAgentService
{
    public async Task<AgentResponse> SendAsync(AgentPromptModel request, CancellationToken cancellationToken)
    {
        var response = await httpClient.PostAsJsonAsync("agent", request, cancellationToken);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<AgentResponse>(cancellationToken)
            ?? new AgentResponse { Response = string.Empty };
    }
}
