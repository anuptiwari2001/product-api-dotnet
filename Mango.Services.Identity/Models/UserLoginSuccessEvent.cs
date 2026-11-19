namespace Mango.Services.Identity.Models;

public class UserLoginSuccessEvent
{
    public string Username { get; }
    public string SubjectId { get; }
    public string DisplayName { get; }
    public string ClientId { get; }

    public UserLoginSuccessEvent(string username, string subjectId, string displayName, string clientId)
    {
        Username = username;
        SubjectId = subjectId;
        DisplayName = displayName;
        ClientId = clientId;
    }
}
