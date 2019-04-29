/*********************************************************
* 
*  Author:        Adam Sobieski
*
*********************************************************/

using Logic.Prolog.Swi;
using Microsoft.ClearScript.V8;
using System;
using System.IO;

namespace Application
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using (var engine = new V8ScriptEngine())
            {
                using (var prolog = new SwiPrologEngine())
                {
                    engine.AddHostType("Console", typeof(Console));
                    engine.AddHostObject("prolog", prolog);

                    string script1 = File.OpenText("script1.js").ReadToEnd();

                    //var now = DateTime.Now;

                    engine.Execute(script1);

                    //Console.WriteLine();
                    //Console.WriteLine("Elapsed: {0}", DateTime.Now - now);
                }
            }
        }
    }
}