﻿/*********************************************************
* 
*  Authors:        Uwe Lesta
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


//#define USE_PRINT_MESSAGE

using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Logic.Prolog.Swi.Exceptions
{
    #region class PlLibException
    /// <summary>This exception is thrown if something in the interface went wrong.</summary>
    [Serializable]
    public class PrologLibraryException : Exception
    {

        /// <inheritdoc />
        public PrologLibraryException()
        {
        }
        /// <inheritdoc />
        public PrologLibraryException(string message)
            : base(message)
        {
        }
        /// <inheritdoc />
        public PrologLibraryException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
        #region implementation of ISerializable

        // ISerializable Constructor
        /// <inheritdoc />
        protected PrologLibraryException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        // see http://msdnwiki.microsoft.com/en-us/mtpswiki/f1d0010b-14fb-402f-974f-16318f0bc19f.aspx
        /// <inheritdoc />
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException("info");
            base.GetObjectData(info, context);
        }
        #endregion implementation of ISerializable
    }
    #endregion class PlLibException

    #region class PlException
    /// <inheritdoc />
    /// <summary>
    /// <para>This class is the base class to catch exceptions thrown by prolog in C#.</para>
    /// </summary>
    /// <example>
    ///     <code source="..\..\TestSwiPl\PlException.cs" region="prolog_exception_sample_doc" />
    /// </example>
    /// <seealso cref="PrologTypeException"/>
    /// <seealso href="http://gollem.science.uva.nl/SWI-Prolog/Manual/exception.html">SWI-Prolog Manual - 4.9 ISO compliant Exception handling</seealso>
    [Serializable]
    public class PrologException : Exception
    {
        private string _messagePl;
        private PrologTerm _exTerm;

        /// <summary>provide somtimes some additional information about the exceptions reason.</summary>
        public string MessagePl { get { return _messagePl; } }


        /// <inheritdoc />
        public PrologException()
        {
            _exTerm = PrologTerm.Variable();
        }
        /// <inheritdoc />
        public PrologException(string message)
            : base(message)
        {
            _messagePl = message;
            _exTerm = new PrologTerm(message);
        }
        /// <inheritdoc />
        public PrologException(string message, Exception innerException)
            : base(message, innerException)
        {
            if (null == innerException)
                throw new ArgumentNullException("innerException");
            _messagePl = message + "; innerExeption:" + innerException.Message;
            _exTerm = new PrologTerm(message);
        }

        #region implementation of ISerializable

        // ISerializable Constructor
        /// <inheritdoc />
        protected PrologException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            if (info == null)
                throw new ArgumentNullException("info");
            _messagePl = (string)info.GetValue("_messagePl", typeof(string));
            _exTerm = (PrologTerm)info.GetValue("_exTerm", typeof(PrologTerm));
        }

        // see http://msdnwiki.microsoft.com/en-us/mtpswiki/f1d0010b-14fb-402f-974f-16318f0bc19f.aspx
        /// <inheritdoc />
        [SecurityPermissionAttribute(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
                throw new ArgumentNullException("info");
            base.GetObjectData(info, context);
            info.AddValue("_messagePl", _messagePl);
            info.AddValue("_exTerm", _exTerm);
        }
        #endregion implementation of ISerializable


        /// <summary>
        /// <para>To catch a exception thrown by prolog</para>
        /// <para>For a example see <see cref="PrologException"/>.</para>
        /// </summary>
        /// <param name="term">A PlTerm containing the Prolog exception</param>
        /// <see cref="PrologException"/>
        public PrologException(PrologTerm term)
        {
            _exTerm = new PrologTerm(term.TermRef);  // If this line is deleted -> update comment in PlTern(term_ref)
        }

        /// <summary>
        /// Get the <see cref="PrologTerm"/> of this exception.
        /// </summary>
        public PrologTerm Term { get { return _exTerm; } }

        /// <inheritdoc />
		public override string Message
        {
            get { return ToString(); }
        }

        //operator char *(void);
        /// <inheritdoc />
        /// <summary>
        /// The exception is translated into a message as produced by print_message/2. The character data is stored in a ring.
        /// </summary>
        /// <returns>A textual description of the Exception</returns>
        override public string ToString()
        {
            if (!SWI.IsInitialized)
                return "A PlException was thrown but it can't formatted because PlEngine is not Initialized.";

            string strRet = "[ERROR: Failed to generate message.  Internal error]\n";
            using (new PrologFrame())
            {

#if USE_PRINT_MESSAGE
				PlTermV av = new PlTermV(2);

                av[0] = PlTerm.PlCompound("print_message", new PlTermV(new PlTerm("error"), new PlTerm( _exTerm.TermRef)));
				PlQuery q = new PlQuery("$write_on_string", av);
				if ( q.NextSolution() )
					strRet = (string)av[1];
				q.Free();
#else
                var av = new PrologTermVector(2);
                av[0] = new PrologTerm(_exTerm.TermRef);
                using (var q = new PrologQuery("$messages", "message_to_string", av))
                {
                    if (q.NextSolution())
                        strRet = av[1].ToString();
                }
#endif
            }
            return strRet;
        }

        /// <summary>
        /// Generate an exception (as throw/1) and return <c>false</c>.
        /// </summary>
        /// <remarks>Used in the PREDICATE() wrapper to pass the exception to Prolog. See PL_raise_exeption().</remarks>
        /// <returns>Generate an exception (as throw/1) and return <c>false</c>.</returns>
		public bool PlThrow()
        {
            return libswipl.PL_raise_exception(_exTerm.TermRef) == 1;
        }

        /// <summary>
        /// Throw this PlException.
        /// </summary>
        /// <remarks>see <see href="http://www.swi-prolog.org/packages/pl2cpp.html#cppThrow()"/></remarks>
		public void Throw()
        {
            // term_t
            uintptr_t a = libswipl.PL_new_term_ref();
            // atom_t 
            uintptr_t name = 0;
            int arity = 0;

            if (0 != libswipl.PL_get_arg(1, _exTerm.TermRef, a) && 0 != libswipl.PL_get_name_arity(a, ref name, ref arity))
            {
                string str = libswipl.PL_atom_wchars(name);

                if (str == "type_error")
                    throw new PrologTypeException(_exTerm);
                if (str == "domain_error")
                    throw new PrologDomainException(_exTerm);
            }
            _messagePl = Message;
            throw this;
        }

    } // class PlException 
    #endregion

    #region class PlTypeException
    /// <inheritdoc />
    /// <summary>
    /// A type error expresses that a term does not satisfy the expected basic Prolog type.
    /// </summary>
    /// <example>
    /// This sample demonstrate how to catch a PlTypeException in C# that is thrown somewhere int the prolog code.
    ///     <code source="..\..\TestSwiPl\PlException.cs" region="prolog_type_exception_sample_doc" />
    /// </example>
    [Serializable]
    public class PrologTypeException : PrologException
    {
        /// <inheritdoc />
		public PrologTypeException()
        {
        }
        /// <inheritdoc />
        public PrologTypeException(string message)
            : base(message)
        {
        }
        /// <inheritdoc />
        public PrologTypeException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
        /// <inheritdoc />
        protected PrologTypeException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }


        /// <inheritdoc />
        public PrologTypeException(PrologTerm term)
            : base(term)
        {
        }

        /// <summary>
        /// Creates an ISO standard Prolog error term expressing the expected type and actual term that does not satisfy this type.
        /// </summary>
        /// <param name="expected">The type which was expected</param>
        /// <param name="actual">The actual term</param>
		public PrologTypeException(string expected, PrologTerm actual)
            : base(
            PrologTerm.Compound("error",
            new PrologTermVector(PrologTerm.Compound("type_error",
            new PrologTermVector(new PrologTerm(expected), actual)),
            PrologTerm.Variable())
            ))
        {
        }
    } // class PlTypeException
    #endregion class PlTypeException

    #region class PlDomainException
    /// <summary>
    /// A domain exception expresses that a term satisfies the basic Prolog type expected, but is unacceptable
    /// to the restricted domain expected by some operation. 
    /// </summary>
    /// <example>
    /// For example, the standard Prolog open/3 call expect an IO-Mode (read, write, append, ...).
    /// If an integer is provided, this is a type error, if an atom other than one of the defined IO-modes is provided it is a domain error.
    ///     <code source="..\..\TestSwiPl\PlException.cs" region="prolog_domain_exception_sample_doc" />
    /// </example>
    [Serializable]
    public class PrologDomainException : PrologException
    {

        /// <inheritdoc cref="T:SbsSW.SwiPlCs.Exceptions.PlException" />
        public PrologDomainException()
        { }

        /// <inheritdoc cref="T:SbsSW.SwiPlCs.Exceptions.PlException" />
        public PrologDomainException(string message)
            : base(message)
        {
        }

        /// <inheritdoc cref="T:SbsSW.SwiPlCs.Exceptions.PlException" />
        public PrologDomainException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <inheritdoc cref="PrologException" />
        protected PrologDomainException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <inheritdoc cref="PrologException" />
        public PrologDomainException(PrologTerm term)
            : base(term)
        { }
        /*
		PlDomainException(string expected, PlTerm actual)
			:
			base(new PlCompound("error",
			new PlTermV(new PlCompound("domain_error",
			new PlTermV(new PlTerm(expected), actual)),
			PlTerm.PlVar())
			)
			)
		{ }
		 */
    }
    #endregion class PlDomainException
}