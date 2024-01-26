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
            from kw in Parser.Expect(TokenType.Identifier, "class").OrElse(Parser.Expect(TokenType.Identifier, "trait"))
            from po in Parser.Position()
            from id in Parser.Expect(TokenType.Identifier)
            from le in Parser.Expect(TokenType.LeftCurlyBracket)
            from fs in field.SeparatedBy(Parser.Expect(TokenType.Comma))
            from ri in Parser.Expect(TokenType.RightCurlyBracket)
            select kw == "class" ? new Class(id, fs) { LineInfo = po } : (Classifier)new Trait(id, fs) { LineInfo = po };
    }

    public static Parser<Classifier> Classifier { get; }
}