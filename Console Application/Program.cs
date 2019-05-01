/*********************************************************
* 
*  Author:        Adam Sobieski
*
*********************************************************/

using Logic.Prolog.Swi;
using Logic.Prolog.Swi.Initialization;
using Microsoft.ClearScript.V8;
using System;
using System.IO;

namespace Application
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var swi_settings = new SwiPrologInitializationSettings
            {
                HomeDirectory = @"C:\Program Files\swipl",
                SetHomeDirectoryEnvironmentVariable = true,
                BinaryDirectory = @"C:\Program Files\swipl\bin",
                PrependBinaryDirectoryToPath = true
            };

            using (var engine = new V8ScriptEngine())
            {
                using (var swi = new SwiPrologEngine(swi_settings))
                {
                    engine.AddHostType("Console", typeof(Console));
                    engine.AddHostObject("prolog", swi);

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