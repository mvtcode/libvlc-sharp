/* vi:set ts=4 et sts=4 sw=4: */
/***************************************************************************
 *  VlcLog.cs
 *  Original Filename: VlcLog.cs
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
using System.Collections;
using System.Runtime.InteropServices;

namespace libvlc
{
    public class VlcLogException : Exception
    {
        public VlcLogException(string m) : base(m) {}
    }

    public class VlcLogHandle : SafeHandle
    {
        public VlcLogHandle() : base(IntPtr.Zero, true)
        {}

        public override bool IsInvalid
        {
            get { return handle == IntPtr.Zero; }
        }

        protected override bool ReleaseHandle()
        {
            if(!IsInvalid)
            {
                VlcException ex = new VlcException();
                libvlc_log_close(this, ex);
                if(!ex.Raised)
                {
                    handle = IntPtr.Zero;
                    return true;
                }
                else
                {
                    Console.WriteLine("Failed to Release VlcLogHandle: " + ex.Message);
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
        private static extern void libvlc_log_close(VlcLogHandle log, VlcExceptionHandle ex);
    }

    public class VlcLog : ICollection
    {
        private VlcInstanceHandle _instance;
        private VlcLogHandle _log;
        private VlcException _excp = new VlcException();

        internal VlcLog(VlcInstanceHandle instance)
        {
            _instance = instance;
            _log = libvlc_log_open(_instance, _excp);
            handle_exception();
        }

        private void handle_exception()
        {
            if (_excp.Raised)
            {
                VlcLogException vlex = new VlcLogException(_excp.Message);
                Console.WriteLine(vlex);
                throw vlex;
            }
        }

        public uint Verbosity
        {
            get
            {
                uint ret = libvlc_get_log_verbosity(_instance, _excp);
                handle_exception();
                return ret;
            }

            set
            {
                libvlc_set_log_verbosity(_instance, value, _excp);
                handle_exception();
            }
        }

        public virtual void CopyTo(Array array, int index)
        {
        }

        public virtual int Count
        {
            get
            {
                uint ret = libvlc_log_count(_log, _excp);
                handle_exception();
                return (int)ret;
            }
        }

        public virtual object SyncRoot
        {
            get { return this; }
        }

        public virtual bool IsSynchronized
        {
            get { return false; }
        }

        public virtual IEnumerator GetEnumerator()
        {
            return new VlcLogEnum(_log);
        }

        public void Clear()
        {
            if(Count > 0)
            {
                libvlc_log_clear(_log, _excp);
                handle_exception();
            }
        }

        [DllImport("libvlc")]
        private static extern VlcLogHandle libvlc_log_open(VlcInstanceHandle engine, VlcExceptionHandle ex);

        [DllImport("libvlc")]
        private static extern uint libvlc_get_log_verbosity(VlcInstanceHandle engine, VlcExceptionHandle ex);

        [DllImport("libvlc")]
        private static extern void libvlc_set_log_verbosity(VlcInstanceHandle engine, uint value, VlcExceptionHandle ex);
        
        [DllImport("libvlc")]
        private static extern uint libvlc_log_count(VlcLogHandle log, VlcExceptionHandle ex);

        [DllImport("libvlc")]
        private static extern void libvlc_log_clear(VlcLogHandle log, VlcExceptionHandle ex);
    }
}
