using System;
using System.Collections.Generic;
using System.Text;

namespace PulsePanel.Core.DTOs.Dashboard
{
    public class DashboardSummaryResponse
    {
        public int TotalServers { get; set; } = 0;
        public int OnlineServers { get; set; } = 0;
        public int OfflineServers { get; set; } = 0;
        public int UnknownServers { get; set; } = 0;
        public DateTime? LastHeartbeatAt { get; set; }
    }
}
