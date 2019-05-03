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

namespace Logic.Prolog.Swi.Streams
{
    /// <summary>
    /// The standard SWI-Prolog streams ( input output error )
    /// </summary>
    public enum SwiStreamType
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

namespace Logic.Prolog.Swi
{
    //internal static class SWI
    //{
    //    //public static void Initialize(string[] argv)
    //    //{
    //    //    if (argv == null)
    //    //        throw new ArgumentNullException("argv", "Minimum is one empty string");
    //    //    if (libswipl.PL_is_initialised(IntPtr.Zero, IntPtr.Zero) != 0)
    //    //        throw new SwiPrologLibraryException("SwiPrologEngine is already initialized");

    //    //    libswipl.LoadLibPl();
    //    //    // redirect input and output stream to receive messages from prolog
    //    //    var wf = new DelegateStreamWriteFunction(Swrite_function);
    //    //    if (!_isStreamFunctionWriteModified)
    //    //    {
    //    //        // TO DO
    //    //        //SetStreamFunctionWrite(PlStreamType.Output, wf);
    //    //        _isStreamFunctionWriteModified = false;
    //    //    }
    //    //    var localArgv = new string[argv.Length + 1];
    //    //    int idx = 0;
    //    //    localArgv[idx++] = "";
    //    //    foreach (var s in argv)
    //    //        localArgv[idx++] = s;

    //    //    if (0 == libswipl.PL_initialise(localArgv.Length, localArgv))
    //    //    {
    //    //        throw new SwiPrologLibraryException("failed to initialize");
    //    //    }
    //    //    if (!_isStreamFunctionReadModified)
    //    //    {
    //    //        var rf = new DelegateStreamReadFunction(Sread_function);
    //    //        // TO DO
    //    //        //SetStreamFunctionRead(PlStreamType.Input, rf);
    //    //        _isStreamFunctionReadModified = false;
    //    //    }
    //    //}

    //    #region stream IO


    //    #region default_io_doc
    //    static internal long Swrite_function(IntPtr handle, string buf, long bufsize)
    //    {
    //        string s = buf.Substring(0, (int)bufsize);
    //        Console.Write(s);
    //        System.Diagnostics.Trace.WriteLine(s);
    //        return bufsize;
    //    }

    //    static internal long Sread_function(IntPtr handle, IntPtr buf, long bufsize)
    //    {
    //        throw new SwiPrologLibraryException("SwiPlCs: Prolog try to read from stdin");
    //    }
    //    #endregion default_io_doc



    //    static bool _isStreamFunctionWriteModified;  // default = false;
    //    static bool _isStreamFunctionReadModified;   // default = false;

    //    /// <summary>
    //    /// This is a primitive approach to enter the output from a stream.
    //    /// </summary>
    //    /// <example>
    //    /// <code source="..\..\TestSwiPl\StreamIO.cs" region="StreamWrite_doc" />
    //    /// </example>
    //    /// <param name="streamType">Determine which stream to use <see cref="Streams.SwiStreamType"/></param>
    //    /// <param name="function">A <see cref="Streams.DelegateStreamWriteFunction"/></param>
    //    static public void SetStreamFunctionWrite(SwiStreamType streamType, DelegateStreamWriteFunction function)
    //    {
    //        libswipl.LoadLibPl();
    //        libswipl.SetStreamFunction(streamType, libswipl.StreamsFunction.Write, function);
    //        _isStreamFunctionWriteModified = true;
    //    }

    //    /// <summary>
    //    /// TODO
    //    /// </summary>
    //    /// <example>
    //    /// <code source="..\..\TestSwiPl\StreamIO.cs" region="StreamRead_doc" />
    //    /// </example>
    //    /// <param name="streamType">Determine which stream to use <see cref="Streams.SwiStreamType"/></param>
    //    /// <param name="function">A <see cref="Streams.DelegateStreamReadFunction"/></param>
    //    static public void SetStreamFunctionRead(SwiStreamType streamType, DelegateStreamReadFunction function)
    //    {
    //        libswipl.LoadLibPl();
    //        libswipl.SetStreamFunction(streamType, libswipl.StreamsFunction.Read, function);
    //        _isStreamFunctionReadModified = true;
    //    }


    //    #endregion stream IO

    //    // *****************************
    //    // STATICs for MULTI THreading
    //    // *****************************
    //    #region STATICs for MULTI THreading

    //    /// <summary>
    //    /// <para>return : reference count of the engine</para>
    //    ///	<para>		If an error occurs, -1 is returned.</para>
    //    ///	<para>		If this Prolog is not compiled for multi-threading, -2 is returned.</para>
    //    /// </summary>
    //    /// <returns>A reference count of the engine</returns>
    //    public static int ThreadAttachEngine()
    //    {
    //        return libswipl.PL_thread_attach_engine(IntPtr.Zero);
    //    }


    //    /// <summary>
    //    /// This method is also provided in the single-threaded version of SWI-Prolog, where it returns -2. 
    //    /// </summary>
    //    /// <returns>Returns the integer Prolog identifier of the engine or -1 if the calling thread has no Prolog engine. </returns>
    //    public static int ThreadSelf()
    //    {
    //        return libswipl.PL_thread_self();
    //    }


    //    /// <summary>
    //    /// Destroy the Prolog engine in the calling thread. 
    //    /// Only takes effect if <c>PL_thread_destroy_engine()</c> is called as many times as <c>PL_thread_attach_engine()</c> in this thread.
    //    /// <para>Please note that construction and destruction of engines are relatively expensive operations. Only destroy an engine if performance is not critical and memory is a critical resource.</para>
    //    /// </summary>
    //    /// <returns>Returns <c>true</c> on success and <c>false</c> if the calling thread has no engine or this Prolog does not support threads.</returns>
    //    public static bool ThreadDestroyEngine()
    //    {
    //        return 0 != libswipl.PL_thread_destroy_engine();
    //    }

    //    #endregion

    //}

    ///// <summary>
    ///// This class is experimental
    ///// </summary>
    //internal class SWI_MT : IDisposable
    //{
    //    private IntPtr _iEngineNumber = IntPtr.Zero;
    //    // private IntPtr _iEngineNumberStore = IntPtr.Zero;

    //    #region IDisposable
    //    // see : "Implementing a Dispose Method  [C#]" in  ms-help://MS.VSCC/MS.MSDNVS/cpguide/html/cpconimplementingdisposemethod.htm
    //    // and IDisposable in class PlQuery

    //    // Implement IDisposable.
    //    // Do not make this method virtual.
    //    // A derived class should not be able to override this method.
    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public void Dispose()
    //    {
    //        Dispose(true);
    //        // Take yourself off of the Finalization queue 
    //        // to prevent finalization code for this object
    //        // from executing a second time.
    //        GC.SuppressFinalize(this);
    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="disposing"></param>
    //    protected virtual void Dispose(bool disposing)
    //    {
    //        if (disposing)
    //        {
    //            // Free other state (managed objects).
    //        }
    //        // Free your own state (unmanaged objects).
    //        // Set large fields to null.
    //        Free();
    //    }


    //    #endregion

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public void Free()
    //    {
    //        if (IntPtr.Zero != _iEngineNumber && libswipl.PL_is_initialised(IntPtr.Zero, IntPtr.Zero) != 0)
    //        {
    //            if (0 == libswipl.PL_destroy_engine(_iEngineNumber))
    //                throw (new SwiPrologLibraryException("failed to destroy engine"));
    //            _iEngineNumber = IntPtr.Zero;
    //        }
    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    ~SWI_MT()
    //    {
    //        Dispose(false);
    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public SWI_MT()
    //    {
    //        if (0 != libswipl.PL_is_initialised(IntPtr.Zero, IntPtr.Zero))
    //        {
    //            try
    //            {
    //                _iEngineNumber = libswipl.PL_create_engine(IntPtr.Zero);
    //            }
    //            catch (Exception ex)
    //            {
    //                throw (new SwiPrologLibraryException("PL_create_engine : " + ex.Message));
    //            }
    //        }
    //        else
    //        {
    //            throw new SwiPrologLibraryException("There is no PlEngine initialized");
    //        }
    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    public void SetEngine()
    //    {
    //        IntPtr pNullPointer = IntPtr.Zero;
    //        int iRet = libswipl.PL_set_engine(_iEngineNumber, ref pNullPointer);
    //        switch (iRet)
    //        {
    //            case libswipl.PL_ENGINE_SET: break; // all is fine
    //            case libswipl.PL_ENGINE_INVAL: throw (new SwiPrologLibraryException("SetEngine returns Invalid")); //break;
    //            case libswipl.PL_ENGINE_INUSE: throw (new SwiPrologLibraryException("SetEngine returns it is used by an other thread")); //break;
    //            default: throw (new SwiPrologLibraryException("Unknown return from SetEngine"));
    //        }
    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
    //    public void DetachEngine()
    //    {
    //        IntPtr pNullPointer = IntPtr.Zero;
    //        int iRet = libswipl.PL_set_engine(IntPtr.Zero, ref pNullPointer);
    //        switch (iRet)
    //        {
    //            case libswipl.PL_ENGINE_SET: break; // all is fine
    //            case libswipl.PL_ENGINE_INVAL: throw (new SwiPrologLibraryException("SetEngine(detach) returns Invalid")); //break;
    //            case libswipl.PL_ENGINE_INUSE: throw (new SwiPrologLibraryException("SetEngine(detach) returns it is used by an other thread")); //break;
    //            default: throw (new SwiPrologLibraryException("Unknown return from SetEngine(detach)"));
    //        }
    //    }

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <returns></returns>
    //    override public string ToString()
    //    {
    //        return _iEngineNumber.ToString();
    //    }
    //}
}