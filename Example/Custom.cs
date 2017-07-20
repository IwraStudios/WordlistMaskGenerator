using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WordMaskGenerator;

namespace Example
{
    public class Custom
    {
        public static void DMain(string[] args)
        {
            string helptext = "Example program with options: \n" +
                "?s = Special Characters \n" +
                "?d = Decimal Characters \n" +
                "?u = Uppercase Characters \n" +
                "?l = Lowercase Characters \n" +
                "[*] = Lower & Uppercase everything within && Any number can be 0-9";
            Console.Write(helptext);
            Console.WriteLine();
            string read = Console.ReadLine();
            CustomMask Basics = new CustomMask(read);
            ConsoleForeach(Basics.GenerateList(100).ToArray());
            Console.WriteLine("First 100 possible solutions");
            Console.ReadLine();
        }
        static void ConsoleForeach<T>(T[] array)
        {
            foreach (T a in array)
            {
                Console.WriteLine(a);
            }
        }
    }
    public class CustomMask
    {
        string value;
        Dictionary<string, object> ActiveMasks = new Dictionary<string, object>();
        public CustomMask(string mask)
        {
            value = mask;
            WordMaskGenerator.Masks.BasicMasks b = new WordMaskGenerator.Masks.BasicMasks();
            ActiveMasks.AddTuple(b.IncludeAlphabetUpper());
            ActiveMasks.AddTuple(b.IncludeAlphabetLower());
            //ActiveMasks.AddTuple(b.IncludeKownUpperLower()); not needed but could be added
            ActiveMasks.AddTuple(IncludeKownUpperLowerWithAnyNumber());
            ActiveMasks.AddTuple(b.IncludeNumbers());
        }

        public List<string> GenerateList(ulong amount)
        {
            string[] MasksToUse = Generic.GetMasks(value, ActiveMasks).ToArray();
            if (amount > Generic.PossibleCombinations(MasksToUse))
            {
                throw new IndexOutOfRangeException("more asked than possible: only" + Generic.PossibleCombinations(MasksToUse).ToString() + " possible");
            }
            return Generic.GenerateList(MasksToUse, amount);

        }

        public Tuple<string, Func<string, string[]>> IncludeKownUpperLowerWithAnyNumber()
        {
            //Will match {ABCDEFG}
            return Tuple.Create("[".RegexEscape() + ".*" + "]".RegexEscape(), GetLowerUpperWithAnyNumber());
        }

        public Func<string, string[]> GetLowerUpperWithAnyNumber()
        {

            ///Does transformation like this:
            ///[abc123] -> {aA}{bB}{cC}{0-9}{0-9}{0-9}
            return (text) =>
            {
                List<string> masks = new List<string>();
                //Assume [*]
                text = text.Remove(0, 1);//remove:[
                text = text.Remove(text.Length - 1, 1);//remove: ]
                foreach (char c in text)
                {
                    if (byte.TryParse(c.ToString(), out byte numb))
                    {
                        masks.Add(new string(WordMaskGenerator.Alphabets.AlphabetProvider.GetNumbers().ToArray()));
                        continue;
                    }

                    string a = c.ToString();
                    masks.Add(c.ToString() + c.ToString().ToUpper()); //Ex. a => a + A
                }
                return masks.ToArray();
            };
        }


    }
}
