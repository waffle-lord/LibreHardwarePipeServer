using LibreHardwareServer.Model;

namespace LibreHardwareServer.Interfaces
{
    public interface IResponseHandler
    {
        /// <summary>
        /// Handle the incomming request from the pipe client
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public ResponseData HandleRequest(string message);
    }
}
