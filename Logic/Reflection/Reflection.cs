using Logic.Expressions;
using System;
using System.Reflection;

namespace Logic.Reflection
{
    public static class Builtin
    {
        public static RuleInfo GetRule(string qualifiedName)
        {
            int length = qualifiedName.Length;
            int firstComma = qualifiedName.IndexOf(',');
            string assemblyName = qualifiedName.Substring(firstComma + 1, length - (firstComma + 1)).TrimStart();
            string path = qualifiedName.Substring(0, firstComma);
            return Assembly.Load(assemblyName).GetRule(path);
        }
        public static FunctorInfo GetFunctor(string qualifiedName)
        {
            int length = qualifiedName.Length;
            int firstComma = qualifiedName.IndexOf(',');
            string assemblyName = qualifiedName.Substring(firstComma + 1, length - (firstComma + 1)).TrimStart();
            string path = qualifiedName.Substring(0, firstComma);
            return Assembly.Load(assemblyName).GetFunctor(path);
        }

        public static RuleInfo GetRule(this Assembly assembly, string name)
        {
            throw new NotImplementedException();
        }
        public static FunctorInfo GetFunctor(this Assembly assembly, string name)
        {
            throw new NotImplementedException();
        }
    }

    public abstract class RuleInfo
    {
        public abstract Assembly Assembly { get; }

        public abstract string Name { get; }
        public abstract string Namespace { get; }
        public string FullName => Namespace + "." + Name;

        public abstract FunctorInfo[] GetDependencies();

        public abstract RuleExpression Expression { get; }
    }

    public abstract class FunctorInfo
    {
        public abstract Assembly Assembly { get; }

        public abstract string Name { get; }
        public abstract string Namespace { get; }
        public string FullName => Namespace + "." + Name;

        public abstract Type FunctorType { get; }
        public abstract ParameterInfo[] GetParameters();
    }
}
