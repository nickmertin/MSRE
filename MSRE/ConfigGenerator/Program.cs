using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConfigGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Enter file path: ");
            using (BinaryWriter w = new BinaryWriter(File.OpenWrite(Console.ReadLine())))
            {
                Console.Write("Press <d> for defaults, or <c> for custom: ");
                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.D:
                        w.Write(7.0);
                        w.Write(.05);
                        w.Write(2.0);
                        w.Write(2.0);
                        w.Write(100.0);
                        w.Write(2.0);
                        w.Write(120.0);
                        w.Write(1.8);
                        w.Write(60.0);
                        w.Write(2.0);
                        w.Write(5.0);
                        w.Write(2.5);
                        w.Write(.2);
                        w.Write(100.0);
                        w.Write(2.5);
                        break;
                    case ConsoleKey.C:
                        break;
                }
            }
        }
    }
}
