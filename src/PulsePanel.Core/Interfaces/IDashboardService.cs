using PulsePanel.Core.DTOs.Dashboard;
using System;
using System.Collections.Generic;
using System.Text;

namespace PulsePanel.Core.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardSummaryResponse> GetSummaryAsync();
    }
}
