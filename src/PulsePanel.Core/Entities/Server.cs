namespace PulsePanel.Core.Entities
{
    public class Server
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Host { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastHeartbeatAt { get; set; }
        public ServerStatus Status { get; set; } = ServerStatus.Unknown;
    }
}
