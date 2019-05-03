/*********************************************************
* 
*  Authors:        Adam Sobieski
*
*********************************************************/

namespace Logic.Prolog.Xsb.Callbacks
{
    public delegate bool XsbPrologCallback0();

    public delegate bool XsbPrologCallback1(XsbPrologTerm term);

    public delegate bool XsbPrologCallback2(XsbPrologTerm term1, XsbPrologTerm term2);

    public delegate bool XsbPrologCallback3(XsbPrologTerm term1, XsbPrologTerm term2, XsbPrologTerm term3);

    public delegate bool XsbPrologCallback4(XsbPrologTerm term1, XsbPrologTerm term2, XsbPrologTerm term3, XsbPrologTerm term4);

    public delegate bool XsbPrologCallback5(XsbPrologTerm term1, XsbPrologTerm term2, XsbPrologTerm term3, XsbPrologTerm term4, XsbPrologTerm term5);

    public delegate bool XsbPrologCallback6(XsbPrologTerm term1, XsbPrologTerm term2, XsbPrologTerm term3, XsbPrologTerm term4, XsbPrologTerm term5, XsbPrologTerm term6);

    public delegate bool XsbPrologCallback7(XsbPrologTerm term1, XsbPrologTerm term2, XsbPrologTerm term3, XsbPrologTerm term4, XsbPrologTerm term5, XsbPrologTerm term6, XsbPrologTerm term7);

    public delegate bool XsbPrologCallback8(XsbPrologTerm term1, XsbPrologTerm term2, XsbPrologTerm term3, XsbPrologTerm term4, XsbPrologTerm term5, XsbPrologTerm term6, XsbPrologTerm term7, XsbPrologTerm term8);



    internal delegate int XsbNativeForeignPredicateCallback();
}