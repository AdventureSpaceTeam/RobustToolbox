using System;
using Robust.Shared.Console;
using System.Management;
using System.Text;

namespace Robust.Shared.Network
{
    internal static class HWId
    {
        private static string? GetWmi(string wmi_class, string property)
        {
            var mbs = new ManagementObjectSearcher($"Select {property} From {wmi_class}");
            ManagementObjectCollection mbsList = mbs.Get();
            foreach (ManagementObject mo in mbsList)
            {
                var id = mo[property].ToString();
                if (id != null)
                    return id;
            }
            return null;
        }
        public static byte[] Calc()
        {
            if (OperatingSystem.IsWindows())
            {
                var processorId = GetWmi("Win32_Processor", "ProcessorId");
                var motherboardSerial = GetWmi("Win32_BaseBoard", "SerialNumber");
                return Encoding.ASCII.GetBytes(processorId + motherboardSerial);
            }

            return Array.Empty<byte>();
        }
    }

#if DEBUG
    internal sealed class HwidCommand : LocalizedCommands
    {
        public override string Command => "hwid";

        public override void Execute(IConsoleShell shell, string argStr, string[] args)
        {
            shell.WriteLine(Convert.ToBase64String(HWId.Calc(), Base64FormattingOptions.None));
        }
    }
#endif
}
