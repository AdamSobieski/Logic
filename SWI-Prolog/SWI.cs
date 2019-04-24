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

using SWI_Prolog.Callback;
using SWI_Prolog.Exceptions;
using SWI_Prolog.Streams;
using System;
using System.Runtime.InteropServices;

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
    public enum StreamType
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

        public static object MarshalFromPrologTerm(Logic.Prolog.PrologTerm term)
        {
            throw new NotImplementedException();
        }
        public static Logic.Prolog.PrologTerm MarshalToPrologTerm(object term)
        {
            throw new NotImplementedException();
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
        /// <param name="streamType">Determine which stream to use <see cref="Streams.StreamType"/></param>
        /// <param name="function">A <see cref="Streams.DelegateStreamWriteFunction"/></param>
        static public void SetStreamFunctionWrite(StreamType streamType, DelegateStreamWriteFunction function)
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
        /// <param name="streamType">Determine which stream to use <see cref="Streams.StreamType"/></param>
        /// <param name="function">A <see cref="Streams.DelegateStreamReadFunction"/></param>
        static public void SetStreamFunctionRead(StreamType streamType, DelegateStreamReadFunction function)
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
        public static int ThreadAttachEngine()
        {
            return libswipl.PL_thread_attach_engine(IntPtr.Zero);
        }


        /// <summary>
        /// This method is also provided in the single-threaded version of SWI-Prolog, where it returns -2. 
        /// </summary>
        /// <returns>Returns the integer Prolog identifier of the engine or -1 if the calling thread has no Prolog engine. </returns>
        public static int ThreadSelf()
        {
            return libswipl.PL_thread_self();
        }


        /// <summary>
        /// Destroy the Prolog engine in the calling thread. 
        /// Only takes effect if <c>PL_thread_destroy_engine()</c> is called as many times as <c>PL_thread_attach_engine()</c> in this thread.
        /// <para>Please note that construction and destruction of engines are relatively expensive operations. Only destroy an engine if performance is not critical and memory is a critical resource.</para>
        /// </summary>
        /// <returns>Returns <c>true</c> on success and <c>false</c> if the calling thread has no engine or this Prolog does not support threads.</returns>
        public static bool ThreadDestroyEngine()
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
        public void SetEngine()
        {
            IntPtr pNullPointer = IntPtr.Zero;
            int iRet = libswipl.PL_set_engine(_iEngineNumber, ref pNullPointer);
            switch (iRet)
            {
                case libswipl.PL_ENGINE_SET: break; // all is fine
                case libswipl.PL_ENGINE_INVAL: throw (new PlLibException("SetEngine returns Invalid")); //break;
                case libswipl.PL_ENGINE_INUSE: throw (new PlLibException("SetEngine returns it is used by an other thread")); //break;
                default: throw (new PlLibException("Unknown return from SetEngine"));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        public void DetachEngine()
        {
            IntPtr pNullPointer = IntPtr.Zero;
            int iRet = libswipl.PL_set_engine(IntPtr.Zero, ref pNullPointer);
            switch (iRet)
            {
                case libswipl.PL_ENGINE_SET: break; // all is fine
                case libswipl.PL_ENGINE_INVAL: throw (new PlLibException("SetEngine(detach) returns Invalid")); //break;
                case libswipl.PL_ENGINE_INUSE: throw (new PlLibException("SetEngine(detach) returns it is used by an other thread")); //break;
                default: throw (new PlLibException("Unknown return from SetEngine(detach)"));
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