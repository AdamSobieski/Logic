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

// The _LINUX part is from Batu Akan from Sweden. Thank you very much. (Linux Support with Mono)
// Uncomment the following line to compile on Linux or MacOS
// #define _LINUX

using Microsoft.Win32.SafeHandles;
using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;

namespace Logic.Prolog.Swi
{
    [StructLayout(LayoutKind.Explicit)]
    internal struct uintptr_t
    {
        [FieldOffset(0)]
#if _PL_X64
        private System.UInt64 _uintptr;
        internal uintptr_t(ulong l)
        {
            this._uintptr = l;
        }
        public static uintptr_t operator +(uintptr_t term1, ulong l)
        {
            return new uintptr_t(term1._uintptr + l);
        }
#else
        private UInt32 _uintptr;
        internal uintptr_t(uint i)
        {
            _uintptr = i;
        }
        public static uintptr_t operator +(uintptr_t term1, ulong l)
        {
            return new uintptr_t(term1._uintptr + (uint)l);
        }
#endif

        public override int GetHashCode()
        {
            return _uintptr.GetHashCode();
        }
        /// <inheritdoc />
        public override bool Equals(Object obj)
        {
            if (obj is uintptr_t)
                return this == ((uintptr_t)obj);
            if (obj is int)
                return this == ((int)obj);
            return false;
        }

        public static bool operator ==(uintptr_t term1, uintptr_t term2)
        {
            return term1._uintptr == term2._uintptr;
        }
        public static bool operator !=(uintptr_t term1, uintptr_t term2)
        {
            return term1._uintptr != term2._uintptr;
        }
        public static bool operator ==(uintptr_t term1, int term2)
        {
            return term1._uintptr == (ulong)term2;
        }
        public static bool operator ==(int term2, uintptr_t term1)
        {
            return term1 == term2;
        }
        public static bool operator !=(uintptr_t term1, int term2)
        {
            return term1._uintptr != (ulong)term2;
        }
        public static bool operator !=(int term2, uintptr_t term1)
        {
            return term1 != term2;
        }

        //---------------
        public static bool operator >(uintptr_t term1, ulong term2)
        {
            return term1._uintptr > term2;
        }
        public static bool operator <(uintptr_t term1, ulong term2)
        {
            return term1._uintptr < term2;
        }

        //---------------
        // assignment = operator
        public static implicit operator uintptr_t(long term2)
        {
            uintptr_t x;
#if _PL_X64
            x._uintptr = (ulong)term2;
#else
            x._uintptr = (uint)term2;
#endif
            return x;
        }

    }

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

        //        internal static IntPtr GetProcHandle(SafeLibraryHandle hModule, String procname)
        //        {
        //#if _LINUX
        //            return dlsym(hModule, procname);
        //#else
        //            return GetProcAddress(hModule, procname);		
        //#endif
        //        }


        internal static IntPtr GetPointerOfIoFunctions(SafeLibraryHandle hModule)
        {
#if _LINUX
            return dlsym(hModule, "S__iob");
#else
            return GetProcAddress(hModule, "S__iob");
#endif
        }


    }
    #endregion // Safe Handles and Native imports

    // for details see http://msdn2.microsoft.com/en-us/library/06686c8c-6ad3-42f7-a355-cbaefa347cfc(vs.80).aspx
    // and http://blogs.msdn.com/fxcop/archive/2007/01/14/faq-how-do-i-fix-a-violation-of-movepinvokestonativemethodsclass.aspx

    //NativeMethods - This class does not suppress stack walks for unmanaged code permission. 
    //    (System.Security.SuppressUnmanagedCodeSecurityAttribute must not be applied to this class.) 
    //    This class is for methods that can be used anywhere because a stack walk will be performed.

    //SafeNativeMethods - This class suppresses stack walks for unmanaged code permission. 
    //    (System.Security.SuppressUnmanagedCodeSecurityAttribute is applied to this class.) 
    //    This class is for methods that are safe for anyone to call. Callers of these methods are not 
    //    required to do a full security review to ensure that the usage is secure because the methods are harmless for any caller.

    //UnsafeNativeMethods - This class suppresses stack walks for unmanaged code permission. 
    //    (System.Security.SuppressUnmanagedCodeSecurityAttribute is applied to this class.) 
    //    This class is for methods that are potentially dangerous. Any caller of these methods must do a 
    //    full security review to ensure that the usage is secure because no stack walk will be performed.

    [SuppressUnmanagedCodeSecurity]
    internal static class SafeNativeMethods
    {
        private const string DllFileName = "libswipl.dll";

        public static string DllFileName1
        {
            get
            {
                if (Environment.OSVersion.Platform == PlatformID.Unix ||
                    Environment.OSVersion.Platform == PlatformID.MacOSX)
                {
                    return "libpl.so";
                }
                return DllFileName;
            }
        }

        //  1    0 0004956F PL_abort_hook
        //  2    1 000495E3 PL_abort_unhook
        //  3    2 000C240A PL_acquire_stream
        //  4    3 00049DDE PL_action
        //  5    4 00049816 PL_add_to_protocol
        //  6    5 000388F8 PL_agc_hook
        //  7    6 00002830 PL_atom_chars
        //  8    7 00038FFA PL_atom_generator
        //  9    8 0003907E PL_atom_generator_w
        // 10    9 000053F0 PL_atom_nchars
        // 11    A 00040E56 PL_atom_wchars
        // 12    B 00098A55 PL_backtrace
        // 13    C 000989E8 PL_backtrace_string
        // 14    D 00005240 PL_blob_data
        // 15    E 00049481 PL_call
        // 16    F 00003DA0 PL_call_predicate
        // 17   10 000C85D4 PL_changed_cwd
        // 18   11 0008E646 PL_chars_to_term
        // 19   12 00083955 PL_check_data
        // 20   13 00071403 PL_check_stacks
        // 21   14 000B056F PL_cleanup
        // 22   15 000A7ED1 PL_cleanup_fork
        // 23   16 00004BA0 PL_clear_exception
        // 24   17 00045F1D PL_close_foreign_frame
        // 25   18 00006010 PL_close_query
        // 26   19 000036D0 PL_compare
        // 27   1A 00003930 PL_cons_functor
        // 28   1B 00044641 PL_cons_functor_v
        // 29   1C 000468B6 PL_cons_list
        // 30   1D 00043A52 PL_context
        // 31   1E 00046242 PL_copy_term_ref
        // 32   1F 000A9515 PL_create_engine
        // 33   20 0002B0C0 PL_current_prolog_flag
        // 34   21 0004A130 PL_current_query
        // 35   22 00002FB0 PL_cut_query
        // 36   23 0004668A PL_cvt_encoding
        // 37   24 0004677F PL_cvt_i_address
        // 38   25 00046750 PL_cvt_i_atom
        // 39   26 00047C02 PL_cvt_i_char
        // 40   27 00046E10 PL_cvt_i_codes
        // 41   28 00046720 PL_cvt_i_float
        // 42   29 000466B3 PL_cvt_i_int
        // 43   2A 000466BD PL_cvt_i_int64
        // 44   2B 000466B8 PL_cvt_i_long
        // 45   2C 00047ABE PL_cvt_i_short
        // 46   2D 00046725 PL_cvt_i_single
        // 47   2E 000466F1 PL_cvt_i_size_t
        // 48   2F 00046E28 PL_cvt_i_string
        // 49   30 00047BF4 PL_cvt_i_uchar
        // 50   31 0004791A PL_cvt_i_uint
        // 51   32 000466C2 PL_cvt_i_uint64
        // 52   33 000479CC PL_cvt_i_ulong
        // 53   34 00047AB0 PL_cvt_i_ushort
        // 54   35 000467E8 PL_cvt_o_address
        // 55   36 000467B9 PL_cvt_o_atom
        // 56   37 00048957 PL_cvt_o_codes
        // 57   38 00048B42 PL_cvt_o_float
        // 58   39 00046784 PL_cvt_o_int64
        // 59   3A 00048B36 PL_cvt_o_single
        // 60   3B 0004896F PL_cvt_o_string
        // 61   3C 00046691 PL_cvt_set_encoding
        // 62   3D 000263B0 PL_cwd
        // 63   3E 0009865F PL_describe_context
        // 64   3F 000A9DE5 PL_destroy_engine
        // 65   40 00002550 PL_discard_foreign_frame
        // 66   41 00049831 PL_dispatch
        // 67   42 0004981B PL_dispatch_hook
        // 68   43 000DC2A7 PL_dlclose
        // 69   44 000DC267 PL_dlerror
        // 70   45 000DC186 PL_dlopen
        // 71   46 000DC26F PL_dlsym
        // 72   47 00060113 PL_domain_error
        // 73   48 0004988D PL_duplicate_record
        // 74   49 00005F50 PL_erase
        // 75   4A 00094F7E PL_erase_external
        // 76   4B 000050A0 PL_exception
        // 77   4C 00007110 PL_existence_error
        // 78   4D 000B0527 PL_exit_hook
        // 79   4E 00049D97 PL_fatal_error
        // 80   4F 00001ED0 PL_find_blob_type
        // 81   50 000494A1 PL_foreign_context
        // 82   51 000494A5 PL_foreign_context_address
        // 83   52 000494AD PL_foreign_context_predicate
        // 84   53 000494A9 PL_foreign_control
        // 85   54 00002210 PL_free
        // 86   55 000463EE PL_functor_arity
        // 87   56 000463B1 PL_functor_arity_sz
        // 88   57 00046384 PL_functor_name
        // 89   58 000474A2 PL_get_arg
        // 90   59 000560B0 PL_get_arg_sz
        // 91   5A 00046BD0 PL_get_atom
        // 92   5B 00004BF0 PL_get_atom_chars
        // 93   5C 0006056B PL_get_atom_ex
        // 94   5D 00046C33 PL_get_atom_nchars
        // 95   5E 000474C5 PL_get_attr
        // 96   5F 00005500 PL_get_blob
        // 97   60 000035B0 PL_get_bool
        // 98   61 00006D80 PL_get_bool_ex
        // 99   62 00060774 PL_get_char_ex
        //100   63 00004CE0 PL_get_chars
        //101   64 00047343 PL_get_compound_name_arity
        //102   65 0004722E PL_get_compound_name_arity_sz
        //103   66 00098567 PL_get_context
        //104   67 00024F60 PL_get_file_name
        //105   68 000C6D85 PL_get_file_nameW
        //106   69 00003A40 PL_get_float
        //107   6A 00006E40 PL_get_float_ex
        //108   6B 00047380 PL_get_functor
        //109   6C 0004752D PL_get_head
        //110   6D 000063F0 PL_get_int64
        //111   6E 00007150 PL_get_int64_ex
        //112   6F 00047008 PL_get_integer
        //113   70 00060920 PL_get_integer_ex
        //114   71 000056E0 PL_get_intptr
        //115   72 0006061E PL_get_intptr_ex
        //116   73 000474F4 PL_get_list
        //117   74 00046E03 PL_get_list_chars
        //118   75 00060A40 PL_get_list_ex
        //119   76 00046D52 PL_get_list_nchars
        //120   77 00047082 PL_get_long
        //121   78 0006059A PL_get_long_ex
        //122   79 000473AF PL_get_module
        //123   7A 000B1D9E PL_get_mpq
        //124   7B 000B1CDB PL_get_mpz
        //125   7C 000472E4 PL_get_name_arity
        //126   7D 000471F5 PL_get_name_arity_sz
        //127   7E 00004CF0 PL_get_nchars
        //128   7F 00055C20 PL_get_nil
        //129   80 00006D50 PL_get_nil_ex
        //130   81 000471A4 PL_get_pointer
        //131   82 000607B0 PL_get_pointer_ex
        //132   83 000960D1 PL_get_signum_ex
        //133   84 00060745 PL_get_size_ex
        //134   85 000BFD53 PL_get_stream
        //135   86 000C240F PL_get_stream_handle
        //136   87 00046CDC PL_get_string
        //137   88 000475B0 PL_get_tail
        //138   89 00046A1B PL_get_term_value
        //139   8A 000A7F95 PL_get_thread_alias
        //140   8B 000A7FF6 PL_get_thread_id_ex
        //141   8C 000058A0 PL_get_wchars
        //142   8D 00006390 PL_halt
        //143   8E 00018F00 PL_handle_signals
        //144   8F 0001D550 PL_initialise
        //145   90 0004965C PL_initialise_hook
        //146   91 0006007C PL_instantiation_error
        //147   92 00098EDF PL_interrupt
        //148   93 00012E30 PL_is_acyclic
        //149   94 00047808 PL_is_atom
        //150   95 00047ED2 PL_is_atomic
        //151   96 000478C9 PL_is_attvar
        //152   97 0004782F PL_is_blob
        //153   98 00047DA3 PL_is_callable
        //154   99 00047D51 PL_is_compound
        //155   9A 00047C13 PL_is_float
        //156   9B 00047DF4 PL_is_functor
        //157   9C 000110C0 PL_is_ground
        //158   9D 000B0437 PL_is_initialised
        //159   9E 00003310 PL_is_integer
        //160   9F 00004D80 PL_is_list
        //161   A0 00047F2F PL_is_number
        //162   A1 00047E63 PL_is_pair
        //163   A2 00047C65 PL_is_rational
        //164   A3 00047F85 PL_is_string
        //165   A4 000477B6 PL_is_variable
        //166   A5 00002950 PL_license
        //167   A6 00045C8D PL_linger
        //168   A7 0004A4DF PL_load_extensions
        //169   A8 00001F30 PL_malloc
        //170   A9 0004A610 PL_malloc_atomic
        //171   AA 000063D0 PL_malloc_atomic_uncollectable
        //172   AB 00002540 PL_malloc_atomic_unmanaged
        //173   AC 00002200 PL_malloc_uncollectable
        //174   AD 000063E0 PL_malloc_unmanaged
        //175   AE 00049321 PL_module_name
        //176   AF 000021B0 PL_new_atom
        //177   B0 000462BD PL_new_atom_mbchars
        //178   B1 00046269 PL_new_atom_nchars
        //179   B2 00046479 PL_new_atom_wchars
        //180   B3 00046341 PL_new_functor
        //181   B4 00055B50 PL_new_functor_sz
        //182   B5 00049325 PL_new_module
        //183   B6 000461C1 PL_new_nil_ref
        //184   B7 0004620D PL_new_term_ref
        //185   B8 00046167 PL_new_term_refs
        //186   B9 0004A620 PL_next_solution
        //187   BA 000B04DF PL_on_halt
        //188   BB 000461A2 PL_open_foreign_frame
        //189   BC 00002AD0 PL_open_query
        //190   BD 0004A4E1 PL_open_resource
        //191   BE 00060153 PL_permission_error
        //192   BF 00004BD0 PL_pred
        //193   C0 00002890 PL_predicate
        //194   C1 0004944E PL_predicate_info
        //195   C2 0008776A PL_prof_call
        //196   C3 00087799 PL_prof_exit
        //197   C4 0004978A PL_prompt_next
        //198   C5 000497B4 PL_prompt_string
        //199   C6 00048059 PL_put_atom
        //200   C7 00002A40 PL_put_atom_chars
        //201   C8 000480CA PL_put_atom_nchars
        //202   C9 00049077 PL_put_blob
        //203   CA 00048088 PL_put_bool
        //204   CB 000481BE PL_put_chars
        //205   CC 00048476 PL_put_float
        //206   CD 000484D9 PL_put_functor
        //207   CE 000483D2 PL_put_int64
        //208   CF 00048401 PL_put_integer
        //209   D0 000485E1 PL_put_list
        //210   D1 000488FC PL_put_list_chars
        //211   D2 000483AB PL_put_list_codes
        //212   D3 0004882D PL_put_list_nchars
        //213   D4 000482D9 PL_put_list_ncodes
        //214   D5 00002350 PL_put_nil
        //215   D6 0004842F PL_put_pointer
        //216   D7 00048116 PL_put_string_chars
        //217   D8 00048171 PL_put_string_nchars
        //218   D9 00048650 PL_put_term
        //219   DA 0008E3D4 PL_put_term_from_chars
        //220   DB 0004802B PL_put_variable
        //221   DC 00049FBF PL_query
        //222   DD 00046EEF PL_quote
        //223   DE 00006300 PL_raise
        //224   DF 00005B50 PL_raise_exception
        //225   E0 00005390 PL_realloc
        //226   E1 00002500 PL_record
        //227   E2 0009422C PL_record_external
        //228   E3 00005A90 PL_recorded
        //229   E4 000945BC PL_recorded_external
        //230   E5 00039940 PL_register_atom
        //231   E6 00001450 PL_register_blob_type
        //232   E7 0004A4D8 PL_register_extensions
        //233   E8 0004A4C1 PL_register_extensions_in_module
        //234   E9 0004A42E PL_register_foreign
        //235   EA 0004A45B PL_register_foreign_in_module
        //236   EB 000876FB PL_register_profile_type
        //237   EC 00024540 PL_release_stream
        //238   ED 000C23EE PL_release_stream_noerror
        //239   EE 000600B6 PL_representation_error
        //240   EF 00045F9B PL_reset_term_refs
        //241   F0 000601B3 PL_resource_error
        //242   F1 00003AD0 PL_rewind_foreign_frame
        //243   F2 0004682F PL_same_compound
        //244   F3 000A88AF PL_set_engine
        //245   F4 00001F50 PL_set_prolog_flag
        //246   F5 000B0471 PL_set_resource_db_mem
        //247   F6 00018870 PL_sigaction
        //248   F7 00018800 PL_signal
        //249   F8 00005290 PL_skip_list
        //250   F9 000985DB PL_step_context
        //251   FA 000492E2 PL_strip_module
        //252   FB 0006020F PL_syntax_error
        //253   FC 000490D1 PL_term_type
        //254   FD 000A845F PL_thread_at_exit
        //255   FE 000A9336 PL_thread_attach_engine
        //256   FF 000A9CBB PL_thread_destroy_engine
        //257  100 000A5571 PL_thread_raise
        //258  101 0001BA40 PL_thread_self
        //259  102 00049529 PL_throw
        //260  103 00006360 PL_toplevel
        //261  104 00049746 PL_ttymode
        //262  105 000600EE PL_type_error
        //263  106 000491F1 PL_unify
        //264  107 00048B4E PL_unify_arg
        //265  108 000578A0 PL_unify_arg_sz
        //266  109 000487CF PL_unify_atom
        //267  10A 00004C80 PL_unify_atom_chars
        //268  10B 000415F0 PL_unify_atom_nchars
        //269  10C 000051D0 PL_unify_blob
        //270  10D 000056A0 PL_unify_bool
        //271  10E 00006DB0 PL_unify_bool_ex
        //272  10F 000026A0 PL_unify_chars
        //273  110 00057BF0 PL_unify_compound
        //274  111 000050F0 PL_unify_float
        //275  112 000487FE PL_unify_functor
        //276  113 00048AC7 PL_unify_int64
        //277  114 00048A95 PL_unify_integer
        //278  115 00048B71 PL_unify_list
        //279  116 00048CD4 PL_unify_list_chars
        //280  117 00048E18 PL_unify_list_codes
        //281  118 00060990 PL_unify_list_ex
        //282  119 00048BAA PL_unify_list_nchars
        //283  11A 00048CF6 PL_unify_list_ncodes
        //284  11B 000B1EE3 PL_unify_mpq
        //285  11C 000B1E6E PL_unify_mpz
        //286  11D 00055EB0 PL_unify_nil
        //287  11E 00060960 PL_unify_nil_ex
        //288  11F 00048AEC PL_unify_pointer
        //289  120 000C3282 PL_unify_stream
        //290  121 00041591 PL_unify_string_chars
        //291  122 00047FD7 PL_unify_string_nchars
        //292  123 0004989E PL_unify_term
        //293  124 000A8394 PL_unify_thread_id
        //294  125 00048995 PL_unify_uint64
        //295  126 00005AD0 PL_unify_wchars
        //296  127 00046551 PL_unify_wchars_diff
        //297  128 0006008E PL_uninstantiation_error
        //298  129 000399B0 PL_unregister_atom
        //299  12A 00038B56 PL_unregister_blob_type
        //300  12B 000465CE PL_utf8_strlen
        //301  12C 0002B510 PL_w32_running_under_wine
        //302  12D 0002BB10 PL_w32_wrap_ansi_console
        //303  12E 000A8026 PL_w32thread_raise
        //304  12F 000DB9D6 PL_wait_for_console_input
        //305  130 00044FC9 PL_warning
        //306  131 0008E657 PL_wchars_to_term
        //307  132 000DC15A PL_win_message_proc
        //308  133 000C58BC PL_write_prompt
        //309  134 000A45F7 PL_write_term
        //310  135 0004A166 PL_yielded
        //311  136 0004666C SP_get_state
        //312  137 0004664A SP_set_state
        //313  138 000271E0 S__fillbuf
        //314  139 00027460 S__fupdatefilepos_getc
        //315  13A 000276E0 S__getiob
        //316  13B 000E4DE0 S__iob
        //317  13C 00027D20 Scanrepresent
        //318  13D 00027710 ScheckBOM
        //319  13E 000CBA3D Scleanup
        //320  13F 00027E50 Sclearerr
        //321  140 000274E0 Sclose
        //322  141 000276A0 Sclosehook
        //323  142 000C9AAF Sdprintf
        //324  143 000CAB1C Sfdopen
        //325  144 000CAE11 Sfeof
        //326  145 000276F0 Sferror
        //327  146 000CB0D6 Sfgetc
        //328  147 00027150 Sfgets
        //329  148 000E5100 Sfilefunctions
        //330  149 000270F0 Sfileno
        //331  14A 000279D0 Sflush
        //332  14B 000279A0 Sfpasteof
        //333  14C 000CB173 Sfprintf
        //334  14D 000CB1CB Sfputs
        //335  14E 000CAF4F Sfread
        //336  14F 000CB9E4 Sfree
        //337  150 000CB27F Sfwrite
        //338  151 000CBAF0 Sgetcode
        //339  152 000CADC2 Sgets
        //340  153 000CB05E Sgetw
        //341  154 00026740 SinitStreams
        //342  155 000E50E0 Slinesize
        //343  156 000277A0 Slock
        //344  157 00026FA0 Snew
        //345  158 00026F20 Sopen_file
        //346  159 000CB94F Sopen_pipe
        //347  15A 00027A20 Sopen_string
        //348  15B 00027B00 Sopenmem
        //349  15C 00027830 Speekcode
        //350  15D 000CA906 Spending
        //351  15E 000CB142 Sprintf
        //352  15F 000CB233 Sputc
        //353  160 00026D40 Sputcode
        //354  161 000CB195 Sputs
        //355  162 000CB2CF Sputw
        //356  163 000CAE61 Sread_pending
        //357  164 000CB9E9 Sreset
        //358  165 000CAD1C Sseek
        //359  166 000CABB8 Sseek64
        //360  167 00027F20 Sset_exception
        //361  168 000CB308 Sset_filter
        //362  169 000CA892 Sset_timeout
        //363  16A 000CB40C Ssetbuffer
        //364  16B 000CA94F Ssetenc
        //365  16C 00027EA0 Sseterr
        //366  16D 000CA9C7 Ssetlocale
        //367  16E 000CAD24 Ssize
        //368  16F 00026820 Ssnprintf
        //369  170 00027D60 Ssprintf
        //370  171 000CAAE4 Stell
        //371  172 000CAA5D Stell64
        //372  173 00027E20 StryLock
        //373  174 000CA8E3 Sungetc
        //374  175 000CAA1F Sunit_size
        //375  176 00027930 Sunlock
        //376  177 000C9AD6 Svdprintf
        //377  178 000268D0 Svfprintf
        //378  179 000CB130 Svprintf
        //379  17A 00026840 Svsnprintf
        //380  17B 00027D90 Svsprintf
        //381  17C 000CAB6E Swinsock
        //382  17D 000CB1FD SwriteBOM
        //383  17E 000B67E7 _PL_atoms
        //384  17F 00047481 _PL_get_arg
        //385  180 0004741D _PL_get_arg_sz
        //386  181 00048FFC _PL_get_atomic
        //387  182 00047633 _PL_get_xpce_reference
        //388  183 0004904D _PL_put_atomic
        //389  184 00048767 _PL_put_xpce_reference_a
        //390  185 0004868E _PL_put_xpce_reference_i
        //391  186 00049498 _PL_retry
        //392  187 00049DBA _PL_retry_address
        //393  188 000C32A7 _PL_streams
        //394  189 00049048 _PL_unify_atomic
        //395  18A 00048E3A _PL_unify_xpce_reference

        /////////////////////////////
        /// libpl
        ///
        // das funktioniert NICHT wenn umlaute e.g. ü im pfad sind.
        // TODO wchar
        [DllImport(DllFileName, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        internal static extern int PL_initialise(int argc, string[] argv);
        [DllImport(DllFileName)]
        internal static extern int PL_is_initialised(IntPtr argc, IntPtr argv);
        [DllImport(DllFileName)]
        internal static extern int PL_halt(int i);

        // The function returns TRUE if successful and FALSE otherwise. Currently, FALSE is returned when an attempt is 
        // made to call PL_cleanup() recursively or if PL_cleanup() is not called from the main-thread. 
        // int PL_cleanup(int status)
        [DllImport(DllFileName)]
        internal static extern int PL_cleanup(int status);


        // PL_EXPORT(int)		PL_register_foreign_in_module(const char *module, const char *name, int arity, pl_function_t func, int flags);
        // typedef unsigned long	foreign_t
        // int PL_register_foreign_in_module(const char *module, const char *name, int arity, foreign_t (*function)(), int flags)
        // TODO wchar
        [DllImport(DllFileName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        internal static extern int PL_register_foreign_in_module(string module, string name, int arity, Delegate function, int flags);

        [DllImport(DllFileName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        internal static extern int PL_foreign_control(IntPtr control_t);

        [DllImport(DllFileName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        internal static extern IntPtr PL_foreign_context_address(IntPtr control_t);

        [DllImport(DllFileName, CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        internal static extern IntPtr _PL_retry_address(IntPtr context);

        //	 ENGINES (MT-ONLY)
        // TYPES :  PL_engine_t			-> void *
        //			PL_thread_attr_t	-> struct
        [DllImport(DllFileName)]
        // PL_EXPORT(PL_engine_t)	PL_create_engine(PL_thread_attr_t *attributes);
        internal static extern IntPtr PL_create_engine(IntPtr attr);
        [DllImport(DllFileName)]    // PL_EXPORT(int)		PlSetEngine(PL_engine_t engine, PL_engine_t *old);
        internal static extern int PL_set_engine(IntPtr engine, [In, Out] ref IntPtr old);
        [DllImport(DllFileName)]    // PL_EXPORT(int)		PL_destroy_engine(PL_engine_t engine);
        internal static extern int PL_destroy_engine(IntPtr engine);

        // atom_t PL_new_atom_nchars(size_t len, const char *s)
        [DllImport(DllFileName, CharSet = CharSet.Unicode, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        internal static extern uintptr_t PL_new_atom_wchars(int len, string text);

        // pl_wchar_t* PL_atom_wchars(atom_t atom, int *len)
        [DllImport(DllFileName, CharSet = CharSet.Unicode, BestFitMapping = false, ThrowOnUnmappableChar = true)] // return const char *
        internal static extern IntPtr PL_atom_wchars(uintptr_t atom, [In, Out] ref int len);

        [DllImport(DllFileName)]
        internal static extern int PL_unregister_atom(uintptr_t atom);

        // Pl_Query
        [DllImport(DllFileName)]
        internal static extern int PL_query(uint plQuerySwitch);

        // PlFrame
        [DllImport(DllFileName)]
        internal static extern uintptr_t PL_open_foreign_frame();
        [DllImport(DllFileName)]
        internal static extern void PL_close_foreign_frame(uintptr_t fid_t);
        [DllImport(DllFileName)]
        internal static extern void PL_rewind_foreign_frame(uintptr_t fid_t);
        // record recorded erase
        [DllImport(DllFileName)]
        internal static extern uintptr_t PL_record(uintptr_t term_t);
        [DllImport(DllFileName)]
        internal static extern void PL_recorded(uintptr_t record_t, uintptr_t term_t);
        [DllImport(DllFileName)]
        internal static extern void PL_erase(uintptr_t record_t);
        // PlQuery
        [DllImport(DllFileName)]
        internal static extern int PL_next_solution(uintptr_t qid_t);
        // TODO wchar
        [DllImport(DllFileName, CharSet = CharSet.Ansi, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        internal static extern IntPtr PL_predicate(string name, int arity, string module);

        [DllImport(DllFileName)]
        internal static extern IntPtr PL_new_module(uintptr_t atom_t);
        [DllImport(DllFileName)]
        //qid_t PL_open_query(module_t m, int flags, predicate_t pred, term_t t0);
        internal static extern uintptr_t PL_open_query(IntPtr module, int flags, IntPtr pred, uintptr_t term);
        [DllImport(DllFileName)]
        internal static extern void PL_cut_query(uintptr_t qid);
        [DllImport(DllFileName)]
        internal static extern void PL_close_query(uintptr_t qid);

        // PlTerm
        //__pl_export term_t	PL_new_term_ref(void);
        [DllImport(DllFileName)] // return term_t
        internal static extern uintptr_t PL_new_term_ref();

        //__pl_export void	PL_put_integer(term_t term, long i);
        [DllImport(DllFileName)]
        internal static extern int PL_put_integer(uintptr_t term, int i);
        [DllImport(DllFileName)]
        internal static extern int PL_put_float(uintptr_t term, double i);
        [DllImport(DllFileName)]
        internal static extern int PL_put_nil(uintptr_t term);

        [DllImport(DllFileName, CharSet = CharSet.Unicode, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        internal static extern int PL_get_wchars(uintptr_t term, [In, Out]ref int len, [In, Out]ref IntPtr s, uint flags);



        // __pl_export int		PL_get_long(term_t term, long *i);
        [DllImport(DllFileName)]
        internal static extern int PL_get_long(uintptr_t term, [In, Out] ref int i);
        // __pl_export int		PL_get_float(term_t term, double *f);
        [DllImport(DllFileName)]
        internal static extern int PL_get_float(uintptr_t term, [In, Out] ref double i);
        //__pl_export int		PL_term_type(term_t term);
        [DllImport(DllFileName)]
        internal static extern int PL_term_type(uintptr_t t);

        // COMPARE
        //__pl_export int		PL_compare(term_t t1, term_t t2);
        [DllImport(DllFileName)]
        internal static extern int PL_compare(uintptr_t term1, uintptr_t term2);



        // PlTermV
        [DllImport(DllFileName)] // return term_t
        internal static extern uintptr_t PL_new_term_refs(int n);
        //__pl_export void	PL_put_term(term_t t1, term_t t2);
        [DllImport(DllFileName)]
        internal static extern void PL_put_term(uintptr_t t1, uintptr_t t2);

        // PlCompound
        // __pl_export int PL_wchars_to_term(const pl_wchar_t *chars, term_t term);
        // __pl_export int PL_chars_to_term(const char *chars, term_t term);
        //__pl_export void	PL_cons_functor_v(term_t h, functor_t fd, term_t A0);
        //__pl_export functor_t	PL_new_functor(atom_t f, int atom);

        [DllImport(DllFileName, CharSet = CharSet.Unicode, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        internal static extern int PL_wchars_to_term([In]string chars, uintptr_t term);
        [DllImport(DllFileName)]
        internal static extern void PL_cons_functor_v(uintptr_t term, uintptr_t functor_t, uintptr_t termA0);
        [DllImport(DllFileName)]
        internal static extern uintptr_t PL_new_functor(uintptr_t atom, int a);

        // Testing the type of a term
        //__pl_export int		PL_is_variable(term_t term);
        //__pl_export int		PL_is_list(term_t term);
        // ...
        [DllImport(DllFileName)]
        internal static extern int PL_is_variable(uintptr_t term_t);
        [DllImport(DllFileName)]
        internal static extern int PL_is_ground(uintptr_t term_t);
        [DllImport(DllFileName)]
        internal static extern int PL_is_atom(uintptr_t term_t);
        [DllImport(DllFileName)]
        internal static extern int PL_is_string(uintptr_t term_t);
        [DllImport(DllFileName)]
        internal static extern int PL_is_integer(uintptr_t term_t);
        [DllImport(DllFileName)]
        internal static extern int PL_is_float(uintptr_t term_t);
        [DllImport(DllFileName)]
        internal static extern int PL_is_compound(uintptr_t term_t);
        [DllImport(DllFileName)]
        internal static extern int PL_is_list(uintptr_t term_t);
        [DllImport(DllFileName)]
        internal static extern int PL_is_atomic(uintptr_t term_t);
        [DllImport(DllFileName)]
        internal static extern int PL_is_number(uintptr_t term_t);

        // LISTS (PlTail)
        //__pl_export term_t	PL_copy_term_ref(term_t from);
        //__pl_export int		PL_unify_list(term_t l, term_t h, term_t term);
        //__pl_export int		PL_unify_nil(term_t l);
        //__pl_export int		PL_get_list(term_t l, term_t h, term_t term);
        //__pl_export int		PL_get_nil(term_t l);
        // __pl_export int		PL_unify(term_t t1, term_t t2);
        [DllImport(DllFileName)]
        internal static extern uintptr_t PL_copy_term_ref(uintptr_t term_t);
        [DllImport(DllFileName)]
        internal static extern int PL_unify_list(uintptr_t termL, uintptr_t termH, uintptr_t termT);
        [DllImport(DllFileName)]
        internal static extern int PL_unify_nil(uintptr_t term_t);
        [DllImport(DllFileName)]
        internal static extern int PL_get_list(uintptr_t termL, uintptr_t termH, uintptr_t termT);
        [DllImport(DllFileName)]
        internal static extern int PL_get_nil(uintptr_t term_t);
        [DllImport(DllFileName)]
        internal static extern int PL_unify(uintptr_t t1, uintptr_t t2);
        //__pl_export int PL_unify_wchars(term_t t, int type, size_t len, const pl_wchar_t *s)
        [DllImport(DllFileName, CharSet = CharSet.Unicode, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        internal static extern int PL_unify_wchars(uintptr_t t1, int type, int len, string atom);

        //[DllImport(DllFileName)]
        //internal static extern int PL_unify_float(uintptr_t term_t, double value);
        //[DllImport(DllFileName)]
        //internal static extern int PL_unify_integer(uintptr_t term_t, int value);

        // Exceptions
        // Handling exceptions
        //__pl_export term_t	PL_exception(qid_t _qid);
        //__pl_export int		PL_raise_exception(term_t exception);
        //__pl_export int		PL_throw(term_t exception);
        [DllImport(DllFileName)]
        internal static extern uintptr_t PL_exception(uintptr_t qid);
        [DllImport(DllFileName)]
        internal static extern int PL_raise_exception(uintptr_t exceptionTerm);
        //__pl_export int		PL_get_arg(int index, term_t term, term_t atom);
        [DllImport(DllFileName)]
        internal static extern int PL_get_arg(int index, uintptr_t t, uintptr_t a);
        //__pl_export int		PL_get_name_arity(term_t term, atom_t *Name, int *Arity);
        [DllImport(DllFileName)]
        internal static extern int PL_get_name_arity(uintptr_t t, ref uintptr_t name, ref int arity);

        // ******************************
        // *	  PROLOG THREADS		*
        // ******************************

        // from file pl-itf.h
        /*
		typedef struct
				{
					unsigned long	    local_size;		// Stack sizes
					unsigned long	    global_size;
					unsigned long	    trail_size;
					unsigned long	    argument_size;
					char *	    alias;					// alias Name
				} PL_thread_attr_t;
		*/
        //PL_EXPORT(int)	PL_thread_self(void);	/* Prolog thread id (-1 if none) */
        //PL_EXPORT(int)	PL_thread_attach_engine(PL_thread_attr_t *attr);
        //PL_EXPORT(int)	PL_thread_destroy_engine(void);
        //PL_EXPORT(int)	PL_thread_at_exit(void (*function)(void *), void *closure, int global);
        [DllImport(DllFileName)]
        internal static extern int PL_thread_self();
        [DllImport(DllFileName)]
        internal static extern int PL_thread_attach_engine(IntPtr attr);
        //internal static extern int PL_thread_attach_engine(ref PL_thread_attr_t attr);
        [DllImport(DllFileName)]
        internal static extern int PL_thread_destroy_engine();



        // ******************************
        // *	  PROLOG STREAM's		*
        // ******************************


        #region structurs

        // int Slinesize

        // IOFUNCTIONS  Sfilefunctions



        /*
         * long ssize_t
         * 
        typedef ssize_t (*Sread_function)(void *handle, char *buf, size_t bufsize);
        typedef ssize_t (*Swrite_function)(void *handle, char*buf, size_t bufsize);
        typedef long  (*Sseek_function)(void *handle, long pos, int whence);
        typedef int64_t (*Sseek64_function)(void *handle, int64_t pos, int whence);
        typedef int   (*Sclose_function)(void *handle);
        typedef int   (*Scontrol_function)(void *handle, int action, void *arg);


        typedef struct io_functions
        { Sread_function	read;		//* fill the buffer
          Swrite_function	write;		//* empty the buffer 
          Sseek_function	seek;		//* seek to position 
          Sclose_function	close;		//* close stream 
          Scontrol_function	control;	//* Info/control 
          Sseek64_function	seek64;		//* seek to position (intptr_t files) 
        } IOFUNCTIONS;
        */


        // IOSTREAM    S__iob[3]
        [StructLayout(LayoutKind.Sequential, Pack = 8)]
        internal struct MIOSTREAM
        {
            /*
            char		    *bufp;		    // `here'
            char		    *limitp;		    // read/write limit 
            char		    *buffer;		    // the buffer 
            char		    *unbuffer;	    // Sungetc buffer 
            int			    lastc;		    // last character written 
            int			    magic;		    // magic number SIO_MAGIC 
            int  			bufsize;	    // size of the buffer 
            int			    flags;		    // Status flags 
            IOPOS			posbuf;		    // location in file 
            IOPOS *		    position;	    // pointer to above 
            IntPtr	        *handle;		    // function's handle 
            MIOFUNCTIONS	*functions;	    // open/close/read/write/seek 
            int		        locks;		    // lock/unlock count 
            */
            //IOLOCK *		    mutex;		    // stream mutex 
            readonly IntPtr mutex;

            readonly long[] place_holder_1;
            // SWI-Prolog 4.0.7 
            //void			    (*close_hook)(void* closure);
            //void *		    closure;
            //                  // SWI-Prolog 5.1.3 
            //int			    timeout;	    // timeout (milliseconds) 
            //                  // SWI-Prolog 5.4.4 
            //char *		    message;	    // error/warning message 
            //IOENC			    encoding;	    // character encoding used 
            //struct io_stream *	tee;		// copy data to this stream 
            //mbstate_t *		mbstate;	    // ENC_ANSI decoding 
            //struct io_stream *	upstream;	// stream providing our input 
            //struct io_stream *	downstream;	// stream providing our output 
            //unsigned		    newline : 2;	// Newline mode 
            //void *		    exception;	    // pending exception (record_t) 
            //intptr_t		    reserved[2];	// reserved for extension 
        };

        /*

         * 
typedef struct io_position
{ int64_t		byteno;		// byte-position in file 
  int64_t		charno;		// character position in file 
  int			lineno;		// lineno in file 
  int			linepos;	// position in line 
  intptr_t		reserved[2];	// future extensions 
} IOPOS;

         * 
typedef struct io_stream{ 
  char		       *bufp;		    // `here'
  char		       *limitp;		    // read/write limit 
  char		       *buffer;		    // the buffer 
  char		       *unbuffer;	    // Sungetc buffer 
  int			    lastc;		    // last character written 
  int			    magic;		    // magic number SIO_MAGIC 
  int  			    bufsize;	    // size of the buffer 
  int			    flags;		    // Status flags 
  IOPOS			    posbuf;		    // location in file 
  IOPOS *		    position;	    // pointer to above 
  void		       *handle;		    // function's handle 
  IOFUNCTIONS	   *functions;	    // open/close/read/write/seek 
  int		        locks;		    // lock/unlock count 
  IOLOCK *		    mutex;		    // stream mutex 
					// SWI-Prolog 4.0.7 
  void			    (*close_hook)(void* closure);
  void *		    closure;
					// SWI-Prolog 5.1.3 
  int			    timeout;	    // timeout (milliseconds) 
					// SWI-Prolog 5.4.4 
  char *		    message;	    // error/warning message 
  IOENC			    encoding;	    // character encoding used 
  struct io_stream *	tee;		// copy data to this stream 
  mbstate_t *		mbstate;	    // ENC_ANSI decoding 
  struct io_stream *	upstream;	// stream providing our input 
  struct io_stream *	downstream;	// stream providing our output 
  unsigned		    newline : 2;	// Newline mode 
  void *		    exception;	    // pending exception (record_t) 
  intptr_t		    reserved[2];	// reserved for extension 
} IOSTREAM;

         */

        #endregion structurs



    } // class SafeNativeMethods
}
