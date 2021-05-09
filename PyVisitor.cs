using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Text;

namespace BasicToPy
{
    public class PyVisitor : BasicBaseVisitor<string>
    {
        HashSet<char> definedVars = new HashSet<char>();
        List<string> imports = new List<string>();
        public override string VisitProgram([NotNull] BasicParser.ProgramContext context)
        {
            string result = "";
            for (int i = 0; i < context.ChildCount; i++)
            {
                string transedLine = Visit(context.line(i));
                Console.WriteLine(transedLine);
                result += transedLine + Environment.NewLine;
            }
            if (imports.Count > 0)
                result = string.Join(Environment.NewLine, imports.ToArray()) + Environment.NewLine + result;
            return result;
        }

        public override string VisitLine([NotNull] BasicParser.LineContext context)
        {
            return Visit(context.statement());
        }

        public override string VisitStPrintExprList([NotNull] BasicParser.StPrintExprListContext context)
        {
            return $"print({Visit(context.exprlist())})";
        }

        public override string VisitExprlist([NotNull] BasicParser.ExprlistContext context)
        {
            return context.GetText();
        }

        public override string VisitStIfThen([NotNull] BasicParser.StIfThenContext context)
        {
            // if expr1 relop expr2:
            //     stmt
            string expr1 = Visit(context.expression(0));
            string relop = context.relop().GetText();
            string expr2 = Visit(context.expression(1));
            string stmt = Visit(context.statement());
            return $"if {expr1} {relop} {expr2}:\n\t{stmt}";
        }

        public override string VisitStInputVarlist([NotNull] BasicParser.StInputVarlistContext context)
        {
            char var = context.VAR().GetText()[0];
            string translated = $"{var} = int(input({context.STRING().GetText()}))";
            if (!definedVars.Contains(var))
                definedVars.Add(var);
            return translated;
        }

        public override string VisitStGotoExpr([NotNull] BasicParser.StGotoExprContext context)
        {
            throw new Exception("ERROR: Unsupported instruction GOTO!");
        }

        public override string VisitVarlist([NotNull] BasicParser.VarlistContext context)
        {
            string result = Visit(context.vara(1)) + " = " + Visit(context.vara(0));
            //for (int i = 0; i < context.ChildCount; i++)
            //{
            //    result += Visit(context.vara(i));
            //}
            return result;
        }

        public override string VisitVara([NotNull] BasicParser.VaraContext context)
        {
            return context.GetText();
        }

        public override string VisitStLetVarAssign([NotNull] BasicParser.StLetVarAssignContext context)
        {
            char var = context.VAR().GetText()[0];
            if (!definedVars.Contains(var))
                definedVars.Add(var);
            else if (!string.IsNullOrWhiteSpace(context.LET()?.GetText()))
                throw new Exception("ERROR: Variable already defined!");
            return $"{var} = {Visit(context.expression())}";
        }

        public override string VisitExpSingle([NotNull] BasicParser.ExpSingleContext context)
        {
            return context.sign?.Text + Visit(context.term());
        }

        public override string VisitExpDuo([NotNull] BasicParser.ExpDuoContext context)
        {
            return Visit(context.expression()) + " " + context.op.Text + " " + Visit(context.term());
        }

        //public override string VisitExpression([NotNull] BasicParser.ExpressionContext context)
        //{
        //    string result = context.sign?.Text + Visit(context.term(0));
        //    int count = context.ChildCount - 1;
        //    if (context.sign != null)
        //        count--;
        //    count = count / 2;
        //    for (int i = 1; i < count; i++)
        //    {
        //        result += context.op.Text + Visit(context.term(i));
        //    }

        //    return result;
        //}

        public override string VisitTermSingle([NotNull] BasicParser.TermSingleContext context)
        {
            return Visit(context.factor());
        }

        public override string VisitTermDuo([NotNull] BasicParser.TermDuoContext context)
        {
            return Visit(context.term()) + " " + context.op.Text + " " + Visit(context.factor());
        }

        //public override string VisitTerm([NotNull] BasicParser.TermContext context)
        //{
        //    char var = Visit(context.factor())[0];
        //    if (!char.IsUpper(var))
        //        return context.GetText();

        //    if (definedVars.Contains(var))
        //        return context.GetText();
        //    else
        //        throw new Exception("ERROR: Using not defined variable! " + var);
        //    //return Visit(context.factor(0)); // TODO add multiple Mul/Devide operations
        //    //return Visit(context.factor(0)) + " " + Visit(context.STAR(0)) + " " + Visit(context.factor(1));
        //}

        public override string VisitFacVar([NotNull] BasicParser.FacVarContext context)
        {
            char var = context.vara().GetText()[0];
            if (definedVars.Contains(var))
                return var.ToString();
            else
                throw new Exception("ERROR: Using not defined variable! " + var);
        }

        public override string VisitFacNumber([NotNull] BasicParser.FacNumberContext context)
        {
            return context.number().GetText();
        }

        public override string VisitStRem([NotNull] BasicParser.StRemContext context)
        {
            return "# " + context.STRING().GetText();
        }

        public override string VisitStRnd([NotNull] BasicParser.StRndContext context)
        {
            string rndImport = "import random";
            if (!imports.Contains(rndImport))
                imports.Add(rndImport);
            return context.VAR().GetText() + " = random.randrange(32565)";
        }
    }
}
