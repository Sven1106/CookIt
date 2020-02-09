using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CstLemmaLibrary;
using UdpipeLibrary;

namespace NLP
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> sentences = new List<string>() { };
            sentences.Add("Finthakkede mandler");
            var bla =  UdpipeWrapper.GetSentenceFromTexts(sentences);
            var blub = CstLemmaWrapper.GetLemmasByTextDictionary(sentences);
        }

    }
}
