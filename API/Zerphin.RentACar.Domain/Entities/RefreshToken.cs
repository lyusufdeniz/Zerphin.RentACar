namespace Zerphin.RentACar.Domain.Entities;

public class RefreshToken : BaseEntity
{
    public string Token { get; set; } = string.Empty;
    
    public DateTime ExpiresAt { get; set; }
    
    public int UserId { get; set; }
    
    public virtual User User { get; set; } = null!;
    
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
}
