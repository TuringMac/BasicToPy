﻿using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Text;

namespace BasicToPy
{
    public class PyVisitor : BasicBaseVisitor<string>
    {
        public override string VisitProgram([NotNull] BasicParser.ProgramContext context)
        {
            string result = "";
            for (int i = 0; i < context.ChildCount; i++)
            {
                string transedLine = Visit(context.line(i));
                Console.WriteLine(transedLine);
                result += transedLine + Environment.NewLine;
            }
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
            return $"{context.VAR().GetText()} = input({context.STRING().GetText()})";
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
            return context.GetText();
        }

        public override string VisitExpression([NotNull] BasicParser.ExpressionContext context)
        {
            return context.GetText();
            //string sign1 = "";
            //string pl = context.PLUS(0)?.GetText();
            //if (!string.IsNullOrWhiteSpace(pl))
            //    sign1 += pl;
            //string min1 = context.MINUS(0)?.GetText();
            //if (!string.IsNullOrWhiteSpace(min1))
            //    sign1 += min1;
            //string term1 = sign1 + Visit(context.term(0));

            //foreach(string term in context.)
            //return Visit(); // TODO add multiple Add/Sub operations
        }

        public override string VisitTerm([NotNull] BasicParser.TermContext context)
        {
            return context.GetText();
            //return Visit(context.factor(0)); // TODO add multiple Mul/Devide operations
            //return Visit(context.factor(0)) + " " + Visit(context.STAR(0)) + " " + Visit(context.factor(1));
        }

        public override string VisitFacVar([NotNull] BasicParser.FacVarContext context)
        {
            return context.vara().GetText();
        }

        public override string VisitFacNumber([NotNull] BasicParser.FacNumberContext context)
        {
            return context.number().GetText();
        }
    }
}