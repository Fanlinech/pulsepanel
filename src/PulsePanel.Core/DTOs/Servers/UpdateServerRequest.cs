using System;
using System.Collections.Generic;
using System.Text;

namespace PulsePanel.Core.DTOs.Servers
{
    public class UpdateServerRequest
    {
        public required string Name { get; set; }
        public required string Host { get; set; }
        public string? Description { get; set; }
    }
}
