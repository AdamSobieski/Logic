/*********************************************************
* 
*  Author:        Adam Sobieski
*
*********************************************************/

using Logic.Prolog.Swi;
using Microsoft.ClearScript.V8;
using System;

namespace Application
{
    public class Program
    {
        public static void Main(string[] args)
        {
            using (var engine = new V8ScriptEngine())
            {
                using (var prolog = new PrologEngine())
                {
                    engine.AddHostType("Console", typeof(Console));
                    engine.AddHostObject("prolog", prolog);

                    //var now = DateTime.Now;

                    engine.Execute(@"

                    var module1 = prolog.createModule('module1', { setting1: 'value', setting2: 'value' });
                    var module2 = prolog.createModule('module2');

                    module1.assert('p(1, 1)');
                    module1.assert('p(2, 1)');
                    module1.assert('p(3, 1)');

                    module2.assert('p(a, 1)');
                    module2.assert('p(b, 1)');
                    module2.assert('p(c, 1)');

                    for (var r of module1.query('p(X, Y)'))
                    {
                        Console.WriteLine('p({0}, {1})', r['X'], r['Y']);
                    }

                    Console.WriteLine();

                    for (var r of module2.query('p(X, Y)'))
                    {
                        Console.WriteLine('p({0}, {1})', r.X, r.Y);
                    }

                    Console.WriteLine();

                    module1.assertRule('p2(X)', 'p(X, X)');

                    for (var r of module1.query('p2(X)'))
                    {
                        Console.WriteLine('p2({0})', r.X);
                    }

                    Console.WriteLine();

                    for(var a of module1.getAssertions())
                    {
                        Console.WriteLine(a);
                    }

                    Console.WriteLine();

                    for(var r of module1.getRules())
                    {
                        Console.WriteLine(r);
                    }

                    Console.WriteLine();

                    module1.addPredicate('p3', 2, function(x, y)
                    {
                        return x.toInteger() > y.toInteger();
                    });

                    if (module1.contains('p3(2, 1)')) { Console.WriteLine('2 > 1'); }
                    if (!module1.contains('p3(1, 2)')) { Console.WriteLine('1 <= 2'); }

                    Console.WriteLine();

                    var data = [1, 2, 3, 4, 5, 6, 7, 8, 9, 10];

                    module1.addPredicateNondeterministic('p4', 1, function(x, context)
                    {
                        if(context.counter == undefined)
                        {
                            context.counter = 0;
                        }
                        else
                        {
                            if(++context.counter > 9) return false;
                        }

                        x.unify(data[context.counter]);
                        return true;
                    });

                    for(var r of module1.query('p4(X)'))
                    {
                        Console.WriteLine('X = {0}', r['X']);
                    }
                ");

                    //Console.WriteLine();
                    //Console.WriteLine("Elapsed: {0}", DateTime.Now - now);
                }
            }
        }
    }
}