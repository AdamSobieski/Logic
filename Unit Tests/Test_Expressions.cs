/*********************************************************
* 
*  Author:        Adam Sobieski
*
*********************************************************/

using Logic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace Logic.Expressions
{
    [TestClass]
    public class Test_Expressions
    {
        [TestMethod]
        public void Initialization()
        {
            var u = Expression.UniversalSet;
            var e = Expression.EmptySet;

            var X = Expression.Variable();
            var Y = Expression.Variable();

            var p = Expression.Predicate(null, "p", 2);
            var c = Expression.Compound(p, X.Parameters[0], Y);

            X = X.AddConstraint(c);
        }
    }
}
