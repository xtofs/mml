namespace model;
public static class NodeExtensions
{

    public static IEnumerable<INode> DescendantsOrSelf(this INode node)
    {
        yield return node;
        foreach (var descendant in Descendants(node))
        {
            yield return descendant;
        }
    }

    /// <summary>
    /// descendants of node (without self)
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public static IEnumerable<INode> Descendants(this INode node)
    {
        foreach (var direct in node.Links
            .Where(lnk => lnk.Label == Label.CONTAINS)
            .Select(lnk => lnk.Target))
        {
            yield return direct;

            foreach (var descendant in Descendants(direct))
            {
                yield return descendant;
            }
        }
    }

}
