/*********************************************************
* 
*  Authors:        Adam Sobieski, Uwe Lesta
*
*  This library is free software; you can redistribute it and/or
*  modify it under the terms of the GNU Lesser General Public
*  License as published by the Free Software Foundation; either
*  version 2.1 of the License, or (at your option) any later version.
*
*  This library is distributed in the hope that it will be useful,
*  but WITHOUT ANY WARRANTY; without even the implied warranty of
*  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
*  Lesser General Public License for more details.
*
*  You should have received a copy of the GNU Lesser General Public
*  License along with this library; if not, write to the Free Software
*  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA
*
*********************************************************/

using System;

namespace Logic.Prolog.Swi.Callback
{
    [Flags]
    public enum ForeignSwitches
    {
        /// <summary>0 - PL_FA_NOTHING: No flags.</summary>
        None = 0,
        /// <summary>1 - PL_FA_NOTRACE: Predicate cannot be seen in the tracer.</summary>
        NoTrace = 1,
        /// <summary>2 - PL_FA_TRANSPARENT: Predicate is module transparent.</summary>
        Transparent = 2,
        /// <summary>4 - PL_FA_NONDETERMINISTIC: Predicate is non-deterministic. See also PL_retry().</summary>
        /// <seealso href="http://gollem.science.uva.nl/SWI-Prolog/Manual/foreigninclude.html#PL_retry()">PL_retry()</seealso>
        Nondeterministic = 4,
        /// <summary>8 - PL_FA_VARARGS: Use alternative calling convention.</summary>
        VarArgs = 8,
    }

    public enum NondeterministicCalltype : int
    {
        FirstCall = 0,
        Pruned = 1,
        Redo = 2
    }

    public delegate IntPtr SwiForeignNondeterministicPredicateCallback0(IntPtr control_t);

    public delegate IntPtr SwiForeignNondeterministicPredicateCallback1(PrologTerm term, IntPtr control_t);

    public delegate IntPtr SwiForeignNondeterministicPredicateCallback2(PrologTerm term1, PrologTerm term2, IntPtr control_t);

    public delegate IntPtr SwiForeignNondeterministicPredicateCallback3(PrologTerm term1, PrologTerm term2, PrologTerm term3, IntPtr control_t);

    public delegate IntPtr SwiForeignNondeterministicPredicateCallback4(PrologTerm term1, PrologTerm term2, PrologTerm term3, PrologTerm term4, IntPtr control_t);

    public delegate IntPtr SwiForeignNondeterministicPredicateCallback5(PrologTerm term1, PrologTerm term2, PrologTerm term3, PrologTerm term4, PrologTerm term5, IntPtr control_t);

    public delegate IntPtr SwiForeignNondeterministicPredicateCallback6(PrologTerm term1, PrologTerm term2, PrologTerm term3, PrologTerm term4, PrologTerm term5, PrologTerm term6, IntPtr control_t);

    public delegate IntPtr SwiForeignNondeterministicPredicateCallback7(PrologTerm term1, PrologTerm term2, PrologTerm term3, PrologTerm term4, PrologTerm term5, PrologTerm term6, PrologTerm term7, IntPtr control_t);

    public delegate IntPtr SwiForeignNondeterministicPredicateCallback8(PrologTerm term1, PrologTerm term2, PrologTerm term3, PrologTerm term4, PrologTerm term5, PrologTerm term6, PrologTerm term7, PrologTerm term8, IntPtr control_t);



    public delegate bool PrologCallback0();

    public delegate bool PrologCallback1(PrologTerm term);

    public delegate bool PrologCallback2(PrologTerm term1, PrologTerm term2);

    public delegate bool PrologCallback3(PrologTerm term1, PrologTerm term2, PrologTerm term3);

    public delegate bool PrologCallback4(PrologTerm term1, PrologTerm term2, PrologTerm term3, PrologTerm term4);

    public delegate bool PrologCallback5(PrologTerm term1, PrologTerm term2, PrologTerm term3, PrologTerm term4, PrologTerm term5);

    public delegate bool PrologCallback6(PrologTerm term1, PrologTerm term2, PrologTerm term3, PrologTerm term4, PrologTerm term5, PrologTerm term6);

    public delegate bool PrologCallback7(PrologTerm term1, PrologTerm term2, PrologTerm term3, PrologTerm term4, PrologTerm term5, PrologTerm term6, PrologTerm term7);

    public delegate bool PrologCallback8(PrologTerm term1, PrologTerm term2, PrologTerm term3, PrologTerm term4, PrologTerm term5, PrologTerm term6, PrologTerm term7, PrologTerm term8);

    public delegate bool PrologNondeterministicCallback0(dynamic context);

    public delegate bool PrologNondeterministicCallback1(PrologTerm term, dynamic context);

    public delegate bool PrologNondeterministicCallback2(PrologTerm term1, PrologTerm term2, dynamic context);

    public delegate bool PrologNondeterministicCallback3(PrologTerm term1, PrologTerm term2, PrologTerm term3, dynamic context);

    public delegate bool PrologNondeterministicCallback4(PrologTerm term1, PrologTerm term2, PrologTerm term3, PrologTerm term4, dynamic context);

    public delegate bool PrologNondeterministicCallback5(PrologTerm term1, PrologTerm term2, PrologTerm term3, PrologTerm term4, PrologTerm term5, dynamic context);

    public delegate bool PrologNondeterministicCallback6(PrologTerm term1, PrologTerm term2, PrologTerm term3, PrologTerm term4, PrologTerm term5, PrologTerm term6, dynamic context);

    public delegate bool PrologNondeterministicCallback7(PrologTerm term1, PrologTerm term2, PrologTerm term3, PrologTerm term4, PrologTerm term5, PrologTerm term6, PrologTerm term7, dynamic context);

    public delegate bool PrologNondeterministicCallback8(PrologTerm term1, PrologTerm term2, PrologTerm term3, PrologTerm term4, PrologTerm term5, PrologTerm term6, PrologTerm term7, PrologTerm term8, dynamic context);
}
