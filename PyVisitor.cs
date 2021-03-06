using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BasicToPy
{
    public class PyVisitor : BasicBaseVisitor<string>
    {
        HashSet<char> definedVars = new HashSet<char>();
        List<string> imports = new List<string>();
        Dictionary<string, string> definedSubs = new Dictionary<string, string>();
        string SubProg = ""; // current subprog processing
        List<string> subsMarks = new List<string>(); // links to subprogs defined by calls
        const string TAB = "    ";

        public override string VisitProgram([NotNull] BasicParser.ProgramContext context)
        {
            string result = "";
            for (int i = 0; i < context.ChildCount; i++)
            {
                if (context.line(i) == null) // Empty lines
                    continue;
                string transedLine = Visit(context.line(i));
                if (string.IsNullOrWhiteSpace(transedLine))
                    continue;
#if DEBUG
                Console.WriteLine(transedLine);
#endif
                result += transedLine + Environment.NewLine;
            }

            if (definedSubs.Count > 0)
                result = string.Join(Environment.NewLine, definedSubs.Values.ToArray()) + Environment.NewLine + result;
            if (imports.Count > 0)
                result = string.Join(Environment.NewLine, imports.ToArray()) + Environment.NewLine + result;

            return result;
        }

        public override string VisitLine([NotNull] BasicParser.LineContext context)
        {
            string lineMark = "";
            if (context.number() != null)
                lineMark = Visit(context.number());
            if (!string.IsNullOrWhiteSpace(SubProg))
            {
                definedSubs[SubProg] += Environment.NewLine + TAB + Visit(context.statement()).Replace(Environment.NewLine + TAB, Environment.NewLine + TAB + TAB); // Add extra TAB for if statement in the subproc
                return "";
            }
            if (!string.IsNullOrWhiteSpace(lineMark) && subsMarks.Contains(lineMark))
            {
                SubProg = lineMark;
                definedSubs[SubProg] = $"def f{lineMark}():" +
                    Environment.NewLine + TAB + Visit(context.statement()).Replace(Environment.NewLine + TAB, Environment.NewLine + TAB + TAB); // Add extra TAB for if statement in the subproc
                return "";
            }
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
            string relop = Visit(context.relop());
            string expr2 = Visit(context.expression(1));
            string stmt = Visit(context.statement());
            return $"if {expr1} {relop} {expr2}:" + Environment.NewLine + TAB + stmt;
        }

        public override string VisitStInputVarlist([NotNull] BasicParser.StInputVarlistContext context)
        {
            string tmpVars = Visit(context.varlist());
            string[] tmpVarsParts = tmpVars.Split(';', StringSplitOptions.RemoveEmptyEntries);
            char[] vars = tmpVarsParts.Select(v => v[0]).ToArray(); // Parse variables from varlist
            List<string> inputList = new List<string>();
            foreach (char var in vars)
            {
                if (!definedVars.Contains(var))
                    throw new Exception("ERROR: Using not defined variable! " + var);
                inputList.Add($"{var} = int(input())");
            }
            return string.Join(Environment.NewLine, inputList);
        }

        public override string VisitStGotoExpr([NotNull] BasicParser.StGotoExprContext context)
        {
            throw new Exception("ERROR: Unsupported instruction GOTO!");
        }

        public override string VisitVarlist([NotNull] BasicParser.VarlistContext context)
        {
            string result = (context.varlist() != null ? (Visit(context.varlist()) + ";") : "") + context.VAR().GetText();
            return result;
        }

        public override string VisitStLetVarAssign([NotNull] BasicParser.StLetVarAssignContext context)
        {
            char var = context.VAR().GetText()[0];
            if (!definedVars.Contains(var))
                throw new Exception("ERROR: Using not defined variable! " + var);
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

        public override string VisitStDim([NotNull] BasicParser.StDimContext context)
        {
            char var = context.VAR().GetText()[0];
            if (definedVars.Contains(var))
                throw new Exception("ERROR: Variable already defined!");
            else
                definedVars.Add(var);
            return "";
        }

        public override string VisitFacVar([NotNull] BasicParser.FacVarContext context)
        {
            return context.VAR().GetText();
        }

        public override string VisitFacNumber([NotNull] BasicParser.FacNumberContext context)
        {
            return context.number().GetText();
        }

        public override string VisitFacExpr([NotNull] BasicParser.FacExprContext context)
        {
            return "(" + Visit(context.expression()) + ")";
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

        public override string VisitStGosubExpr([NotNull] BasicParser.StGosubExprContext context)
        {
            string expr = Visit(context.expression());
            subsMarks.Add(expr);
            return $"f{expr}()";
        }

        public override string VisitStReturn([NotNull] BasicParser.StReturnContext context)
        {
            SubProg = "";
            return Environment.NewLine + Environment.NewLine;
        }

        public override string VisitNumber([NotNull] BasicParser.NumberContext context)
        {
            return context.GetText();
        }

        #region Undefined rules

        public override string VisitRelop([NotNull] BasicParser.RelopContext context)
        {
            return context.GetText();
        }

        #endregion Undefined rules
    }
}
