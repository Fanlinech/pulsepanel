using PulsePanel.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PulsePanel.Core.Services
{
    public class ServerStatusCalculator
    {

        public ServerStatusCalculator() { }
        public ServerStatus GetStatus(DateTime? LastHeartbeatAt, DateTime? LastCheckAt, string? LastCheckMessage)
        {
            if (LastHeartbeatAt is null && LastCheckAt is null)
            {
                return ServerStatus.Unknown;
            }

            return LastHeartbeatAt >= DateTime.UtcNow.AddMinutes(-5) || (LastCheckAt >= DateTime.UtcNow.AddMinutes(-5) && LastCheckMessage == "Connection successful")
                ? ServerStatus.Online
                : ServerStatus.Offline;
        }
    }
}
