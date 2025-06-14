namespace CuratorAPI.Models
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; } = null!;
        public DateTime Expires { get; set; }
        public bool IsRevoked { get; set; }

        public int CuratorId { get; set; }
        public Curator Curator { get; set; } = null!;
    }

}
