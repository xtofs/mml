namespace edmml;


public record struct Result<T>(T Value, ReadOnlyMemory<Token> Remainder);

public delegate bool Parser<T>(ReadOnlyMemory<Token> tokens, [MaybeNullWhen(false)] out Result<T> result);

public class Result
{
    internal static Result<T> New<T>(T value, ReadOnlyMemory<Token> remainder)
    {
        return new Result<T>(value, remainder);
    }
}

public static class Parser
{
    public static Parser<string> Expect(TokenType tokenType, string value)
    {
        return (ReadOnlyMemory<Token> tokens, [MaybeNullWhen(false)] out Result<String> result) =>
        {
            if (tokens.IsEmpty) { result = default; return false; }
            var first = tokens.Span[0]; ;
            if (first.Type == tokenType && first.Value.Equals(value))
            {
                result = Result.New(first.Value, tokens[1..]); return true;
            }
            result = default; return false;
        };
    }

    public static Parser<string> Expect(TokenType tokenType)
    {
        return (ReadOnlyMemory<Token> tokens, [MaybeNullWhen(false)] out Result<String> result) =>
        {
            if (tokens.IsEmpty) { result = default; return false; }
            var first = tokens.Span[0];
            if (first.Type == tokenType)
            {
                result = Result.New(first.Value, tokens[1..]); return true;
            }
            result = default; return false;
        };
    }

    public static Parser<R> Select<T, R>(this Parser<T> first, Func<T, R> selector)
    {
        return (ReadOnlyMemory<Token> tokens, [MaybeNullWhen(false)] out Result<R> result) =>
        {
            if (first(tokens, out var firstResult))
            {
                result = Result.New(selector(firstResult.Value), firstResult.Remainder);
                return true;
            }
            result = default; return false;
        };
    }

    public static Parser<R> SelectMany<S, T, R>(this Parser<S> first, Func<S, Parser<T>> secondFactory, Func<S, T, R> selector)
    {
        return (ReadOnlyMemory<Token> tokens, [MaybeNullWhen(false)] out Result<R> result) =>
        {
            if (first(tokens, out var firstResult))
            {
                var second = secondFactory(firstResult.Value);
                if (second(firstResult.Remainder, out var secondResult))
                {
                    result = Result.New(selector(firstResult.Value, secondResult.Value), secondResult.Remainder);
                    return true;
                }
            }
            result = default; return false;
        };
    }

    public static Parser<T> OrElse<T>(this Parser<T> first, Parser<T> second)
    {
        return (ReadOnlyMemory<Token> tokens, [MaybeNullWhen(false)] out Result<T> result) =>
        {
            if (first(tokens, out var firstResult))
            {
                result = Result.New(firstResult.Value, firstResult.Remainder);
                return true;
            }
            else if (second(firstResult.Remainder, out var secondResult))
            {
                result = Result.New(secondResult.Value, secondResult.Remainder);
                return true;
            }
            result = default;
            return false;
        };
    }

    public static Parser<T> Alt<T>(params Parser<T>[] alternatives)
    {
        return (ReadOnlyMemory<Token> tokens, [MaybeNullWhen(false)] out Result<T> result) =>
        {
            foreach (var parser in alternatives)
            {
                if (parser(tokens, out var match))
                {
                    result = Result.New(match.Value, match.Remainder);
                    return true;
                }
            }
            result = default;
            return false;
        };
    }

    public static Parser<LineInfo> Position()
    {
        return (ReadOnlyMemory<Token> tokens, [MaybeNullWhen(false)] out Result<LineInfo> result) =>
        {
            if (tokens.IsEmpty) { result = default; return false; }
            result = Result.New(tokens.Span[0].Position, tokens);
            return true;
        };
    }


    public static Parser<IReadOnlyList<T>> Many<T>(this Parser<T> itemParser)
    {
        return (ReadOnlyMemory<Token> tokens, [MaybeNullWhen(false)] out Result<IReadOnlyList<T>> result) =>
        {
            var items = new List<T>();
            while (itemParser(tokens, out var item))
            {
                items.Add(item.Value);
                tokens = item.Remainder;
            }

            result = Result.New<IReadOnlyList<T>>(items, tokens);
            return true;
        };
    }

    public static Parser<T?> Optional<T>(this Parser<T> parser) // where T : class
    {
        return (ReadOnlyMemory<Token> tokens, [MaybeNullWhen(false)] out Result<T?> result) =>
         {
             if (parser(tokens, out var item))
             {
                 result = Result.New((T?)item.Value, item.Remainder);
                 return true;
             }
             result = Result.New(default(T?), tokens); ;
             return true;
         };
    }

    public static Parser<T> Optional<T>(this Parser<T> parser, T @default) // where T : class
    {
        return (ReadOnlyMemory<Token> tokens, [MaybeNullWhen(false)] out Result<T> result) =>
        {
            if (parser(tokens, out var item))
            {
                result = Result.New((T)item.Value, item.Remainder);
                return true;
            }
            result = Result.New(@default, tokens); ;
            return true;
        };
    }

    public static Parser<IReadOnlyList<T>> SeparatedBy<T, S>(this Parser<T> itemParser, Parser<S> separatorParser)
    {
        return (ReadOnlyMemory<Token> tokens, [MaybeNullWhen(false)] out Result<IReadOnlyList<T>> result) =>
        {
            var items = new List<T>();
            while (itemParser(tokens, out var item))
            {
                items.Add(item.Value);
                tokens = item.Remainder;

                if (!separatorParser(tokens, out var separator))
                {
                    break;
                }
                else
                {
                    tokens = separator.Remainder;
                }
            }

            result = Result.New<IReadOnlyList<T>>(items, tokens);
            return true;
        };
    }
}
