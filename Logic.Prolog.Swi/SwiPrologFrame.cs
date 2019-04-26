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

namespace Logic.Prolog.Swi
{
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
    public class SwiPrologFrame : IDisposable
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
        public SwiPrologFrame()
        {
            _fid = libswipl.PL_open_foreign_frame();
        }

        /// <summary>
        /// Reclaims all term-references created after constructing the instance.
        /// </summary>
        ~SwiPrologFrame()
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
