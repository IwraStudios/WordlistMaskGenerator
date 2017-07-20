using System;
using System.Collections.Generic;
using System.Text;

namespace Example
{
    class Menu
    {
        const string helptext = "1 - Use Default Masking example \n" +
            "2 - Use Custom Masking example \n";
        static void Main(string[] args)
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine(helptext);
                if (int.TryParse(Console.ReadKey().KeyChar.ToString(), out int b))
                {
                    Console.Clear();
                    switch (b)
                    {
                        case 1:
                            Default.DMain(null);
                            break;
                        case 2:
                            Custom.DMain(null);
                            break;
                    }
                }
            }
        }
    }
}
