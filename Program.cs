using Antlr4.Runtime;
using System;
using System.IO;
using System.Text;

namespace BasicToPy
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                using (StreamReader fileStream = new StreamReader("BasicCode.bas"))
                {
                    AntlrInputStream inputStream = new AntlrInputStream(fileStream);
                    BasicLexer basicLexer = new BasicLexer(inputStream);
                    CommonTokenStream commonTokenStream = new CommonTokenStream(basicLexer);
                    BasicParser basicParser = new BasicParser(commonTokenStream);
                    var context = basicParser.program();
                    PyVisitor visitor = new PyVisitor();
                    string pyCode = visitor.Visit(context);
                    Console.WriteLine(pyCode);
                }
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
            }
        }
    }
}
