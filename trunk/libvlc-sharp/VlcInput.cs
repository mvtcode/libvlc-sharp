/* vi:set ts=4 et sts=4 sw=4: */
/***************************************************************************
 *  VlcInput.cs
 *  Original Filename: VlcInput.cs
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
    public class VlcInputHandle : SafeHandle
    {
        private bool _disposed = false;

        public VlcInputHandle() : base(IntPtr.Zero, true)
        { }

        public override bool IsInvalid
        {
            get { return handle == IntPtr.Zero; }
        }

        protected override bool ReleaseHandle()
        {
            if (!IsInvalid && !_disposed)
            {
                _disposed = true;
                libvlc_input_free(this);
            }
            return true;
        }

        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                ReleaseHandle();
            }
            base.Dispose(disposing);
        }

        [DllImport("libvlc")]
        private static extern void libvlc_input_free(VlcInputHandle input);
    }

    public class VlcInput : IDisposable
    {
        private bool _disposed = false;
        private VlcInputHandle _input;
        private VlcException _excp = new VlcException();

        internal VlcInput(VlcInstanceHandle _inst)
        {
            _input = libvlc_playlist_get_input(_inst, _excp);
        }

        ~VlcInput()
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
                if (!_input.IsInvalid)
                    _input.Dispose();
            }
        }

        public bool IsInvalid
        {
            get
            {
                return _input.IsInvalid;
            }
        }

        public Int64 Time
        {
            get
            {
                Int64 ret = libvlc_input_get_time(_input, _excp);
                VlcException.HandleVlcException(ref _excp);
                return ret;
            }

            set
            {
                libvlc_input_set_time(_input, value, _excp);
                VlcException.HandleVlcException(ref _excp);
            }
        }

        public float Position
        {
            get
            {
                float ret = libvlc_input_get_position(_input, _excp);
                VlcException.HandleVlcException(ref _excp);
                return ret;
            }
            set
            {
                libvlc_input_set_position(_input, value, _excp);
                VlcException.HandleVlcException(ref _excp);
            }
        }

        public Int64 Length
        {
            get
            {
                Int64 ret = libvlc_input_get_length(_input, _excp);
                VlcException.HandleVlcException(ref _excp);
                return ret;
            }
        }

        public float Rate
        {
            get
            {
                float i = libvlc_input_get_rate(_input, _excp);
                VlcException.HandleVlcException(ref _excp);
                return i;
            }

            set
            {
                libvlc_input_set_rate(_input, value, _excp);
                VlcException.HandleVlcException(ref _excp);
            }
        }

        public bool WillPlay
        {
            get
            {
                bool b = libvlc_input_will_play(_input, _excp);
                VlcException.HandleVlcException(ref _excp);
                return b;
            }
        }

        public Int32 State
        {
            get
            {
                Int32 i = libvlc_input_get_state(_input, _excp);
                VlcException.HandleVlcException(ref _excp);
                return i;
            }
        }

        public bool HasVout
        {
            get
            {
                bool b = libvlc_input_has_vout(_input, _excp);
                VlcException.HandleVlcException(ref _excp);
                return b;
            }
        }

        public float FPS
        {
            get
            {
                float i = libvlc_input_get_fps(_input, _excp);
                VlcException.HandleVlcException(ref _excp);
                return i;
            }
        }

        public bool VideoFullScreen
        {
            get
            {
                bool b = libvlc_get_fullscreen(_input, _excp);
                VlcException.HandleVlcException(ref _excp);
                return b;
            }
			
			set
			{
				libvlc_set_fullscreen(_input, value, _excp);
				VlcException.HandleVlcException(ref _excp);
			}
        }

        public Int32 VideoHeight
        {
            get
            {
                Int32 i = libvlc_video_get_height(_input, _excp);
                VlcException.HandleVlcException(ref _excp);
                return i;
            }
        }

        public Int32 VideoWidth
        {
            get
            {
                Int32 i = libvlc_video_get_width(_input, _excp);
                VlcException.HandleVlcException(ref _excp);
                return i;
            }
        }

        public string VideoAspectRatio
        {
            get
            {
                string s = libvlc_video_get_aspect_ratio(_input, _excp);
                VlcException.HandleVlcException(ref _excp);
                return s;
            }

            set
            {
                libvlc_video_set_aspect_ratio(_input, value, _excp);
                VlcException.HandleVlcException(ref _excp);
            }
        }

        public void VideoToggleFullScreen()
        {
            libvlc_toggle_fullscreen(_input, _excp);
            VlcException.HandleVlcException(ref _excp);
        }

        public void TakeSnapshot(string fileName)
        {
            libvlc_video_take_snapshot(_input, fileName, _excp);
            VlcException.HandleVlcException(ref _excp);
        }

        #region libvlc api
        [DllImport("libvlc")]
        private static extern VlcInputHandle libvlc_playlist_get_input(VlcInstanceHandle engine, VlcExceptionHandle ex);

        [DllImport("libvlc")]
        private static extern int libvlc_video_reparent(VlcInputHandle p_input, IntPtr hwnd, VlcExceptionHandle p_exception);

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

        [DllImport("libvlc")]
        private static extern float libvlc_input_get_rate(VlcInputHandle p_input, VlcExceptionHandle p_exception);

        [DllImport("libvlc")]
        private static extern void libvlc_input_set_rate(VlcInputHandle p_input, float f, VlcExceptionHandle p_exception);

        [DllImport("libvlc")]
        private static extern Int32 libvlc_input_get_state(VlcInputHandle p_input, VlcExceptionHandle p_exception);

        [DllImport("libvlc")]
        private static extern bool libvlc_input_will_play(VlcInputHandle p_input, VlcExceptionHandle p_exception);

        [DllImport("libvlc")]
        private static extern bool libvlc_input_has_vout(VlcInputHandle p_input, VlcExceptionHandle p_exception);

        [DllImport("libvlc")]
        private static extern float libvlc_input_get_fps(VlcInputHandle p_input, VlcExceptionHandle p_exception);

        [DllImport("libvlc")]
        private static extern void libvlc_set_fullscreen(VlcInputHandle p_input, bool b, VlcExceptionHandle p_exception);

        [DllImport("libvlc")]
        private static extern bool libvlc_get_fullscreen(VlcInputHandle p_input, VlcExceptionHandle p_exception);

        [DllImport("libvlc")]
        private static extern int libvlc_video_get_height(VlcInputHandle p_input, VlcExceptionHandle p_exception);

        [DllImport("libvlc")]
        private static extern int libvlc_video_get_width(VlcInputHandle p_input, VlcExceptionHandle p_exception);

        [DllImport("libvlc")]
        private static extern string libvlc_video_get_aspect_ratio(VlcInputHandle p_input, VlcExceptionHandle p_exception);

        [DllImport("libvlc")]
        private static extern void libvlc_video_set_aspect_ratio(
            VlcInputHandle p_input,
            [MarshalAs(UnmanagedType.LPStr)] string aspect_ratio,
            VlcExceptionHandle p_exception);

        [DllImport("libvlc")]
        private static extern void libvlc_toggle_fullscreen(VlcInputHandle p_input, VlcExceptionHandle p_exception);

        [DllImport("libvlc")]
        private static extern void libvlc_video_take_snapshot(
            VlcInputHandle p_input,
            [MarshalAs(UnmanagedType.LPStr)] 
            string file_name,
            VlcExceptionHandle p_exception);
        #endregion

    }
}
