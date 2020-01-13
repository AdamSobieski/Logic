using System;
using System.Collections.Generic;

namespace Logic.Collections
{
    internal interface ITupleStore : ICloneable, IDisposable, IEnumerable<IRow>
    {
        bool Contains(out object value, params object[] tuple);
        void Store(object value, params object[] tuple);
        bool Remove(params object[] tuple);
        void Clear();

        new ITupleStore Clone();
    }
}
