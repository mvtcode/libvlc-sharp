/* vi:set ts=4 et sts=4 sw=4: */
/***************************************************************************
 *  VlcInstance.cs
 *  Original Filename: VlcEngine.cs
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
using System.Collections.Generic;
using System.IO;

namespace Atx.LibVLC
{
    internal class VlcInstanceHandle : SafeHandle
    {
        public VlcInstanceHandle() : base(IntPtr.Zero, true)
        { }

        public override bool IsInvalid
        {
            get
            {
                return handle == IntPtr.Zero; 
            }
        }

        protected override bool ReleaseHandle()
        {
            if(!IsInvalid)
            {
                libvlc_destroy(handle);
                handle = IntPtr.Zero;
            }
            return true;
        }

        protected override void Dispose(bool disposing)
        {
            ReleaseHandle();
            base.Dispose(disposing);
        }

        [DllImport ("libvlc")]
        private static extern void libvlc_destroy(IntPtr engine);
    }

    public class VlcInstance : IDisposable
    {
        private bool _disposed = false;
        private VlcException _excp = new VlcException();

        public VlcInstance(VlcConfig vlcConfig)
        {
            VlcConfig = vlcConfig;

            VlcInstanceHandle = libvlc_new(VlcConfig.Arguments.Length, VlcConfig.Arguments, _excp);
            VlcException.HandleVlcException(ref _excp);
            
            VlcPlaylist = new VlcPlaylist(this);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _disposed = true;

                if (_log != null )
                    _log.Dispose();

                if (VlcObject != null)
                    VlcObject.Dispose();

                VlcInstanceHandle.Dispose();

                if ( _excp != null )
                    _excp.Dispose();
            }
        }

        ~VlcInstance()
        {
            Dispose(false);
        }

        private VlcInstanceHandle _vlcInstanceHandle;
        internal VlcInstanceHandle VlcInstanceHandle
        {
            get
            {
                return _vlcInstanceHandle;
            }

            set
            {
                _vlcInstanceHandle = value;
            }
        }
        
        public IntPtr Parent
        {
            get
            {
                IntPtr hwnd = (IntPtr)VlcObject.GetIntValue("drawable");
                return hwnd;
            }

            set
            {
                if (Parent != value)
                {
                    libvlc_video_set_parent(VlcInstanceHandle, value, _excp);
                    VlcException.HandleVlcException(ref _excp);
                }
            }
        }

        private VlcConfig _vlcConfig;
        public VlcConfig VlcConfig
        {
            get
            {
                return _vlcConfig;
            }

            private set
            {
                _vlcConfig = value;
            }
        }

        private VlcLog _log;
        public VlcLog VlcLog
        {
            get
            {
                if(_log == null)
                    _log = new VlcLog(VlcInstanceHandle);

                return _log;
            }
        }

        public Int32 VlcObjectID
        {
            get
            {
                Int32 id = libvlc_get_vlc_id(VlcInstanceHandle);
                return id;
            }
        }

        private VlcObject _object;
        public VlcObject VlcObject
        {
            get
            {
                if (_object == null)
                    _object = new VlcObject(VlcObjectID);

                return _object;
            }
        }

        public VlcInput VlcInput
        {
            get
            {
                VlcInput input = new VlcInput(VlcInstanceHandle);
                return input;
            }
        }

        private VlcPlaylist _vlcPlaylist;
        public VlcPlaylist VlcPlaylist
        {
            get
            {
                return _vlcPlaylist;
            }

            private set
            {
                _vlcPlaylist = value;
            }
        }

        public static string VlcLibraryVersion
        {
            get
            {
                String version = Marshal.PtrToStringAnsi(VLC_Version());
                return version;
            }
        }

        public int Volume
        {
            get
            {
                int ret = libvlc_audio_get_volume(VlcInstanceHandle, _excp);
                VlcException.HandleVlcException(ref _excp);
                return ret;
            }

            set
            {
                libvlc_audio_set_volume(VlcInstanceHandle, value, _excp);
                VlcException.HandleVlcException(ref _excp);
            }
        }

        public bool Mute
        {
            get
            {
                bool b = libvlc_audio_get_mute(VlcInstanceHandle, _excp);
                VlcException.HandleVlcException(ref _excp);
                return b;
            }

            set
            {
                libvlc_audio_set_mute(VlcInstanceHandle, value, _excp);
                VlcException.HandleVlcException(ref _excp);
            }
        }

        public void ToggleMute()
        {
            libvlc_audio_toggle_mute(VlcInstanceHandle, _excp);
            VlcException.HandleVlcException(ref _excp);
        }

        #region libvlc api
        [DllImport ("libvlc")]
        private  static extern VlcInstanceHandle libvlc_new(int argc, string[] args, VlcExceptionHandle ex);

        [DllImport("libvlc")]
        private static extern Int32 libvlc_get_vlc_id(VlcInstanceHandle p_instance);

        [DllImport ("libvlc")]
        private static extern int libvlc_audio_get_volume(VlcInstanceHandle engine, VlcExceptionHandle exception);

        [DllImport ("libvlc")]
        private static extern void libvlc_audio_set_volume(VlcInstanceHandle engine, int volume, VlcExceptionHandle exception);

        [DllImport("libvlc")]
        private static extern void libvlc_audio_toggle_mute(VlcInstanceHandle p_instance, VlcExceptionHandle p_exception);

        [DllImport("libvlc")]
        private static extern bool libvlc_audio_get_mute(VlcInstanceHandle p_instance, VlcExceptionHandle p_exception);

        [DllImport("libvlc")]
        private static extern void libvlc_audio_set_mute(VlcInstanceHandle p_instance, bool b , VlcExceptionHandle p_exception );

        [DllImport("libvlc")]
        private static extern void libvlc_video_set_parent(VlcInstanceHandle p_instance, IntPtr window, VlcExceptionHandle p_exception);

        [DllImport ("libvlc")]
        private static extern IntPtr VLC_Version();
        #endregion
    }
}

