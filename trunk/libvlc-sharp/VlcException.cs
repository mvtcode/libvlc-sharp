/* vi:set ts=4 et sts=4 sw=4: */
/***************************************************************************
 *  VlcException.cs
 *  Original Filename: VlcException.cs
 *  
 *  Copyright (C) 2007 Timothy J Fontaine <tjfontaine@gmail.com>
 *  Written by Timothy J Fontaine <tjfontaine@gmail.com>
 *  Modified by Scott E Graves <scott.e.graves@gmail.com>
 ****************************************************************************/

/*  THIS FILE IS LICENSED UNDER THE MIT LICENSE AS OUTLINED IMMEDIATELY BELOW: 
 *
 *  Permission is hereby granted, free of charge, to any person obtaining a
 *  copy of this software and associated documentation files (the "Software"),  
 *  to deal in the Software without restriction, including without limitation  
 *  the rights to use, copy, modify, merge, publish, distribute, sublicense,  
 *  and/or sell copies of the Software, and to permit persons to whom the  
 *  Software is furnished to do so, subject to the following conditions:
 *
 *  The above copyright notice and this permission notice shall be included in 
 *  all copies or substantial portions of the Software.
 *
 *  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
 *  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
 *  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
 *  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
 *  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING 
 *  FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER 
 *  DEALINGS IN THE SOFTWARE.
 */

using System;
using System.Runtime.InteropServices;
using System.IO;

namespace Atx.LibVLC
{
    public class VlcExceptionHandle : SafeHandle
    {
        public VlcExceptionHandle()
            : base(IntPtr.Zero, true)
        {
            handle = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(int)) + Marshal.SizeOf(typeof(IntPtr)));
            libvlc_exception_init(this);
        }

        protected override bool ReleaseHandle()
        {
            if (!IsInvalid)
            {
                libvlc_exception_clear(this);
                Marshal.FreeHGlobal(handle);
                handle = IntPtr.Zero;
            }

            return true;
        }

        protected override void Dispose(bool disposing)
        {
            ReleaseHandle();
            base.Dispose(disposing);
        }

        public override bool IsInvalid
        {
            get { return handle == IntPtr.Zero; }
        }

        public void Clear()
        {
            libvlc_exception_clear(this);
        }

        #region libvlc api
        [DllImport("libvlc")]
        private static extern void libvlc_exception_init(VlcExceptionHandle p_exception);

        [DllImport("libvlc")]
        private static extern void libvlc_exception_clear(VlcExceptionHandle p_exception);
        #endregion
    }

    public class VlcException : Exception, IDisposable
    {
        private bool _disposed = false;
        private VlcExceptionHandle _excp = new VlcExceptionHandle();

        public VlcException() : base()
        {
        }

        ~VlcException()
        {
            Dispose(false);
        }
        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _disposed = true;
                
                if (!_excp.IsInvalid)
                    _excp.Dispose();
            }
        }

        public void Clear()
        {
            _excp.Clear();
        }

        public override string Message
        {
            get
            {
                IntPtr imessage = libvlc_exception_get_message(_excp);
                string message = Marshal.PtrToStringAnsi(imessage);
                return message;
            }
        }

        public override string ToString()
        {
            return Message;
        }

        public bool Raised
        {
            get
            {
                return libvlc_exception_raised(_excp) == 1 ? true : false;
            }
        }

        public static void HandleVlcException(ref VlcException ex)
        {
            if (ex.Raised)
                throw ex;
        }

        public static implicit operator VlcExceptionHandle (VlcException ex)
        {
            return ex._excp;
        }

        #region libvlc api
        [DllImport ("libvlc")]
        private static extern int libvlc_exception_raised(VlcExceptionHandle ex);

        [DllImport ("libvlc")]
        private static extern IntPtr libvlc_exception_get_message(VlcExceptionHandle ex);
        #endregion
    }
}

