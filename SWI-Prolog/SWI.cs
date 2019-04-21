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
using SWI_Prolog;
using SWI_Prolog.Callback;
using SWI_Prolog.Exceptions;
using SWI_Prolog.Streams;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using System.Text;

/********************************
*	       TYPES	Comment     *
********************************/
#region used type from SWI-Prolog.h

/*
<!-- 
#ifdef _PL_INCLUDE_H
	typedef module		module_t;	// atom module 
	typedef Procedure	predicate_t;// atom predicate handle
	typedef Record		record_t;	// handle to atom recorded term
#else
typedef	unsigned long	atom_t;		// Prolog atom
typedef void *		    module_t;		// Prolog module
typedef void *		    predicate_t;	// Prolog procedure
typedef void *		    record_t;		// Prolog recorded term
typedef unsigned long	term_t;		// opaque term handle
typedef unsigned long	qid_t;		// opaque query handle
typedef unsigned long	PL_fid_t;	// opaque foreign context handle
#endif
typedef unsigned long	functor_t;	// Name/Arity pair
typedef unsigned long	atomic_t;	// same atom word
typedef unsigned long	control_t;	// non-deterministic control arg
typedef void *		    PL_engine_t;	// opaque engine handle 
typedef unsigned long	foreign_t;	// return type of foreign functions

unsigned long	-  uint
-->
*/

#endregion

namespace SWI_Prolog.Streams
{
    /// <summary>
    /// The standard SWI-Prolog streams ( input output error )
    /// </summary>
    public enum PlStreamType
    {
        /// <summary>0 - The standard input stream.</summary>
        Input = 0,
        /// <summary>1 - The standard input stream.</summary>
        Output = 1,
        /// <summary>1 - The standard error stream.</summary>
        Error = 2
    }

    /// <summary>
    /// See <see cref="SWI.SetStreamFunctionRead"/>
    /// </summary>
    /// <param name="handle">A C stream handle. simple ignore it.</param>
    /// <param name="buffer">A pointer to a string buffer</param>
    /// <param name="bufferSize">The size of the string buffer</param>
    /// <returns>A <see cref="System.Delegate"/></returns>
    [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    public delegate long DelegateStreamReadFunction(IntPtr handle, IntPtr buffer, long bufferSize);

    /// <summary>
    /// See <see cref="SWI.SetStreamFunctionWrite"/>
    /// </summary>
    /// <param name="handle">A C stream handle. simple ignore it.</param>
    /// <param name="buffer">A pointer to a string buffer</param>
    /// <param name="bufferSize">The size of the string buffer</param>
    /// <returns>A <see cref="System.Delegate"/></returns>
    [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    public delegate long DelegateStreamWriteFunction(IntPtr handle, string buffer, long bufferSize);

    /*
    <!--
    [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    private delegate long Sseek_function(IntPtr handle, long pos, int whence);
    [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    private delegate Int64 Sseek64_function(IntPtr handle, Int64 pos, int whence);
    [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    private delegate int Sclose_function(IntPtr handle);
    [System.Runtime.InteropServices.UnmanagedFunctionPointerAttribute(System.Runtime.InteropServices.CallingConvention.Cdecl)]
    private delegate int Scontrol_function(IntPtr handle, int action, IntPtr arg);


    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct MIOFUNCTIONS
    {
        Sread_function read;		//* fill the buffer
        DelegateStreamWriteFunction write;		//* empty the buffer 
        Sseek_function seek;		//* seek to position 
        Sclose_function close;		//* close stream 
        Scontrol_function control;	//* Info/control 
        Sseek64_function seek64;		//* seek to position (intptr_t files) 
    };
    
     -->
     */
}

// The namespace summary is above class NamespaceDoc
namespace Logic.Prolog
{
    /// <summary>
    /// Obtain the type of a term, which should be a term returned by one of the other 
    /// interface predicates or passed as an argument. The function returns the type of 
    /// the Prolog term. The type identifiers are listed below. 
    /// </summary>
    /// <remarks>see <see href="http://gollem.science.uva.nl/SWI-Prolog/Manual/foreigninclude.html#PL_term_type()">PL_term_type(term_t)</see> in the SWI-Prolog Manual.</remarks>
    /// <seealso cref="P:SbsSW.SwiPlCs.PlTerm.PlType"/>
    /// <example>
    /// In this sample a Prolog variable is created in <see cref="Term">PlTerm t</see> and the <see cref="P:SbsSW.SwiPlCs.PlTerm.PlType"/> 
    /// is checked by his integer representation and his name.
    /// <code>
    ///     PlTerm t = PlTerm.PlVar();
    ///     Assert.AreEqual(1, (int)t.PlType);
    ///     Assert.AreEqual(PlType.PlVariable, t.PlType);
    /// </code>
    /// </example>
    public enum PrologTermType
    {
        /// <summary>0 - PL_UNKNOWN: Undefined </summary>
        Unknown = 0,
        /// <summary>1 - PL_VARIABLE: An unbound variable. The value of term as such is a unique identifier for the variable.</summary>
        Variable = 1,
        /// <summary>2 - PL_ATOM: A Prolog atom.</summary>
        Atom = 2,
        /// <summary>3 - PL_INTEGER: A Prolog integer.</summary>
        Integer = 3,
        /// <summary>4 - PL_FLOAT: A Prolog floating point number.</summary>
        Float = 4,
        /// <summary>5 - PL_STRING: A Prolog string.</summary>
        String = 5,
        /// <summary>6 - PL_TERM: A compound term. Note that a list is a compound term ./2.</summary>
        Term = 6,

        /// <summary>14 - PL_CODE_LIST: [ascii...].</summary>
        CodeList = 14,
        /// <summary>15 - PL_CHAR_LIST: [h,e,l,l,o].</summary>
        CharList = 15,
    }

    /********************************
    *     GENERIC PROLOG TERM		*
    ********************************/
    #region public struct PrologTerm
#pragma warning disable 1574
    // warning CS1574: XML comment on 'SbsSW.SwiPlCs.PlTerm' has cref attribute 'System.Linq' that 
    // SwiPlCs need no assambly reference to Linq

    /// <summary>
    ///  <para>The PlTerm <see langword="struct"/> plays a central role in conversion and operating on Prolog data.</para>
    ///  <para>PlTerm implements <see cref="System.IComparable"/> to support ordering in <see cref="T:System.Linq"/> queries if PlTerm is a List.
    /// see <see cref="PlTerm.Append"/> for examples.
    /// </para>
    ///  <para>Creating a PlTerm can be done by the <see href="Overload_SbsSW_SwiPlCs_PlTerm__ctor.htm">Constructors</see> or by the following static methods:</para>
    ///  <para>PlVar(), PlTail(), PlCompound, PlString(), PlCodeList(), PlCharList() (see remarks)</para>
    /// </summary>
    /// <remarks>
    /// <list type="table">  
    /// <listheader><term>static method</term><description>Description </description></listheader>  
    /// <item><term><see cref="Variable()"/></term><description>Creates a new initialised term (holding a Prolog variable).</description></item>  
    /// <item><term><see cref="PlTail(PlTerm)"/></term><description>PlTail is for analysing and constructing lists.</description></item>  
    /// <item><term><see href="Overload_SbsSW_SwiPlCs_PlTerm_PlCompound.htm">PlCompound(string)</see></term><description>Create compound terms. E.g. by parsing (as read/1) the given text.</description></item>  
    /// <item><term><see cref="String(string)"/></term><description>Create a SWI-Prolog string.</description></item>  
    /// <item><term><see cref="PlCodeList(string)"/></term><description>Create a Prolog list of ASCII codes from a 0-terminated C-string.</description></item>  
    /// <item><term><see cref="PlCharList(string)"/></term><description>Create a Prolog list of one-character atoms from a 0-terminated C-string.</description></item>  
    /// </list>   
    /// </remarks>
#pragma warning restore 1574
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
    public struct PrologTerm : IComparable, IEnumerable<PrologTerm>// TODO, IList<PlTerm> // LISTS
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

        ///<inheritdoc />
        public int CompareTo(object obj)
        {
            if (obj is PrologTerm)
                return libswipl.PL_compare(TermRef, ((PrologTerm)obj).TermRef);
            throw new ArgumentException("object is not a PlTerm");
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
        public PrologTerm this[int position]
        {
            get
            {
                if (!IsCompound)
                    throw new NotSupportedException("Work only for compound terms!");

                if (position < 0 || position > Arity)
                    throw new ArgumentOutOfRangeException("position", "Must be greater than 0 and lesser then the arity of the term");

                if (0 == position)
                {
                    return IsList ? new PrologTerm("'.'") : new PrologTerm(Name);
                }
                uintptr_t a = libswipl.PL_new_term_ref();
                if (0 != libswipl.PL_get_arg(position, TermRef, a))
                    return new PrologTerm(a);
                throw new InvalidOperationException("PlTerm indexer: PL_get_arg return 0");
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
        internal PrologTerm(uintptr_t termRef)
        {
            _termRef = termRef;
        }


        /// <overloads>
        /// <summary>
        /// A new PlTerm can be also created by the static methods:
        /// <list type="table">  
        /// <listheader><term>static method</term><description>Description </description></listheader>  
        /// <item><term><see cref="Variable()"/></term><description>Creates a new initialised term (holding a Prolog variable).</description></item>  
        /// <item><term><see cref="Tail(PrologTerm)"/></term><description>PlTail is for analysing and constructing lists.</description></item>  
        /// <item><term><see href="Overload_SbsSW_SwiPlCs_PlTerm_PlCompound.htm">PlCompound(string)</see></term><description>Create compound terms. E.g. by parsing (as read/1) the given text.</description></item>  
        /// <item><term><see cref="String(string)"/></term><description>Create a SWI-Prolog string.</description></item>  
        /// <item><term><see cref="PlCodeList(string)"/></term><description>Create a Prolog list of ASCII codes from a 0-terminated C-string.</description></item>  
        /// <item><term><see cref="PlCharList(string)"/></term><description>Create a Prolog list of one-character atoms from a 0-terminated C-string.</description></item>  
        /// </list>   
        /// </summary>
        /// </overloads>
        /// <summary>
        /// Creates a term-references holding a Prolog term representing text.
        /// </summary>
        /// <param name="text">the text</param>
        public PrologTerm(string text)
        {
            if (string.IsNullOrEmpty(text))
                text = "''";

            uintptr_t t = libswipl.PL_new_term_ref();

            if (0 == libswipl.PL_wchars_to_term(text, t))
                throw new PlException(new PrologTerm(t));

            _termRef = libswipl.PL_new_term_ref();
            libswipl.PL_put_term(TermRef, t);
        }
        /// <summary>
        /// Creates a term-references holding a Prolog integer representing value.
        /// </summary>
        /// <param name="value">a integer value</param>
        public PrologTerm(int value)
        {
            _termRef = libswipl.PL_new_term_ref();
            libswipl.PL_put_integer(TermRef, value);
        }
        /// <summary>
        /// Creates a term-references holding a Prolog float representing value.
        /// </summary>
        /// <param name="value">a double value</param>
        public PrologTerm(double value)
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
        static public PrologTerm Variable()
        {
            return new PrologTerm { _termRef = libswipl.PL_new_term_ref() };
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
        /// <inheritdoc cref="Append(PrologTerm)" select="example"/>
        /// <param name="list">The initial PlTerm</param>
        /// <returns>A PlTerm for which is_list/1 succeed.</returns>
        /// <seealso cref="Append(PrologTerm)"/>
        /// <seealso cref="Add(PrologTerm)"/>
        /// <seealso cref="AddList(PrologTerm)"/>
        /// <seealso cref="Close()"/>
        /// <seealso cref="NextValue()"/>
        static public PrologTerm Tail(PrologTerm list)
        {
            //Check.Require(list != null);
            Contract.Requires(list != null);
            //Check.Require(list.IsList || list.IsVar);
            Contract.Requires(list.IsList || list.IsVar);

            var term = new PrologTerm();
            if (0 != libswipl.PL_is_variable(list.TermRef) || 0 != libswipl.PL_is_list(list.TermRef))
                term._termRef = libswipl.PL_copy_term_ref(list.TermRef);
            else
                throw new PlTypeException("list", list);

            return term;
        }
        #endregion

        #region Compound Creation
        /// <overloads>
        /// <summary>
        /// <para>These static methods creates a new compound <see cref="PrologTerm"/>.</para>
        /// <para>For an example <see cref="Compound(string, PrologTermVector)"/></para>
        /// </summary>
        /// </overloads>
        /// <summary>
        /// It is the same as new PlTerm(text).
        /// </summary>
        /// <param name="text">The string representing the compound term.</param>
        /// <returns>a new <see cref="PrologTerm"/></returns>
        [Obsolete("PlTerm.PlCompound(test) is deprecated, please use new PlTerm(text) instead.")]
        static internal PrologTerm Compound(string text)
        {
            return new PrologTerm(text);
        }

        /// <summary>
        /// <para>Create a compound term with the given name from the given vector of arguments. See <see cref="PrologTermVector"/> for details.</para>
        /// </summary>
        /// <example>
        /// <para>The example below creates the Prolog term hello(world).</para>
        /// <code>
        ///  PlTerm t = PlTerm.PlCompound("hello", new PlTermv("world"));
        /// </code>
        /// </example>
        /// <param name="functor">The functor (name) of the compound term</param>
        /// <param name="args">the arguments as a <see cref="PrologTermVector"/></param>
        /// <returns>a new <see cref="PrologTerm"/></returns>
        static public PrologTerm Compound(string functor, PrologTermVector args)
        {
            //Check.Require(args.A0 != 0);
            Contract.Requires(args.A0 != 0);
            var term = new PrologTerm { _termRef = libswipl.PL_new_term_ref() };
            var atom = libswipl.PL_new_atom_wchars(functor);
            libswipl.PL_cons_functor_v(term.TermRef, libswipl.PL_new_functor(atom, args.Size), args.A0);
            libswipl.PL_unregister_atom(atom);
            return term;
        }

        /// <summary>
        /// <para>Create a compound term with the given name ant the arguments</para>
        /// </summary>
        /// <param name="functor">The functor (name) of the compound term</param>
        /// <param name="arg1">The first Argument as a <see cref="PrologTerm"/></param>
        /// <returns>a new <see cref="PrologTerm"/></returns>
        static public PrologTerm Compound(string functor, PrologTerm arg1)
        {
            var args = new PrologTermVector(arg1);
            return Compound(functor, args);
        }
#pragma warning disable 1573
        ///<inheritdoc cref="Compound(string, PrologTerm)" />
        /// <param name="arg2">The second Argument as a <see cref="PrologTerm"/></param>
        static public PrologTerm Compound(string functor, PrologTerm arg1, PrologTerm arg2)
        {
            var args = new PrologTermVector(arg1, arg2);
            return Compound(functor, args);
        }
        ///<inheritdoc cref="Compound(string, PrologTerm, PrologTerm)" />
        /// <param name="arg3">The third Argument as a <see cref="PrologTerm"/></param>
        static public PrologTerm Compound(string functor, PrologTerm arg1, PrologTerm arg2, PrologTerm arg3)
        {
            var args = new PrologTermVector(arg1, arg2, arg3);
            return Compound(functor, args);
        }
#pragma warning restore 1573
        #endregion PlCompound Creation

        #region String Creation

        /// <summary>
        /// A SWI-Prolog string represents a byte-string on the global stack.
        /// It's lifetime is the same as for compound terms and other data living on the global stack.
        /// Strings are not only a compound representation of text that is garbage-collected,
        /// but as they can contain 0-bytes, they can be used to contain arbitrary C-data structures.
        /// </summary>
        /// <param name="text">the string</param>
        /// <returns>a new PlTerm</returns>
        /// <remarks>NOTE: this Method do *not* work with unicode characters. Concider to use new PlTerm(test) instead.</remarks>
        public static PrologTerm String(string text)
        {
            var t = new PrologTerm { _termRef = libswipl.PL_new_term_ref() };
            libswipl.PL_unify_wchars(t.TermRef, PrologTermType.String, text);
            return t;
        }
#pragma warning disable 1573
        /// <inheritdoc cref="String(string)" />
        /// <param name="len">the length of the string</param>
        static public PrologTerm String(string text, int len)
        {
            var t = new PrologTerm { _termRef = libswipl.PL_new_term_ref() };
            libswipl.PL_unify_wchars(t.TermRef, PrologTermType.String, len, text);
            return t;
        }
#pragma warning restore 1573
        #endregion PlString Creation

        #region PlCodeList Creation
        /// <summary>
        /// Create a Prolog list of ASCII codes from a 0-terminated C-string.
        /// </summary>
        /// <param name="text">The text</param>
        /// <returns>a new <see cref="PrologTerm"/></returns>
        static public PrologTerm PlCodeList(string text)
        {
            var term = new PrologTerm { _termRef = libswipl.PL_new_term_ref() };
            libswipl.PL_unify_wchars(term.TermRef, PrologTermType.CodeList, text);
            return term;
        }
        #endregion

        #region PlCharList Creation
        /// <overloads>
        /// <summary>
        /// <para>These static methods creates a new PlCharList.</para>
        /// </summary>
        /// </overloads>
        /// <summary>Create a Prolog list of one-character atoms from a C#-string.</summary>
        /// <remarks>Character lists are compliant to Prolog's <see href="http://gollem.science.uva.nl/SWI-Prolog/Manual/manipatom.html#atom_chars/2">atom_chars/2</see> predicate.</remarks>
        /// <param name="text">a string</param>
        /// <returns>A new PlTerm containing a prolog list of character</returns>
        static public PrologTerm PlCharList(string text)
        {
            var term = new PrologTerm { _termRef = libswipl.PL_new_term_ref() };
            libswipl.PL_unify_wchars(term.TermRef, PrologTermType.CharList, text);
            return term;
        }
        #endregion

        /***************************************
        *	                            		*
        ***************************************/

        #region Testing the type of a term ( IsVar, IsList, .... )

        /// <summary>
        /// return false for a PlTerm variable wihich is only declareted 
        /// and tru if it is also Initialized
        /// </summary>
        [NoScriptAccess]
        public bool IsInitialized { get { return 0 != _termRef; } }

        /// <summary>Get the <see cref="T:SbsSW.SwiPlCs.PlType"/> of a <see cref="PrologTerm"/>.</summary>
        [NoScriptAccess]
        public PrologTermType PlType
        {
            get { return (PrologTermType)libswipl.PL_term_type(TermRef); }
        }

        // all return non zero if condition succeed

        /// <summary>Return true if <see cref="PrologTerm"/> is a variable</summary>
        /// <seealso cref="T:SbsSW.SwiPlCs.PlType"/>
        [ScriptMember("isVariable")]
        public bool IsVar { get { return 0 != libswipl.PL_is_variable(TermRef); } }

        /// <summary>Return true if <see cref="PrologTerm"/> is a ground term. See also ground/1. This function is cycle-safe.</summary>
        /// <seealso cref="T:SbsSW.SwiPlCs.PlType"/>
        [ScriptMember("isGround")]
        public bool IsGround { get { return 0 != libswipl.PL_is_ground(TermRef); } }

        /// <summary>Return true if <see cref="PrologTerm"/> is an atom.</summary>
        /// <seealso cref="T:SbsSW.SwiPlCs.PlType"/>
        [ScriptMember("isAtom")]
        public bool IsAtom { get { return 0 != libswipl.PL_is_atom(TermRef); } }

        /// <summary>Return true if <see cref="PrologTerm"/> is a string.</summary>
        /// <seealso cref="T:SbsSW.SwiPlCs.PlType"/>
        [ScriptMember("isString")]
        public bool IsString { get { return 0 != libswipl.PL_is_string(TermRef); } }

        /// <summary>Return true if <see cref="PrologTerm"/> is an integer.</summary>
        /// <seealso cref="T:SbsSW.SwiPlCs.PlType"/>
        [ScriptMember("isInteger")]
        public bool IsInteger { get { return 0 != libswipl.PL_is_integer(TermRef); } }

        /// <summary>Return true if <see cref="PrologTerm"/> is a float.</summary>
        /// <seealso cref="T:SbsSW.SwiPlCs.PlType"/>
        [ScriptMember("isFloat")]
        public bool IsFloat { get { return 0 != libswipl.PL_is_float(TermRef); } }

        /// <summary>Return true if <see cref="PrologTerm"/> is a compound term. Note that a list is a compound term ./2</summary>
        /// <seealso cref="T:SbsSW.SwiPlCs.PlType"/>
        [ScriptMember("isCompound")]
        public bool IsCompound { get { return 0 != libswipl.PL_is_compound(TermRef); } }

        /// <summary>Return true if <see cref="PrologTerm"/> is a compound term with functor ./2 or the atom [].</summary>
        /// <seealso cref="T:SbsSW.SwiPlCs.PlType"/>
        [ScriptMember("isList")]
        public bool IsList { get { return 0 != libswipl.PL_is_list(TermRef); } }

        /// <summary>Return true if <see cref="PrologTerm"/> is atomic (not variable or compound).</summary>
        /// <seealso cref="T:SbsSW.SwiPlCs.PlType"/>
        [ScriptMember("isAtomic")]
        public bool IsAtomic { get { return 0 != libswipl.PL_is_atomic(TermRef); } }

        /// <summary>Return true if <see cref="PrologTerm"/> is an integer or float.</summary>
        /// <seealso cref="T:SbsSW.SwiPlCs.PlType"/>
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
        public bool Append(PrologTerm term)
        {
            //Check.Require(IsList || IsVar);
            Contract.Requires(IsList || IsVar);
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
        public bool Add(PrologTerm term)
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

        public bool AddList(PrologTerm listToAppend)
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
            Contract.Requires(IsList || IsVar);
            return libswipl.PL_unify_nil(TermRef);
        }

        /// <summary>
        /// return a PlTerm bound to the next element of the list PlTail and advance PlTail. 
        /// Returns the element on success or a free PlTerm (Variable) if PlTail represents the empty list. 
        /// If PlTail is neither a list nor the empty list, a PlTypeException (type_error) is thrown. 
        /// </summary>
        /// <inheritdoc cref="AddList(PrologTerm)" select="example"/>
        /// <returns>The Next element in the list as a PlTerm which is a variable for the last element or an empty list</returns>
        public PrologTerm NextValue()
        {
            //Check.Require(IsList);
            Contract.Requires(IsList);
            PrologTerm term = Variable();
            if (0 != libswipl.PL_get_list(TermRef, term.TermRef, TermRef))
            {
                return term;
            }
            if (0 != libswipl.PL_get_nil(TermRef))
            {
                return term;
            }
            throw new PlTypeException("list", this);
        }

        /// <summary>
        /// Converts to a strongly typed ReadOnlyCollection of PlTerm objects that can be accessed by index
        /// </summary>
        /// <returns>A strongly typed ReadOnlyCollection of PlTerm objects</returns>
        [NoScriptAccess]
        public ReadOnlyCollection<PrologTerm> ToList()
        {
            //Check.Require(IsList);
            Contract.Requires(IsList);
            // make a copy to keep the list
            var tmp = new PrologTerm(libswipl.PL_copy_term_ref(TermRef));
            var l = new List<PrologTerm>();
            foreach (PrologTerm t in tmp)
            {
                l.Add(t);
            }
            return new ReadOnlyCollection<PrologTerm>(l);
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
            var tmp = new PrologTerm(libswipl.PL_copy_term_ref(TermRef));
            var l = new List<string>();
            foreach (PrologTerm t in tmp)
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
        public IEnumerator<PrologTerm> GetEnumerator()
        {
            //Check.Require(IsList);
            Contract.Requires(IsList);
            PrologTerm t; //null;
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
        private bool Next(out PrologTerm termRef)
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
            throw new PlTypeException("list", this);
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
            foreach (PrologTerm t in list)
            {
                if (0 < sb.Length)
                    sb.Append(',');
                sb.Append(t.ToString());
            }
            sb.Insert(0, '[');
            sb.Append(']');
            return sb.ToString();
        }

        /// <inheritdoc />
        /// <summary>
        /// <para>If PlTerm is a list the string is build by calling ToString() for each element in the list 
        /// separated by ',' and put the brackets around '[' ']'.</para>
        /// <para></para>
        /// </summary>
        /// <seealso cref="O:string"/>
        /// <returns>A string representing the PlTerm.</returns>
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
        /// <exception cref="PlTypeException">Throws a PlTypeException if PL_get_chars/3 didn't succeeds.</exception>
        public string ToStringCanonical()
        {
            string s;
            if (0 != libswipl.PL_get_wchars(TermRef, out s, libswipl.CVT_WRITE_CANONICAL | libswipl.BUF_RING | libswipl.REP_UTF8))
                return s;
            throw new PlTypeException("text", this);
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
        /// <para>See <see cref="Unify(PrologTerm)"/> for an example.</para>
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
        public bool Unify(PrologTerm term)
        {
            return 0 != libswipl.PL_unify(TermRef, term.TermRef);
        }

        /// <inheritdoc cref="Unify(PrologTerm)"/>
        /// <param name="atom">A string to unify with</param>
        [ScriptMember("unify")]
        public bool Unify(string atom)
        {
            return 0 != libswipl.PL_unify_wchars(TermRef, PrologTermType.Atom, atom);
        }

        [ScriptMember("unify")]
        public bool Unify(int value)
        {
            return 0 != libswipl.PL_unify(TermRef, new PrologTerm(value).TermRef);
        }

        [ScriptMember("unify")]
        public bool Unify(double value)
        {
            return 0 != libswipl.PL_unify(TermRef, new PrologTerm(value).TermRef);
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
        /// <summary><para>Get the arity of the functor if <see cref="PrologTerm"/> is a compound term.</para></summary>
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
        /// <para>Get a holding the name of the functor if <see cref="PrologTerm"/> is a compound term.</para>
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

        #region cast oprators
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
        /// <exception cref="PlTypeException">Throws a PlTypeException exception</exception>
        /// <exception cref="SbsSW.DesignByContract.PreconditionException">Is thrown if the operator is used on an uninitialized PlTerm</exception>
        public static explicit operator string(PrologTerm term)
        {
            string s;
            if (0 != libswipl.PL_get_wchars(term.TermRef, out s, libswipl.CVT_ALL | libswipl.CVT_WRITE | libswipl.BUF_RING | libswipl.REP_UTF8))
                return s;
            throw new PlTypeException("text", term);
        }

        /// <summary>
        /// Yields a int if the PlTerm is a Prolog integer or float that can be converted 
        /// without loss to a int. Throws a PlTypeException exception otherwise
        /// </summary>
        /// <param name="term">A PlTerm is a Prolog integer or float that can be converted without loss to a int.</param>
        /// <returns>A C# int</returns>
        /// <exception cref="PlTypeException">Throws a PlTypeException exception if <see cref="PlType"/> 
        /// is not a <see langword="PlType.PlInteger"/> or a <see langword="PlType.PlFloat"/>.</exception>
        /// <exception cref="SbsSW.DesignByContract.PreconditionException">Is thrown if the operator is used on an uninitialized PlTerm</exception>
        public static explicit operator int(PrologTerm term)
        {
            int v = 0;
            if (0 != libswipl.PL_get_long(term.TermRef, ref v))
                return v;
            throw new PlTypeException("long", term);
        }

        /// <summary>
        /// Yields the value as a C# double if PlTerm represents a Prolog integer or float. 
        /// Throws a PlTypeException exception otherwise. 
        /// </summary>
        /// <param name="term">A PlTerm represents a Prolog integer or float</param>
        /// <returns>A C# double</returns>
        /// <exception cref="PlTypeException">Throws a PlTypeException exception if <see cref="PlType"/> 
        /// is not a <see langword="PlType.PlInteger"/> or a <see langword="PlType.PlFloat"/>.</exception>
        /// <exception cref="SbsSW.DesignByContract.PreconditionException">Is thrown if the operator is used on an uninitialized PlTerm</exception>
        public static explicit operator double(PrologTerm term)
        {
            double v = 0;
            if (0 != libswipl.PL_get_float(term.TermRef, ref v))
                return v;
            throw new PlTypeException("float", term);
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
            if (obj is PrologTerm)
                return this == ((PrologTerm)obj);
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
        public static bool operator ==(PrologTerm term1, PrologTerm term2)
        {
            return libswipl.PL_compare(term1.TermRef, term2.TermRef) == 0;
        }
        /// <inheritdoc cref="op_Equality(PrologTerm, PrologTerm)" />
        public static bool operator !=(PrologTerm term1, PrologTerm term2)
        {
            return libswipl.PL_compare(term1.TermRef, term2.TermRef) != 0;
        }
        /// <inheritdoc cref="op_Equality(PrologTerm, PrologTerm)" />
        public static bool operator <(PrologTerm term1, PrologTerm term2)
        {
            return libswipl.PL_compare(term1.TermRef, term2.TermRef) < 0;
        }
        /// <inheritdoc cref="op_Equality(PrologTerm, PrologTerm)" />
        public static bool operator >(PrologTerm term1, PrologTerm term2)
        {
            return libswipl.PL_compare(term1.TermRef, term2.TermRef) > 0;
        }
        /// <inheritdoc cref="op_Equality(PrologTerm, PrologTerm)" />
        public static bool operator <=(PrologTerm term1, PrologTerm term2)
        {
            return libswipl.PL_compare(term1.TermRef, term2.TermRef) <= 0;
        }
        /// <inheritdoc cref="op_Equality(PrologTerm, PrologTerm)" />
        public static bool operator >=(PrologTerm term1, PrologTerm term2)
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
        public static bool operator ==(PrologTerm term, int value)
        {
            int v0 = 0;
            if (0 != libswipl.PL_get_long(term.TermRef, ref v0))
                return v0 == value;
            return false; // throw new PlTypeException("integer", term);
        }
        /// <inheritdoc cref="op_Equality(PrologTerm, PrologTerm)" />
        public static bool operator ==(int value, PrologTerm term)
        {
            return term == value;
        }
        // comparison (string)
        /// <inheritdoc cref="op_Equality(PrologTerm, PrologTerm)" />
        public static bool operator ==(PrologTerm term, string value)
        {
            return ((string)term).Equals(value);
        }
        /// <inheritdoc cref="op_Equality(PrologTerm, PrologTerm)" />
        public static bool operator ==(string value, PrologTerm term)
        {
            return term == value;
        }

        #endregion

        #region Inequality Method
        /// <overloads>
        /// <summary>
        /// <para>Inequality Method overload</para>
        /// <see cref="op_Equality(PrologTerm, PrologTerm)"/>
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
        public static bool operator !=(PrologTerm term, int value)
        {
            int v0 = 0;
            if (0 != libswipl.PL_get_long(term.TermRef, ref v0))
                return v0 != value;
            return true; // throw new PlTypeException("integer", term);
        }
        /// <inheritdoc cref="op_Inequality(PrologTerm, int)" />
        public static bool operator !=(int value, PrologTerm term)
        {
            return term != value;
        }
        /// <summary>
        /// test
        /// </summary>
        /// <param name="term"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool operator !=(PrologTerm term, string value)
        {
            return !(term == value);
        }
        /// <inheritdoc cref="op_Inequality(PrologTerm, string)" />
        public static bool operator !=(string value, PrologTerm term)
        {
            return term != value;
        }

        #endregion compare operators
    }
    #endregion

    #region public class PrologTermVector
    /// <summary>
    /// <preliminary>The struct PrologTermVector represents an array of term-references.</preliminary>
    /// <para>This type is used to pass the arguments to a foreign defined predicate (see <see cref="Callback.DelegateParameterVarArgs"/>), 
    /// construct compound terms (see <see cref="PrologTerm.Compound(string, PrologTermVector)"/> 
    /// and to create queries (see <see cref="T:SbsSW.SwiPlCs.PlQuery"/>).
    /// </para>
    /// <para>The only useful member function is the overloading of [], providing (0-based) access to the elements. <see cref="PrologTermVector.this[Int32]"/> 
    /// Range checking is performed and raises a ArgumentOutOfRangeException exception.</para> 
    /// </summary>
    public struct PrologTermVector : IEquatable<PrologTermVector>
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
        public PrologTermVector(int size)
        {
            _a0 = libswipl.PL_new_term_refs(size);
            _size = size;
        }

        /// <summary>Create a PlTermV from the given <see cref="PrologTerm"/>s.</summary>
        /// <param name="term0">The first <see cref="PrologTerm"/> in the vector.</param>
        public PrologTermVector(PrologTerm term0)
        {
            _size = 1;
            _a0 = term0.TermRef;
        }

        // warning CS1573: Parameter 'term0' has no matching param tag in the XML comment for 'SbsSW.SwiPlCs.PlTermV.PlTermV(SbsSW.SwiPlCs.PlTerm, SbsSW.SwiPlCs.PlTerm)' (but other parameters do)
#pragma warning disable 1573
        /// <inheritdoc cref="PlTermV(PlTerm)" />
        /// <param name="term1">The second <see cref="PlTerm"/> in the vector.</param>
        public PrologTermVector(PrologTerm term0, PrologTerm term1)
        {
            _size = 2;
            _a0 = libswipl.PL_new_term_refs(2);
            libswipl.PL_put_term(_a0 + 0, term0.TermRef);
            libswipl.PL_put_term(_a0 + 1, term1.TermRef);
        }

        /// <inheritdoc cref="PrologTermVector(PrologTerm, PrologTerm)" />
        /// <param name="term2">The third <see cref="PrologTerm"/> in the vector.</param>
        public PrologTermVector(PrologTerm term0, PrologTerm term1, PrologTerm term2)
        {
            _size = 3;
            _a0 = libswipl.PL_new_term_refs(3);
            libswipl.PL_put_term(_a0 + 0, term0.TermRef);
            libswipl.PL_put_term(_a0 + 1, term1.TermRef);
            libswipl.PL_put_term(_a0 + 2, term2.TermRef);
        }

        public PrologTermVector(PrologTerm term0, PrologTerm term1, PrologTerm term2, PrologTerm term3)
        {
            _size = 4;
            _a0 = libswipl.PL_new_term_refs(4);
            libswipl.PL_put_term(_a0 + 0, term0.TermRef);
            libswipl.PL_put_term(_a0 + 1, term1.TermRef);
            libswipl.PL_put_term(_a0 + 2, term2.TermRef);
            libswipl.PL_put_term(_a0 + 3, term3.TermRef);
        }

        /// <summary>Create a PlTermV from the given <see cref="PrologTerm"/>[] array.</summary>
        /// <param name="terms">An array of <see cref="PrologTerm"/>s to build the vector.</param>
        /// <example>
        /// Use of Initializing an Array in CSharp
        /// <code>
        ///    PlTermV v = new PlTermV(new PlTerm[] {t1, t2, t3, t4});
        /// </code>
        /// </example>
        public PrologTermVector(/*params*/ PrologTerm[] terms)
        {
            if (null == terms)
                throw new ArgumentNullException("terms");
            _size = terms.Length;
            _a0 = libswipl.PL_new_term_refs(terms.Length);
            ulong count = 0;
            foreach (PrologTerm t in terms)
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
        public PrologTerm this[int index]
        {
            get
            {
                if (index < 0 || index >= Size)
                    throw new ArgumentOutOfRangeException("index");
                return new PrologTerm(A0 + (uint)index);  // If this line is deleted -> update comment in PlTern(term_ref)
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
            if (!(obj is PrologTermVector))
                return false;
            return Equals((PrologTermVector)obj);
        }

        ///<inheritdoc />
        /// <summary>
        /// Compare the size and A0 of the PltermV
        /// </summary>
        /// <param name="other">The PlTermV to compare</param>
        /// <returns>Return <c>false</c> if size or A0 are not equal otherwise <c>true</c>.</returns>
        ///<remarks>// TODO compare each PlTerm in PlTermV not only the refereces in A0</remarks>
        public bool Equals(PrologTermVector other)
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
        public static bool operator ==(PrologTermVector termVector1, PrologTermVector termVector2)
        {
            return termVector1.Equals(termVector2);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="termVector1"></param>
        /// <param name="termVector2"></param>
        /// <returns></returns>
        public static bool operator !=(PrologTermVector termVector1, PrologTermVector termVector2)
        {
            return !termVector1.Equals(termVector2);
        }


        #endregion


    } // class PlTermV
    #endregion

    #region public class PlFrame

    /// <summary>
    /// <para>The class PlFrame provides an interface to discard unused term-references as well as rewinding unifications (data-backtracking). 
    /// Reclaiming unused term-references is automatically performed after a call to a C#-defined predicate has finished and 
    /// returns control to Prolog. In this scenario PlFrame is rarely of any use.</para>
    /// <para>This class comes into play if the top level program is defined in C# and calls Prolog multiple times. 
    /// Setting up arguments to a query requires term-references and using PlFrame is the only way to reclaim them.</para>
    /// </summary>
    /// <remarks>see <see href="http://www.swi-prolog.org/pldoc/package/pl2cpp.html#sec:8.1"/></remarks>
    /// <example>
    /// A typical use for PlFrame is the definition of C# methods that call Prolog and may be called repeatedly from C#.
    /// Consider the definition of assertWord(), adding a fact to word/1:
    ///     <code source="..\..\TestSwiPl\PlFrame.cs" region="AssertWord2_doc" />
    /// alternatively you can use
    ///     <code source="..\..\TestSwiPl\PlFrame.cs" region="AssertWord_doc" />
    /// <b><note type="caution"> NOTE: in any case you have to destroy any query object used inside a PlFrame</note></b>
    /// </example>
    public class PrologFrame : IDisposable
    {
        #region IDisposable
        // see : "Implementing a Dispose Method  [C#]" in  ms-help://MS.VSCC/MS.MSDNVS/cpguide/html/cpconimplementingdisposemethod.htm
        // and IDisposable in class PlQuery

        private bool _disposed;

        /// <summary>Implement IDisposable.</summary>
        /// <remarks>
        /// <para>Do not make this method virtual.</para>
        /// <para>A derived class should not be able to override this method.</para>
        /// </remarks>
        public void Dispose()
        {
            Dispose(true);
            // Take yourself off of the Finalization queue 
            // to prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Free other state (managed objects).
                }
                // Free your own state (unmanaged objects).
                // Set large fields to null.
                _disposed = true;
                Free();
            }
        }

        #endregion

        private uintptr_t _fid; // fid_t

        /// <summary>
        /// Creating an instance of this class marks all term-references created afterwards to be valid only in the scope of this instance.
        /// </summary>
        public PrologFrame()
        {
            _fid = libswipl.PL_open_foreign_frame();
        }

        /// <summary>
        /// Reclaims all term-references created after constructing the instance.
        /// </summary>
        ~PrologFrame()
        {
            Dispose(false);
        }

        /// <summary>
        /// Discards all term-references and global-stack data created as well as undoing all unifications after the instance was created.
        /// </summary>
        public void Rewind()
        {
            libswipl.PL_rewind_foreign_frame(_fid);
        }

        /// <summary>called by Dispose</summary>
        private void Free()
        {
            if (_fid > 0 && SWI.IsInitialized)
            {
                libswipl.PL_close_foreign_frame(_fid);
            }
            _fid = 0;
        }
    }
    #endregion
}

namespace SWI_Prolog
{
    /// <summary>
    /// This static class represents the prolog engine.
    /// </summary>
    /// <example>
    /// A sample
    /// <code>
    ///    if (!PlEngine.IsInitialized)
    ///    {
    ///        String[] empty_param = { "" };
    ///        PlEngine.Initialize(empty_param);
    ///        // do some funny things ...
    ///        PlEngine.PlCleanup();
    ///    } 
    ///    // program ends here
    /// </code>
    /// The following sample show how a file is consult via comand-line options.
    /// <code source="..\..\TestSwiPl\PlEngine.cs" region="demo_consult_pl_file_by_param" />
    /// </example>
    public static class SWI
    {
        public static bool RegisterForeign(string module, string name, int arity, Delegate method)
        {
            return RegisterForeign(module, name, arity, method, ForeignSwitches.None);
        }
        public static bool RegisterForeign(string module, string name, int arity, Delegate method, ForeignSwitches plForeign)
        {
            return Convert.ToBoolean(libswipl.PL_register_foreign_in_module(module, name, arity, method, (int)plForeign));
        }
        public static NondeterministicCalltype GetNondeterministicCallType(IntPtr control_t)
        {
            return (NondeterministicCalltype)libswipl.PL_foreign_control(control_t);
        }
        public static IntPtr Retry(object context)
        {
            return libswipl.PL_retry_address(Marshal.GetIUnknownForObject(context));
        }
        public static object GetContext(IntPtr control_t)
        {
            return Marshal.GetObjectForIUnknown(libswipl.PL_foreign_context_address(control_t));
        }

        /// <summary>To test if the prolog engine is up.</summary>
        public static bool IsInitialized
        {
            get
            {
                var i = libswipl.PL_is_initialised(IntPtr.Zero, IntPtr.Zero);
                return 0 != i;
            }
        }

        /// <summary>
        /// <para>Initialise SWI-Prolog</para>
        /// <para>The write method of the output stream is redirected to <see cref="SbsSW.SwiPlCs.Streams"/> 
        /// before Initialize. The read method of the input stream just after Initialize.</para>
        /// </summary>
        /// <remarks>
        /// <para>A known bug: Initialize work *not* as expected if there are e.g. German umlauts in the parameters
        /// See marshalling in the sorce NativeMethods.cs</para>
        /// </remarks>
        /// <param name="argv">
        /// <para>For a complete parameter description see the <a href="http://gollem.science.uva.nl/SWI-Prolog/Manual/cmdline.html" target="_new">SWI-Prolog reference manual section 2.4 Command-line options</a>.</para>
        /// <para>sample parameter: <code>String[] param = { "-q", "-f", @"some\filename" };</code>
        /// At the first position a parameter "" is added in this method. <see href="http://www.swi-prolog.org/pldoc/doc_for?object=section(3%2C%20%279.6.20%27%2C%20swi(%27%2Fdoc%2FManual%2Fforeigninclude.html%27))">PL_initialise</see>
        /// </para>
        /// </param>
        /// <example>For an example see <see cref="T:SbsSW.SwiPlCs.PlEngine"/> </example>
        public static void Initialize(string[] argv)
        {
            if (argv == null)
                throw new ArgumentNullException("argv", "Minimum is one empty string");
            if (IsInitialized)
                throw new PlLibException("PlEngine is already initialized");

            libswipl.LoadLibPl();
            // redirect input and output stream to receive messages from prolog
            var wf = new DelegateStreamWriteFunction(Swrite_function);
            if (!_isStreamFunctionWriteModified)
            {
                // TO DO
                //SetStreamFunctionWrite(PlStreamType.Output, wf);
                _isStreamFunctionWriteModified = false;
            }
            var localArgv = new string[argv.Length + 1];
            int idx = 0;
            localArgv[idx++] = "";
            foreach (var s in argv)
                localArgv[idx++] = s;

            if (0 == libswipl.PL_initialise(localArgv.Length, localArgv))
            {
                throw new PlLibException("failed to initialize");
            }
            if (!_isStreamFunctionReadModified)
            {
                var rf = new DelegateStreamReadFunction(Sread_function);
                // TO DO
                //SetStreamFunctionRead(PlStreamType.Input, rf);
                _isStreamFunctionReadModified = false;
            }
        }

        /// <summary>
        /// Try a clean up but it is buggy
        /// search the web for "possible regression from pl-5.4.7 to pl-5.6.27" to see reasons
        /// </summary>
        /// <remarks>Use this method only at the last call before run program ends</remarks>
        static public void Cleanup()
        {
            libswipl.PL_cleanup(0);
        }

        /// <summary>Stops the PlEngine and <b>the program</b></summary>
        /// <remarks>SWI-Prolog calls internally pl_cleanup and than exit(0)</remarks>
        static public void Halt()
        {
            libswipl.PL_halt(0);
        }

        /// <summary>Stops the PlEngine and <b>the program</b></summary>
        /// <remarks>SWI-Prolog calls internally pl_cleanup and than exit(0)</remarks>
        static public void Halt(int code)
        {
            libswipl.PL_halt(code);
        }

        // *****************************
        // STATICs for STREAMS
        // *****************************
        #region stream IO


        #region default_io_doc
        static internal long Swrite_function(IntPtr handle, string buf, long bufsize)
        {
            string s = buf.Substring(0, (int)bufsize);
            Console.Write(s);
            System.Diagnostics.Trace.WriteLine(s);
            return bufsize;
        }

        static internal long Sread_function(IntPtr handle, IntPtr buf, long bufsize)
        {
            throw new PlLibException("SwiPlCs: Prolog try to read from stdin");
        }
        #endregion default_io_doc



        static bool _isStreamFunctionWriteModified;  // default = false;
        static bool _isStreamFunctionReadModified;   // default = false;

        /// <summary>
        /// This is a primitive approach to enter the output from a stream.
        /// </summary>
        /// <example>
        /// <code source="..\..\TestSwiPl\StreamIO.cs" region="StreamWrite_doc" />
        /// </example>
        /// <param name="streamType">Determine which stream to use <see cref="Streams.PlStreamType"/></param>
        /// <param name="function">A <see cref="Streams.DelegateStreamWriteFunction"/></param>
        static public void SetStreamFunctionWrite(PlStreamType streamType, DelegateStreamWriteFunction function)
        {
            libswipl.LoadLibPl();
            libswipl.SetStreamFunction(streamType, libswipl.StreamsFunction.Write, function);
            _isStreamFunctionWriteModified = true;
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <example>
        /// <code source="..\..\TestSwiPl\StreamIO.cs" region="StreamRead_doc" />
        /// </example>
        /// <param name="streamType">Determine which stream to use <see cref="Streams.PlStreamType"/></param>
        /// <param name="function">A <see cref="Streams.DelegateStreamReadFunction"/></param>
        static public void SetStreamFunctionRead(PlStreamType streamType, DelegateStreamReadFunction function)
        {
            libswipl.LoadLibPl();
            libswipl.SetStreamFunction(streamType, libswipl.StreamsFunction.Read, function);
            _isStreamFunctionReadModified = true;
        }


        #endregion stream IO

        // *****************************
        // STATICs for MULTI THreading
        // *****************************
        #region STATICs for MULTI THreading

        /// <summary>
        /// <para>return : reference count of the engine</para>
        ///	<para>		If an error occurs, -1 is returned.</para>
        ///	<para>		If this Prolog is not compiled for multi-threading, -2 is returned.</para>
        /// </summary>
        /// <returns>A reference count of the engine</returns>
        public static int PlThreadAttachEngine()
        {
            return libswipl.PL_thread_attach_engine(IntPtr.Zero);
        }


        /// <summary>
        /// This method is also provided in the single-threaded version of SWI-Prolog, where it returns -2. 
        /// </summary>
        /// <returns>Returns the integer Prolog identifier of the engine or -1 if the calling thread has no Prolog engine. </returns>
        public static int PlThreadSelf()
        {
            return libswipl.PL_thread_self();
        }


        /// <summary>
        /// Destroy the Prolog engine in the calling thread. 
        /// Only takes effect if <c>PL_thread_destroy_engine()</c> is called as many times as <c>PL_thread_attach_engine()</c> in this thread.
        /// <para>Please note that construction and destruction of engines are relatively expensive operations. Only destroy an engine if performance is not critical and memory is a critical resource.</para>
        /// </summary>
        /// <returns>Returns <c>true</c> on success and <c>false</c> if the calling thread has no engine or this Prolog does not support threads.</returns>
        public static bool PlThreadDestroyEngine()
        {
            return 0 != libswipl.PL_thread_destroy_engine();
        }

        #endregion

    }

    /// <summary>
    /// This class is experimental
    /// </summary>
    public class SWI_MT : IDisposable
    {
        private IntPtr _iEngineNumber = IntPtr.Zero;
        // private IntPtr _iEngineNumberStore = IntPtr.Zero;


        #region IDisposable
        // see : "Implementing a Dispose Method  [C#]" in  ms-help://MS.VSCC/MS.MSDNVS/cpguide/html/cpconimplementingdisposemethod.htm
        // and IDisposable in class PlQuery

        // Implement IDisposable.
        // Do not make this method virtual.
        // A derived class should not be able to override this method.
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            // Take yourself off of the Finalization queue 
            // to prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // Free other state (managed objects).
            }
            // Free your own state (unmanaged objects).
            // Set large fields to null.
            Free();
        }


        #endregion

        /// <summary>
        /// 
        /// </summary>
        public void Free()
        {
            if (IntPtr.Zero != _iEngineNumber && SWI.IsInitialized)
            {
                if (0 == libswipl.PL_destroy_engine(_iEngineNumber))
                    throw (new PlLibException("failed to destroy engine"));
                _iEngineNumber = IntPtr.Zero;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        ~SWI_MT()
        {
            Dispose(false);
        }

        /// <summary>
        /// 
        /// </summary>
        public SWI_MT()
        {
            if (0 != libswipl.PL_is_initialised(IntPtr.Zero, IntPtr.Zero))
            {
                try
                {
                    _iEngineNumber = libswipl.PL_create_engine(IntPtr.Zero);
                }
                catch (Exception ex)
                {
                    throw (new PlLibException("PL_create_engine : " + ex.Message));
                }
            }
            else
            {
                throw new PlLibException("There is no PlEngine initialized");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void PlSetEngine()
        {
            IntPtr pNullPointer = IntPtr.Zero;
            int iRet = libswipl.PL_set_engine(_iEngineNumber, ref pNullPointer);
            switch (iRet)
            {
                case libswipl.PL_ENGINE_SET: break; // all is fine
                case libswipl.PL_ENGINE_INVAL: throw (new PlLibException("PlSetEngine returns Invalid")); //break;
                case libswipl.PL_ENGINE_INUSE: throw (new PlLibException("PlSetEngine returns it is used by an other thread")); //break;
                default: throw (new PlLibException("Unknown return from PlSetEngine"));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        public void PlDetachEngine()
        {
            IntPtr pNullPointer = IntPtr.Zero;
            int iRet = libswipl.PL_set_engine(IntPtr.Zero, ref pNullPointer);
            switch (iRet)
            {
                case libswipl.PL_ENGINE_SET: break; // all is fine
                case libswipl.PL_ENGINE_INVAL: throw (new PlLibException("PlSetEngine(detach) returns Invalid")); //break;
                case libswipl.PL_ENGINE_INUSE: throw (new PlLibException("PlSetEngine(detach) returns it is used by an other thread")); //break;
                default: throw (new PlLibException("Unknown return from PlSetEngine(detach)"));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        override public string ToString()
        {
            return _iEngineNumber.ToString();
        }
    }
}