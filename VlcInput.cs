/* vi:set ts=4 et sts=4 sw=4: */
/***************************************************************************
 *  VlcInput.cs
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
using System.Runtime.InteropServices;

namespace Atx.LibVLC
{
    public class VlcInputHandle : SafeHandle
    {
        public VlcInputHandle() : base(IntPtr.Zero, true)
        { }

        public override bool IsInvalid
        {
            get { return handle == IntPtr.Zero; }
        }

        protected override bool ReleaseHandle()
        {
            if (!IsInvalid)
            {
                libvlc_input_free(this);
            }
            return true;
        }

        protected override void Dispose(bool disposing)
        {
            ReleaseHandle();
            base.Dispose(disposing);
        }

        [DllImport("libvlc")]
        private static extern void libvlc_input_free(VlcInputHandle input);
    }

    public class VlcInput
    {
        private VlcEngineHandle engine;

        public VlcInput(VlcEngine engine)
        {
            this.engine = engine.engine;
        }

        private void handle_exception(VlcException ex)
        {
            if (ex.Raised)
            {
                VlcEngineException vlcex;
                switch (ex.Message)
                {
                    case "No active input":
                        vlcex = new VlcNoActiveInputException(ex.Message) as VlcEngineException;
                        break;
                    default:
                        vlcex = new VlcEngineException(ex.Message);
                        break;
                }
                ex.Clear();
                throw vlcex;
            }
        }

        public Int64 Time
        {
            get
            {
                try
                {
                    VlcException ex = new VlcException();
                    VlcInputHandle input = libvlc_playlist_get_input(engine, ex);
                    handle_exception(ex);

                    Int64 ret = 0;
                    ret = libvlc_input_get_time(input, ex);
                    handle_exception(ex);
                    return ret;
                }
                catch (VlcNoActiveInputException)
                {
                    return 0;
                }
            }

            set
            {
                VlcException ex = new VlcException();
                VlcInputHandle input = libvlc_playlist_get_input(engine, ex);
                handle_exception(ex);
                libvlc_input_set_time(input, value, ex);
                handle_exception(ex);
            }
        }

        public float Position
        {
            get
            {
                try
                {
                    VlcException ex = new VlcException();
                    VlcInputHandle input = libvlc_playlist_get_input(engine, ex);
                    handle_exception(ex);

                    float ret = 0;
                    ret = libvlc_input_get_position(input, ex);
                    handle_exception(ex);
                    return ret;
                }
                catch (VlcNoActiveInputException)
                {
                    return 0;
                }
            }
            set
            {
                VlcException ex = new VlcException();
                VlcInputHandle input = libvlc_playlist_get_input(engine, ex);
                handle_exception(ex);
                libvlc_input_set_position(input, value, ex);
                handle_exception(ex);
            }
        }

        public Int64 Length
        {
            get
            {
                try
                {
                    VlcException ex = new VlcException();
                    VlcInputHandle input = libvlc_playlist_get_input(engine, ex);
                    handle_exception(ex);

                    Int64 ret = 0;
                    ret = libvlc_input_get_length(input, ex);
                    handle_exception(ex);
                    return ret;
                }
                catch (VlcNoActiveInputException)
                {
                    return 0;
                }
            }
        }

        [DllImport("libvlc")]
        private static extern VlcInputHandle libvlc_playlist_get_input(VlcEngineHandle engine, VlcExceptionHandle ex);

        [DllImport("libvlc")]
        private static extern Int64 libvlc_input_get_length(VlcInputHandle input, VlcExceptionHandle exception);

        [DllImport("libvlc")]
        private static extern Int64 libvlc_input_get_time(VlcInputHandle input, VlcExceptionHandle exception);

        [DllImport("libvlc")]
        private static extern void libvlc_input_set_time(VlcInputHandle input, Int64 time, VlcExceptionHandle exception);

        [DllImport("libvlc")]
        private static extern float libvlc_input_get_position(VlcInputHandle input, VlcExceptionHandle exception);

        [DllImport("libvlc")]
        private static extern void libvlc_input_set_position(VlcInputHandle input, float pos, VlcExceptionHandle exception);

    }
}
