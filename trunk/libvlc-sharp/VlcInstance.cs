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
                libvlc_destroy(this);
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
        private static extern void libvlc_destroy(VlcInstanceHandle engine);
    }

    public class VlcInstance : IDisposable
    {
        private bool _disposed = false;
        private VlcException _excp = new VlcException();
        private VlcInstanceHandle _instance;

        public VlcInstance(VlcConfig cfg)
        {
            _instance = libvlc_new(cfg.Arguments.Length, cfg.Arguments, _excp);
            VlcException.HandleVlcException(ref _excp);
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

                if (!_instance.IsInvalid)
                    _instance.Dispose();

                if ( _excp != null )
                    _excp.Dispose();
            }
        }

        ~VlcInstance()
        {
            Dispose(false);
        }

        private IntPtr _owner;
        public IntPtr Owner
        {
            get
            {
                return _owner;
            }

            set
            {
                SetOwner(value);
                _owner = value;
            }
        }

        private VlcLog _log;
        public VlcLog Log
        {
            get
            {
                if(_log == null)
                    _log = new VlcLog(_instance);

                return _log;
            }
        }

        public Int32 ID
        {
            get
            {
                Int32 id = libvlc_get_vlc_id(_instance);
                return id;
            }
        }

        public VlcObject Object
        {
            get
            {
                VlcObject o = new VlcObject(ID);
                return o;
            }
        }

        public VlcInput Input
        {
            get
            {
                VlcInput input = new VlcInput(_instance);
                return input;
            }
        }

        public static string Version
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
                int ret = libvlc_audio_get_volume(_instance, _excp);
                VlcException.HandleVlcException(ref _excp);
                return ret;
            }

            set
            {
                libvlc_audio_set_volume(_instance, value, _excp);
                VlcException.HandleVlcException(ref _excp);
            }
        }

        public bool Mute
        {
            get
            {
                bool b = libvlc_audio_get_mute(_instance, _excp);
                VlcException.HandleVlcException(ref _excp);
                return b;
            }

            set
            {
                libvlc_audio_set_mute(_instance, value, _excp);
                VlcException.HandleVlcException(ref _excp);
            }
        }

        public bool IsPlaying
        {
            get
            {
                int ret = libvlc_playlist_isplaying(_instance, _excp);
                VlcException.HandleVlcException(ref _excp);
                return ret == 1 ? true : false;
            }
        }

        public int Count
        {
            get
            {
                int ret = libvlc_playlist_items_count(_instance, _excp);
                VlcException.HandleVlcException(ref _excp);
                return ret;
            }
        }

        public void Play()
        {
            Play(-1);
        }

        public void Play(Int32 item)
        {
            SetOwner(Owner);
            libvlc_playlist_play(_instance, item, 0, null, _excp);
            VlcException.HandleVlcException(ref _excp);
        }

        public void Pause()
        {
            libvlc_playlist_pause(_instance, _excp);
            VlcException.HandleVlcException(ref _excp);
        }

        public void Stop()
        {
            libvlc_playlist_stop(_instance, _excp);
            VlcException.HandleVlcException(ref _excp);
        }

        public void Prev()
        {
            libvlc_playlist_prev(_instance, _excp);
            VlcException.HandleVlcException(ref _excp);
        }

        public void Next()
        {
            libvlc_playlist_next(_instance, _excp);
            VlcException.HandleVlcException(ref _excp);
        }

        public void PlaylistClear()
        {
            libvlc_playlist_clear(_instance, _excp);
            VlcException.HandleVlcException(ref _excp);
        }

        public int PlaylistAdd(string uri)
        {
            int ret = libvlc_playlist_add(_instance, uri, null, _excp);
            VlcException.HandleVlcException(ref _excp);
            return ret;
        }

        public int PlaylistAdd(string uri, string name)
        {
            int ret = libvlc_playlist_add(_instance, uri, name, _excp);
            VlcException.HandleVlcException(ref _excp);
            return ret;
        }

        public int PlaylistDeleteItem(int item)
        {
            int ret = libvlc_playlist_delete_item(_instance, item, _excp);
            VlcException.HandleVlcException(ref _excp);
            return ret;
        }

        public void ToggleMute()
        {
            libvlc_audio_toggle_mute(_instance, _excp);
            VlcException.HandleVlcException(ref _excp);
        }

        protected void SetOwner(IntPtr hwnd)
        {
            if (!_instance.IsInvalid)
            {
                if (!Input.IsInvalid)
                    Input.Owner = hwnd;
                else
                {
                    libvlc_video_set_parent(_instance, hwnd, _excp);
                    VlcException.HandleVlcException(ref _excp);
                }
            }
        }

        #region libvlc api
        [DllImport ("libvlc")]
        private  static extern VlcInstanceHandle libvlc_new(int argc, string[] args, VlcExceptionHandle ex);

        [DllImport("libvlc")]
        private static extern Int32 libvlc_get_vlc_id(VlcInstanceHandle p_instance);

        [DllImport ("libvlc")]
        private static extern void libvlc_playlist_play(VlcInstanceHandle p_instance, Int32 i, Int32 i2, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr)] string[] a, VlcExceptionHandle _excp);

        [DllImport ("libvlc")]
        private static extern void libvlc_playlist_pause(VlcInstanceHandle engine, VlcExceptionHandle ex);

        [DllImport ("libvlc")]
        private static extern int libvlc_playlist_isplaying(VlcInstanceHandle engine, VlcExceptionHandle ex);

        [DllImport ("libvlc")]
        private static extern int libvlc_playlist_items_count(VlcInstanceHandle engine, VlcExceptionHandle ex);

        [DllImport ("libvlc")]
        private static extern void libvlc_playlist_stop(VlcInstanceHandle engine, VlcExceptionHandle ex);

        [DllImport ("libvlc")]
        private static extern void libvlc_playlist_next(VlcInstanceHandle engine, VlcExceptionHandle ex);

        [DllImport ("libvlc")]
        private static extern void libvlc_playlist_prev(VlcInstanceHandle engine, VlcExceptionHandle ex);

        [DllImport ("libvlc")]
        private static extern void libvlc_playlist_clear(VlcInstanceHandle engine, VlcExceptionHandle ex);

        [DllImport ("libvlc")]
        private static extern int libvlc_playlist_add(VlcInstanceHandle engine, string uri, string name, VlcExceptionHandle ex);

        [DllImport ("libvlc")]
        private static extern int libvlc_playlist_delete_item(VlcInstanceHandle engine, int item, VlcExceptionHandle ex);

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

