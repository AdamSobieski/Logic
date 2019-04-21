/*********************************************************
* 
*  Author:        Adam Sobieski
*
*********************************************************/

namespace Logic.Prolog.Callback
{
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