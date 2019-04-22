/*********************************************************
* 
*  Author:        Adam Sobieski
*
*********************************************************/

using Logic.Prolog.Callback;
using Microsoft.ClearScript.V8;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Logic.Prolog
{
    [TestClass]
    public class Prolog_Tests
    {
        static V8ScriptEngine v8;
        static PrologEngine prolog;

        [ClassInitialize]
        public static void Initialize(TestContext context)
        {
            v8 = new V8ScriptEngine();
            prolog = new PrologEngine();
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
            const string ErrorMessage = "Expected sequence and Prolog query result sequence are not equal.";

            var module = prolog.CreateModule();

            module.Assert("p(1)");
            module.Assert("p(2)");
            module.Assert("p(3)");

            Assert.IsTrue(module.Query("p(X)").Select(r => r["X"].ToInteger()).SequenceEqual(new int[] { 1, 2, 3 }), ErrorMessage);
        }

        [TestMethod]
        public void Retracting_Facts()
        {
            const string ErrorMessage = "Expected sequence and Prolog query result sequence are not equal.";

            var module = prolog.CreateModule();

            module.Assert("p(1)");
            module.Assert("p(2)");
            module.Assert("p(3)");

            Assert.IsTrue(module.Query("p(X)").Select(r => r["X"].ToInteger()).SequenceEqual(new int[] { 1, 2, 3 }), ErrorMessage);

            module.Retract("p(1)");
            module.Retract("p(2)");
            module.Retract("p(3)");

            Assert.IsTrue(module.Query("p(X)").Select(r => r["X"].ToInteger()).SequenceEqual(new int[] { }), ErrorMessage);
        }

        [TestMethod]
        public void Separation_of_Module_Content()
        {
            const string ErrorMessage = "Expected sequence and Prolog query result sequence are not equal.";

            var module1 = prolog.CreateModule();
            var module2 = prolog.CreateModule();

            module1.Assert("p(1)");
            module1.Assert("p(2)");
            module1.Assert("p(3)");

            module2.Assert("p(a)");
            module2.Assert("p(b)");
            module2.Assert("p(c)");

            Assert.IsTrue(module1.Query("p(X)").Select(r => r["X"].ToInteger()).SequenceEqual(new int[] { 1, 2, 3 }), ErrorMessage);

            Assert.IsTrue(module2.Query("p(X)").Select(r => r["X"].ToString()).SequenceEqual(new string[] { "a", "b", "c" }), ErrorMessage);

            //module1.Clear();
            //module2.Clear();
        }

        [TestMethod]
        public void Asserting_Rules()
        {
            const string ErrorMessage = "Expected sequence and Prolog query result sequence are not equal.";

            var module = prolog.CreateModule();

            module.Assert("p(1, 1)");
            module.Assert("p(2, 1)");
            module.Assert("p(2, 2)");
            module.Assert("p(3, 1)");

            module.Assert("p2(2)");

            module.AssertRule("p3(X)", "p(X, X), p2(X)");

            Assert.IsTrue(module.Query("p3(X)").Select(r => r["X"].ToInteger()).SequenceEqual(new int[] { 2 }), ErrorMessage);
        }

        [TestMethod]
        public void Retracting_Rules()
        {
            const string ErrorMessage = "Expected sequence and Prolog query result sequence are not equal.";

            var module = prolog.CreateModule();

            module.Assert("p(1, 1)");
            module.Assert("p(2, 1)");
            module.Assert("p(2, 2)");
            module.Assert("p(3, 1)");

            module.Assert("p2(2)");

            module.AssertRule("p3(X)", "p(X, X), p2(X)");

            Assert.IsTrue(module.Query("p3(X)").Select(r => r["X"].ToInteger()).SequenceEqual(new int[] { 2 }), ErrorMessage);

            module.RetractRule("p3(X)", "p(X, X), p2(X)");

            Assert.IsTrue(module.Query("p3(X)").Select(r => r["X"].ToInteger()).SequenceEqual(new int[] { }), ErrorMessage);
        }

        [TestMethod]
        public void Enumerating_Facts()
        {
            const string ErrorMessage = "Expected sequence and Prolog query result sequence are not equal.";

            var module = prolog.CreateModule();

            module.Assert("p(1, 1)");
            module.Assert("p(2, 1)");
            module.Assert("p(2, 2)");
            module.Assert("p(3, 1)");

            Assert.IsTrue(module.GetAssertions().Select(n => n.ToString()).SequenceEqual(new object[] { "p(1,1)", "p(2,1)", "p(2,2)", "p(3,1)" }), ErrorMessage);
        }

        [TestMethod]
        public void Enumerating_Rules()
        {
            const string ErrorMessage = "Expected sequence and Prolog query result sequence are not equal.";

            var module = prolog.CreateModule();

            module.AssertRule("p3(X)", "p(X, X), p2(X)");

            Assert.IsTrue(module.GetRules().Select(n => Regex.Replace(n.ToString(), "_[0-9]+", "_1")).SequenceEqual(new object[] { "p3(_1):-p(_1,_1),p2(_1)" }), ErrorMessage);
        }

        [TestMethod]
        public void Foreign_Deterministic_Predicates()
        {
            var module = prolog.CreateModule();

            module.AddPredicate("p", 2, new PrologCallback2((PrologTerm x, PrologTerm y) =>
            {
                return x.ToInteger() > y.ToInteger();
            }));

            Assert.IsTrue(module.Contains("p(2, 1)"));
            Assert.IsFalse(module.Contains("p(1, 2)"));
        }

        [TestMethod]
        public void Foreign_Nondeterministic_Predicates()
        {
            const string ErrorMessage = "Expected sequence and Prolog query result sequence are not equal.";

            var module = prolog.CreateModule();

            var data = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

            module.AddPredicateNondeterministic("p", 1, new PrologNondeterministicCallback1((PrologTerm x, dynamic context) =>
            {
                try
                {
                    ++context.counter;
                }
                catch
                {
                    context.counter = 0;
                }

                if (context.counter > 9) return false;

                x.Unify(data[context.counter]);
                return true;
            }));

            Assert.IsTrue(module.Query("p(X)").Select(r => r["X"].ToInteger()).Cast<int>().SequenceEqual(data), ErrorMessage);
        }

        [TestMethod]
        public void Creating_Prolog_in_Foreign_Deterministic_Predicate()
        {
            const string ErrorMessage = "Expected sequence and Prolog query result sequence are not equal.";

            var module = prolog.CreateModule();

            module.AddPredicate("p", 1, new PrologCallback1((PrologTerm x) =>
            {
                return x.Unify(prolog.Compound("z", prolog.Atom(1), prolog.Atom(2)));
            }));

            Assert.IsTrue(module.Query("p(X)").Select(r => r["X"].ToString()).SequenceEqual(new string[] { "z(1,2)" }), ErrorMessage);
        }
    }
}