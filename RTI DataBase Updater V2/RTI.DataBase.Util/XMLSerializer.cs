using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace RTI.DataBase.Util
{
    public class XMLSerialization
    {
        public void Serialize(object obj,string path, Type type=null)
        {
            XmlSerializer serializer = (type == null) ? new XmlSerializer(obj.GetType()): new XmlSerializer(type);
            using (TextWriter writer = new StreamWriter(path))
            {
                serializer.Serialize(writer, obj);
            }
        }
    }
}
