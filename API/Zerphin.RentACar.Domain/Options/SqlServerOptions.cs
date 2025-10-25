namespace Zerphin.RentACar.Domain.Options;

public class SqlServerOptions
{
    public const string SectionName = "SqlServer";
    
    public string ConnectionString { get; set; } = string.Empty;
}
