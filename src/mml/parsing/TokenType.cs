using System.Text.RegularExpressions;

namespace mml;


public enum TokenType
{
    [Regex(@"\s+")] Whitespace,
    [Regex("[_a-zA-Z][_a-zA-Z0-9]*")] Identifier,

    [Regex(",")] Comma,
    [Regex(@"\.")] Period,              // https://www.fileformat.info/info/unicode/char/002e/index.htm
    [Regex(":")] Colon,
    [Regex(";")] Semicolon,
    [Regex("{")] LeftCurlyBracket,      // https://www.fileformat.info/info/unicode/char/007b/index.htm
    [Regex(@"\|")] Pipe,                // https://www.fileformat.info/info/unicode/char/007c/index.htm
    [Regex("}")] RightCurlyBracket,     // https://www.fileformat.info/info/unicode/char/007d/index.htm
    [Regex(@"\[")] LeftSquareBracket,  // https://www.fileformat.info/info/unicode/char/005b/index.htm
    [Regex(@"\]")] RightSquareBracket, // https://www.fileformat.info/info/unicode/char/005d/index.htm

    [Regex("<")] LessThanSign,      // https://www.fileformat.info/info/unicode/char/003c/index.htm
    [Regex("=")] EqualsSign,        // https://www.fileformat.info/info/unicode/char/003d/index.htm
    [Regex(">")] GreaterThanSign,   // https://www.fileformat.info/info/unicode/char/003e/index.htm
    [Regex(@"\+")] PlusSign,
    [Regex(@"&")] Ampersand,

    [Regex("\\/\\/.*\n")] LineComment,

    Unknown,
}
