using System;
using System.Collections.Generic;
using System.Text;

namespace PulsePanel.Core.DTOs.Servers
{
    public class ServerResponse
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
