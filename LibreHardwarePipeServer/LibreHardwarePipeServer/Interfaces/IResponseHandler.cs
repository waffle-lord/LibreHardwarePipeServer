using LibreHardwarePipeServer.Model;

namespace LibreHardwarePipeServer.Interfaces
{
    public interface IResponseHandler
    {
        /// <summary>
        /// Handle the incomming request from the pipe client
        /// </summary>
        /// <param name="message"></param>
        /// <returns>The requested response as a json string</returns>
        public string HandleRequest(string message);
    }
}
