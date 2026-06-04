using PulsePanel.Core.DTOs.Servers;
using System;
using System.Collections.Generic;
using System.Text;

namespace PulsePanel.Core.Interfaces
{
    public interface IServerCheckService
    {
        Task<CheckServerResponse?> CheckAsync(Guid ServerId);
    }
}
