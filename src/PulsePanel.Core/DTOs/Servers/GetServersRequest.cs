using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PulsePanel.Core.DTOs.Servers
{
    public class GetServersRequest
    {
        [MaxLength(100)]
        public string? Search { get; set; }

        [RegularExpression("(?i)^(name|host|createdAt|status|lastHeartbeatAt)$")]
        public string? SortBy { get; set; } = "createdAt";

        [RegularExpression("(?i)^(asc|desc)$")]
        public string? SortDirection { get; set; } = "desc";

    }
}
