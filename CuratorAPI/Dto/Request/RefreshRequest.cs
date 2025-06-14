namespace CuratorAPI.Dto.Request
{
    public class RefreshRequest
    {
        public string Username { get; set; } = null!;
        public string RefreshToken { get; set; } = null!;
    }
}
