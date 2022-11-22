﻿using System.Text.Json;

namespace LibreHardwarePipeServer.Model
{
    public class ResponseData
    {
        public ResponseStatus Status { get; set; }
        public object Data { get; set; }

        public ResponseData(object data)
        {
            Data = data ?? "Failed to get data";
            Status = data != null ? ResponseStatus.OK : ResponseStatus.ERROR;
        }

        public ResponseData WithStatus(ResponseStatus status)
        {
            Status = status;
            return this;
        }

        public string? Serialize()
        {
            try
            {
                return JsonSerializer.Serialize(this);
            }
            catch
            {
                return null;
            }
        }
    }
}
