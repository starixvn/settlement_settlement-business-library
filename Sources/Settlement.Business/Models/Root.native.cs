using System.Xml.Serialization;

namespace Settlement.Business.Models;
[XmlRoot("root")]
public class Root<T>
{
    [XmlElement("r")]
    public List<T> Rows { get; set; }
}
