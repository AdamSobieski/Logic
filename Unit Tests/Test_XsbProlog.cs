/*********************************************************
* 
*  Author:        Adam Sobieski
*
*********************************************************/

using Logic.Prolog.Xsb.Callbacks;
using Logic.Prolog.Xsb.Initialization;
using Microsoft.ClearScript.V8;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

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
        public void Asserting_Facts()
        {
            prolog.Assert("p1(1)");
            prolog.Assert("p1(2)");
            prolog.Assert("p1(3)");

            Assert.IsTrue(prolog.Contains("p1(1)"));
            Assert.IsTrue(prolog.Contains("p1(2)"));
            Assert.IsTrue(prolog.Contains("p1(3)"));

            prolog.Retract("p1(1)");
            prolog.Retract("p1(2)");
            prolog.Retract("p1(3)");
        }

        [TestMethod]
        public void Retracting_Facts()
        {
            prolog.Assert("p2(1)");
            prolog.Assert("p2(2)");
            prolog.Assert("p2(3)");

            Assert.IsTrue(prolog.Contains("p2(1)"));
            Assert.IsTrue(prolog.Contains("p2(2)"));
            Assert.IsTrue(prolog.Contains("p2(3)"));

            prolog.Retract("p2(1)");
            prolog.Retract("p2(2)");
            prolog.Retract("p2(3)");

            Assert.IsFalse(prolog.Contains("p2(1)"));
            Assert.IsFalse(prolog.Contains("p2(2)"));
            Assert.IsFalse(prolog.Contains("p2(3)"));
        }

        [TestMethod]
        public void Asserting_Rules()
        {
            const string ErrorMessage = "Expected sequence and Prolog query result sequence are not equal.";

            prolog.Assert("p3(1, 1)");
            prolog.Assert("p3(2, 1)");
            prolog.Assert("p3(2, 2)");
            prolog.Assert("p3(3, 1)");

            prolog.Assert("p4(2)");

            prolog.AssertRule("p5(X)", "p3(X, X), p4(X)");

            Assert.IsTrue(prolog.Query("p5(X)").Select(r => r.Answer[1].ToInteger()).SequenceEqual(new int[] { 2 }), ErrorMessage);

            prolog.RetractRule("p5(X)", "p3(X, X), p4(X)");

            prolog.Retract("p4(2)");

            prolog.Retract("p3(1, 1)");
            prolog.Retract("p3(2, 1)");
            prolog.Retract("p3(2, 2)");
            prolog.Retract("p3(3, 1)");
        }

        [TestMethod]
        public void Retracting_Rules()
        {
            const string ErrorMessage = "Expected sequence and Prolog query result sequence are not equal.";

            prolog.Assert("p3(1, 1)");
            prolog.Assert("p3(2, 1)");
            prolog.Assert("p3(2, 2)");
            prolog.Assert("p3(3, 1)");

            prolog.Assert("p4(2)");

            prolog.AssertRule("p5(X)", "p3(X, X), p4(X)");

            Assert.IsTrue(prolog.Query("p5(X)").Select(r => r.Answer[1].ToInteger()).SequenceEqual(new int[] { 2 }), ErrorMessage);

            prolog.RetractRule("p5(X)", "p3(X, X), p4(X)");

            Assert.IsTrue(prolog.Query("p5(X)").Select(r => r.Answer[1].ToInteger()).SequenceEqual(new int[] { }), ErrorMessage);

            prolog.Retract("p4(2)");

            prolog.Retract("p3(1, 1)");
            prolog.Retract("p3(2, 1)");
            prolog.Retract("p3(2, 2)");
            prolog.Retract("p3(3, 1)");
        }

        [TestMethod]
        public void Foreign_Deterministic_Predicates()
        {
            prolog.AddPredicate("p6", 2, new XsbPrologCallback2((XsbPrologTerm x, XsbPrologTerm y) =>
            {
                return x.ToInteger() > y.ToInteger();
            }));

            Assert.IsTrue(prolog.Contains("p6(2, 1)"));
            Assert.IsFalse(prolog.Contains("p6(1, 2)"));
        }
    }
}