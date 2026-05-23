namespace PulsePanel.Core.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using PulsePanel.Core.DTOs.Servers;
    public interface IServerService
    {
        Task<ServerResponse> CreateAsync(CreateServerRequest request);
        Task<IReadOnlyList<ServerResponse>> GetAllAsync();
        Task<ServerResponse?> GetByIDAsync(Guid id);
        Task<ServerResponse?> UpdateHeartbeatAsync(Guid id);
        
        Task<ServerResponse?> UpdateAsync(Guid id, UpdateServerRequest request);
        Task<bool> DeleteAsync(Guid id);
    }
}