using System.Collections.Generic;
using System.Linq;
using TinyJson;

namespace MD.Net
{
    public class DeviceManager : IDeviceManager
    {
        public DeviceManager(IToolManager toolManager)
        {
            this.ToolManager = toolManager;
        }

        public IToolManager ToolManager { get; private set; }

        public IEnumerable<IDevice> GetDevices()
        {
            var output = default(string);
            var error = default(string);
            var process = this.ToolManager.Start(Tools.NETMDCLI);
            var code = this.ToolManager.Exec(process, out output, out error);
            return this.GetDevices(output);
        }

        protected IEnumerable<IDevice> GetDevices(string output)
        {
            if (string.IsNullOrEmpty(output) || output.Contains(Constants.NETMDCLI_NO_DEVICE, true) || output.Contains(Constants.NETMDCLI_ERROR, true))
            {
                return Enumerable.Empty<IDevice>();
            }
            //Only a single device is supported.
            var device = output.FromJson<_Device>();
            return new[]
            {
                new Device(device.device)
            };
        }

        private struct _Device
        {
            public string device;
        }
    }
}
