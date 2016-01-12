using System;

using Irony.Parsing;

namespace ID3SQL
{
    public class ID3SQLGrammar : Grammar
    {
        public static ParseTree GenerateParseTree(string statement)
        {
            Grammar grammar = new ID3SQLGrammar();
            LanguageData languageData = new LanguageData(grammar);
            Parser parser = new Parser(languageData);
            ParseTree parseTree = parser.Parse(statement);
            return parseTree;
        }

        public const string NumberTermName = "number";
        public const string StringLiteralTermName = "string";
        public const string IdTermName = "id";

        public const string CommaKeyTermText = ",";
        public const string NotKeyTermText = "NOT";
        public const string UpdateKeyTermText = "UPDATE";
        public const string SetKeyTermText = "SET";
        public const string DeleteKeyTermText = "DELETE";
        public const string SelectKeyTermText = "SELECT";

        public const string StatementNonTermName = "statement";
        public const string SelectStatementNonTermName = "selectStatement";
        public const string UpdateStatementNonTermName = "updateStatement";
        public const string DeleteStatementNonTermName = "deleteStatement";
        public const string AssignListNonTermName = "assignList";
        public const string WhereClauseNonTermName = "whereClause";
        public const string AssignmentNonTermName = "assignment";
        public const string ExpressionNonTermName = "expression";
        public const string ExpressionListNonTermName = "expressionList";
        public const string SelectListNonTermName = "selectList";
        public const string ColumnItemListNonTermName = "columnItemList";
        public const string TermNonTermName = "term";
        public const string UnaryExpressionNonTermName = "unaryExpression";
        public const string UnaryOperatorNonTermName = "unaryOperand";
        public const string BinaryExpressionNonTermName = "binaryExpression";
        public const string BinaryOperatorNonTermName = "binaryOperand";
        public const string BetweenExpressionNonTermName = "betweenExpression";
        public const string NotOperatorNonTermName = "notOperand";
        public const string InStatementNonTermName = "inStatement";

        private ID3SQLGrammar() : base(false)
        {
            Terminal numberTerm = new NumberLiteral(NumberTermName);
            Terminal stringLiteralTerm = new StringLiteral(StringLiteralTermName, "'", StringOptions.AllowsDoubledQuote);
            Terminal idTerm = TerminalFactory.CreateSqlExtIdentifier(this, IdTermName); //covers normal identifiers (abc) and quoted id's ([abc d], "abc d")

            KeyTerm commaKeyTerm = ToTerm(CommaKeyTermText);
            KeyTerm notKeyTerm = ToTerm(NotKeyTermText);
            KeyTerm updateKeyTerm = ToTerm(UpdateKeyTermText);
            KeyTerm setKeyTerm = ToTerm(SetKeyTermText);
            KeyTerm deleteKeyTerm = ToTerm(DeleteKeyTermText);
            KeyTerm selectKeyTerm = ToTerm(SelectKeyTermText);

            NonTerminal statementNonTerm = new NonTerminal(StatementNonTermName);
            NonTerminal selectStatementNonTerm = new NonTerminal(SelectStatementNonTermName);
            NonTerminal updateStatementNonTerm = new NonTerminal(UpdateStatementNonTermName);
            NonTerminal deleteStatementNonTerm = new NonTerminal(DeleteStatementNonTermName);
            NonTerminal assignListNonTerm = new NonTerminal(AssignListNonTermName);
            NonTerminal whereClauseNonTerm = new NonTerminal(WhereClauseNonTermName);
            NonTerminal assignmentNonTerm = new NonTerminal(AssignmentNonTermName);
            NonTerminal expressionNonTerm = new NonTerminal(ExpressionNonTermName);
            NonTerminal expressionListNonTerm = new NonTerminal(ExpressionListNonTermName);
            NonTerminal selectListNonTerm = new NonTerminal(SelectListNonTermName);
            NonTerminal columnItemListNonTerm = new NonTerminal(ColumnItemListNonTermName);
            NonTerminal termNonTerm = new NonTerminal(TermNonTermName);
            NonTerminal unaryExpressionNonTerm = new NonTerminal(UnaryExpressionNonTermName);
            NonTerminal unaryOperatorNonTerm = new NonTerminal(UnaryOperatorNonTermName);
            NonTerminal binaryExpressionNonTerm = new NonTerminal(BinaryExpressionNonTermName);
            NonTerminal binaryOperatorNonTerm = new NonTerminal(BinaryOperatorNonTermName);
            NonTerminal betweenExpressionNonTerm = new NonTerminal(BetweenExpressionNonTermName);
            NonTerminal notOperatorNonTerm = new NonTerminal(NotOperatorNonTermName);
            NonTerminal inStatementNonTerm = new NonTerminal(InStatementNonTermName);

            this.Root = statementNonTerm;

            statementNonTerm.Rule = selectStatementNonTerm | updateStatementNonTerm | deleteStatementNonTerm;

            //Select stmt
            selectStatementNonTerm.Rule = selectKeyTerm + selectListNonTerm + whereClauseNonTerm;
            selectListNonTerm.Rule = columnItemListNonTerm | "*";
            columnItemListNonTerm.Rule = MakePlusRule(columnItemListNonTerm, commaKeyTerm, idTerm);
            whereClauseNonTerm.Rule = Empty | "WHERE" + expressionNonTerm;

            //Update stmt
            updateStatementNonTerm.Rule = updateKeyTerm + setKeyTerm + assignListNonTerm + whereClauseNonTerm;
            assignListNonTerm.Rule = MakePlusRule(assignListNonTerm, commaKeyTerm, assignmentNonTerm);
            assignmentNonTerm.Rule = idTerm + "=" + expressionNonTerm;

            //Delete stmt
            deleteStatementNonTerm.Rule = deleteKeyTerm + whereClauseNonTerm;
 
            //Expression
            expressionListNonTerm.Rule = MakePlusRule(expressionListNonTerm, commaKeyTerm, expressionNonTerm);
            expressionNonTerm.Rule = termNonTerm | unaryExpressionNonTerm | binaryExpressionNonTerm;
            termNonTerm.Rule = idTerm | stringLiteralTerm | numberTerm;
            unaryExpressionNonTerm.Rule = unaryOperatorNonTerm + termNonTerm;
            unaryOperatorNonTerm.Rule = notKeyTerm | ToTerm("+") | ToTerm("-") | ToTerm("~");
            binaryExpressionNonTerm.Rule = expressionNonTerm + binaryOperatorNonTerm + expressionNonTerm;
            binaryOperatorNonTerm.Rule = ToTerm("+") | ToTerm("-") | ToTerm("*") | ToTerm("/") | ToTerm("%") //arithmetic
                        | ToTerm("&") | ToTerm("|") | ToTerm("^")                     //bit
                        | ToTerm("=") | ToTerm(">") | ToTerm("<") | ToTerm(">=") | ToTerm("<=") | ToTerm("!=")
                        | ToTerm("AND") | ToTerm("OR") | ToTerm("LIKE") | notKeyTerm + ToTerm("LIKE") | ToTerm("IN") | notKeyTerm + ToTerm("IN");
            betweenExpressionNonTerm.Rule = expressionNonTerm + notOperatorNonTerm + "BETWEEN" + expressionNonTerm + "AND" + expressionNonTerm;
            notOperatorNonTerm.Rule = Empty | notKeyTerm;
            inStatementNonTerm.Rule = expressionNonTerm + "IN" + "(" + expressionListNonTerm + ")";

            //Operators
            RegisterOperators(10, "*", "/", "%");
            RegisterOperators(9, "+", "-");
            RegisterOperators(8, "=", ">", "<", ">=", "<=", "<>", "!=", "!<", "!>", "LIKE", "IN");
            RegisterOperators(7, "^", "&", "|");
            RegisterOperators(6, notKeyTerm);
            RegisterOperators(5, "AND");
            RegisterOperators(4, "OR");

            MarkPunctuation(",", "(", ")");
            //Note: we cannot declare binOp as transient because it includes operators "NOT LIKE", "NOT IN" consisting of two tokens. 
            // Transient non-terminals cannot have more than one non-punctuation child nodes.
            // Instead, we set flag InheritPrecedence on binOp , so that it inherits precedence value from it's children, and this precedence is used
            // in conflict resolution when binOp node is sitting on the stack
            base.MarkTransient(statementNonTerm, termNonTerm, expressionNonTerm, unaryOperatorNonTerm);
            binaryOperatorNonTerm.SetFlag(TermFlags.InheritPrecedence);
        }
    }
}
