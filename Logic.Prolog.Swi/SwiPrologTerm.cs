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

using Microsoft.ClearScript;
using Logic.Prolog.Swi.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Text;

namespace Logic.Prolog.Swi
{
    // from: http://www.swi-prolog.org/pldoc/doc_for?object=c(%27PL_term_type%27)
    //---------------------------------------------------------------------------
    //PL_VARIABLE A variable or attributed variable
    //PL_ATOM A Prolog atom
    //PL_NIL The constant[]
    //PL_BLOB A blob(see section 12.4.8.2)
    //PL_STRING A string (see section 5.2)
    //PL_INTEGER A integer
    //PL_FLOAT    A floating point number
    //PL_TERM A compound term
    //PL_LIST_PAIR A list cell([H|T])
    //PL_DICT A dict(see section 5.4))

    //from: SWI-Prolog.h
    //---------------------------------------------------------------------------
    ///*******************************
    //*      TERM-TYPE CONSTANTS	*
    //*******************************/
    //  /* PL_unify_term() arguments */
    //#define PL_VARIABLE	 (1)		/* nothing */
    //#define PL_ATOM		 (2)		/* const char * */
    //#define PL_INTEGER	 (3)		/* int */
    //#define PL_FLOAT	 (4)		/* double */
    //#define PL_STRING	 (5)		/* const char * */
    //#define PL_TERM		 (6)

    //#define PL_NIL		 (7)		/* The constant [] */
    //#define PL_BLOB		 (8)		/* non-atom blob */
    //#define PL_LIST_PAIR	 (9)		/* [_|_] term */

    //  /* PL_unify_term() */
    //#define PL_FUNCTOR	 (10)		/* functor_t, arg ... */
    //#define PL_LIST		 (11)		/* length, arg ... */
    //#define PL_CHARS	 (12)		/* const char * */
    //#define PL_POINTER	 (13)		/* void * */
    //  /* PlArg::PlArg(text, type) */
    //#define PL_CODE_LIST	 (14)		/* [ascii...] */
    //#define PL_CHAR_LIST	 (15)		/* [h,e,l,l,o] */
    //#define PL_BOOL		 (16)		/* PL_set_prolog_flag() */
    //#define PL_FUNCTOR_CHARS (17)		/* PL_unify_term() */
    //#define _PL_PREDICATE_INDICATOR (18)	/* predicate_t (Procedure) */
    //#define PL_SHORT	 (19)		/* short */
    //#define PL_INT		 (20)		/* int */
    //#define PL_LONG		 (21)		/* long */
    //#define PL_DOUBLE	 (22)		/* double */
    //#define PL_NCHARS	 (23)		/* size_t, const char * */
    //#define PL_UTF8_CHARS	 (24)		/* const char * */
    //#define PL_UTF8_STRING	 (25)		/* const char * */
    //#define PL_INT64	 (26)		/* int64_t */
    //#define PL_NUTF8_CHARS	 (27)		/* size_t, const char * */
    //#define PL_NUTF8_CODES	 (29)		/* size_t, const char * */
    //#define PL_NUTF8_STRING	 (30)		/* size_t, const char * */
    //#define PL_NWCHARS	 (31)		/* size_t, const wchar_t * */
    //#define PL_NWCODES	 (32)		/* size_t, const wchar_t * */
    //#define PL_NWSTRING	 (33)		/* size_t, const wchar_t * */
    //#define PL_MBCHARS	 (34)		/* const char * */
    //#define PL_MBCODES	 (35)		/* const char * */
    //#define PL_MBSTRING	 (36)		/* const char * */
    //#define PL_INTPTR	 (37)		/* intptr_t */
    //#define PL_CHAR		 (38)		/* int */
    //#define PL_CODE		 (39)		/* int */
    //#define PL_BYTE		 (40)		/* int */
    //  /* PL_skip_list() */
    //#define PL_PARTIAL_LIST	 (41)		/* a partial list */
    //#define PL_CYCLIC_TERM	 (42)		/* a cyclic list/term */
    //#define PL_NOT_A_LIST	 (43)		/* Object is not a list */
    //  /* dicts */
    //#define PL_DICT		 (44)

    internal enum SwiPrologTermType
    {
        Unknown = 0,
        Variable = 1,
        Atom = 2,
        Integer = 3,
        Float = 4,
        String = 5,
        Term = 6,
        Nil = 7,
        Blob = 8,
        ListPair = 9,
        Functor = 10,
        List = 11,
        Chars = 12,
        Pointer = 13,
        CodeList = 14,
        CharList = 15,
        Bool = 16,
        FunctorChars = 17,
        PredicateIndicator = 18,
        Short = 19,
        Int = 20,
        Long = 21,
        Double = 22,
        NChars = 23,
        UTF8Chars = 24,
        UTF8String = 25,
        Int64 = 26,
        NUTF8Chars = 27,
        NUTF8Codes = 29,
        NUTF8String = 30,
        NWChars = 31,
        NWCodes = 32,
        NWString = 33,
        MBChars = 34,
        MBCodes = 35,
        MBString = 36,
        IntPtr = 37,
        Char = 38,
        Code = 39,
        Byte = 40,
        PartialList = 41,
        CyclicTerm = 42,
        NotAList = 43,
        Dict = 44
    }

    /********************************
    *     GENERIC PROLOG TERM		*
    ********************************/
    #region public struct PrologTerm

    /// <summary>
    ///  <para>The PrologTerm <see langword="struct"/> plays a central role in operating on Prolog data.</para>
    /// </summary>
    public struct SwiPrologTerm : IComparable, IEnumerable<SwiPrologTerm>// TODO, IList<PlTerm> // LISTS
    {
        private uintptr_t _termRef;

        // Properties
        internal uintptr_t TermRef
        {
            get
            {
                //Check.Require(_termRef != 0, "use of an uninitialized PlTerm. If you need a variable use PlTerm.PlVar() instead");
                Contract.Requires(_termRef != 0, "use of an uninitialized PlTerm. If you need a variable use PlTerm.PlVar() instead");
                return _termRef;
            }
        }

        #region implementing IComparable CompareTo

        [NoScriptAccess]
        public int CompareTo(object obj)
        {
            if (obj is SwiPrologTerm)
                return libswipl.PL_compare(TermRef, ((SwiPrologTerm)obj).TermRef);
            throw new ArgumentException("object is not a PrologTerm");
        }

        #endregion

        /// <summary>
        /// <para>If PlTerm is compound and index is between 0 and Arity (including), the nth PlTerm is returned.</para>
        /// <para>If pos is 0 the functor of the term is returned (For a list '.').</para>
        /// <para>See: <see href="http://gollem.science.uva.nl/SWI-Prolog/Manual//foreigninclude.html#PL_get_arg()">PL_get_arg/3</see></para>
        /// </summary>
        /// <param name="position">To Get the nth PlTerm</param>
        /// <returns>a PlTerm</returns>
        /// <example>
        ///     <code source="..\..\TestSwiPl\PlTerm.cs" region="PlTerm_indexer_doc" />
        /// </example>
        /// <exception cref="NotSupportedException">Is thrown if PlTerm is not of Type PlCompound see <see cref="IsCompound"/></exception>
        /// <exception cref="ArgumentOutOfRangeException">Is thrown if (pos &lt;  0 || pos >= Arity)</exception>
        /// <exception cref="InvalidOperationException">Is thrown if PL_get_arg returns 0.</exception>
        public SwiPrologTerm this[int position]
        {
            get
            {
                if (!IsCompound)
                    throw new NotSupportedException("Works only for compound terms!");

                if (position < 0 || position > Arity)
                    throw new ArgumentOutOfRangeException(nameof(position), "Must be greater than 0 and lesser then the arity of the term");

                if (0 == position)
                {
                    return IsList ? new SwiPrologTerm("'.'") : new SwiPrologTerm(Name);
                }
                uintptr_t a = libswipl.PL_new_term_ref();
                if (0 != libswipl.PL_get_arg(position, TermRef, a))
                    return new SwiPrologTerm(a);
                throw new InvalidOperationException("PrologTerm indexer: PL_get_arg return 0");
            }
            //set
            //{
            //    myData[pos] = value;
            //}
        }

        #region constructors


        // NOTE : Be Careful you *can* delete this constructor or make it private 
        //        it compiles but the tests will fail
        /// <summary>
        /// Create a PlTerm but *no* new term_ref it only copies the term_ref into the new object
        /// Used Intern by
        /// - PlException constructor
        /// - PlQueryQ.GetSolutions()
        /// - PlTermV this[int index] indexer
        /// </summary>
        /// <param name="termRef"></param>
        internal SwiPrologTerm(uintptr_t termRef)
        {
            _termRef = termRef;
        }

        public SwiPrologTerm(string text)
        {
            if (string.IsNullOrEmpty(text))
                text = "''";

            uintptr_t t = libswipl.PL_new_term_ref();

            if (0 == libswipl.PL_wchars_to_term(text, t))
                throw new SwiPrologException(new SwiPrologTerm(t));

            _termRef = libswipl.PL_new_term_ref();
            libswipl.PL_put_term(TermRef, t);
        }

        public SwiPrologTerm(int value)
        {
            _termRef = libswipl.PL_new_term_ref();
            libswipl.PL_put_integer(TermRef, value);
        }

        public SwiPrologTerm(double value)
        {
            _termRef = libswipl.PL_new_term_ref();
            libswipl.PL_put_float(TermRef, value);
        }

        #endregion

        /***************************************
        *	    SPECIALISED TERM CREATION		*
        *	    as static methods               *
        ***************************************/

        #region PlVar Creation

        /// <summary>
        /// Creates a new initialised term (holding a Prolog variable).
        /// </summary>
        /// <returns>a PlTerm</returns>
        public static SwiPrologTerm Variable()
        {
            return new SwiPrologTerm { _termRef = libswipl.PL_new_term_ref() };
        }
        #endregion

        #region PlList Creation
        /// <summary>
        /// <para>
        /// PlTail is for analysing and constructing lists. 
        /// It is called PlTail as enumeration-steps make the term-reference follow the `tail' of the list.
        /// </para>
        /// <para>
        /// A PlTail is created by making a new term-reference pointing to the same object. 
        /// As PlTail is used to enumerate or build a Prolog list, the initial list 
        /// term-reference keeps pointing to the head of the list.
        /// </para>
        /// </summary>
        /// <inheritdoc cref="Append(SwiPrologTerm)" select="example"/>
        /// <param name="list">The initial PlTerm</param>
        /// <returns>A PlTerm for which is_list/1 succeed.</returns>
        /// <seealso cref="Append(SwiPrologTerm)"/>
        /// <seealso cref="Add(SwiPrologTerm)"/>
        /// <seealso cref="AddList(SwiPrologTerm)"/>
        /// <seealso cref="Close()"/>
        /// <seealso cref="NextValue()"/>
        public static SwiPrologTerm Tail(SwiPrologTerm list)
        {
            //Check.Require(list != null);
            Contract.Requires(list != null);
            //Check.Require(list.IsList || list.IsVar);
            Contract.Requires(list.IsList || list.IsVariable);

            var term = new SwiPrologTerm();
            if (0 != libswipl.PL_is_variable(list.TermRef) || 0 != libswipl.PL_is_list(list.TermRef))
                term._termRef = libswipl.PL_copy_term_ref(list.TermRef);
            else
                throw new SwiPrologTypeException("list", list);

            return term;
        }
        #endregion

        #region Compound Creation
        [Obsolete("PrologTerm.Compound(text) is deprecated, please use new PrologTerm(text) instead.")]
        static internal SwiPrologTerm Compound(string text)
        {
            return new SwiPrologTerm(text);
        }

        public static SwiPrologTerm Compound(string functor, SwiPrologTermVector args)
        {
            //Check.Require(args.A0 != 0);
            Contract.Requires(args.A0 != 0);
            var term = new SwiPrologTerm { _termRef = libswipl.PL_new_term_ref() };
            var atom = libswipl.PL_new_atom_wchars(functor);
            libswipl.PL_cons_functor_v(term.TermRef, libswipl.PL_new_functor(atom, args.Size), args.A0);
            libswipl.PL_unregister_atom(atom);
            return term;
        }

        public static SwiPrologTerm Compound(string functor, SwiPrologTerm arg1)
        {
            var args = new SwiPrologTermVector(arg1);
            return Compound(functor, args);
        }

        public static SwiPrologTerm Compound(string functor, SwiPrologTerm arg1, SwiPrologTerm arg2)
        {
            var args = new SwiPrologTermVector(arg1, arg2);
            return Compound(functor, args);
        }

        public static SwiPrologTerm Compound(string functor, SwiPrologTerm arg1, SwiPrologTerm arg2, SwiPrologTerm arg3)
        {
            var args = new SwiPrologTermVector(arg1, arg2, arg3);
            return Compound(functor, args);
        }

        #endregion PlCompound Creation

        #region String Creation

        /// <summary>
        /// A SWI-Prolog string represents a byte-string on the global stack.
        /// It's lifetime is the same as for compound terms and other data living on the global stack.
        /// Strings are not only a compound representation of text that is garbage-collected,
        /// but as they can contain 0-bytes, they can be used to contain arbitrary C-data structures.
        /// </summary>
        /// <param name="text">the string</param>
        /// <returns>a new PrologTerm</returns>
        /// <remarks>NOTE: this Method does *not* work with unicode characters. Concider to use new PlTerm(text) instead.</remarks>
        public static SwiPrologTerm String(string text)
        {
            var t = new SwiPrologTerm { _termRef = libswipl.PL_new_term_ref() };
            libswipl.PL_unify_wchars(t.TermRef, SwiPrologTermType.String, text);
            return t;
        }

        public static SwiPrologTerm String(string text, int len)
        {
            var t = new SwiPrologTerm { _termRef = libswipl.PL_new_term_ref() };
            libswipl.PL_unify_wchars(t.TermRef, SwiPrologTermType.String, len, text);
            return t;
        }
        #endregion PlString Creation

        #region CodeList Creation
        /// <summary>
        /// Create a Prolog list of ASCII codes from a 0-terminated C-string.
        /// </summary>
        /// <param name="text">The text</param>
        /// <returns>a new <see cref="SwiPrologTerm"/></returns>
        public static SwiPrologTerm CodeList(string text)
        {
            var term = new SwiPrologTerm { _termRef = libswipl.PL_new_term_ref() };
            libswipl.PL_unify_wchars(term.TermRef, SwiPrologTermType.CodeList, text);
            return term;
        }
        #endregion

        #region CharList Creation
        /// <overloads>
        /// <summary>
        /// <para>These static methods creates a new CharList.</para>
        /// </summary>
        /// </overloads>
        /// <summary>Create a Prolog list of one-character atoms from a C#-string.</summary>
        /// <remarks>Character lists are compliant to Prolog's <see href="http://gollem.science.uva.nl/SWI-Prolog/Manual/manipatom.html#atom_chars/2">atom_chars/2</see> predicate.</remarks>
        /// <param name="text">a string</param>
        /// <returns>A new PlTerm containing a prolog list of character</returns>
        public static SwiPrologTerm CharList(string text)
        {
            var term = new SwiPrologTerm { _termRef = libswipl.PL_new_term_ref() };
            libswipl.PL_unify_wchars(term.TermRef, SwiPrologTermType.CharList, text);
            return term;
        }
        #endregion

        /***************************************
        *	                            		*
        ***************************************/

        #region Testing the type of a term ( IsVar, IsList, .... )

        [NoScriptAccess]
        public bool IsInitialized { get { return 0 != _termRef; } }

        internal SwiPrologTermType PrologTermType
        {
            get { return (SwiPrologTermType)libswipl.PL_term_type(TermRef); }
        }

        // all return non zero if condition succeed

        [ScriptMember("isVariable")]
        public bool IsVariable { get { return 0 != libswipl.PL_is_variable(TermRef); } }

        [ScriptMember("isGround")]
        public bool IsGround { get { return 0 != libswipl.PL_is_ground(TermRef); } }

        [ScriptMember("isAtom")]
        public bool IsAtom { get { return 0 != libswipl.PL_is_atom(TermRef); } }

        [ScriptMember("isString")]
        public bool IsString { get { return 0 != libswipl.PL_is_string(TermRef); } }

        [ScriptMember("isInteger")]
        public bool IsInteger { get { return 0 != libswipl.PL_is_integer(TermRef); } }

        [ScriptMember("isFloat")]
        public bool IsFloat { get { return 0 != libswipl.PL_is_float(TermRef); } }

        [ScriptMember("isCompound")]
        public bool IsCompound { get { return 0 != libswipl.PL_is_compound(TermRef); } }

        [ScriptMember("isList")]
        public bool IsList { get { return 0 != libswipl.PL_is_list(TermRef); } }

        [ScriptMember("isAtomic")]
        public bool IsAtomic { get { return 0 != libswipl.PL_is_atomic(TermRef); } }

        [ScriptMember("isNumber")]
        public bool IsNumber { get { return 0 != libswipl.PL_is_number(TermRef); } }

        #endregion

        /***************************************
        *	LIST ( PlTerm ) implementation      *
        ***************************************/

        #region list ( PlTail ) Methods

        // building
        /// <summary>
        /// Appends element to the list and make the PlTail reference point to the new variable tail. 
        /// If A is a variable, and this method is called on it using the argument "gnat", 
        /// a list of the form [gnat|B] is created and the PlTail object now points to the new variable B.
        /// 
        /// This method returns TRUE if the unification succeeded and FALSE otherwise. No exceptions are generated.
        /// </summary>
        /// <example>
        ///     <code source="..\..\TestSwiPl\PlTail.cs" region="List_Append_from_doc" />
        /// </example>
        /// <param name="term">The PlTerm to append on the list.</param>
        /// <returns>true if successful otherwise false</returns>
        /// <example>
        /// <para>The following samples shows how <see cref="T:System.Linq"/> can be used to filter a Prolog list.</para>
        /// <code source="..\..\TestSwiPl\LinqPlTail.cs" region="query_prologlist_PlTail_with_Linq_doc" />
        /// </example>
        [ScriptMember("append")]
        public bool Append(SwiPrologTerm term)
        {
            //Check.Require(IsList || IsVar);
            Contract.Requires(IsList || IsVariable);
            //Check.Require(term._termRef != 0);
            Contract.Requires(term._termRef != 0);

            uintptr_t tmp = libswipl.PL_new_term_ref();
            if (0 != libswipl.PL_unify_list(TermRef, tmp, TermRef) && 0 != libswipl.PL_unify(tmp, term.TermRef))
                return true;

            return false;
        }

        /// <summary>
        /// Appends an element to a list by creating a new one and copy all elements
        /// Note This is a slow version
        /// see my mail from Jan from 2007.11.06 14:44
        /// </summary>
        /// <param name="term">a closed list</param>
        /// <returns>True if Succeed</returns>
        [ScriptMember("add")]
        public bool Add(SwiPrologTerm term)
        {
            //Check.Require(IsList);
            Contract.Requires(IsList);
            //Check.Require(term != null);
            Contract.Requires(term != null);

            uintptr_t list, head, tail;
            BuildOpenList(out list, out head, out tail);

            if (0 == libswipl.PL_unify_list(tail, head, tail))	// extend the list with a variable
                return false;
            if (0 == libswipl.PL_unify(term.TermRef, head))	// Unify this variable with the new list
                return false;
            libswipl.PL_unify_nil(tail);

            _termRef = list;
            return true;
        }

        /// <summary>
        /// Appends a list ( PlTail ) to a list by creating a new one and copy all elements
        /// </summary>
        /// <example>
        /// <code source="..\..\TestSwiPl\PlTail.cs" region="List_Add_list_doc" />
        /// </example>
        /// <param name="listToAppend">a closed list</param>
        /// <returns>True if Succeed</returns>
        [ScriptMember("addList")]
        public bool AddList(SwiPrologTerm listToAppend)
        {
            //Check.Require(IsList);
            Contract.Requires(IsList);
            //Check.Require(listToAppend != null);
            Contract.Requires(listToAppend != null);
            //Check.Require(listToAppend.IsList);
            Contract.Requires(listToAppend.IsList);

            uintptr_t list, head, tail;
            BuildOpenList(out list, out head, out tail);

            uintptr_t list2 = libswipl.PL_copy_term_ref(listToAppend.TermRef);
            uintptr_t elem = libswipl.PL_new_term_ref(); 			// 'elem' for iterating the old list
            while (0 != libswipl.PL_get_list(list2, elem, list2))
            {
                libswipl.PL_unify_list(tail, head, tail);	// extend the list with a variable
                libswipl.PL_unify(elem, head);				// Unify this variable with the new list
            }

            libswipl.PL_unify_nil(tail);

            _termRef = list;
            return true;
        }

        /// <summary>
        /// Unifies the term with [] and returns the result of the unification.
        /// </summary>
        /// <returns>The <c>int</c> value of <c>PL_unify_nil(TermRef)</c></returns>
        public int Close()
        {
            //Check.Require(IsList || IsVar);
            Contract.Requires(IsList || IsVariable);
            return libswipl.PL_unify_nil(TermRef);
        }

        /// <summary>
        /// return a PlTerm bound to the next element of the list PlTail and advance PlTail. 
        /// Returns the element on success or a free PlTerm (Variable) if PlTail represents the empty list. 
        /// If PlTail is neither a list nor the empty list, a PlTypeException (type_error) is thrown. 
        /// </summary>
        /// <inheritdoc cref="AddList(SwiPrologTerm)" select="example"/>
        /// <returns>The Next element in the list as a PlTerm which is a variable for the last element or an empty list</returns>
        public SwiPrologTerm NextValue()
        {
            //Check.Require(IsList);
            Contract.Requires(IsList);
            SwiPrologTerm term = Variable();
            if (0 != libswipl.PL_get_list(TermRef, term.TermRef, TermRef))
            {
                return term;
            }
            if (0 != libswipl.PL_get_nil(TermRef))
            {
                return term;
            }
            throw new SwiPrologTypeException("list", this);
        }

        /// <summary>
        /// Converts to a strongly typed ReadOnlyCollection of PlTerm objects that can be accessed by index
        /// </summary>
        /// <returns>A strongly typed ReadOnlyCollection of PlTerm objects</returns>
        [NoScriptAccess]
        public ReadOnlyCollection<SwiPrologTerm> ToList()
        {
            //Check.Require(IsList);
            Contract.Requires(IsList);
            // make a copy to keep the list
            var tmp = new SwiPrologTerm(libswipl.PL_copy_term_ref(TermRef));
            var l = new List<SwiPrologTerm>();
            foreach (SwiPrologTerm t in tmp)
            {
                l.Add(t);
            }
            return new ReadOnlyCollection<SwiPrologTerm>(l);
        }

        /// <summary>
        /// Converts to a strongly typed Collection of strings that can be accessed by index
        /// </summary>
        /// <returns>A strongly typed string Collection</returns>
        [NoScriptAccess]
        public Collection<string> ToListString()
        {
            //Check.Require(IsList);
            Contract.Requires(IsList);
            // make a copy to keep the list
            var tmp = new SwiPrologTerm(libswipl.PL_copy_term_ref(TermRef));
            var l = new List<string>();
            foreach (SwiPrologTerm t in tmp)
            {
                l.Add(t.ToString());
            }
            return new Collection<string>(l);
        }

        #region IEnumerable<T> Members
        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A System.Collections.Generic.IEnumerator&lt;T that can be used to iterate through the collection.</returns>
        public IEnumerator<SwiPrologTerm> GetEnumerator()
        {
            //Check.Require(IsList);
            Contract.Requires(IsList);
            SwiPrologTerm t; //null;
            while (Next(out t))
            {
                yield return t;
            }
        }
        #endregion IEnumerable<T> Members

        // private list helper methods

        // enumerating
        // see: http://www.mycsharp.de/wbb2/thread.php?threadid=53241

        #region IEnumerable Members
        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An System.Collections.IEnumerator object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator(); // Looks recursive but it is *not*.
        }
        #endregion

        /// <summary>
        /// Bind termRef to the next element of the list PlTail and advance PlTail. 
        /// Returns TRUE on success and FALSE if PlTail represents the empty list. 
        /// If PlTail is neither a list nor the empty list, a type_error is thrown. 
        /// </summary>
        /// <param name="termRef"></param>
        /// <returns></returns>
        private bool Next(out SwiPrologTerm termRef)
        {
            //Check.Require(IsList);
            Contract.Requires(IsList);
            termRef = Variable();  // new PlTerm();
            if (0 != libswipl.PL_get_list(TermRef, termRef._termRef, TermRef))
            {
                return true;
            }
            if (0 != libswipl.PL_get_nil(TermRef))
            {
                return false;
            }
            throw new SwiPrologTypeException("list", this);
        }

        private void BuildOpenList(out uintptr_t list, out uintptr_t head, out uintptr_t tail)
        {
            list = libswipl.PL_new_term_ref();			    // our list (starts unbound)
            tail = libswipl.PL_copy_term_ref(list);	    // the tail of it (starts as the whole)
            head = libswipl.PL_new_term_ref(); 			// placeholder for the element
            uintptr_t elem = libswipl.PL_new_term_ref(); 		// for iterating the old list
            while (0 != libswipl.PL_get_list(TermRef, elem, TermRef))
            {
                libswipl.PL_unify_list(tail, head, tail);	// extend the list with a variable
                libswipl.PL_unify(elem, head);				// Unify this variable with the new list
            }
        }

        #endregion list ( PlTail ) Methods

        #region ToString and helpers

        private string ToStringAsListFormat()
        {
            var sb = new StringBuilder("");
            var list = Tail(this);
            foreach (SwiPrologTerm t in list)
            {
                if (0 < sb.Length)
                    sb.Append(',');
                sb.Append(t.ToString());
            }
            sb.Insert(0, '[');
            sb.Append(']');
            return sb.ToString();
        }

        [ScriptMember("toString")]
        public override string ToString()
        {
            string s;
            if (IsList)    //switch (this.PlType)
                s = ToStringAsListFormat();
            else
                s = (string)this;
            return s;
        }

        /// <summary>
        /// Convert a PlTerm to a string by <see href="http://www.swi-prolog.org/pldoc/doc_for?object=c(%27PL_get_chars%27)">PL_get_chars/1</see>
        /// with the CVT_WRITE_CANONICAL flag. If it fails PL_get_chars/3 is called again with REP_MB flag.
        /// </summary>
        /// <returns>return the string of a PlTerm</returns>
        /// <exception cref="SwiPrologTypeException">Throws a PlTypeException if PL_get_chars/3 didn't succeeds.</exception>
        
        public string ToStringCanonical()
        {
            string s;
            if (0 != libswipl.PL_get_wchars(TermRef, out s, libswipl.CVT_WRITE_CANONICAL | libswipl.BUF_RING | libswipl.REP_UTF8))
                return s;
            throw new SwiPrologTypeException("text", this);
        }

        #endregion

        [ScriptMember("toInteger")]
        public int ToInteger()
        {
            return (int)this;
        }

        [ScriptMember("toDouble")]
        public double ToDouble()
        {
            return (double)this;
        }

        #region unification
        /// <overloads>
        /// This methods performs Prolog unification and returns true if successful and false otherwise.
        /// It is equal to the prolog =/2 operator.
        /// <para>See <see cref="Unify(SwiPrologTerm)"/> for an example.</para>
        /// <remarks>
        /// This methods are introduced for clear separation between the destructive assignment in C# using =
        /// and prolog unification.
        /// </remarks>
        /// </overloads>
        /// <summary>Unify a PlTerm with a PlTerm</summary>
        /// <example>
        /// <code source="..\..\TestSwiPl\PlTerm.cs" region="UnifyTermVar_doc" />
        /// </example>
        /// <param name="term">the second term for unification</param>
        /// <returns>true or false</returns>
        [ScriptMember("unify")]
        public bool Unify(SwiPrologTerm term)
        {
            return 0 != libswipl.PL_unify(TermRef, term.TermRef);
        }

        [ScriptMember("unify")]
        public bool Unify(string text)
        {
            return 0 != libswipl.PL_unify_wchars(TermRef, SwiPrologTermType.String /* was PrologTermType.Atom */, text);
        }

        [ScriptMember("unify")]
        public bool Unify(int value)
        {
            return 0 != libswipl.PL_unify(TermRef, new SwiPrologTerm(value).TermRef);
        }

        [ScriptMember("unify")]
        public bool Unify(double value)
        {
            return 0 != libswipl.PL_unify(TermRef, new SwiPrologTerm(value).TermRef);
        }

        // <summary>
        // Useful e.g. for lists list.Copy().ToList(); list.ToString();
        // </summary>
        // <returns>Return a unifies PlTerm.PlVar of this term</returns>
        //internal PlTerm Copy()
        //{
        //    PlTerm tc = PlVar();
        //    if (!Unify(tc))
        //        throw new PlLibException("Copy term fails (Unification return false)");
        //    return tc;
        //}
        /*
        <!--
        int operator =(const PlTerm &t2)	// term 
        {
            return PL_unify(_termRef, t2._termRef);
        }
        int operator =(const PlAtom &atom)	// atom
        {
            return PL_unify_atom(TermRef, atom._handle);
        }
        int operator =(const char *v)		// atom (from char *)
        {
            return PL_unify_atom_chars(_termRef, v);
        }
        int operator =(long v)		// integer
        {
            return PL_unify_integer(_termRef, v);
        }
        int operator =(int v)			// integer
        {
            return PL_unify_integer(_termRef, v);
        }
        int operator =(double v)		// float
        {
            return PL_unify_float(_termRef, v);
        }
        int operator =(const PlFunctor &f)	// functor
        {
            return PL_unify_functor(_termRef, f.functor);
        }
        -->
        */
        #endregion unification

        #region Arity and Name
        /// <summary><para>Get the arity of the functor if <see cref="SwiPrologTerm"/> is a compound term.</para></summary>
        /// <remarks><para><see cref="Arity"/> and <see cref="Name"/> are for compound terms only</para></remarks>
        /// <exception cref="NotSupportedException">Is thrown if the term isn't compound</exception>
        [ScriptMember("arity")]
        public int Arity
        {
            get
            {
                uintptr_t name = 0; // atom_t 
                int arity = 0;
                if (0 != libswipl.PL_get_name_arity(TermRef, ref name, ref arity))
                    return arity;

                throw new NotSupportedException("Only possible for compound or atoms");
                //throw new PlTypeException("compound", this);   // FxCop Don't like this type of exception
            }
        }

        /// <summary>
        /// <para>Get a holding the name of the functor if <see cref="SwiPrologTerm"/> is a compound term.</para>
        /// </summary>
        /// <inheritdoc cref="Arity" />
        [ScriptMember("name")]
        public string Name
        {
            get
            {
                uintptr_t name = 0; // atom_t 
                int arity = 0;

                if (0 != libswipl.PL_get_name_arity(TermRef, ref name, ref arity))
                    return libswipl.PL_atom_wchars(name);

                throw new NotSupportedException("Only possible for compound or atoms");
                //throw new PlTypeException("compound", this);   // FyCop Don't like this type of exception
            }
        }
        #endregion Arity and Name

        #region cast operators
        /// <summary>
        /// Converts the Prolog argument into a string which implies Prolog atoms and strings
        /// are converted to the represented text or throw a PlTypeException. 
        /// </summary>
        /// <remarks>
        /// <para>Converts the Prolog argument using PL_get_chars() using the 
        /// flags CVT_ALL|CVT_WRITE|BUF_RING, which implies Prolog atoms and strings
        /// are converted to the represented text or throw a PlTypeException. 
        /// </para>
        /// <para>If the above call return 0 <see href="http://gollem.science.uva.nl/SWI-Prolog/Manual/foreigninclude.html#PL_get_chars()">PL_get_chars</see> 
        /// is called a second time with the flags CVT_ALL|CVT_WRITE|BUF_RING|REP_UTF8.</para>
        /// <para>All other data is handed to write/1.</para>
        /// </remarks>
        /// <param name="term">A PlTerm that can be converted to a string</param>
        /// <returns>A C# string</returns>
        /// <exception cref="SwiPrologTypeException">Throws a PlTypeException exception</exception>
        /// <exception cref="SbsSW.DesignByContract.PreconditionException">Is thrown if the operator is used on an uninitialized PlTerm</exception>
        public static explicit operator string(SwiPrologTerm term)
        {
            string s;
            if (0 != libswipl.PL_get_wchars(term.TermRef, out s, libswipl.CVT_ALL | libswipl.CVT_WRITE | libswipl.BUF_RING | libswipl.REP_UTF8))
                return s;
            throw new SwiPrologTypeException("text", term);
        }

        /// <summary>
        /// Yields a int if the PlTerm is a Prolog integer or float that can be converted 
        /// without loss to a int. Throws a PlTypeException exception otherwise
        /// </summary>
        /// <param name="term">A PlTerm is a Prolog integer or float that can be converted without loss to a int.</param>
        /// <returns>A C# int</returns>
        /// <exception cref="SwiPrologTypeException">Throws a PlTypeException exception if <see cref="PrologTermType"/> 
        /// is not a <see langword="PlType.PlInteger"/> or a <see langword="PlType.PlFloat"/>.</exception>
        /// <exception cref="SbsSW.DesignByContract.PreconditionException">Is thrown if the operator is used on an uninitialized PlTerm</exception>
        public static explicit operator int(SwiPrologTerm term)
        {
            int v = 0;
            if (0 != libswipl.PL_get_long(term.TermRef, ref v))
                return v;
            throw new SwiPrologTypeException("long", term);
        }

        /// <summary>
        /// Yields the value as a C# double if PlTerm represents a Prolog integer or float. 
        /// Throws a PlTypeException exception otherwise. 
        /// </summary>
        /// <param name="term">A PlTerm represents a Prolog integer or float</param>
        /// <returns>A C# double</returns>
        /// <exception cref="SwiPrologTypeException">Throws a PlTypeException exception if <see cref="PrologTermType"/> 
        /// is not a <see langword="PlType.PlInteger"/> or a <see langword="PlType.PlFloat"/>.</exception>
        /// <exception cref="SbsSW.DesignByContract.PreconditionException">Is thrown if the operator is used on an uninitialized PlTerm</exception>
        public static explicit operator double(SwiPrologTerm term)
        {
            double v = 0;
            if (0 != libswipl.PL_get_float(term.TermRef, ref v))
                return v;
            throw new SwiPrologTypeException("float", term);
        }

        #endregion cast oprators

        #region compare operators
        // Comparison standard order terms
        /// <inheritdoc />
        public override int GetHashCode()
        {
            return TermRef.GetHashCode();
        }

        /// <inheritdoc />
        public override bool Equals(Object obj)
        {
            if (obj is SwiPrologTerm)
                return this == ((SwiPrologTerm)obj);
            if (obj is int)
                return this == ((int)obj);
            return false;
        }
        /// <overload>Compare the instance term1 with term2 and return the result according to the Prolog defined standard order of terms.</overload>
        /// <summary>
        /// Yields TRUE if the PlTerm is an atom or string representing the same text as the argument, 
        /// FALSE if the conversion was successful, but the strings are not equal and an 
        /// type_error exception if the conversion failed.
        /// </summary>
        /// <param name="term1">a PlTerm</param>
        /// <param name="term2">a PlTerm</param>
        /// <returns>true or false</returns>
        public static bool operator ==(SwiPrologTerm term1, SwiPrologTerm term2)
        {
            return libswipl.PL_compare(term1.TermRef, term2.TermRef) == 0;
        }
        /// <inheritdoc cref="op_Equality(SwiPrologTerm, SwiPrologTerm)" />
        public static bool operator !=(SwiPrologTerm term1, SwiPrologTerm term2)
        {
            return libswipl.PL_compare(term1.TermRef, term2.TermRef) != 0;
        }
        /// <inheritdoc cref="op_Equality(SwiPrologTerm, SwiPrologTerm)" />
        public static bool operator <(SwiPrologTerm term1, SwiPrologTerm term2)
        {
            return libswipl.PL_compare(term1.TermRef, term2.TermRef) < 0;
        }
        /// <inheritdoc cref="op_Equality(SwiPrologTerm, SwiPrologTerm)" />
        public static bool operator >(SwiPrologTerm term1, SwiPrologTerm term2)
        {
            return libswipl.PL_compare(term1.TermRef, term2.TermRef) > 0;
        }
        /// <inheritdoc cref="op_Equality(SwiPrologTerm, SwiPrologTerm)" />
        public static bool operator <=(SwiPrologTerm term1, SwiPrologTerm term2)
        {
            return libswipl.PL_compare(term1.TermRef, term2.TermRef) <= 0;
        }
        /// <inheritdoc cref="op_Equality(SwiPrologTerm, SwiPrologTerm)" />
        public static bool operator >=(SwiPrologTerm term1, SwiPrologTerm term2)
        {
            return libswipl.PL_compare(term1.TermRef, term2.TermRef) >= 0;
        }


        /*
    int operator <=(const PlTerm &t2)
    {
        return PL_compare(_termRef, t2.TermRef) <= 0;
    }
    int operator >=(const PlTerm &t2)
    {
        return PL_compare(_termRef, t2._termRef) >= 0;
    }
    */
        // comparison (long)
        #endregion

        #region Equality Method
        /// <overload>Compare the instance term1 with term2 and return the result according to the Prolog defined standard order of terms.</overload>
        /// <summary>
        /// Yields TRUE if the PlTerm is an atom or string representing the same integer as the argument, 
        /// FALSE if the conversion was not successful.
        /// conversion of the term is done by  PL_get_long
        /// </summary>
        /// <param name="term">a PlTerm</param>
        /// <param name="value">a int</param>
        /// <returns>A bool</returns>
        public static bool operator ==(SwiPrologTerm term, int value)
        {
            int v0 = 0;
            if (0 != libswipl.PL_get_long(term.TermRef, ref v0))
                return v0 == value;
            return false; // throw new PlTypeException("integer", term);
        }
        /// <inheritdoc cref="op_Equality(SwiPrologTerm, SwiPrologTerm)" />
        public static bool operator ==(int value, SwiPrologTerm term)
        {
            return term == value;
        }
        // comparison (string)
        /// <inheritdoc cref="op_Equality(SwiPrologTerm, SwiPrologTerm)" />
        public static bool operator ==(SwiPrologTerm term, string value)
        {
            return ((string)term).Equals(value);
        }
        /// <inheritdoc cref="op_Equality(SwiPrologTerm, SwiPrologTerm)" />
        public static bool operator ==(string value, SwiPrologTerm term)
        {
            return term == value;
        }

        #endregion

        #region Inequality Method
        /// <overloads>
        /// <summary>
        /// <para>Inequality Method overload</para>
        /// <see cref="op_Equality(SwiPrologTerm, SwiPrologTerm)"/>
        /// a
        /// <see cref="M:SbsSW.SwiPlCs.PlTerm.op_Equality(SbsSW.SwiPlCs.PlTerm,System.Int32)"/>
        /// </summary>
        /// </overloads>
        /// 
        /// <summary>
        /// summary
        /// </summary>
        /// <param name="term"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool operator !=(SwiPrologTerm term, int value)
        {
            int v0 = 0;
            if (0 != libswipl.PL_get_long(term.TermRef, ref v0))
                return v0 != value;
            return true; // throw new PlTypeException("integer", term);
        }
        /// <inheritdoc cref="op_Inequality(SwiPrologTerm, int)" />
        public static bool operator !=(int value, SwiPrologTerm term)
        {
            return term != value;
        }
        /// <summary>
        /// test
        /// </summary>
        /// <param name="term"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool operator !=(SwiPrologTerm term, string value)
        {
            return !(term == value);
        }
        /// <inheritdoc cref="op_Inequality(SwiPrologTerm, string)" />
        public static bool operator !=(string value, SwiPrologTerm term)
        {
            return term != value;
        }

        #endregion compare operators
    }
    #endregion

    #region public class PrologTermVector
    /// <summary>
    /// <preliminary>The PrologTermVector <see langword="struct"/> represents an array of term references.</preliminary>
    /// <para>This type is used to pass the arguments to a foreign defined predicate (see <see cref="Callback.DelegateParameterVarArgs"/>), 
    /// construct compound terms (see <see cref="SwiPrologTerm.Compound(string, SwiPrologTermVector)"/> 
    /// and to create queries (see <see cref="T:SbsSW.SwiPlCs.PlQuery"/>).
    /// </para>
    /// <para>The only useful member function is the overloading of [], providing (0-based) access to the elements. <see cref="SwiPrologTermVector.this[Int32]"/> 
    /// Range checking is performed and raises a ArgumentOutOfRangeException exception.</para> 
    /// </summary>
    public struct SwiPrologTermVector : IEquatable<SwiPrologTermVector>
    {

        private readonly uintptr_t _a0; // term_t
        private readonly int _size;


        #region constructors

        /// <overloads>Create a PlTermV vector from the given PlTerm parameters
        /// <summary>
        /// <para>Create a new vector with PlTerm as elements</para>
        /// <para>It can be created with <paramref name="size"/> elements</para>
        /// <para>or</para>
        /// <para>automatically for 1, 2 or 3 plTerms</para>
        /// </summary>
        /// </overloads>
        /// 
        /// <summary>
        /// Create a vector of PlTerms with <paramref name="size"/> elements
        /// </summary>
        /// <param name="size">The amount of PlTerms in the vector</param>
        public SwiPrologTermVector(int size)
        {
            _a0 = libswipl.PL_new_term_refs(size);
            _size = size;
        }

        /// <summary>Create a PlTermV from the given <see cref="SwiPrologTerm"/>s.</summary>
        /// <param name="term0">The first <see cref="SwiPrologTerm"/> in the vector.</param>
        public SwiPrologTermVector(SwiPrologTerm term0)
        {
            _size = 1;
            _a0 = term0.TermRef;
        }

        // warning CS1573: Parameter 'term0' has no matching param tag in the XML comment for 'SbsSW.SwiPlCs.PlTermV.PlTermV(SbsSW.SwiPlCs.PlTerm, SbsSW.SwiPlCs.PlTerm)' (but other parameters do)
#pragma warning disable 1573
        /// <inheritdoc cref="PlTermV(PlTerm)" />
        /// <param name="term1">The second <see cref="PlTerm"/> in the vector.</param>
        public SwiPrologTermVector(SwiPrologTerm term0, SwiPrologTerm term1)
        {
            _size = 2;
            _a0 = libswipl.PL_new_term_refs(2);
            libswipl.PL_put_term(_a0 + 0, term0.TermRef);
            libswipl.PL_put_term(_a0 + 1, term1.TermRef);
        }

        /// <inheritdoc cref="SwiPrologTermVector(SwiPrologTerm, SwiPrologTerm)" />
        /// <param name="term2">The third <see cref="SwiPrologTerm"/> in the vector.</param>
        public SwiPrologTermVector(SwiPrologTerm term0, SwiPrologTerm term1, SwiPrologTerm term2)
        {
            _size = 3;
            _a0 = libswipl.PL_new_term_refs(3);
            libswipl.PL_put_term(_a0 + 0, term0.TermRef);
            libswipl.PL_put_term(_a0 + 1, term1.TermRef);
            libswipl.PL_put_term(_a0 + 2, term2.TermRef);
        }

        public SwiPrologTermVector(SwiPrologTerm term0, SwiPrologTerm term1, SwiPrologTerm term2, SwiPrologTerm term3)
        {
            _size = 4;
            _a0 = libswipl.PL_new_term_refs(4);
            libswipl.PL_put_term(_a0 + 0, term0.TermRef);
            libswipl.PL_put_term(_a0 + 1, term1.TermRef);
            libswipl.PL_put_term(_a0 + 2, term2.TermRef);
            libswipl.PL_put_term(_a0 + 3, term3.TermRef);
        }

        /// <summary>Create a PlTermV from the given <see cref="SwiPrologTerm"/>[] array.</summary>
        /// <param name="terms">An array of <see cref="SwiPrologTerm"/>s to build the vector.</param>
        /// <example>
        /// Use of Initializing an Array in CSharp
        /// <code>
        ///    PlTermV v = new PlTermV(new PlTerm[] {t1, t2, t3, t4});
        /// </code>
        /// </example>
        public SwiPrologTermVector(/*params*/ SwiPrologTerm[] terms)
        {
            if (null == terms)
                throw new ArgumentNullException("terms");
            _size = terms.Length;
            _a0 = libswipl.PL_new_term_refs(terms.Length);
            ulong count = 0;
            foreach (SwiPrologTerm t in terms)
            {
                libswipl.PL_put_term(_a0 + count, t.TermRef);
                count++;
            }
        }
#pragma warning restore 1573
        #endregion

        //        internal PlTermV(PlTermV toCopy)
        //            : this(toCopy._size)
        //        {
        //            for (uint i = 0; i < toCopy._size; i++)
        //            {
        ////                libpl.PL_put_term(_a0 + i, new PlTerm(toCopy[(int)i].TermRef).TermRef);
        //                this[i].TermRef = libpl.PL_copy_term_ref(toCopy[i].TermRef);
        //            }
        //        }

        // Properties

        /// <summary>
        /// the first term_t reference of the array
        /// </summary>
        internal uintptr_t A0
        {
            get { return _a0; }
        }

        /// <summary>Get the size of a PlTermV</summary>
        public int Size
        {
            get { return _size; }
        }


        /// <summary>
        /// A zero based list
        /// </summary>
        /// <param name="index"></param>
        /// <returns>The PlTerm for the given index</returns>
        /// <exception cref="ArgumentOutOfRangeException">Is thrown if (index &lt;  0 || index >= Size)</exception>
        /// <exception cref="SbsSW.DesignByContract.PreconditionException">Is thrown if the operator is used on an uninitialized PlTerm</exception>
        public SwiPrologTerm this[int index]
        {
            get
            {
                if (index < 0 || index >= Size)
                    throw new ArgumentOutOfRangeException("index");
                return new SwiPrologTerm(A0 + (uint)index);  // If this line is deleted -> update comment in PlTern(term_ref)
            }
            set
            {
                if (index < 0 || index >= Size)
                    throw new ArgumentOutOfRangeException("index");
                libswipl.PL_put_term(_a0 + (uint)index, value.TermRef);  // TermRef == 0, "use of an uninitialized PlTerm. If you need a variable use PlTerm.PlVar() instead
            }
        }


        #region IEquatable<PlTermV> Members
        // see http://msdn.microsoft.com/de-de/ms182276.aspx


        ///<inheritdoc />
        public override int GetHashCode()
        {
            return A0.GetHashCode();
        }

        ///<inheritdoc />
        public override bool Equals(object obj)
        {
            if (!(obj is SwiPrologTermVector))
                return false;
            return Equals((SwiPrologTermVector)obj);
        }

        ///<inheritdoc />
        /// <summary>
        /// Compare the size and A0 of the PltermV
        /// </summary>
        /// <param name="other">The PlTermV to compare</param>
        /// <returns>Return <c>false</c> if size or A0 are not equal otherwise <c>true</c>.</returns>
        ///<remarks>// TODO compare each PlTerm in PlTermV not only the refereces in A0</remarks>
        public bool Equals(SwiPrologTermVector other)
        {
            if (_size != other._size)
                return false;

            return A0 == other.A0;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="termVector1"></param>
        /// <param name="termVector2"></param>
        /// <returns></returns>
        public static bool operator ==(SwiPrologTermVector termVector1, SwiPrologTermVector termVector2)
        {
            return termVector1.Equals(termVector2);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="termVector1"></param>
        /// <param name="termVector2"></param>
        /// <returns></returns>
        public static bool operator !=(SwiPrologTermVector termVector1, SwiPrologTermVector termVector2)
        {
            return !termVector1.Equals(termVector2);
        }


        #endregion


    }
    #endregion
}
