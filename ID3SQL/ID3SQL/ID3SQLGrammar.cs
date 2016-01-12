﻿using System;

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

        private ID3SQLGrammar() : base(false)
        {
            Terminal number = new NumberLiteral("number");
            Terminal string_literal = new StringLiteral("string", "'", StringOptions.AllowsDoubledQuote);
            Terminal Id_simple = TerminalFactory.CreateSqlExtIdentifier(this, "id_simple"); //covers normal identifiers (abc) and quoted id's ([abc d], "abc d")
            Terminal comma = ToTerm(",");
            Terminal dot = ToTerm(".");
            Terminal NOT = ToTerm("NOT");
            Terminal UPDATE = ToTerm("UPDATE");
            Terminal SET = ToTerm("SET");
            Terminal DELETE = ToTerm("DELETE");
            Terminal SELECT = ToTerm("SELECT");

            NonTerminal Id = new NonTerminal("Id");
            NonTerminal stmt = new NonTerminal("stmt");
            NonTerminal selectStmt = new NonTerminal("selectStmt");
            NonTerminal updateStmt = new NonTerminal("updateStmt");
            NonTerminal deleteStmt = new NonTerminal("deleteStmt");
            NonTerminal assignList = new NonTerminal("assignList");
            NonTerminal whereClauseOpt = new NonTerminal("whereClauseOpt");
            NonTerminal assignment = new NonTerminal("assignment");
            NonTerminal expression = new NonTerminal("expression");
            NonTerminal exprList = new NonTerminal("exprList");
            NonTerminal selList = new NonTerminal("selList");
            NonTerminal columnItemList = new NonTerminal("columnItemList");
            NonTerminal tuple = new NonTerminal("tuple");
            NonTerminal term = new NonTerminal("term");
            NonTerminal unExpr = new NonTerminal("unExpr");
            NonTerminal unOp = new NonTerminal("unOp");
            NonTerminal binExpr = new NonTerminal("binExpr");
            NonTerminal binOp = new NonTerminal("binOp");
            NonTerminal betweenExpr = new NonTerminal("betweenExpr");
            NonTerminal notOpt = new NonTerminal("notOpt");
            NonTerminal inStmt = new NonTerminal("inStmt");

            this.Root = stmt;

            //ID
            Id.Rule = MakePlusRule(Id, dot, Id_simple);

            stmt.Rule = selectStmt | updateStmt | deleteStmt;

            //Select stmt
            selectStmt.Rule = SELECT + selList + whereClauseOpt;
            selList.Rule = columnItemList | "*";
            columnItemList.Rule = MakePlusRule(columnItemList, comma, Id);
            whereClauseOpt.Rule = Empty | "WHERE" + expression;

            //Update stmt
            updateStmt.Rule = UPDATE + SET + assignList + whereClauseOpt;
            assignList.Rule = MakePlusRule(assignList, comma, assignment);
            assignment.Rule = Id + "=" + expression;

            //Delete stmt
            deleteStmt.Rule = DELETE + whereClauseOpt;
 
            //Expression
            exprList.Rule = MakePlusRule(exprList, comma, expression);
            expression.Rule = term | unExpr | binExpr;
            term.Rule = Id | string_literal | number | tuple;
            tuple.Rule = "(" + exprList + ")";
            unExpr.Rule = unOp + term;
            unOp.Rule = NOT | "+" | "-" | "~"; 
            binExpr.Rule = expression + binOp + expression;
            binOp.Rule = ToTerm("+") | "-" | "*" | "/" | "%" //arithmetic
                        | "&" | "|" | "^"                     //bit
                        | "=" | ">" | "<" | ">=" | "<=" | "<>" | "!=" | "!<" | "!>"
                        | "AND" | "OR" | "LIKE" | NOT + "LIKE" | "IN" | NOT + "IN" ; 
            betweenExpr.Rule = expression + notOpt + "BETWEEN" + expression + "AND" + expression;
            notOpt.Rule = Empty | NOT;
            inStmt.Rule = expression + "IN" + "(" + exprList + ")";

            //Operators
            RegisterOperators(10, "*", "/", "%"); 
            RegisterOperators(9, "+", "-");
            RegisterOperators(8, "=", ">", "<", ">=", "<=", "<>", "!=", "!<", "!>", "LIKE", "IN");
            RegisterOperators(7, "^", "&", "|");
            RegisterOperators(6, NOT); 
            RegisterOperators(5, "AND");
            RegisterOperators(4, "OR");

            MarkPunctuation(",", "(", ")", ";");
            //Note: we cannot declare binOp as transient because it includes operators "NOT LIKE", "NOT IN" consisting of two tokens. 
            // Transient non-terminals cannot have more than one non-punctuation child nodes.
            // Instead, we set flag InheritPrecedence on binOp , so that it inherits precedence value from it's children, and this precedence is used
            // in conflict resolution when binOp node is sitting on the stack
            base.MarkTransient(stmt, term, expression, unOp, tuple);
            binOp.SetFlag(TermFlags.InheritPrecedence); 
        }
    }
}
