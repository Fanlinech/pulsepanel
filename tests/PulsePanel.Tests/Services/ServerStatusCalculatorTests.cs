using PulsePanel.Core;
using PulsePanel.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace PulsePanel.Tests.Services
{
    public class ServerStatusCalculatorTests
    {
        
        [Fact]
        public void Test_Nulable_Type()
        {
            var calcStatus = new ServerStatusCalculator();

            var status = calcStatus.GetStatus(null, null, null);

            Assert.Equal(ServerStatus.Unknown, status);
        }

        [Fact]
        public void Test_UtcNow()
        {
            var calcStatus = new ServerStatusCalculator();

            var status = calcStatus.GetStatus(DateTime.UtcNow, null, null);

            Assert.Equal(ServerStatus.Online, status);
        }

        [Fact]
        public void Test_UtcNow_Minus_Thrity_Second()
        {
            var calcStatus = new ServerStatusCalculator();

            var status = calcStatus.GetStatus(DateTime.UtcNow.AddSeconds(-30), null, null);

            Assert.Equal(ServerStatus.Online, status);
        }

        [Fact]
        public void Test_UtcNow_Minus_Six_Minute()
        {
            var calcStatus = new ServerStatusCalculator();

            var status = calcStatus.GetStatus(DateTime.UtcNow.AddMinutes(-6), null, null);

            Assert.Equal(ServerStatus.Offline, status);
        }
        
        [Fact]
        public void Test_UtcNow_Minus_Five_Minute()
        {
            var calcStatus = new ServerStatusCalculator();

            var status = calcStatus.GetStatus(DateTime.UtcNow.AddMinutes(-5), null, null);

            Assert.Equal(ServerStatus.Offline, status);
        }

        [Fact]
        public void Test_Recent_Successful_Check_Without_Heartbeat()
        {
            var calcStatus = new ServerStatusCalculator();

            var status = calcStatus.GetStatus(null, DateTime.UtcNow.AddMinutes(-1), "Connection successful");

            Assert.Equal(ServerStatus.Online, status);
        }

        [Fact]
        public void Test_Recent_Failed_Check_Without_Heartbeat()
        {
            var calcStatus = new ServerStatusCalculator();

            var status = calcStatus.GetStatus(null, DateTime.UtcNow.AddMinutes(-1), "Connection error");

            Assert.Equal(ServerStatus.Offline, status);
        }

        [Fact]
        public void Test_Recent_Heartbeat_Overrides_Failed_Check()
        {
            var calcStatus = new ServerStatusCalculator();

            var status = calcStatus.GetStatus(DateTime.UtcNow.AddMinutes(-1), DateTime.UtcNow.AddMinutes(-1), "Connection error");

            Assert.Equal(ServerStatus.Online, status);
        }

        [Fact]
        public void Test_Old_Heartbeat_With_Recent_Successful_Check()
        {
            var calcStatus = new ServerStatusCalculator();

            var status = calcStatus.GetStatus(DateTime.UtcNow.AddMinutes(-10), DateTime.UtcNow.AddMinutes(-1), "Connection successful");

            Assert.Equal(ServerStatus.Online, status);
        }

        [Fact]
        public void Test_Recent_Check_Without_Success_Message()
        {
            var calcStatus = new ServerStatusCalculator();

            var status = calcStatus.GetStatus(null, DateTime.UtcNow.AddMinutes(-1), null);

            Assert.Equal(ServerStatus.Offline, status);
        }
    }
}
