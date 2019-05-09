/*********************************************************
* 
*  Author:        Adam Sobieski
*
*********************************************************/

using Logic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace Logic.Prolog.Expressions
{
    [TestClass]
    public class Test_Expressions
    {
        [TestMethod]
        public void Initialization()
        {
            var u = Expression.UniversalSet;
            var e = Expression.EmptySet;

            var x = Expression.Variable();
            var y = Expression.Variable();

            var p = Expression.Predicate(null, "p", 2);
            var c = Expression.Compound(p, x.Parameter, y);

            x = x.AddConstraint(c);
        }
    }
}
