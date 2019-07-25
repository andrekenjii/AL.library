using AL.library.Utility.Validation;
using System;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace AL.library.Utility.Tools
{
    public static class XmlTools
    {
        private static readonly Type[] WriteTypes = { typeof(string), typeof(DateTime), typeof(Enum), typeof(decimal), typeof(Guid) };

        public static T DeserializeObject<T>(string xml) where T : new()
        {
            if (string.IsNullOrEmpty(xml))
            {
                return new T();
            }
            try
            {
                using (var stringReader = new StringReader(xml))
                {
                    var serializer = new XmlSerializer(typeof(T));
                    return (T)serializer.Deserialize(stringReader);
                }
            }
            catch
            {
                return new T();
            }
        }

        public static string SerializeObject<T>(T dataObject, XmlSerializerNamespaces ns = null)
        {
            if (dataObject == null)
            {
                return string.Empty;
            }
            try
            {
                using (StringWriter stringWriter = new UTF8StringWriter())
                {
                    var serializer = new XmlSerializer(typeof(T));
                    serializer.Serialize(stringWriter, dataObject, ns);
                    return stringWriter.ToString();
                }
            }
            catch
            {
                return string.Empty;
            }
        }

        //public static void Sign(ref XmlElement node, X509Certificate2 certificate, string referenceNode = null)
        //{
        //    if (certificate == null)
        //        throw new ArgumentNullException("certificate is required");

        //    var reference = new Reference();

        //    reference.Uri = "#" + referenceNode;

        //    try
        //    {
        //        reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
        //        reference.AddTransform(new XmlDsigC14NTransform());

        //        var signedXml = new SignedXml(node);
        //        signedXml.SigningKey = certificate.PrivateKey;
        //        signedXml.AddReference(reference);
        //        signedXml.ComputeSignature();

        //        var keyInfo = new KeyInfo();
        //        keyInfo.AddClause(new KeyInfoX509Data(certificate));
        //        signedXml.KeyInfo = keyInfo;
        //        var element = signedXml.GetXml();
        //        node.AppendChild(element);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}        

        public static void SignNode(ref XmlElement node, X509Certificate2 certificate, string signMethod = null)
        {
            var formatter = new RSAPKCS1SignatureFormatter(certificate.PrivateKey);
            if (!string.IsNullOrEmpty(signMethod))
                formatter.SetHashAlgorithm(signMethod);

            var bytes = new ASCIIEncoding().GetBytes(node.InnerXml);
            var hash = new SHA1CryptoServiceProvider().ComputeHash(bytes);
            node.InnerXml = Convert.ToBase64String(formatter.CreateSignature(hash));
        }

        public static void Sign(ref XmlElement node, X509Certificate2 certificate, string referenceNode = null, string signMethod = null)
        {
            AssertionConcern.AssertArgumentNotNull(certificate, "certificate is required");
            var reference = new Reference();
            if ((referenceNode == null))
                reference.Uri = "";
            else
                reference.Uri = "#" + referenceNode;

            try
            {
                reference.AddTransform(new XmlDsigEnvelopedSignatureTransform());
                reference.AddTransform(new XmlDsigC14NTransform());

                var signedXml = new SignedXml(node);

                if (signMethod.ToEnum<HashAlgorithmType>() == HashAlgorithmType.Sha1)
                {
                    reference.DigestMethod = SignedXml.XmlDsigSHA1Url;
                    signedXml.SignedInfo.SignatureMethod = SignedXml.XmlDsigRSASHA1Url;
                }

                signedXml.SigningKey = certificate.PrivateKey;
                signedXml.AddReference(reference);
                signedXml.ComputeSignature();

                var keyInfo = new KeyInfo();
                keyInfo.AddClause(new KeyInfoX509Data(certificate));
                signedXml.KeyInfo = keyInfo;
                var element = signedXml.GetXml();
                node.AppendChild(element);
            }
            catch (ArgumentException ex)
            {
                throw new ArgumentException(string.Format("Algoritmo de assinatura inexistente: {0}", ex.Message));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static T ToObject<T>(this XElement input, string elementName = null, string xmlNamespace = null)
        {
            T result;

            XElement xml;

            if (string.IsNullOrEmpty(elementName))
            {
                xml = input;
                elementName = input.Name.LocalName;
            }
            else
            {
                xml = input
                    .Descendants(string.IsNullOrEmpty(xmlNamespace)
                        ? elementName
                        : (XNamespace)xmlNamespace + elementName)
                    .FirstOrDefault();
            }

            if (xml == null)
            {
                result = default(T);
            }
            else
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T),
                    new XmlRootAttribute
                    {
                        ElementName = elementName,
                        Namespace = xmlNamespace
                    });

                result = (T)serializer.Deserialize(xml.CreateReader());
            }

            return result;
        }

        public static XmlDocument ToXml(this object input, XNamespace xmlNamespace = null)
        {
            var res = new XmlDocument();
            res.LoadXml(input.ToXml(null, xmlNamespace).ToString());

            //return input.ToXml(null, xmlNamespace);
            return res;
        }
        //public static XmlDocument ToXml(this string input)
        //{
        //    var xmlDocument = new XmlDocument();
        //    xmlDocument.LoadXml(input);

        //    return xmlDocument;
        //}


        public static bool Validate(string xml, string defaultNamespace, string schemaPath, ref string errors)
        {
            var temp = new StringBuilder();
            var doc = XDocument.Load(new StringReader(xml));
            var schemas = new XmlSchemaSet();

            var fullSchemaPath = schemaPath;

            if (File.GetAttributes(fullSchemaPath).HasFlag(FileAttributes.Directory))
            {
                if (!Directory.Exists(fullSchemaPath))
                    throw new InvalidOperationException(string.Format("Erro em OmegaNF.Util.ValidateXML -- O diretorio não existe -- {0}", fullSchemaPath));

                foreach (var currentSchema in Directory.GetFiles(fullSchemaPath, "*.xsd"))
                {
                    using (var schemaStream = new StreamReader(currentSchema))
                    {
                        schemas.Add(XmlSchema.Read(schemaStream, null));
                    }
                }
            }
            else
            {
                if (!File.Exists(fullSchemaPath))
                    throw new System.InvalidOperationException(string.Format("Erro em OmegaNF.Util.ValidateXML -- O arquivo não existe -- {0}", fullSchemaPath));

                schemas.Add(defaultNamespace, fullSchemaPath);
            }

            doc.Validate(schemas, (o, e) =>
            {
                if (o is XElement)
                {
                    var element = (XElement)o;
                    temp.AppendFormat("'{0}'<br/>{1}. Value: \"{2}\"<br/><br/>", GetElementFullHierarchy(element), e.Message, element.Value);
                }

                if (o is XAttribute)
                {
                    var attribute = ((XAttribute)o);
                    temp.AppendFormat("'{0}[@{1}]'<br/>{1}<br/><br/>", attribute.Parent.Name, attribute.Name, e.Message);
                }
            });

            if (temp.Length > 0)
                errors = "Ocorreram os seguinte erros na validação do XML:<br/>" + temp.ToString();

            return string.IsNullOrEmpty(errors);
        }

        public static bool IsXml(this string input)
        {
            try
            {
                XDocument.Parse(input);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static string BuildQRCode(this string xml, string QRCodeTokenID, string QRCodeTokenCsc, DateTime? ContingencyDate)
        {
            AssertionConcern.AssertArgumentNotNullEmpty(xml, "XML obrigatório para gerar o QRCode");

            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xml);

            var ns = new XmlNamespaceManager(xmlDocument.NameTable);
            ns.AddNamespace("nfe", "http://www.portalfiscal.inf.br/nfe");
            ns.AddNamespace("sign", "http://www.w3.org/2000/09/xmldsig#");

            var model = xmlDocument.SelectSingleNode("//*[local-name()='mod']");
			//AssertionConcern.AssertArgumentNotNull(model, "Não foi possível encontrar o node \"mod\" no XML da NF para poder montar o QRCode");
			if (model?.InnerText != "65") return xml;

			var tokenID = QRCodeTokenID;
            var tokenCsc = QRCodeTokenCsc;

            AssertionConcern.AssertArgumentFalse((string.IsNullOrEmpty(tokenID) || string.IsNullOrEmpty(tokenCsc)), "QrCode Token e QrCode Token Csc são obrigatórios para NFCe");

            var qrcodeUrl = xmlDocument.SelectSingleNode("//*[local-name()='qrCode']");
            AssertionConcern.AssertArgumentNotNull(qrcodeUrl, "Não foi possível encontrar o node \"qrCode\" no XML da NF para poder montar o QRCode");

            AssertionConcern.AssertArgumentNotNull(xmlDocument.SelectSingleNode("//nfe:infNFe", ns), "Não foi possível encontrar o node \"infNFe\" no XML da NF para poder montar o QRCode");
            AssertionConcern.AssertArgumentNotNull(xmlDocument.SelectSingleNode("//nfe:tpAmb", ns), "Não foi possível encontrar o node \"tpAmb\" no XML da NF para poder montar o QRCode");
            AssertionConcern.AssertArgumentNotNull(xmlDocument.SelectSingleNode("//nfe:ide/nfe:dhEmi", ns), "Não foi possível encontrar o node \"dhEmi\" no XML da NF para poder montar o QRCode");
            AssertionConcern.AssertArgumentNotNull(xmlDocument.SelectSingleNode("//nfe:total/nfe:ICMSTot/nfe:vNF", ns), "Não foi possível encontrar o node \"vNF\" no XML da NF para poder montar o QRCode");
            AssertionConcern.AssertArgumentNotNull(xmlDocument.SelectSingleNode("//nfe:total/nfe:ICMSTot/nfe:vICMS", ns), "Não foi possível encontrar o node \"vICMS\" no XML da NF para poder montar o QRCode");
            AssertionConcern.AssertArgumentNotNull(xmlDocument.SelectSingleNode("//nfe:enviNFe/nfe:NFe/sign:Signature/sign:SignedInfo/sign:Reference/sign:DigestValue", ns), "Não foi possível encontrar o node \"DigestValue\" no XML da NF para poder montar o QRCode");

            var tpEmis = xmlDocument.SelectSingleNode("//nfe:tpEmis", ns).InnerText;

            //[44 exatos] - chNFe -- Chave de Acesso da NFCe
            //ex: NFe13140213998916000124650010000003461000003464
            var chNFe = xmlDocument.SelectSingleNode("//nfe:infNFe", ns).Attributes["Id"].InnerText.Substring(3); //Remove "NFe" do início

            //[3 exatos] nVersao -- Versão do QR Code -- Nessa versão informar "100"
            const string nVersao = "2";

            //[1 exato] tpAmb -- Identificação do Ambiente (1 – Produção, 2 –Homologação)
            var tpAmb = xmlDocument.SelectSingleNode("//nfe:tpAmb", ns).InnerText;

            //[11-20] cDest -- Documento de Identificação do Consumidor (CNPJ/CPF/ID Estrangeiro)
            var cDest = "";
            if (xmlDocument.SelectSingleNode("//nfe:dest/nfe:CNPJ", ns) != null)
                cDest = xmlDocument.SelectSingleNode("//nfe:dest/nfe:CNPJ", ns).InnerText.PadLeft(14, '0');
            else if (xmlDocument.SelectSingleNode("//nfe:dest/nfe:CPF", ns) != null)
                cDest = xmlDocument.SelectSingleNode("//nfe:dest/nfe:CPF", ns).InnerText.PadLeft(11, '0');
            else if (xmlDocument.SelectSingleNode("//nfe:dest/nfe:idEstrangeiro", ns) != null)
                cDest = xmlDocument.SelectSingleNode("//nfe:dest/nfe:idEstrangeiro", ns).InnerText.PadLeft(11, '0');
            //Identificação do cliente não é obrigatória para NFCe

            //[50 convertido para hex] dhEmi -- Data e Hora de Emissão da NFC-e
            DateTime dEmi;
            DateTime.TryParse(xmlDocument.SelectSingleNode("//nfe:ide/nfe:dhEmi", ns).InnerText, out dEmi);
            var dhEmi = dEmi.ToString("dd");

            //[15] vNF -- Valor Total da NFC-e
            var vNF = xmlDocument.SelectSingleNode("//nfe:total/nfe:ICMSTot/nfe:vNF", ns).InnerText;

            //[15] vICMS -- Valor Total ICMS na NFC-e
            var vICMS = xmlDocument.SelectSingleNode("//nfe:total/nfe:ICMSTot/nfe:vICMS", ns).InnerText;

            //[56 exatos convertido para hex] digVal -- Digest Value da NFC-e
            var digVal = xmlDocument.SelectSingleNode("//nfe:enviNFe/nfe:NFe/sign:Signature/sign:SignedInfo/sign:Reference/sign:DigestValue", ns).InnerText;
            digVal = digVal.ToHexString();

            var sb = new StringBuilder();

            int tokenIDSignificantNumber;
            if (!int.TryParse(tokenID, out tokenIDSignificantNumber))
                throw new InvalidOperationException(string.Format("Código identificador do CSC é inválido. Devem ser informados apenas números. Valor: {0}", tokenID));

            var qrCodeTemplate = string.Empty;
            if (ContingencyDate.HasValue)
                qrCodeTemplate = "{0}|{1}|{2}|{3}|{4}|{5}|{6}";
            else
                qrCodeTemplate = "{0}|{1}|{2}|{6}";

            sb.AppendFormat(qrCodeTemplate, chNFe, nVersao, tpAmb, dhEmi, vNF, digVal, tokenIDSignificantNumber);

            var hash = string.Concat(sb.ToString(), tokenCsc).GenerateHash();
            sb.AppendFormat("|{0}", hash);

            var qrCode = string.Format(qrcodeUrl.InnerText.IndexOf('?') > -1 ? "{0}&p={1}" : "{0}?p={1}", qrcodeUrl.InnerText, sb);

            xmlDocument.SelectSingleNode("//*[local-name()='qrCode']").InnerText = "{QRCODE}";
            xmlDocument.InnerXml = xmlDocument.OuterXml.Replace("{QRCODE}", string.Format("<![CDATA[{0}]]>", qrCode));

            return xmlDocument.OuterXml;
        }

        private static XElement CreateElement(XNamespace xmlNamespace, string elementName, object content = null)
        {
            return new XElement(xmlNamespace == null
                    ? elementName
                    : xmlNamespace + elementName,
                content is DateTime ? $"{content:o}" : content);
        }

        private static XElement GetArrayElement(PropertyInfo info, Array input, XNamespace xmlNamespace = null)
        {
            var name = XmlConvert.EncodeName(info.Name) ?? "Object";
            XElement rootElement = CreateElement(xmlNamespace, name);
            var arrayCount = input?.GetLength(0) ?? 0;
            for (int i = 0; i < arrayCount; i += 1)
            {
                var val = input?.GetValue(i);
                var typeName = GetTypeName(val.GetType());
                XElement childElement = val.GetType().IsSimpleType()
                    ? CreateElement(xmlNamespace, $"{typeName}", val)
                    : val.ToXml(typeName, xmlNamespace);
                rootElement.Add(childElement);
            }

            return rootElement;
        }

        private static string GetElementFullHierarchy(XElement element)
        {
            var ret = "";
            if (element.Parent == null)
                ret = element.Name.LocalName;
            else
                ret = GetElementFullHierarchy(element.Parent) + "/" + element.Name.LocalName;
            return ret;
        }

        private static string GetTypeName(this Type type)
        {
            if (type == typeof(bool))
                return "boolean";
            if (type == typeof(short) || type == typeof(int) || type == typeof(long))
                return "int";
            if (type == typeof(byte))
                return "byte";
            if (type == typeof(DateTime))
                return "dateTime";
            if (type == typeof(double) || type == typeof(decimal) || type == typeof(float))
                return "double";
            if (type == typeof(string))
                return "string";
            return type.Name;
        }

        private static bool IsSimpleType(this Type type)
        {
            return type.IsPrimitive || WriteTypes.Contains(type);
        }

        private static XElement ToXml(this object input, string element, XNamespace xmlNamespace = null)
        {
            if (input == null)
                return null;


            if (string.IsNullOrEmpty(element))
            {
                string name = input.GetType().Name;
                element = name.Contains("AnonymousType") ? "Object" : name;
            }

            element = XmlConvert.EncodeName(element);
            var ret = CreateElement(xmlNamespace, element);

            var type = input.GetType();
            var props = type.GetProperties();

            var elements = props.Select(p =>
            {
                var pType = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType;
                var name = XmlConvert.EncodeName(p.Name);
                var val = pType.IsArray ? "array" : p.GetValue(input, null);
                var value = pType.IsArray ? GetArrayElement(p, (Array)p.GetValue(input, null), xmlNamespace) : pType.IsSimpleType() || pType.IsEnum ? CreateElement(xmlNamespace, name, val) : val.ToXml(name, xmlNamespace);
                return value;
            }).Where(v => v != null);

            ret.Add(elements);

            return ret;
        }

        public class DynamicXml : DynamicObject
        {
            XElement _root;
            private DynamicXml(XElement root)
            {
                _root = root;
            }

            public static DynamicXml Parse(string xmlString)
            {
                return new DynamicXml(RemoveNamespaces(XDocument.Parse(xmlString).Root));
            }

            public static DynamicXml Load(string filename)
            {
                return new DynamicXml(RemoveNamespaces(XDocument.Load(filename).Root));
            }

            private static XElement RemoveNamespaces(XElement xElem)
            {
                var attrs = xElem.Attributes()
                            .Where(a => !a.IsNamespaceDeclaration)
                            .Select(a => new XAttribute(a.Name.LocalName, a.Value))
                            .ToList();

                if (!xElem.HasElements)
                {
                    XElement xElement = new XElement(xElem.Name.LocalName, attrs);
                    xElement.Value = xElem.Value;
                    return xElement;
                }

                var newXElem = new XElement(xElem.Name.LocalName, xElem.Elements().Select(e => RemoveNamespaces(e)));
                newXElem.Add(attrs);
                return newXElem;
            }

            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {
                result = null;

                var att = _root.Attribute(binder.Name);
                if (att != null)
                {
                    result = att.Value;
                    return true;
                }

                var nodes = _root.Elements(binder.Name);
                if (nodes.Count() > 1)
                {
                    result = nodes.Select(n => n.HasElements ? (object)new DynamicXml(n) : n.Value).ToList();
                    return true;
                }

                var node = _root.Element(binder.Name);
                if (node != null)
                {
                    result = node.HasElements || node.HasAttributes ? (object)new DynamicXml(node) : node.Value;
                    return true;
                }

                return true;
            }
        }
    }

    internal class UTF8StringWriter : StringWriter
    {
        public override Encoding Encoding { get { return Encoding.UTF8; } }
    }
}
