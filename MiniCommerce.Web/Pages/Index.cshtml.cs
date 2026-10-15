using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.JsonWebTokens;
using MiniCommerce.Web.Models.Agent;
using MiniCommerce.Web.Services.Agent;

namespace MiniCommerce.Web.Pages;

public class IndexModel(IAgentService agentService) : PageModel
{
    [BindProperty]
    public AgentPromptModel AiAgentModel { get; set; } = new();

    public void OnGet()
    {
        var userId = User.FindFirst(JwtRegisteredClaimNames.NameId)?.Value
            ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (Guid.TryParse(userId, out var userGuid))
        {
            AiAgentModel.UserGuid = userGuid;
        }
    }

    public async Task<IActionResult> OnPostSendAsync([FromBody] AgentPromptModel request, CancellationToken cancellationToken)
    {
        if (request?.UserGuid == Guid.Empty)
        {
            return BadRequest("You need to log in.");
        }

        if (string.IsNullOrWhiteSpace(request?.Prompt))
        {
            return BadRequest("Prompt is required.");
        }

        var result = await agentService.SendAsync(request, cancellationToken);

        return new JsonResult(new { response = result.Response });
    }
}
