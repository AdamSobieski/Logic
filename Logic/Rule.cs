using Logic.Collections;
using System.Collections.Generic;

namespace Logic
{
    public delegate IEnumerable<object> Rule(IKnowledgebase kb, object[] data);
}
