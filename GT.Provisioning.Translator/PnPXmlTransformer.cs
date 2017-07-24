using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Xml.Xsl;

namespace GT.Provisioning.Translator
{
    public class PnPXmlTransformer
    {
        public RequestMessage TransformXmlStringWithXslString(string xmlFilePath)
        {
            try
            {
                // read the request xml as string
                String requestXml = File.ReadAllText(xmlFilePath);

                // build request message object
                RequestMessage requestMessage = GetMessage(requestXml);

                // get xslt file
                var xslCompiledTransform = new XslCompiledTransform();
                string xsltFilePath = XsltFile(requestMessage.MessageType);
                xslCompiledTransform.Load(xsltFilePath);

                //process our xml
                XmlTextReader xmlTextReader =
                    new XmlTextReader(new StringReader(requestXml));
                XPathDocument xPathDocument = new XPathDocument(xmlTextReader);

                //handle the output stream
                StringBuilder stringBuilder = new StringBuilder();
                TextWriter textWriter = new StringWriter(stringBuilder);

                //do the transform
                xslCompiledTransform.Transform(xPathDocument, null, textWriter);
                requestMessage.Request = GenerateStreamFromString(stringBuilder.ToString());

                return requestMessage;
            }
            catch (Exception exception)
            {
                throw exception;
            }           
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
                    throw new ArgumentOutOfRangeException(nameof(messageType));
            }

            return xsltFile;
        }

        private RequestMessage GetMessage(string xmlString)
        {
            byte[] encodedString = Encoding.UTF8.GetBytes(xmlString);
            MemoryStream sourceStream = new MemoryStream(encodedString);
            sourceStream.Flush();
            sourceStream.Position = 0;

            XElement xElement = XElement.Load(sourceStream);
            var requestMessage = new RequestMessage
            {
                Id = Int32.Parse(xElement.Attribute("id").Value),
                MessageType = (RequestMessageType)Enum.Parse(typeof(RequestMessageType), xElement.Attribute("type").Value),
                SystemId = xElement.Attribute("systemId").Value,
                Requester = xElement.Attribute("requestor").Value
            };

            return requestMessage;
        }

        private Stream GenerateStreamFromString(string xmlString)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(xmlString);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}