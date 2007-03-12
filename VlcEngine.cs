/* vi:set ts=4 et sts=4 sw=4: */
/***************************************************************************
 *  VlcEngine.cs
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
using System.IO;

namespace Atx.LibVLC
{
    public class VlcEngineException : Exception
    {
        public VlcEngineException(string m) : base(m) {}
    }

    public class VlcNoActiveInputException : VlcEngineException
    {
        public VlcNoActiveInputException(string m) : base(m) {}
    }

    internal class VlcEngineHandle : SafeHandle
    {
        public VlcEngineHandle() : base(IntPtr.Zero, true)
        { }

        public override bool IsInvalid
        {
            get { return handle == IntPtr.Zero; }
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
        private static extern void libvlc_destroy(VlcEngineHandle engine);
    }

    public class VlcEngine
    {
        internal VlcEngineHandle engine;
        
        private VlcLog vlclog;
        private string version;
        
        public VlcEngine ()
        {
            VlcException ex = new VlcException();
            string[] argv = new string[] { "-l", "dummy" };
            engine = libvlc_new (argv.Length, argv, ex);
            handle_exception(ex);
        }

        private void handle_exception(VlcException ex)
        {
            if(ex.Raised)
            {
                VlcEngineException vlcex;
                switch(ex.Message)
                {
                    case "No active input":
                        vlcex = new VlcNoActiveInputException(ex.Message) as VlcEngineException;
                        break;
                    default:
                        vlcex = new VlcEngineException(ex.Message);
                        Console.WriteLine(vlcex);
                        break;
                }
                ex.Clear();
                throw vlcex;
            }
        }

        public VlcLog Log
        {
            get
            {
                if(vlclog == null)
                    vlclog = new VlcLog(this);

                return vlclog;
            }
        }

        public string Version
        {
            get
            {
                if(version == null)
                {
                    IntPtr iversion = VLC_Version();
                    version = Marshal.PtrToStringAnsi(iversion);
                }
                return version;
            }
        }

        public int Volume
        {
            get
            {
                int ret = 0;
                VlcException ex = new VlcException();
                ret = libvlc_audio_get_volume(engine, ex);
                handle_exception(ex);
                return ret;
            }

            set
            {
                VlcException ex = new VlcException();
                libvlc_audio_set_volume(engine, value, ex);
                handle_exception(ex);
            }
        }

        public Int64 Time
        {
            get
            {
                try
                {
                    VlcException ex = new VlcException();
                    IntPtr input = libvlc_playlist_get_input(engine, ex);
                    handle_exception(ex);
    
                    Int64 ret = 0;
                    if(input != IntPtr.Zero)
                    {
                        ret = libvlc_input_get_time(input, ex);
                        handle_exception(ex);
                        libvlc_input_free(input);
                    }
                    return ret;
                }
                catch(VlcNoActiveInputException)
                {
                    return 0;
                }
            }

            set
            {
                VlcException ex = new VlcException();
                IntPtr input = libvlc_playlist_get_input(engine, ex);
                handle_exception(ex);
                libvlc_input_set_time(input, value, ex);
                handle_exception(ex);
                libvlc_input_free(input);
            }
        }

        public float Position
        {
            get
            {
                try
                {
                    VlcException ex = new VlcException();
                    IntPtr input = libvlc_playlist_get_input(engine, ex);
                    handle_exception(ex);

                    float ret = 0;
                    if (input != IntPtr.Zero)
                    {
                        ret = libvlc_input_get_position(input, ex);
                        handle_exception(ex);
                        libvlc_input_free(input);
                    }
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
                IntPtr input = libvlc_playlist_get_input(engine, ex);
                handle_exception(ex);
                libvlc_input_set_position(input, value, ex);
                handle_exception(ex);
                libvlc_input_free(input);
            }
        }

        public Int64 Length
        {
            get
            {
                try
                {
                    VlcException ex = new VlcException();
                    IntPtr input = libvlc_playlist_get_input(engine, ex);
                    handle_exception(ex);
                   
                    Int64 ret = 0; 
                    if(input != IntPtr.Zero)
                    {
                        ret = libvlc_input_get_length(input, ex);
                        handle_exception(ex);
                        libvlc_input_free(input);
                    }
                    return ret;
                }
                catch(VlcNoActiveInputException)
                {
                    return 0;
                }
            }
        }

        public bool IsPlaying
        {
            get
            {
                VlcException ex = new VlcException();
                int ret = libvlc_playlist_isplaying(engine, ex);
                handle_exception(ex);
                return ret == 1 ? true : false;
            }
        }

        public int Count
        {
            get
            {
                VlcException ex = new VlcException();
                int ret = libvlc_playlist_items_count(engine, ex);
                handle_exception(ex);
                return ret;
            }
        }

        public void Play(int item)
        {
            VlcException ex = new VlcException();
            libvlc_playlist_play(engine, item, 0, IntPtr.Zero, ex);
            handle_exception(ex);
        }

        public void Pause()
        {
            VlcException ex = new VlcException();
            libvlc_playlist_pause(engine, ex);
            handle_exception(ex);
        }

        public void Stop()
        {
            VlcException ex = new VlcException();
            libvlc_playlist_stop(engine, ex);
            handle_exception(ex);
        }

        public void Prev()
        {
            VlcException ex = new VlcException();
            libvlc_playlist_prev(engine, ex);
            handle_exception(ex);
        }

        public void Next()
        {
            VlcException ex = new VlcException();
            libvlc_playlist_next(engine, ex);
            handle_exception(ex);
        }

        public void Clear()
        {
            VlcException ex = new VlcException();
            libvlc_playlist_clear(engine, ex);
            handle_exception(ex);
        }

        public int Add(string uri, string name)
        {
            int ret = -1;
            VlcException ex = new VlcException();
            ret = libvlc_playlist_add(engine, uri, name, ex);
            handle_exception(ex);
            return ret;
        }

        public int Delete(int item)
        {
            int ret = -1;
            VlcException ex = new VlcException();
            ret = libvlc_playlist_delete_item(engine, item, ex);
            handle_exception(ex);
            return ret;
        }

        [DllImport ("libvlc")]
        private  static extern VlcEngineHandle libvlc_new(int argc, string[] args, VlcExceptionHandle ex);
        
        [DllImport ("libvlc")]
        private static extern void libvlc_playlist_play(VlcEngineHandle engine, int item, int argc, IntPtr argv, VlcExceptionHandle ex);

        [DllImport ("libvlc")]
        private static extern void libvlc_playlist_pause(VlcEngineHandle engine, VlcExceptionHandle ex);

        [DllImport ("libvlc")]
        private static extern int libvlc_playlist_isplaying(VlcEngineHandle engine, VlcExceptionHandle ex);

        [DllImport ("libvlc")]
        private static extern int libvlc_playlist_items_count(VlcEngineHandle engine, VlcExceptionHandle ex);

        [DllImport ("libvlc")]
        private static extern void libvlc_playlist_stop(VlcEngineHandle engine, VlcExceptionHandle ex);

        [DllImport ("libvlc")]
        private static extern void libvlc_playlist_next(VlcEngineHandle engine, VlcExceptionHandle ex);

        [DllImport ("libvlc")]
        private static extern void libvlc_playlist_prev(VlcEngineHandle engine, VlcExceptionHandle ex);

        [DllImport ("libvlc")]
        private static extern void libvlc_playlist_clear(VlcEngineHandle engine, VlcExceptionHandle ex);

        [DllImport ("libvlc")]
        private static extern int libvlc_playlist_add(VlcEngineHandle engine, string uri, string name, VlcExceptionHandle ex);

        [DllImport ("libvlc")]
        private static extern int libvlc_playlist_delete_item(VlcEngineHandle engine, int item, VlcExceptionHandle ex);

        [DllImport ("libvlc")]
        private static extern IntPtr libvlc_playlist_get_input(VlcEngineHandle engine, VlcExceptionHandle ex);

        [DllImport ("libvlc")]
        private static extern void libvlc_input_free(IntPtr input);

        [DllImport ("libvlc")]
        private static extern Int64 libvlc_input_get_length(IntPtr input, VlcExceptionHandle exception);

        [DllImport ("libvlc")]
        private static extern Int64 libvlc_input_get_time(IntPtr input, VlcExceptionHandle exception);

        [DllImport ("libvlc")]
        private static extern void libvlc_input_set_time(IntPtr input, Int64 time, VlcExceptionHandle exception);

        [DllImport ("libvlc")]
        private static extern float libvlc_input_get_position(IntPtr input, VlcExceptionHandle exception);

        [DllImport ("libvlc")]
        private static extern void libvlc_input_set_position(IntPtr input, float pos, VlcExceptionHandle exception);

        [DllImport ("libvlc")]
        private static extern int libvlc_audio_get_volume(VlcEngineHandle engine, VlcExceptionHandle exception);

        [DllImport ("libvlc")]
        private static extern void libvlc_audio_set_volume(VlcEngineHandle engine, int volume, VlcExceptionHandle exception);

        [DllImport ("libvlc")]
        private static extern IntPtr VLC_Version();
    }
}

