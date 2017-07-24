using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GT.Provisioning.Translator
{
    public class RequestMessage
    {
        public Int32 Id { get; set; }

        public RequestMessageType MessageType { get; set; }

        public string SystemId { get; set; }

        public string Requester { get; set; }

        public Stream Request { get; set; }
    }
}
