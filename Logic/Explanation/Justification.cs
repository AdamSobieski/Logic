using System.Collections.Generic;

namespace Logic.Explanation
{
    public enum Mode
    {
        OptimizeForSpeed = 0,
        ProvideJustification = 1
    }

    public sealed class Justification
    {
        public string Functor;
        public object[] Data;
        public List<RuleApplication> Derivations;
    }

    public sealed class RuleApplication
    {
        public string Rule;
        public List<Justification> Dependencies;
    }
}