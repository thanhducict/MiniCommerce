namespace Agent.Api.Agents;

public class AgentModel
{
    public string Prompt { get; set; } = string.Empty;
    public Guid UserGuid { get; set; }
}
