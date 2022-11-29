using Newtonsoft.Json;

namespace LibreHardwareServer.Model
{
    public class ResponseData
    {
        public ResponseStatus Status { get; set; }
        public object Data { get; set; }

        [JsonIgnore]
        public string SerializedData { get; private set; }

        public ResponseData(object data)
        {
            Data = data ?? "Failed to get data";
            Status = data != null ? ResponseStatus.OK : ResponseStatus.Error;
        }

        public ResponseData WithStatus(ResponseStatus status)
        {
            Status = status;
            return this;
        }

        public string Serialize(string failedToSerializeMessage = "Serialization failed")
        {
            try
            {
                var serialized = JsonConvert.SerializeObject(this, new JsonSerializerSettings { Formatting = Formatting.Indented });
                SerializedData = serialized;
                return SerializedData;
            }
            catch
            {
                Data = failedToSerializeMessage;
                Status = ResponseStatus.Error;
                var serialized = JsonConvert.SerializeObject(this, new JsonSerializerSettings { Formatting = Formatting.Indented });
                SerializedData = serialized;
                return SerializedData;
            }
        }
    }
}
