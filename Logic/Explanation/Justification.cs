using System;
using System.Collections.Generic;

namespace Logic.Explanation
{
    public enum JustificationSettings
    {
        Optimize = 0,
        ProvideJustification = 1
    }

    public sealed class Justification : IEquatable<Justification>
    {
        public string Functor;
        public object[] Data;
        public List<RuleApplication> Derivations;

        public override int GetHashCode()
        {
            int hash = Functor.GetHashCode();
            int n = Data.Length;
            for (int i = 0; i < n; ++i)
            {
                hash = (hash * 33) ^ Data[i].GetHashCode();
            }
            n = Derivations.Count;
            for (int i = 0; i < n; ++i)
            {
                hash = (hash * 33) ^ Derivations[i].GetHashCode();
            }
            return hash;
        }
        public override bool Equals(object obj)
        {
            if (obj is Justification other)
            {
                if (!Functor.Equals(other.Functor)) return false;
                int n = Data.Length;
                if (other.Data.Length != n) return false;
                for (int i = 0; i < n; ++i)
                {
                    if (!Data[i].Equals(other.Data[i])) return false;
                }
                n = Derivations.Count;
                if (other.Derivations.Count != n) return false;
                for (int i = 0; i < n; ++i)
                {
                    if (!Derivations[i].Equals(other.Derivations[i])) return false;
                }
                return true;
            }
            else return false;
        }
        public bool Equals(Justification other)
        {
            if (!Functor.Equals(other.Functor)) return false;
            int length = Data.Length;
            if (other.Data.Length != length) return false;
            for (int i = 0; i < length; ++i)
            {
                if (!Data[i].Equals(other.Data[i])) return false;
            }
            length = Derivations.Count;
            if (other.Derivations.Count != length) return false;
            for (int i = 0; i < length; ++i)
            {
                if (!Derivations[i].Equals(other.Derivations[i])) return false;
            }
            return true;
        }
    }

    public sealed class RuleApplication : IEquatable<RuleApplication>
    {
        public string Rule;
        public List<Justification> Dependencies;

        public override int GetHashCode()
        {
            int hash = Rule.GetHashCode();
            int n = Dependencies.Count;
            for (int i = 0; i < n; ++i)
            {
                hash = (hash * 33) ^ Dependencies[i].GetHashCode();
            }
            return hash;
        }
        public override bool Equals(object obj)
        {
            if (obj is RuleApplication other)
            {
                if (!Rule.Equals(other.Rule)) return false;
                int n = Dependencies.Count;
                for (int i = 0; i < n; ++i)
                {
                    if (!Dependencies[i].Equals(other.Dependencies[i])) return false;
                }
                return true;
            }
            else return false;
        }
        public bool Equals(RuleApplication other)
        {
            if (!Rule.Equals(other.Rule)) return false;
            int n = Dependencies.Count;
            for (int i = 0; i < n; ++i)
            {
                if (!Dependencies[i].Equals(other.Dependencies[i])) return false;
            }
            return true;
        }
    }
}