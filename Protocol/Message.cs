using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;

namespace Protocol
{
    // We will serialize and deserialize this data from JSON
    public class Message
    {
        public Guid?     FromId          { get; set; }
        public string?   FromUsername    { get; set; }
        // public Guid?     ToId            { get; set; }
        //public string?   ToUsername      { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public Command?  Command         { get; set; }
        public byte[]?   Data            { get; set; }

        public string GetStringData()
        {
            return System.Text.Encoding.UTF8.GetString(Data!);
        }
    }
}
