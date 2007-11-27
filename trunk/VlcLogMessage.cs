/* vi:set ts=4 et sts=4 sw=4: */
/***************************************************************************
 *  VlcLogMessage.cs
 *  Original Filename: VlcLogMessage.cs
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

namespace libvlc
{
    public class VlcLogMessageHandle : SafeHandle
    {
        public VlcLogMessageHandle() : base(IntPtr.Zero, true)
        {
            handle = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(VlcLogMessagePtr)));
        }

        public override bool IsInvalid
        {
            get { return handle == IntPtr.Zero; }
        }

        protected override bool ReleaseHandle()
        {
            if(!IsInvalid)
            {
                Marshal.FreeCoTaskMem(handle);
                handle = IntPtr.Zero;
                return true;
            }
            return false;
        }

        protected override void Dispose(bool disposing)
        {
            ReleaseHandle();
            base.Dispose(disposing);
        }

        public static implicit operator IntPtr(VlcLogMessageHandle vlmh)
        {
            return vlmh.handle;
        }
    }

    public class VlcLogMessage
    {
        private VlcLogMessagePtr ptr;
        private VlcLogMessageHandle rptr;

        private string type;
        private string name;
        private string header;
        private string message;

        public VlcLogMessage(VlcLogMessageHandle handle)
        {
            rptr = handle;
            ptr = (VlcLogMessagePtr)Marshal.PtrToStructure(rptr, typeof(VlcLogMessagePtr));
        }

        public string Type
        {
            get
            {
                if(type == null)
                {
                    if(ptr.type != IntPtr.Zero)
                        type = Marshal.PtrToStringAnsi(ptr.type);
                    else
                        type = string.Empty;
                }
                return type;
            }
        }

        public string Name
        {
            get
            {
                if(name == null)
                {
                    if(ptr.name != IntPtr.Zero)
                        name = Marshal.PtrToStringAnsi(ptr.name);
                    else
                        name = string.Empty;
                }
                return name;
            }
        }

        public string Header
        {
            get
            {
                if(type == null)
                {
                    if(ptr.header != IntPtr.Zero)
                        header = Marshal.PtrToStringAnsi(ptr.header);
                    else
                        header = string.Empty;
                }
                return header;
            }
        }

        public string Message
        {
            get
            {
                if(message == null)
                {
                    if(ptr.message != IntPtr.Zero)
                        message = Marshal.PtrToStringAnsi(ptr.message);
                    else
                        message = string.Empty;
                }
                return message;
            }
        }

        public int Severity
        {
            get { return ptr.severity; }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct VlcLogMessagePtr
    {
        public uint message_size;
        public int severity;
        public IntPtr type;
        public IntPtr name;
        public IntPtr header;
        public IntPtr message;
    }
}
