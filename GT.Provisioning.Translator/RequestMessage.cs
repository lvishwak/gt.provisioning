using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GT.Provisioning.Translator
{
    internal class RequestMessage
    {
        public Int32 Id { get; set; }

        public RequestMessageType MessageType { get; set; }

        public string SystemId { get; set; }

        public string Requester { get; set; }
    }
}
