using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Interpreter;
using Irony.Interpreter.Ast;
using Irony.Parsing;

namespace Provisioning.Expressions
{
    [Language("Expression", "1.0", "Simple expressions used in the SDP configuration file.")]

    public class Grammar : InterpretedLanguageGrammar

    {

        public Grammar()

            : base(caseSensitive: false)

        {

            var number = new NumberLiteral("number");

            number.DefaultIntTypes = new TypeCode[] { TypeCode.Int32, TypeCode.Int64, NumberLiteral.TypeCodeBigInt };

            var stringLiteral = new StringLiteral("string", "\"");

            var identifier = new IdentifierTerminal("identifier");

            var comma = ToTerm(",");



            var Expr = new NonTerminal("Expr");

            var Term = new NonTerminal("Term");

            var BinExpr = new NonTerminal("BinExpr", typeof(BinaryOperationNode));

            var ParExpr = new NonTerminal("ParExpr");

            var UnExpr = new NonTerminal("UnExpr", typeof(UnaryOperationNode));

            var TernaryIfExpr = new NonTerminal("TernaryIf", typeof(IfNode));

            var ArgList = new NonTerminal("ArgList", typeof(ExpressionListNode));

            var FunctionCall = new NonTerminal("FunctionCall", typeof(FunctionCallNode));

            var MemberAccess = new NonTerminal("MemberAccess", typeof(MemberAccessNode));

            var IndexedAccess = new NonTerminal("IndexedAccess", typeof(IndexedAccessNode));

            var UnOp = new NonTerminal("UnOp");

            var BinOp = new NonTerminal("BinOp", "operator");

            var PrefixIncDec = new NonTerminal("PrefixIncDec", typeof(IncDecNode));

            var PostfixIncDec = new NonTerminal("PostfixIncDec", typeof(IncDecNode));

            var IncDecOp = new NonTerminal("IncDecOp");

            var Program = new NonTerminal("Program", typeof(ExpressionListNode));



            Expr.Rule = Term | UnExpr | BinExpr | PrefixIncDec | PostfixIncDec | TernaryIfExpr;

            Term.Rule = number | ParExpr | stringLiteral | FunctionCall | identifier | MemberAccess | IndexedAccess;

            ParExpr.Rule = "(" + Expr + ")";

            UnExpr.Rule = UnOp + Term + ReduceHere();

            UnOp.Rule = ToTerm("+") | "-";

            BinExpr.Rule = Expr + BinOp + Expr;

            BinOp.Rule = ToTerm("+") | "-" | "*" | "/" | "**" | "==" | "<" | "<=" | ">" | ">=" | "!=" | "&&" | "||" | "&" | "|";

            PrefixIncDec.Rule = IncDecOp + identifier;

            PostfixIncDec.Rule = identifier + PreferShiftHere() + IncDecOp;

            IncDecOp.Rule = ToTerm("++") | "--";

            TernaryIfExpr.Rule = Expr + "?" + Expr + ":" + Expr;

            MemberAccess.Rule = Expr + PreferShiftHere() + "." + identifier;

            ArgList.Rule = MakeStarRule(ArgList, comma, Expr);

            FunctionCall.Rule = Expr + PreferShiftHere() + "(" + ArgList + ")";

            FunctionCall.NodeCaptionTemplate = "call #{0}(...)";

            IndexedAccess.Rule = Expr + PreferShiftHere() + "[" + Expr + "]";

            Program.Rule = Expr | Empty;



            this.Root = Program;



            RegisterOperators(10, "?");

            RegisterOperators(15, "&", "&&", "|", "||");

            RegisterOperators(20, "==", "<", "<=", ">", ">=", "!=");

            RegisterOperators(30, "+", "-");

            RegisterOperators(40, "*", "/");

            RegisterOperators(50, Associativity.Right, "**");



            MarkPunctuation("(", ")", "?", ":", "[", "]");

            RegisterBracePair("(", ")");

            RegisterBracePair("[", "]");

            MarkTransient(Term, Expr, BinOp, UnOp, IncDecOp, ParExpr);



            MarkNotReported("++", "--");

            AddToNoReportGroup("(", "++", "--");

            AddOperatorReportGroup("operator");



            this.LanguageFlags = LanguageFlags.CreateAst | LanguageFlags.SupportsBigInt;

        }



        public override LanguageRuntime CreateRuntime(LanguageData language)

        {

            return new ExpressionRuntime(language);

        }

    }
}
