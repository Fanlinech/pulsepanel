using System;
using System.Collections.Generic;
using System.Text;

namespace PulsePanel.Core.Options
{
    public class ServerCheckOptions
    {
        public bool Enabled = true;
        public int IntervalSeconds { get; set; } = 60;
        public int TimeoutSeconds { get; set; } = 3;

    }
}
