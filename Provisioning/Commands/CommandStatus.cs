using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Provisioning.Commands.Model;

namespace Provisioning.Commands
{
    [DataContract(Name = "Response", Namespace = "http://tempuri.org")]

    public struct CommandStatus

    {

        private static readonly JsonSerializerSettings settings = new JsonSerializerSettings

        {

            Converters = new List<JsonConverter>() { new StringEnumConverter() },

            DateFormatString = "yyyy-MM-ddTHH:mm:ss"

        };



        [DataMember(Name = "Status", Order = 1)]

        public CommandCode StatusCode { get; set; }



        [DataMember(Order = 2)]

        public string Description { get; set; }



        public object Data { get; set; }



        [DataMember(Order = 3)]

        public DateTime Started { get; set; }



        [DataMember(Order = 4)]

        public DateTime Finished { get; set; }



        [DataMember(Order = 5)]

        public Decimal Duration { get; set; }



        [DataMember(Order = 6)]

        public string ErrorCode { get; set; }



        [DataMember(Order = 7)]

        public string ErrorLocation { get; set; }



        [DataMember(Order = 8)]

        public string ErrorMessage { get; set; }



        [DataMember(Order = 9)]

        public string Tag { get; set; }



        public override string ToString()

        {

            // Serialize to JSON, so it's easy to parse later:

            return JsonConvert.SerializeObject(this, Formatting.None, settings);

        }

    }
}
