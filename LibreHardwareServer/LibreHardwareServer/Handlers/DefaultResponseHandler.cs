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

        public string HandleRequest(string message)
        {
            switch(message)
            {
                case "cpu":
                    {
                        return GetCpuDataResponse();
                    }
                case "memory":
                    {
                        return GetMemoryDataResponse();
                    }
                case "gpu":
                    {
                        return GetGpuDataResponse();
                    }
                default:
                    {
                        return GetResponseData("Invalid Request");
                    }
            }
        }

        private string GetResponseData(object data)
        {
            var response = new ResponseData(data).Serialize();

            if (response == null)
            {
                var error = new ResponseData("Failed to serialize data").WithStatus(ResponseStatus.ERROR).Serialize();

                if (error == null)
                {
                    return "unknown error";
                }

                return error;
            }

            return response;
        }

        private string GetCpuDataResponse()
        {
            return GetResponseData(_helper.GetCpuData());
        }

        private string GetMemoryDataResponse()
        {
            return GetResponseData(_helper.GetMemoryData());
        }

        private string GetGpuDataResponse()
        {
            return GetResponseData(_helper.GetGpuData());
        }
    }
}
