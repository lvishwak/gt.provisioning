using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace GT.Provisioning.Translator
{
    public class PnPXmlTransformer
    {
        public Stream TransformXml(Stream xmlDoc)
        {
            // get message type from request
            RequestMessageType requestMessageType = GetMessageType(xmlDoc);

            // get xslt file
            var xslt = new XslCompiledTransform();
            string xsltFilePath = XsltFile(requestMessageType);
            xslt.Load(xsltFilePath);

            XPathDocument xmlDocument = new XPathDocument(xmlDoc);
            var streamWriter = new StreamWriter(Console.OpenStandardOutput());
            streamWriter.AutoFlush = true;
            xslt.Transform(xmlDocument, null, streamWriter);

            return streamWriter.BaseStream;
        }

        private string XsltFile(RequestMessageType messageType)
        {
            string xsltFile = string.Empty;

            switch (messageType)
            {
                case RequestMessageType.CreateTaxSymphonyPost:
                    xsltFile = "migratelibrary-request-xml.xslt";
                    break;
                case RequestMessageType.CreateTaxSymphonyPre:
                    break;
                case RequestMessageType.SetSiteGroupMembers:
                    xsltFile = "setsitegroupmembers-request-xml.xslt";
                    break;
                case RequestMessageType.SetSiteName:
                    xsltFile = "setsitename-request-xml.xslt";
                    break;
                case RequestMessageType.SetSiteStatus:
                    xsltFile = "setsitestatus-request-xml.xslt";
                    break;
                default:
                    break;
            }

            return xsltFile;
        }

        private RequestMessageType GetMessageType(Stream sourceStream)
        {
            //byte[] encodedString = Encoding.UTF8.GetBytes(requestXml);
            //MemoryStream sourceStream = new MemoryStream(encodedString);
            //sourceStream.Flush();
            //sourceStream.Position = 0;

            XElement xElement = XElement.Load(sourceStream);
            var requestMessage = new RequestMessage
            {
                Id = Int32.Parse(xElement.Attribute("id").Value),
                MessageType = (RequestMessageType)Enum.Parse(typeof(RequestMessageType), xElement.Attribute("type").Value),
                SystemId = xElement.Attribute("systemId").Value,
                Requester = xElement.Attribute("requestor").Value
            };

            return requestMessage.MessageType;
        }
    }
}
