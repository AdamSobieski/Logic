/*********************************************************
* 
*  Authors:        Adam Sobieski
*
*********************************************************/

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Logic.Prolog.Xsb
{
    internal static class xsb
    {
        static SafeLibraryHandle _hLibrary;

        private static bool IsValid
        {
            get { return _hLibrary != null && !_hLibrary.IsInvalid; }
        }

        private static void LoadUnmanagedLibrary(string fileName)
        {
            if (_hLibrary == null)
            {
                _hLibrary = NativeMethods.LoadDll(fileName);
                if (_hLibrary.IsInvalid)
                {
                    int hr = Marshal.GetHRForLastWin32Error();
                    Marshal.ThrowExceptionForHR(hr);
                }
            }
        }

        private static void UnLoadUnmanagedLibrary()
        {
            if (!_hLibrary.IsClosed)
            {
                _hLibrary.Close();
                do
                {
                    // be sure to unload swipl.sll
                } while (_hLibrary.UnLoad());
                // m_hLibrary.UnLoad();
                _hLibrary.Dispose();
                _hLibrary = null;
            }
        }

        internal static void LoadXsb()
        {
            LoadUnmanagedLibrary(SafeNativeMethods.DllFileName1);
        }

        internal static int xsb_init(int argc, string[] argv)
        {
            LoadXsb();
            return SafeNativeMethods.xsb_init(argc, argv);
        }
        internal static string xsb_get_init_error_type()
        {
            IntPtr ptr = SafeNativeMethods.xsb_get_init_error_type();
            return Marshal.PtrToStringAnsi(ptr);
        }
        internal static string xsb_get_init_error_message()
        {
            IntPtr ptr = SafeNativeMethods.xsb_get_init_error_message();
            return Marshal.PtrToStringAnsi(ptr);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int xsb_command()
        {
            return SafeNativeMethods.xsb_command();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int xsb_command_string(string command)
        {
            return SafeNativeMethods.xsb_command_string(command);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int xsb_query()
        {
            return SafeNativeMethods.xsb_query();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int xsb_query_string(string query)
        {
            return SafeNativeMethods.xsb_query_string(query);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int xsb_next()
        {
            return SafeNativeMethods.xsb_next();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int xsb_close_query()
        {
            return SafeNativeMethods.xsb_close_query();
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static ulong reg_term(int register)
        {
            return SafeNativeMethods.reg_term(register);
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int c2p_int(int value, ulong term)
        {
            return SafeNativeMethods.c2p_int(value, term);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int c2p_float(double value, ulong term)
        {
            return SafeNativeMethods.c2p_float(value, term);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int c2p_string(string value, ulong term)
        {
            return SafeNativeMethods.c2p_string(value, term);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int c2p_functor(string symbol, int arity, ulong term)
        {
            return SafeNativeMethods.c2p_functor(symbol, arity, term);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int c2p_functor_in_mod(string module, string symbol, int arity, ulong term)
        {
            return SafeNativeMethods.c2p_functor_in_mod(module, symbol, arity, term);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int c2p_list(ulong term)
        {
            return SafeNativeMethods.c2p_list(term);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int c2p_nil(ulong term)
        {
            return SafeNativeMethods.c2p_nil(term);
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string p2c_functor(ulong term)
        {
            return Marshal.PtrToStringAnsi(SafeNativeMethods.p2c_functor(term));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static string p2c_string(ulong term)
        {
            return Marshal.PtrToStringAnsi(SafeNativeMethods.p2c_string(term));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int p2c_arity(ulong term)
        {
            return SafeNativeMethods.p2c_arity(term);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int p2c_int(ulong term)
        {
            return SafeNativeMethods.p2c_int(term);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static double p2c_float(ulong term)
        {
            return SafeNativeMethods.p2c_float(term);
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static ulong p2p_new()
        {
            return SafeNativeMethods.p2p_new();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int p2p_unify(ulong term1, ulong term2)
        {
            return SafeNativeMethods.p2p_unify(term1, term2);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static ulong p2p_arg(ulong term, int index)
        {
            return SafeNativeMethods.p2p_arg(term, index);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static ulong p2p_car(ulong term)
        {
            return SafeNativeMethods.p2p_car(term);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static ulong p2p_cdr(ulong term)
        {
            return SafeNativeMethods.p2p_cdr(term);
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int is_var(ulong term)
        {
            return SafeNativeMethods.is_var(term);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int is_int(ulong term)
        {
            return SafeNativeMethods.is_int(term);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int is_float(ulong term)
        {
            return SafeNativeMethods.is_float(term);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int is_string(ulong term)
        {
            return SafeNativeMethods.is_string(term);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int is_atom(ulong term)
        {
            return SafeNativeMethods.is_atom(term);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int is_list(ulong term)
        {
            return SafeNativeMethods.is_list(term);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int is_nil(ulong term)
        {
            return SafeNativeMethods.is_nil(term);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int is_functor(ulong term)
        {
            return SafeNativeMethods.is_functor(term);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int is_charlist(ulong term, out int length)
        {
            return SafeNativeMethods.is_charlist(term, out length);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int is_attv(ulong term)
        {
            return SafeNativeMethods.is_attv(term);
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int xsb_add_c_predicate(string modname, string predname, int arity, /*int (* cfun)()*/ Delegate cfun)
        {
            return SafeNativeMethods.xsb_add_c_predicate(modname, predname, arity, cfun);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int xsb_query_save(int numregs)
        {
            return SafeNativeMethods.xsb_query_save(numregs);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int xsb_query_restore()
        {
            return SafeNativeMethods.xsb_query_restore();
        }



        public static int xsb_close()
        {
            int iRet = 0;
            if (IsValid)
            {
                iRet = SafeNativeMethods.xsb_close();
                UnLoadUnmanagedLibrary();
            }
            return iRet;
        }
    }
}
