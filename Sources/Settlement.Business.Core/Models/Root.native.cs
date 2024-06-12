using System.Xml.Serialization;

namespace Settlement.Business.Core.Models;

[XmlRoot("root")]
public class Root<T>
{
    [XmlElement("r")]
    public List<T> Rows { get; set; }
}