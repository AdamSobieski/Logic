/*********************************************************
* 
*  Authors:        Adam Sobieski
*
*********************************************************/

using Microsoft.ClearScript;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Logic.Prolog.Xsb
{
    public class XsbPrologQueryResult
    {
        internal XsbPrologQueryResult(XsbPrologTerm query, XsbPrologTerm answer)
        {
            m_query = query;
            m_answer = answer;
        }

        XsbPrologTerm m_query;
        XsbPrologTerm m_answer;

        [ScriptMember("query")]
        public XsbPrologTerm Query => m_query;
        [ScriptMember("answer")]
        public XsbPrologTerm Answer => m_answer;

        public XsbPrologTerm this[string variableName]
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }

    class XsbPrologQueryEnumerator : IEnumerator<XsbPrologQueryResult>
    {
        public XsbPrologQueryEnumerator(XsbPrologQuery query)
        {
            m_query = query;
            m_current = null;
        }
        XsbPrologQuery m_query;
        XsbPrologQueryResult m_current;

        public XsbPrologQueryResult Current => m_current;

        object IEnumerator.Current => m_current;

        public void Dispose()
        {
            if (!m_query.Closed)
            {
                m_query.Close();
            }
        }

        public bool MoveNext()
        {
            if (!m_query.Closed && (m_current == null || xsb.xsb_next() == 0))
            {
                m_current = new XsbPrologQueryResult(new XsbPrologTerm(xsb.reg_term(1)), new XsbPrologTerm(xsb.reg_term(2)));
                return true;
            }
            else
            {
                m_query.Close();
                return false;
            }
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }

    public class XsbPrologQuery : IEnumerable<XsbPrologQueryResult>
    {
        internal XsbPrologQuery(bool succeeded)
        {
            m_succeeded = succeeded;
            m_closed = !succeeded;
            m_answers = new XsbPrologQueryEnumerator(this);
        }

        bool m_succeeded;
        bool m_closed;
        XsbPrologQueryEnumerator m_answers;

        internal bool Closed => m_closed;

        internal void Close()
        {
            xsb.xsb_close_query();
            m_closed = true;
        }

        public IEnumerator<XsbPrologQueryResult> GetEnumerator()
        {
            return m_answers;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_answers;
        }
    }
}
