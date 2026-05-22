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
    }
}