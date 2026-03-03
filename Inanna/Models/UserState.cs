namespace Inanna.Models;

public sealed class UserState
{
    public UserState(Guid id, string login, string email, DateTimeOffset expired)
    {
        Id = id;
        Login = login;
        Email = email;
        Expired = expired;
    }

    public Guid Id { get; }
    public string Login { get; }
    public string Email { get; }
    public DateTimeOffset Expired { get; }
}
