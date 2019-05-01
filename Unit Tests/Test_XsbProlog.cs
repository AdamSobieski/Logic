using Logic.Prolog.Xsb;
using Logic.Prolog.Xsb.Initialization;
using Microsoft.ClearScript.V8;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Logic.Prolog.Xsb
{
    [TestClass]
    public class Test_XsbProlog
    {
        static V8ScriptEngine v8;
        static XsbPrologEngine prolog;

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            var settings = new XsbPrologInitializationSettings
            {
                HomeDirectory = @"C:\Program Files (x86)\XSB",
                BinaryDirectory = @"C:\Program Files (x86)\XSB\config\x64-pc-windows\bin",
                PrependBinaryDirectoryToPath = true
            };

            v8 = new V8ScriptEngine();
            prolog = new XsbPrologEngine(settings);
            v8.AddHostType("Console", typeof(Console));
            v8.AddHostObject("prolog", prolog);
        }

        [ClassCleanup]
        public static void Shutdown()
        {
            ((IDisposable)prolog).Dispose();
            ((IDisposable)v8).Dispose();
            v8 = null;
            prolog = null;
        }

        [TestMethod]
        public void Function()
        {

        }
    }
}