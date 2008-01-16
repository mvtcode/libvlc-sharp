/***************************************************************************
 *  VlcPlaylist.cs
 *  Original Filename: VlcPlaylist.cs
 *  
 *  Copyright (C) 2007 Scott E Graves <scott.e.graves@gmail.com>
 *  Written by Scott E Graves <scott.e.graves@gmail.com>
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
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace Atx.LibVLC
{
    public class VlcPlaylist
    {
        private VlcException _excp = new VlcException();

        internal VlcPlaylist(VlcInstance vlcInstance)
        {
            VlcInstance = vlcInstance;
        }

        private VlcInstance _vlcInstance;
        private VlcInstance VlcInstance
        {
            get
            {
                return _vlcInstance;
            }

            set
            {
                _vlcInstance = value;
            }
        }

        public bool IsPlaying
        {
            get
            {
                int ret = libvlc_playlist_isplaying(VlcInstance.VlcInstanceHandle, _excp);
                VlcException.HandleVlcException(ref _excp);
                return ret == 1 ? true : false;
            }
        }

        public int Count
        {
            get
            {
                int ret = libvlc_playlist_items_count(VlcInstance.VlcInstanceHandle, _excp);
                VlcException.HandleVlcException(ref _excp);
                return ret;
            }
        }

        public int CurrentItem
        {
            get
            {
                return VLC_PlaylistIndex(VlcInstance.VlcObjectID);
            }

            set
            {
                Play(value);
            }
        }

        public void Play()
        {
            Play(-1);
        }

        public void Play(Int32 item)
        {
            libvlc_playlist_play(VlcInstance.VlcInstanceHandle, item, 0, null, _excp);
            VlcException.HandleVlcException(ref _excp);
        }

        public void Pause()
        {
            libvlc_playlist_pause(VlcInstance.VlcInstanceHandle, _excp);
            VlcException.HandleVlcException(ref _excp);
        }

        public void Stop()
        {
            libvlc_playlist_stop(VlcInstance.VlcInstanceHandle, _excp);
            VlcException.HandleVlcException(ref _excp);
        }

        public void Prev()
        {
            libvlc_playlist_prev(VlcInstance.VlcInstanceHandle, _excp);
            VlcException.HandleVlcException(ref _excp);
        }

        public void Next()
        {
            libvlc_playlist_next(VlcInstance.VlcInstanceHandle, _excp);
            VlcException.HandleVlcException(ref _excp);
        }

        public void Clear()
        {
            libvlc_playlist_clear(VlcInstance.VlcInstanceHandle, _excp);
            VlcException.HandleVlcException(ref _excp);
        }

        public int Add(string uri)
        {
            int ret = libvlc_playlist_add(VlcInstance.VlcInstanceHandle, uri, null, _excp);
            VlcException.HandleVlcException(ref _excp);
            return ret;
        }

        public int Add(string uri, string name)
        {
            int ret = libvlc_playlist_add(VlcInstance.VlcInstanceHandle, uri, name, _excp);
            VlcException.HandleVlcException(ref _excp);
            return ret;
        }

        public int DeleteItem(int item)
        {
            int ret = libvlc_playlist_delete_item(VlcInstance.VlcInstanceHandle, item, _excp);
            VlcException.HandleVlcException(ref _excp);
            return ret;
        }

        #region libvlc api
        [DllImport("libvlc")]
        private static extern void libvlc_playlist_play(VlcInstanceHandle p_instance, Int32 i, Int32 i2, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.LPStr)] string[] a, VlcExceptionHandle _excp);

        [DllImport("libvlc")]
        private static extern void libvlc_playlist_pause(VlcInstanceHandle engine, VlcExceptionHandle ex);

        [DllImport("libvlc")]
        private static extern int libvlc_playlist_isplaying(VlcInstanceHandle engine, VlcExceptionHandle ex);

        [DllImport("libvlc")]
        private static extern int libvlc_playlist_items_count(VlcInstanceHandle engine, VlcExceptionHandle ex);

        [DllImport("libvlc")]
        private static extern void libvlc_playlist_stop(VlcInstanceHandle engine, VlcExceptionHandle ex);

        [DllImport("libvlc")]
        private static extern void libvlc_playlist_next(VlcInstanceHandle engine, VlcExceptionHandle ex);

        [DllImport("libvlc")]
        private static extern void libvlc_playlist_prev(VlcInstanceHandle engine, VlcExceptionHandle ex);

        [DllImport("libvlc")]
        private static extern void libvlc_playlist_clear(VlcInstanceHandle engine, VlcExceptionHandle ex);

        [DllImport("libvlc")]
        private static extern int libvlc_playlist_add(VlcInstanceHandle engine, string uri, string name, VlcExceptionHandle ex);

        [DllImport("libvlc")]
        private static extern int libvlc_playlist_delete_item(VlcInstanceHandle engine, int item, VlcExceptionHandle ex);

        [DllImport("libvlc")]
        private static extern int VLC_PlaylistIndex(int i_object);

        [DllImport("libvlc")]
        private static extern int VLC_PlaylistNumberOfItems(int i_object);

        #endregion
    }
}
