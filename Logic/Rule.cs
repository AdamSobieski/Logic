using Logic.Collections;
using Logic.Explanation;
using System.Collections.Generic;

namespace Logic
{
    public sealed class RuleSettings
    {
        public JustificationSettings Justification;
    }

    public delegate IEnumerable<Justification> Rule(IKnowledgebase kb, object[] pattern, RuleSettings settings);
}