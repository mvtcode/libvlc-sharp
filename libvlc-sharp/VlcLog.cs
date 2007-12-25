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

namespace Atx.LibVLC
{
    public class VlcLogHandle : SafeHandle
    {
        VlcException _excp = new VlcException();

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
                libvlc_log_close(this, _excp);
                if(!_excp.Raised)
                {
                    handle = IntPtr.Zero;
                }
                else
                {
                    Console.WriteLine("Failed to Release VlcLogHandle: " + _excp.Message);
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

    public class VlcLog : ICollection, IDisposable
    {
        private bool _disposed = false;
        private VlcException _excp = new VlcException();
        private VlcInstanceHandle _instance;
        private VlcLogHandle _log;
        
        internal VlcLog(VlcInstanceHandle instance)
        {
            _instance = instance;
            _log = libvlc_log_open(_instance, _excp);
            VlcException.HandleVlcException(ref _excp);
        }

        ~VlcLog()
        {
            Dispose(false);
        }

        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _disposed = true;
                if ( !_log.IsInvalid )
                    _log.Dispose();
                _excp.Dispose();
            }
        }
        public uint Verbosity
        {
            get
            {
                uint ret;
                lock (SyncRoot)
                {
                    ret = libvlc_get_log_verbosity(_instance, _excp);
                    VlcException.HandleVlcException(ref _excp);
                }
                return ret;
            }

            set
            {
                lock (SyncRoot)
                {
                    libvlc_set_log_verbosity(_instance, value, _excp);
                    VlcException.HandleVlcException(ref _excp);
                }
            }
        }

        public virtual void CopyTo(Array array, int index)
        {
        }

        public virtual int Count
        {
            get
            {
                uint ret;
                lock (SyncRoot)
                {
                    ret = libvlc_log_count(_log, _excp);
                    VlcException.HandleVlcException(ref _excp);
                }
                return (int)ret;
            }
        }

        private object _synchObject = new object();
        public virtual object SyncRoot
        {
            get { return _synchObject; }
        }

        public virtual bool IsSynchronized
        {
            get { return  true; }
        }

        public virtual IEnumerator GetEnumerator()
        {
            return new VlcLogEnum(_log);
        }

        public void Clear()
        {
            lock (SyncRoot)
            {
                if (Count > 0)
                {
                    libvlc_log_clear(_log, _excp);
                    VlcException.HandleVlcException(ref _excp);
                }
            }
        }

        #region libvlc api
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
        #endregion
    }
}
