using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PulsePanel.Core.DTOs.Servers
{
    public class CreateServerRequest
    {
        [Required]
        [MinLength(2)]
        [MaxLength(100)]
        public required string Name { get; set; }
        [Required]
        [MaxLength(255)]
        public required string Host { get; set; }
        [MaxLength(500)]
        public string? Description { get; set; }
        [Range(1, 65535)]
        public ushort CheckPort { get; set; } = 80;
    }
}
