/*********************************************************
* 
*  Authors:        Adam Sobieski
*
*********************************************************/

// Uncomment the following line to compile on Linux or MacOS
// #define _LINUX

using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;

namespace Logic.Prolog.Xsb
{
    #region Safe Handles and Native imports
    // See http://msdn.microsoft.com/msdnmag/issues/05/10/Reliability/ for more about safe handles.
    [SecurityPermission(SecurityAction.LinkDemand, UnmanagedCode = true)]
    sealed class SafeLibraryHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        private SafeLibraryHandle() : base(true) { }

        protected override bool ReleaseHandle()
        {
            return NativeMethods.UnLoadDll(handle);
        }

        public bool UnLoad()
        {
            return ReleaseHandle();
        }
    }

    internal static class NativeMethods
    {
#if _LINUX
#warning _LINUX is defined (libdl.so must be available). Compiling for Linux
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////// 
        //Linux Compatibility		
        public static int RTLD_NOW = 2; // for dlopen's flags

        const string s_kernel_linux = "libdl.so";

        [DllImport(s_kernel_linux, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Auto)]
        private static extern SafeLibraryHandle dlopen([In] string filename, [In] int flags);

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [DllImport(s_kernel_linux, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool dlclose(IntPtr hModule);

        // see: http://blogs.msdn.com/jmstall/archive/2007/01/06/Typesafe-GetProcAddress.aspx
        [DllImport(s_kernel_linux, CharSet = CharSet.Ansi, BestFitMapping = false, SetLastError = true)]
        internal static extern IntPtr dlsym(SafeLibraryHandle hModule, String procname);
        
#else
#warning _LINUX is *not* defined (kernel32.dll must be available). Compiling for Windows
        //////////////////////////////////////////////////////////////////////////////////////////////////////////////// 
        //Windows
        const string WinKernel32 = "kernel32";
        [DllImport(WinKernel32, CharSet = CharSet.Auto, BestFitMapping = false, SetLastError = true)]
        private static extern SafeLibraryHandle LoadLibrary(string fileName);

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [DllImport(WinKernel32, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool FreeLibrary(IntPtr hModule);

        // see: http://blogs.msdn.com/jmstall/archive/2007/01/06/Typesafe-GetProcAddress.aspx
        [DllImport(WinKernel32, CharSet = CharSet.Ansi, BestFitMapping = false, SetLastError = true)]
        internal static extern IntPtr GetProcAddress(SafeLibraryHandle hModule, String procname);
#endif

        //////////////////////////////////////////////////////////////////////////////////////////////////////////////// 
        //Platform independant function calls
        internal static SafeLibraryHandle LoadDll(string filename)
        {
#if _LINUX
            //if (Environment.OSVersion.Platform == PlatformID.Unix ||
            //    Environment.OSVersion.Platform == PlatformID.MacOSX) {
                return dlopen(filename, RTLD_NOW);
            //}
#else
            return LoadLibrary(filename);
#endif
        }

        public static bool UnLoadDll(IntPtr hModule)
        {
#if _LINUX
            return dlclose(hModule);
#else
            return FreeLibrary(hModule);
#endif
        }
    }

    [SuppressUnmanagedCodeSecurity]
    internal static class SafeNativeMethods
    {
        private const string DllFileName = "xsb.dll";

        public static string DllFileName1
        {
            get
            {
                if (Environment.OSVersion.Platform == PlatformID.Unix ||
                    Environment.OSVersion.Platform == PlatformID.MacOSX)
                {
                    return "libxsb.so";
                }
                return DllFileName;
            }
        }

        [DllImport(DllFileName)]
        internal static extern int xsb_init(int argc, string[] argv);
        [DllImport(DllFileName)]
        internal static extern int xsb_init_string(string text);
        [DllImport(DllFileName)]
        internal static extern IntPtr xsb_get_init_error_type();
        [DllImport(DllFileName)]
        internal static extern IntPtr xsb_get_init_error_message();



        [DllImport(DllFileName)]
        internal static extern int xsb_command();
        [DllImport(DllFileName, BestFitMapping = true, CharSet = CharSet.Ansi)]
        internal static extern int xsb_command_string(string command);
        [DllImport(DllFileName)]
        internal static extern int xsb_query();
        [DllImport(DllFileName, BestFitMapping = true, CharSet = CharSet.Ansi)]
        internal static extern int xsb_query_string(string query);
        [DllImport(DllFileName)]
        internal static extern int xsb_next();
        [DllImport(DllFileName)]
        internal static extern int xsb_close_query();



        [DllImport(DllFileName)]
        internal static extern ulong reg_term(int register);



        [DllImport(DllFileName)]
        internal static extern int c2p_int(int value, ulong term);
        [DllImport(DllFileName)]
        internal static extern int c2p_float(double value, ulong term);
        [DllImport(DllFileName, BestFitMapping = true, CharSet = CharSet.Ansi)]
        internal static extern int c2p_string(string value, ulong term);
        [DllImport(DllFileName, BestFitMapping = true, CharSet = CharSet.Ansi)]
        internal static extern int c2p_functor(string symbol, int arity, ulong term);
        [DllImport(DllFileName, BestFitMapping = true, CharSet = CharSet.Ansi)]
        internal static extern int c2p_functor_in_mod(string module, string symbol, int arity, ulong term);
        [DllImport(DllFileName)]
        internal static extern int c2p_list(ulong term);
        [DllImport(DllFileName)]
        internal static extern int c2p_nil(ulong term);



        [DllImport(DllFileName)]
        internal static extern IntPtr p2c_functor(ulong term);
        [DllImport(DllFileName)]
        internal static extern IntPtr p2c_string(ulong term);
        [DllImport(DllFileName)]
        internal static extern int p2c_arity(ulong term);
        [DllImport(DllFileName)]
        internal static extern int p2c_int(ulong term);
        [DllImport(DllFileName)]
        internal static extern double p2c_float(ulong term);



        [DllImport(DllFileName)]
        internal static extern ulong p2p_new();
        [DllImport(DllFileName)]
        internal static extern int p2p_unify(ulong term1, ulong term2);
        [DllImport(DllFileName)]
        internal static extern ulong p2p_arg(ulong term, int index);
        [DllImport(DllFileName)]
        internal static extern ulong p2p_car(ulong term);
        [DllImport(DllFileName)]
        internal static extern ulong p2p_cdr(ulong term);



        [DllImport(DllFileName)]
        internal static extern int is_var(ulong term);
        [DllImport(DllFileName)]
        internal static extern int is_int(ulong term);
        [DllImport(DllFileName)]
        internal static extern int is_float(ulong term);
        [DllImport(DllFileName)]
        internal static extern int is_string(ulong term);
        [DllImport(DllFileName)]
        internal static extern int is_atom(ulong term);
        [DllImport(DllFileName)]
        internal static extern int is_list(ulong term);
        [DllImport(DllFileName)]
        internal static extern int is_nil(ulong term);
        [DllImport(DllFileName)]
        internal static extern int is_functor(ulong term);
        [DllImport(DllFileName)]
        internal static extern int is_charlist(ulong term, out int length);
        [DllImport(DllFileName)]
        internal static extern int is_attv(ulong term);



        [DllImport(DllFileName, BestFitMapping = true, CharSet = CharSet.Ansi)]
        internal static extern int xsb_add_c_predicate(string modname, string predname, int arity, /*int (* cfun)()*/ Delegate cfun);
        [DllImport(DllFileName)]
        internal static extern int xsb_query_save(int numregs);
        [DllImport(DllFileName)]
        internal static extern int xsb_query_restore();



        [DllImport(DllFileName)]
        internal static extern int xsb_close();
    }
    #endregion
}