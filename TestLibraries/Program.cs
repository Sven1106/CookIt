using CstLemmaLibrary;
using System;
using System.Collections.Generic;

namespace TestLibraries
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> strings = new List<string>() { new string("hello") };
            Dictionary<string, List<string>> lemmasByIngredientName = CstLemmaWrapper.GetLemmasByTextDictionary(strings);
            Console.WriteLine("Hello World!");

        }
    }
}
