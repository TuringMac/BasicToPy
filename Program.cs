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
                string pyCode = "";
                using (StreamReader fileStream = new StreamReader("BasicCode.bas"))
                {
                    AntlrInputStream inputStream = new AntlrInputStream(fileStream);
                    BasicLexer basicLexer = new BasicLexer(inputStream);
                    CommonTokenStream commonTokenStream = new CommonTokenStream(basicLexer);
                    BasicParser basicParser = new BasicParser(commonTokenStream);
                    var context = basicParser.program();
                    PyVisitor visitor = new PyVisitor();
                    pyCode = visitor.Visit(context);
                }
                File.WriteAllText("python.py", pyCode);
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex);
            }
        }
    }
}
