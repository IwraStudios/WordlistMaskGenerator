using System;
using System.Collections.Generic;
using System.Linq;

namespace WordMaskGenerator.Generators
{
    public class BasicMask
    {
        string value;
        Dictionary<string, object> ActiveMasks = new Dictionary<string, object>();
        //Allowed object types(atm):
        //Func<String>
        //Func<String,String>
        //Func<String,String[]>
        public BasicMask(string mask)
        {
            value = mask;
            Masks.BasicMasks b = new Masks.BasicMasks();
            ActiveMasks.AddTuple(b.IncludeAlphabetUpper());
            ActiveMasks.AddTuple(b.IncludeAlphabetLower());
            ActiveMasks.AddTuple(b.IncludeKownUpperLower());
            ActiveMasks.AddTuple(b.IncludeNumbers());
            ActiveMasks.AddTuple(b.IncludeSpecial());
        }
        
        public List<string> GenerateList(ulong amount)
        {
            string[] MasksToUse = Generic.GetMasks(value,ActiveMasks).ToArray();
            if(amount > Generic.PossibleCombinations(MasksToUse))
            {
                throw new IndexOutOfRangeException("more asked than possible: only" + Generic.PossibleCombinations(MasksToUse).ToString() + " possible");
            }
            return Generic.GenerateList(MasksToUse, amount);

        }

    }
}

namespace WordMaskGenerator
{

    public static class Generic
    {
        public static ulong PossibleCombinations(string[] Mask)
        {
            ulong possibleAmount = 1;
            foreach (string s in Mask) possibleAmount *= (uint)s.Length;
            return possibleAmount;
        }

        public static List<string> GetMasks(string input, Dictionary<string, object> LookupMasks)
        {
            List<string> masks = new List<string>();
            string[] ToUse = input.SplitRegX(LookupMasks.Keys.ToArray());
            ToUse = ToUse.RemoveEmptyStrings();

            foreach (string key in ToUse)
            {
                string rkey = LookupMasks.Where(x => System.Text.RegularExpressions.Regex.Match(key, x.Key).Success).Single().Key;
                //Func<String>
                //Func<String,String>
                //Func<String,String[]>
                if (LookupMasks[rkey] is Func<string> a)
                {
                    masks.Add(a());
                }

                if (LookupMasks[rkey] is Func<string, string> b)
                {
                    masks.Add(b(key));
                }

                if (LookupMasks[rkey] is Func<string, string[]> c)
                {
                    masks.AddRange(c(key));
                }
                //ActiveMasks[key](ToUse);
            }
            return masks;
        }


        public static List<string> GenerateList(string[] Mask, ulong amount)
        {
            List<string> list = new List<string>();
            for (ulong j = 0; j < amount; j++)
            {
                char[] newString = new char[Mask.Length];
                ulong seed = j;
                for (int c = 0, i = Mask.Length - 1; i > -1; i--, c++)
                {
                    int Length = Mask[i].Length;
                    ulong thiss = seed % (uint)Length;
                    seed = seed / (uint)Length;
                    newString[i] = Mask[i].ToCharArray()[thiss];
                }
                list.Add(new string(newString));
            }
            //return new string(newString);
            return list;
        }
    }


    public static class DictExtensions {
        public static void AddTuple<T,TD>(this Dictionary<T, object> dict, Tuple<T, TD> value)
        {
            dict.Add(value.Item1, value.Item2);
        }
    }

    public static class ArrayListExtensions
    {
        public static string[] RemoveEmptyStrings(this string[] arr)
        {
            List<string> oarr = arr.ToList();
            oarr.RemoveAll(x => x == "");
            return oarr.ToArray();
        }
    }

    public static class StringExtensions
    {
        public static string[] SplitRegX(this string text, string[] regexs)
        {
            //string input = "123xx456yy789";
            string pattern = "(" + String.Join("|", regexs) + ")";
            string[] result = System.Text.RegularExpressions.Regex.Split(text, pattern);
            return result;
        }

        public static string RegexEscape(this string text)
        {
            return System.Text.RegularExpressions.Regex.Escape(text);
        }

        public static IEnumerable<string> SplitX(this string text, string[] delimiters)
        {
            var split = text.Split(delimiters, StringSplitOptions.None);

            foreach (string part in split)
            {
                yield return part;
                text = text.Substring(part.Length);

                string delim = delimiters.FirstOrDefault(x => text.StartsWith(x));
                if (delim != null)
                {
                    yield return delim;
                    text = text.Substring(delim.Length);
                }
            }
        }
    }
}

namespace WordMaskGenerator.Masks
{
    public class BasicMasks
    {

        Alphabets.GetAlphabetFunc builtin_alpha;
        public BasicMasks(){
            builtin_alpha = new Alphabets.GetAlphabetFunc();
        }

        public Tuple<string, Func<string, string[]>> IncludeKownUpperLower()
        {
            //Will match {ABCDEFG}
            return Tuple.Create("{".RegexEscape() + ".*" + "}".RegexEscape(), new InputFunc.GetBuiltinFunc().GetWithLowerUpper());
        }

        public Tuple<string, Func<string>> IncludeAlphabetLower()
        {
            return Tuple.Create("?l".RegexEscape(), builtin_alpha.GetAlphabet());
        }

        public Tuple<string, Func<string>> IncludeAlphabetUpper()
        {
            return Tuple.Create("?u".RegexEscape(), builtin_alpha.GetAlphabetUpper());
        }
        public Tuple<string, Func<string>> IncludeSpecial()
        {
            return Tuple.Create("?s".RegexEscape(), builtin_alpha.GetSpecial());
        }
        public Tuple<string, Func<string>> IncludeNumbers()
        {
            return Tuple.Create("?d".RegexEscape(), builtin_alpha.GetNumbers());
        }


    }
}

namespace WordMaskGenerator.InputFunc
{
    public class GetBuiltinFunc
    {
        public Func<string, string[]> GetWithLowerUpper()
        {
            return (text) =>
            {
                List<string> masks = new List<string>();
                //Assume {*}
                text = text.Remove(0,1);//remove:{
                text = text.Remove(text.Length - 1, 1);//remove: }
                foreach (char c in text)
                {
                    string a = c.ToString();
                    masks.Add(c.ToString().ToLower() + c.ToString().ToUpper()); //Ex. a => a + A
                }
                return masks.ToArray();
            };
        }

    }
}

namespace WordMaskGenerator.Alphabets
{

    public class GetAlphabetFunc
    {
        public Func<string> GetAlphabet()
        {
            return () => new string(AlphabetProvider.GetAlphabet().ToArray());
        }

        public Func<string> GetAlphabetUpper()
        {
            return () => new string(AlphabetProvider.GetAlphabet().ToArray()).ToUpper();
        }

        public Func<string> GetNumbers()
        {
            return () => new string(AlphabetProvider.GetNumbers().ToArray());
        }
        public Func<string> GetSpecial()
        {
            return () => new string(AlphabetProvider.GetSpecialCharacters().ToArray());
        }
    }

    public static class AlphabetProvider
    {
        public static System.Collections.Generic.IEnumerable<char> GetAlphabet()
        {
            for (char c = 'a'; c <= 'z'; c++)
            {
                yield return c;
            }
        }

        public static  System.Collections.Generic.IEnumerable<char> GetNumbers()
        {
            for(char c = '0'; c <= '9'; c++)
            {
                yield return c;
            }
        }

        public static System.Collections.Generic.IEnumerable<char> GetSpecialCharacters()
        {
            for (char c = '!'; c <= '/'; c++)
            {
                yield return c;
            }
            for (char c = ':'; c <= '@'; c++)
            {
                yield return c;
            }
            for (char c = '['; c <= '`'; c++)
            {
                yield return c;
            }
            for (char c = '{'; c <= '~'; c++)
            {
                yield return c;
            }
        }

    }
}