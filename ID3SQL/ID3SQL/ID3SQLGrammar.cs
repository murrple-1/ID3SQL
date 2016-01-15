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
        public const string NotOperatorNonTermName = "notOperand";
        public const string InStatementNonTermName = "inStatement";

        private ID3SQLGrammar() : base(false)
        {
            Terminal numberTerm = new NumberLiteral(NumberTermName);
            Terminal stringLiteralTerm = new StringLiteral(StringLiteralTermName, "'", StringOptions.AllowsDoubledQuote);
            Terminal idTerm = TerminalFactory.CreateSqlExtIdentifier(this, IdTermName);

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
            NonTerminal notOperatorNonTerm = new NonTerminal(NotOperatorNonTermName);
            NonTerminal inStatementNonTerm = new NonTerminal(InStatementNonTermName);

            this.Root = statementNonTerm;

            statementNonTerm.Rule = selectStatementNonTerm | updateStatementNonTerm | deleteStatementNonTerm;

            //Select statement
            selectStatementNonTerm.Rule = ToTerm("SELECT") + selectListNonTerm + whereClauseNonTerm;
            selectListNonTerm.Rule = columnItemListNonTerm | "*";
            columnItemListNonTerm.Rule = MakePlusRule(columnItemListNonTerm, ToTerm(","), idTerm);
            whereClauseNonTerm.Rule = Empty | "WHERE" + expressionNonTerm;

            //Update statement
            updateStatementNonTerm.Rule = ToTerm("UPDATE") + "SET" + assignListNonTerm + whereClauseNonTerm;
            assignListNonTerm.Rule = MakePlusRule(assignListNonTerm, ToTerm(","), assignmentNonTerm);
            assignmentNonTerm.Rule = idTerm + "=" + expressionNonTerm;

            //Delete statement
            deleteStatementNonTerm.Rule = ToTerm("DELETE") + whereClauseNonTerm;
 
            //Expression
            expressionListNonTerm.Rule = MakePlusRule(expressionListNonTerm, ToTerm(","), expressionNonTerm);
            expressionNonTerm.Rule = termNonTerm | unaryExpressionNonTerm | binaryExpressionNonTerm;
            termNonTerm.Rule = idTerm | stringLiteralTerm | numberTerm;
            unaryExpressionNonTerm.Rule = unaryOperatorNonTerm + termNonTerm;
            unaryOperatorNonTerm.Rule = ToTerm("NOT") | "-";
            binaryExpressionNonTerm.Rule = expressionNonTerm + binaryOperatorNonTerm + expressionNonTerm;
            binaryOperatorNonTerm.Rule = ToTerm("=") | ">" | "<" | ">=" | "<=" | "!="
                        | "AND" | "OR" | "LIKE" | "NOT" + "LIKE" | "IN" | "NOT" + "IN";
            notOperatorNonTerm.Rule = Empty | "NOT";
            inStatementNonTerm.Rule = expressionNonTerm + "IN" + "(" + expressionListNonTerm + ")";

            //Operators
            RegisterOperators(10, "*", "/", "%");
            RegisterOperators(9, "+", "-");
            RegisterOperators(8, "=", ">", "<", ">=", "<=", "<>", "!=", "!<", "!>", "LIKE", "IN");
            RegisterOperators(7, "^", "&", "|");
            RegisterOperators(6, "NOT");
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
