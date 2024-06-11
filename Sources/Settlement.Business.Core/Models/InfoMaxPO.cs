using System.Xml.Serialization;

namespace Settlement.Business.Core.Models;
public class InfoMaxPO
{
    [XmlElement("a1")]
    public long CustId { get; set; }

    [XmlElement("a2")]
    public int ProductId { get; set; }

    [XmlElement("a3")]
    public int GroupId { get; set; }

    [XmlElement("a4")]
    public decimal WinLost { get; set; }

    [XmlElement("a6")]
    public decimal ActualPayout { get; set; }

    [XmlElement("a7")]
    public int LevelId { get; set; }

    [XmlElement("a8")]
    public int ActionId { get; set; }

    [XmlElement("a12")]
    public int CreateBy { get; set; }

    [XmlElement("a13")]
    public DateTime WinLostDate { get; set; }

    [XmlElement("a16")]
    public int BetType { get; set; }

    [XmlElement("a17")]
    public long TransId { get; set; }

}
