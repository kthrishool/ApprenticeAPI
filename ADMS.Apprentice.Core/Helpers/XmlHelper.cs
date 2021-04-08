using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Xml;
using System.Xml.Linq;

namespace ADMS.Apprentice.Core.Helpers
{

    public interface IXmlHelper
    {
        string XmlToJson(string xml);
    }

    public class XmlHelper : IXmlHelper
    {
    
        public XmlHelper()
        {
        }

        public string XmlToJson(string xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);

            return JsonConvert.SerializeXmlNode(doc);
        }

        public T XmlToObject<T>(string xml)
        {
            string json = XmlToJson(xml);
            return  JsonConvert.DeserializeObject<T>(json);
        }

        public T XmlToParentObject<T>(string input)
        {
            var xml = XElement.Parse(input);
            var json = JsonConvert.SerializeXNode(xml);

            var jsonResult = JsonConvert.DeserializeObject(json).ToString();
            var o = JsonConvert.DeserializeObject<T>(jsonResult);
            return o;
        }

        public T XmlToSelectedObject<T>(string input)
        {
            var typ = typeof(T).Name;

            var xml = XElement.Parse(input);
            var json = JsonConvert.SerializeXNode(xml);

            var jsonResult = JsonConvert.DeserializeObject(json).ToString();
            return JObject.Parse(jsonResult).SelectToken(typ).ToObject<T>();
        }

        public string TtoXml<T>(T input)
        {
            var typ = typeof(T).Name;

            var json = JsonConvert.SerializeObject(input);

            XmlDocument doc =  JsonConvert.DeserializeXmlNode(json, typ);
            XmlNode docNode = doc.CreateXmlDeclaration("1.0", "utf-16", "no");
            doc.PrependChild(docNode);

            return doc.InnerXml;
        }
    }
}
