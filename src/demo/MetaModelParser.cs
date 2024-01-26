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

        var dict =
           from id in Parser.Expect(TokenType.Identifier, "Dictionary")
           from op in Parser.Expect(TokenType.LessThanSign)
           from pa in Parser.Expect(TokenType.Identifier).SeparatedBy(Parser.Expect(TokenType.Period))
           from cl in Parser.Expect(TokenType.GreaterThanSign)
           select new Dictionary(pa);
        // Token { Value = 'Named', Type = Identifier, Position = Ln 4, Col 15 }
        // Token { Value = '<', Type = LessThanSign, Position = Ln 4, Col 20 }
        // Token { Value = 'SchemaElement', Type = Identifier, Position = Ln 4, Col 21 }
        // Token { Value = '>', Type = GreaterThanSign, Position = Ln 4, Col 34 }

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