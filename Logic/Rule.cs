using Logic.Collections;
using Logic.Explanation;
using System.Collections.Generic;

namespace Logic
{
    public delegate IEnumerable<Justification> Rule(IKnowledgebase kb, object[] data, JustificationSettings justification);
}