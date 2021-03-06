using Antlr4.Runtime;
using System;
using System.IO;
using System.Text;

namespace BasicToPy
{
    class Program
    {
        /// <summary>
        /// -test arg for autotest
        /// -f BasicCode.bas
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                if (string.Equals(args[0], "-tests"))
                {
                    var filenames = Directory.GetFiles("Tests", "*.bas");
                    foreach (var filename in filenames)
                        Translate(filename);
                }
                else if (args.Length == 2 && string.Equals(args[0], "-f"))
                {
                    Translate(args[1]);
                }
            }
            else
                Console.WriteLine("BasicToPy.exe [-test] [-f <filename.bas>]");
            Console.ReadKey();
        }

        static void Translate(string filename)
        {
            try
            {
                Console.WriteLine("---=== " + filename + " ===---");
                string pyCode = "";
                using (StreamReader fileStream = new StreamReader(filename))
                {
                    AntlrInputStream inputStream = new AntlrInputStream(fileStream);
                    BasicLexer basicLexer = new BasicLexer(inputStream);
                    CommonTokenStream commonTokenStream = new CommonTokenStream(basicLexer);
                    BasicParser basicParser = new BasicParser(commonTokenStream);
                    PyErrorListener pyErrorListener = new PyErrorListener();
                    basicParser.AddErrorListener(pyErrorListener);
                    var context = basicParser.program();
                    if (pyErrorListener.ExceptionList.Count > 0)
                    {
                        foreach (var msg in pyErrorListener.ExceptionList)
                            Console.WriteLine(msg);
                    }
                    else
                    {
                        PyVisitor visitor = new PyVisitor();
                        pyCode = visitor.Visit(context);
                        File.WriteAllText(Path.ChangeExtension(filename, "py"), pyCode);
                        Console.WriteLine("Successfull translated");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(filename + " Error: " + ex);
            }
        }
    }
}
