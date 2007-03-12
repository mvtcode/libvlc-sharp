/* vi:set ts=4 et sts=4 sw=4: */
/***************************************************************************
 *  VlcLogEnum.cs
 *
 *  Copyright (C) 2007 Timothy J Fontaine <tjfontaine@gmail.com>
 *  Written by Timothy J Fontaine <tjfontaine@gmail.com>
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
using System.Collections;
using System.Runtime.InteropServices;

namespace Atx.LibVLC
{
    public class VlcLogEnumException : Exception
    {
        public VlcLogEnumException(string m) : base(m) {}
    }

    public class VlcLogEnumHandle : SafeHandle
    {
        public VlcLogEnumHandle() : base(IntPtr.Zero, true)
        { }

        public override bool IsInvalid
        {
            get { return handle == IntPtr.Zero; }
        }

        protected override bool ReleaseHandle()
        {
            if(!IsInvalid)
            {
                VlcException ex = new VlcException();
                libvlc_log_iterator_free(this, ex);
                if(!ex.Raised)
                {
                    handle = IntPtr.Zero;
                    return true;
                }
                else
                {
                    Console.WriteLine("Failed to Release VlcLogEnum Handle: " + ex.Message);
                    ex.Clear();
                    return false;
                }
            }

            return true;
        }

        protected override void Dispose(bool disposing)
        {
            ReleaseHandle();
            base.Dispose(disposing);
        }
        
        [DllImport("libvlc")]
        private static extern void libvlc_log_iterator_free(VlcLogEnumHandle iter, VlcExceptionHandle ex);
    }

    public class VlcLogEnum : IEnumerator
    {
        private VlcLogHandle log;
        private VlcLogEnumHandle iter;
        private VlcLogMessageHandle last_ptr;

        public VlcLogEnum(VlcLogHandle log)
        {
            this.log = log;
            get_iter();
        }

        private void get_iter() { VlcException ex = new VlcException(); get_iter(ex); }

        private void get_iter(VlcException ex)
        {
            iter = libvlc_log_get_iterator(log, ex);
            handle_exception(ex);
        }

        private void free(VlcException ex)
        {
            if(!iter.IsInvalid)
            {
                libvlc_log_iterator_free(iter, ex);
                handle_exception(ex);
            }
        }

        private bool has_next(VlcException ex)
        {
            int ret = libvlc_log_iterator_has_next(iter, ex);
            handle_exception(ex);
            return ret == 1 ? true : false;
        }

        private void handle_exception(VlcException ex)
        {
            if(ex.Raised)
            {
                VlcLogEnumException vlee = new VlcLogEnumException(ex.Message);
                Console.WriteLine(vlee);
                ex.Clear();
                throw vlee;
            }
        }

        public virtual bool MoveNext()
        {
            VlcException ex = new VlcException();
            if(has_next(ex))
            {
                int size = Marshal.SizeOf(typeof(VlcLogMessagePtr));

                //libvlc wants us to allocate the buffer and set the size
                //so it knows that the structs match in layout and size on
                //runtime arch
                VlcLogMessagePtr tmp = new VlcLogMessagePtr();
                tmp.message_size = (uint)size;

                //take our existing structure and put it in the unmanaged memory
                //also let it deallocate the original
                VlcLogMessageHandle ptr = new VlcLogMessageHandle();
                Marshal.StructureToPtr(tmp, ptr, true);

                last_ptr = libvlc_log_iterator_next(iter, ptr, ex);

                if(!ex.Raised)
                    return true;
            
                handle_exception(ex);
                return false;
            }
            return false;
        }

        public virtual void Reset()
        {
            VlcException ex = new VlcException();
            free(ex);
            get_iter(ex);
        }

        public virtual Object Current
        {
            get
            {
                if(!last_ptr.IsInvalid)
                {
                    VlcLogMessage message = new VlcLogMessage(last_ptr);
                    return message;
                }
                return null;
            }
        }

        [DllImport("libvlc")]
        private static extern VlcLogEnumHandle libvlc_log_get_iterator(VlcLogHandle log, VlcExceptionHandle ex);

        [DllImport("libvlc")]
        private static extern void libvlc_log_iterator_free(VlcLogEnumHandle iter, VlcExceptionHandle ex);

        [DllImport("libvlc")]
        private static extern int libvlc_log_iterator_has_next(VlcLogEnumHandle iter, VlcExceptionHandle ex);

        [DllImport("libvlc")]
        private static extern VlcLogMessageHandle libvlc_log_iterator_next(VlcLogEnumHandle iter, VlcLogMessageHandle buffer, VlcExceptionHandle ex);
    }
}
