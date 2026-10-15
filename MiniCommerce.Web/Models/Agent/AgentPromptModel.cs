namespace MiniCommerce.Web.Models.Agent;

public class AgentPromptModel
{
    public string Prompt { get; set; } = string.Empty;
    public Guid UserGuid { get; set; } = Guid.Empty;
}
