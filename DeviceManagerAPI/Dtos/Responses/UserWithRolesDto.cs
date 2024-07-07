namespace DevicesApp.Dtos.Responses;

public class UserWithRolesDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public IList<string> Roles { get; set; }
}
