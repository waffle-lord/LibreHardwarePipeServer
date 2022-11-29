using LibreHardware_Helper;
using LibreHardwareServer.Interfaces;
using LibreHardwareServer.Model;

namespace LibreHardwareServer.Handlers
{
    internal class DefaultResponseHandler : IResponseHandler
    {
        LibreHardwareHelper _helper;

        public DefaultResponseHandler()
        {
            _helper = new LibreHardwareHelper();
        }

        public ResponseData HandleRequest(string message)
        {
            switch(message)
            {
                case "cpu":
                    {
                        return new ResponseData(_helper.GetCpuData());
                    }
                case "memory":
                    {
                        return new ResponseData(_helper.GetMemoryData());
                    }
                case "gpu":
                    {
                        return new ResponseData(_helper.GetGpuData());
                    }
                case "ping":
                    {
                        return new ResponseData("pong");
                    }
                case "close":
                    {
                        return new ResponseData("goodbye").WithStatus(ResponseStatus.Closing);
                    }
                default:
                    {
                        return new ResponseData("Invalid Request").WithStatus(ResponseStatus.Error);
                    }
            }
        }
    }
}
