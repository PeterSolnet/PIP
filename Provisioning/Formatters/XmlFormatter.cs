using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Provisioning.Commands;


namespace Provisioning.Formatters
{
    public class XML : IResponseFormatter
    {
        public string ContentType
        {
            get { return "application/xml"; }
        }

        public string GetResponse(CommandStatus status)
        {
            string response = string.Empty;

            using (var stream = new MemoryStream())
            {
                var s = new DataContractSerializer(typeof(CommandStatus));
                s.WriteObject(stream, status);
                stream.Seek(0, SeekOrigin.Begin);
                response = new StreamReader(stream).ReadToEnd();
            }

            return response;
        }
    }
}
