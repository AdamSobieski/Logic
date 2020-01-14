using Logic.Collections;
using Logic.Explanation;
using System.Collections.Generic;

namespace Logic
{
    public delegate IEnumerable<object> Rule(IKnowledgebase kb, object[] data, Mode mode);
}