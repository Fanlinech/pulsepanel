namespace PulsePanel.Core.Entities
{
    public class Server
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Host { get; set; }
        public ushort CheckPort { get; set; } = 80;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        // Server send signal for panel
        public DateTime? LastHeartbeatAt { get; set; }
        // Panel check server
        public DateTime LastCheckAt { get; set; }
        public string? LastCheckMessage { get; set; }
        public int LastResponseTimeMs {  get; set; }
        public ServerStatus Status { get; set; } = ServerStatus.Unknown;
    }
}
