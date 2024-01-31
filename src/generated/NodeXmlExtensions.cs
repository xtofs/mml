namespace model;

using System.Xml;
using System.Xml.Linq;

public static class NodeXmlExtensions
{

    public static void WriteXml(this Model model, Schema schema, string path)
    {
        using var text = File.CreateText(path);
        model.WriteXml(schema, text);
        text.WriteLine("xxx");
    }

    public static void WriteXml(this Model model, Schema schema, TextWriter writer)
    {
        var xml = schema.ToXml(model);
        using (var xmlWriter = XmlWriter.Create(writer, new XmlWriterSettings { Indent = true }))
        {
            xml.WriteTo(xmlWriter);
            // xmlWriter.Flush();
        }
        writer.WriteLine();
    }

    public static XElement ToXml(this INode node, Model root)
    {
        var element = new XElement(node.NodeTag);

        foreach (var (name, value) in node.Attributes)
        {
            element.SetAttributeValue(name, value);
        }

        foreach (var contained in node.Links
            .Where(lnk => lnk.Label == Label.CONTAINS && lnk.Label.IsForward)
            .Select(lnk => lnk.Target))
        {
            element.Add(contained.ToXml(root));
        }

        foreach (var (target, labelName) in node.Links
            .Where(lnk => lnk.Label != Label.CONTAINS && lnk.Label.IsForward)
            .Select(lnk => (lnk.Target, lnk.Label.Name)))
        {
            element.SetAttributeValue(labelName, target.GetQualifiedName(root));
        }
        return element;
    }
}
