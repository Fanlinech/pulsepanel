using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Net;
using System.Text;

namespace PulsePanel.Core.DTOs.Common
{
    public class ErrorResponse
    {
        public string? Message { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public DateTime TimeStamp { get; set; }
        public string? Path { get; set; }
    }
}
