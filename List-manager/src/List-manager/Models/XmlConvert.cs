using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace List_manager.Models
{
    public static class XmlConvert
    {
        
        public static string SerializeObject<T>(T data)
        {
            if(data == null)
            {
                return string.Empty;
            }
            try {

                using (System.IO.StringWriter stringWriter = new System.IO.StringWriter())
                {
                    var serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
                    serializer.Serialize(stringWriter, data);
                    return stringWriter.ToString();
                }

            }
            catch (Exception ex)
            {
                return string.Empty;
            }
        }

        //Might need to rewrite this in the future to use XmlSerializerInputFormatter 
        public static T DeserializeObject<T>(string xml) 
            where T : new()
        {

            if (string.IsNullOrEmpty(xml))
            {
                return new T();
            }
            try
            {
                using (XmlReader reader = XmlReader.Create(new MemoryStream(Encoding.UTF8.GetBytes(xml))))
                {

                    var serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));
                    return (T)serializer.Deserialize(reader);

                }
            }
            catch (Exception ex)
            {
                return new T();
            }
        }
    }
}
