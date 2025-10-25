namespace Zerphin.RentACar.Domain.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
public class RequireRoleAttribute : Attribute
{
    public string[] Roles { get; }

    public RequireRoleAttribute(params string[] roles)
    {
        Roles = roles;
    }
}
