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

namespace Atx.LibVLC
{
    public class VlcLogMessage
    {
        private libvlc_log_message_t _msg;
        private string type;
        private string name;
        private string header;
        private string message;

        internal VlcLogMessage(libvlc_log_message_t msg)
        {
            _msg = msg;
        }

        public string Type
        {
            get
            {
                if(type == null)
                {
                    if(_msg.type != IntPtr.Zero)
                        type = Marshal.PtrToStringAnsi(_msg.type);
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
                    if(_msg.name != IntPtr.Zero)
                        name = Marshal.PtrToStringAnsi(_msg.name);
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
                    if(_msg.header != IntPtr.Zero)
                        header = Marshal.PtrToStringAnsi(_msg.header);
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
                    if(_msg.message != IntPtr.Zero)
                        message = Marshal.PtrToStringAnsi(_msg.message);
                    else
                        message = string.Empty;
                }
                return message;
            }
        }

        public int Severity
        {
            get { return _msg.severity; }
        }
    }
}
