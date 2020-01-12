using System.Collections.Generic;

namespace Logic.Collections
{
    public interface ITupleStore : IEnumerable<IRow>
    {
        bool Contains(out object value, params object[] tuple);
        void Store(object value, params object[] tuple);
        bool Remove(params object[] tuple);
        void Clear();
    }
}
