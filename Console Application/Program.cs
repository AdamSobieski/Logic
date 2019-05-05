/*********************************************************
* 
*  Author:        Adam Sobieski
*
*********************************************************/

using Logic.Prolog.Swi;
using Logic.Prolog.Swi.Initialization;
using Logic.Prolog.Xsb;
using Logic.Prolog.Xsb.Initialization;
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

            var xsb_settings = new XsbPrologInitializationSettings
            {
                HomeDirectory = @"C:\Program Files (x86)\XSB",
                BinaryDirectory = @"C:\Program Files (x86)\XSB\config\x64-pc-windows\bin",
                PrependBinaryDirectoryToPath = true
            };

            using (var engine = new V8ScriptEngine())
            {
                engine.AddHostType("Console", typeof(Console));

                if (Directory.Exists(swi_settings.HomeDirectory))
                {
                    using (var swi = new SwiPrologEngine(swi_settings))
                    {
                        Console.WriteLine("SWI Prolog " + swi.Version);

                        engine.AddHostObject("swi", swi);

                        string script1 = File.OpenText("script1.js").ReadToEnd();

                        engine.Execute(script1);

                        Console.WriteLine();
                    }
                }

                if (Directory.Exists(xsb_settings.HomeDirectory))
                {
                    using (var xsb = new XsbPrologEngine(xsb_settings))
                    {
                        Console.WriteLine("XSB Prolog " + xsb.Version);

                        engine.AddHostObject("xsb", xsb);

                        string script2 = File.OpenText("script2.js").ReadToEnd();

                        engine.Execute(script2);

                        Console.WriteLine();
                    }
                }
            }
        }
    }
}