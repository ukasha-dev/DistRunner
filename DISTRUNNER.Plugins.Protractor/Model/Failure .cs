using System.Xml.Serialization;

namespace DISTRUNNER.Plugins.Protractor.Model;

[XmlRoot(ElementName = "failure")]
public class Failure
{
    [XmlElement(ElementName = "message")]
    public string Message { get; set; }
    [XmlElement(ElementName = "stack-trace")]
    public string Stacktrace { get; set; }
}
