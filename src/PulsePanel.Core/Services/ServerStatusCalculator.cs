using PulsePanel.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace PulsePanel.Core.Services
{
    public class ServerStatusCalculator
    {

        public ServerStatusCalculator() { }
        public ServerStatus GetStatus(DateTime? LastHeartbeatAt)
        {
            if (LastHeartbeatAt is null)
            {
                return ServerStatus.Unknown;
            }

            return LastHeartbeatAt >= DateTime.UtcNow.AddMinutes(-5)
                ? ServerStatus.Online
                : ServerStatus.Offline;
        }
    }
}
