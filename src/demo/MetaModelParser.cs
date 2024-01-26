using edmml;

static class MetaModelParser
{
    static MetaModelParser()
    {

        var builtin = Parser.Alt<Builtin>(
            from id in Parser.Expect(TokenType.Identifier, "string") select Builtin.String,
            from id in Parser.Expect(TokenType.Identifier, "int") select Builtin.Int,
            from id in Parser.Expect(TokenType.Identifier, "bool") select Builtin.Bool
        );

        var type = Parser.Alt<FieldType>(
            from bi in builtin select (FieldType)bi,
            from id in Parser.Expect(TokenType.Identifier) select (FieldType)new Contained(id),
            from aa in Parser.Expect(TokenType.Ampersand) from id in Parser.Expect(TokenType.Identifier) select (FieldType)new Reference(id)
        );

        var field =
             from po in Parser.Position()
             from id in Parser.Expect(TokenType.Identifier)
             from co in Parser.Expect(TokenType.Colon)
             from ty in type
             select new Field(id, ty, po);

        Classifier =
            from kw in Parser.Alt(Parser.Expect(TokenType.Identifier, "trait"), Parser.Expect(TokenType.Identifier, "class"))
            from po in Parser.Position()
            from id in Parser.Expect(TokenType.Identifier)
            from le in Parser.Expect(TokenType.LeftCurlyBracket)
            from fs in field.SeparatedBy(Parser.Expect(TokenType.Comma))
            from ri in Parser.Expect(TokenType.RightCurlyBracket)
            select kw == "class"
                ? (Classifier)new Class(id, fs) { LineInfo = po }
                : (Classifier)new Trait(id, fs) { LineInfo = po };

        Classifiers = Classifier.Many();
    }

    public static Parser<Classifier> Classifier { get; }
    public static Parser<IReadOnlyList<Classifier>> Classifiers { get; }
}