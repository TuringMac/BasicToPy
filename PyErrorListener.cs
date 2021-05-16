using Antlr4.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BasicToPy
{
    class PyErrorListener : BaseErrorListener, IAntlrErrorListener<int>
    {
        public List<string> ExceptionList { get; private set; } = new List<string>();
        public override void SyntaxError(TextWriter output, IRecognizer recognizer, IToken offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
        {
            ExceptionList.Add(msg);
            base.SyntaxError(output, recognizer, offendingSymbol, line, charPositionInLine, msg, e);
        }

        public void SyntaxError(TextWriter output, IRecognizer recognizer, int offendingSymbol, int line, int charPositionInLine, string msg, RecognitionException e)
        {
            ExceptionList.Add(msg);
        }
    }
}
