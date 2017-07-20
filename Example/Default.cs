using System;
using WordMaskGenerator.Generators;

namespace Example
{
    public class Default
    {
        public static void DMain(string[] args)
        {
            string helptext = "Example program with options: \n"+
                "?s = Special Characters \n" +
                "?d = Decimal Characters \n" +
                "?u = Uppercase Characters \n"+
                "?l = Lowercase Characters \n" +
                "{*} = Lower & Uppercase everything within";
            Console.Write(helptext);
            Console.WriteLine();
            string read = Console.ReadLine();
            BasicMask Basics = new BasicMask(read);
            ConsoleForeach(Basics.GenerateList(100).ToArray());
            Console.WriteLine("First 100 possible solutions");
            Console.ReadLine();
        }


        static void ConsoleForeach<T>(T[] array)
        {
            foreach(T a in array)
            {
                Console.WriteLine(a);
            }
        }
    }
}