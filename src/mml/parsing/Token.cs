

using System.Text;

namespace mml;
public readonly record struct Token(TokenType Type, string Value, LineInfo Position)
{

    private bool PrintMembers(StringBuilder builder)
    {
        builder.AppendFormat("Value = '{0}'", Value.Escape());
        builder.AppendFormat(", Type = {0}", Type);
        builder.AppendFormat(", Position = {0}", Position);
        return true;
    }
}

public readonly record struct LineInfo(int Line, int Column)
{
    public static LineInfo operator +(LineInfo position, string text)
    {
        var (line, column) = position;
        for (int i = 0; i < text.Length; i++)
        {
            var ch = text[i];
            switch (ch)
            {
                // the ␍␊ combination
                case '\r' when text.Length > i + 1 && text[i + 1] == '\n':
                    i += 1; line += 1; column = 1;
                    break;
                // a single ␊
                case '\n':
                    line += 1; column = 1;
                    break;
                // a single ␍
                case '\r':
                    line += 1; column = 1;
                    break;
                default:
                    column += 1;
                    break;
            }
        }
        return new LineInfo(line, column);
    }

    public override string ToString() => $"Ln {Line}, Col {Column}";
}




public static class StringExtensions
{
    public static string Escape(this String str)
    {
        return string.Create(str.Length, str, (span, val) =>
        {
            for (int i = 0; i < val.Length; i++)
            {
                span[i] = val[i] < 32 ? (char)(val[i] + '\u2400') : val[i];
            }
        });
    }
}
