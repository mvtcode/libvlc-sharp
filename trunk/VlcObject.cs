/***************************************************************************
 *  VlcObject.cs
 *  Original Filename: VlcObject.cs
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
using System.Runtime.InteropServices;


namespace Atx.LibVLC
{
    internal class VlcObjectHandle : SafeHandle
    {
        public VlcObjectHandle()
            : base(IntPtr.Zero, true)
        {
        }


        public override bool IsInvalid
        {
            get
            {
                return handle == IntPtr.Zero;
            }
        }


        protected override bool ReleaseHandle()
        {
            if (!IsInvalid)
            {
                __vlc_object_release(this);
                handle = IntPtr.Zero;
            }

            return true;
        }


        protected override void Dispose(bool disposing)
        {
            ReleaseHandle();
            base.Dispose(disposing);
        }

        [DllImport("libvlc")]
        private static extern void __vlc_object_release(VlcObjectHandle p_object);
    }

    public enum VlcObjectType : int
    {
        Root = -1,
        VLC = -2,
        Module = -3,
        Intf = -4,
        Playlist = -5,
        Item = -6,
        Input = -7,
        Decoder = -8,
        VOut = -9,
        AOut = -10,
        SOut = -11,
        HTTPD = -12,
        Packetizer = -13,
        Encoder = -14,
        Dialogs = -15,
        VLM = -16,
        Announce = -17,
        Demux = -18,
        Access = -19,
        Stream = -20,
        OpenGL = -21,
        Filter = -22,
        VOD = -23,
        SPU = -24,
        TLS = -25,
        SD = -26,
        XML = -27,
        OSDMenu = -28,
        Stats = -29,
        HTTPD_Host = -30,
        Generic = -666,
    }

    [Flags]
    public enum VlcObjectSearchMode : int
    {
        Parent = 1,
        Child = 2,
        Anywhere = 3,
        Strict = 4
    }

    internal enum VlcChangeAction : int
    {
		SetMin = 0x10,
        SetMax = 0x11,
        SetStep = 0x12,
        SetValue = 0x13,
        SetText = 0x14,
        GetText = 0x15,
        AddChoice = 0x20,
        DelChoice = 0x21,
        ClearChoices = 0x22,
        SetDefault = 0x23,
        GetChoices = 0x24,
        FreeChoices = 0x25,
        GetList = 0x26,
        FreeList = 0x27,
        ChoicesCount = 0x28,
        InheritValue = 0x30,
        TriggerCallbacks = 0x35
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct vlc_value_t
    {
        [FieldOffset(0)]
        public Int32 i_int;
        [FieldOffset(0)]
        public Int32 b_bool;
        [FieldOffset(0)]
        [MarshalAs(UnmanagedType.R4)]
        public float f_float;
        [FieldOffset(0)]
        public IntPtr psz_string;
        [FieldOffset(0)]
        public IntPtr p_address;
        [FieldOffset(0)]
        public IntPtr p_object;
        [FieldOffset(0)]
        public IntPtr p_list;
        [FieldOffset(0)]
        [MarshalAs(UnmanagedType.I8)]
        public Int64 i_time;
        [FieldOffset(0)]
        public IntPtr psz_name;
        [FieldOffset(4)]
        public Int32 i_object_id;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct vlc_list_t
    {
        public int i_count;
        public IntPtr p_values;
        public IntPtr pi_types;
    }

    public class VlcObject : IDisposable
    {
        private bool _disposed = false;

        private VlcObjectHandle _vlcObject;


        internal VlcObject(Int32 ID)
        {
            _vlcObject = vlc_current_object(ID);
        }


        private VlcObject(VlcObjectHandle p)
        {
            _vlcObject = p;
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
                if (!_vlcObject.IsInvalid)
                    _vlcObject.Dispose();
            }
        }


        ~VlcObject()
        {
            Dispose(false);
        }


        public VlcObject FindObject(VlcObjectType type, VlcObjectSearchMode mode)
        {
            if (_vlcObject.IsInvalid)
                throw new NullReferenceException("VLC object is NULL");

            VlcObject o = new VlcObject(__vlc_object_find(_vlcObject, type, mode));
            return o;
        }

		
		public void GetListChoices(string name, out List<string> choices)
		{
			if (_vlcObject.IsInvalid)
                throw new NullReferenceException("VLC object is NULL");

            choices = new List<string>();

			vlc_value_t a = new vlc_value_t();
            vlc_value_t b = new vlc_value_t();
			__var_Change(_vlcObject, name, VlcChangeAction.GetList, ref a, ref b );
		
            vlc_list_t t = (vlc_list_t)Marshal.PtrToStructure(b.p_list, typeof(vlc_list_t));
            for (int i = 0; i < t.i_count; i++)
            {
                IntPtr textPtr = new IntPtr(t.p_values.ToInt32() + i * Marshal.SizeOf(typeof(vlc_value_t)));
                vlc_value_t textValue = (vlc_value_t)Marshal.PtrToStructure(textPtr, typeof(vlc_value_t));
                choices.Add(Marshal.PtrToStringAnsi(textValue.psz_string));
            }

			__var_Change(_vlcObject, name, VlcChangeAction.FreeList, ref a, ref b );
		}


        public string GetStringValue(string name)
        {
            if (_vlcObject.IsInvalid)
                throw new NullReferenceException("VLC object is NULL");

			vlc_value_t v = new vlc_value_t();
            __var_Get(_vlcObject, name, ref v);
			
			return Marshal.PtrToStringAuto(v.psz_string);
        }


        public void SetValue(string name, string val)
        {
            if (_vlcObject.IsInvalid)
                throw new NullReferenceException("VLC object is NULL");

            //__var_Set(_vlcObject, name, ref var);
        }
       
        #region libvlc api
        [DllImport("libvlc")]
        private static extern VlcObjectHandle vlc_current_object(int i_object);

        [DllImport("libvlc")]
        private static extern VlcObjectHandle __vlc_object_find(VlcObjectHandle p_object, VlcObjectType objectType, VlcObjectSearchMode mode);

        [DllImport("libvlc")]
        private static extern Int32 __var_Get(VlcObjectHandle p_object, [MarshalAs(UnmanagedType.LPStr)] String name, ref vlc_value_t value);

        [DllImport("libvlc")]
        private static extern Int32 __var_Set(VlcObjectHandle p_object, [MarshalAs(UnmanagedType.LPStr)] String name, ref vlc_value_t value);
		
        [DllImport("libvlc")]
        private static extern Int32 __var_Change(
            VlcObjectHandle p_ojbect, 
            [MarshalAs(UnmanagedType.LPStr)] string name,
           VlcChangeAction action, 
            ref vlc_value_t valA, 
            ref vlc_value_t valB);
        #endregion
    }
}

