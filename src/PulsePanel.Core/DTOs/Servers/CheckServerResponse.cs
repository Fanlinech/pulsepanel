using System;
using System.Collections.Generic;
using System.Text;

namespace PulsePanel.Core.DTOs.Servers
{
    public class CheckServerResponse
    {
        public Guid ServerId { get; set; }
        public required string Host { get; set; }
        public ushort Port { get; set; }
        public bool IsAvailable { get; set; }
        public ServerStatus Status { get; set; } = ServerStatus.Unknown;
        public DateTime CheckedAt { get; set; }
        public int ResponseTimeMs { get; set; }
        public string? Message { get; set; }
    }
}
