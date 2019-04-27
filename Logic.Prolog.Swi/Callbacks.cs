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

namespace Logic.Prolog.Swi.Callbacks
{
    [Flags]
    internal enum SwiForeignSwitches
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

    internal enum SwiNondeterministicCalltype : int
    {
        FirstCall = 0,
        Pruned = 1,
        Redo = 2
    }

    internal delegate IntPtr SwiNativeForeignNondeterministicPredicateCallback0(IntPtr control_t);

    internal delegate IntPtr SwiNativeForeignNondeterministicPredicateCallback1(SwiPrologTerm term, IntPtr control_t);

    internal delegate IntPtr SwiNativeForeignNondeterministicPredicateCallback2(SwiPrologTerm term1, SwiPrologTerm term2, IntPtr control_t);

    internal delegate IntPtr SwiNativeForeignNondeterministicPredicateCallback3(SwiPrologTerm term1, SwiPrologTerm term2, SwiPrologTerm term3, IntPtr control_t);

    internal delegate IntPtr SwiNativeForeignNondeterministicPredicateCallback4(SwiPrologTerm term1, SwiPrologTerm term2, SwiPrologTerm term3, SwiPrologTerm term4, IntPtr control_t);

    internal delegate IntPtr SwiNativeForeignNondeterministicPredicateCallback5(SwiPrologTerm term1, SwiPrologTerm term2, SwiPrologTerm term3, SwiPrologTerm term4, SwiPrologTerm term5, IntPtr control_t);

    internal delegate IntPtr SwiNativeForeignNondeterministicPredicateCallback6(SwiPrologTerm term1, SwiPrologTerm term2, SwiPrologTerm term3, SwiPrologTerm term4, SwiPrologTerm term5, SwiPrologTerm term6, IntPtr control_t);

    internal delegate IntPtr SwiNativeForeignNondeterministicPredicateCallback7(SwiPrologTerm term1, SwiPrologTerm term2, SwiPrologTerm term3, SwiPrologTerm term4, SwiPrologTerm term5, SwiPrologTerm term6, SwiPrologTerm term7, IntPtr control_t);

    internal delegate IntPtr SwiNativeForeignNondeterministicPredicateCallback8(SwiPrologTerm term1, SwiPrologTerm term2, SwiPrologTerm term3, SwiPrologTerm term4, SwiPrologTerm term5, SwiPrologTerm term6, SwiPrologTerm term7, SwiPrologTerm term8, IntPtr control_t);



    public delegate bool SwiPrologCallback0();

    public delegate bool SwiPrologCallback1(SwiPrologTerm term);

    public delegate bool SwiPrologCallback2(SwiPrologTerm term1, SwiPrologTerm term2);

    public delegate bool SwiPrologCallback3(SwiPrologTerm term1, SwiPrologTerm term2, SwiPrologTerm term3);

    public delegate bool SwiPrologCallback4(SwiPrologTerm term1, SwiPrologTerm term2, SwiPrologTerm term3, SwiPrologTerm term4);

    public delegate bool SwiPrologCallback5(SwiPrologTerm term1, SwiPrologTerm term2, SwiPrologTerm term3, SwiPrologTerm term4, SwiPrologTerm term5);

    public delegate bool SwiPrologCallback6(SwiPrologTerm term1, SwiPrologTerm term2, SwiPrologTerm term3, SwiPrologTerm term4, SwiPrologTerm term5, SwiPrologTerm term6);

    public delegate bool SwiPrologCallback7(SwiPrologTerm term1, SwiPrologTerm term2, SwiPrologTerm term3, SwiPrologTerm term4, SwiPrologTerm term5, SwiPrologTerm term6, SwiPrologTerm term7);

    public delegate bool SwiPrologCallback8(SwiPrologTerm term1, SwiPrologTerm term2, SwiPrologTerm term3, SwiPrologTerm term4, SwiPrologTerm term5, SwiPrologTerm term6, SwiPrologTerm term7, SwiPrologTerm term8);



    public delegate bool SwiPrologNondeterministicCallback0(dynamic context);

    public delegate bool SwiPrologNondeterministicCallback1(SwiPrologTerm term, dynamic context);

    public delegate bool SwiPrologNondeterministicCallback2(SwiPrologTerm term1, SwiPrologTerm term2, dynamic context);

    public delegate bool SwiPrologNondeterministicCallback3(SwiPrologTerm term1, SwiPrologTerm term2, SwiPrologTerm term3, dynamic context);

    public delegate bool SwiPrologNondeterministicCallback4(SwiPrologTerm term1, SwiPrologTerm term2, SwiPrologTerm term3, SwiPrologTerm term4, dynamic context);

    public delegate bool SwiPrologNondeterministicCallback5(SwiPrologTerm term1, SwiPrologTerm term2, SwiPrologTerm term3, SwiPrologTerm term4, SwiPrologTerm term5, dynamic context);

    public delegate bool SwiPrologNondeterministicCallback6(SwiPrologTerm term1, SwiPrologTerm term2, SwiPrologTerm term3, SwiPrologTerm term4, SwiPrologTerm term5, SwiPrologTerm term6, dynamic context);

    public delegate bool SwiPrologNondeterministicCallback7(SwiPrologTerm term1, SwiPrologTerm term2, SwiPrologTerm term3, SwiPrologTerm term4, SwiPrologTerm term5, SwiPrologTerm term6, SwiPrologTerm term7, dynamic context);

    public delegate bool SwiPrologNondeterministicCallback8(SwiPrologTerm term1, SwiPrologTerm term2, SwiPrologTerm term3, SwiPrologTerm term4, SwiPrologTerm term5, SwiPrologTerm term6, SwiPrologTerm term7, SwiPrologTerm term8, dynamic context);
}
