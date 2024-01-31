namespace mml.parsing;

static class MetaModelParser
{
    static MetaModelParser()
    {
        var builtin = Parser.Alt<Builtin>(
            from id in Parser.Expect(TokenType.Identifier, "string") select Builtin.String,
            from id in Parser.Expect(TokenType.Identifier, "int") select Builtin.Int,
            from id in Parser.Expect(TokenType.Identifier, "bool") select Builtin.Bool
        );

        // var dict =
        //    from id in Parser.Expect(TokenType.Identifier, "Dictionary")
        //    from op in Parser.Expect(TokenType.LessThanSign)
        //    from ty in Parser.Expect(TokenType.Identifier)
        //    from pr in Parser.Expect(TokenType.Period)
        //    from pa in Parser.Expect(TokenType.Identifier).SeparatedBy(Parser.Expect(TokenType.Period))
        //    from cl in Parser.Expect(TokenType.GreaterThanSign)
        //    select new Dictionary(ty, pa);

        // [<id>|<id>.<id>]
        var dict =
            from id in Parser.Expect(TokenType.LeftSquareBracket)
            from ty in Parser.Expect(TokenType.Identifier)
            from pr in Parser.Expect(TokenType.Semicolon)
            from pa in Parser.Expect(TokenType.Identifier).SeparatedBy(Parser.Expect(TokenType.Period))
            from cl in Parser.Expect(TokenType.RightSquareBracket)
            select new Dictionary(ty, pa);

        var reference =
            from aa in Parser.Expect(TokenType.Ampersand)
            from id in Parser.Expect(TokenType.Identifier)
            select new Reference(id);

        var type = Parser.Alt<FieldType>(
            from nc in dict select (FieldType)nc,
            from bi in builtin select (FieldType)bi,
            from id in Parser.Expect(TokenType.Identifier) select (FieldType)new Contained(id),
            from re in reference select (FieldType)re
        );

        var field =
             from po in Parser.Position()
             from id in Parser.Expect(TokenType.Identifier)
             from co in Parser.Expect(TokenType.Colon)
             from ty in type
             select new Field(id, ty, po);

        var extends =
            from kw in Parser.Expect(TokenType.Identifier, "extends")
            from ids in Parser.Expect(TokenType.Identifier).SeparatedBy(Parser.Expect(TokenType.PlusSign))
            select ids;

        Classifier =
            from po in Parser.Position()
            from kw in Parser.Alt(Parser.Expect(TokenType.Identifier, "trait"), Parser.Expect(TokenType.Identifier, "class"))
            from id in Parser.Expect(TokenType.Identifier)
            from ex in extends.Optional([])
            from le in Parser.Expect(TokenType.LeftCurlyBracket)
            from fs in field.SeparatedBy(Parser.Expect(TokenType.Comma))
            from ri in Parser.Expect(TokenType.RightCurlyBracket)
            select kw == "class"
                ? (Classifier)new Class(id, fs, ex) { LineInfo = po }
                : (Classifier)new Trait(id, fs, ex) { LineInfo = po };

        Classifiers = Classifier.Many();
    }

    public static Parser<Classifier> Classifier { get; }
    public static Parser<IReadOnlyList<Classifier>> Classifiers { get; }
}